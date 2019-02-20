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
using XenAdmin.Core;
using XenAdmin.Wizards.ImportWizard;
using XenAPI;


namespace XenAdmin.Commands
{
	class ImportCommand : Command
	{
		public ImportCommand()
		{ }

		public ImportCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
			: base(mainWindow, selection)
		{ }

        public ImportCommand(IMainWindow mainWindow,Pool pool)
            : base(mainWindow, pool)
        { }

		public override string MenuText { get { return Messages.HOST_MENU_IMPORT_VM_TEXT; } }
		
		public override string ContextMenuText{get{return Messages.HOST_MENU_IMPORT_VM_TEXT;}}

		protected override void ExecuteCore(SelectedItemCollection selection)
		{
			var con = selection.GetConnectionOfFirstItem();
			MainWindowCommandInterface.ShowPerConnectionWizard(con, new ImportWizard(con, selection.FirstAsXenObject, null, false));
		}
	}
}
