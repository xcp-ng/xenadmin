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
using XenAdmin;
using XenAdmin.Network;
using System.Windows.Forms;
using XenAPI;

namespace XenAdminTests
{
    internal class MockMainWindow : IMainWindow
    {
        #region IMainWindow Members

        public Form Form => null;

        public void Invoke(MethodInvoker method)
        {

        }

        public bool SelectObjectInTree(IXenObject xenObject)
        {
            return false;
        }

        public void RequestRefreshTreeView()
        {
        }

        public void ShowPerXenModelObjectWizard(IXenObject obj, Form wizard)
        {

        }

        public void ShowPerConnectionWizard(IXenConnection connection, Form wizard, Form parentForm = null)
        {

        }

        public Form ShowForm(Type type)
        {
            return null;
        }

        public Form ShowForm(Type type, object[] args)
        {
            return null;
        }

        public void CloseActiveWizards(IXenConnection connection)
        {
            throw new NotImplementedException();
        }

        public void CloseActiveWizards(IXenObject xenObject)
        {
            throw new NotImplementedException();
        }

        public System.Collections.ObjectModel.Collection<IXenConnection> GetXenConnectionsCopy()
        {
            throw new NotImplementedException();
        }

        public void SaveServerList()
        {
            throw new NotImplementedException();
        }

        public bool DoSearch(string filename)
        {
            throw new NotImplementedException();
        }

        public bool RunInAutomatedTestMode => throw new NotImplementedException();

        public void RemoveConnection(IXenConnection connection)
        {
            throw new NotImplementedException();
        }

        public void PutSelectedNodeIntoEditMode()
        {
            throw new NotImplementedException();
        }

        public void SwitchToTab(MainWindow.Tab tab)
        {
            throw new NotImplementedException();
        }

        public void TrySelectNewObjectInTree(Predicate<object> tagMatch, bool selectNode, bool expandNode, bool ensureNodeVisible)
        {
            throw new NotImplementedException();
        }

        public bool MenuShortcutsEnabled
        {
            get { return true; }
            set { }
        }

        public void TrySelectNewObjectInTree(IXenConnection c, bool selectNode, bool expandNode, bool ensureNodeVisible)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
