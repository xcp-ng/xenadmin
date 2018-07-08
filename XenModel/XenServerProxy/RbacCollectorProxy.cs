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
using XenAdmin.ServerDBs;
using XenAPI;

namespace XenAdmin.Core
{
    // A proxy for constructing a session that just collects up all the API calls it would normally do,
    // so that we can calculate what privileges would be required to do the calls for real.
    public class RbacCollectorProxy : IInvocationHandler
    {
        private static readonly ClassGenerator generator = new ClassGenerator(typeof(Proxy));
        private static readonly Type proxyType = generator.Generate();

        public static Proxy GetProxy(RbacMethodList rbacMethods)
        {
            return (Proxy)generator.CreateProxyInstance(proxyType, new RbacCollectorProxy(rbacMethods));
        }

        private RbacMethodList rbacMethods = new RbacMethodList();
        public RbacCollectorProxy(RbacMethodList rbacMethods)
        {
            this.rbacMethods = rbacMethods;
        }

        public object Invoke(string proxyMethodName, string returnType, params object[] args)
        {
             if(proxyMethodName== "Url")
                    return null;
            ProxyMethodInfo pmi = SimpleProxyMethodParser.ParseTypeAndNameOnly(proxyMethodName);
            string method = pmi.TypeName + "." + pmi.MethodName;
            
            if (proxyMethodName == "task_get_record")
            {
                Proxy_Task task = new Proxy_Task() { progress = 100, status=XenAPI.task_status_type.success.ToString(),result = ""};
                return new Response<Proxy_Task>(task);
            }
            
            if (proxyMethodName == "host_call_plugin" && args != null && args.Length > 2 && "trim".Equals(args[2]))
                return new Response<string>("True");;

            if (pmi.MethodName == "add_to_other_config" || pmi.MethodName == "remove_from_other_config")  // these calls are special because they can have per-key permissions
                rbacMethods.Add(method, (string)args[2]);
            else
                rbacMethods.Add(method);
            
            return ResponseByType(returnType);
        }

        private object ResponseByType(string returnType)
        {
            switch (returnType)
            {
                case "String":
                    return new Response<string>("");
                case "String[]":
                    return new Response<string[]>(new string[0]);
                case "Proxy_Probe_result[]":
                    return new Response<Proxy_Probe_result[]>(new Proxy_Probe_result[0]);
                default:
                    System.Diagnostics.Trace.Assert(false);  // need to add more types here
                    return new Response<string>("");
            }
        }
    }
}
