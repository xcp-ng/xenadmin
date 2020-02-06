/* Copyright (c) Citrix Systems, Inc. 
 * All rights reserved. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using DiscUtils;
using DiscUtils.Wim;
using XenAdmin.Controls;
using XenCenterLib;
using XenCenterLib.Compression;
using XenAdmin.Dialogs;
using XenAdmin.Controls.Common;
using XenCenterLib.Archive;
using XenOvf;
using XenOvf.Definitions.VMC;
using XenOvf.Utilities;


namespace XenAdmin.Wizards.ImportWizard
{
	internal partial class ImportSourcePage : XenTabPage
	{
		#region Private fields
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string[] m_supportedImageTypes = { ".vhd", ".vmdk" };//CA-61385: remove ".vdi", ".wim" support for Boston
        private readonly string[] m_supportedApplianceTypes = { ".ovf", ".ova", ".ova.gz" };
        private readonly string[] m_supportedXvaTypes = {".xva", ".xva.gz", "ova.xml"};

		/// <summary>
		/// Stores the last valid selected appliance
		/// </summary>
		private string m_lastValidAppliance;
        private bool m_buttonNextEnabled;

        private string _unzipFileIn;
        private string _unzipFileOut;

        private Uri _uri;
        private string _downloadFolder;
        private WebClient _webClient;
        private Queue<ApplianceFile> _filesToDownload;

	    private bool longProcessInProgress;
		#endregion

		public ImportSourcePage()
		{
			InitializeComponent();
            m_ctrlError.HideError();
			m_tlpEncryption.Visible = false;
            m_tlpError.Visible = false;
            progressBar1.Visible = false;
            labelProgress.Visible = false;
		}

		#region Base class (XenTabPage) overrides

		/// <summary>
		/// Gets the page's title (headline)
		/// </summary>
		public override string PageTitle { get { return Messages.IMPORT_SOURCE_PAGE_TITLE; } }

		/// <summary>
		/// Gets the page's label in the (left hand side) wizard progress panel
		/// </summary>
		public override string Text { get { return Messages.IMPORT_SOURCE_PAGE_TEXT; } }

		/// <summary>
		/// Gets the value by which the help files section for this page is identified
		/// </summary>
		public override string HelpID { get { return "Source"; } }

        protected override bool ImplementsIsDirty()
        {
            return true;
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
		{
			if (direction == PageLoadedDirection.Forward && IsDirty)
			{
                if (IsUri() && !Download())
                {
                    cancel = true;
                    return;
                }

                if (!PerformCheck(CheckIsSupportedType, CheckPathExists))
				{
					cancel = true;
					return;
				}

                string extension = Path.GetExtension(FilePath).ToLower();

                if (extension == ".gz" && (!PerformCheck(CheckSourceIsWritable) || !Uncompress()))
                {
                    cancel = true;
                    return;
                }

                if (extension == ".ova" && !PerformCheck(CheckSourceIsWritable))
                {
                    cancel = true;
                    return;
                }

                CheckDelegate check = null;
				switch (TypeOfImport)
				{
					case ImportWizard.ImportType.Xva:
                        check = GetDiskCapacityXva;
						break;
					case ImportWizard.ImportType.Ovf:
                        check = LoadAppliance;
						break;
					case ImportWizard.ImportType.Vhd:
                        check = GetDiskCapacityImage;
						break;
				}

                if (!PerformCheck(check))
				{
					cancel = true;
					return;
				}

                if (TypeOfImport == ImportWizard.ImportType.Ovf && SelectedOvfPackage.HasEncryption())
				{
					cancel = true;
					m_tlpEncryption.Visible = true;
                    m_buttonNextEnabled = false;
                    OnPageUpdated();
				}
			}
		}

        public override void PageCancelled(ref bool cancel)
        {
            if (_webClient != null && _webClient.IsBusy)
                _webClient.CancelAsync();

            if (_unzipWorker.IsBusy)
                _unzipWorker.CancelAsync();
        }

        public override void PopulatePage()
		{
			lblIntro.Text = OvfModeOnly ? Messages.IMPORT_SOURCE_PAGE_INTRO_OVF_ONLY : Messages.IMPORT_SOURCE_PAGE_INTRO;
		}

        public override bool EnableNext()
        {
            return m_buttonNextEnabled;
        }

		#endregion

		#region Accessors

		public bool OvfModeOnly { private get; set; }

		public ImportWizard.ImportType TypeOfImport { get; private set; }

		public string FilePath => m_textBoxFile.Text.Trim();

        public Package SelectedOvfPackage { get; private set; }

		public ulong ImageLength { get; private set; }

		public bool IsWIM { get; private set; }

		public bool IsXvaVersion1 { get; private set; }

		public ulong DiskCapacity { get; private set; }

		#endregion

		public void SetFileName(string fileName)
		{
			m_textBoxFile.Text = fileName;
		}

		#region Private methods

        /// <summary>
        /// Performs certain checks on the page's input data and shows/hides an error accordingly
        /// </summary>
        /// <param name="checks">The checks to perform</param>
        private bool PerformCheck(params CheckDelegate[] checks)
        {
            m_buttonNextEnabled = m_ctrlError.PerformCheck(checks);
            OnPageUpdated();
            return m_buttonNextEnabled;
        }

		private bool GetDiskCapacityXva(out string error)
		{
			error = string.Empty;

			try
			{
                FileInfo info = new FileInfo(FilePath);
				ImageLength = info.Length > 0 ? (ulong)info.Length : 0;

				DiskCapacity = IsXvaVersion1
				               	? GetTotalSizeFromXmlGeneva() //Geneva style
				               	: GetTotalSizeFromXmlXva(GetXmlStringFromTarXVA()); //xva style
			}
			catch (Exception)
			{
				DiskCapacity = ImageLength;
			}
			return true;
		}

		private ulong GetTotalSizeFromXmlGeneva()
		{
			ulong totalSize = 0;
			XmlDocument xmlMetadata = new XmlDocument();
            xmlMetadata.Load(FilePath);
			XPathNavigator nav = xmlMetadata.CreateNavigator();
			XPathNodeIterator nodeIterator = nav.Select(".//vdi");

			while (nodeIterator.MoveNext())
				totalSize += UInt64.Parse(nodeIterator.Current.GetAttribute("size", ""));

			return totalSize;
		}

		private string GetXmlStringFromTarXVA()
		{
            using (Stream stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            {
                using (var iterator = ArchiveFactory.Reader(ArchiveFactory.Type.Tar, stream))
                    if (iterator.HasNext())
                    {
                        Stream ofs = new MemoryStream();
                        iterator.ExtractCurrentFile(ofs);
                        return new StreamReader(ofs).ReadToEnd();
                    }

                return string.Empty;
            }
        }

		private ulong GetTotalSizeFromXmlXva(string xmlString)
		{
			ulong totalSize = 0;
			XmlDocument xmlMetadata = new XmlDocument();
			xmlMetadata.LoadXml(xmlString);
			XPathNavigator nav = xmlMetadata.CreateNavigator();
			XPathNodeIterator nodeIterator = nav.Select(".//name[. = \"virtual_size\"]");

			while (nodeIterator.MoveNext())
			{
				XPathNavigator vdiNavigator = nodeIterator.Current;

				if (vdiNavigator.MoveToNext())
					totalSize += UInt64.Parse(vdiNavigator.Value);
			}
			return totalSize;
		}

		private bool GetDiskCapacityImage(out string error)
		{
			error = string.Empty;
            string filename = FilePath;

			FileInfo info = new FileInfo(filename);
			ImageLength = info.Length > 0 ? (ulong)info.Length : 0;

			if (IsWIM)
			{
				try
				{
					using (FileStream wimstream = new FileStream(filename, FileMode.Open, FileAccess.Read))
					{
						WimFile wimDisk = new WimFile(wimstream);
						string manifest = wimDisk.Manifest;
						Wim_Manifest wimManifest = Tools.Deserialize<Wim_Manifest>(manifest);
						DiskCapacity = wimManifest.Image[wimDisk.BootImage].TotalBytes; //image data size
						return true;
					}
				}
				catch (Exception)
				{
					error = Messages.IMAGE_SELECTION_PAGE_ERROR_CORRUPT_FILE;
					return false;
				}
			}

			try
			{
				using (VirtualDisk vd = VirtualDisk.OpenDisk(filename, FileAccess.Read))
				{
					DiskCapacity = (ulong)vd.Capacity;
					return true;
				}
			}
			catch (IOException ioe)
			{
				error = ioe.Message.Contains("Invalid VMDK descriptor file")
				        	? Messages.IMAGE_SELECTION_PAGE_ERROR_INVALID_VMDK_DESCRIPTOR
				        	: Messages.IMAGE_SELECTION_PAGE_ERROR_INVALID_FILE_TYPE;
				return false;
			}
			catch (Exception)
			{
				error = Messages.IMAGE_SELECTION_PAGE_ERROR_CORRUPT_FILE;
				return false;
			}
		}

		private bool LoadAppliance(out string error)
		{
			error = string.Empty;

            if (m_lastValidAppliance == FilePath)
				return true;

            SelectedOvfPackage = Package.Create(FilePath);

            if (!OVF.Validate(SelectedOvfPackage, out List<string> warnings))
            {
                error = warnings[warnings.Count - 1];
                return false;
            }

            if (warnings != null && warnings.Count > 0 && !Properties.Settings.Default.IgnoreOvfValidationWarnings)
            {
                using (var dlg = new OvfValidationDialog(warnings))
                {
                    if (dlg.ShowDialog(this) == DialogResult.Cancel)
                        return false;
                }
            }

            m_lastValidAppliance = FilePath;
            return true;
        }

		private bool IsUri()
		{
			Regex regex = new Regex(Messages.URI_REGEX, RegexOptions.IgnoreCase);
            return regex.Match(FilePath).Success;
		}

		/// <summary>
		/// Check on the fly
		/// </summary>
		private bool CheckPathValid(out string error)
		{
			error = string.Empty;

            if (String.IsNullOrEmpty(FilePath))
				return false;

            if (IsUri())
                return CheckDownloadFromUri(out error);

            if (!PathValidator.IsPathValid(FilePath))
			{
				error = Messages.IMPORT_SELECT_APPLIANCE_PAGE_ERROR_INVALID_PATH;
				return false;
			}

			return true;
		}

		/// <summary>
		/// Check when we leave the page
		/// </summary>
		private bool CheckIsSupportedType(out string error)
		{
			error = string.Empty;
			IsWIM = false;

            string filepath = FilePath;

			foreach (var ext in m_supportedXvaTypes)
			{
				if (filepath.ToLower().EndsWith(ext))
				{
					if (OvfModeOnly)
					{
						error = Messages.IMPORT_SOURCE_PAGE_ERROR_OVF_ONLY;
						return false;
					}

					if (ext == "ova.xml")
						IsXvaVersion1 = true;

					TypeOfImport = ImportWizard.ImportType.Xva;
					return true;
				}
			}

			foreach (var ext in m_supportedApplianceTypes)
			{
				if (filepath.ToLower().EndsWith(ext))
				{
					TypeOfImport = ImportWizard.ImportType.Ovf;
					return true;
				}
			}

			foreach (var ext in m_supportedImageTypes)
			{
				if (filepath.ToLower().EndsWith(ext))
				{
					if (OvfModeOnly)
					{
						error = Messages.IMPORT_SOURCE_PAGE_ERROR_OVF_ONLY;
						return false;
					}

					if (ext == ".wim")
						IsWIM = true;

					TypeOfImport = ImportWizard.ImportType.Vhd;
					return true;
				}
			}
			error = Messages.ERROR_UNSUPPORTED_FILE_TYPE;
			return false;
		}

		/// <summary>
		/// Check when we leave the page
		/// </summary>
		private bool CheckPathExists(out string error)
		{
			error = string.Empty;

            if (File.Exists(FilePath))
				return true;

			error = Messages.IMPORT_SELECT_APPLIANCE_PAGE_ERROR_NONE_EXIST_PATH;
			return false;
		}

        /// <summary>
        /// Check the source folder is writable
        /// </summary>
        private bool CheckSourceIsWritable(out string error)
        {
            error = string.Empty;

            var directory = Path.GetDirectoryName(FilePath);
            var touchFile = Path.Combine(directory, "_");

            try
            {
                //attempt writing in the destination directory
                using (FileStream fs = File.OpenWrite(touchFile))
                {
                    //it works
                }
                File.Delete(touchFile);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                error = Messages.IMPORT_SELECT_APPLIANCE_PAGE_ERROR_SOURCE_IS_READONLY;
                return false;
            }
        }

        private bool CheckDownloadFromUri(out string error)
        {
            error = string.Empty;
            try
            {
                _uri = new Uri(FilePath);
                return true;
            }
            catch (ArgumentNullException)
            {
                error = Messages.IMPORT_SELECT_APPLIANCE_PAGE_ERROR_INVALID_URI;
                return false;
            }
            catch (UriFormatException)
            {
                error = Messages.IMPORT_SELECT_APPLIANCE_PAGE_ERROR_INVALID_URI;
                return false;
            }
        }

        #endregion

        #region Download and Uncompression

        private bool Download()
        {
            using (var dlog = new FolderBrowserDialog
            {
                Description = Messages.FOLDER_BROWSER_DOWNLOAD_APPLIANCE,
                SelectedPath = Win32.GetKnownFolderPath(Win32.KnownFolders.Downloads)
            })
            {
                if (dlog.ShowDialog() == DialogResult.OK)
                {
                    if (_webClient == null)
                    {
                        _webClient = new WebClient { Proxy = XenAdminConfigManager.Provider.GetProxyFromSettings(null, false) };
                        _webClient.DownloadFileCompleted += webclient_DownloadFileCompleted;
                        _webClient.DownloadProgressChanged += webclient_DownloadProgressChanged;
                    }

                    _downloadFolder = dlog.SelectedPath;
                    var downloadedPath = Path.Combine(_downloadFolder, Path.GetFileName(_uri.AbsolutePath));
                    if (string.IsNullOrEmpty(Path.GetExtension(downloadedPath))) //CA-41747
                        downloadedPath += ".ovf";

                    var file = new ApplianceFile(_uri, downloadedPath);
                    _filesToDownload = new Queue<ApplianceFile>();
                    _filesToDownload.Enqueue(file);

                    LongProcessWrapper(() => _webClient.DownloadFileAsync(_uri, downloadedPath, file),
                        string.Format(Messages.IMPORT_WIZARD_DOWNLOADING, Path.GetFileName(downloadedPath).Ellipsise(50)));
                    return m_textBoxFile.Text == downloadedPath;
                }
                return false;
            }
        }

        private bool Uncompress()
        {
            _unzipFileIn = FilePath;
            if (string.IsNullOrEmpty(_unzipFileIn))
                return false;

            _unzipFileOut = Path.Combine(Path.GetDirectoryName(_unzipFileIn), Path.GetFileNameWithoutExtension(_unzipFileIn));

            var msg = string.Format(Messages.UNCOMPRESS_APPLIANCE_DESCRIPTION,
                Path.GetFileName(_unzipFileOut), Path.GetFileName(_unzipFileIn));

            using (var dlog = new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Exclamation, msg, Messages.XENCENTER),
                new ThreeButtonDialog.TBDButton(Messages.YES, DialogResult.Yes),
                new ThreeButtonDialog.TBDButton(Messages.NO, DialogResult.No)))
            {
                if (dlog.ShowDialog(this) == DialogResult.Yes)
                {
                    LongProcessWrapper(() => _unzipWorker.RunWorkerAsync(),
                        string.Format(Messages.IMPORT_WIZARD_UNCOMPRESSING, Path.GetFileName(_unzipFileIn).Ellipsise(50)));
                    return m_textBoxFile.Text == _unzipFileOut;
                }

                return false;
            }
        }

        private void LongProcessWrapper(Action process, string processMessage)
        {
            m_textBoxFile.Enabled = false;
            m_buttonBrowse.Enabled = false;
            m_buttonNextEnabled = false;
            OnPageUpdated();

            m_tlpError.Visible = false;
            labelProgress.Text = processMessage;
            labelProgress.Visible = true;
            progressBar1.Visible = true;
            progressBar1.Value = 0;

            longProcessInProgress = true;
            process.Invoke();
            while (longProcessInProgress)
                Application.DoEvents();

            m_textBoxFile.Enabled = true;
            m_buttonBrowse.Enabled = true;
        }

        #endregion

        #region Event handlers

        private void m_buttonBrowse_Click(object sender, EventArgs e)
        {
            using (FileDialog ofd = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DereferenceLinks = true,
                Filter = OvfModeOnly ? Messages.IMPORT_SOURCE_PAGE_FILETYPES_OVF_ONLY : Messages.IMPORT_SOURCE_PAGE_FILETYPES,
                RestoreDirectory = true,
                Multiselect = false,
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                    m_textBoxFile.Text = ofd.FileName;
            }
        }

		private void m_textBoxImage_TextChanged(object sender, EventArgs e)
		{
			m_tlpEncryption.Visible = false;
            m_tlpError.Visible = false;
			PerformCheck(CheckPathValid);
			IsDirty = true;
		}


        private void _unzipWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            FileInfo info = new FileInfo(_unzipFileIn);
            long length = info.Length;

            using (Stream inStream = File.Open(_unzipFileIn, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            using (Stream outStream = File.Open(_unzipFileOut, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                try
                {
                    using (CompressionStream bzis = CompressionFactory.Reader(CompressionFactory.Type.Gz, inStream))
                    {
                        byte[] buffer = new byte[4 * 1024];

                        int bytesRead;
                        while ((bytesRead = bzis.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            outStream.Write(buffer, 0, bytesRead);

                            int percentage = (int)Math.Floor((double)bzis.Position * 100 / length);

                            if (percentage < 0)
                                _unzipWorker.ReportProgress(0);
                            else if (percentage > 100)
                                _unzipWorker.ReportProgress(100);
                            else
                                _unzipWorker.ReportProgress(percentage);

                            if (_unzipWorker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }
                        }
                    }

                    e.Result = _unzipFileOut;
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Error while uncompressing {0} to {1}", _unzipFileIn, _unzipFileOut), ex);
                    throw;
                }
                finally
                {
                    outStream.Flush();
                }
            }
        }

        private void _unzipWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void _unzipWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Visible = false;
            labelProgress.Visible = false;

            if (e.Cancelled || e.Error != null)
            {
                if (e.Error != null)
                {
                    _labelError.Text = string.Format(Messages.IMPORT_WIZARD_FAILED_UNCOMPRESS,
                        Path.GetFileName(_unzipFileIn).Ellipsise(50));
                    m_tlpError.Visible = true;
                }

                try
                {
                    File.Delete(_unzipFileOut);
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Failed to delete uncompressed file {0}.", _unzipFileOut), ex);
                }

                longProcessInProgress = false;
                return;
            }

            var uncompressedFile = e.Result as string;
            progressBar1.Value = 100;
            m_textBoxFile.Text = uncompressedFile;

            try
            {
                File.Delete(_unzipFileIn);
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Failed to delete compressed file {0}.", _unzipFileIn), ex);
            }
            longProcessInProgress = false;
        }


        private void webclient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Program.Invoke(this, () => progressBar1.Value = e.ProgressPercentage);
        }

        private void webclient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Program.Invoke(this, () =>
            {
                var appfile = (ApplianceFile)e.UserState;

                if (e.Cancelled)
                {
                    longProcessInProgress = false;
                }
                else if (e.Error != null) // failure
                {
                    progressBar1.Visible = false;
                    labelProgress.Visible = false;
                    _labelError.Text = string.Format(Messages.IMPORT_WIZARD_FAILED_DOWNLOAD,
                        Path.GetFileName(appfile.LocalPath));
                    m_tlpError.Visible = true;
                    log.Error(string.Format("Failed to download file {0}.", appfile), e.Error);
                    longProcessInProgress = false;
                }
                else // success
                {
                    if (_filesToDownload.Peek() == appfile)
                        _filesToDownload.Dequeue();

                    if (appfile.LocalPath.ToLower().EndsWith(".ovf"))
                    {
                        var envType = Tools.DeserializeOvfXml(Tools.LoadFile(appfile.LocalPath));

                        if (envType != null)
                        {
                            int index = _uri.OriginalString.LastIndexOf('/') + 1;
                            var remoteDir = _uri.OriginalString.Substring(0, index);

                            foreach (var file in envType.References.File)
                            {
                                var remoteUri = new Uri(remoteDir + file.href);
                                var localPath = Path.Combine(_downloadFolder, file.href);
                                _filesToDownload.Enqueue(new ApplianceFile(remoteUri, localPath));
                            }
                        }
                    }

                    if (_filesToDownload.Count == 0)
                    {
                        progressBar1.Value = 100;
                        progressBar1.Visible = false;
                        labelProgress.Visible = false;
                        m_textBoxFile.Text = Path.Combine(_downloadFolder, Path.GetFileName(_uri.AbsolutePath));
                        longProcessInProgress = false;
                    }
                    else
                    {
                        progressBar1.Value = 0;
                        var file = _filesToDownload.Peek();
                        labelProgress.Text = string.Format(Messages.IMPORT_WIZARD_DOWNLOADING,
                            Path.GetFileName(file.LocalPath).Ellipsise(50));
                        _webClient.DownloadFileAsync(file.RemoteUri, file.LocalPath, file);
                    }
                }
            });
        }

        #endregion

        private class ApplianceFile
        {
            public ApplianceFile(Uri remoteUri, string localPath)
            {
                RemoteUri = remoteUri;
                LocalPath = localPath;
            }

            public Uri RemoteUri { get; private set; }

            public string LocalPath { get; private set; }
        }
    }
}
