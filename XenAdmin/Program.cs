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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAPI;
using XenCenterLib;


namespace XenAdmin
{
    public static class Program
    {
        /// <summary>
        /// A UUID for the current instance of XenCenter.  Used to identify our own task instances.
        /// </summary>
        public static readonly string XenCenterUUID = Guid.NewGuid().ToString();

        private static NamedPipes.Pipe _pipe;
        private static string _pipePath;
        private const string PIPE_PATH_PATTERN = @"\\.\pipe\{0}-{1}-{2}-{3}";

        public static Font DefaultFont = FormFontFixer.DefaultFont;
        public static Font DefaultFontBold;
        public static Font DefaultFontUnderline;
        public static Font DefaultFontItalic;
        public static Font DefaultFontHeader;

        public static MainWindow MainWindow;

        public static CollectionChangeEventHandler ProgramInvokeHandler(CollectionChangeEventHandler handler)
        {
            return delegate (object s, CollectionChangeEventArgs args)
            {
                if (MainWindow != null)
                {
                    BeginInvoke(MainWindow, delegate
                    {
                        if (handler != null)
                            handler(s, args);
                    });
                }
            };
        }

        /// <summary>
        /// The secure hash of the main password used to load the client session.
        /// If this is null then no prior session existed and the user should be prompted
        /// to save his session when the UI is quit.
        /// </summary>
        public static byte[] MainPassword = null;

        /// <summary>
        /// A true value here indicates the user does not want to save session information for this
        /// particular instance of the UI; but when the UI is restarted he should be prompted again.
        /// </summary>
        public static bool SkipSessionSave = false;

        public static bool RunInAutomatedTestMode = false;
        public static string TestExceptionString;  // an exception passed back to the test framework
        private static readonly log4net.ILog log;

        public static volatile bool Exiting;

        public static readonly string AssemblyDir;
        public static readonly Version Version;
        public static readonly string VersionText;

        private static readonly System.Threading.Timer dailyTimer;

        static Program()
        {
            XenAdminConfigManager.Provider = new WinformsXenAdminConfigProvider();
            // Start timer to record resource usage every 24hrs
            dailyTimer = new System.Threading.Timer(delegate
            {
                LogApplicationStats();
            }, null, new TimeSpan(24, 0, 0), new TimeSpan(24, 0, 0));

            var assembly = Assembly.GetExecutingAssembly();
            AssemblyDir = Path.GetDirectoryName(assembly.Location);
            Version = assembly.GetName().Version;

            VersionText = Version.Major == 0 && Version.Minor == 0 && Version.Build == 0
                ? "vNext"
                : $"{Version.Major}.{Version.Minor}.{Version.Build}";

            var logFolder = Path.Combine(
                Properties.Settings.GetSettingsPath,
                BrandManager.ProductBrand,
                BrandManager.BrandConsole,
                "logs");
            
            log4net.GlobalContext.Properties["LOG_FILE"] = Path.Combine(logFolder, $"{BrandManager.BrandConsole}.log");
            log4net.GlobalContext.Properties["AUDIT_TRAIL"] = Path.Combine(logFolder, $"{BrandManager.BrandConsole}-AuditTrail.log");
            log4net.GlobalContext.Properties["NETWORK_TRACE"] = Path.Combine(logFolder, $"{BrandManager.BrandConsole}-NetworkTrace.log");

            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(Assembly.GetCallingAssembly().Location + ".config"));
            log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

            SetDefaultFonts();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            _pipePath = string.Format(PIPE_PATH_PATTERN,
                BrandManager.BrandConsole,
                Process.GetCurrentProcess().SessionId,
                Environment.UserName,
                Assembly.GetExecutingAssembly().Location.Replace('\\', '-'));

            if (NamedPipes.Pipe.ExistsPipe(_pipePath))
            {
                NamedPipes.Pipe.SendMessageToPipe(_pipePath, string.Join(" ", args));
                return;
            }

            log.Info("Application started");

            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.ThreadException -= Application_ThreadException;
            Application.ThreadException += Application_ThreadException;
            Application.ApplicationExit -= Application_ApplicationExit;
            Application.ApplicationExit += Application_ApplicationExit;

            ConnectPipe();

            // We must NEVER request ProfessionalColors before we have called Application.EnableVisualStyles
            // as it may prevent the program from displayed as expected.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Force the current culture, to make the layout the same whatever the culture of the underlying OS (CA-46983).
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = new CultureInfo(InvisibleMessages.LOCALE, false);

