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

using XenAdmin.Core;
using XenAdmin.Model;
using XenAdmin.Network;


namespace XenAdmin.Actions
{
    public abstract class FolderAction : AsyncAction
    {
        // CA-34379 Many folder operations do not work well with RBAC, because they operate across multiple connections.
        // Our strategy is roughly as follows:
        //
        // * As always, each Action has a designated Session. This may be an escalated Session in the case that we have
        //   sudo'ed, or passed in an escalated Session through RunExternal(). Sometimes, for example adding several
        //   objects to a folder, we can break the work down into several actions which can be escalated separately.
        //
        // * However, many actions, such as deleting or renaming a whole folder, inevitably operate across multiple
        //   connections. In that case we use this.Session for the relevant connection, and build up a dictionary of
        //   sessions for other connections using DuplicateSession(). However, these sessions are not escalated, so
        //   we block any such actions in the UI if we would need to operate on a read-only connection: otherwise we
        //   would sudo and then the action would still fail on the non-primary connection. (We could in principle
        //   escalate a whole collection of Sessions and pass them in, but it doesn't seem necessary, and perhaps
        //   it would be a bit odd because when renaming a folder, you feel you're operating on that folder, not really
        //   on the objects within it. For consistency, we even block the operation when all the objects are on a
        //   single read-only connection).
        //
        // * It's not actually quite true that adding an object to a folder only operates on that object's connection. If
        //   the folder was empty before, we should also now remove it from the empty folders list. But it's harmless to
        //   leave extra empty folders in the list, so we just catch the RBAC error and fail silently.
        //
        // * For safety, destructive operations are called last: duplication is not as bad as destructive failure.
        //
        // * When this action is used to move or delete multiple objects across multiple connections, 
        //   the action can not be sudo'ed as it does not have a single session that can be escalated. In this the action
        //   would fail if any of the connections does not have sufficient roles.
        //

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Dictionary<IXenConnection, Session> Sessions = new Dictionary<IXenConnection, Session>();

        protected FolderAction(IXenConnection connection, string title)
            : base(connection, title)
        {
            if (connection != null)
            {
                ApiMethodsToRoleCheck.Add("pool.remove_from_other_config", Folders.EMPTY_FOLDERS);
                ApiMethodsToRoleCheck.Add("pool.add_to_other_config", Folders.EMPTY_FOLDERS);
            }
        }
        
        public override void RecomputeCanCancel()
        {
            // CanCancel is always true.
        }

        private readonly Dictionary<IXenConnection, List<string>> emptyFolders = new Dictionary<IXenConnection, List<string>>();

       /// <summary>
       /// Mark a folder as empty. The folder will be added to a collection of empty folders that will be used later to update the server
       /// </summary>
       protected void MarkEmptyFolder(IXenConnection connection, string folder)
        {
            if (!emptyFolders.ContainsKey(connection))
                emptyFolders.Add(connection, new List<string>());
            emptyFolders[connection].Add(folder);
        }

        private readonly List<string> nonEmptyFolders = new List<string>();

        /// <summary>
        /// Mark a folder as non-empty. The folder will be added to a collection of non-empty folders that will be used later to update the server
        /// </summary>
        protected void MarkNonEmptyFolder(string folder)
        {
            nonEmptyFolders.Add(folder);
        }
        
        /// <summary>
        /// Update the EMPTY_FOLDERS property on all servers, using the emptyFolders and nonEmptyFolders collections
        /// </summary>
        protected void UpdateEmptyFolders(Func<bool> cancelling)
        {
            foreach (var con in emptyFolders.Keys)
            {
                AddFoldersToEmptyList(con, cancelling, emptyFolders[con].ToArray());
            }
            RemoveFoldersFromEmptyList(nonEmptyFolders.ToArray(), cancelling);
        }

