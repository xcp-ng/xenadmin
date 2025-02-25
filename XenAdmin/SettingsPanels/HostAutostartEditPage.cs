using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Diagnostics.Problems.VMProblem;
using XenAPI;
using XenOvf.Definitions;

namespace XenAdmin.SettingsPanels
{
    public partial class HostAutostartEditPage: UserControl, IEditPage
    {
        private Host host;

        public HostAutostartEditPage()
        {
            InitializeComponent();
        }

        public sealed override string Text => Messages.AUTOSTART;

        public string SubText => host.GetVmAutostartEnabled() ? Messages.ENABLED : Messages.DISABLED;

        public Image Image { get; } = Properties.Resources._000_EnablePowerControl_h32bit_16;

        public AsyncAction SaveSettings()
        {
            return new ChangeHostAutostartAction(host, enableAutostart.Checked);
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            host = clone as Host;
            Repopulate();
        }

        private void Repopulate()
        {
            if (host == null)
                return;

            enableAutostart.Checked = host.GetVmAutostartEnabled();
        }

        public bool ValidToSave
        {
            get { return true; }
        }

        public void ShowLocalValidationMessages()
        {
            
        }

        public void HideLocalValidationMessages()
        {
            
        }

        public void Cleanup()
        {
            
        }

        public bool HasChanged
        {
            get
            {
                return enableAutostart.Checked != host.GetVmAutostartEnabled();
            }
        }
    }
}
