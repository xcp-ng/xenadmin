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
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Plugins
{
    internal class XenServerPowershellCmd : ShellCmd
    {
        // TODO: CA:40580: These version numbers are not checked anywhere at the moment, and thus are not documented as part of the plugin spec
        /// <summary>
        /// optional - "version_min" attribute on the "XenServerPowerShellCmd" tag
        /// </summary>
        public readonly Version SnapInMinVersion;
#pragma warning disable 649, 169
        /// <summary>
        /// optional - "version_max" attribute on the "XenServerPowerShellCmd" tag
        /// </summary>
        public readonly Version SnapInMaxVersion;
#pragma warning restore 649, 169

        /// <summary>
        /// optional - "debug" attribute on the "XenServerPowerShellCmd" tag
        /// </summary>
        public readonly bool Debug;

        /// <summary>
        /// optional - "function" attribute on the "PowerShellCmd" tag
        /// </summary>
        public readonly string Function;

        public const string ATT_DEBUG = "debug";
        public const string ATT_VERSION_MIN = "version_min";
        public const string ATT_VERSION_MAX = "version_max";
        public const string ATT_FUNCTION = "function";

        private const string XEN_PARAM_ARRAY_VAR_NAME = "ObjInfoArray";
        private const string XEN_PARAM_ARRAY_HASH_KEY_URL = "url";
        private const string XEN_PARAM_ARRAY_HASH_KEY_SESSION_REF = "sessionRef";
        private const string XEN_PARAM_ARRAY_HASH_KEY_CLASS = "class";
        private const string XEN_PARAM_ARRAY_HASH_KEY_OBJECT_UUID = "objUuid";

        private const string USER_PARAM_ARRAY_VAR_NAME = "ParamArray";

        // This is the parameter to a string.Format call, so the double braces come out as single braces in the PowerShell.
        private const string DebugFunction =
            "function __XenCenter_DebugPlugin {{" +
            "  trap [Exception] {{" +
            "    write-host $($_.Exception.GetType().FullName); " +
            "    write-host $($_.Exception.Message);" +
            "    read-host \"[Press Enter to Exit]\" " +
            "  }};" +
            "  {0};" +
            "}};" +
            "__XenCenter_DebugPlugin";

        private const string InvokeExpression =
            "Invoke-Expression $([System.String]::Join([System.Environment]::NewLine, (Get-Content -Path \"{0}\"))); {1}";

        public XenServerPowershellCmd(XmlNode node, List<string> extraParams)
            : base(node, extraParams)
        {
            Debug = Helpers.GetBoolXmlAttribute(node, ATT_DEBUG, false);
            string versionString = Helpers.GetStringXmlAttribute(node, ATT_VERSION_MIN);
            if (versionString != null)
                SnapInMinVersion = new Version(versionString);

            versionString = Helpers.GetStringXmlAttribute(node, ATT_VERSION_MAX);
            if (versionString != null)
                SnapInMinVersion = new Version(versionString);

            Function = Helpers.GetStringXmlAttribute(node, ATT_FUNCTION);

            Registry.CheckXenServerPSSnapIn();
        }

        public override Process CreateProcess(List<string> procParams, IList<IXenObject> targets)
        {
            Registry.CheckXenServerPSSnapIn();
            string snapin = Registry.XenServerSnapInLocation();
            // execute the initialize script
            // get ourselves in the correct directory
            // put the parameters in the objInfoArray variable
            // exectute the plugin, with debugging if required
            string command = string.Format(
                              "& {{" +
                              "  . \"{0}\";" + 
                              "  {1}" +
                              "}}",
                              Path.Combine(Path.GetDirectoryName(snapin), "Initialize-Environment.ps1"),
                              MakeInvocationExpression(Filename, Function, procParams, Params, targets, Debug));

            // finally we escape the entire command statement again as it is being passed into the -Command parameter in quotes
            command = EscapeQuotes(EscapeBackSlashes(command));

            Process proc = new Process();
            proc.StartInfo.FileName = PluginDescriptor.PowerShellExecutable;
            proc.StartInfo.Arguments = string.Format("-PSConsoleFile \"{0}\" -NoLogo -Command \"{1}\"", 
                EscapeQuotes(snapin), 
                command);
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = !Window;
            proc.StartInfo.WindowStyle = !Window ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal;
            return proc;
        }

        public static string EscapeQuotes(string s)
        {
            // replacing " with \"
            return s.Replace("\"", "\\\"");
        }

        public static string EscapeBackSlashes(string s)
        {
            // replacing \ with \\
            return s.Replace("\\", "\\\\");
        }

        public static string MakeInvocationExpression(string filename, string function, IList<string> proc_params, IList<string> extra_params,
            IList<IXenObject> objs, bool debug)
        {
            string expression = string.Format(InvokeExpression,
                Placeholders.Substitute(filename, objs),
                Placeholders.Substitute(function ?? "", objs));
            string xenArrayStatement = XenArrayStatement(proc_params);
            string extraArrayStatement = ExtraArrayStatement(extra_params, objs);

            return string.Format("cd \"{0}\"; {1} {2} {3};",
                Program.AssemblyDir, xenArrayStatement, extraArrayStatement,
                debug ? string.Format(DebugFunction, expression) : expression);
        }

        private static string XenArrayStatement(IList<string> procParams)
        {
            // check how many objects we are passing in, each is a set of 4 strings (url, uuid, obj classname, obj uuid)
            int count = (int)(procParams.Count / 4);

            // now we form a statement that will initialise a powershell array in the format (a,b,c,d),(a2,b2,c2,d2),(a3,b3,c3,d3) e.t.c 
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("${0}=@(", XEN_PARAM_ARRAY_VAR_NAME));
            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                    sb.Append(',');
                int index = i * 4;
                sb.AppendFormat(" @{{{0}=\"{1}\"; {2}=\"{3}\"; {4}=\"{5}\"; {6}=\"{7}\"}}",
                    XEN_PARAM_ARRAY_HASH_KEY_URL, EscapeQuotes(EscapeBackSlashes(procParams[index])),
                    XEN_PARAM_ARRAY_HASH_KEY_SESSION_REF, EscapeQuotes(EscapeBackSlashes(procParams[index + 1])),
                    XEN_PARAM_ARRAY_HASH_KEY_CLASS, EscapeQuotes(EscapeBackSlashes(procParams[index + 2])),
                    XEN_PARAM_ARRAY_HASH_KEY_OBJECT_UUID, EscapeQuotes(EscapeBackSlashes(procParams[index + 3])));
            }
            sb.Append(");");
            return sb.ToString();
        }

        private static string ExtraArrayStatement(IList<string> extraParams, IList<IXenObject> objs)
        {
            // now we form a statement that will initialise a powershell array in the format (a,b,c,d,e,f,g)
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("${0}=@(", USER_PARAM_ARRAY_VAR_NAME));
            for (int i = 0; i < extraParams.Count; i++)
            {
                if (i > 0)
                    sb.Append(',');
                sb.Append('"');
                sb.Append(EscapeQuotes(EscapeBackSlashes(Placeholders.Substitute(extraParams[i], objs))));
                sb.Append('"');
            }
            sb.Append(");");

            return sb.ToString();
        }
    }
}
