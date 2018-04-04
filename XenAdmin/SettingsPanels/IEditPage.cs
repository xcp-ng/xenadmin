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
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAPI;


namespace XenAdmin.SettingsPanels
{
    public interface IEditPage : VerticalTabs.IVerticalTab
    {
        /// <summary>
        /// Each page's SaveSettings() can do two things:
        ///   1) Edit the cloned XenObject (which is a clone of the one in the cache)
        ///      for simple field changes;
        ///   2) Return an AsyncAction for later or more complicated actions.
        /// When the user clicks OK on the PropertiesDialog, we run SaveSettings
        /// for each page. We then save all changes to the XenModelObject from step (1).
        /// Finally we execute each of the actions we have collected from step (2).
        /// Because we are expecting things to happen in that order, the SaveSettings()
        /// should not make any API calls itself: it should put them in its returned action.
        /// </summary>
        /// <returns></returns>
        AsyncAction SaveSettings();

        /// <summary>
        /// Sets the control's active object to match the current selection.
        /// Most actions should be carried out on the cloned object, which is guaranteed
        ///  not to change while the dialog is open. Most pages therefore ignore the orig
        /// object, but some delegates from SaveSettings() need to know it too.
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="clone"></param>
        void SetXenObjects(IXenObject orig, IXenObject clone);

        /// <summary>
        /// Performs local validation on the fields in the control and lets the
        /// user know if it is ok to save the XenObject.
        /// </summary>
        bool ValidToSave { get; }

        /// <summary>
        /// Show local validation balloon tooltips
        /// </summary>
        void ShowLocalValidationMessages();

        /// <summary>
        /// Unregister listeners, dispose balloon tooltips, etc.
        /// </summary>
        void Cleanup();

        /// <summary>
        /// Have any of the fields in the view changed since init?
        /// </summary>
        bool HasChanged { get; }
    }
}
