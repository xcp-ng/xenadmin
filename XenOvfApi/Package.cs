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
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using XenCenterLib;
using XenCenterLib.Archive;
using XenOvf.Definitions;
using XenOvf.Utilities;


namespace XenOvf
{
    /// <summary>
    /// Provides the properties of a digest for a file in the manifest of an OVF/OVA appliance.
    /// </summary>
    public class FileDigest
    {
        /// <summary>
        /// Creates a new instance from a line in the manifest.
        /// </summary>
        /// <param name="line"></param>
        public FileDigest(string line)
        {
            var fields = line.Split('(', ')', '=');

            // Expect the first field to be the security algorithm used to compute the hash of the file.
            AlgorithmName = fields[0].Trim();

            // Expect the second field to be the name of the file.
            Match nameMatch = new Regex(@"[\(](.*)[\)][=]").Match(line);
            Name = nameMatch.Success ? nameMatch.Groups[1].Value.Trim() : fields[1].Trim();

            // Expect the last field to be the digest of the file.
            DigestAsString = fields[fields.Length - 1].Trim();
            Digest = ToArray(DigestAsString);
        }

        /// <summary>
        /// Name of the file with the digest.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Name of the algorithm to compute the digest. It must be recognized by HashAlgorithm.Create().
        /// </summary>
        public string AlgorithmName { get; }

        public string DigestAsString { get; }

        public byte[] Digest{ get; }

        /// <summary>
        /// Convert a hex string to a binary array.
        /// </summary>
        private static byte[] ToArray(string HexString)
        {
            if (HexString.Length % 2 > 0)
            {
                // The length is odd.
                // Pad the hex string on the left with a space to interpret as a leading zero.
                HexString = HexString.PadLeft(HexString.Length + 1);
            }

            var array = new byte[HexString.Length / 2];

            try
            {
                for (int i = 0; i < HexString.Length; i += 2)
                {
                    array[i / 2] = byte.Parse(HexString.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
                }
            }
            catch
            {
                throw new Exception(HexString + " contains an invalid hexadecimal number.");
            }

            return array;
        }
    }


    /// <summary>
    /// OVF package, i.e. appliance in the form of a folder
    /// </summary>
    internal class FolderPackage : Package
    {
        private string _Folder;

        public FolderPackage(string path)
            : base(path)
        {
            _Folder = path;

            var extension = Path.GetExtension(path);

            if (string.Compare(extension, Properties.Settings.Default.ovfFileExtension, true) == 0)
                _Folder = Path.GetDirectoryName(path);

            if (_Folder == null)
            {
                // Handle a package in the root or current folder.
                _Folder = "";
            }
        }

        public override string DescriptorFileName => Path.GetFileName(PackageSourceFile);

        protected override byte[] ReadAllBytes(string fileName)
        {
            try
            {
                return File.ReadAllBytes(Path.Combine(_Folder, fileName));
            }
            catch
            {
                return null;
            }
        }

        protected override string ReadAllText(string fileName)
        {
            try
            {
                return File.ReadAllText(Path.Combine(_Folder, fileName));
            }
            catch
            {
                return null;
            }
        }

        public override void VerifyManifest()
        {
            // Verify the presence of a manifest.
            var manifest = Manifest;

            // For a folder package, it is efficient to iterate by the order of files in the manifest.
            foreach (FileDigest fileDigest in manifest)
            {
                using (var stream = File.OpenRead(Path.Combine(_Folder, fileDigest.Name)))
                {
                    if (!StreamUtilities.VerifyAgainstDigest(stream, stream.Length, fileDigest.AlgorithmName, fileDigest.Digest))
                        throw new Exception(string.Format(Messages.SECURITY_SIGNATURE_FAILED, fileDigest.Name));
                }
            }
        }

        public override bool HasFile(string fileName)
        {
            try
            {
                return File.Exists(Path.Combine(_Folder, fileName));
            }
            catch
            {
                return false;
            }
        }

        public override void ExtractToWorkingDir()
        {
            WorkingDir = Path.GetDirectoryName(PackageSourceFile);
        }
    }

    /// <summary>
    /// OVA package (may additionally be compressed), i.e. Appliance in the form of a tape archive (tar).
    /// </summary>
    internal class TarPackage : Package
    {
        private readonly Dictionary<string, byte[]> _DirectoryCache = new Dictionary<string, byte[]>();
        private string _DescriptorFileName;

        private Stream _tarStream;
        private ArchiveIterator _archiveIterator;
        private readonly ArchiveFactory.Type _tarType = ArchiveFactory.Type.Tar;

        public TarPackage(string path)
            : base(path)
        {
            var extension = Path.GetExtension(PackageSourceFile);

            if (string.Compare(extension, ".gz", true) == 0)
                _tarType = ArchiveFactory.Type.TarGz;
            else if (string.Compare(extension, ".bz2", true) == 0)
                _tarType = ArchiveFactory.Type.TarBz2;
        }