            if (string.IsNullOrEmpty(Thread.CurrentThread.Name))
                Thread.CurrentThread.Name = "Main program thread";

            ServicePointManager.DefaultConnectionLimit = 20;
            ServicePointManager.ServerCertificateValidationCallback = SSL.ValidateServerCertificate;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            Session.UserAgent = $"{BrandManager.BrandConsole} {Version}";

            LogSystemDetails();

            Application.Run(new SplashScreenContext(args));

            log.Info("Application main thread exited" + Environment.NewLine);
        }

        /// <summary>
        /// If the pipe doesn't already exist, it creates a new one and starts a thread
        /// that listens for incoming data on the pipe (from new invocations of XenCenter)
        /// and deals with the command line arguments of those instances.
        /// </summary>
        private static void ConnectPipe()
        {
            _pipe = new NamedPipes.Pipe(_pipePath);

            if (_pipe != null)
            {
                _pipe.Read += pipe_Read;
                _pipe.BeginRead();
            }
        }

        private static void pipe_Read(string message)
        {
            MainWindow m = MainWindow;
            
            if (m != null && !RunInAutomatedTestMode)
            {
                Invoke(m, delegate
                {
                    m.WindowState = FormWindowState.Normal;
                    m.ProcessCommand(message.Split(' ').ToArray());
                });
            }
        }

        private static void DisconnectPipe()
        {
            if (_pipe != null)
            {
                log.Debug("Disconnecting from named pipe in Program.DisconnectPipe()");
                ThreadPool.QueueUserWorkItem(state => _pipe.EndRead());
            }
        }

        private static void LogSystemDetails()
        {
            log.InfoFormat("Version: {0}", Version);
            log.InfoFormat(".NET runtime version: {0}", Environment.Version.ToString(4));
            log.InfoFormat("OS version: {0}", Environment.OSVersion);
            log.InfoFormat("UI Culture: {0}", Thread.CurrentThread.CurrentUICulture.EnglishName);
            log.InfoFormat("Bitness: {0}-bit", IntPtr.Size * 8);
        }

