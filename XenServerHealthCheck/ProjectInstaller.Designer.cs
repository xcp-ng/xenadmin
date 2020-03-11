namespace XenServerHealthCheck
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.XenServerHealthCheckInstaller = new System.ServiceProcess.ServiceInstaller();
            this.XenServerHealthCheckProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            // 
            // XenServerHealthCheckInstaller
            // 
            this.XenServerHealthCheckInstaller.ServiceName = "XenServerHealthCheck";
            this.XenServerHealthCheckInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.XenServerHealthCheckInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.XenServerHealthCheckInstaller_AfterInstall);
            // 
            // XenServerHealthCheckProcessInstaller
            // 
            this.XenServerHealthCheckProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.XenServerHealthCheckProcessInstaller.Password = null;
            this.XenServerHealthCheckProcessInstaller.Username = null;
            this.XenServerHealthCheckProcessInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.XenServerHealthCheckProcessInstaller_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.XenServerHealthCheckInstaller,
            this.XenServerHealthCheckProcessInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceInstaller XenServerHealthCheckInstaller;
        private System.ServiceProcess.ServiceProcessInstaller XenServerHealthCheckProcessInstaller;
    }
}