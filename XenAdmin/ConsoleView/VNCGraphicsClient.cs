/* Copyright (c) Cloud Software Group, Inc. 
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using DotNetVnc;
using XenAdmin.Core;
using System.Linq;
using XenCenterLib;

namespace XenAdmin.ConsoleView
{
    public class VNCGraphicsClient : UserControl, IVNCGraphicsClient, IRemoteConsole
    {
        public const int BORDER_PADDING = 5;
        public const int BORDER_WIDTH = 1;

        private const int MOUSE_EVENTS_BEFORE_UPDATE = 2;
        private const int MOUSE_EVENTS_DROPPED = 5; // should this be proportional to bandwidth?
        
        private const int WM_KEYDOWN = 0x100;
        private const int WM_SYSKEYDOWN = 0x104;

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public event Action<object, Exception> ErrorOccurred;
        public event EventHandler ConnectionSuccess;
        private VNCStream _vncStream;

        /// <summary>
        /// connected implies that vncStream is non-null and ready to be used.
        /// </summary>
        private volatile bool _connected;
        public bool Connected => _connected;

        /// <summary>
        /// terminated means that we have been told to disconnect.  connected starts off false, becomes
        /// true when a connection is made, and then becomes false again when connection goes away for
        /// whatever reason.  In contrast, terminated may become true even before a connection has been
        /// made.
        /// </summary>
        private volatile bool _terminated;
        public bool Terminated => _terminated;

        private CustomCursor _remoteCursor;
        private readonly CustomCursor _localCursor = new CustomCursor(Images.StaticImages.vnc_local_cursor, 2, 2);
        private bool _cursorOver;

        /// <summary>
        /// This field is locked before any drawing is done through backGraphics or frontGraphics.
        /// It is set in the constructor below, and then there is a delicate handover during desktop
        /// resize to keep this safe.
        /// </summary>
        private Bitmap _backBuffer;

        /// <summary>
        /// The contents of the backbuffer are interesting if the backbuffer has been drawn to,
        /// and we will present them to the user even after disconnect (if they are not interesting
        /// we just show a blank rectangle instead).
        /// </summary>
        private volatile bool _backBufferInteresting;

        /// <summary>
        /// A Graphics object onto backBuffer.  Access to this field must always be locked under backBuffer.
        /// This field will be set to null in Dispose.
        /// </summary>
        private Graphics _backGraphics;

        /// <summary>
        /// Graphics on actual screen.  Access to this field must always be locked under backBuffer.
        /// This field will be set to null in Dispose.
        /// </summary>
        private Graphics _frontGraphics;

        private readonly object _mouseEventLock = new object();

        private Rectangle _damage = Rectangle.Empty;

        private float _scale;
        private float _dx;
        private float _dy;
        private float _oldDx;
        private float _oldDy;
        private int _bump;
        private bool _scaling;
        private bool _sendScanCodes = true;
        private bool _useSource;

        private static bool _handlingChange;
        private bool _updateClipboardOnFocus;
        private string _clipboardStash = string.Empty;
        private int _currentMouseState;
        /*
         * We're going to track how many moves we have between screen updates 
         * to stop the mouse running away with long updates.
         * Unfortunately we sometimes seem to send moves and get no update
         * back, so we cap the number of 'dropped' moves.
         */
        private volatile int _mouseMoved;
        private volatile int _mouseNotMoved;

        private MouseEventArgs _pending ;
        private int _pendingState;
        private int _lastState;

        private Set<Keys> _pressedKeys = new Set<Keys>();
        private Set<int> _pressedScans = new Set<int>();

        private bool _modifierKeyPressedAlone;
        private bool _helperIsPaused = true;
        private bool _displayBorder = true;
        private bool _altGrReleaseSent;

        public bool UseSource
        {
            set => _useSource = value;
        }

        private bool TalkingToVNCTerm => !_sendScanCodes && _useSource;

        public string UUID => SourceVM.uuid;

        public XenAPI.VM SourceVM { get; set; }

        public string VmName => SourceVM.name_label;

        public object Console;

        public event EventHandler DesktopResized;

        public bool UseQemuExtKeyEncoding { set; private get; }

        public VNCGraphicsClient(ContainerControl parent)
        {
            Program.AssertOnEventThread();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                        | ControlStyles.UserPaint
                        | ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Opaque, false);

            _frontGraphics = CreateGraphics();
            SetupGraphicsOptions(_frontGraphics);
            _backBuffer = new Bitmap(640, 480, _frontGraphics);
            _backGraphics = Graphics.FromImage(_backBuffer);
            SetupGraphicsOptions(_backGraphics);
            using (SolidBrush backBrush = new SolidBrush(BackColor))
            {
                _backGraphics.FillRectangle(backBrush, 0, 0, 640, 480);
            }
            DesktopSize = new Size(640, 480);