        public override string DescriptorFileName
        {
            get
            {
                if (_DescriptorFileName == null)
                    Load();
                return _DescriptorFileName;
            }
        }

        protected override byte[] ReadAllBytes(string fileName)
        {
            if (!_DirectoryCache.ContainsKey(fileName))
                Load();
            return _DirectoryCache.TryGetValue(fileName, out byte[] buffer) ? buffer : null;
        }

        protected override string ReadAllText(string fileName)
        {
            var buffer = ReadAllBytes(fileName);
            if (buffer == null)
                return null;

            using (var stream = new MemoryStream(buffer))
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }

        /// <summary>
        /// Load metadata files from an OVA package. Rules:
        /// - Load the first file name with an ovf extension as the package descriptor
        /// - Load the first manifest or certificate file that has the same base name as the descriptor
        /// - Do not load any other files
        /// In most cases the ovf file is first in the package, so all metadata files
        /// can be read in one go. Otherwise, the package will have to be searched more times.
        /// </summary>
        private void Load()
        {
            try
            {
                Open();

                while (_archiveIterator.HasNext())
                {
                    var extension = Path.GetExtension(_archiveIterator.CurrentFileName());

                    if (_DescriptorFileName == null && string.Compare(extension, Properties.Settings.Default.ovfFileExtension, true) == 0)
                    {
                        _DescriptorFileName = _archiveIterator.CurrentFileName();
                    }
                    else if (string.Compare(extension, Properties.Settings.Default.manifestFileExtension, true) == 0)
                    {
                        if (_DescriptorFileName == null || string.Compare(_archiveIterator.CurrentFileName(), ManifestFileName, true) != 0)
                            continue;
                    }
                    else if (string.Compare(extension, Properties.Settings.Default.certificateFileExtension, true) == 0)
                    {
                        if (_DescriptorFileName == null || string.Compare(_archiveIterator.CurrentFileName(), CertificateFileName, true) != 0)
                            continue;
                    }
                    else
                    {
                        //add other files to the directory cache without, obviously, loading them
                        _DirectoryCache[_archiveIterator.CurrentFileName()] = null;
                        continue;
                    }

                    _DirectoryCache[_archiveIterator.CurrentFileName()] = ReadAllBytes();
                }
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// Always remember to call Close after having used the opened package,
        /// usually in a try-finally block
        /// </summary>
        private void Open()
        {
            _tarStream = File.OpenRead(PackageSourceFile);
            _archiveIterator = ArchiveFactory.Reader(_tarType, _tarStream);
        }

        /// <summary>
        /// Normally used in conjunction with Open in a try-finally block
        /// </summary>
        private void Close()
        {
            _archiveIterator?.Dispose();
            _tarStream?.Dispose();
        }

        public override void CleanUpWorkingDir()
        {
            try
            {
                Directory.Delete(WorkingDir, true);
            }
            catch
            {
                //ignore
            }
        }

        public override void ExtractToWorkingDir()
        {
            WorkingDir = OVF.ExtractArchive(PackageSourceFile);
        }

        public override void VerifyManifest()
        {
            var manifest = new List<FileDigest>(Manifest);

            try
            {
                Open();

                // For a tar package, it is more efficient to iterate the files in the order within the archive.
                while (_archiveIterator.HasNext())
                {
                    var extension = Path.GetExtension(_archiveIterator.CurrentFileName());
                    if (string.Compare(extension, Properties.Settings.Default.manifestFileExtension, true) == 0 ||
                        string.Compare(extension, Properties.Settings.Default.certificateFileExtension, true) == 0)
                        continue;

                    var fileDigest = manifest.Find(fd => string.Compare(fd.Name, _archiveIterator.CurrentFileName(), true) == 0);

                    if (fileDigest == null)
                    {
                        log.ErrorFormat("File {0} contained in the appliance is not listed in the manifest.", _archiveIterator.CurrentFileName());
                        throw new Exception(Messages.SECURITY_FILE_MISSING_FROM_MANIFEST);
                    }

                    manifest.Remove(fileDigest);
                    if (!_archiveIterator.VerifyCurrentFileAgainstDigest(fileDigest.AlgorithmName, fileDigest.Digest))
                        throw new Exception(string.Format(Messages.SECURITY_SIGNATURE_FAILED, fileDigest.Name));
                }
            }
            finally
            {
                Close();
            }

            if (manifest.Count > 0)
            {
                log.ErrorFormat("The following files are listed in the manifest but missing from the appliance: {0}",
                    string.Join(", ", manifest.Select(fd => fd.Name)));
                throw new Exception(Messages.SECURITY_FILE_MISSING_FROM_PACKAGE);
            }
        }

        public override bool HasFile(string fileName)
        {
            try
            {
                return DescriptorFileName != null && _DirectoryCache.ContainsKey(fileName);
            }
            catch
            {
                return false;
            }
        }

        private byte[] ReadAllBytes()
        {
            using (var ms = new MemoryStream())
            {
                _archiveIterator.ExtractCurrentFile(ms);
                return ms.Length > 0 ? ms.ToArray() : null;
            }
        }
    }


    /// <summary>
    /// Abstract class to describe an OVF/OVA appliance.
    /// </summary>
    public abstract class Package
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Cache these properties because they are expensive to get
        private string _descriptorXml;
        private byte[] _RawManifest;
        private byte[] _RawCertificate;
        private EnvelopeType _ovfEnvelope;

        protected Package(string path)
        {
            PackageSourceFile = path;
        }

        public static Package Create(string path)
        {
            var extension = Path.GetExtension(path);

            if (string.Compare(extension, Properties.Settings.Default.ovfFileExtension, true) == 0)
                return new FolderPackage(path);

            // Assume it is an archive.
            return new TarPackage(path);
        }

        #region Properties

        /// <summary>
        /// According to the OVF specification, name of the *package* is base name of the descriptor file.
        /// </summary>
        public string Name => Path.GetFileNameWithoutExtension(DescriptorFileName);

        /// <summary>
        /// According to the OVF specification, base name of the manifest file must be the same as the descriptor file.
        /// </summary>
        protected string ManifestFileName => Name + Properties.Settings.Default.manifestFileExtension;

        /// <summary>
        /// According to the OVF specification, base name of the certificate file must be the same as the descriptor file.
        /// </summary>
        protected string CertificateFileName => Name + Properties.Settings.Default.certificateFileExtension;

        /// <summary>
        /// Contents of the OVF file.
        /// </summary>
        public string DescriptorXml
        {
            get
            {
                if (_descriptorXml == null)
                    _descriptorXml = ReadAllText(DescriptorFileName);
                return _descriptorXml;
            }
        }

        protected List<FileDigest> Manifest
        {
            get
            {
                var manifest = new List<FileDigest>();

                using (var stream = new MemoryStream(RawManifest))
                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        manifest.Add(new FileDigest(reader.ReadLine()));
                    }
                }

                return manifest;
            }
        }

