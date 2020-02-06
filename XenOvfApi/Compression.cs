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
using System.IO;
using XenCenterLib.Compression;
using XenOvf.Definitions;


namespace XenOvf
{
    public class OvfCompressor
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    	/// <summary>
    	/// Set to TRUE to Cancel current Compression process.
    	/// </summary>
		public bool CancelCompression { get; set; }

        /// <summary>
        /// Check the Envelope to see if the files are compressed.
        /// </summary>
        /// <param name="ovfFilename">fullpath/filename to OVF</param>
        /// <param name="method">out method used: Gzip | BZip2</param>
        /// <returns>True|False</returns>
        public bool IsCompressed(string ovfFilename, out string method)
        {
            EnvelopeType env = OVF.Load(ovfFilename);
            return IsCompressed(env, out method);
        }
        /// <summary>
        /// Check the Envelope to see if the files are compressed.
        /// </summary>
        /// <param name="env">EnvelopeType</param>
        /// <param name="method">out method used: Gzip | BZip2</param>
        /// <returns>True|False</returns>
        public bool IsCompressed(EnvelopeType env, out string method)
        {
            if (env.References != null && env.References.File != null && env.References.File.Length > 0)
            {
                foreach (File_Type file in env.References.File)
                {
                    if (!string.IsNullOrEmpty(file.compression))
                    {
                        method = file.compression;
                        return true;
                    }
                }
            }
            method = "none";
            return false;
        }
        /// <summary>
        /// Open the OVF file and compress the uncompressed files.
        /// </summary>
        /// <param name="ovfFilename">pull path and filename.</param>
        /// <param name="method">GZip | BZip2</param>
        public void CompressOvfFiles(string ovfFilename, string method)
        {
			CompressOvfFiles(OVF.Load(ovfFilename), ovfFilename, method, true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="env"></param>
        /// <param name="ovfilename"></param>
        /// <param name="method"></param>
        /// <param name="compress"></param>
        public void CompressOvfFiles(EnvelopeType env, string ovfilename, string method, bool compress)
        {
            ProcessCompression(env, Path.GetDirectoryName(ovfilename), method, compress);
			OVF.SaveAs(env, ovfilename);
        }
        /// <summary>
        /// Open the OVF file and uncompress the compressed files.
        /// </summary>
        /// <param name="ovfFilename">pull path and filename.</param>
        public void UncompressOvfFiles(string ovfFilename)
        {
			EnvelopeType env = OVF.Load(ovfFilename);
            ProcessCompression(env, Path.GetDirectoryName(ovfFilename), null, false);
			OVF.SaveAs(env, ovfFilename);
        }
        /// <summary>
        /// Uncompress a stream
        /// </summary>
        /// <param name="inputstream">compressed stream</param>
        /// <param name="method">GZip or Bzip2</param>
        /// <returns>uncompressed stream</returns>
        public CompressionStream UncompressFileStream(Stream inputstream, string method)
        {
        	switch (method.ToLower())
        	{
        		case "gzip":
                    return CompressionFactory.Reader(CompressionFactory.Type.Gz, inputstream);
        		case "bzip2":
                    return CompressionFactory.Reader(CompressionFactory.Type.Bz2, inputstream);
        	}
        	throw new InvalidDataException(string.Format(Messages.COMPRESS_INVALID_METHOD, method));
        }

    	/// <summary>
        /// Compress a Stream 
        /// </summary>
        /// <param name="inputstream">Source Input</param>
        /// <param name="method">GZip or BZip2</param>
        /// <returns>compressed stream</returns>
        public CompressionStream CompressFileStream(Stream inputstream, string method)
        {
        	switch (method.ToLower())
        	{
        		case "gzip":
        			return CompressionFactory.Writer(CompressionFactory.Type.Gz, inputstream);
        		case "bzip2":
        			return CompressionFactory.Writer(CompressionFactory.Type.Bz2, inputstream);
        	}
        	throw new InvalidDataException(string.Format(Messages.COMPRESS_INVALID_METHOD, method));
        }

    	/// <summary>
        /// 
        /// </summary>
        /// <param name="inputfile"></param>
        /// <param name="outputfile"></param>
        /// <param name="method"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
                                                         Justification = "Logging only usage")]
        public void UncompressFile(string inputfile, string outputfile, string method)
        {
            if (method.ToLower() == "gzip")
            {
                Stream InStream = File.Open(inputfile, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                Stream OutStream = File.Open(outputfile, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                try
                {
                    GZipDecompress(InStream, OutStream);
                }
                catch (Exception ex)
                {
					if (ex is OperationCanceledException)
						throw;
                    log.Warn("Ignoring uncompression issue: ", ex);
                }
                finally
                {
                    InStream.Dispose();
                    OutStream.Flush();
                    OutStream.Dispose();
                }
            }
            else
            {
                throw new InvalidDataException(string.Format(Messages.COMPRESS_INVALID_METHOD, method));
            }
        }


        #region PRIVATE
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
                                                         Justification = "Logging only usage")]
        private void ProcessCompression(EnvelopeType env, string path, string method, bool compress)
        {
            if (env.References != null && env.References.File != null && env.References.File.Length > 0)
            {
                foreach (File_Type file in env.References.File)
                {
                    if (compress)
                    {
                        if (method.ToLower() != "gzip" && method.ToLower() != "bzip2")
                            throw new InvalidDataException(string.Format(Messages.COMPRESS_INVALID_METHOD, method));
                    }
                    else
                    {
                        if (file.compression == null)
                        {
                            log.InfoFormat("File {0} was not marked as compressed, skipped.", file.href);
                            continue;
                        }
						if (file.compression.ToLower() != "gzip" && file.compression.ToLower() != "bzip2")
                        {
                            log.ErrorFormat("Invalid compression method File: {0} Method: {1}, must be Gzip or BZip2", file.href, file.compression);
                            continue;
                        }

                    	method = file.compression;
                    }

                    int slash = file.href.LastIndexOf('/');
                    string filename =  slash >= 0 ? file.href.Substring(slash + 1) : file.href;

                    filename = Path.Combine(path, filename);

                    string tempfile = Path.Combine(path, Path.GetFileName(Path.GetTempFileName()));

                    try
                    {
                    	if (method.ToLower() == "gzip")
						{
							using(var inputFile = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
							{
								using (var outputFile = new FileStream(tempfile, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None))
								{
									if (compress)
										GZipCompress(inputFile, outputFile);
									else
										GZipDecompress(inputFile, outputFile);
								}
								inputFile.Flush();
							}

							//the original file should be overwritten only if the process has succeeded, so it should
							//be done here and not in the finally block, because if compression/decompression fails
							//for some reason, the original file is lost
							File.Delete(filename);
							File.Move(tempfile, filename);
						}
                        else
                        {
                            throw new InvalidDataException(string.Format(Messages.COMPRESS_INVALID_METHOD, method));
                        }
                    	file.compression = compress ? method : null;
                    }
                    catch (EndOfStreamException eose)
                    {
                        log.Error("End of Stream: ", eose);
                    }
                    catch (Exception ex)
                    {
						if (ex is OperationCanceledException)
							throw;

                        log.Error("Compression failure", ex);
                        throw new Exception(string.Format(Messages.COMPRESS_FAILED, filename), ex);
                    }
                    finally
                    {
                    	//in case of failure or cancellation delete the temp file;
						//in case of success it won't be there, but no exception is thrown
						File.Delete(tempfile);
                    }
                    FileInfo fi = new FileInfo(filename);
                    file.size = (ulong)fi.Length;
                }
            }
        }
        
		private void GZipCompress(Stream inputStream, Stream outputStream)
        {
            if (inputStream == null || outputStream == null) { throw new ArgumentNullException(); }
            CompressionStream bzos = CompressionFactory.Writer(CompressionFactory.Type.Gz, outputStream);
            StreamCopy(inputStream, (CompressionStream)bzos);
            bzos.Dispose();
        }

        private void GZipDecompress(Stream inputStream, Stream outputStream)
        {
            if (inputStream == null || outputStream == null) { throw new ArgumentNullException(); }
            CompressionStream bzis = CompressionFactory.Reader(CompressionFactory.Type.Gz, inputStream);
            StreamCopy((CompressionStream)bzis, outputStream);           
            outputStream.Flush();
            bzis.Dispose();
        }

		/// <summary>
		/// This code is similar to the one in XenOvf.Utilities.Tool.StreamCopy.
		/// It's duplicated (almost duplicated, I removed the block clearing to improve performance) as part
		/// of the fix to CA-54859 in order to avoid too large scale changes at a late stage of development.
		/// </summary>
		private void StreamCopy(Stream Input, Stream Output)
		{
			int bufsize = 2 * MB;
			byte[] block = new byte[bufsize];

			while (true)
			{
				int n = Input.Read(block, 0, block.Length);
				if (n <= 0)
					break;
				Output.Write(block, 0, n);

				if (CancelCompression)
					throw new OperationCanceledException();
			}
			Output.Flush();
		}

		private const int KB = 1024;
		private const int MB = (KB * 1024);
        #endregion
    }
}