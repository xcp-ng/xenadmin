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
using System.Text;
using XenAPI;
using XenAdmin.Properties;
using System.Drawing;
using XenAdmin.Dialogs;


namespace XenAdmin.Commands
{
    internal class VMOperationWlbOptimalServerCommand : VMOperationCommand
    {
        private readonly WlbRecommendations _recommendations;

        public VMOperationWlbOptimalServerCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection, vm_operations operation, WlbRecommendations recommendations)
            : base(mainWindow, selection, operation)
        {
            Util.ThrowIfParameterNull(recommendations, "recommendations");
            _recommendations = recommendations;
        }

        protected override Host GetHost(VM vm)
        {
            return _recommendations.GetOptimalServer(vm);
        }

        protected override bool CanExecute(VM vm)
        {
            return GetHost(vm) != null;
        }

        public override string MenuText
        {
            get
            {
                return Messages.WLB_OPT_MENU_OPTIMAL_SERVER;
            }
        }

        public override Image MenuImage
        {
            get
            {
                return Images.StaticImages._000_ServerWlb_h32bit_16;
            }
        }
        
        protected override CommandErrorDialog GetErrorDialogCore(IDictionary<IXenObject, string> cantExecuteReasons)
        {
            return new CommandErrorDialog(ErrorDialogTitle, ErrorDialogText, cantExecuteReasons);
        }
    }
}
