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
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using XenCenterLib;

namespace XenAdmin.Core
{
    class Clip
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        internal static EventHandler ClipboardChanged = null;

        internal static string ClipboardText = "";

        private static IntPtr registeredClipboardHandle = IntPtr.Zero;
        private static IntPtr nextClipboardViewer = IntPtr.Zero;
        private static bool processingChangeCBChain = false;
        private static bool processingDrawClipboard = false;

        private static long lastDrawClipboard = 0L;
        private static int drawClipboardCount = 0;

        private static volatile bool GettingClipboard = false;

        internal static void RegisterClipboardViewer()
        {
            Program.AssertOnEventThread();

            UnregisterClipboardViewer();

            // Register ourselves at the front of the chain.
            Win32.SetLastError(0);
            registeredClipboardHandle = Program.MainWindow.Handle;
            nextClipboardViewer = Win32.SetClipboardViewer(registeredClipboardHandle);

            int err = Marshal.GetLastWin32Error();
            if (err != 0)
            {
                log.ErrorFormat("SetClipboardViewer failed with code {0}: {1}", err, Win32.GetMessageString((uint)err));
                registeredClipboardHandle = IntPtr.Zero;
                nextClipboardViewer = IntPtr.Zero;
                return;
            }

            // Sanity check -- I can't see why this should ever happen, but xorg/cygwin checks.
            if (nextClipboardViewer == Program.MainWindow.Handle)
            {
                log.Error("SetClipboardViewer has given us our Handle back!");
                try
                {
                    Win32.ChangeClipboardChain(registeredClipboardHandle, IntPtr.Zero);
                }
                catch (Exception)
                {
                    log.WarnFormat("ChangeClipboardChain failed");
                }
                registeredClipboardHandle = IntPtr.Zero;
                nextClipboardViewer = IntPtr.Zero;
                return;
            }

            StartGetClipboard();
        }

        internal static void UnregisterClipboardViewer()
        {
            if (registeredClipboardHandle != IntPtr.Zero)
            {
                // Remove existing registration from chain, if any.
                Win32.SetLastError(0);
                Win32.ChangeClipboardChain(registeredClipboardHandle, nextClipboardViewer);
                int err = Marshal.GetLastWin32Error();
                if (err != 0)
                {
                    log.ErrorFormat("ChangeClipboardChain failed with code {0}: {1}", err, Win32.GetMessageString((uint)err));
                }

                registeredClipboardHandle = IntPtr.Zero;
                nextClipboardViewer = IntPtr.Zero;
            }
        }

        internal static void ProcessWMChangeCBChain(Message e)
        {
            Program.AssertOnEventThread();

            if (!processingChangeCBChain)
            {
                processingChangeCBChain = true;
                try
                {
                    if (nextClipboardViewer == e.WParam)
                    {
                        // The handle being removed (WParam) is the one that we've got.  We need to
                        // switch to the next handle in the chain (LParam).
                        nextClipboardViewer = e.LParam;
                    }
                    else
                    {
                        // Pass the message on -- it's not our link in the chain that is affected.
                        ForwardClipboardMessage(e);
                    }
                }
                finally
                {
                    processingChangeCBChain = false;
                }
            }
        }

        internal static void ProcessWMDrawClipboard(Message e)
        {
            Program.AssertOnEventThread();

            if (!processingDrawClipboard)
            {
                processingDrawClipboard = true;
                try
                {
                    // Windows XP Remote Desktop server is broken wrt the clipboard.
                    // If we receive 10 clipboard events in 500 msec then we move 
                    // ourselves back to the front of the chain.
                    // Without this, we receive endless WM_DRAWCLIPBOARD events
                    // after disconnecting and reconnecting the RDP session.
                    long now = DateTime.Now.Ticks;
                    if (now - lastDrawClipboard > 5000000)
                    {
                        drawClipboardCount = 0;
                        lastDrawClipboard = now;
                    }
                    else
                    {
                        drawClipboardCount++;
                        if (drawClipboardCount > 10)
                        {
                            drawClipboardCount = 0;
                            lastDrawClipboard = now;
                            RegisterClipboardViewer();
                        }
                    }

                    StartGetClipboard();

                    ForwardClipboardMessage(e);
                }
                finally
                {
                    processingDrawClipboard = false;
                }
            }
        }

        private static void ForwardClipboardMessage(Message e)
        {
            Program.AssertOnEventThread();

            if (nextClipboardViewer == IntPtr.Zero)
                return;

            Win32.SetLastError(0);
            Win32.SendMessage(nextClipboardViewer, e.Msg, e.WParam, e.LParam);
            int err = Marshal.GetLastWin32Error();
            if (err != 0)
            {
                log.ErrorFormat("SendMessage({0}) failed with code {1}: {2}",
                    Win32.GetWindowsMessageName(e.Msg), err, Win32.GetMessageString((uint)err));
            }
        }

        private static void StartGetClipboard()
        {
            Program.AssertOnEventThread();

            if (!GettingClipboard)
            {
                GettingClipboard = true;
                Thread clipboardThread = new Thread(GetClipboard);
                clipboardThread.SetApartmentState(ApartmentState.STA);
                clipboardThread.IsBackground = true;
                clipboardThread.Start();
            }
        }

        private static void GetClipboard()
        {
            Program.AssertOffEventThread();

            try
            {
                if (Clipboard.ContainsText())
                {
                    string s = Clipboard.GetText();
                    if (s != ClipboardText)
                    {
                        ClipboardText = s;
                        Program.Invoke(Program.MainWindow, OnClipboardChanged);
                    }
                }
            }
            catch
            {
            }
            GettingClipboard = false;
        }

        private static void OnClipboardChanged()
        {
            Program.AssertOnEventThread();

            if (ClipboardChanged != null)
                ClipboardChanged(null, null);
        }

        internal static void SetClipboardText(string text)
        {
            Program.AssertOnEventThread();

            try
            {
                Clipboard.SetText(text);
            }
            catch (Exception ex)
            {
                log.Error("Exception while trying to set clipboard text.", ex);
            }
        }
    }
}
