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
using XenAPI;
using XenAdmin.Alerts;
using XenAdmin.Core;
using XenAdmin.Model;
using System.Linq;


namespace XenAdmin.Actions
{
    public class PerfmonDefinitionAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IXenObject xo;
        private readonly List<PerfmonDefinition> perfmonDefinitions;

        public PerfmonDefinitionAction(IXenObject xo, List<PerfmonDefinition> perfmonDefinitions, bool suppressHistory)
            : base(xo.Connection, Messages.ACTION_SAVE_ALERTS, string.Format(Messages.ACTION_SAVING_ALERTS_FOR, xo), suppressHistory)
        {
            this.xo = xo;
            this.perfmonDefinitions = perfmonDefinitions;

            string type = xo.GetType().Name.ToLowerInvariant();
            ApiMethodsToRoleCheck.Add(type + ".remove_from_other_config", PerfmonDefinition.PERFMON_KEY_NAME);
            if (perfmonDefinitions != null && perfmonDefinitions.Count != 0)
                ApiMethodsToRoleCheck.Add(type + ".add_to_other_config", PerfmonDefinition.PERFMON_KEY_NAME);
        }

        protected override void Run()
        {
            if (xo == null)
                return;
            Host theHost = xo as Host;

            // Dom0 Memory usage alert is an exception. While configuration for all the alerts (eg. related to the Host) have to be saved to this "xo", 
            // dom0 Memory usage's has to be in the Dom0's other config.
            //
            var dom0_memory_usage = perfmonDefinitions.FirstOrDefault(d => d.IsDom0MemoryUsage);
            if (dom0_memory_usage != null)
            {
                perfmonDefinitions.Remove(dom0_memory_usage);

                var dom0Vm = theHost == null ? null : theHost.ControlDomainZero();
                if (dom0Vm != null)
                {
                    var dom0PerfmonDefinitions = PerfmonDefinition.GetPerfmonDefinitions(dom0Vm).ToList();

                    bool found = false;
                    for (int ii = 0; ii < dom0PerfmonDefinitions.Count; ii++)
                    {
                        var pmd = dom0PerfmonDefinitions[ii];
                        if (pmd != null && pmd.IsDom0MemoryUsage)
                        {
                            dom0PerfmonDefinitions[ii] = dom0_memory_usage;
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        dom0PerfmonDefinitions.Add(dom0_memory_usage);
                    }

                    string dom0PerfmonConfigXML = PerfmonDefinition.GetPerfmonDefinitionXML(dom0PerfmonDefinitions);
                    Helpers.SetOtherConfig(Session, dom0Vm, PerfmonDefinition.PERFMON_KEY_NAME, dom0PerfmonConfigXML);
                }
            }
            else
            {
                var dom0Vm = theHost == null ? null : theHost.ControlDomainZero();
                if (dom0Vm != null)
                {
                    var dom0PerfmonDefinitions = PerfmonDefinition.GetPerfmonDefinitions(dom0Vm).ToList();

                    int found = dom0PerfmonDefinitions.RemoveAll(d => d.IsDom0MemoryUsage);
                    if (found > 0)
                    {
                        string dom0PerfmonDefinitionsXml = PerfmonDefinition.GetPerfmonDefinitionXML(dom0PerfmonDefinitions);
                        Helpers.SetOtherConfig(Session, dom0Vm, PerfmonDefinition.PERFMON_KEY_NAME, dom0PerfmonDefinitionsXml);
                    }
                }
            }

            if (perfmonDefinitions == null || perfmonDefinitions.Count == 0)
            {
                Helpers.RemoveFromOtherConfig(Session, xo, PerfmonDefinition.PERFMON_KEY_NAME);
            }
            else
            {
                string perfmonConfigXML = PerfmonDefinition.GetPerfmonDefinitionXML(perfmonDefinitions);
                Helpers.SetOtherConfig(Session, xo, PerfmonDefinition.PERFMON_KEY_NAME, perfmonConfigXML);
            }

            var hosts = new List<Host>();

            if (theHost == null)
            {
                VM vm = xo as VM;
                if (vm == null)
                {
                    SR sr = xo as SR;
                    if (sr != null)
                    {
                        foreach (var pbdRef in sr.PBDs)
                        {
                            PBD pbd = sr.Connection.Resolve(pbdRef);
                            if (pbd == null)
                                continue;

                            var host = pbd.Connection.Resolve(pbd.host);
                            if (host != null)
                                hosts.Add(host);
                        }
                    }
                }
                else
                {
                    var host = vm.Home();
                    if (host != null)
                        hosts.Add(host);
                }
            }
            else
            {
                hosts.Add(theHost);
            }

            foreach (var host in hosts)
            {
                try
                {
                    //NB The refresh causes the server to re-read the configuration
                    //immediately. But even if the refresh fails, the change will be
                    //noticed, just a bit later.

                    new ExecutePluginAction(host.Connection, host,
                        XenServerPlugins.PLUGIN_PERFMON_PLUGIN,
                        XenServerPlugins.PLUGIN_PERFMON_FUNCTION_REFRESH,
                        new Dictionary<string, string>(), true).RunExternal(Session);
                }
                catch (Exception e)
                {
                    // Handle perfmon randomly being stopped
                    if (e.Message.StartsWith(XenServerPlugins.PLUGIN_PERFMON_ERROR_NOT_RUNNING))
                    {
                        // start perfmon and try again
                        try
                        {
                            new ExecutePluginAction(host.Connection, host,
                                XenServerPlugins.PLUGIN_PERFMON_PLUGIN,
                                XenServerPlugins.PLUGIN_PERFMON_FUNCTION_START,
                                new Dictionary<string, string>(), true).RunExternal(Session);

                            new ExecutePluginAction(host.Connection, host,
                                XenServerPlugins.PLUGIN_PERFMON_PLUGIN,
                                XenServerPlugins.PLUGIN_PERFMON_FUNCTION_REFRESH,
                                new Dictionary<string, string>(), true).RunExternal(Session);
                        }
                        catch (Exception ex)
                        {
                            log.Debug("Perfmon refresh failed. Alerts will start being produced within half an hour.", ex);
                        }
                    }
                    else
                    {
                        log.Debug("Perfmon refresh failed. Alerts will start being produced within half an hour.", e);
                    }
                }
            }
        }
    }
}
