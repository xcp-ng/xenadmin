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
using System.Text;

using XenCenterLib.Archive;


namespace XenAdmin.Actions
{
    public class ZipStatusReportAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The folder containing the raw files as downloaded from the server
        /// </summary>
        private readonly string _inputTempFolder;
        /// <summary>
        /// The destination zip file for the repackaged server log files
        /// </summary>
        private readonly string _destFile;

        /// <summary>
        /// A dictionary mapping file to original modification time.  The filepath used is the full path
        /// within the staging directory (extractTempDir) where we put all the files before repacking them.
        /// The modification time either comes from the source tarball (if downloaded from a server), or
        /// the source file (if copying a local file).
        /// </summary>
        private readonly Dictionary<string, DateTime> ModTimes = new Dictionary<string, DateTime>();

        private long bytesToCompress = 1;

        /// <summary>
        ///  Constructor to zip downloaded file action
        /// </summary>
        /// <param name="tempFolder">Temporary folder to store the downloaded logs</param>
        /// <param name="destFile">The target file to store the compressed result</param>
        /// <param name="suppressHistory">Whether to suppress history in the event tab</param>
        public ZipStatusReportAction(string tempFolder, string destFile, bool suppressHistory=true)
            : base(null, Messages.BUGTOOL_SAVING, Messages.BUGTOOL_SAVING, suppressHistory)
        {
            _inputTempFolder = tempFolder;
            _destFile = destFile;
        }

        protected override void Run()
        {
            // The directory in which we assemble the log files from the server before repackaging them
            // in a single zip file.
            string extractTempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            try
            {
                // Calculate total bytes to save
                long bytesToExtract = 1, bytesExtracted = 0;
                foreach (string inputFile in Directory.GetFiles(_inputTempFolder))
                {
                    bytesToExtract += new FileInfo(inputFile).Length;
                }

                // Create temp dir for extracted stuff
                if (Directory.Exists(extractTempDir))
                {
                    Directory.Delete(extractTempDir);
                }
                Directory.CreateDirectory(extractTempDir);

                // Extract each of the raw server files to the temp extraction directory
                foreach (string inputFile in Directory.GetFiles(_inputTempFolder))
                {
                    if (inputFile.ToLowerInvariant().EndsWith(".tar"))
                    {
                        // Un-tar it. SharpZipLib doesn't account for illegal filenames or characters in
                        // filenames (e.g. ':'in Windows), so first we stream the tar to a new tar,
                        // sanitizing any bad filenames as we go.

                        // We also need to record the modification times of all the files, so that we can
                        // restore them into the final zip.
                        
                        string outFilename = inputFile.Substring(0, inputFile.Length - 4);
                        if (outFilename.Length == 0)
                            outFilename = Path.GetRandomFileName();
                        string outputDir = Path.Combine(extractTempDir, Path.GetFileName(outFilename));

                        string sanitizedTar = Path.GetTempFileName();

                        using (ArchiveIterator tarIterator = ArchiveFactory.Reader(ArchiveFactory.Type.Tar, File.OpenRead(inputFile)))
                        {
                            using (ArchiveWriter tarWriter = ArchiveFactory.Writer(ArchiveFactory.Type.Tar, File.OpenWrite(sanitizedTar)))
                            {
                                Dictionary<string, string> usedNames = new Dictionary<string, string>();
                                while (tarIterator.HasNext())
                                {
                                    if (Cancelling)
                                    {
                                        throw new CancelledException();
                                    }

                                    using( MemoryStream ms = new MemoryStream() )
                                    {
                                        tarIterator.ExtractCurrentFile(ms);
                                        string saneName = SanitizeTarName(tarIterator.CurrentFileName(), usedNames);
                                        tarWriter.Add(ms, saneName);
                                        ModTimes[Path.Combine(outputDir, saneName)] = tarIterator.CurrentFileModificationTime();
                                    }

                                }
                            }
                        }

                        // Now extract the sanitized tar
                        using(FileStream fs = File.OpenRead(sanitizedTar))
                        {
                            using (ArchiveIterator tarIterator = ArchiveFactory.Reader(ArchiveFactory.Type.Tar, fs))
                            {
                                Directory.CreateDirectory(outputDir);
                                tarIterator.ExtractAllContents(outputDir);
                                bytesToCompress += Core.Helpers.GetDirSize(new DirectoryInfo(outputDir));
                            }
                        }
                    }
                    else
                    {
                        // Just copy vanilla input files unmodified to the temp directory
                        string outputFile = Path.Combine(extractTempDir, Path.GetFileName(inputFile));
                        File.Copy(inputFile, outputFile);
                        ModTimes[outputFile] = new FileInfo(inputFile).LastWriteTimeUtc;
                        bytesToCompress += new FileInfo(outputFile).Length;
                    }

                    bytesExtracted += new FileInfo(inputFile).Length;
                    File.Delete(inputFile);
                    this.PercentComplete = (int)(50.0 * bytesExtracted / bytesToExtract);

                    if (Cancelling)
                    {
                        throw new CancelledException();
                    }
                }

                // Now zip up all the temporarily extracted files into a single zip file for the user
                log.DebugFormat("Packing {0} of bug report files into zip file {1}",
                    Util.DiskSizeString(bytesToCompress), _destFile);

                LogDescriptionChanges = false;
                try
                {

                    ZipToOutputFile(extractTempDir);
                    PercentComplete = 100;

                    // Only cleanup files if it succeeded (or cancelled)
                    CleanupFiles(extractTempDir);
                }
                finally
                {
                    LogDescriptionChanges = true;
                }

                if (Cancelling)
                    throw new CancelledException();
            }
            catch (CancelledException)
            {
                CleanupFiles(extractTempDir, true);

                throw;
            }
            catch (Exception exn)
            {
                ZipToOutputFile(_inputTempFolder);
                PercentComplete = 100;
                log.ErrorFormat("An exception was trapped while creating a server status report: " + exn.Message);
                throw new Exception(Messages.STATUS_REPORT_ZIP_FAILED);
            }
        }

