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
using System.Diagnostics;
using System.Xml;

using XenAPI;
using XenAdmin.Core;

namespace XenAdmin.Plugins
{
    internal class PowerShellCmd : ShellCmd
    {
        /// <summary>
        /// (Optional) "debug" attribute on the "PowerShellCmd" tag
        /// </summary>
        public readonly bool Debug;

        /// <summary>
        /// (Optional) "function" attribute on the "PowerShellCmd" tag
        /// </summary>
        public readonly string Function;

        public const string ATT_DEBUG = "debug";
        public const string ATT_FUNCTION = "function";

        public PowerShellCmd(XmlNode node, List<string> extraParams)
            : base(node, extraParams)
        {
            Debug = Helpers.GetBoolXmlAttribute(node, ATT_DEBUG);
            Function = Helpers.GetStringXmlAttribute(node, ATT_FUNCTION);
        }

        public override Process CreateProcess(List<string> procParams, IList<IXenObject> targets)
        {
            Registry.AssertPowerShellInstalled();

            // get ourselves in the correct directory
            // put the parameters in the objInfoArray variable
            // exectute the plugin, with debugging if required
            string command = string.Format(
                                         "& {{ {0} }}",
                                         XenServerPowershellCmd.MakeInvocationExpression(Filename, Function, procParams, Params, targets, Debug));

            // finally we escape the entire command statement again as it is being passed into the -Command parameter in quotes
            command = XenServerPowershellCmd.EscapeQuotes(XenServerPowershellCmd.EscapeBackSlashes(command));

            Process proc = new Process();
            proc.StartInfo.FileName = PluginDescriptor.PowerShellExecutable;
            proc.StartInfo.Arguments = string.Format("-NoLogo -Command \"{0}\" {1}", command, string.Join(" ", new List<string>(Params).ToArray()));
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = !Window;
            proc.StartInfo.WindowStyle = !Window ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal;
            return proc;
        }
    }
}
