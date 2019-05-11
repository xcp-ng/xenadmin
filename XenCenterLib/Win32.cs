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
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace XenCenterLib
{
    public class Win32
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [DllImport("kernel32.dll")]
        public static extern void SetLastError(uint dwErrCode);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        /// From George Shepherd's Windows Forms FAQ:
        /// http://www.syncfusion.com/FAQ/WindowsForms/FAQ_c73c.aspx
        /// 
        /// uiFlags: 0 - Count of GDI objects 
        /// uiFlags: 1 - Count of USER objects 
        /// - Win32 GDI objects (pens, brushes, fonts, palettes, regions, device contexts, bitmap headers) 
        /// - Win32 USER objects: 
        ///      - WIN32 resources (accelerator tables, bitmap resources, dialog box templates, font resources, menu resources, raw data resources, string table entries, message table entries, cursors/icons) 
        /// - Other USER objects (windows, menus) 
        /// 
        [DllImport("User32")]
        extern public static int GetGuiResources(IntPtr hProcess, int uiFlags);

        public static int GetGuiResourcesGDICount(IntPtr processHandle)
        {
            return GetGuiResources(processHandle, 0);
        }

        public static int GetGuiResourcesUserCount(IntPtr processHandle)
        {
            return GetGuiResources(processHandle, 1);
        }

        public const int TO_UNICODE_BUFFER_SIZE = 64;
        [DllImport("user32.dll")]
        public extern static int ToUnicode(uint wVirtKey, uint wScanCode, IntPtr lpKeyState,
                                           [Out, MarshalAs(UnmanagedType.LPWStr, SizeConst = TO_UNICODE_BUFFER_SIZE)] StringBuilder pwszBuff,
                                           int cchBuff, uint wFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FlushFileBuffers(Microsoft.Win32.SafeHandles.SafeFileHandle hFile);

        [DllImport("user32.dll")]
        public extern static bool GetKeyboardState(IntPtr lpKeyState);

        // So we can flash minimized windows in the taskbar to alert the user.
        // See http://pinvoke.net/default.aspx/user32.FlashWindowEx and
        // http://blogs.msdn.com/hippietim/archive/2006/03/28/563094.aspx
        [DllImport("user32.dll")]
        public static extern Int32 FlashWindowEx(ref FLASHWINFO pwfi);

        [StructLayout(LayoutKind.Sequential)]
        public struct FLASHWINFO
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public UInt32 dwFlags;
            public UInt32 uCount;
            public UInt32 dwTimeout;
        }

        /// <summary>
        /// Stop flashing. The system restores the window to its original state.
        /// </summary>
        public const UInt32 FLASHW_STOP = 0;
        /// <summary> 
        /// Flash the window caption.
        /// </summary>
        public const UInt32 FLASHW_CAPTION = 1;
        /// <summary>
        /// Flash the taskbar button.
        /// </summary>
        public const UInt32 FLASHW_TRAY = 2;
        /// <summary>
        /// Flash both the window caption and taskbar button = FLASHW_CAPTION | FLASHW_TRAY flags.
        /// </summary>
        public const UInt32 FLASHW_ALL = FLASHW_CAPTION | FLASHW_TRAY;
        /// <summary>
        /// Flash continuously, until the FLASHW_STOP flag is set.
        /// </summary>
        public const UInt32 FLASHW_TIMER = 4;
        /// <summary>
        /// Flash continuously until the window comes to the foreground.
        /// </summary>
        public const UInt32 FLASHW_TIMERNOFG = 12;

        /// <summary>
        /// Flash the taskbar button for the given window.
        /// </summary>
        public static void FlashTaskbar(IntPtr hwnd)
        {
            FLASHWINFO fwi = new FLASHWINFO();
            fwi.cbSize = (UInt32)Marshal.SizeOf(typeof(FLASHWINFO));
            fwi.dwFlags = FLASHW_TRAY;
            fwi.dwTimeout = 0; // The default, which is the caret blink rate
            fwi.uCount = 3;
            fwi.hwnd = hwnd;
            FlashWindowEx(ref fwi);
        }

        /// <summary>
        /// Stop the given window flashing.
        /// </summary>
        public static void StopFlashing(IntPtr hwnd)
        {
            FLASHWINFO fwi = new FLASHWINFO();
            fwi.cbSize = (UInt32)Marshal.SizeOf(typeof(FLASHWINFO));
            fwi.dwFlags = FLASHW_STOP;
            fwi.dwTimeout = 0;
            fwi.uCount = 0;
            fwi.hwnd = hwnd;
            FlashWindowEx(ref fwi);
        }

        [DllImport("user32.dll")]
        public extern static IntPtr GetClipboardViewer();

        [DllImport("user32.dll", SetLastError = true)]
        public extern static IntPtr SetClipboardViewer(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public extern static bool ChangeClipboardChain(IntPtr hWnd, IntPtr hWndNext);

        /// <summary>
        /// There is not enough space on the disk. See winerror.h.
        /// </summary>
        public const UInt32 ERROR_DISK_FULL = 112;

        public const int CBN_CLOSEUP = 8;
        public const int EN_KILLFOCUS = 512;

        public const int WM_DESTROY = 2;
        public const int WM_ACTIVATE = 6;
        /// <summary>
        /// Sent when a control gains focus: see VNCGraphicsClient.
        /// </summary>
        public const int WM_SETFOCUS = 7;
        public const int WM_GETTEXT = 13;
        public const int WM_GETTEXTLENGTH = 14;
        public const int WM_PAINT = 15;
        /// <summary>
        /// Prevent Redraw background on paint
        /// </summary>
        public const int WM_ERASEBKGND = 20;
        public const int WM_ACTIVATEAPP = 28;
        public const int WM_SETCURSOR = 32;
        public const int WM_MOUSEACTIVATE = 33;
        public const int WM_WINDOWPOSCHANGING = 70;
        public const int WM_WINDOWPOSCHANGED = 71;
        public const int WM_NOTIFY = 78;
        public const int WM_NCHITTEST = 132;
        public const int WM_NCPAINT = 133;
        public const int WM_NCACTIVATE = 134;
        public const int WM_TIMER = 275;
        public const int WM_VSCROLL = 277;

        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;

        // Mouse Hooks
        public const int HC_ACTION = 0;
        public const int WH_MOUSE_LL = 14;
        public const int WM_MOUSEMOVE = 0x200;
        public const int WM_LBUTTONDOWN = 0x201;
        public const int WM_LBUTTONUP = 0x202;
        public const int WM_LBUTTONDBLCLK = 0x203;
        public const int WM_RBUTTONDOWN = 0x204;
        public const int WM_RBUTTONUP = 0x205;
        public const int WM_RBUTTONDBLCLK = 0x206;
        public const int WM_MBUTTONDOWN = 0x207;
        public const int WM_MBUTTONUP = 0x208;
        public const int WM_MBUTTONDBLCLK = 0x209;
        public const int WM_MOUSEWHEEL = 0x20A;
        public const int WM_MOUSEHWHEEL = 0x20E;

        public const int WM_PARENTNOTIFY = 0x210;

        public const int WM_INITDIALOG = 0x110;
        public const int WM_SETFONT = 0x0030;
        public const int WM_GETFONT = 0x0031;
        public const int WM_SETTEXT = 0x000c;

        public const int WM_DRAWCLIPBOARD = 776;
        public const int WM_CHANGECBCHAIN = 781;

        public const int WM_HSCROLL = 0x114;

        public const int OCM_DRAWITEM = 8235;
        public const int OCM_NOTIFY = 8270;

        public const int OFN_ENABLEHOOK = 0x00000020;
        public const int OFN_EXPLORER = 0x00080000;
        public const int OFN_FILEMUSTEXIST = 0x00001000;
        public const int OFN_HIDEREADONLY = 0x00000004;
        public const int OFN_CREATEPROMPT = 0x00002000;
        public const int OFN_NOTESTFILECREATE = 0x00010000;
        public const int OFN_OVERWRITEPROMPT = 0x00000002;
        public const int OFN_PATHMUSTEXIST = 0x00000800;
        public const int OFN_SHOWHELP = 0x00000010;

        public const int SWP_NOSIZE = 0x0001;
        public const int SWP_NOMOVE = 0x0002;
        public const int SWP_NOZORDER = 0x0004;

        public const int BS_AUTOCHECKBOX = 0x0003;
        public const int BS_PUSHBUTTON = 0x0000;

        public const int BM_SETCHECK = 0x00f1;
        public const int BM_GETCHECK = 0x00f0;

        public const int BST_CHECKED = 0x0001;
        public const int BST_UNCHECKED = 0x0000;

        public const uint WS_VISIBLE = 0x10000000;
        public const uint WS_CHILD = 0x40000000;
        public const uint WS_TABSTOP = 0x00010000;

        public const int CDN_FILEOK = -606;
        public const int CDN_HELP = -605;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr CreateWindowEx(uint dwExStyle, string lpClassName, string lpWindowName, uint dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public struct POINT
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll")]
        public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [DllImport("user32.dll")]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool SetWindowText(int hWnd, string lpString);

        [DllImport("user32.dll")]
        public static extern bool DestroyWindow(IntPtr hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hwnd);

        /// <summary>
        /// Get the _HResult field from the given exception.
        /// </summary>
        public static int GetHResult(Exception exn)
        {
            /* Since there are no useful subclasses of IOException
             * (e.g. DiskFullException), in order to detect a disk full error we have to extract the
             * (hidden) HRESULT code from the exception using reflection and manually compare the
             * error code bits with the error as defined in winerror.h.
             */
            try
            {
                int hresult = (int)exn.GetType().GetField("_HResult",
                    BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetValue(exn);
                // The error code is stored in just the lower 16 bits
                return hresult & 0xFFFF;
            }
            catch
            {
                return 0;
            }
        }

        // Thank you, pinvoke.net!
        // http://www.pinvoke.net/default.aspx/kernel32/FormatMessage.html
        [DllImport("Kernel32.dll", SetLastError = true)]
        static extern uint FormatMessage(uint dwFlags, IntPtr lpSource,
            uint dwMessageId, uint dwLanguageId, ref IntPtr lpBuffer,
            uint nSize, IntPtr pArguments);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr LocalFree(IntPtr hMem);

        const uint FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
        const uint FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
        const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

        public static string GetMessageString(uint message)
        {
            IntPtr lpMsgBuf = IntPtr.Zero;

            uint dwChars = FormatMessage(
                FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
                IntPtr.Zero,
                message,
                0, // Default language
                ref lpMsgBuf,
                0,
                IntPtr.Zero);

            if (dwChars == 0)
            {
                return "";
            }

            string sRet = Marshal.PtrToStringAnsi(lpMsgBuf);
            lpMsgBuf = LocalFree(lpMsgBuf);
            return sRet;
        }

        public static string GetWindowsMessageName(int msg)
        {
            Type t = typeof(Win32);
            foreach (FieldInfo f in t.GetFields())
            {
                object i = f.GetValue(null);
                if (i is Int32 && ((Int32)i) == msg)
                {
                    return f.Name;
                }
            }
            return msg.ToString();
        }

        public delegate bool EnumUILanguagesProc(string lpUILanguageString, IntPtr lParam);

        [DllImport("kernel32.dll")]
        public static extern bool EnumUILanguages(EnumUILanguagesProc pUILanguageEnumProc,
           uint dwFlags, IntPtr lParam);

        #region Window scrolling functions

        [StructLayout(LayoutKind.Sequential)]
        public struct ScrollInfo
        {
            public uint cbSize;
            public uint fMask;
            public int nMin;
            public int nMax;
            public uint nPage;
            public int nPos;
            public int nTrackPos;
        }

        public enum ScrollInfoMask
        {
            SIF_RANGE = 0x1,
            SIF_PAGE = 0x2,
            SIF_POS = 0x4,
            SIF_DISABLENOSCROLL = 0x8,
            SIF_TRACKPOS = 0x10,
            SIF_ALL = SIF_RANGE + SIF_PAGE + SIF_POS + SIF_TRACKPOS
        }

        public enum ScrollBarCommands
        {
            SB_LINEUP = 0,
            SB_LINELEFT = 0,
            SB_LINEDOWN = 1,
            SB_LINERIGHT = 1,
            SB_PAGEUP = 2,
            SB_PAGELEFT = 2,
            SB_PAGEDOWN = 3,
            SB_PAGERIGHT = 3,
            SB_THUMBPOSITION = 4,
            SB_THUMBTRACK = 5,
            SB_TOP = 6,
            SB_LEFT = 6,
            SB_BOTTOM = 7,
            SB_RIGHT = 7,
            SB_ENDSCROLL = 8
        }

        public enum ScrollBarConstants
        {
            /// <summary>
            /// The horizontal scroll bar of the specified window
            /// </summary>
            SB_HORZ = 0,
            /// <summary>
            /// The vertical scroll bar of the specified window
            /// </summary>
            SB_VERT = 1,
            /// <summary>
            /// A scroll bar control
            /// </summary>
            SB_CTL = 2,
            /// <summary>
            /// The horizontal and vertical scroll bars of the specified window
            /// </summary>
            SB_BOTH = 3
        }

        public enum ScrollState
        {
            AutoScrolling = 0x0001,
            HScrollVisible = 0x0002,
            VScrollVisible = 0x0004,
            UserHasScrolled = 0x0008,
            FullDrag = 0x0010
        }

        /// <summary>
        /// See http://msdn2.microsoft.com/en-us/library/e14hhbe6(VS.80).aspx
        /// </summary>
        public const int SB_THUMBTRACK = 5;

        /// <summary>
        /// See http://msdn2.microsoft.com/en-us/library/bb787583.aspx and
        /// http://pinvoke.net/default.aspx/user32/GetScrollInfo.html
        /// </summary>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetScrollInfo(IntPtr hWnd, int n, ref ScrollInfo lpScrollInfo);

        [DllImport("user32.dll")]
        public static extern int SetScrollInfo(IntPtr hwnd, int fnBar, [In] ref ScrollInfo lpsi, bool fRedraw);

        #endregion

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        #region Disk space functions

        /// <summary>
        /// Will return null if the disk parameters could not be determined.
        /// </summary>
        /// <param name="path">An absolute path</param>
        /// <returns></returns>
        public static DiskSpaceInfo GetDiskSpaceInfo(string path)
        {
            try
            {
                string DriveLetter = Path.GetPathRoot(path).TrimEnd(new char[] { '\\' });
                System.Management.ManagementObject o = new System.Management.ManagementObject(
                    string.Format("Win32_LogicalDisk.DeviceID=\"{0}\"", DriveLetter));
                string fsType = o.Properties["FileSystem"].Value.ToString();
                bool isFAT = (fsType == "FAT" || fsType == "FAT32");
                UInt64 freeBytes = UInt64.Parse(o.Properties["FreeSpace"].Value.ToString());
                UInt64 totalBytes = UInt64.Parse(o.Properties["Size"].Value.ToString());
                return new DiskSpaceInfo(freeBytes, totalBytes, isFAT);
            }
            catch (Exception exn)
            {
                log.Warn(exn, exn);
                return null;
            }
        }

        public class DiskSpaceInfo
        {
            public readonly UInt64 FreeBytesAvailable;
            public readonly UInt64 TotalBytes;
            public readonly bool IsFAT;

            public DiskSpaceInfo(UInt64 freeBytesAvailable, UInt64 totalBytes, bool isFAT)
            {
                FreeBytesAvailable = freeBytesAvailable;
                TotalBytes = totalBytes;
                IsFAT = isFAT;
            }
        }

        public const int CP_NOCLOSE_BUTTON = 0x200;

        public const int GWL_WNDPROC = -4;

        public delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr newWndProc);

        [DllImport("user32.dll")]
        public static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public const int LB_ITEMFROMPOINT = 425;

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        #endregion

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hwnd);
        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        // pinvoke.net
        [DllImport("psapi.dll")]
        public static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, [In] [MarshalAs(UnmanagedType.U4)] int nSize);

        /*
         * ToolHelpHandle and related methods are based upon
         * http://blogs.msdn.com/jasonz/archive/2007/05/11/code-sample-is-your-process-using-the-silverlight-clr.aspx
         * and http://www.csharpfriends.com/Forums/ShowPost.aspx?PostID=27395.
         */

        public class ToolHelpHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            private ToolHelpHandle() : base(true)
            {
            }

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
            override protected bool ReleaseHandle()
            {
                return CloseHandle(handle);
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static public extern bool CloseHandle(IntPtr hHandle);

        [DllImport("kernel32.dll")]
        static public extern bool Process32First(ToolHelpHandle hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll")]
        static public extern bool Process32Next(ToolHelpHandle hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        static public extern ToolHelpHandle CreateToolhelp32Snapshot(SnapshotFlags dwFlags, uint th32ProcessID);

        [Flags]
        public enum SnapshotFlags : uint
        {
            HeapList = 0x00000001,
            Process = 0x00000002,
            Thread = 0x00000004,
            Module = 0x00000008,
            Module32 = 0x00000010,
            Inherit = 0x80000000,
            All = 0x0000001F
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct PROCESSENTRY32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExeFile;
        }

        [Flags]
        public enum TOKEN_ACCESS : uint
        {
            TOKEN_QUERY = 0x0008
        };

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool OpenProcessToken(IntPtr ProcessHandle, TOKEN_ACCESS DesiredAccess, out IntPtr TokenHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public extern static bool QueryPerformanceCounter(out long x);
        [DllImport("kernel32.dll", SetLastError = true)]
        public extern static bool QueryPerformanceFrequency(out long x);

        public delegate IntPtr OFNHookProcDelegate(IntPtr hdlg, int msg, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct OPENFILENAME
        {
            public int lStructSize;
            public IntPtr hwndOwner;
            public int hInstance;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpstrFilter;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpstrCustomFilter;
            public int nMaxCustFilter;
            public int nFilterIndex;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpstrFile;
            public int nMaxFile;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpstrFileTitle;
            public int nMaxFileTitle;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpstrInitialDir;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpstrTitle;
            public int Flags;
            public short nFileOffset;
            public short nFileExtension;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpstrDefExt;
            public int lCustData;
            public OFNHookProcDelegate lpfnHook;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpTemplateName;
            //only if on nt 5.0 or higher
            public int pvReserved;
            public int dwReserved;
            public int FlagsEx;
        }

        [DllImport("Comdlg32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetSaveFileName(ref OPENFILENAME lpofn);

        [DllImport("Comdlg32.dll")]
        public static extern int CommDlgExtendedError();

        /// <summary>
        /// Extended message header for WM_NOTIFY
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct NMHEADER
        {
            public NMHDR hdr;
            public int iItem;
            public int iButton;
            public IntPtr pitem;
        }

        /// <summary>
        /// message header for WM_NOTIFY
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct NMHDR
        {
            public IntPtr HwndFrom;
            public IntPtr IdFrom;
            public int code;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMUPDOWN
        {
            public NMHDR hdr;
            public int pos;
            public int delta;
        }

        public const int S_OK = unchecked((int)0x00000000);
        public const int E_ACCESSDENIED = unchecked((int)0x80070005);
        public const int INET_E_DEFAULT_ACTION = unchecked((int)0x800C0011);

        [ComImport, Guid("6d5140c1-7436-11ce-8034-00aa006009fa"),
         InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
         ComVisible(false)]
        public interface IServiceProvider
        {
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int QueryService(ref Guid guidService, ref Guid riid, out IntPtr ppvObject);
        }

        private const string _IID_IAuthenticate = "79eac9d0-baf9-11ce-8c82-00aa004ba90b";
        public static readonly Guid IID_IAuthenticate = new Guid(_IID_IAuthenticate);
        [ComImport, Guid(_IID_IAuthenticate),
         InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
         ComVisible(false)]
        public interface IAuthenticate
        {
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int Authenticate(ref IntPtr phwnd, ref IntPtr pszUsername, ref IntPtr pszPassword);
        }
    }
}
