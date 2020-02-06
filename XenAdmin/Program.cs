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
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Linq;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin.Dialogs;
using XenAdmin.Help;
using XenAdmin.XenSearch;
using XenAPI;
using XenCenterLib;


namespace XenAdmin
{
    public static class Program
    {
        public const int DEFAULT_WLB_PORT = 8012;

        /// <summary>
        /// Module for authenticating with proxy server using the Basic authentication scheme.
        /// </summary>
        private static IAuthenticationModule BasicAuthenticationModule;
        /// <summary>
        /// Module for authenticating with proxy server using the Digest authentication scheme.
        /// </summary>
        private static IAuthenticationModule DigestAuthenticationModule;

        /// <summary>
        /// A UUID for the current instance of XenCenter.  Used to identify our own task instances.
        /// </summary>
        public static readonly string XenCenterUUID = Guid.NewGuid().ToString();

        private static NamedPipes.Pipe pipe;
        private const string PIPE_PATH_PATTERN = @"\\.\pipe\XenCenter-{0}-{1}-{2}";

        /// <summary>
        /// Color.Transparent on most platforms; SystemColors.Window when ClearType is enabled.
        /// This is to work around a bug in TextRenderer.DrawText which causes text written to a double-buffer
        /// using ClearType to anti-alias onto black rather than onto the background colour.  In this case,
        /// we use Window, because by luck those labels are always on top of that colour on Vista and XP.
        /// We indicate that we're writing to a buffer (rather than the screen) by setting Graphics.TextContrast
        /// to 5 (the default is 4). This hack was needed because there's no easy way to add info to
        /// a Graphics object. (CA-22938).
        /// </summary>
        public static Color TransparentUsually = Color.Transparent;

        public static Font DefaultFont = FormFontFixer.DefaultFont;
        public static Font DefaultFontBold;
        public static Font DefaultFontUnderline;
        public static Font DefaultFontItalic;
        public static Font DefaultFontHeader;

        // Set in Main() AFTER we call EnableVisualStyles().
        // We set them here only so something decent shows up in the Designer.
        // We must not request ProfessionalColors before we have called Application.EnableVisualStyles
        // as it may prevent the program from displayed as expected.
        public static Color TitleBarStartColor;
        public static Color TitleBarEndColor;
        public static Color TitleBarBorderColor;
        public static Color TitleBarForeColor;
        public static Color HeaderGradientForeColor;
        public static Font HeaderGradientFont;
        public static Font TabbedDialogHeaderFont;
        public static Color TabPageRowBorder;
        public static Color TabPageRowHeader;

        public static MainWindow MainWindow = null;


        public static CollectionChangeEventHandler ProgramInvokeHandler(CollectionChangeEventHandler handler)
        {
            return delegate(object s, CollectionChangeEventArgs args)
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
        /// The secure hash of the master password used to load the client session.
        /// If this is null then no prior session existed and the user should be prompted
        /// to save his session when the UI is quit.
        /// </summary>
        public static byte[] MasterPassword = null;

        /// <summary>
        /// A true value here indicates the user does not want to save session information for this
        /// particular instance of the UI; but when the UI is restarted he should be prompted again.
        /// </summary>
        public static bool SkipSessionSave = false;

        public static bool RunInAutomatedTestMode = false;
        public static string TestExceptionString;  // an exception passed back to the test framework
        private static log4net.ILog log;

        public static volatile bool Exiting;

        public static readonly string AssemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private static readonly System.Threading.Timer dailyTimer;

        static Program()
        {
            XenAdminConfigManager.Provider = new WinformsXenAdminConfigProvider();
            // Start timer to record resource usage every 24hrs
            dailyTimer = new System.Threading.Timer(delegate
            {
                logApplicationStats();
            }, null, new TimeSpan(24, 0, 0), new TimeSpan(24, 0, 0));

            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(Assembly.GetCallingAssembly().Location + ".config"));
            log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

            SetDefaultFonts();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static public void Main(string[] Args)
        {
            //Upgrade settings
            Assembly a = Assembly.GetExecutingAssembly();
            Version appVersion = a.GetName().Version;
            string appVersionString = appVersion.ToString();
            log.DebugFormat("Application version of new settings {0}", appVersionString);

            try
            {
                if (Properties.Settings.Default.ApplicationVersion != appVersion.ToString())
                {
                    log.Debug("Upgrading settings...");
                    Properties.Settings.Default.Upgrade();

                    // if program's hash has changed (e.g. by upgrading to .NET 4.0), then Upgrade() doesn't import the previous application settings 
                    // because it cannot locate a previous user.config file. In this case a new user.config file is created with the default settings.
                    // We will try and find a config file from a previous installation and update the settings from it
                    if (Properties.Settings.Default.ApplicationVersion == "" && Properties.Settings.Default.DoUpgrade)
                        SettingsUpdate.Update();
                    log.DebugFormat("Settings upgraded from '{0}' to '{1}'", Properties.Settings.Default.ApplicationVersion, appVersionString);
                    Properties.Settings.Default.ApplicationVersion = appVersionString;
                    Settings.TrySaveSettings();
                }
            }
            catch (ConfigurationErrorsException ex)
            {
                log.Error("Could not load settings.", ex);
                var msg = string.Format("{0}\n\n{1}", Messages.MESSAGEBOX_LOAD_CORRUPTED_TITLE,
                                        string.Format(Messages.MESSAGEBOX_LOAD_CORRUPTED, Settings.GetUserConfigPath()));
                using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Error,msg,Messages.XENCENTER))
                               {
                                   StartPosition = FormStartPosition.CenterScreen,
                                   //For reasons I do not fully comprehend at the moment, the runtime
                                   //overrides the above StartPosition with WindowsDefaultPosition if
                                   //ShowInTaskbar is false. However it's a good idea anyway to show it
                                   //in the taskbar since the main form is not launcched at this point.
                                   ShowInTaskbar = true
                               })
                {
                    dlg.ShowDialog();
                }
                Application.Exit();
                return;
            }