        private void ZipToOutputFile(string folderToZip)
        {
            using (ArchiveWriter zip = ArchiveFactory.Writer(ArchiveFactory.Type.Zip, File.OpenWrite(_destFile)))
            {
                zip.CreateArchive(folderToZip);
            }
        }

        private void CleanupFiles(string extractTempDir, bool deleteDestFile = false)
        {
            // We completed successfully: delete temporary files
            log.Debug("Deleting temporary files");
            try
            {
                // Delete temp directory of raw server files to-be-decompressed
                Directory.Delete(_inputTempFolder, true);
            }
            catch (Exception exn)
            {
                log.Warn("Could not delete temporary decompressed files directory", exn);
            }

            try
            {
                // Try to remove temp decompressed files dir
                Directory.Delete(extractTempDir, true);
            }
            catch (Exception exn)
            {
                log.Warn("Could not delete temporary extracted files directory", exn);
            }

            try
            {
                if (deleteDestFile)
                {
                    File.Delete(_destFile);
                }
            }
            catch (Exception ex)
            {
                log.Warn("Could not delete destination file", ex);
            }
        }

        /// <summary>
        /// Maps file/directory names that are illegal under Windows to 'sanitized' versions. The usedNames
        /// parameter ensures this is done consistently within a directory tree.
        /// 
        /// The dictionary is used by SanitizeTarName() to ensure names are consistently sanitized. e.g.:
        /// dir1: -> dir1_
        /// dir1? -> dir1_ (1)
        /// dir1_ -> dir1_ (2)
        /// dir1:/file -> dir1_/file
        /// dir1?/file -> dir1_ (1)/file
        ///
        /// Pass the same dictionary to each invocation to get unique outputs within the same tree.
        /// </summary>
        private static string SanitizeTarName(string path, Dictionary<string, string> usedNames)
        {
            string sanitizedPath = "";
            Stack<string> bitsToEscape = new Stack<string>();
            // Trim any trailing slashes (usually indicates path is a directory)
            path = path.TrimEnd(new char[] { '/' });
            // Take members off the end of the path until we have a name that already is
            // a key in our dictionary, or until we have the empty string.
            while (!usedNames.ContainsKey(path) && path.Length > 0)
            {
                string[] bits = path.Split(new char[] { '/' });
                string lastBit = bits[bits.Length - 1];
                int lengthOfLastBit = lastBit.Length;
                bitsToEscape.Push(lastBit);
                path = path.Substring(0, path.Length - lengthOfLastBit);
                path = path.TrimEnd(new char[] { '/' });
            }

            if (usedNames.ContainsKey(path))
            {
                sanitizedPath = usedNames[path];
            }

            // Now for each member in the path, look up the escaping of that member if it exists; otherwise
            // generate a new, unique escaping. Then append the escaped member to the end of the sanitized
            // path and continue.
            foreach (string member in bitsToEscape)
            {
                System.Diagnostics.Trace.Assert(member.Length > 0);
                string sanitizedMember = SanitizeTarPathMember(member);
                sanitizedPath = Path.Combine(sanitizedPath, sanitizedMember);
                path = path + Path.DirectorySeparatorChar + member;

                // Note: even if sanitizedMember == member, we must add it to the dictionary, since
                // tar permits names that differ only in case, while Windows does not. We must e.g.:
                // abc -> abc
                // aBC -> aBC (1)
                
                if (usedNames.ContainsKey(path))
                {
                    // We have already generated an escaping for this path prefix: use it
                    sanitizedPath = usedNames[path];
                    continue;
                }

                // Generate the unique mapping
                string pre = sanitizedPath;
                int i = 1;
                while (DictionaryContainsIgnoringCase(usedNames, sanitizedPath))
                {
                    sanitizedPath = string.Format("{0} ({1})", pre, i);
                    i++;
                }

                usedNames.Add(path, sanitizedPath);
            }
            return sanitizedPath;
        }

