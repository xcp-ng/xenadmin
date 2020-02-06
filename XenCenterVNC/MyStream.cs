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
using System.Text;

namespace DotNetVnc
{
    public class MyStream
    {
        private const int MAX_STRING_LENGTH = 1 << 16;
        private const int MAX_CLIPBOARD_SIZE = 1024 * 1024; 

        private readonly Stream inStream;
        private readonly Stream outStream;
        private readonly byte[] readbuf = new byte[4];
        private readonly byte[] writebuf = new byte[4];
        private readonly byte[] zerobuf = new byte[4];

        public MyStream(Stream stream)
        {
            this.outStream = new BufferedStream(stream, 1024);
            this.inStream = new BufferedStream(stream, 65536);
        }

        public void writePadding(int n)
        {
            outStream.Write(zerobuf, 0, n);
        }

        public void writeFlag(bool v)
        {
            outStream.WriteByte(v ? (byte)1 : (byte)0);
        }

        public void writeInt8(int v)
        {
            outStream.WriteByte((byte)v);
        }

        public void writeInt16(int v)
        {
            writebuf[0] = (byte)((v >> 8) & 0xff);
            writebuf[1] = (byte)( v       & 0xff);
            outStream.Write(writebuf, 0, 2);
        }

        public void writeInt32(int v)
        {
            writebuf[0] = (byte)((v >> 24) & 0xff);
            writebuf[1] = (byte)((v >> 16) & 0xff);
            writebuf[2] = (byte)((v >> 8)  & 0xff);
            writebuf[3] = (byte)( v        & 0xff);
            outStream.Write(writebuf, 0, 4);
        }

        public void writeString(String s)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            var size = Math.Min(bytes.Length, MAX_CLIPBOARD_SIZE);
            writeInt32(size);
            outStream.Write(bytes, 0, size);
        }

        public void readPadding(int n)
        {
            inStream.Read(readbuf, 0, n);
        }

        public void readFully(byte[] b, int off, int len)
        {
            int n = 0;
            while (n < len)
            {
                int count = inStream.Read(b, off + n, len - n);
                if (count <= 0)
                {
                    throw new EndOfStreamException();
                }
                n += count;
            }
        }

        public bool readFlag()
        {
            return readCard8() == 0 ? false : true;
        }

        public int readCard8()
        {
            int v = inStream.ReadByte();
            if (v < 0)
            {
                throw new EndOfStreamException();
            }
            return v;
        }

        public int readCard16()
        {
            int b1 = readCard8();
            int b0 = readCard8();
            return (short)((b1 << 8) | b0);
        }

        public int readCard32()
        {
            int b3 = readCard8();
            int b2 = readCard8();
            int b1 = readCard8();
            int b0 = readCard8();
            return (b3 << 24) | (b2 << 16) | (b1 << 8) | b0;
        }

        public String readString()
        {
            int length = readCard32();
            if (length < 0 || length >= MAX_STRING_LENGTH)
            {
                throw new IOException("Invalid string length: " + length);
            }
            byte[] buffer = new byte[length];
            readFully(buffer, 0, length);
            char[] chars = new char[length];
            Encoding.ASCII.GetDecoder().GetChars(buffer, 0, length, chars, 0);
            return new String(chars);
        }

        public void Flush()
        {
            outStream.Flush();
        }

        public void Write(byte[] data, int offset, int count)
        {
            outStream.Write(data, offset, count);
        }

        public void Close()
        {
            outStream.Close();
        }
    }
}