        private static void LogApplicationStats()
        {
            Process p = Process.GetCurrentProcess();

            log.Info("Log Application State");
            log.InfoFormat("ExitCode: {0}", Environment.ExitCode);

            log.InfoFormat("Time since process started: {0}", (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString());

            log.InfoFormat("Handles open: {0}", p.HandleCount.ToString());
            log.InfoFormat("USER handles open: {0}", Win32.GetGuiResourcesUserCount(p.Handle));
            log.InfoFormat("GDI handles open: {0}", Win32.GetGuiResourcesGDICount(p.Handle));
            log.InfoFormat("Thread count: {0}", p.Threads.Count);

            log.InfoFormat("Virtual memory size: {0} B({1})", p.VirtualMemorySize64, Util.MemorySizeStringSuitableUnits(p.VirtualMemorySize64, false));
            log.InfoFormat("Working set: {0} B({1})", p.WorkingSet64, Util.MemorySizeStringSuitableUnits(p.WorkingSet64, false));
            log.InfoFormat("Private memory size: {0} B({1})", p.PrivateMemorySize64, Util.MemorySizeStringSuitableUnits(p.PrivateMemorySize64, false));
            log.InfoFormat("Nonpaged system memory size: {0} B({1})", p.NonpagedSystemMemorySize64, Util.MemorySizeStringSuitableUnits(p.NonpagedSystemMemorySize64, false));
            log.InfoFormat("Paged memory size: {0} B({1})", p.PagedMemorySize64, Util.MemorySizeStringSuitableUnits(p.PagedMemorySize64, false));
            log.InfoFormat("Paged system memory size: {0} B({1})", p.PagedSystemMemorySize64, Util.MemorySizeStringSuitableUnits(p.PagedSystemMemorySize64, false));

            log.InfoFormat("Peak paged memory size: {0} B({1})", p.PeakPagedMemorySize64, Util.MemorySizeStringSuitableUnits(p.PeakPagedMemorySize64, false));
            log.InfoFormat("Peak virtual memory size: {0} B({1})", p.PeakVirtualMemorySize64, Util.MemorySizeStringSuitableUnits(p.PeakVirtualMemorySize64, false));
            log.InfoFormat("Peak working set: {0} B({1})", p.PeakWorkingSet64, Util.MemorySizeStringSuitableUnits(p.PeakWorkingSet64, false));

            log.InfoFormat("Process priority class: {0}", p.PriorityClass.ToString());
            log.InfoFormat("Privileged processor time: {0}", p.PrivilegedProcessorTime.ToString());

            log.InfoFormat("Total processor time: {0}", p.TotalProcessorTime.ToString());
            log.InfoFormat("User processor time: {0}", p.UserProcessorTime.ToString());
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            Exiting = true;
            try
            {
                Clip.UnregisterClipboardViewer();
            }
            catch (Exception ex)
            {
                log.Warn("Error while unregistering clipboard viewer on application exit", ex);
            }

            try
            {
                // Let's save the connections first, so we can save their connected state
                Settings.SaveServerList(); //this calls Settings.TrySaveSettings()
            }
            catch (Exception ex)
            {
                log.Warn("Error while saving server list on application exit", ex);
            }

            try
            {
                // The application is about to exit - gracefully close connections to
                // avoid a bunch of WinForms related race conditions...
                foreach (IXenConnection conn in ConnectionsManager.XenConnectionsCopy)
                    conn.EndConnect(false, true);
            }
            catch (Exception ex)
            {
                log.Warn("Error while ending connections on application exit", ex);
            }

            DisconnectPipe();
            LogApplicationStats();
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ProcessUnhandledException(e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ProcessUnhandledException(e.ExceptionObject as Exception);
        }

        private static void ProcessUnhandledException(Exception e)
        {
            try
            {
                if (e != null)
                {
                    log.Fatal("Uncaught exception", e);
                    if (e.InnerException != null)
                        log.Fatal("Inner exception", e.InnerException);
                }
                else
                {
                    log.Fatal("Fatal error");
                }
                LogSystemDetails();
                LogApplicationStats();

                if (RunInAutomatedTestMode)
                {
                    if (e != null && TestExceptionString == null)
                        TestExceptionString = e.GetBaseException().ToString();
                }
                else
                {
                    string filepath = GetLogFile() ?? Messages.MESSAGEBOX_LOGFILE_MISSING;

                    using (var d = new ErrorDialog(String.Format(Messages.MESSAGEBOX_PROGRAM_UNEXPECTED, HelpersGUI.DateTimeToString(DateTime.Now, "yyyy-MM-dd HH:mm:ss", false), filepath))
                    { WindowTitle = string.Format(Messages.MESSAGEBOX_PROGRAM_UNEXPECTED_TITLE, BrandManager.BrandConsole) })
                    {
                        // CA-44733
                        if (IsInvokable(MainWindow) && !MainWindow.InvokeRequired)
                            d.ShowDialog(MainWindow);
                        else
                            d.ShowDialog();
                    }
                }
            }
            catch (Exception exception)
            {
                try
                {
                    log.Fatal("Fatal error while handling fatal error!", exception);
                }
                catch (Exception)
                {
                    // ignored
                }

                if (!RunInAutomatedTestMode)
                {
                    using (var dlg = new ErrorDialog(exception.ToString()))
                        dlg.ShowDialog();
                    // To be handled by WER
                    throw;
                }
            }
        }

        /// <summary>
        /// Set the default fonts.
        /// Some locales have a known correct font, which we just use. Otherwise (e.g. English),
        /// we need to make sure that our fonts fit in with our panel layout.  This can be a problem
        /// on far-Eastern platforms in particular, because the default fonts there are wider in the
        /// English range than Tahoma / Segoe UI. Anything bigger than them is going to cause us to
        /// overflow on the panels, so we force it back to an appropriate font. We need to be careful
        /// not to override the user's settings if they've deliberately scaled up the text for readability
        /// reasons, so we only override if the font size is 9pt or smaller, which is the usual default.
        /// We also define a registry key to turn this off, just in case someone in the field complains.
        /// </summary>
        private static void SetDefaultFonts()
        {
            if (Registry.ForceSystemFonts)
            {
                log.Debug("ForceSystemFonts registry key is defined");
            }
            else if (InvisibleMessages.LOCALE.StartsWith("ja-"))
            {
                Font new_font =
                    (Environment.OSVersion.Version.Major < 6 || Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 0) ?
                        new Font("MS UI Gothic", 9.0f) : // 2K (5.0), XP (5.1), 2K3 and XP Pro (5.2), Vista and 2K8 (6.0)
                        new Font("Meiryo UI", 9.0f); // Win 7 and 2K8R2 (6.1) and above
                log.DebugFormat("Running on localized resources with Japanese fonts; switching the font from {0} {1}pt to {2} {3}pt.",
                                DefaultFont.Name, DefaultFont.Size, new_font.Name, new_font.Size);
                SetDefaultFonts(new_font);
                return;
            }
            else if (InvisibleMessages.LOCALE.StartsWith("zh-"))
            {
                Font new_font =
                        Environment.OSVersion.Version.Major < 6 ?
                        new Font("MS Shell Dlg", 9.0f) : // 2K (5.0), XP (5.1), 2K3 and XP Pro (5.2)
                        new Font("Microsoft YaHei", 9.0f); // Vista and above
                log.DebugFormat("Running on localized resources with Simplified Chinese fonts; switching the font from {0} {1}pt to {2} {3}pt.",
                                DefaultFont.Name, DefaultFont.Size, new_font.Name, new_font.Size);
                SetDefaultFonts(new_font);
                return;
            }
            else
            {
                if (DefaultFont.Size <= 9.0f)
                {
                    Size s = Drawing.MeasureText("Lorum ipsum", DefaultFont);
                    // Segoe UI 9pt gives 78x15 here.  Tahoma 8pt gives 66x13.
                    // We allow a bit of slop just in case antialias or hinting settings make a small difference.
                    if (s.Width > 80 || s.Height > 16)
                    {
                        Font new_font =
                            Environment.OSVersion.Version.Major < 6 ?
                                new Font("Tahoma", 8.0f) : // 2K (5.0), XP (5.1), 2K3 and XP Pro (5.2)
                                new Font("Segoe UI", 9.0f); // Vista and above

                        log.DebugFormat("Running on default resources with large default font; switching the font from {0} {1}pt to {2} {3}pt.",
                            DefaultFont.Name, DefaultFont.Size, new_font.Name, new_font.Size);
                        SetDefaultFonts(new_font);
                        return;
                    }
                    else
                    {
                        log.Debug("Running on default resources and happy with the font.");
                    }
                }
                else
                {
                    log.Debug("Running on default resources but the font is bigger than usual.");
                }
            }
            log.DebugFormat("Leaving the default font as {0} {1}pt.", DefaultFont.Name, DefaultFont.Size);
            SetDefaultFonts(DefaultFont);
        }

        private static void SetDefaultFonts(Font f)
        {
            DefaultFont = f;
            DefaultFontBold = new Font(f, FontStyle.Bold);
            DefaultFontUnderline = new Font(f, FontStyle.Underline);
            DefaultFontItalic = new Font(f, FontStyle.Italic);
            DefaultFontHeader = new Font(f.FontFamily, 9.75f, FontStyle.Bold);

            FormFontFixer.DefaultFont = f;
        }

        internal static void AssertOffEventThread()
        {
            if (MainWindow.Visible && !MainWindow.InvokeRequired)
            {
                FatalError();
            }
        }

        internal static void AssertOnEventThread()
        {
            if (MainWindow != null && MainWindow.Visible && MainWindow.InvokeRequired)
            {
                FatalError();
            }
        }

        private static void FatalError()
        {
            string msg = String.Format(Messages.MESSAGEBOX_PROGRAM_UNEXPECTED,
                HelpersGUI.DateTimeToString(DateTime.Now, "yyyy-MM-dd HH:mm:ss", false), GetLogFile());

            var msgWithStackTrace = string.Format("{0}\n{1}", msg, Environment.StackTrace);
            log.Fatal(msgWithStackTrace);

            MainWindow m = MainWindow;
            if (m == null)
            {
                log.Fatal("Program.MainWindow is null");
            }
            else
            {
                log.FatalFormat("Program.MainWindow.Visible == {0}", m.Visible);
                log.FatalFormat("Program.MainWindow.InvokeRequired == {0}", m.InvokeRequired);
                log.FatalFormat("CurrentThread.Name == {0}", Thread.CurrentThread.Name);
            }

            if (RunInAutomatedTestMode)
            {
                if (TestExceptionString == null)
                    TestExceptionString = msgWithStackTrace;
            }
            else
            {
                using (var dlg = new ErrorDialog(msg)
                { WindowTitle = string.Format(Messages.MESSAGEBOX_PROGRAM_UNEXPECTED_TITLE, BrandManager.BrandConsole) })
                    dlg.ShowDialog();
            }
        }

        public static void ViewLogFiles()
        {
            string s = GetLogFile();
            if (s == null)
            {
                using (var dlg = new ErrorDialog(Messages.MESSAGEBOX_LOGFILE_MISSING))
                    dlg.ShowDialog();
            }
            else
            {
                Process.Start(Path.GetDirectoryName(s));
            }
        }

        public static string GetLogFile()
        {
            foreach (log4net.Appender.IAppender appender in log.Logger.Repository.GetAppenders())
            {
                var fileAppender = appender as log4net.Appender.FileAppender;
                if (fileAppender != null)
                {
                    // Assume that the first FileAppender is the primary log file.
                    return fileAppender.File;
                }
            }
            return null;
        }

        #region Invocations

        /// <summary>
        /// Safely invoke the given delegate on the given control.
        /// </summary>
        public static void Invoke(Control c, MethodInvoker f)
        {
            try
            {
                if (IsInvokable(c))
                {
                    if (c.InvokeRequired)
                    {
                        MethodInvoker exceptionLogger = () =>
                        {
                            try
                            {
                                f();
                            }
                            catch (Exception e)
                            {
                                log.Error(string.Format("Exception in Invoke (ControlType={0}, MethodName={1})",
                                    c.GetType(), f.Method.Name), e);
                                throw;
                            }
                        };

                        c.Invoke(exceptionLogger);
                    }
                    else
                    {
                        f();
                    }
                }
            }
            catch (ObjectDisposedException e)
            {
                if (IsInvokable(c))
                    throw;
                log.Error(e);
            }
            catch (InvalidAsynchronousStateException e)
            {
                if (IsInvokable(c))
                    throw;
                log.Error(e);
            }
            catch (InvalidOperationException e)
            {
                if (IsInvokable(c))
                    throw;
                log.Error(e);
            }
        }

        /// <summary>
        /// Safely invoke the given delegate on the given control.
        /// </summary>
        /// <returns>The result of the function call, or null if the control cannot be invoked.</returns>
        public static object Invoke(Control c, Delegate f, params object[] p)
        {
            try
            {
                if (IsInvokable(c))
                {
                    if (c.InvokeRequired)
                    {
                        Func<object> exceptionLogger = () =>
                        {
                            try
                            {
                                return f.DynamicInvoke(p);
                            }
                            catch (Exception e)
                            {
                                log.Error(string.Format("Exception in Invoke (ControlType={0}, MethodName={1})",
                                    c.GetType(), f.Method.Name), e);
                                throw;
                            }
                        };

                        return c.Invoke(exceptionLogger);
                    }
                    else
                    {
                        return f.DynamicInvoke(p);
                    }
                }
            }
            catch (ObjectDisposedException e)
            {
                if (IsInvokable(c))
                    throw;
                log.Error(e);
            }
            catch (InvalidAsynchronousStateException e)
            {
                if (IsInvokable(c))
                    throw;
                log.Error(e);
            }
            catch (InvalidOperationException e)
            {
                if (IsInvokable(c))
                    throw;
                log.Error(e);
            }

            return null;
        }

        public static IAsyncResult BeginInvoke(Control c, Delegate f, params object[] p)
        {
            try
            {
                if (IsInvokable(c))
                    return c.BeginInvoke(f, p);
            }
            catch (ObjectDisposedException e)
            {
                if (IsInvokable(c))
                    throw;
                log.Error(e);
            }
            catch (InvalidAsynchronousStateException e)
            {
                if (IsInvokable(c))
                    throw;
                log.Error(e);
            }
            catch (InvalidOperationException e)
            {
                if (IsInvokable(c))
                    throw;
                log.Error(e);
            }

            return null;
        }

        public static void BeginInvoke(Control c, MethodInvoker f)
        {
            try
            {
                if (IsInvokable(c))
                    c.BeginInvoke(f);
            }
            catch (ObjectDisposedException e)
            {
                if (IsInvokable(c))
                    throw;
                log.Error(e);
            }
            catch (InvalidAsynchronousStateException e)
            {
                if (IsInvokable(c))
                    throw;
                log.Error(e);
            }
            catch (InvalidOperationException e)
            {
                if (IsInvokable(c))
                    throw;
                log.Error(e);
            }
        }

        private static bool IsInvokable(Control c)
        {
            return !Exiting && c != null && !c.Disposing && !c.IsDisposed && c.IsHandleCreated;
        }

        #endregion

        public static void OpenURL(string url)
        {
            if (RunInAutomatedTestMode || string.IsNullOrEmpty(url))
                return;
            try
            {
                Process.Start(url);
            }
            catch (Exception e)
            {
                log.Debug(string.Format("Exception while opening url {0}:", url), e);
            }
        }

        /// <summary>
        /// If true action threads will close themselves instantly...
        /// </summary>
        public static bool ForcedExiting = false;

        public static string CurrentLanguage => Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

        public static string VersionAndLanguage => $"{Version}.{CurrentLanguage}";

        public static CultureInfo CurrentCulture => Thread.CurrentThread.CurrentCulture;
    }
}
