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
using System.Collections.Specialized;
using System.Globalization;
using System.Threading;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;


namespace XenOvfTransport
{
    public class Http
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public Action<XenOvfTransportEventArgs> UpdateHandler { get; set; }
		
		private void OnUpdate(XenOvfTransportEventArgs e)
		{
			if (UpdateHandler != null)
				UpdateHandler.Invoke(e);
		}

        internal enum Tag
        {
            Print = 0, Load = 1, HttpGet = 12, HttpPut = 13, Prompt = 3, Exit = 4,
            Error = 14, OK = 5, Failed = 6, Chunk = 7, End = 8, Command = 9, Response = 10,
            Blob = 11, Debug = 15, PrintStderr = 16
        };

        private TcpClient tcpClient = null;

        private const long KB = 1024;
        private const long MB = (KB * 1024);
        private const long GB = (MB * 1024);

		public bool Cancel { get; set; }

        public void Put(Stream readstream, Uri serverUri, string p2vUri, NameValueCollection headers, long offset, long filesize, bool isChunked)
        {
            try
            {
                using (var http = DoHttp("PUT", serverUri, p2vUri, headers))
                {
                    if (http == null)
                        return;

                    if (isChunked)
                        SendChunkedData(http, readstream, offset, filesize);
                    else
                        SendData(http, readstream, filesize);
                }
            }
            catch (ReConnectException)
            {
                throw;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error("OVF.Tools.Http.Put: Exception: {0}", ex);
            }
        }

