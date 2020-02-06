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
using System.IO;
using XenAPI;

using XenAdmin.Core;
using XenAdmin.Model;
using XenAdmin.Network;
using XenCenterLib;

namespace XenAdmin.XenSearch
{
    public class Search : IComparable<Search>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly bool defaultSearch = false;

        private readonly Query query;
        private readonly Grouping grouping;

        private IXenConnection connection;

        // This is a list because order is important. More a list of tuples than a dictionary.
        // It could be null
        private readonly List<KeyValuePair<String, int>> columns;

        private readonly Sort[] sorting;

        // These will initially be set to null, to indicate they need populating when saving
        private String name;
        private String uuid;
        private int items;

        public Search(Query query, Grouping grouping, string name, string uuid, bool defaultSearch)
            : this(query, grouping, name, uuid, null, new Sort[] { })
        {
            this.defaultSearch = defaultSearch;
        }

        public Search(Query query, Grouping grouping, string name, 
            string uuid, List<KeyValuePair<string, int>> columns, Sort[] sorting)
        {
            if (query == null)
                this.query = new Query(null, null);
            else
                this.query = query;
            this.grouping = grouping;
            this.name = name;
            this.uuid = uuid; // This can be null
            this.columns = columns;
            this.sorting = sorting;
            this.connection = null;
        }

        public bool DefaultSearch
        {
            get
            {
                return defaultSearch;
            }
        }

        public String UUID
        {
            get
            {
                return uuid;
            }
        }

        public Query Query
        {
            get
            {
                return query;
            }
        }

        // The grouping from the user's point of view (i.e., according to the UI).
        public Grouping Grouping
        {
            get
            {
                return grouping;
            }
        }

        // The grouping we actually use internally. This is different because of CA-26708:
        // if we show the folder navigator, we don't also show the ancestor folders in the
        // main results, but we still pretend to the user that it's grouped by folder.
        public Grouping EffectiveGrouping
        {
            get
            {
                return (FolderForNavigator == null ? Grouping : null);
            }
        }

        public String Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public List<KeyValuePair<String, int>> Columns
        {
            get
            {
                return columns;
            }
        }

        public Sort[] Sorting
        {
            get
            {
                return sorting;
            }
        }

