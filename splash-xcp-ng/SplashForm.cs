using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace splash_xcp_ng
{
    public partial class SplashForm : Form
    {
        private System.IO.Stream stream;
        private System.Reflection.Assembly assembly;
        const string exe = "XenCenterMain.exe";
        private string exeFullPath = string.Empty;
        private string appdir = string.Empty;

        Version AssemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        string ProductVersion = System.Windows.Forms.Application.ProductVersion; 

        private BackgroundWorker bworker = new BackgroundWorker();
        private ProcessStartInfo startInfo = new ProcessStartInfo();
        private Process proc;

        public SplashForm()
        {
            InitializeComponent();
            Style();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();
            Launch();
        }

        private void Style()
        {
            labelVersion.Text = ProductVersion + " (Build " + AssemblyVersion + ")";

            Image bitmap;
            assembly = System.Reflection.Assembly.LoadFrom(Application.ExecutablePath);
            stream = assembly.GetManifestResourceStream("splash_xcp_ng.Resources.splash.bmp");
            bitmap = Image.FromStream(stream);

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
            this.BackgroundImage = bitmap;
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.Width = bitmap.Width;
            this.Height = bitmap.Height;
            this.CenterToScreen();
        }

        private void Launch()
        {
            appdir = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            exeFullPath = Path.Combine(appdir, exe);

            if (!File.Exists(exeFullPath))
            {
                MessageBox.Show("[ERROR] Application not found: " + Environment.NewLine + exeFullPath);
                Exit();
                return;
            }

            #region Arguments

            // test for agruments: "XCP-ng Center.exe" messageboxtest

            string arguments = String.Empty;
            string[] args = Environment.GetCommandLineArgs();

            if (args != null && args.Length > 0)
            {
                //remove first argument that always is the current 
                List<string> temp = new List<string>(args);
                temp.RemoveAt(0);
                string[] argsonly = temp.ToArray();

                arguments = string.Join(" ", argsonly);
            }

            #endregion

            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = exeFullPath;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = arguments;
            startInfo.WorkingDirectory = appdir;



            bworker.WorkerSupportsCancellation = true;
            bworker.DoWork += new DoWorkEventHandler(Start);
            bworker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bworkerCompleted);
            bworker.RunWorkerAsync();

            //safty counter to exit if something goes wrong
            int timeoutSeconds = 60;
            int sleepMilliseconds = 100;
            int max = timeoutSeconds * 1000;

            int counter = 0;

            while (proc == null)
            {
                System.Threading.Thread.Sleep(sleepMilliseconds);
                if (proc != null) proc.Refresh();

                counter++;
                if ((counter * sleepMilliseconds) >= max) break;
            }

            if (proc == null)
            {
                MessageBox.Show("[ERROR] Something went wrong, program did not start in time: " + Environment.NewLine + exeFullPath);
            }
            else
            {
                if (proc.HasExited)
                {
                    MessageBox.Show("[ERROR] Something went wrong, program stopped already: " + Environment.NewLine + exeFullPath);
                }
                else
                {
                    for (int i = 1; i * sleepMilliseconds <= max; i++)
                    {
                        try
                        {
                            while (string.IsNullOrEmpty(proc.MainWindowTitle))
                            {
                                System.Threading.Thread.Sleep(sleepMilliseconds);
                                if (proc != null) proc.Refresh();

                                counter++;
                                if ((counter * sleepMilliseconds) >= max) break;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("[ERROR] Something went wrong, failed to start: " + Environment.NewLine + exeFullPath);
                            Exit();
                        }
                    }
                }

            }

            Exit();
        }

        private void Start(object sender, DoWorkEventArgs e)
        {
                proc = Process.Start(startInfo);
        }

        private void bworkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        private void Exit()
        {
            if (bworker != null && bworker.WorkerSupportsCancellation) bworker.CancelAsync();
            System.Environment.Exit(1);
        }


    }
}
