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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenAdmin.Core
{
    public class HiddenFeatures
    {
        internal static bool CPSOptimizationHidden
        {
            get
            { return Registry.HiddenFeatures != null && Registry.HiddenFeatures.Contains(CPS_OPTIMIZATION_HIDDEN); }
        }

        internal static bool RDPPollingHidden
        {
            get
            { return Registry.HiddenFeatures != null && Registry.HiddenFeatures.Contains(RDP_POLLING_HIDDEN); }
        }

        internal static bool LearnMoreButtonHidden
        {
            get
            { return Registry.HiddenFeatures != null && Registry.HiddenFeatures.Contains(LEARN_MORE_HIDDEN); }
        }

        internal static bool LinkLabelHidden
        {
            get
            { return Registry.HiddenFeatures != null && Registry.HiddenFeatures.Contains(LINK_LABEL_HIDDEN); }
        }

        internal static bool ToolStripMenuItemHidden
        {
            get
            { return Registry.HiddenFeatures != null && Registry.HiddenFeatures.Contains(TOOL_STRIP_MENU_ITEM_HIDDEN); }
        }

        internal static bool CrossServerPrivateNetworkHidden
        {
            get
            { return Registry.HiddenFeatures != null && Registry.HiddenFeatures.Contains(CROSS_SERVER_PRIVATE_NETWORK_HIDDEN); }
        }

        internal static bool CopyrightHidden
        {
            get
            { return Registry.HiddenFeatures != null && Registry.HiddenFeatures.Contains(COPYRIGHT_HIDDEN); }
        }

        internal static bool UploadOptionHidden
        {
            get
            { return Registry.HiddenFeatures != null && Registry.HiddenFeatures.Contains(UPLOAD_OPTION_HIDDEN); }
        }

        internal static bool LicenseNagVisible
        {
            get
            { return Registry.AdditionalFeatures != null && Registry.AdditionalFeatures.Contains(LICENSE_NAG); }
        }

        internal static bool LicenseOperationsHidden
        {
            get
            { return Registry.HiddenFeatures != null && Registry.HiddenFeatures.Contains(LICENSE_OPERATIONS_HIDDEN); }
        }

        internal static bool WindowsUpdateHidden
        {
            get { return Registry.HiddenFeatures != null && Registry.HiddenFeatures.Contains(WINDOWS_UPDATE_HIDDEN); }
        }

        private const string CPS_OPTIMIZATION_HIDDEN = "cps_optimization";
        private const string RDP_POLLING_HIDDEN = "rdp_polling";
        private const string LEARN_MORE_HIDDEN = "learn_more";
        private const string LINK_LABEL_HIDDEN = "link_label";
        private const string TOOL_STRIP_MENU_ITEM_HIDDEN = "tool_strip_menu_item";
        private const string CROSS_SERVER_PRIVATE_NETWORK_HIDDEN = "cross_server_private_network";
        private const string COPYRIGHT_HIDDEN = "copyright";
        private const string UPLOAD_OPTION_HIDDEN = "upload_option";
        private const string LICENSE_NAG = "license_nag";
        private const string LICENSE_OPERATIONS_HIDDEN = "license_operations";
        private const string WINDOWS_UPDATE_HIDDEN = "windows_update";
    }
}
