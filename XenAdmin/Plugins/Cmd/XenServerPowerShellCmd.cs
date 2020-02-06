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
using System.Text;
using System.Xml;

using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Plugins
{
    internal class XenServerPowershellCmd : ShellCmd
    {
        /// <summary>
        /// optional - "debug" attribute on the "XenServerPowerShellCmd" tag
        /// </summary>
        public readonly bool Debug;

        /// <summary>
        /// optional - "function" attribute on the "PowerShellCmd" tag
        /// </summary>
        public readonly string Function;

        public const string ATT_DEBUG = "debug";
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

        public XenServerPowershellCmd(XmlNode node, List<string> extraParams)
            : base(node, extraParams)
        {
            Debug = Helpers.GetBoolXmlAttribute(node, ATT_DEBUG);
            Function = Helpers.GetStringXmlAttribute(node, ATT_FUNCTION);
        }

        public override Process CreateProcess(List<string> procParams, IList<IXenObject> targets)
        {
            Registry.AssertPowerShellInstalled();
            Registry.AssertPowerShellExecutionPolicyNonRestricted();

            string command = MakeInvocationExpression(Filename, Function, procParams, Params, targets, Debug);

            //escape the entire command statement as it is being passed into the -Command parameter in quotes
            command = EscapeQuotes(EscapeBackSlashes(command));

            return new Process
            {
                StartInfo =
                {
                    FileName = PluginDescriptor.PowerShellExecutable,
                    Arguments = $"-NoLogo -Command \"Import-Module XenServerPSModule; {command}\"",
                    UseShellExecute = false,
                    CreateNoWindow = !Window,
                    WindowStyle = Window ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden
                }
            };
        }

        public static string EscapeQuotes(string s)
        {
            return s.Replace("\"", "\\\"");
        }

        public static string EscapeBackSlashes(string s)
        {
            return s.Replace("\\", "\\\\");
        }

        public static string MakeInvocationExpression(string filename, string function, IList<string> proc_params,
            IList<string> extra_params, IList<IXenObject> objs, bool debug)
        {
            filename = Placeholders.Substitute(filename, objs);
            var expression = $"Invoke-Expression $(Get-Content -Path \"{filename}\" -Raw)";

            if (!string.IsNullOrEmpty(function))
                function = Placeholders.Substitute(function, objs);
            if (!string.IsNullOrEmpty(function))
                expression = $"{expression}; {function}";

            if (debug)
                expression = string.Format(DebugFunction, expression);

            string xenArrayStatement = XenArrayStatement(proc_params);
            string extraArrayStatement = ExtraArrayStatement(extra_params, objs);

            return $"cd \"{Program.AssemblyDir}\"; {xenArrayStatement} {extraArrayStatement} {expression};";
        }

        private static string XenArrayStatement(IList<string> procParams)
        {
            // check how many objects we are passing in, each is a set of 4 strings (url, uuid, obj classname, obj uuid)
            int count = (int)(procParams.Count / 4);

            // now we form a statement that will initialise a powershell array in the format (a,b,c,d),(a2,b2,c2,d2),(a3,b3,c3,d3) e.t.c 
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("${0}=@(", XEN_PARAM_ARRAY_VAR_NAME);

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