        private byte[] RawManifest
        {
            get
            {
                if (_RawManifest == null)
                    _RawManifest = ReadAllBytes(ManifestFileName);
                return _RawManifest;
            }
        }

        public byte[] RawCertificate
        {
            get
            {
                if (_RawCertificate == null)
                    _RawCertificate = ReadAllBytes(CertificateFileName);
                return _RawCertificate;
            }
        }

        public string PackageSourceFile { get; }

        public string WorkingDir { get; protected set; }

        /// <summary>
        /// May be null if no valid OVF file has been found
        /// </summary>
        public EnvelopeType OvfEnvelope
        {
            get
            {
                if (_ovfEnvelope == null)
                {
                    try
                    {
                        _ovfEnvelope = Tools.DeserializeOvfXml(DescriptorXml);
                    }
                    catch (Exception ex)
                    {
                        log.Error($"Failed to load OVF content from appliance {PackageSourceFile}", ex);
                    }
                }
                return _ovfEnvelope;
            }
        }

        #endregion

        public bool HasEncryption()
        {
            return OVF.HasEncryption(OvfEnvelope);
        }

        public bool HasManifest()
        {
            return RawManifest != null;
        }

        public bool HasSignature()
        {
            return RawCertificate != null;
        }

        public void VerifySignature()
        {
            using (var certificate = new X509Certificate2(RawCertificate))
            {
                if (!certificate.Verify())
                    throw new Exception(Messages.CERTIFICATE_IS_INVALID);

                // Get the package signature from the certificate file.
                // This is the digest of the first file listed in the certificate file,
                // hence we only need to read the first line
                FileDigest fileDigest;

                using (Stream stream = new MemoryStream(RawCertificate))
                using (StreamReader reader = new StreamReader(stream))
                {
                    fileDigest = new FileDigest(reader.ReadLine());
                }

                // Verify the stored signature against the computed signature using the certificate's public key.
                // Do this independently to minimize the number of files opened concurrently.
                using (Stream stream = new MemoryStream(RawManifest))
                {
                    if (!StreamUtilities.VerifyAgainstDigest(stream, stream.Length, fileDigest.AlgorithmName, fileDigest.Digest, certificate.PublicKey.Key as RSACryptoServiceProvider))
                        throw new Exception(string.Format(Messages.SECURITY_SIGNATURE_FAILED, fileDigest.Name));
                }
            }
        }

        public virtual void CleanUpWorkingDir()
        {
        }

        #region Abstract members

        /// <summary>
        /// The full name of the OVF file within the package
        /// </summary>
        public abstract string DescriptorFileName { get; }

        protected abstract byte[] ReadAllBytes(string fileName);

        protected abstract string ReadAllText(string fileName);

        public abstract void VerifyManifest();

        public abstract bool HasFile(string fileName);

        /// <returns>The directory with the extracted files</returns>
        public abstract void ExtractToWorkingDir();

        #endregion
    }
}