        private static bool DictionaryContainsIgnoringCase(Dictionary<string, string> dict, string value)
        {
            foreach (string v in dict.Values)
            {
                if (v.ToUpperInvariant() == value.ToUpperInvariant())
                {
                    return true;
                }
            }
            return false;
        }

        // See http://msdn.microsoft.com/library/default.asp?url=/library/en-us/fileio/fs/naming_a_file.asp
        private static readonly string[] forbiddenNames = { "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };
        public static string SanitizeTarPathMember(string member)
        {
            // Strip any whitespace, or Windows will do it for us, and we might generate non-unique names
            member = member.Trim();

            foreach (string reserved in forbiddenNames)
            {
                // Names can't be any of com1, com2, or com1.xyz, com2.abc etc.
                if (member.ToUpperInvariant() == reserved.ToUpperInvariant()
                    || member.ToUpperInvariant().StartsWith(reserved.ToUpperInvariant() + "."))
                {
                    member = "_" + member;
                }
            }

            // Allow only 31 < c < 126, excluding  < > : " / \ | ? *
            StringBuilder sb = new StringBuilder(member.Length);
            foreach (char c in member.ToCharArray())
            {
                if (c > 31 && c < 127 && !IsCharExcluded(c))
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append("_");
                }
            }
            member = sb.ToString();

            // Windows also seems not to like filenames ending '.'
            if (member.EndsWith("."))
            {
                member = member.Substring(0, member.Length - 1) + "_";
            }

            // Don't allow empty filename
            if (member.Length == 0)
            {
                member = "_";
            }

            return member;
        }

        private static readonly char[] excludedChars = new char[] { '<', '>', ':', '"', '/', '\\', '|', '?', '*' };
        private static bool IsCharExcluded(char c)
        {
            foreach (char excluded in excludedChars)
            {
                if (c == excluded)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Copies the specified number of bytes from one stream to another via the provided buffer.
        /// </summary>
        private static void CopyStream(Stream inputStream, Stream outputStream, long bytesToCopy, byte[] buf)
        {
            while (bytesToCopy > 0)
            {
                int bytesRead = inputStream.Read(buf, 0, Math.Min(bytesToCopy > int.MaxValue ? int.MaxValue : (int)bytesToCopy, buf.Length));
                outputStream.Write(buf, 0, bytesRead);
                bytesToCopy -= bytesRead;
            }
        }

        public override void RecomputeCanCancel()
        {
            CanCancel = !Cancelling && !IsCompleted;
        }

        protected override void CancelRelatedTask()
        {
        }
    }
}