        protected void DeleteOrMove(List<IXenObject> objects, Folder folder, Func<bool> cancelling)
        {
            XenConnection connection = null;
            if (objects.Count > 0)
                connection = objects[0].Connection as XenConnection;

            connection?.OnBeforeMajorChange(true);
            try
            {
                foreach(var obj in objects)
                {
                    DeleteOrMove(obj, folder, cancelling);
                }

                // Update EMPTY_FOLDERS on all servers
                UpdateEmptyFolders(cancelling);
            }
            finally
            {
                connection?.OnAfterMajorChange(true);
            }
        }

        protected void DeleteOrMove(IXenObject obj, Folder folder, Func<bool> cancelling)
        {
            // if, by moving candidate, we make its parent
            // empty, then we must add from's parent to the empty list
            // CA-34379: Folder actions do not work with sudo. Make sure destructive operations are called last.

            string parent = obj.Path;
            if (!string.IsNullOrEmpty(parent))
            {
                if (new List<IXenObject>(Folders.Children(parent)).Count == 1)
                {
                    MarkEmptyFolder(obj.Connection, parent);
                }
            }

            MoveContents(obj, folder == null ? null : folder.opaque_ref, cancelling);
        }

        protected void Rename(IXenObject obj, string path, Func<bool> cancelling)
        {
            // do the whole thing in a BackgroundMajorChange so the treeview doesn't get updated while the update
            // is only partially completed.

            ((XenConnection)obj.Connection).OnBeforeMajorChange(true);
            try
            {
                // CA-34379: Folder actions do not work with sudo. Make sure destructive operations are called last.
                if (obj.opaque_ref != path)
                {
                    MarkEmptyFolder(obj.Connection, path);

                    foreach (IXenObject ixmo in Folders.Children(obj.opaque_ref))
                        MoveContents(ixmo, path, cancelling);

                    MarkNonEmptyFolder(obj.opaque_ref);
                    // Update EMPTY_FOLDERS
                    UpdateEmptyFolders(cancelling);
                }
            }
            finally
            {
                ((XenConnection)obj.Connection).OnAfterMajorChange(true);
            }
        }

        protected void MoveContents(IXenObject from, string to, Func<bool> cancelling)
        {
            // CA-34379: Folder actions do not work with sudo. Make sure destructive operations are called last.
            if (cancelling())
                throw new CancelledException();

            Folder folder = from as Folder;
            if (folder == null)
            {
                log.DebugFormat("Moving {0} to {1}", Helpers.GetName(from), to ?? "<null>");
                SetFolder(from, to, cancelling);
                MarkNonEmptyFolder(to);
                return;
            }

            string newLocation = to == null ? null : Folders.AppendPath(to, folder.name_label);

            log.DebugFormat("Moving contents of {0} to {1}", folder, newLocation ?? "<null>");

            bool empty = true;
            foreach (IXenObject ixmo in Folders.Children(folder.opaque_ref))
            {
                empty = false;

                MoveContents(ixmo, newLocation, cancelling);
            }

            MarkNonEmptyFolder(from.opaque_ref);

            if (empty && newLocation != null)
            {
                MarkEmptyFolder(from.Connection, newLocation);
            }
        }

        // WARNING: This function isn't thread safe. It can't be called twice in quick succession
        // on separate threads, because we have to wait for the new empty folders list to come back
        // from the server in between. Each thread does that in WaitForEmptyFoldersCacheChange(),
        // but that won't stop another thread getting the wrong list.
        //Returns true if anyfolder was add to the empty list
        protected bool AddFoldersToEmptyList(IXenConnection connection, Func<bool> cancelling, params string[] paths)
        {
            if (connection == null)
                return false;

            Pool pool = Helpers.GetPoolOfOne(connection);
            if (pool == null)
                return false;

            List<string> emptyFoldersOnThisConnection = new List<string>(Folders.GetEmptyFolders(pool));
            bool anyAdded = false;
            foreach (string path in paths)
            {
                if (!emptyFoldersOnThisConnection.Contains(path))
                {
                    log.DebugFormat("Adding {0} to empty list on {1}", path, Helpers.GetName(pool));
                    emptyFoldersOnThisConnection.Add(path);
                    anyAdded = true;
                }
            }
            if (anyAdded)
            {
                SetEmptyFolders(connection, emptyFoldersOnThisConnection, cancelling);
                return true;
            }
            return false;
        }

