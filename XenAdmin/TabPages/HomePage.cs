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
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Commands;
using XenAdmin.Help;


namespace XenAdmin.TabPages
{
    public partial class HomePage : DoubleBufferedPanel, IControlWithHelp
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string XCNS = "XenCenter://";
        bool initializing = true;

        public HomePage()
        {
            InitializeComponent();

            try
            {
                var location = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), InvisibleMessages.HOMEPAGE_FILENAME);
                webBrowser.Navigate(location);
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Failed to load the HomePage. Url = {0}", Location), ex);
            }

            initializing = false;
        }

        private void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (initializing)
                return;

            e.Cancel = true;

            string url = e.Url.OriginalString;

            // this is not abstracted away as long as we have only a very limited number of functionalities:
            if (url != null && url.StartsWith(XCNS, StringComparison.InvariantCultureIgnoreCase))
            {
                if (url.Contains("HelpContents"))
                {
                    XenAdmin.Help.HelpManager.Launch(null);
                }
                else if (url.Contains("AddServer"))
                {
                    new AddHostCommand(Program.MainWindow, this).Execute();
                }
            }
            else
            {
                Program.OpenURL(url);
            }
        }

        public string HelpID => "TabPageHome";
    }
}
