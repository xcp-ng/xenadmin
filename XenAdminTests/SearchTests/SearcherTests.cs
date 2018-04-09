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
using System.Windows.Forms;

using NUnit.Framework;
using XenAdmin.Controls.XenSearch;
using XenAdmin.CustomFields;
using XenAPI;
using XenAdmin.Model;
using XenAdmin.XenSearch;

namespace XenAdminTests.SearchTests
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class SearcherTests : MainWindowTester
    {
        [SetUp]
        public void Setup()
        {
            // clear any existing tree searches.
            ApplyTreeSearch(string.Empty);

            // set search to search-for-everything.
            MWWaitFor(() => SearchForComboButton.SelectItem<ObjectTypes>(o => o == ObjectTypes.AllExcFolders),
                "Couldn't select all-folders");
        }

        private Button EditSearchButton
        {
            get { return TestUtils.GetButton(MainWindowWrapper.Item, "SearchPage.buttonEditSearch"); }
        }

        private Searcher SearcherPanel
        {
            get
            {
                return TestUtils.GetFieldDeep<Searcher>(MainWindowWrapper.Item,
                                                        "SearchPage.Searcher");
            }
        }

        private DropDownComboButton SearchForComboButton
        {
            get
            {
                return TestUtils.GetFieldDeep<DropDownComboButton>(SearcherPanel,
                    "searchFor.searchForComboButton");
            }
        }

        private DropDownComboButton ComboButton
        {
            get
            {
                return TestUtils.GetFieldDeep<DropDownComboButton>(SearcherPanel,
                    "QueryElement.ComboButton");
            }
        }

        private DropDownComboButton QueryTypeComboButton
        {
            get
            {
                return TestUtils.GetFieldDeep<DropDownComboButton>(SearcherPanel,
                    "QueryElement.queryTypeComboButton");
            }
        }

        /// <summary>
        /// Tests that the filter value combo box updates when the Tags change for the selected object.
        /// </summary>
        /// <param name="xenObject">The xen object to be tested.</param>
        [Test]
        [Ignore("???")]
        public void TestSearcherUpdatesWhenTagsChange()
        {
            MW(() => EditSearchButton.PerformClick());

            // now test that the filter-value combo updates when the tags change for each of these objects.
            foreach (IXenObject xenObject in new IXenObject[] { GetAnyHost(), GetAnyNetwork(), GetAnyPool(), GetAnySR(), GetAnyVM() })
            {
                // select Tags for the query type
                MWWaitFor(() => QueryTypeComboButton.SelectItem<QueryElement.QueryType>(qt => qt.ToString() == "Tags"),
                    "Couldn't select Tags");

                string newTag = Guid.NewGuid().ToString();

                // add a new tag to the object.
                Tags.AddTag(xenObject, newTag);

                // now see if this new tag has been added to the searcher.
                MWWaitFor(() => null != ComboButton.Items.Find(ts => ts.Text == newTag),
                    "New tag not added to the Searcher with " + xenObject.GetType() + " selected.");
            }
        }

        [Test]
        public void TestSearcherDoesntUpdateWhenTagsChangeOnUnsearchableObject()
        {
            MW(() => EditSearchButton.PerformClick());

            // select Tags for the query type
            MWWaitFor(() => QueryTypeComboButton.SelectItem<QueryElement.QueryType>(qt => qt.ToString() == "Tags"),
                "Couldn't select Tags");

            string newTag = Guid.NewGuid().ToString();

            // add a new tag to the object.
            Tags.AddTag(GetAnyVBD(), newTag);

            // now check that this new tag has not been added to the searcher.
            MWWaitFor(() => null == ComboButton.Items.Find(ts => ts.Text == newTag),
                "New tag added to the Searcher with VBD selected.");
        }

        [Test]
        [Ignore("")]
        public void TestSearcherUpdatesWhenCustomFieldsChange()
        {
            MW(() => EditSearchButton.PerformClick());

            // create a new custom field
            string newCustomField = Guid.NewGuid().ToString();
            VM vm = GetAnyVM();

            CustomFieldsManager.AddCustomField(vm.Connection.Session, vm.Connection,
                new CustomFieldDefinition(newCustomField, CustomFieldDefinition.Types.String));

            // now see if this new custom field has been added to the searcher.
            MWWaitFor(delegate
            {
                // need to click for repopulate.
                QueryTypeComboButton.PerformClick();
                return null != QueryTypeComboButton.Items.Find(ts => ts.Text == newCustomField);
            }, "New custom field not added to the Searcher.");
        }
    }
}