            // Reset statics, because XenAdminTests likes to call Main() twice.
            TestExceptionString = null;
            Exiting = false;
            // Clear XenConnections and History so static classes like OtherConfigAndTagsWatcher 
            // listening to changes still work when Main is called more than once.
            ConnectionsManager.XenConnections.Clear();
            ConnectionsManager.History.Clear();

            Search.InitSearch(Branding.Search);
            TreeSearch.InitSearch();
            
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.ThreadException -= Application_ThreadException;
            Application.ThreadException += Application_ThreadException;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                if (SystemInformation.FontSmoothingType == 2) // ClearType
                    TransparentUsually = SystemColors.Window;
            }
            catch (NotSupportedException)
            {
                // Leave TransparentUsually == Color.Transparent.  This is an old platform
                // without FontSmoothingType support.
            }

            switch (Environment.OSVersion.Version.Major)
            {
                case 6: // Vista, 2K8, Win7.
                    if (Application.RenderWithVisualStyles)
                    {
                        // Vista, Win7 with styles.
                        TitleBarStartColor = Color.FromArgb(242, 242, 242);
                        TitleBarEndColor = Color.FromArgb(207, 207, 207);
                        TitleBarBorderColor = Color.FromArgb(160, 160, 160);
                        TitleBarForeColor = Color.FromArgb(60, 60, 60);
                        HeaderGradientForeColor = Color.White;
                        HeaderGradientFont = new Font(DefaultFont.FontFamily, 11.25f);
                        TabbedDialogHeaderFont = HeaderGradientFont;
                        TabPageRowBorder = Color.Gainsboro;
                        TabPageRowHeader = Color.WhiteSmoke;
                    }
                    else
                    {
                        // 2K8 and Vista, Win7 without styles.
                        TitleBarStartColor = ProfessionalColors.OverflowButtonGradientBegin;
                        TitleBarEndColor = ProfessionalColors.OverflowButtonGradientEnd;
                        TitleBarBorderColor = TitleBarEndColor;
                        TitleBarForeColor = SystemColors.ControlText;
                        HeaderGradientForeColor = SystemColors.ControlText;
                        HeaderGradientFont = new Font(DefaultFont.FontFamily, DefaultFont.Size + 1f, FontStyle.Bold);
                        TabbedDialogHeaderFont = HeaderGradientFont;
                        TabPageRowBorder = Color.DarkGray;
                        TabPageRowHeader = Color.Silver;
                    }
                    break;

                default:
                    TitleBarStartColor = ProfessionalColors.OverflowButtonGradientBegin;
                    TitleBarEndColor = ProfessionalColors.OverflowButtonGradientEnd;
                    TitleBarBorderColor = TitleBarEndColor;
                    TitleBarForeColor = SystemColors.ControlText;
                    HeaderGradientForeColor = Application.RenderWithVisualStyles ? Color.White : SystemColors.ControlText;
                    HeaderGradientFont = new Font(DefaultFont.FontFamily, DefaultFont.Size + 1f, FontStyle.Bold);
                    TabbedDialogHeaderFont = new Font(DefaultFont.FontFamily, DefaultFont.Size + 1.75f, FontStyle.Bold);
                    TabPageRowBorder = Color.DarkGray;
                    TabPageRowHeader = Color.Silver;
                    break;
            }

            // Force the current culture, to make the layout the same whatever the culture of the underlying OS (CA-46983).
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = new CultureInfo(InvisibleMessages.LOCALE, false);

            if (string.IsNullOrEmpty(Thread.CurrentThread.Name))
                Thread.CurrentThread.Name = "Main program thread";

            ServicePointManager.DefaultConnectionLimit = 20;
            ServicePointManager.ServerCertificateValidationCallback = SSL.ValidateServerCertificate;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            Session.UserAgent = string.Format("XenCenter/{0}", ClientVersion());
            RememberProxyAuthenticationModules();
            ReconfigureConnectionSettings();

            log.Info("Application started");
            logSystemDetails();
            OptionsDialog.Log();

            // Remove the '--wait' argument, which may have been passed to the splash screen
            var sanitizedArgs = Args.Where(ar => ar != "--wait").ToArray();

            var firstArgType = ParseFileArgs(sanitizedArgs, out string[] tailArgs);

            if (firstArgType == ArgType.Passwords)
            {
                try
                {
                    PasswordsRequest.HandleRequest(tailArgs[0]);
                }
                catch (Exception exn)
                {
                    log.Fatal(exn, exn);
                }

                Application.Exit();
                return;
            }

            try
            {
                ConnectPipe();
            }
            catch (Win32Exception exn)
            {
                log.Error("Creating named pipe failed. Continuing to launch XenCenter.", exn);
            }

            Application.ApplicationExit -= Application_ApplicationExit;
            Application.ApplicationExit += Application_ApplicationExit;

            //set the help version before launching the main window;
            //the call starts a different thread so it won't delay the main window launch;
            //in most cases it is expected to have returned by the time the users request help;
            //if they do before it has returned, the thread requesting help will wait for it
            HelpManager.SetHelpVersion();

            MainWindow mainWindow = new MainWindow(firstArgType, tailArgs);
            Application.Run(mainWindow);

            log.Info("Application main thread exited");
        }

        private static ArgType ParseFileArgs(string[] args, out string[] tailArgs)
        {
            tailArgs = new string[0];

            if (args == null || args.Length < 2)
            {
                log.Warn("Too few args passed in via command line");
                return ArgType.None;
            }

            var firstArgType = ArgType.None;

            switch (args[0])
            {
                case "import":
                    firstArgType = ArgType.Import;
                    break;
                case "license":
                    firstArgType = ArgType.License;
                    break;
                case "restore":
                    firstArgType = ArgType.Restore;
                    break;
                case "update":
                    firstArgType = ArgType.Update;
                    break;
                case "search":
                    firstArgType = ArgType.XenSearch;
                    break;
                case "passwords":
                    firstArgType = ArgType.Passwords;
                    break;
                case "connect":
                    firstArgType = ArgType.Connect;
                    break;
                default:
                    log.Warn("Wrong syntax or unknown command line options.");
                    break;
            }

            if (firstArgType != ArgType.None)
            {
                log.InfoFormat("Command line option passed in: {0}", firstArgType.ToString());
                tailArgs = args.Skip(1).ToArray();
            }

            return firstArgType;
        }

        /// <summary>
        /// Connects to the XenCenter named pipe. If the pipe didn't already exist, a new thread is started
        /// that listens for incoming data on the pipe (from new invocations of XenCenter) and deals
        /// with the command line arguments of those instances. If the pipe does exist, a Win32Exception is thrown.
        /// </summary>
        /// <exception cref="Win32Exception">If creating the pipe failed for any reason.</exception>
        private static void ConnectPipe()
        {
            string pipe_path = string.Format(PIPE_PATH_PATTERN, Process.GetCurrentProcess().SessionId, Environment.UserName, Assembly.GetExecutingAssembly().Location.Replace('\\', '-'));

            // Pipe path must be limited to 256 characters in length
            if (pipe_path.Length > 256)
            {
                pipe_path = pipe_path.Substring(0, 256);
            }

            log.InfoFormat(@"Connecting to pipe '{0}'", pipe_path);
            // Line below may throw Win32Exception
            pipe = new NamedPipes.Pipe(pipe_path);

            log.InfoFormat(@"Successfully created pipe '{0}' - proceeding to launch XenCenter", pipe_path);

            pipe.Read += delegate(object sender, NamedPipes.PipeReadEventArgs e)
            {
                MainWindow m = MainWindow;
                if (m == null || RunInAutomatedTestMode)
                    return;

                Invoke(m, delegate
                {
                    var bits = e.Message.Split(' ').Where(ar => ar != "--wait").ToArray();

                    var firstArgType = ParseFileArgs(bits, out string[] tailArgs);

                    if (firstArgType == ArgType.Passwords)
                    {
                        log.Error("Refusing to accept passwords request down pipe.  Use XenCenterMain.exe directly");
                        return;
                    }
                    if (firstArgType == ArgType.Connect)
                    {
                        log.Error("Connect not supported down pipe. Use XenCenterMain.exe directly");
                        return;
                    }
                    if (firstArgType == ArgType.None)
                        return;

                    // The C++ splash screen passes its command line as a literal string.
                    // This means we will get an e.Message like
                    //      open "C:\Documents and Settings\foo.xva"
                    // INCLUDING the double quotes, thus we need to trim them

                    m.WindowState = FormWindowState.Normal;
                    m.ProcessCommand(firstArgType, tailArgs[0].Trim('"'));
                });
            };
            pipe.BeginRead();
            // We created the pipe successfully - i.e. nobody was listening, so go ahead and start XenCenter
        }

        internal static void DisconnectPipe()
        {
            if (pipe != null)
            {
                log.Debug("Disconnecting from named pipe in Program.DisconnectPipe()");
                ThreadPool.QueueUserWorkItem(state => pipe.Disconnect());
            }
        }

        private static void logSystemDetails()
        {
            log.InfoFormat("Version: {0}", Assembly.GetExecutingAssembly().GetName().Version);
            log.InfoFormat(".NET runtime version: {0}", Environment.Version.ToString(4));
            log.InfoFormat("OS version: {0}", Environment.OSVersion);
            log.InfoFormat("UI Culture: {0}", Thread.CurrentThread.CurrentUICulture.EnglishName);
            log.InfoFormat("Bitness: {0}-bit", IntPtr.Size * 8);
        }

        static void Application_ApplicationExit(object sender, EventArgs e)
        {
            Exiting = true;

            logApplicationStats();

            Clip.UnregisterClipboardViewer();

            try
            {
                // Lets save the connections first, so we can save their connected state
                Settings.SaveIfRequired();
            }
            catch (Exception)
            {
                // Ignore here
            }
            // The application is about to exit - gracefully close connections to
            // avoid a bunch of WinForms related race conditions...
            foreach (IXenConnection conn in ConnectionsManager.XenConnectionsCopy)
                conn.EndConnect(false, true);
        }

        private static void logApplicationStats()
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
            log.InfoFormat("Private memory size: {0} B({1})",p.PrivateMemorySize64, Util.MemorySizeStringSuitableUnits(p.PrivateMemorySize64, false));
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

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ProcessUnhandledException(e.Exception);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
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
                logSystemDetails();
                logApplicationStats();

                if (RunInAutomatedTestMode)
                {
                    if (e != null && TestExceptionString == null)
                        TestExceptionString = e.GetBaseException().ToString();
                }
                else
                {
                    string filepath = GetLogFile() ?? Messages.MESSAGEBOX_LOGFILE_MISSING;

                    using (var d = new ThreeButtonDialog(
                       new ThreeButtonDialog.Details(
                           SystemIcons.Error,
                           String.Format(Messages.MESSAGEBOX_PROGRAM_UNEXPECTED, HelpersGUI.DateTimeToString(DateTime.Now, "yyyy-MM-dd HH:mm:ss", false), filepath),
                           Messages.MESSAGEBOX_PROGRAM_UNEXPECTED_TITLE)))
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
                }

                if (!RunInAutomatedTestMode)
                {
                    using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Error, exception.ToString(), Messages.XENCENTER)))
                    {
                        dlg.ShowDialog();
                    }
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
                using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Error, msg, Messages.MESSAGEBOX_PROGRAM_UNEXPECTED_TITLE)))
                {
                    dlg.ShowDialog();
                }
            }
        }

        public static void ViewLogFiles()
        {
            string s = GetLogFile();
            if (s == null)
            {
                using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Error, Messages.MESSAGEBOX_LOGFILE_MISSING, Messages.XENCENTER)))
                {
                    dlg.ShowDialog();
                }
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

        private static readonly List<string> HiddenObjects = new List<string>();

        internal static void HideObject(string opaqueRef)
        {
            lock (HiddenObjects)
            {
                HiddenObjects.Add(opaqueRef);
            }
            if (MainWindow != null)
            {
                MainWindow.RequestRefreshTreeView();
            }
        }

        internal static void ShowObject(string opaqueRef)
        {
            lock (HiddenObjects)
            {
                HiddenObjects.Remove(opaqueRef);
            }
            if (MainWindow != null)
            {
                MainWindow.RequestRefreshTreeView();
            }
        }

        internal static bool ObjectIsHidden(string opaqueRef)
        {
            lock (HiddenObjects)
            {
                return HiddenObjects.Contains(opaqueRef);
            }
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

        private static string ClientVersion()
        {
            foreach (object o in Assembly.GetExecutingAssembly().GetCustomAttributes(true))
            {
                var attr = o as XSVersionAttribute;
                if (attr != null)
                {
                    string result = attr.Version;
                    return result == "[BRANDING_PRODUCT_VERSION]" ? "PRIVATE" : result;
                }
            }
            return "MISSING";
        }

        public static void ReconfigureConnectionSettings()
        {
            ReconfigureProxyAuthenticationSettings();
            Session.Proxy = XenAdminConfigManager.Provider.GetProxyFromSettings(null);
        }

        /// <summary>
        /// Stores the Basic and Digest authentication modules, used for proxy server authentication, 
        /// for later use; this is needed because we cannot create new instances of them and it 
        /// saves us needing to create our own custom authentication modules.
        /// </summary>
        private static void RememberProxyAuthenticationModules()
        {
            var authModules = AuthenticationManager.RegisteredModules;
            while (authModules.MoveNext())
            {
                var module = (IAuthenticationModule)authModules.Current;
                if (module.AuthenticationType == "Basic")
                    BasicAuthenticationModule = module;
                else if (module.AuthenticationType == "Digest")
                    DigestAuthenticationModule = module;
            }
        }

        /// <summary>
        /// Configures .NET's AuthenticationManager to only use the authentication module that is 
        /// specified in the ProxyAuthenticationMethod setting. Also sets XenAPI's HTTP class to 
        /// use the same authentication method.
        /// </summary>
        private static void ReconfigureProxyAuthenticationSettings()
        {
            var authModules = AuthenticationManager.RegisteredModules;
            var modulesToUnregister = new List<IAuthenticationModule>();

            while (authModules.MoveNext())
            {
                var module = (IAuthenticationModule)authModules.Current;
                modulesToUnregister.Add(module);
            }

            foreach (var module in modulesToUnregister)
                AuthenticationManager.Unregister(module);

            var authSetting = (HTTP.ProxyAuthenticationMethod)Properties.Settings.Default.ProxyAuthenticationMethod;
            if (authSetting == HTTP.ProxyAuthenticationMethod.Basic)
                AuthenticationManager.Register(BasicAuthenticationModule);
            else
                AuthenticationManager.Register(DigestAuthenticationModule);

            HTTP.CurrentProxyAuthenticationMethod = authSetting;
        }

        private const string SplashWindowClass = "XenCenterSplash0001";

        internal static void CloseSplash()
        {
            IntPtr hWnd = Win32.FindWindow(SplashWindowClass, null);

            if (hWnd == IntPtr.Zero)
                return;

            if (!Win32.PostMessage(hWnd, Win32.WM_DESTROY, IntPtr.Zero, IntPtr.Zero))
            {
                log.Warn("PostMessage WM_DESTROY failed in CloseSplash()", new Win32Exception());
            }
        }

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

        public static Version Version
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

		public static string CurrentLanguage
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            }
        }

        public static string VersionAndLanguage
        {
            get
            {
                return string.Format("{0}.{1}", Version, CurrentLanguage);
            }
        }

        public static CultureInfo CurrentCulture
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture;
            }
        }
    }

    public enum ArgType { Import, License, Restore, Update, None, XenSearch, Passwords, Connect }
}