        private void SetFolder(IXenObject xmo, string to, Func<bool> cancelling)
        {
            Session sess = GetSession(xmo.Connection);
            if (to == null)
                Helpers.RemoveFromOtherConfig(sess, xmo, Folders.FOLDER);
            else
                Helpers.SetOtherConfig(sess, xmo, Folders.FOLDER, to);
            WaitForFolderCacheChange(xmo, to, cancelling);
        }

        private Session GetSession(IXenConnection conn)
        {
            // First we look at this.Session. This allows us to sudo if we have only one connection.
            if (Session != null && Session.Connection == conn)
                return Session;

            // Otherwise we dig into our dictionary of sessions for other connections. These cannot be sudo'ed,
            // because there is no good way to make a sudo dialog (or a series of dialogs) for several connections.
            if (Sessions.ContainsKey(conn))
                return Sessions[conn];
            Session s = conn.DuplicateSession();
            Sessions[conn] = s;
            return s;
        }

        private void RemoveFoldersFromEmptyList(string[] paths, Func<bool> cancelling)
        {
            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                if (!connection.IsConnected)
                    continue;

                List<string> emptyFoldersOnThisConnection = new List<string>(Folders.GetEmptyFolders(connection));
                
                bool somethingChanged = false;
                foreach(string path in paths)
                {
                    if (emptyFoldersOnThisConnection.Contains(path))
                    {
                        emptyFoldersOnThisConnection.Remove(path);
                        somethingChanged = true;
                    }
                }
                
                if (somethingChanged)
                {
                    try
                    {
                        SetEmptyFolders(connection, emptyFoldersOnThisConnection, cancelling);
                    }
                    // We ignore RBAC exceptions. They are caused by trying to remove a folder from
                    // the empty list on a read-only connection. Leaving an additional empty folder
                    // lying around is harmless: we don't want to report an error for it. See CA-40412
                    // for an example of this.
                    catch (Exception e)
                    {
                        Failure f = e as Failure;
                        if (f == null || f.ErrorDescription[0] != Failure.RBAC_PERMISSION_DENIED)
                            throw;
                    }
                }
            }
        }

        private void SetEmptyFolders(IXenConnection connection, List<string> folders, Func<bool> cancelling)
        {
            string folder_str = string.Join(Folders.EMPTY_FOLDERS_SEPARATOR, folders.ToArray());
            Pool pool = Helpers.GetPoolOfOne(connection);
            if (pool == null)
                throw new Failure(Failure.INTERNAL_ERROR, Messages.POOL_GONE);
            Helpers.SetOtherConfig(GetSession(pool.Connection), pool, Folders.EMPTY_FOLDERS, folder_str);
            WaitForEmptyFoldersCacheChange(pool, folder_str, cancelling);
        }

        private static void WaitForFolderCacheChange(IXenObject xmo, string expected, Func<bool> cancelling)
        {
            xmo.Connection.WaitFor(delegate()
            {
                return Folders.GetFolderString(xmo) == expected;
            },
                cancelling);
            if (cancelling())
                throw new CancelledException();
        }

