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
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace XenCenterLib
{
    public class NamedPipes
    {
        [Flags]
        public enum FileAccess : uint
        {
            GenericRead = 0x80000000,
            GenericWrite = 0x40000000,
            GenericExecute = 0x20000000,
            GenericAll = 0x10000000
        }

        [Flags]
        public enum FileShare : uint
        {
            None = 0x00000000,
            /// <summary>
            /// Enables subsequent open operations on an object to request read access.
            /// Otherwise, other processes cannot open the object if they request read access.
            /// If this flag is not specified, but the object has been opened for read access, the function fails.
            /// </summary>
            Read = 0x00000001,
            /// <summary>
            /// Enables subsequent open operations on an object to request write access.
            /// Otherwise, other processes cannot open the object if they request write access.
            /// If this flag is not specified, but the object has been opened for write access, the function fails.
            /// </summary>
            Write = 0x00000002,
            /// <summary>
            /// Enables subsequent open operations on an object to request delete access.
            /// Otherwise, other processes cannot open the object if they request delete access.
            /// If this flag is not specified, but the object has been opened for delete access, the function fails.
            /// </summary>
            Delete = 0x00000004
        }

        public enum FileMode : uint
        {
            /// <summary>
            /// Creates a new file. The function fails if a specified file exists.
            /// </summary>
            CreateNew = 1,
            /// <summary>
            /// Creates a new file, always.
            /// If a file exists, the function overwrites the file, clears the existing attributes, combines the specified file attributes,
            /// and flags with FILE_ATTRIBUTE_ARCHIVE, but does not set the security descriptor that the SECURITY_ATTRIBUTES structure specifies.
            /// </summary>
            CreateAlways = 2,
            /// <summary>
            /// Opens a file. The function fails if the file does not exist.
            /// </summary>
            OpenExisting = 3,
            /// <summary>
            /// Opens a file, always.
            /// If a file does not exist, the function creates a file as if dwCreationDisposition is CREATE_NEW.
            /// </summary>
            OpenAlways = 4,
            /// <summary>
            /// Opens a file and truncates it so that its size is 0 (zero) bytes. The function fails if the file does not exist.
            /// The calling process must open the file with the GENERIC_WRITE access right.
            /// </summary>
            TruncateExisting = 5
        }

        [Flags]
        public enum FileAttributes : uint
        {
            Readonly = 0x00000001,
            Hidden = 0x00000002,
            System = 0x00000004,
            Directory = 0x00000010,
            Archive = 0x00000020,
            Device = 0x00000040,
            Normal = 0x00000080,
            Temporary = 0x00000100,
            SparseFile = 0x00000200,
            ReparsePoint = 0x00000400,
            Compressed = 0x00000800,
            Offline = 0x00001000,
            NotContentIndexed = 0x00002000,
            Encrypted = 0x00004000,
            Write_Through = 0x80000000,
            Overlapped = 0x40000000,
            NoBuffering = 0x20000000,
            RandomAccess = 0x10000000,
            SequentialScan = 0x08000000,
            DeleteOnClose = 0x04000000,
            BackupSemantics = 0x02000000,
            PosixSemantics = 0x01000000,
            OpenReparsePoint = 0x00200000,
            OpenNoRecall = 0x00100000,
            FirstPipeInstance = 0x00080000
        }

        /// <summary>
        /// Duplex pipe access.
        /// </summary>
        private const uint PIPE_ACCESS_DUPLEX = 0x00000003;

        /// <summary>
        /// Pipe blocking mode.
        /// </summary>
        private const uint PIPE_WAIT = 0x00000000;

        /// <summary>
        /// Pipe read mode of type Message.
        /// </summary>
        private const uint PIPE_READMODE_MESSAGE = 0x00000002;

        /// <summary>
        /// Message pipe type.
        /// </summary>
        private const uint PIPE_TYPE_MESSAGE = 0x00000004;

        /// <summary>
        /// Unlimited server pipe instances.
        /// </summary>
        private const uint PIPE_UNLIMITED_INSTANCES = 255;

        /// <summary>
        /// Waits indefinitely when connecting to a pipe.
        /// </summary>
        private const uint NMPWAIT_WAIT_FOREVER = 0xffffffff;

        /// <summary>
        /// Invalid operating system handle.
        /// </summary>
        private const int INVALID_HANDLE_VALUE = -1;

        /// <summary>
        /// More data is available.
        /// </summary>
        private const int ERROR_MORE_DATA = 234;

        /// <summary>
        /// There is a process on other end of the pipe.
        /// </summary>
        private const int ERROR_PIPE_CONNECTED = 535;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CreateNamedPipe(String pipeName, uint openMode, uint pipeMode,
            uint maxInstances, uint outputBufferSize, uint inputBufferSize, uint timeoutInterval,
            IntPtr pipeSecurityDescriptor);

        /// <summary>
        /// Enables a named pipe server process to wait for a client 
        /// process to connect to an instance of a named pipe.
        /// </summary>
        /// <param name="hHandle">Handle to the server end of a named pipe instance.</param>
        /// <param name="lpOverlapped">Pointer to an 
        /// <see cref="AppModule.NamedPipes.Overlapped">Overlapped</see> object.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ConnectNamedPipe(IntPtr namedPipeHandle, ref NativeOverlapped overlapped);

        /// <summary>
        /// Disconnects the server end of a named pipe instance from a client process.
        /// </summary>
        /// <param name="hHandle">Handle to an instance of a named pipe.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DisconnectNamedPipe(IntPtr hHandle);

        /// <summary>
        /// Reads data from a file, starting at the position indicated by the file pointer.
        /// </summary>
        /// <param name="hHandle">Handle to the file to be read.</param>
        /// <param name="lpBuffer">Pointer to the buffer that receives the data read from the file.</param>
        /// <param name="nNumberOfBytesToRead">Number of bytes to be read from the file.</param>
        /// <param name="lpNumberOfBytesRead">Pointer to the variable that receives the number of bytes read.</param>
        /// <param name="lpOverlapped">Pointer to an 
        /// <see cref="AppModule.NamedPipes.Overlapped">Overlapped</see> object.</param>
        /// <returns>The ReadFile function returns when one of the following 
        /// conditions is met: a write operation completes on the write end of 
        /// the pipe, the number of bytes requested has been read, or an error occurs.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ReadFile(IntPtr fileHandle, [Out] byte[] dataBuffer, uint nNumberOfBytesToRead,
            out uint lpNumberOfBytesRead, [In] ref NativeOverlapped overlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteFile(IntPtr fileHandle, [Out] byte[] dataBuffer, uint nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten, [In] ref NativeOverlapped overlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool PeekNamedPipe(IntPtr handle,  [Out] byte[] buffer, uint nBufferSize,
            out uint bytesRead, out uint bytesAvail, out uint BytesLeftThisMessage);

        [DllImport("kernel32.dll")]
        private static extern bool CallNamedPipe(string lpNamedPipeName, byte[] lpInBuffer,
           uint nInBufferSize, [Out] byte[] lpOutBuffer, uint nOutBufferSize,
           out uint lpBytesRead, uint nTimeOut);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        private const string STOP_LISTENING_MSG = "stop-listening-on-pipe";

        public class Pipe
        {
            private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            public readonly IntPtr Handle;
            public EventHandler<PipeReadEventArgs> Read;
            private Thread pipeThread;
            private readonly string pipePath;
            private volatile bool run;

            /// <exception cref="Win32Exception">If creating the pipe failed for any reason.</exception>
            public Pipe(string path)
            {
                this.pipePath = path;
                Handle = CreateNamedPipe(pipePath, PIPE_ACCESS_DUPLEX,
                    PIPE_TYPE_MESSAGE | PIPE_READMODE_MESSAGE | PIPE_WAIT,
                    PIPE_UNLIMITED_INSTANCES, 512, 512, NMPWAIT_WAIT_FOREVER, IntPtr.Zero);
                if (Handle.ToInt32() == INVALID_HANDLE_VALUE)
                {
                    // Throw last win32 exception
                    throw new Win32Exception();
                }
            }

            /// <summary>
            /// Starts reading from this pipe on a new background thread. This call returns immediately.
            /// Whenever data is read from the pipe, the Read event is fired.
            /// </summary>
            public void BeginRead()
            {
                if (pipeThread == null)
                {
                    run = true;
                    pipeThread = new Thread((ThreadStart)BackgroundPipeThread);
                    pipeThread.Name = "Named pipe thread";
                    pipeThread.IsBackground = true;
                    pipeThread.Start();
                }
            }

            /// <summary>
            /// May block for up to 30 seconds.
            /// </summary>
            public void Disconnect()
            {
                // Now connect to our own NamedPipe and sent a disconnect message
                if (pipeThread != null)
                {
                    byte[] msg = Encoding.UTF8.GetBytes(STOP_LISTENING_MSG);
                    byte[] rcv = new byte[0];
                    UInt32 bytesRead;
                    UInt32 timeout = 30 * 1000;

                    CallNamedPipe(pipePath, msg, (UInt32)msg.Length, rcv, (UInt32)rcv.Length, out bytesRead, timeout);
                }
            }

            private void BackgroundPipeThread()
            {
                while (run)
                {
                    try
                    {
                        NativeOverlapped overlapped = new NativeOverlapped();
                        if (!ConnectNamedPipe(Handle, ref overlapped))
                        {
                            Win32Exception exn = new Win32Exception();
                            if (exn.NativeErrorCode == ERROR_PIPE_CONNECTED)
                            {
                                // This is OK. It simply means a remote process has connected to the
                                // named pipe since we did the CreateNamedPipe.
                            }
                            else
                            {
                                throw new Win32Exception("ConnectNamedPipe failed", exn);
                            }
                        }

                        ProcessPipeMessage();

                        if (!DisconnectNamedPipe(Handle))
                        {
                            throw new Win32Exception("DisconnectNamedPipe failed", new Win32Exception());
                        }
                    }
                    catch (Exception exn)
                    {
                        log.Error("Error in named pipe thread.", exn);
                        // Sanity: pause after errors to prevent massive log spamming/CPU drain
                        // in the event of an infinite error loop
                        Thread.Sleep(1000);
                    }
                }

                // Close pipe handle to destroy it
                if (CloseHandle(Handle))
                    log.Debug("Closed NamedPipe handle");
                else
                    log.Warn("CloseHandle() in NamedPipes failed", new Win32Exception());

                log.Debug("NamedPipe thread exited");
            }

            private void ProcessPipeMessage()
            {
                NativeOverlapped overlapped = new NativeOverlapped();
                byte[] readMessage;

                using (MemoryStream ms = new MemoryStream())
                {
                    while (true)
                    {
                        // Loop as follows:
                        // Peek at the pipe to see how much data is available
                        // Read the data and append it to the buffer
                        // If we get ERROR_MORE_DATA, repeat
                        UInt32 bytesRead, bytesAvailable, bytesLeft;

                        // First peek into the pipe to see how much data is waiting.
                        byte[] peekBuf = new byte[0];
                        if (!PeekNamedPipe(Handle, peekBuf, (UInt32)peekBuf.Length, out bytesRead, out bytesAvailable, out bytesLeft))
                        {
                            throw new Win32Exception(
                                string.Format("PeekNamedPipe failed. bytesRead={0} bytesAvailable={1} bytesLeft={2}",
                                bytesRead, bytesAvailable, bytesLeft),
                                new Win32Exception());
                        }

                        // Sanity check: throw away message if it is > 1 MB
                        if (ms.Length + bytesLeft > 1024 * 1024)
                        {
                            throw new Exception("Indecently large message sent into named pipe: rejecting.");
                        }

                        // Now allocate a buffer of the correct size and read in the message.
                        byte[] readBuf = new byte[bytesAvailable];
                        if (!ReadFile(Handle, readBuf, (UInt32)readBuf.Length, out bytesRead, ref overlapped))
                        {
                            Win32Exception exn = new Win32Exception();
                            if (exn.NativeErrorCode == ERROR_MORE_DATA)
                            {
                                // The peek may have looked into the pipe before the write operation
                                // had completed, and so reported a message size before the whole
                                // message was sent. In this case we need to go round the loop again
                                // with a larger buffer.
                                ms.Write(readBuf, 0, (int)bytesRead);
                                continue;
                            }
                            else
                            {
                                throw new Win32Exception(
                                    string.Format("ReadFile failed. readBuf.Length={0} bytesRead={1}",
                                    readBuf.Length, bytesRead), exn);
                            }
                        }
                        else
                        {
                            ms.Write(readBuf, 0, (int)bytesRead);
                            break;
                        }
                    }
                    readMessage = ms.ToArray();
                }

                // Now perform a zero-byte write. This causes the CallNamedPipe call to
                // return successfully in the splash screen.
                UInt32 bytesWritten;
                byte[] toWrite = new byte[0];
                if (!WriteFile(Handle, toWrite, (UInt32)toWrite.Length, out bytesWritten, ref overlapped))
                {
                    throw new Win32Exception(
                        string.Format("WriteFile failed. toWrite.Length={0} bytesWritten={1}",
                        toWrite.Length, bytesWritten),
                        new Win32Exception());
                }

                PipeReadEventArgs e = new PipeReadEventArgs(readMessage);

                if (e.Message == STOP_LISTENING_MSG)
                {
                    log.Debug("NamedPipe thread was told to stop listening");
                    run = false;
                    return;
                }

                // Now inform any listeners of the received data.
                if (Read != null)
                {
                    Read(null, e);
                }
            }
        }

        public class PipeReadEventArgs : EventArgs
        {
            public readonly string Message;

            public PipeReadEventArgs(byte[] bytes)
            {
                Message = Encoding.Unicode.GetString(bytes);
            }
        }
    }
}
