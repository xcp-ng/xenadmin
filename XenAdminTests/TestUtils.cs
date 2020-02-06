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
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using NUnit.Framework;
using XenAdmin;
using XenAdmin.Controls;
using XenAdmin.Controls.DataGridViewEx;
using XenAdmin.Controls.MainWindowControls;
using XenAdmin.Controls.XenSearch;

namespace XenAdminTests
{
    internal static class TestUtils
    {
        /// <summary>
        /// Gets recursively a private field contained in a class of the specified instance.
        /// </summary>
        /// <param name="o">The instance of the class.</param>
        /// <param name="name">The name of the private field.</param>
        /// <returns>The value of the private field from the class of the specified instance.</returns>
        public static TField GetFieldDeep<TField>(object o, string name)
        {
            Util.ThrowIfStringParameterNullOrEmpty(name, "name");

            string[] parts = name.Split(new[] { '.' }, 2);

            object obj = null;
            try
            {
                Type type = o.GetType();
                FieldInfo info = GetBaseTypeField(type, parts[0]);
                obj = info.GetValue(o);
            }
            catch (Exception e)
            {
                Assert.Fail($"Field {parts[0]} of {o.GetType().Name} throws {e.GetType().Name}: {e.Message}.");
            }

            if (parts.Length == 1)
                return (TField)obj;

            return GetFieldDeep<TField>(obj, parts[1]);
        }

        /// <exception cref="NullReferenceException">Thrown if type==null</exception>
        private static FieldInfo GetBaseTypeField(Type type, string fieldName)
        {
            FieldInfo info = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (info == null)
            {
                Type baseType = type.BaseType;
                return GetBaseTypeField(baseType, fieldName);
            }

            return info;
        }

        public static TextBox GetTextBox(object o, string name)
        {
            return GetFieldDeep<TextBox>(o, name);
        }

        public static SearchTextBox GetSearchTextBox(object o, string name)
        {
            return GetFieldDeep<SearchTextBox>(o, name);
        }

        public static ComboBox GetComboBox(object o, string name)
        {
            return GetFieldDeep<ComboBox>(o, name);
        }

        public static CheckBox GetCheckBox(object o, string name)
        {
            return GetFieldDeep<CheckBox>(o, name);
        }

        public static Button GetButton(object o, string name)
        {
            return GetFieldDeep<Button>(o, name);
        }

        public static CheckedListBox GetCheckedListBox(object o, string name)
        {
            return GetFieldDeep<CheckedListBox>(o, name);
        }

        public static SrPicker GetSrPicker(object o, string name)
        {
            return GetFieldDeep<SrPicker>(o, name);
        }

        public static RadioButton GetRadioButton(object o, string name)
        {
            return GetFieldDeep<RadioButton>(o, name);
        }

        public static DataGridView GetDataGridView(object o, string name)
        {
            return GetFieldDeep<DataGridView>(o, name);
        }

        public static DoubleBufferedListView GetDoubleBufferedListView(object o, string name)
        {
            return GetFieldDeep<DoubleBufferedListView>(o, name);
        }

        public static FlickerFreeTreeView GetFlickerFreeTreeView(object o, string name)
        {
            return GetFieldDeep<FlickerFreeTreeView>(o, name);
        }

        public static NavigationView GetNavigationView(object o, string name)
        {
            return GetFieldDeep<NavigationView>(o, name);
        }

        public static NotificationsView GetNotificationsView(object o, string name)
        {
            return GetFieldDeep<NotificationsView>(o, name);
        }

        public static DataGridViewEx GetDataGridViewEx(object o, string name)
        {
            return GetFieldDeep<DataGridViewEx>(o, name);
        }

        public static XenTabPage GetXenTabPage(object o, string name)
        {
            return GetFieldDeep<XenTabPage>(o, name);
        }

        public static Label GetLabel(object o, string name)
        {
            return GetFieldDeep<Label>(o, name);
        }
		
		public static DropDownComboButton GetDropDownComboButton(object o, string name)
        {
            return GetFieldDeep<DropDownComboButton>(o, name);
        }

        public static ToolStripItem GetToolStripItem(object o, string name)
        {
            return GetFieldDeep<ToolStripItem>(o, name);
        }

        public static ToolStripButton GetToolStripButton(object o, string name)
        {
            return GetFieldDeep<ToolStripButton>(o, name);
        }

        public static ToolStripMenuItem GetToolStripMenuItem(object o, string name)
        {
            return GetFieldDeep<ToolStripMenuItem>(o, name);
        }

        public static ContextMenuStrip GetContextMenuStrip(object o, string name)
        {
            return GetFieldDeep<ContextMenuStrip>(o, name);
        }


        /// <summary>
        /// Executes the private method with the specified name from the wrapped class.
        /// </summary>
        /// <param name="name">The name of the private method.</param>
        /// <param name="parameters">The parameters of the private method.</param>
        public static object ExecuteMethod(object item, string name, object[] parameters)
        {
            return item.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(item, parameters);
        }

        /// <summary>
        /// Executes the private method with the specified name from the wrapped class.
        /// </summary>
        /// <param name="name">The name of the private method.</param>
        /// <param name="types">The types of the parameters of the private method.</param>
        /// <param name="parameters">The parameters of the private method.</param>
        public static object ExecuteMethod(object item, string name, Type[] types, object[] parameters)
        {
            return item.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic, null, types, null).Invoke(item, parameters);
        }


        public static string GetTestResource(string name)
        {
            var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(assemblyFolder, "TestResources", name);
        }
    }
}
