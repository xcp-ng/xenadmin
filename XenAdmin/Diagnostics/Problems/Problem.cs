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
using System.Drawing;
using XenAdmin.Actions;
using XenAdmin.Diagnostics.Checks;

namespace XenAdmin.Diagnostics.Problems
{
    public abstract class Problem : IComparable<Problem>, IDisposable
    {
        private AsyncAction solutionAction;
        private readonly Check _check;

        public virtual bool IsFixable
        {
            get { return true; }
        }

        private void SolutionAction_Completed(ActionBase sender)
        {
            SolutionActionCompleted = true;
        }

        private void RegisterSolutionActionEvent()
        {
            if (solutionAction != null)
                solutionAction.Completed += SolutionAction_Completed;
        }

        private void DeregisterSolutionActionEvent()
        {
            if (solutionAction != null)
                solutionAction.Completed -= SolutionAction_Completed;
        }

        protected Problem(Check check)
        {
            if (check == null)
                throw new ArgumentNullException();
            _check = check;
        }

        public abstract string Title { get; }
        public abstract string Description { get; }
        public bool SolutionActionCompleted { get; private set; }
        public AsyncAction SolutionAction { get { return solutionAction; } }

        protected virtual AsyncAction CreateAction(out bool cancelled)
        {
            cancelled = false;
            return null;
        }

        public AsyncAction GetSolutionAction(out bool cancelled)
        {
            DeregisterSolutionActionEvent();
            solutionAction = CreateAction(out cancelled);
            SolutionActionCompleted = false;
            RegisterSolutionActionEvent();
            return solutionAction;
        }

        public abstract string HelpMessage { get; }

        public Check Check
        {
            get { return _check; }
        }

        public virtual AsyncAction CreateUnwindChangesAction()
        {
            return null;
        }

        public int CompareTo(Problem other)
        {
            if (other == null)
                return 1;

            var result = string.Compare(Description, other.Description);

            if (result == 0)
                result = string.Compare(Title, other.Title);

            if (result == 0 && Check != null && Check.XenObject != null && other.Check != null)
                result = Check.XenObject.CompareTo(other.Check.XenObject);

            return result;
        }

        public override bool Equals(object obj)
        {
            Problem other = obj as Problem;
            if (other == null)
                return false;

            if (GetType() != other.GetType())
                return false;

            if (CompareTo(other) != 0)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return _check.GetHashCode();
        }

        public virtual Image Image 
        {
            get { return Images.GetImage16For(Icons.Error); }
        }

        #region IDisposable Members

        public void Dispose()
        {
            DeregisterSolutionActionEvent();
        }

        #endregion
    }
}