#pragma warning disable 0219
            IntPtr _ = Handle;
#pragma warning restore 0219

            Clip.ClipboardChanged += ClipboardChanged;
            parent.Controls.Add(this);
        }

        private static void SetupGraphicsOptions(Graphics g)
        {
            g.CompositingQuality = CompositingQuality.AssumeLinear;
            g.InterpolationMode = InterpolationMode.Default;
            g.SmoothingMode = SmoothingMode.Default;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!disposing)
                    return;

                Clip.ClipboardChanged -= ClipboardChanged;

                Disconnect();

                lock (_backBuffer)
                {
                    _frontGraphics.Dispose();
                    _backGraphics.Dispose();
                    _backBuffer.Dispose();
                    _backGraphics = null;
                    _frontGraphics = null;
                }

                if (_remoteCursor != null)
                {
                    _remoteCursor.Dispose();
                    _remoteCursor = null;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        internal void Connect(Stream stream, char[] password)
        {
            Program.AssertOnEventThread();

            _vncStream = new VNCStream(this, stream, _helperIsPaused);
            _vncStream.ErrorOccurred += OnError;
            _vncStream.ConnectionSuccess += vncStream_ConnectionSuccess;
            _connected = true;
            _vncStream.Connect(password);
        }

        private void vncStream_ConnectionSuccess(object sender, EventArgs e)
        {
            Program.AssertOffEventThread();

            // Set the remote clipboard based on the current contents.
            Program.Invoke(this, (EventHandler)ClipboardChanged, null, null);

            if (ConnectionSuccess != null)
                Program.Invoke(this, ConnectionSuccess, sender, e);
        }

        private void OnError(object sender, Exception e)
        {
            Program.AssertOffEventThread();
            System.Diagnostics.Debug.Assert(sender == _vncStream); // Please see to CA-236844 if this assertion fails

            if (sender != _vncStream)
                return;
            
            _connected = false;
            if (ErrorOccurred != null)
                ErrorOccurred(this, e);
        }

        private void Disconnect()
        {
            _connected = false;
            _terminated = true;
            if (_vncStream != null)
            {
                _vncStream.ErrorOccurred -= OnError;
                _vncStream.ConnectionSuccess -= vncStream_ConnectionSuccess;
            } 
            VNCStream s = _vncStream;
            _vncStream = null;
            s?.Close();
        }

        private bool RedirectingClipboard()
        {
            return Properties.Settings.Default.ClipboardAndPrinterRedirection;
        }

        private void ClipboardChanged(object sender, EventArgs args)
        {
            Program.AssertOnEventThread();

            if (!RedirectingClipboard())
                return;

            try
            {
                if (!_handlingChange && _connected)
                {
                    if (Focused)
                        SetConsoleClipboard();
                    else
                        _updateClipboardOnFocus = true;
                }
            }
            catch (IOException)
            {
                // The server's gone away -- that's fine.
            }
            catch (Exception exn)
            {
                Log.Warn(exn, exn);
                // Nothing more we can do with this.
            }
        }

        private void SetConsoleClipboard()
        {
            try
            {
                _handlingChange = true;
                string text = Clip.ClipboardText;
                if (TalkingToVNCTerm)
                    text = text.Replace("\r\n", "\n");
                _vncStream.ClientCutText(text);
                _updateClipboardOnFocus = false;
            }
            finally
            {
                _handlingChange = false;
            }
        }

        /// <summary>
        /// Records damage to the screen.
        /// </summary>
        private void Damage(int x, int y, int width, int height)
        {
            Program.AssertOffEventThread();

            Rectangle r = new Rectangle(x, y, width, height);

            if (_scaling)
            {
                r.Inflate(_bump, _bump); // Fix for scaling issues
            }

            if (_damage.IsEmpty)
            {
                _damage = r;
            }
            else
            {
                _damage = Rectangle.Union(_damage, r);
            }
        }

        private void CheckAssertion(bool assertion, String message, params Object[] args)
        {
            if (!assertion)
            {
                Log.Error("Bad VNC server message: " + String.Format(message, args));
            }
        }

        #region Functions called by vncStream to update display

        public void ClientDrawImage(Bitmap image, int x, int y, int width, int height)
        {
            Program.AssertOffEventThread();

            Damage(x, y, width, height);
            lock (_backBuffer)
            {
                try
                {
                    if (_backGraphics != null)
                        _backGraphics.DrawImageUnscaled(image, x, y);
                }
                catch (Exception e)
                {
                    // We seem to be very occasionally getting weird exception from this.  These are probably due to
                    // bad server messages, so we can just log and ignore them

                    Log.Error("Error drawing image from server", e);

                    try
                    {
                        CheckAssertion(image.Width == width, "Width {0} != {1}", image.Width, width);
                        CheckAssertion(image.Height == height, "Height {0} != {1}", image.Height, height);
                        CheckAssertion(x < DesktopSize.Width, "x {0} >= {1}", x, DesktopSize.Width);
                        CheckAssertion(y < DesktopSize.Height, "y {0} >= {1}", y, DesktopSize.Height);
                        CheckAssertion(x + width <= DesktopSize.Width, "x + width {0} + {1} > {2}", x, width, DesktopSize.Width);
                        CheckAssertion(y + height <= DesktopSize.Height, "y + height {0} + {1} > {2}", y, height, DesktopSize.Height);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }

        #endregion

        #region Custom Cursor handling

        // Taken from 
        // http://www.codeproject.com/cs/miscctrl/DragDropTreeview.asp?df=100&forumid=84437&exp=0&select=1838138#xx1838138xx

        [StructLayout(LayoutKind.Sequential)]
        public struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconIndirect(ref IconInfo iconinfo);

        [DllImport("user32.dll")]
        public static extern bool DestroyIcon(IntPtr hIcon);

        private class CustomCursor : IDisposable
        {
            private IntPtr _handle = IntPtr.Zero;

            internal CustomCursor(Bitmap bitmap, int x, int y)
            {
                var iconInfo = new IconInfo
                {
                    fIcon = false,
                    xHotspot = x,
                    yHotspot = y,
                    hbmMask = bitmap.GetHbitmap(),
                    hbmColor = bitmap.GetHbitmap()
                };

                if (Type.GetType("Mono.Runtime") == null){
                    _handle = CreateIconIndirect(ref iconInfo);
                    Cursor = new Cursor(_handle);
                }
                else
                {
                    // Doesn't work under Mono
                }
            }

            ~CustomCursor()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
                Dispose(true);
            }

            protected virtual void Dispose(bool disposing)
            {
                try
                {
                    if (_handle != IntPtr.Zero)
                        DestroyIcon(_handle);
                }
                catch
                {
                    // ignored
                }
                finally
                {
                    _handle = IntPtr.Zero;
                }
            }

            public Cursor Cursor { get; }
        }

        public void ClientSetCursor(Bitmap image, int x, int y, int width, int height)
        {
            Program.AssertOffEventThread();

            _remoteCursor?.Dispose();
            _remoteCursor = new CustomCursor(image, x, y);

            Program.Invoke(this, () =>
            {
                if (_cursorOver)
                    Cursor = _remoteCursor.Cursor;
            });
        }

        #endregion

        public void ClientCopyRectangle(int x, int y, int width, int height, int dx, int dy)
        {
            Program.AssertOffEventThread();

            Damage(dx, dy, width, height);
            lock (_backBuffer)
            {
                if (_backGraphics != null)
                    GraphicsUtils.copyRect(_backBuffer, x, y, width, height, _backGraphics, dx, dy);
            }
        }

        public void ClientFillRectangle(int x, int y, int width, int height, Color color)
        {
            Program.AssertOffEventThread();

            Damage(x, y, width, height);
            lock (_backBuffer)
            {
                if (_backGraphics != null)
                {
                    using (SolidBrush backBrush = new SolidBrush(color))
                    {
                        _backGraphics.FillRectangle(backBrush, x, y, width, height);
                    }
                }
            }
        }

        public void ClientFrameBufferUpdate()
        {
            Program.AssertOffEventThread();

            lock (_backBuffer)
            {
                try
                {
                    if (!_damage.IsEmpty)
                    {
                        if (_frontGraphics != null)
                            _frontGraphics.DrawImage(_backBuffer, _damage, _damage, GraphicsUnit.Pixel);
                        _damage = Rectangle.Empty;
                    }

                    _backBufferInteresting = true;
                }
                catch
                {
                    // ignored
                }
            }

            /*
             * If there is a pending mouse event, send it.
             * Also reset the mouse event counters
             */
            lock (_mouseEventLock)
            {
                if (_pending != null)
                {
                    MouseEvent(_pendingState, _pending.X, _pending.Y);
                    _pending = null;
                }

                _mouseMoved = 0;
                _mouseNotMoved = 0;
            }
        }

        public void ClientBell()
        {
        }

        public void ClientCutText(String text)
        {
            Program.AssertOffEventThread();

            if (TalkingToVNCTerm)
            {
                // Lets translate from unix line endings to windows ones...
                _clipboardStash = Regex.Replace(text, "\r?\n", "\r\n");
            }
            else
            {
                if (!RedirectingClipboard())
                    return;
                Program.Invoke(this, () =>
                {
                    if (Clipboard.ContainsText() && Clipboard.GetText() == text)
                        return;
                    Clip.SetClipboardText(text);
                });
            }
        }

        public void ClientDesktopSize(int width, int height)
        {
            Program.AssertOffEventThread();

            // Cannot do an invoke with a locked back buffer, as it may event thread
            // (onPaint) tried to lock back buffer as well - therefore deadlock.

            Program.Invoke(this, () =>
            {
                Bitmap oldBackBuffer;
                lock (_backBuffer)
                {
                    if (width <= 0)
                        width = 1;
                    if (height <= 0)
                        height = 1;

                    if (width == DesktopSize.Width &&
                        height == DesktopSize.Height)
                    {
                        return;
                    }

                    oldBackBuffer = _backBuffer;

                    _backGraphics.Dispose();
                    _frontGraphics.Dispose();

                    _frontGraphics = CreateGraphics();
                    SetupGraphicsOptions(_frontGraphics);

                    Bitmap newBackBuffer = new Bitmap(width, height, _frontGraphics);
                    _backGraphics = Graphics.FromImage(newBackBuffer);
                    SetupGraphicsOptions(_backGraphics);

                    using (SolidBrush backBrush = new SolidBrush(BackColor))
                    {
                        _backGraphics.FillRectangle(backBrush, 0, 0, width, height);
                    }

                    DesktopSize = new Size(width, height);

                    // Now that backGraphics is valid, we can switch backBuffer.  We're relying on backBuffer
                    // as our lock.
                    _backBuffer = newBackBuffer;
                }

                lock (_backBuffer)
                {
                    SetupScaling();
                }

                Invalidate();
                Update();

                oldBackBuffer.Dispose();

                OnDesktopResized();
            });
        }

        private void OnDesktopResized()
        {
            Program.AssertOnEventThread();

            if (DesktopResized != null)
                DesktopResized(this, null);
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);

            if (!_scaling && se.OldValue != se.NewValue)
            {
                if (se.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                {
                    _dx = -1 * se.NewValue + (_displayBorder ? BORDER_PADDING : 0);
                    lock (_backBuffer)
                    {
                        if (_frontGraphics != null)
                        {
                            _frontGraphics.ResetTransform();
                            _frontGraphics.TranslateTransform(_dx, _dy);
                        }
                    }
                }

                if (se.ScrollOrientation == ScrollOrientation.VerticalScroll)
                {
                    _dy = (-1 * se.NewValue) + (_displayBorder ? BORDER_PADDING : 0);
                    lock (_backBuffer)
                    {
                        if (_frontGraphics != null)
                        {
                            _frontGraphics.ResetTransform();
                            _frontGraphics.TranslateTransform(_dx, _dy);
                        }
                    }
                }

                this.Refresh();
            }
        }

        private void MyPaintBackground(Graphics g, int x, int y, int width, int height)
        {
            Rectangle r = new Rectangle(x, y, width, height);

            // Hack pulled from Control.cs:PaintTransparentBackground
            if (Application.RenderWithVisualStyles)
            {
                ButtonRenderer.DrawParentBackground(g, r, this);
            }
            else
            {
                using (Brush backBrush = new SolidBrush(BackColor))
                {
                    g.FillRectangle(backBrush, r);
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //do nothing
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Program.AssertOnEventThread();

            if (_connected || _backBufferInteresting)
            {
                /*
                 * Draw the background by working out the surrounding bars
                 * as opposed to just one big black rect, to reduce flicker.
                 */

                // Draw two vertical bars at either end

                int w = (int)_dx;

                Graphics g = e.Graphics;

                lock (_backBuffer)
                {
                    if (_frontGraphics != null)
                        _frontGraphics.DrawImageUnscaled(_backBuffer, 0, 0);
                }

                if (w > 0)
                    MyPaintBackground(g, 0, 0, w + 1, ClientSize.Height);

                w = (int)(ClientSize.Width - _dx - (DesktopSize.Width * _scale)) + 1;

                if (w > 0)
                    MyPaintBackground(g, ClientSize.Width - w, 0, w, ClientSize.Height);

                // Draw two horizontal bars at top and bottom

                int h = (int)_dy;

                if (h > 0)
                    MyPaintBackground(g, 0, 0, Size.Width, h + 1);

                h = (int)(ClientSize.Height - _dy - (DesktopSize.Height * _scale)) + 1;

                if (h > 0)
                    MyPaintBackground(g, 0, ClientSize.Height - h, Size.Width, h);
            }
            else
            {
                base.OnPaintBackground(e);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            Program.AssertOnEventThread();
            base.OnResize(e);

            /*
             * Need to recreate the front graphics or we
             * get weird clipping problems.
             */
            lock (_backBuffer)
            {
                if (_frontGraphics != null)
                    _frontGraphics.Dispose();
                _frontGraphics = CreateGraphics();
                SetupGraphicsOptions(_frontGraphics);
                SetupScaling();

                // We don't need to redraw, as the window will repaint us
            }
        }

        private void MouseEvent(int state, int x, int y)
        {
            DoIfConnected(() =>
            {
                if (TalkingToVNCTerm && _lastState == 4 && state == 0)
                {
                    ShowPopupMenu(x, y);
                }
                else if (_scaling)
                {
                    _vncStream.PointerEvent(state, (int)((x - _dx) / _scale), (int)((y - _dy) / _scale));
                }
                else
                {
                    _vncStream.PointerEvent(state, x - (int)_dx, y - (int)_dy);
                }

                _lastState = state;
            });
        }

        private void ShowPopupMenu(int x, int y)
        {
            ToolStripDropDownMenu popupMenu = new ToolStripDropDownMenu();

            ToolStripMenuItem copyItem = new ToolStripMenuItem(Messages.COPY);

            copyItem.Image = Images.StaticImages.copy_16;
            copyItem.Click += copyItem_Click;

            popupMenu.Items.Add(copyItem);
            if (SourceVM != null && SourceVM.power_state == XenAPI.vm_power_state.Running)
            {
                ToolStripMenuItem pasteItem = new ToolStripMenuItem(Messages.PASTE);
                pasteItem.Image = Images.StaticImages.paste_16;
                pasteItem.Click += pasteItem_Click;

                popupMenu.Items.Add(pasteItem);
            }
            popupMenu.Show(this, x, y);
        }

        private void copyItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.Assert(TalkingToVNCTerm);
            Program.AssertOnEventThread();
            if (_clipboardStash != "")
            {
                Clip.SetClipboardText(_clipboardStash);
            }
        }

        private void pasteItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.Assert(TalkingToVNCTerm);
            Program.AssertOnEventThread();

            if (Clipboard.ContainsText())
            {
                _vncStream.ClientCutText(Clipboard.GetText().Replace("\r\n", "\n"));
                MouseEvent(2, 0, 0);
                MouseEvent(0, 0, 0);
            }
        }

        private void DisableMenuShortcuts()
        {
            Program.MainWindow.MenuShortcutsEnabled = false;
        }

        private void EnableMenuShortcuts()
        {
            Program.MainWindow.MenuShortcutsEnabled = true;
        }

        protected override void OnGotFocus(EventArgs e)
        {
            Program.AssertOnEventThread();
            base.OnGotFocus(e);

            DisableMenuShortcuts();

            if (_sendScanCodes)
            {
                if (Type.GetType("Mono.Runtime") == null)
                {
                    InterceptKeys.grabKeys(KeyScan, false);
                }
                else
                {
                    // TODO: The code in `XenAdmin/VNC/KeyMap.cs`
                    // must be crossplatform to enable this feature
                }
            }

            if (_updateClipboardOnFocus)
                SetConsoleClipboard();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            Program.AssertOnEventThread();
            base.OnLostFocus(e);

            EnableMenuShortcuts();

            if (Type.GetType("Mono.Runtime") == null)
            {
                InterceptKeys.releaseKeys();
            }
            else
            {
                // TODO: The code in `XenAdmin/VNC/KeyMap.cs`
                // must be crossplatform to enable this feature
            }

            _cursorOver = false;

            Cursor = Cursors.Default;

            //Release any held keys
            DoIfConnected(() =>
            {
                foreach (Keys key in _pressedKeys)
                {
                    // This won't release composite key events atm.
                    int sym = KeyMap.translateKey(ConsoleKeyHandler.GetSimpleKey(key));
                    if (sym > 0)
                        _vncStream.keyCodeEvent(false, sym);
                }

                foreach (int key in _pressedScans)
                {
                    _vncStream.keyScanEvent(false, key, -1, UseQemuExtKeyEncoding);
                }
            });

            _pressedKeys = new Set<Keys>();
            _pressedScans = new Set<int>();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Only hide the mouse when over the actual screen,
            // some parts of this control will be window blinds

            if (Focused && Connected)
            {
                int top = (int)_dy;
                int left = (int)_dx;
                int bottom = (int)((DesktopSize.Height * _scale) + top);
                int right = (int)((DesktopSize.Width * _scale) + left);

                if (e.X > left && e.X < right
                 && e.Y > top && e.Y < bottom)
                {
                    _cursorOver = true;

                    if (_remoteCursor == null)
                        Cursor = _localCursor.Cursor;
                    else
                        Cursor = _remoteCursor.Cursor;

                    lock (_mouseEventLock)
                    {
                        if (_mouseMoved < MOUSE_EVENTS_BEFORE_UPDATE)
                        {
                            _mouseMoved++;

                            MouseEvent(_currentMouseState, e.X, e.Y);
                        }
                        else if (_mouseNotMoved > MOUSE_EVENTS_DROPPED)
                        {
                            _mouseMoved = 0;
                            _mouseNotMoved = 0;
                        }
                        else
                        {
                            _mouseNotMoved++;

                            _pendingState = _currentMouseState;
                            _pending = e;
                        }
                    }
                }
                else
                {
                    _cursorOver = false;
                    Cursor = Cursors.Default;
                }
            }
        }

        /// <summary>
        /// We're not going to buffer any clicks etc.
        /// Don't forget to clear any pending moves.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            //added this line to track mouse clicks for automatic switch to RDP
            //if there are any issues with focus start looking here
            base.OnMouseDown(e);
            Focus();

            _pending = null;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    _currentMouseState |= 1;
                    break;
                case MouseButtons.Right:
                    _currentMouseState |= 4;
                    break;
                case MouseButtons.Middle:
                    _currentMouseState |= 2;
                    break;
            }
            MouseEvent(_currentMouseState, e.X, e.Y);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _pending = null;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    _currentMouseState &= ~1;
                    break;
                case MouseButtons.Right:
                    _currentMouseState &= ~4;
                    break;
                case MouseButtons.Middle:
                    _currentMouseState &= ~2;
                    break;
            }
            MouseEvent(_currentMouseState, e.X, e.Y);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (Focused)
            {
                DoIfConnected(() => _vncStream.PointerWheelEvent((int)((e.X - _dx) / _scale), (int)((e.Y - _dy) / _scale),
                    e.Delta * -SystemInformation.MouseWheelScrollLines / 120));

            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            Cursor = Cursors.Default;
            _cursorOver = false;
            base.OnMouseLeave(e);
            _currentMouseState = 0;
            _mouseMoved = 0;
        }

        protected override bool ProcessTabKey(bool forward)
        {
            return true;
        }

        private void DoIfConnected(MethodInvoker methodInvoker)
        {
            if (_connected)
            {
                try
                {
                    methodInvoker();
                }
                catch (IOException)
                {
                    // The server's gone away -- that's fine.
                }
                catch (Exception exn)
                {
                    Log.Warn(exn, exn);
                    // Nothing more we can do with this.
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool down = ((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN));

            Keys key = keyData;

            if ((key & Keys.Control) == Keys.Control)
                key &= ~Keys.Control;

            if ((key & Keys.Alt) == Keys.Alt)
                key &= ~Keys.Alt;

            if ((key & Keys.Shift) == Keys.Shift)
                key &= ~Keys.Shift;

            // use TranslateKeyMessage to identify if Left or Right modifier keys have been pressed/released
            Keys extKey = ConsoleKeyHandler.TranslateKeyMessage(msg);

            return KeySym(down, key, extKey);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            // we cannot identify if Left or Right modifier keys have been pressed
            e.Handled = KeySym(false, e.KeyCode, e.KeyCode);
        }

        private void SendAltGrKeySym(bool pressed)
        {
            KeySym_(pressed, Keys.ControlKey);
            KeySym_(pressed, Keys.Menu);
        }

        private void HandleAltGrKeySym(bool pressed, Keys key)
        {
            if (pressed)
            {
                if (_pressedKeys.Count > 2 && key != Keys.ControlKey && key != Keys.Menu)
                {
                    bool isAltGrPressed = _pressedKeys.Where(
                        extKey => ConsoleKeyHandler.GetSimpleKey(extKey) == Keys.ControlKey ||
                                  ConsoleKeyHandler.GetSimpleKey(extKey) == Keys.Menu).ToList().Count == 2;

                    if (isAltGrPressed &&
                        (KeyMap.IsMapped(key) && _altGrReleaseSent || !KeyMap.IsMapped(key) && !_altGrReleaseSent))
                    {
                        SendAltGrKeySym(_altGrReleaseSent);
                        _altGrReleaseSent = !_altGrReleaseSent;
                    }
                }
            }
            else
            {
                if (key == Keys.ControlKey || key == Keys.Menu)
                    _altGrReleaseSent = false;
            }
        }

        private bool KeySym(bool pressed, Keys key, Keys extendedKey)
        {
            if (_sendScanCodes)
                return true;

            if (KeyHandler.handleExtras(pressed, _pressedKeys, KeyHandler.ExtraKeys, extendedKey, KeyHandler.ModifierKeys, ref _modifierKeyPressedAlone))
            {
                if (!pressed && _modifierKeyPressedAlone)
                {
                    // send key up anyway
                    _modifierKeyPressedAlone = false;
                    return KeySym_(false, key);
                }
                Parent.Focus();
                return true;
            }

            HandleAltGrKeySym(pressed, key);

            // on keyup, try to remove extended keys (i.e. LControlKey, LControlKey, RShiftKey, LShiftKey, RMenu, LMenu)
            // we need to do this here, because we cannot otherwise distinguish between Left and Right modifier keys on KeyUp
            if (!pressed)
            {
                List<Keys> extendedKeys = ConsoleKeyHandler.GetExtendedKeys(key);
                foreach (var k in extendedKeys)
                {
                    _pressedKeys.Remove(k);
                }
            }

            return KeySym_(pressed, key);
        }

        private bool KeySym_(bool pressed, Keys key)
        {
            int keysym = KeyMap.translateKey(key);

            if (keysym > 0)
            {
                DoIfConnected(() => _vncStream.keyCodeEvent(pressed, keysym));
                return true;
            }

            return false;
        }

        private void KeyScan(bool pressed, int scanCode, int keySym)
        {
            if (KeyHandler.handleExtras(pressed, _pressedScans, KeyHandler.ExtraScans, scanCode, KeyHandler.ModifierScans, ref _modifierKeyPressedAlone))
            {
                if (!pressed && _modifierKeyPressedAlone)
                {
                    // send key up anyway
                    _modifierKeyPressedAlone = false;
                    KeyScan_(false, scanCode, keySym);
                    return;
                }
                this.Focus();
                return;
            }

            KeyScan_(pressed, scanCode, keySym);
        }

        private void KeyScan_(bool pressed, int scanCode, int keySym = -1)
        {
            DoIfConnected(() => _vncStream.keyScanEvent(pressed, scanCode, keySym, UseQemuExtKeyEncoding));
        }

        #region IRemoteConsole implementation
        
        public ConsoleKeyHandler KeyHandler { get; set; }

        public Control ConsoleControl => this;

        public void Activate()
        {
            Select();
        }

        public void DisconnectAndDispose()
        {
            Disconnect();
            Dispose();
        }

        public void Pause()
        {
            _helperIsPaused = true;
            _vncStream?.Pause();
        }

        public void UnPause()
        {
            _helperIsPaused = false;
            _vncStream?.UnPause();
        }

        public void SendCAD()
        {
            if (_sendScanCodes)
            {
                KeyScan_(true, ConsoleKeyHandler.CTRL_SCAN);
                KeyScan_(true, ConsoleKeyHandler.ALT_SCAN);
                KeyScan_(true, ConsoleKeyHandler.DEL_SCAN);

                KeyScan_(false, ConsoleKeyHandler.CTRL_SCAN);
                KeyScan_(false, ConsoleKeyHandler.ALT_SCAN);
                KeyScan_(false, ConsoleKeyHandler.DEL_SCAN);
            }
            else
            {
                KeySym_(true, Keys.ControlKey);
                KeySym_(true, Keys.Menu);
                KeySym_(true, Keys.Delete);

                KeySym_(false, Keys.ControlKey);
                KeySym_(false, Keys.Menu);
                KeySym_(false, Keys.Delete);
            }
        }

        public Image Snapshot()
        {
            while (_vncStream == null)
                Thread.Sleep(100);

            _vncStream.UnPause(true); //request full update

            lock (_vncStream.updateMonitor)
            {
                Monitor.Wait(_vncStream.updateMonitor, 1000);
            }

            lock (_backBuffer)
            {
                Image image = new Bitmap(_backBuffer);
                return image;
            }
        }    

        public bool SendScanCodes
        {
            set
            {
                Program.AssertOnEventThread();
                if (_sendScanCodes == value)
                    return;

                Log.InfoFormat("VNCGraphicsClient.SetSendScanCodes newSendScanCodes={0}", value);

                if (!value)
                {
                    if (Type.GetType("Mono.Runtime") == null)
                    {
                        InterceptKeys.releaseKeys();
                    }
                    else
                    {
                        // TODO: The code in `XenAdmin/VNC/KeyMap.cs`
                        // must be crossplatform to enable this feature
                    }
                }
                else if (Focused)
                {
                    if (Type.GetType("Mono.Runtime") == null)
                    {
                        InterceptKeys.grabKeys(KeyScan, false);
                    }
                    else
                    {
                        // TODO: The code in `XenAdmin/VNC/KeyMap.cs`
                        // must be crossplatform to enable this feature
                    }
                }

                _sendScanCodes = value;
            }
        }

        public bool Scaling
        {
            get
            {
                Program.AssertOnEventThread();
                return _scaling;
            }
            set
            {
                Program.AssertOnEventThread();

                if (_scaling == value)
                    return;

                // it going to scaling, quickly save the old dx, dy values
                if (value)
                {
                    _oldDx = _dx;
                    _oldDy = _dy;
                }

                lock (_backBuffer)
                {
                    _scaling = value;
                    SetupScaling();
                }
                Invalidate();
            }
        }
        /// <summary>
        /// Invoke holding backBuffer lock
        /// </summary>
        private void SetupScaling()
        {
            Program.AssertOnEventThread();

            if (_frontGraphics == null)
                return;

            if (_scaling)
            {
                AutoScroll = false;

                float xScale = Size.Width /
                    (float)(_displayBorder ? DesktopSize.Width + BORDER_PADDING * 3 : DesktopSize.Width);
                float yScale = Size.Height /
                    (float)(_displayBorder ? DesktopSize.Height + BORDER_PADDING * 3 : DesktopSize.Height);

                _scale = xScale > yScale ? yScale : xScale;
                _scale = _scale > 0.01 ? _scale : (float)0.01;

                _bump = (int)Math.Ceiling(1 / _scale);

                // Now do the offset

                _dx = (Size.Width - DesktopSize.Width * _scale) / 2;
                _dy = (Size.Height - DesktopSize.Height * _scale) / 2;

                Matrix transform = new Matrix();
                transform.Translate(_dx, _dy);
                transform.Scale(_scale, _scale);

                _frontGraphics.Transform = transform;
            }
            else
            {
                _scale = 1;
                _bump = 0;

                if (_connected)
                {
                    AutoScrollMinSize = new Size(
                        _displayBorder ? DesktopSize.Width + BORDER_PADDING + BORDER_PADDING : DesktopSize.Width,
                        _displayBorder ? DesktopSize.Height + BORDER_PADDING + BORDER_PADDING : DesktopSize.Height);
                }
                else
                {
                    AutoScrollMinSize = new Size(0, 0);
                }

                AutoScroll = true;

                if (Size.Height >= (_displayBorder ? DesktopSize.Height + BORDER_PADDING + BORDER_PADDING : DesktopSize.Height))
                {
                    _dy = ((float) Size.Height - DesktopSize.Height) / 2;
                }
                else
                {
                    if (_displayBorder)
                    {
                        _dy = BORDER_PADDING;
                        AutoScrollPosition = new Point(BORDER_PADDING, (int)_oldDy);
                    }
                    else
                    {
                        _dy = 0;
                        AutoScrollPosition = new Point(0, (int)_oldDy);
                    }
                }

                if (Size.Width >= (_displayBorder ? DesktopSize.Width + BORDER_PADDING + BORDER_PADDING : DesktopSize.Width))
                {
                    _dx = ((float)Size.Width - DesktopSize.Width) / 2;
                }
                else
                {
                    if (_displayBorder)
                    {
                        _dx = BORDER_PADDING;
                        AutoScrollPosition = new Point((int)_oldDx, BORDER_PADDING);
                    }
                    else
                    {
                        _dx = 0;
                        AutoScrollPosition = new Point((int)_oldDx, 0);
                    }
                }

                Matrix transform = new Matrix();
                transform.Translate(_dx, _dy);

                _frontGraphics.Transform = transform;
            }
        }

        /// <summary>
        /// Whether or not to display the blue rectangle around the control when it has focus.
        /// </summary>
        public bool DisplayBorder
        {
            set
            {
                _displayBorder = value;
                lock (_backBuffer)
                {
                    SetupScaling();
                }
                Invalidate();
                Update();
            }
        }

        public Size DesktopSize { get; set; }

        public Rectangle ConsoleBounds =>
            _scaling
                ? new Rectangle((int)_dx, (int)_dy, Size.Width - 2 * (int)_dx, Size.Height - 2 * (int)_dy)
                : new Rectangle((int)_dx, (int)_dy, DesktopSize.Width, DesktopSize.Height);

        #endregion
    }
}
