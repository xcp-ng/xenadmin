using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenAdminTests;

namespace XenAdminDemo
{
    public class DemoLauncher : MainWindowLauncher
    {
        public DemoLauncher(string database) : base(database)
        {}

        public void Launch()
        {
            base._SetUp();
        }
    }
}
