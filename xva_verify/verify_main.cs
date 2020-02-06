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
using System.IO.Compression;
using System.Text;

namespace xva_verify
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Export.verbose_debugging = true;
            if (args.Length < 1 || args.Length > 2)
            {
                var sb = new StringBuilder();
                sb.AppendLine();
                sb.AppendLine("Usage").AppendLine();
                sb.AppendLine("  xva_verify <archive> [<copy>]").AppendLine();
                sb.AppendLine("where").AppendLine();
                sb.AppendLine("  <archive>  The name of the archive file to verify. Use '-' to read from stdin.");
                sb.AppendLine("  <copy>     If specified, a copy of the archive file is created with this name.").AppendLine();

                Console.WriteLine(sb.ToString());
                Environment.Exit(1);
            }

            try
            {
                string filename = args[0];

                Stream g = null;
                if (args.Length == 2)
                    g = new FileStream(args[1], FileMode.Create);

                Stream f = args[0].Equals("-")
                    ? Console.OpenStandardInput()
                    : new FileStream(filename, FileMode.Open, FileAccess.Read);

                // check for gzip compression (only on seekable inputs - i.e. not the stdin stream )
                if (f.CanSeek)
                {
                    try
                    {
                        GZipStream zip = new GZipStream(f, CompressionMode.Decompress);
                        // try reading a byte
                        zip.ReadByte();

                        // success - reset stream, use the gunzipped stream from now on
                        f.Seek(0, SeekOrigin.Begin);
                        f = new GZipStream(f, CompressionMode.Decompress);
                    }
                    catch(InvalidDataException)
                    {
                        // just reset the stream - Exception means the stream is not compressed
                        f.Seek(0, SeekOrigin.Begin);
                    }
                }

                new Export().verify(f, g, () => false);
            }
            catch(UnauthorizedAccessException)
            {
                Console.WriteLine("Permission denied, check access rights to file");
            }
            catch(FileNotFoundException)
            {
                Console.WriteLine("File not found, verify filename is correct");
            }
            catch(IOException)
            {
                Console.WriteLine("IO Exception, file may be truncated.");
            }
            catch(BlockChecksumFailed)
            {
                Console.WriteLine("Verification failed, file appears to be corrupt");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