        public void Get(string filename, Uri serverUri, string p2vUri)
        {
            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
                {
                    Stream http = Connect(serverUri.Host, serverUri.Port);
                    String header = "GET " + p2vUri + " HTTP/1.0\r\n\r\n";
                    WriteLine(http, header);
                    String response = ReadLine(http);
                    int code = GetResultCode(response);
                    switch (code)
                    {
                        case 302:
                            string url = "";
                            while (true)
                            {
                                response = ReadLine(http);
                                if (response.StartsWith("Location: "))
                                    url = response.Substring(10);
                                if (response.Equals("\r\n") || response.Equals("")) break;
                            }
                            Uri redirect = new Uri(url.Trim());
                            http.Close();
                            Get(filename, redirect, p2vUri);
                            break;
                        default:
                            http.Close();
                            return;
                    }

                    while (true)
                    {
                        response = ReadLine(http);
                        if (response.Equals("\r\n") || response.Equals("")) break;
                    }
                    // Stream should be positioned after the headers
                }
            }
            catch (EndOfStreamException eof)
            {
                log.Debug("OVF.Tools.Http.Get::No Data: ", eof);
            }
            catch (Exception ex)
            {
                log.Error("OVF.Tools.Http.Get::Exception: ", ex);
            }
        }

        private Stream DoHttp(String method, Uri serverUri, string p2vUri, NameValueCollection headers)
        {
            log.DebugFormat("Connect To: {0}:{1}", serverUri.Host, serverUri.Port);
            Stream http = Connect(serverUri.Host, serverUri.Port);
            if (http == null)
                throw new Exception("HTTP == NULL");

            var httprequest = string.Format("{0} {1} http:/1.0\r\n", method, p2vUri);

            WriteLine(http, httprequest);
            WriteMIMEHeaders(http, headers);

            var response = ReadLine(http);
            int code = GetResultCode(response);

            if (code == 0)
            {
                log.Debug("No Return data at this time. Assuming Http status code 200.");
                code = 200;
            }
            
            log.DebugFormat("Http status code: {0}", code);

            if (code != 200)
            {
                http.Close();
                return null;
            }

            while (true)
            {
                response = ReadLine(http);
                if (response.Equals("\r\n") || response.Equals(""))
                    break;
            }
            return http;
        }

        internal void WriteMIMEHeaders(Stream http, NameValueCollection headers)
        {
            StringBuilder request = new StringBuilder();
            // Create the MIME Headers.
            if (headers != null && headers.Count > 0)
            {
                foreach (string key in headers.Keys)
                {
                    request.AppendFormat("{0}: {1}\r\n", key, headers[key]);
                }
            }
            request.Append("\r\n");
            WriteBuffer(http, request.ToString());
        }

        internal void SendTestData(Stream http)
        {
            byte[] testBuffer = new byte[KB];
            byte x = 0x70;
            byte y = 0;
            for (int i = 0; i < 1024; i++)
            {
                if ((i % 10) == 0)
                {
                    x = 0x70;
                    y = 0;
                }
                testBuffer[i] = (byte)(x + y++);
            }
            byte[] message = Encoding.UTF8.GetBytes(string.Format("{0}\r\n", testBuffer.Length));
            http.Write(message, 0, message.Length);
            http.Write(testBuffer, 0, testBuffer.Length);
            message = Encoding.UTF8.GetBytes(string.Format("\r\n0\r\n"));
            http.Write(message, 0, message.Length);
            http.Flush();
            log.DebugFormat("TestBuffer {0} bytes written", testBuffer.Length);
        }
        internal void SendFilesInDirectory(Stream http, string pathname)
        {
            byte[] message;

            foreach (string file in Directory.GetFiles(pathname))
            {
                byte[] block = File.ReadAllBytes(file);

                message = Encoding.UTF8.GetBytes(string.Format("{0}\r\n", block.Length));
                http.Write(message, 0, message.Length);
                http.Write(block, 0, block.Length);
                message = Encoding.UTF8.GetBytes(string.Format("\r\n"));
                http.Write(message, 0, message.Length);
                log.DebugFormat("Block [{0}] written", file);
            }
            message = Encoding.UTF8.GetBytes(string.Format("0\r\n"));
            http.Write(message, 0, message.Length);
            http.Flush();
            log.DebugFormat("Finished sending files in: {0}", pathname);
        }
        internal void SendChunkedData(Stream http, Stream filestream, long offset, long filesize)
        {
            byte Zero = 0x0;
            byte[] sizeblock;
            byte[] datablock = new byte[2 * MB];
            byte[] endblock = Encoding.UTF8.GetBytes(string.Format("\r\n"));
            byte[] finalblock = Encoding.UTF8.GetBytes(string.Format("0\r\n"));
            List<byte> fullblock = new List<byte>();

            long bytessent = offset;
            int i = 0;
            bool skipblock = false;
            OnUpdate(new XenOvfTransportEventArgs(TransportStep.SendData, "Disk Copy", (ulong)offset, (ulong)filesize));
            while (true)
            {
                // Form: chunked
                // size\r\ndata\r\n\r\nsize\r\n\r\n0\r\n
                // 1234
                // asdfasdfasdfasdfsdfd
                //
                // 1234
                // asdfasdfasdfasdfasdf
                //
                // 0
                //
                fullblock.Clear();
                bool IsAllZeros = true;
                int n = filestream.Read(datablock, 0, datablock.Length);
                if (n <= 0) break;
                sizeblock = Encoding.UTF8.GetBytes(string.Format("{0}\r\n", n));
                try
                {
                    for (int j = 0; j < datablock.Length; j++)
                    {
                        if (!datablock[j].Equals(Zero))
                        {
                            IsAllZeros = false;
                            break;
                        }
                    }
                    if (!IsAllZeros)
                    {
                        if (!skipblock)
                        {
                            fullblock.AddRange(sizeblock);
                            fullblock.AddRange(datablock);
                            fullblock.AddRange(endblock);
                            http.Write(fullblock.ToArray(), 0, fullblock.Count);
                            bytessent += n;
                            OnUpdate(new XenOvfTransportEventArgs(TransportStep.SendData, "Disk Copy", (ulong)bytessent, (ulong)filesize));
                            Thread.Sleep(100);
                        }
                        else
                        {
                            throw new ReConnectException($"Skipped Empty. Sent {bytessent} bytes.");
                        }
                    }
                    else
                    {
                        bytessent += n;
                        skipblock = true;
                    }
                }
                catch (Exception ex)
                {
                    log.Debug(ex, ex);
                    throw new ReConnectException($"Sent {bytessent} bytes.", ex);
                }

                log.DebugFormat(">>> {0} <<< Block {1} : Total {2} : Full {3} Skipped: {4}",
                                           i++,
                                           n.ToString("N0", CultureInfo.InvariantCulture),
                                           bytessent.ToString("N0", CultureInfo.InvariantCulture),
                                           filesize.ToString("N0", CultureInfo.InvariantCulture),
                                           skipblock);
                Thread.Sleep(100);
            }

            http.Write(finalblock, 0, finalblock.Length);
            http.Flush();
            OnUpdate(new XenOvfTransportEventArgs(TransportStep.SendData, "Disk Copy", (ulong)bytessent, (ulong)filesize));

            log.DebugFormat("=== {0} === Total {1} : Full {2}",
                                       i,
                                       bytessent.ToString("N0", CultureInfo.InvariantCulture),
                                       filesize.ToString("N0", CultureInfo.InvariantCulture));
        }

        internal void SendData(Stream http, Stream filestream, long filesize)
        {
            byte[] block = new byte[MB];
            ulong p = 0;
            int n = 0;
            OnUpdate(new XenOvfTransportEventArgs(TransportStep.SendData, "Disk Copy", 0, (ulong)filesize));
            while (true)
            {

                try
                {                    
                    n = filestream.Read(block, 0, block.Length);
                    if (n <= 0) break;
                    OnUpdate(new XenOvfTransportEventArgs(TransportStep.SendData, "Disk Copy", p, (ulong)filesize));
                    if (Cancel)
                    {
                        log.Warn("OVF.Tools.Http.SendData IMPORT CANCELED: resulting vm may be bad.");
                        throw new OperationCanceledException("Import canceled.");
                    }
                    http.Write(block, 0, n);
                    p += (ulong)n;
                    if (p >= (ulong)filesize)
                        break;
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    log.Error("OVF.Tools.Http.SendData failed.", ex);
                    throw;
                }
            }
            http.Flush();
            OnUpdate(new XenOvfTransportEventArgs(TransportStep.SendData, "Disk Copy", p, (ulong)filesize));
        }

        internal string ReadLine(Stream stream)
        {
            StringBuilder messageData = new StringBuilder();
            do
            {
                stream.ReadTimeout = 10000;
                try
                {
                    int i = stream.ReadByte();
                    if (i > 0)
                    {
                        char b = (char)i;
                        messageData.Append(b);
                        if (b == '\n') break;
                    }
                }
                catch
                {
                    break;
                }
            } while (true);
            return messageData.ToString();
        }
        internal void WriteLine(Stream stream, string line)
        {
            byte[] message = Encoding.UTF8.GetBytes(line);
            stream.Write(message, 0, message.Length);
            stream.Flush();
        }
        internal void WriteBuffer(Stream stream, string line)
        {
            byte[] message = Encoding.UTF8.GetBytes(line);
            stream.Write(message, 0, message.Length);
        }
        internal int GetResultCode(string line)
        {
            string[] bits = line.Split(new char[] { ' ' });
            if (bits.Length < 2) return 0;
            return Int32.Parse(bits[1]);
        }
        internal void MarshalResponse(Stream stream, Tag t)
        {
            MarshalBaseTypes.MarshalInt(stream, 4 + 4);
            MarshalTag(stream, Tag.Response);
            MarshalTag(stream, t);
        }
        internal void MarshalTag(Stream stream, Tag tag)
        {
            MarshalBaseTypes.MarshalInt(stream, (int)tag);
        }
        internal bool ValidateServerCertificate(
             object sender,
             X509Certificate certificate,
             X509Chain chain,
             SslPolicyErrors sslPolicyErrors)
        {
            // Do allow this client to communicate with unauthenticated servers.
            return true;
        }
        internal Stream Connect(String hostname, int port)
        {

            //tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            //tcpSocket.Blocking = true;
            //tcpSocket.NoDelay = true;
            //tcpSocket.LingerState = new LingerOption(true, 10);
            //tcpSocket.SendTimeout = 50000;
            //tcpSocket.Connect(hostname, port);
            //NetworkStream stream = new NetworkStream(tcpSocket);
            tcpClient = new TcpClient(hostname, port);
            tcpClient.NoDelay = true;
            tcpClient.LingerState = new LingerOption(true, 10);
            tcpClient.SendTimeout = 50000;
            NetworkStream stream = tcpClient.GetStream();
            if (port == 443)
            {
                // Create an SSL stream that will close the client's stream.
                SslStream sslStream = new SslStream(stream, false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                try
                {
                    sslStream.AuthenticateAsClient("", null, SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12, true);
                }
                catch
                {
                    if (tcpClient != null)
                        tcpClient.Close();
                    return null;
                }
                return sslStream;
            }
            return stream;
        }
    }

    [Serializable]
    public class ReConnectException : Exception
    {
        public ReConnectException()
            : base()
        {

        }
        public ReConnectException(string message)
            : base(message)
        {

        }
        public ReConnectException(string message, Exception ex)
            : base(message, ex)
        {

        }
    }
    public class MarshalBaseTypes
    {
        public static uint UnmarshalInt32(Stream stream)
        {
            uint a = (uint)stream.ReadByte();
            uint b = (uint)stream.ReadByte();
            uint c = (uint)stream.ReadByte();
            uint d = (uint)stream.ReadByte();
            //Console.WriteLine("a = " + a + " b = " + b + " c = " + c + " d = " + d);
            return (a << 0) | (b << 8) | (c << 16) | (d << 24);
        }
        public static void MarshalInt32(Stream stream, uint x)
        {
            uint mask = 0xff;
            stream.WriteByte((byte)((x >> 0) & mask));
            stream.WriteByte((byte)((x >> 8) & mask));
            stream.WriteByte((byte)((x >> 16) & mask));
            stream.WriteByte((byte)((x >> 24) & mask));
        }
        public static int UnmarshalInt(Stream stream)
        {
            return (int)UnmarshalInt32(stream);
        }
        public static void MarshalInt(Stream stream, int x)
        {
            MarshalInt32(stream, (uint)x);
        }
        public static byte[] Unmarshaln(Stream stream, uint n)
        {
            byte[] buffer = new byte[n];
            int toread = (int)n;
            int offset = 0;
            while (toread > 0)
            {
                int nread = stream.Read(buffer, offset, toread);
                offset = nread; toread -= nread;
            }
            return buffer;
        }
        public static string UnmarshalString(Stream stream)
        {
            uint length = UnmarshalInt32(stream);
            byte[] buffer = Unmarshaln(stream, length);
            Decoder decoder = Encoding.UTF8.GetDecoder();
            char[] chars = new char[decoder.GetCharCount(buffer, 0, (int)length)];
            decoder.GetChars(buffer, 0, (int)length, chars, 0);
            return new string(chars);
        }
        public static void MarshalString(Stream stream, string x)
        {
            MarshalInt(stream, x.Length);
            char[] c = x.ToCharArray();
            Encoder encoder = Encoding.UTF8.GetEncoder();
            byte[] bytes = new byte[encoder.GetByteCount(c, 0, c.Length, true)];
            encoder.GetBytes(c, 0, c.Length, bytes, 0, true);
            stream.Write(bytes, 0, bytes.Length);
        }
    }

}