        private static void WaitForEmptyFoldersCacheChange(Pool pool, string expected, Func<bool> cancelling)
        {
            pool.Connection.WaitFor(delegate()
            {
                return Folders.GetEmptyFoldersString(pool) == expected;
            },
                cancelling);
            if (cancelling())
                throw new CancelledException();
        }
    }

    public class CreateFolderAction : FolderAction
    {
        private readonly string[] paths;

        public CreateFolderAction(IXenConnection connection, params string[] paths)
            : base(connection, GetTitle(paths))
        {
            this.paths = paths;

            foreach (string path in paths)
                AppliesTo.Add(path);
        }

        public string[] NewPaths
        {
            get { return paths; }
        }

        protected override void Run()
        {
            CanCancel = true;

            Description = paths.Length == 1 ? Messages.CREATING_NEW_FOLDER : Messages.CREATING_NEW_FOLDERS;

            if (!AddFoldersToEmptyList(Connection, GetCancelling, paths))
                throw new Exception(Messages.FOLDER_ALREADY_EXISTS);

            Description = paths.Length == 1 ? Messages.CREATED_NEW_FOLDER : Messages.CREATED_NEW_FOLDERS;
        }

        private static string GetTitle(params string[] paths)
        {
            return paths.Length == 1
                ? string.Format(Messages.CREATE_NEW_FOLDER, paths[0])
                : string.Format(Messages.CREATE_NEW_FOLDERS, string.Join("; ", paths));
        }
    }

    public class RenameFolderAction : FolderAction
    {
        private readonly string path;
        protected readonly IXenObject obj;

        public RenameFolderAction(Folder folder, String name)
            : base(folder.Connection, string.Format(Messages.RENAMING_FOLDER, Helpers.GetName(folder), name))
        {
            obj = folder;
            path = Folders.AppendPath(obj.Path, name);
            AppliesTo.Add(obj.opaque_ref);
            AppliesTo.Add(path);
        }
        protected override void Run()
        {
            CanCancel = true;

            Description = Messages.RENAMING;

            Rename(obj, path, GetCancelling);

            Description = Messages.RENAMED;
        }
    }

    public class MoveToFolderAction : FolderAction
    {
        private readonly List<IXenObject> objs = new List<IXenObject>();
        private readonly Folder folder;

        public MoveToFolderAction(IXenObject obj, Folder folder)
            : base(obj.Connection, string.Format(Messages.MOVE_OBJECT_TO_FOLDER, Helpers.GetName(obj), folder.Name()))
        {
            this.objs.Add(obj);
            this.folder = folder;
            if (obj.GetType() != typeof(Folder))
            {
                ApiMethodsToRoleCheck.Add(obj.GetType().Name.ToLowerInvariant() + ".remove_from_other_config",
                    Folders.FOLDER);
                ApiMethodsToRoleCheck.Add(obj.GetType().Name.ToLowerInvariant() + ".add_to_other_config",
                    Folders.FOLDER);
            }

            AppliesTo.Add(obj.opaque_ref);
            AppliesTo.Add(folder.opaque_ref);
        }


        // Constructor used for moving multiple objects, across multiple connections
        public MoveToFolderAction(List<IXenObject> objs, Folder folder)
            : base(null, string.Format(Messages.MOVE_OBJECTS_TO_FOLDER, folder.Name()))
        {
            this.objs.AddRange(objs);
            this.folder = folder; 
            if (this.folder != null)
                AppliesTo.Add(this.folder.opaque_ref);
        }

        protected override void Run()
        {
            CanCancel = true;
            Description = Messages.MOVING;

            DeleteOrMove(objs, folder, GetCancelling);

            Description = Messages.MOVED;
        }
    }

    public class DeleteFolderAction : FolderAction
    {
        private readonly List<IXenObject> objs = new List<IXenObject>();

        public DeleteFolderAction(IXenObject obj)
            : base(obj.Connection, Messages.DELETING_FOLDER)
        {
            objs.Add(obj);
            if (obj.GetType() != typeof(Folder))
            {
                ApiMethodsToRoleCheck.Add(obj.GetType().Name.ToLowerInvariant() + ".remove_from_other_config",
                    Folders.FOLDER);
                ApiMethodsToRoleCheck.Add(obj.GetType().Name.ToLowerInvariant() + ".add_to_other_config",
                    Folders.FOLDER);
            }

            AppliesTo.Add(obj.opaque_ref);
        }

        // Constructor used for deleting multiple objects, across multiple connections
        public DeleteFolderAction(List<IXenObject> objs)
            : base(null, Messages.DELETING_FOLDERS)
        {
            this.objs.AddRange(objs);
        }

        protected override void Run()
        {
            CanCancel = true;
            Description = objs.Count == 1 ? Messages.DELETING_FOLDER : Messages.DELETING_FOLDERS;

            DeleteOrMove(objs, null, GetCancelling);

            Description = objs.Count == 1 ? Messages.DELETED_FOLDER : Messages.DELETED_FOLDERS;
        }
    }
}
