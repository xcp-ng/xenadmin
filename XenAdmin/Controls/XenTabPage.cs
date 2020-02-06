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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using XenAdmin.Help;
using XenAdmin.Network;


namespace XenAdmin.Controls
{
    public delegate void XenTabPageStatusChanged(XenTabPage sender);

    [Designer(typeof(ParentControlDesigner))]
    public partial class XenTabPage : UserControl, IControlWithHelp
    {
        public IXenConnection Connection;

        /// <summary>
        /// Set this value to true to grey the step out in the wizard progress and mark it to be skipped
        /// </summary>
        public bool DisableStep;

        /// <summary>
        /// Gets or sets a value describing whether it is the first time the page is loaded after
        /// a change in a previous page affecting this one has taken place. Default value is True.
        /// </summary>
        public bool IsFirstLoad { get; set; }

        /// <summary>
        /// Gets or sets a value describing whether there are pending changes in the page.
        /// Default value is True (so the page takes in the default values of the controls)
        /// </summary>
        protected bool IsDirty { get; set; }

        /// <summary>
        /// Does the page use IsDirty?
        /// </summary>
        protected virtual bool ImplementsIsDirty()
        {
            return false;
        }

        public XenTabPage()
        {
            InitializeComponent();
            IsFirstLoad = true;
            IsDirty = true;
        }

        public Action<XenTabPage> WizardContentUpdater { protected get; set; }
        public Func<XenTabPage, bool> NextPagePrecheck { protected get; set; }

        protected override bool ScaleChildren
        {
            get { return false; }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        public virtual string PageTitle
        {
            get { return null; }
        }

        /// <summary>
        /// Note that most derived classes override the getter to return a fixed string
        /// </summary>
        public virtual string HelpID => "";

        public virtual string NextText(bool isLastPage)
        {
            return isLastPage ? Messages.FINISH : Messages.WIZARD_BUTTON_NEXT;
        }

        public virtual List<KeyValuePair<string, string>> PageSummary { get { return new List<KeyValuePair<string, string>>(); } }

        public virtual bool EnableNext()
        {
            return true;
        }

        public virtual bool EnablePrevious()
        {
            return true;
        }

        public virtual bool EnableCancel()
        {
            return true;
        }

        public void PageLoaded(PageLoadedDirection direction)
        {
            if (direction == PageLoadedDirection.Forward && IsFirstLoad)
            {
                if (ImplementsIsDirty())
                    IsDirty = true;

                PopulatePage();
                IsFirstLoad = false;
            }

            PageLoadedCore(direction);
        }

        protected virtual void PageLoadedCore(PageLoadedDirection direction)
        {
        }

        public void PageLeave(PageLoadedDirection direction, ref bool cancel)
        {
            PageLeaveCore(direction, ref cancel);

            if (direction == PageLoadedDirection.Forward && !cancel)
            {
                if (ImplementsIsDirty() && IsDirty && WizardContentUpdater != null)
                {
                    WizardContentUpdater.Invoke(this);//notify the wizard that the page contents have been updated
                    IsDirty = false; //reset edited state
                    return;
                }

                if (NextPagePrecheck != null)
                    cancel = !NextPagePrecheck.Invoke(this);

                if (!cancel && WizardContentUpdater != null)
                    WizardContentUpdater.Invoke(this);//notify the wizard that the page contents have been updated
            }
        }

        protected virtual void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
        }

        /// <summary>
        /// Called when the wizard's Cancel button is hit while on this page
        /// </summary>
        public virtual void PageCancelled(ref bool cancel)
        {
        }

        /// <summary>
        /// Fired when the page is ready (or not) for the user to advance to the next page).
        /// Currently not implemented by all pages.
        /// </summary>
        public event XenTabPageStatusChanged StatusChanged;

        /// <summary>
        /// Not always overriden in derived classes
        /// </summary>
        public virtual void PopulatePage() { }

        /// <summary>
        /// Check whether this step needs to be disabled. Not always overriden in derived classes
        /// </summary>
        public virtual void CheckPageDisabled() { }
        
        /// <summary>
        /// Select a control on the page. Not always overriden in derived classes
        /// </summary>
        public virtual void SelectDefaultControl() { }

        protected void OnPageUpdated()
        {
            if (StatusChanged != null)
                StatusChanged(this);
        }
    }

    public enum PageLoadedDirection
    {
        Forward,
        Back
    }
}
