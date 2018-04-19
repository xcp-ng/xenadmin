using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace XenAdminDemo
{
    static class ProgramDemo
    {
        [STAThread]
        static void Main(string[] args)
        {
            OpenFileDialog StateDbChooser = new OpenFileDialog();
            if (DialogResult.OK == StateDbChooser.ShowDialog())
            {
                new DemoLauncher(StateDbChooser.FileName).Launch();
            }
        }
    }
}