        public int Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
            }
        }

        public IXenConnection Connection
        {
            get
            {
                return connection;
            }
            set
            {
                connection = value;
            }
        }

        // Do we want the FolderNavigator at the top of the results?
        // If so, return the folder to display there. If not, return null.
        public string FolderForNavigator
        {
            get
            {
                if (Query == null || Query.QueryFilter == null)
                    return null;

                RecursiveXMOPropertyQuery<Folder> filter = Query.QueryFilter as RecursiveXMOPropertyQuery<Folder>;
                if (filter == null)
                    return null;  // only show a folder for RecursiveXMOPropertyQuery<Folder>

                StringPropertyQuery subFilter = filter.subQuery as StringPropertyQuery;
                if (subFilter == null || subFilter.property != PropertyNames.uuid)
                    return null;  // also only show a folder if the subquery is "folder is"

                return subFilter.query;
            }
        }

        public bool PopulateAdapters(params IAcceptGroups[] adapters)
        {
            Group group = Group.GetGrouped(this);
            bool added = false;

            foreach (IAcceptGroups adapter in adapters)
                added |= group.Populate(adapter);

            return added;
        }

        public override bool Equals(object obj)
        {
            Search other = obj as Search;
            if (other == null)
                return false;

            // Ifs are expanded to aid debuggin'

            if (!((name == null && other.name == null) || (name != null && name.Equals(other.Name))))
                return false;

            if (!((query == null && other.query == null) || (query != null && query.Equals(other.Query))))
                return false;

            if (!((grouping == null && other.grouping == null) || (grouping != null && grouping.Equals(other.grouping))))
                return false;

            if (!((sorting == null && other.sorting == null) || (Helper.AreEqual(sorting, other.sorting))))
                return false;

            if (!((connection == null && other.connection == null) || (connection != null && connection.Equals(other.connection))))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return grouping == null ? query.GetHashCode() : (query.GetHashCode() + 1) * grouping.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

        #region IComparable<Search> Members

        public int CompareTo(Search other)
        {
            int i = DefaultSearch.CompareTo(other.DefaultSearch);
            if (i == 0)
                return Name.CompareTo(other.Name);

            return i;
        }

        #endregion

        public String GetXML()
        {
            return SearchMarshalling.SearchToXML(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="o"></param>
        /// <param name="query"></param>
        public bool Save()
        {
            InvokeHelper.AssertOffEventThread();

            if (uuid == null)
                uuid = System.Guid.NewGuid().ToString();

            String key = SearchPrefix + uuid;
            String value = GetXML();

            if (connection == null)
                return false;

            if (!connection.IsConnected)
                return false;

            Session session = connection.DuplicateSession();
            foreach (Pool pool in connection.Cache.Pools)
            {
                Pool.remove_from_gui_config(session, pool.opaque_ref, key);
                Pool.add_to_gui_config(session, pool.opaque_ref, key, value);

                return true;
            }

            return false;
        }

        public void Save(String filename)
        {
            if (uuid == null)
                uuid = Guid.NewGuid().ToString();

            List<Search> l = new List<Search>();
            l.Add(this);
            String xml = SearchMarshalling.SearchesToXML(l);

            StreamWriter stream = new StreamWriter(filename, false);
            try
            {
                stream.WriteLine(xml);
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }

        public void Delete()
        {
            String key = SearchPrefix + uuid;

            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                if (!connection.IsConnected)
                    continue;

                Session session = connection.DuplicateSession();
                foreach (Pool pool in connection.Cache.Pools)
                {
                    Pool.remove_from_gui_config(session, pool.opaque_ref, key);
                }
            }

            InvokeHelper.Invoke(delegate()
                {
                    searches.Remove(uuid);
                    OnSearchesChanged();
                });
        }

        // Make a new search which is the same as the current search but with an additional filter
        public Search AddFilter(QueryFilter addFilter)
        {
            QueryScope scope = (Query == null ? null : Query.QueryScope);

            QueryFilter filter;
            if (Query == null || Query.QueryFilter == null)
                filter = addFilter;
            else if (addFilter == null)
                filter = Query.QueryFilter;
            else
                filter = new GroupQuery(new QueryFilter[] { Query.QueryFilter, addFilter }, GroupQuery.GroupQueryType.And);

            return new Search(new Query(scope, filter), Grouping, "", "", Columns, Sorting);
        }

        /// <summary>
        /// Adds a text filter that searches through several relevant fields
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public Search AddFullTextFilter(string s)
        {
            if (string.IsNullOrEmpty(s))
                return this;
            return AddFilter(FullQueryFor(s));
        }

        public static List<Search> LoadFile(String filename)
        {
            try
            {
                StreamReader stream = new StreamReader(filename);
                try
                {
                    return SearchMarshalling.LoadSearches(stream.ReadToEnd());
                }
                finally
                {
                    stream.Close();
                    stream.Dispose();
                }
            }
            catch (Exception e)
            {
                log.Debug($"Exception loading search from file '{filename}'", e);
                return null;
            }
        }

        public static Search SearchFor(IEnumerable<IXenObject> objects)
        {
            if(objects==null)
                return SearchFor((IXenObject)null);

            List<IXenObject> objectList = new List<IXenObject>(objects);

            if (objectList.Count == 0)
            {
                return SearchFor((IXenObject)null);
            }
            else if (objectList.Count == 1)
            {
                return SearchFor(objectList[0]);
            }
            else
            {
                bool containsHost = false;
                bool containsPool = false;

                List<QueryFilter> queryFilters = new List<QueryFilter>();
                foreach (IXenObject obj in objects)
                {
                    Pool poolAncestor = obj != null ? Helpers.GetPool(obj.Connection) : null;

                    if (poolAncestor != null)
                    {
                        containsPool = true;
                        QueryFilter uuidQuery = new StringPropertyQuery(PropertyNames.uuid, Helpers.GetUuid(poolAncestor), StringPropertyQuery.PropertyQueryType.exactmatch, true);
                        queryFilters.Add(new RecursiveXMOPropertyQuery<Pool>(PropertyNames.pool, uuidQuery));
                    }
                    else 
                    {
                        Host hostAncestor = Helpers.GetHostAncestor(obj);
                        if (hostAncestor != null)
                        {
                            containsHost = true;
                            QueryFilter uuidQuery = new StringPropertyQuery(PropertyNames.uuid, Helpers.GetUuid(hostAncestor), StringPropertyQuery.PropertyQueryType.exactmatch, true);
                            queryFilters.Add(new RecursiveXMOListPropertyQuery<Host>(PropertyNames.host, uuidQuery));
                        }
                    }

                }
                Grouping grouping = null;
                if (containsPool)
                {
                    Grouping hostGrouping = new XenModelObjectPropertyGrouping<Host>(PropertyNames.host, null);
                    grouping = new XenModelObjectPropertyGrouping<Pool>(PropertyNames.pool, hostGrouping);
                }
                else if (containsHost)
                {
                    grouping = new XenModelObjectPropertyGrouping<Host>(PropertyNames.host, null);
                }

                GroupQuery groupQuery = new GroupQuery(queryFilters.ToArray(), GroupQuery.GroupQueryType.Or);
                Query query = new Query(GetOverviewScope(), groupQuery);

                return new Search(query, grouping, Messages.SEARCH_TITLE_OVERVIEW, null, false);
            }
        }

        /// <summary>
        /// This gets the default search for the overview panel when an object in the tree is selected.
        /// Pass null as value and you'll get the default overview
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Search SearchFor(IXenObject value)
        {
            var scope = GetOverviewScopeExcludingGivenTypes(ObjectTypes.DockerContainer);
            var search = SearchFor(value, scope);
            
            return search;
        }

        /// <summary>
        /// This gets the default search for the overview panel when an object in the tree is selected.
        /// Pass null as value and you'll get the default search (this could be the default overview or default treeview search, 
        /// depending on the value of scope).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Search SearchFor(IXenObject value, QueryScope scope)
        {
            if (value is Host)
            {
                Grouping hostGrouping = new XenModelObjectPropertyGrouping<Host>(PropertyNames.host, null);

                QueryFilter uuidQuery = new StringPropertyQuery(PropertyNames.uuid, Helpers.GetUuid(value), StringPropertyQuery.PropertyQueryType.exactmatch, true);
                QueryFilter hostQuery = new RecursiveXMOListPropertyQuery<Host>(PropertyNames.host, uuidQuery);

                Query query = new Query(scope, hostQuery);
                return new Search(query, hostGrouping, String.Format(Messages.SEARCH_TITLE_HOST, Helpers.GetName(value)), null, false);
            }
            else if (value is Pool)
            {
                Grouping hostGrouping = new XenModelObjectPropertyGrouping<Host>(PropertyNames.host, null);
                Grouping poolGrouping = new XenModelObjectPropertyGrouping<Pool>(PropertyNames.pool, hostGrouping);

                QueryFilter uuidQuery = new StringPropertyQuery(PropertyNames.uuid, Helpers.GetUuid(value), StringPropertyQuery.PropertyQueryType.exactmatch, true);
                QueryFilter poolQuery = new RecursiveXMOPropertyQuery<Pool>(PropertyNames.pool, uuidQuery);

                Query query = new Query(scope, poolQuery);
                return new Search(query, poolGrouping, String.Format(Messages.SEARCH_TITLE_POOL, Helpers.GetName(value)), null, false);
            }
            else if (value is Folder)
            {
                Folder folder = value as Folder;
                return Search.SearchForFolder(folder.opaque_ref);
            }
            else
            {
                // This is the default search on the treeview

                Grouping dockervmGrouping = new XenModelObjectPropertyGrouping<VM>(PropertyNames.dockervm, null);
                Grouping hostGrouping = new XenModelObjectPropertyGrouping<Host>(PropertyNames.host, dockervmGrouping);
                Grouping poolGrouping = new XenModelObjectPropertyGrouping<Pool>(PropertyNames.pool, hostGrouping);

                return new Search(new Query(scope, null),
                    poolGrouping, String.Format(Messages.SEARCH_TITLE_OVERVIEW), null, false);
            }
        }

        public static ObjectTypes DefaultObjectTypes()
        {
            ObjectTypes types = ObjectTypes.DisconnectedServer | ObjectTypes.Server | ObjectTypes.VM |
                                ObjectTypes.RemoteSR | ObjectTypes.DockerContainer;

            return types;
        }

        internal static QueryScope GetOverviewScope()
        {
            ObjectTypes types = DefaultObjectTypes();

            // To avoid excessive number of options in the search-for drop-down,
            // the search panel doesn't respond to the options on the View menu.
            types |= ObjectTypes.UserTemplate;

            return new QueryScope(types);
        }

        internal static QueryScope GetOverviewScopeExcludingGivenTypes(ObjectTypes excludedTypes)
        {
            QueryScope overviewScope = GetOverviewScope();

            return new QueryScope(overviewScope.ObjectTypes & ~excludedTypes);
        }

        public static QueryFilter FullQueryFor(String p)
        {
            String[] elts = p.Split(new char[] { ' ' });
            List<QueryFilter> queries = new List<QueryFilter>();

            foreach (String elt in elts)
            {
                if (String.IsNullOrEmpty(elt))
                    continue;

                queries.Add(new StringPropertyQuery(PropertyNames.label, elt,
                    StringPropertyQuery.PropertyQueryType.contains, false));
                queries.Add(new StringPropertyQuery(PropertyNames.description, elt,
                    StringPropertyQuery.PropertyQueryType.contains, false));

                ComparableAddress address;
                if (!ComparableAddress.TryParse(elt, true, false, out address))
                    continue;

                queries.Add(new IPAddressQuery(PropertyNames.ip_address, address));
            }

            if (queries.Count == 0)
                queries.Add(new StringPropertyQuery(PropertyNames.label, "",
                    StringPropertyQuery.PropertyQueryType.contains, false));

            return new GroupQuery(queries.ToArray(), GroupQuery.GroupQueryType.Or);
        }

        public static Search SearchForTag(string tag)
        {
            Query tagQuery = new Query(null, new StringListContainsQuery(PropertyNames.tags, tag, true));
           
            return new Search(tagQuery, null, String.Format(Messages.OBJECTS_WITH_TAG, tag), null, false);
        }

        public static Search SearchForFolder(string path)
        {
            QueryScope scope = new QueryScope(ObjectTypes.AllIncFolders);
            QueryFilter innerFilter = new StringPropertyQuery(PropertyNames.uuid, path,
                    StringPropertyQuery.PropertyQueryType.exactmatch, true);
            QueryFilter filter = new RecursiveXMOPropertyQuery<Folder>(PropertyNames.folder, innerFilter);
            Query q = new Query(scope, filter);
            Grouping grouping = new FolderGrouping((Grouping)null);
            string[] pathParts = Folders.PointToPath(path);
            string name = ((pathParts.Length == 0 || (pathParts.Length == 1 && pathParts[pathParts.Length - 1] == String.Empty)) ?
                Messages.FOLDERS : pathParts[pathParts.Length - 1]);
            return new Search(q, grouping, name, null, false);
        }

        public static Search SearchForAllFolders()
        {
            Query query = new Query(new QueryScope(ObjectTypes.Folder), null);
            FolderGrouping grouping = new FolderGrouping((Grouping)null);
            Sort sort = new Sort("name", true);
            Sort[] sorts = { sort };
            return new Search(query, grouping, "", "", null, sorts);
        }

        public static Search SearchForAllTypes()
        {
            Query query = new Query(new QueryScope(ObjectTypes.AllExcFolders), null);
            return new Search(query, null, "", null, false);
        }

        public static Search SearchForTags()
        {
            var tagsQuery = new ListEmptyQuery<String>(PropertyNames.tags, false);
            Query query = new Query(new QueryScope(ObjectTypes.AllIncFolders), tagsQuery);
            return new Search(query, null, "", null, false);
        }

        public static Search SearchForFolders()
        {
            var foldersQuery = new NullQuery<Folder>(PropertyNames.folder, false);
            Query query = new Query(new QueryScope(ObjectTypes.AllIncFolders), foldersQuery);
            return new Search(query, null, "", null, false);
        }

        public static Search SearchForCustomFields()
        {
            var fieldsQuery = new BooleanQuery(PropertyNames.has_custom_fields, true);
            Query query = new Query(new QueryScope(ObjectTypes.AllIncFolders), fieldsQuery);
            return new Search(query, null, "", null, false);
        }

        public static Search SearchForVapps()
        {
            var vAppsQuery = new BooleanQuery(PropertyNames.in_any_appliance, true);
            Query query = new Query(new QueryScope(ObjectTypes.AllIncFolders), vAppsQuery);
            return new Search(query, null, "", null, false);
        }

        public static Search SearchForFolderGroup(Grouping grouping, object parent, object v)
        {
            return new Search(new Query(new QueryScope(ObjectTypes.AllIncFolders), grouping.GetSubquery(parent, v)),
                              grouping.GetSubgrouping(v), grouping.GetGroupName(v), "", false);
        }

        public static Search SearchForNonVappGroup(Grouping grouping, object parent, object v)
        {
            return new Search(new Query(new QueryScope(ObjectTypes.AllExcFolders), grouping.GetSubquery(parent, v)),
                              grouping.GetSubgrouping(v), grouping.GetGroupName(v), "", false);
        }

        public static Search SearchForVappGroup(Grouping grouping, object parent, object v)
        {
            return new Search(new Query(new QueryScope(ObjectTypes.VM), grouping.GetSubquery(parent, v)),
                              grouping.GetSubgrouping(v), grouping.GetGroupName(v), "", false);
        }

        private static Dictionary<String, Search> searches =new Dictionary<string, Search>();
        public static string BrandedSearchKey { get; private set; }
        public static event Action SearchesChanged;

        private static void OnSearchesChanged()
        {
            if (SearchesChanged != null)
                SearchesChanged();
        }

        public static Search[] Searches
        {
            get
            {
                lock (searches)
                {
                    Search[] searchArray = new Search[searches.Values.Count];
                    searches.Values.CopyTo(searchArray, 0);
                    return searchArray;
                }
            }
        }

        public static void InitSearch(string brandedSearchKey)
        {
            searches = new Dictionary<String, Search>();
            BrandedSearchKey = brandedSearchKey;

            InitDefaultSearches();

            ConnectionsManager.XenConnections.CollectionChanged += CollectionChanged;
            SynchroniseSearches();
        }
        
        private static void CollectionChanged(Object sender, System.ComponentModel.CollectionChangeEventArgs e)
        {
            //InvokeHelper.AssertOnEventThread();
            InvokeHelper.BeginInvoke(SynchroniseSearches);
        }

        static void Pool_BatchCollectionChanged(object sender, EventArgs e)
        {
            SynchroniseSearches();
        }

        private static string SearchPrefix
        {
            get { return BrandedSearchKey + ".Search-"; }
        }

        private static void SynchroniseSearches()
        {
            Dictionary<String, Search> localSearches = new Dictionary<String, Search>();
            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                connection.Cache.RegisterBatchCollectionChanged<Pool>(Pool_BatchCollectionChanged);

                foreach (Pool pool in connection.Cache.Pools)
                    foreach (KeyValuePair<String, String> kvp in pool.gui_config)
                    {
                        if (!kvp.Key.StartsWith(SearchPrefix))
                            continue;

                        String uuid = kvp.Key.Substring(SearchPrefix.Length);
                        Search search = SearchMarshalling.LoadSearch(kvp.Value);
                        if (search == null)
                            continue;

                        search.Connection = connection;

                        localSearches[uuid] = search;
                    }
            }

            bool changed = false;

            lock (searches)
            {
                String[] uuids = new String[searches.Keys.Count];
                searches.Keys.CopyTo(uuids, 0);
                foreach (String uuid in uuids)
                {
                    if (searches[uuid].DefaultSearch)
                        continue;

                    if (localSearches.ContainsKey(uuid))
                        continue;

                    searches.Remove(uuid);
                    changed = true;
                }

                foreach (Search search in localSearches.Values)
                {
                    if (searches.ContainsKey(search.UUID))
                    {
                        if (searches[search.UUID].Equals(search))
                            continue;

                        // try and persist the number of items in a
                        // search, so they don't flick back to 0
                        search.items = searches[search.UUID].items;
                    }

                    searches[search.UUID] = search;
                    changed = true;
                }
            }

            if (changed)
                OnSearchesChanged();
        }

        private static void InitDefaultSearches()
        {
            // VMs by OS

            Search VMsByOS = new Search(
                new Query(
                    new QueryScope(ObjectTypes.VM),
                    null),
                new PropertyGrouping<String>(PropertyNames.os_name, null),
                Messages.DEFAULT_SEARCH_VMS_BY_OS, "dead-beef-1234-vmsbyos", true
            );

            searches["dead-beef-1234-vmsbyos"] = VMsByOS;

            // VMs without tools

            Search VMsWithoutTools = new Search(
                new Query(
                    new QueryScope(ObjectTypes.VM),
                    new GroupQuery(
                        new QueryFilter[] {
                            new EnumPropertyQuery<vm_power_state>(PropertyNames.power_state, vm_power_state.Running, true),
                            new EnumPropertyQuery<VM.VirtualisationStatus>(PropertyNames.virtualisation_status, VM.VirtualisationStatus.IO_DRIVERS_INSTALLED | VM.VirtualisationStatus.MANAGEMENT_INSTALLED, false)
                        }, GroupQuery.GroupQueryType.And)),
                new PropertyGrouping<VM.VirtualisationStatus>(PropertyNames.virtualisation_status, null),
                Messages.DEFAULT_SEARCH_VMS_WO_XS_TOOLS, "dead-beef-1234-vmswotools", true
            );

            searches["dead-beef-1234-vmswotools"] = VMsWithoutTools;

            // VMs by power state

            Search VMsByPowerState = new Search(
                new Query(
                    new QueryScope(ObjectTypes.VM),
                    null),
                new PropertyGrouping<vm_power_state>(PropertyNames.power_state, null),
                Messages.DEFAULT_SEARCH_VMS_BY_POWERSTATE, "dead-beef-1234-vmsbyps", true
            );

            searches["dead-beef-1234-vmsbyps"] = VMsByPowerState;

            // VMs by network

            Search VMsByNetwork = new Search(
                new Query(
                    new QueryScope(ObjectTypes.VM),
                    null),
                new XenModelObjectPropertyGrouping<XenAPI.Network>(PropertyNames.networks, null),
                Messages.DEFAULT_SEARCH_VMS_BY_NETWORK, "dead-beef-1234-vmsbynet", true
            );

            searches["dead-beef-1234-vmsbynet"] = VMsByNetwork;

			//VMs by vApps

            Search VMsByAppliance = new Search(new Query(new QueryScope(ObjectTypes.VM), new BooleanQuery(PropertyNames.in_any_appliance, true)),
                new XenModelObjectPropertyGrouping<VM_appliance>(PropertyNames.appliance, null),
                Messages.DEFAULT_SEARCH_VMS_BY_APPLIANCE, "dead-beef-1234-vmsbyappliance", true);

        	searches["dead-beef-1234-vmsbyappliance"] = VMsByAppliance;

            // Objects by Tag

            Search ObjByTag = new Search(
                new Query(
                    null,
                    new ListEmptyQuery<String>(PropertyNames.tags, false)),
                new PropertyGrouping<String>(PropertyNames.tags, null),
                Messages.DEFAULT_SEARCH_OBJECTS_BY_TAG, "dead-beef-1234-objbytags", true
            );

            searches["dead-beef-1234-objbytags"] = ObjByTag;

            // Snapshots by VM

            Search SnapshotsByVM = new Search(
                new Query(new QueryScope(ObjectTypes.Snapshot | ObjectTypes.VM), null),
                new XenModelObjectPropertyGrouping<Pool>(PropertyNames.pool,
                    new XenModelObjectPropertyGrouping<Host>(PropertyNames.host,
                        new XenModelObjectPropertyGrouping<VM>(PropertyNames.vm, null))),
                Messages.DEFAULT_SEARCH_SNAPSHOTS_BY_VM,
                "dead-beef-1234-snapshotsbyvm",
                true);

            searches["dead-beef-1234-snapshotsbyvm"] = SnapshotsByVM;

            /*
            //Docker containers by VM
            Search dockerContainersByVM = new Search(
                new Query(new QueryScope(ObjectTypes.DockerContainer | ObjectTypes.VM), null),
                new XenModelObjectPropertyGrouping<Pool>(PropertyNames.pool,
                    new XenModelObjectPropertyGrouping<Host>(PropertyNames.host,
                        new XenModelObjectPropertyGrouping<VM>(PropertyNames.vm, null))),
                false,
                "Default search docker containers by vm",
                "dead-beef-1234-dockerbyvm",
                true);

            searches["dead-beef-1234-dockerbyvm"] = dockerContainersByVM;
            */
        }
    }
}
