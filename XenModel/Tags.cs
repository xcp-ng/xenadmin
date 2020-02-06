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

using XenAPI;
using XenCenterLib;
using XenAdmin.Actions;
using XenAdmin.Network;


namespace XenAdmin.Model
{
    public class Tags
    {
        public static String[] GetTags(IXenObject o)
        {
            return o.Get("tags") as String[];
        }

        public static ComparableList<String> GetTagList(IXenObject o)
        {
            String[] tags = GetTags(o);

            if (tags == null)
                return null;
            else
                return new ComparableList<String>(tags);
        }

        private static void RenameTag(Session session, IXenObject o, String tagOriginal, string newTag)
        {
            //Program.AssertOffEventThread();
            foreach (string tag in GetAllTags())
            {
                if (tag == tagOriginal)
                {
                    RemoveTag(session, o, tagOriginal);
                    o.Connection.WaitFor(delegate()
                    {
                        return !new List<string>(GetTags(o)).Contains(tagOriginal);
                    },
                  null);
                    AddTag(session, o, newTag);
                    o.Connection.WaitFor(delegate()
                    {
                        return new List<string>(GetTags(o)).Contains(newTag);
                    },
                    null);
                }
            }
        }

        public static DelegatedAsyncAction RemoveTagAction(IXenObject o, string tag)
        {
            return new DelegatedAsyncAction(o.Connection,
                 String.Format(Messages.DELETE_TAG, tag),
                 String.Format(Messages.DELETING_TAG, tag),
                 String.Format(Messages.DELETED_TAG, tag),
                 delegate(Session session)
                 {
                     Tags.RemoveTag(session, o, tag);
                 },
                 o.GetType().Name.ToLowerInvariant() + ".remove_tags"
            );
        }

        public static void RemoveTag(Session session, IXenObject o, String tag)
        {
            //Program.AssertOffEventThread();

            String[] tags = GetTags(o);
            if (tags != null && Array.IndexOf<String>(tags, tag) < 0)
                return;

            o.Do("remove_tags", session, o.opaque_ref, tag);
        }

        public static void RemoveTagGlobally(string tag)
        {
            var conn = ConnectionsManager.XenConnectionsCopy[0] as XenConnection;
            conn?.OnBeforeMajorChange(true);

            try
            {
                foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
                {
                    foreach (IXenObject o in connection.Cache.XenSearchableObjects)
                    {
                        string[] tags = GetTags(o);
                        if (tags == null)
                            continue;

                        foreach (string existingTag in tags)
                        {
                            if (existingTag == tag)
                            {
                                RemoveTag(connection.Session, o, existingTag);
                                break;
                            }
                        }
                    }
                }
            }
            finally
            {
                conn?.OnAfterMajorChange(true);
            }
        }

        public static void RenameTagGlobally(string oldTag, string newTag)
        {
            var conn = ConnectionsManager.XenConnectionsCopy[0] as XenConnection;
            conn?.OnBeforeMajorChange(true);

            try
            {
                foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
                {
                    foreach (IXenObject o in connection.Cache.XenSearchableObjects)
                    {
                        string[] tags = GetTags(o);
                        if (tags == null)
                            continue;

                        foreach (string existingTag in tags)
                        {
                            if (existingTag == oldTag)
                            {
                                RenameTag(connection.Session, o, existingTag, newTag);
                                break;
                            }
                        }
                    }
                }
            }
            finally
            {
                conn?.OnAfterMajorChange(true);
            }
        }

        /// <summary>
        /// All objects which are tagged with a particular tag, on all connections
        /// </summary>
        public static List<IXenObject> Users(string tag)
        {
            List<IXenObject> ans = new List<IXenObject>();

            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                foreach (IXenObject o in connection.Cache.XenSearchableObjects)
                {
                    string[] tags = GetTags(o);
                    if (tags == null)
                        continue;

                    foreach (string existingTag in tags)
                    {
                        if (existingTag == tag)
                            ans.Add(o);
                    }
                }
            }

            return ans;
        }

        public static void AddTag(Session session, IXenObject o, String tag)
        {
            //Program.AssertOffEventThread();

            String[] tags = GetTags(o);
            if (tags != null && Array.IndexOf<String>(tags, tag) > -1)
                return;

            o.Do("add_tags", session, o.opaque_ref, tag);
        }

        public static void AddTag(IXenObject o, string tag)
        {
            AddTag(o.Connection.Session, o, tag);
        }

        public static DelegatedAsyncAction AddTagAction(IXenObject o, string tag)
        {
            return new DelegatedAsyncAction(o.Connection,
                 String.Format(Messages.ADD_TAG, tag),
                 String.Format(Messages.ADDING_TAG, tag),
                 String.Format(Messages.ADDED_TAG, tag),
                 delegate(Session session)
                 {
                     Tags.AddTag(session, o, tag);
                 },
                 o.GetType().Name.ToLowerInvariant() + ".add_tags"
            );
        }

        public static Dictionary<String, int> GetTagCounts()
        {
            Dictionary<String, int> tagsDict = new Dictionary<String, int>();

            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                foreach (IXenObject o in connection.Cache.XenSearchableObjects)
                {
                    String[] tags = GetTags(o);
                    if (tags == null)
                        continue;

                    foreach (String tag in tags)
                    {
                        if (!tagsDict.ContainsKey(tag))
                            tagsDict[tag] = 0;

                        tagsDict[tag]++;
                    }
                }
            }

            return tagsDict;
        }

        public static IDictionary<string, IList<IXenObject>> GetTags()
        {
            IDictionary<string, IList<IXenObject>> tagsDict = new Dictionary<string, IList<IXenObject>>();

            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                foreach (IXenObject o in connection.Cache.XenSearchableObjects)
                {
                    String[] tags = GetTags(o);
                    if (tags == null)
                        continue;

                    foreach (string tag in tags)
                    {
                        IList<IXenObject> list;
                        if (tagsDict.TryGetValue(tag, out list))
                        {
                            list.Add(o);
                        }
                        else
                        {
                            tagsDict.Add(tag, new List<IXenObject>());
                            tagsDict[tag].Add(o);
                        }
                    }
                }
            }

            return tagsDict;
        }

        public static String[] GetAllTags()
        {
            Dictionary<String, int> tagsDict = GetTagCounts();

            String[] tags = new String[tagsDict.Keys.Count];
            tagsDict.Keys.CopyTo(tags, 0);

            return tags;
        }
    }
}
