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

using System.Collections.Generic;
using System.Drawing;
using XenAdmin.Core;
using XenAdmin.Properties;
using XenAPI;
using XenAdmin.Dialogs;


namespace XenAdmin.Commands
{
    internal partial class VMOperationWlbHostCommand : VMOperationCommand
    {
        private readonly static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _menuText;
        private readonly Image _menuImage;
        private readonly Image _secondImage;
        private readonly double _starRating;
        private readonly Host _host;
        private readonly WlbRecommendations.WlbRecommendation _recommendation;

        public VMOperationWlbHostCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> vms, Host host, vm_operations operation, WlbRecommendations.WlbRecommendation recommendation)
            : base(mainWindow, vms, operation)
        {
            Util.ThrowIfParameterNull(recommendation, "recommendation");
            Util.ThrowIfParameterNull(host, "host");

            _host = host;
            _menuText = _host.Name().EscapeAmpersands();
            
            //Default or failure case, there is no score/star rating actually, just don't display star
            _secondImage = null;
            _menuImage = Images.StaticImages._000_ServerDisconnected_h32bit_16;
            _recommendation = recommendation;

            if (CanExecute())
            {
                _starRating = _recommendation.StarRating;
                _menuImage = Images.StaticImages._000_TreeConnected_h32bit_16;
                _secondImage = GetWLBStarImage(_starRating);
            }
            else
            {
                // get the failure reason if the dictionary is populated with reasons that are all the same.
                // otherwise don't display a reason - leave this to the error dialog.

                string reason = null;
                foreach (string r in _recommendation.CantExecuteReasons.Values)
                {
                    if (reason != null && r != reason)
                    {
                        return;
                    }
                    reason = r;
                }

                if (!string.IsNullOrEmpty(reason))
                {
                    _menuText = string.Format(Messages.MAINWINDOW_CONTEXT_REASON, _menuText, reason);
                }
            }
        }

        protected override Host GetHost(VM vm)
        {
            return _host;
        }

        public override string MenuText
        {
            get
            {
                return _menuText;
            }
        }

        public override Image MenuImage
        {
            get
            {
                return _menuImage;
            }
        }

        public override Image SecondImage
        {
            get
            {
                return _secondImage;
            }
        }

        public override double StarRating
        {
            get
            {
                return _starRating;
            }
        }

        protected override bool CanExecute(VM vm)
        {
            return vm != null && _recommendation.CanExecuteByVM.ContainsKey(vm) && _recommendation.CanExecuteByVM[vm];
        }

        /// <summary>
        /// WLB: Get proper star image for a host
        /// </summary>
        private static Image GetWLBStarImage(double stars)
        {
            Image img = Images.StaticImages._000_host_0_star;

            if (stars >= 4.8)
                img = Images.StaticImages._000_host_10_star;
            else if (stars >= 4.3)
                img = Images.StaticImages._000_host_9_star;
            else if (stars >= 3.8)
                img = Images.StaticImages._000_host_8_star;
            else if (stars >= 3.3)
                img = Images.StaticImages._000_host_7_star;
            else if (stars >= 2.8)
                img = Images.StaticImages._000_host_6_star;
            else if (stars >= 2.3)
                img = Images.StaticImages._000_host_5_star;
            else if (stars >= 1.8)
                img = Images.StaticImages._000_host_4_star;
            else if (stars >= 1.3)
                img = Images.StaticImages._000_host_3_star;
            else if (stars >= .8)
                img = Images.StaticImages._000_host_2_star;
            else if (stars > 0)
                img = Images.StaticImages._000_host_1_star;

            return img;
        }

        protected override CommandErrorDialog GetErrorDialogCore(IDictionary<IXenObject, string> cantExecuteReasons)
        {
            return new CommandErrorDialog(ErrorDialogTitle, ErrorDialogText, cantExecuteReasons);
        }

        protected override string GetCantExecuteReasonCore(IXenObject item)
        {
            VM vm = item as VM;
            if (vm == null)
                return base.GetCantExecuteReasonCore(item);

            if (_recommendation.CantExecuteReasons.ContainsKey(vm))
            {
                return _recommendation.CantExecuteReasons[vm];
            }

            return base.GetCantExecuteReasonCore(item);
        }
    }
}
