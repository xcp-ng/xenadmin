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
using System.Xml;
using System.Diagnostics;
using XenAPI;
using XenAdmin.Core;
using System.Collections.ObjectModel;


namespace XenAdmin.Plugins
{
    internal class ShellCmd
    {
        public readonly string Filename;     // required - "filename" attribute on the cmd's tag
        public readonly bool Window;         // optional - "window" attribute on the cmd's tag
        public readonly bool LogOutput;      // optional - "log_output" attribute on the cmd's tag
        public readonly ReadOnlyCollection<string> Params; // optional - "param" attribute, extra params that passed into the plugin
        private readonly float _disposeTime = 20.0f; // optional - how long we give the plugin to respond to a cancel request
        private readonly RbacMethodList requiredMethods; // optional - enables us to check the list of methods it wants to use against RBAC
        private readonly XmlNode node;

        // TODO: CA-40584: This feature is not documented in MR as it was introduced too late.
        private readonly string requiredMethodList; // optional - enables us to check the list of methods it wants to use against RBAC

        public const string ATT_FILENAME = "filename";
        public const string ATT_WINDOW = "window";
        public const string ATT_LOG_OUTPUT = "log_output";
        public const string ATT_DISPOSE_TIME = "dispose_time";
        public const string ATT_REQUIRED_METHODS = "required_methods";
        public const string ATT_REQUIRED_METHOD_LIST = "required_method_list";


        public ShellCmd(XmlNode node, List<string> extraParams)
        {
            this.node = node;
            Filename = Helpers.GetStringXmlAttribute(node, ATT_FILENAME);
            Window = Helpers.GetBoolXmlAttribute(node, ATT_WINDOW, true);
            LogOutput = Helpers.GetBoolXmlAttribute(node, ATT_LOG_OUTPUT);
            _disposeTime = Helpers.GetFloatXmlAttribute(node, ATT_DISPOSE_TIME, 20.0f);
            // The required methods to run this plugin are a comma separated list of strings that can be parsed by the RbacMethodList class
            requiredMethods = new RbacMethodList(Helpers.GetStringXmlAttribute(node, ATT_REQUIRED_METHODS, "").Split(','));
            requiredMethodList = Helpers.GetStringXmlAttribute(node, ATT_REQUIRED_METHOD_LIST);
            Params = new ReadOnlyCollection<string>(extraParams);
        }

		public ShellCmd(string filename, bool window, bool logoutput, float disposetime, string[] reqdMethods, string reqMethodList, string[] extraParams)
		{
			Filename = filename;
			Window = window;
			LogOutput = logoutput;
			_disposeTime = disposetime;
			// The required methods to run this plugin are a comma separated list of strings that can be parsed by the RbacMethodList class
			requiredMethods = new RbacMethodList(reqdMethods);
			requiredMethodList = reqMethodList;
			Params = new ReadOnlyCollection<string>(extraParams);
		}

    	public virtual string CheckForError()
        {
            if (string.IsNullOrEmpty(Filename))
                return string.Format(Messages.CANNOT_PARSE_NODE_PARAM, node.Name, ATT_FILENAME);

            return null;
        }

        

        public RbacMethodList RequiredMethods
        {
            get { return requiredMethods; }
        }

        /// <summary>
        /// The name of the method list to use defined as a child of the feature node in the plugin xml
        /// </summary>
        public string RequiredMethodList
        {
            get { return requiredMethodList; }
        }

        public float DisposeTime
        {
            get { return _disposeTime; }
        }

        /// <returns>bool.Parse of the given attribute on the given node, or def if the attribute is not present.</returns>
        private static bool BoolOption(XmlNode node, string attribute, bool def)
        {
            return node.Attributes[attribute] == null ? def : bool.Parse(node.Attributes[attribute].Value);
        }

        public virtual Process CreateProcess(List<string> procParams, IList<IXenObject> targets)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = Filename;
            // Targets can be null if the XenCenter node is being targetted, placeholders can cope with this
            proc.StartInfo.FileName = Placeholders.Substitute(Filename, targets);

            // 'Params' are defined in the plugin xml and may require substitution
            List<string> allParams = new List<string>(Params);
            for (int i = 0; i < allParams.Count; i++)
            {
                // sub in null, multi_target, or object properties depending on how many targets there are
                allParams[i] = Placeholders.Substitute(allParams[i], targets);
            }
            // 'procParams' come from ExternalPluginAction, and are tuples about each target (require no substitution)
            allParams.AddRange(procParams);
            proc.StartInfo.Arguments = string.Join(" ", allParams.ToArray());
            proc.StartInfo.UseShellExecute = !LogOutput;
            proc.StartInfo.CreateNoWindow = !Window;
            proc.StartInfo.WindowStyle = !Window ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal;
            proc.StartInfo.RedirectStandardOutput = LogOutput;
            proc.StartInfo.RedirectStandardError = LogOutput;
            return proc;
        }
    }
}
