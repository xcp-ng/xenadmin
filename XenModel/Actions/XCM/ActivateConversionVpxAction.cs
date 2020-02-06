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
using System.Linq;
using System.Threading;
using XenAPI;


namespace XenAdmin.Actions.Xcm
{
    public class ActivateConversionVpxAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int TIMEOUT = 2 * 60 * 1000; //milliseconds
        private const int SLEEP = 2000; //milliseconds

        public ActivateConversionVpxAction(VM conversionVm)
            : base(conversionVm?.Connection, "", true)
        {
            ConversionVm = conversionVm;
        }

        public VM ConversionVm { get; private set; }
        public string ServiceIp { get; private set; }

        protected override void Run()
        {
            if (ConversionVm == null)
                throw new Exception(Messages.CONVERSION_CANNOT_FIND_VPX);

            switch (ConversionVm.power_state)
            {
                case vm_power_state.Halted:
                case vm_power_state.Paused:
                case vm_power_state.Suspended:
                case vm_power_state.Running:
                    break;
                default:
                    log.Error($"The conversion VPX {ConversionVm.uuid} is in an unknown power state");
                    throw new Exception(Messages.CONVERSION_VPX_UNKNOWN_POWER_STATE);
            }

            try
            {
                switch (ConversionVm.power_state)
                {
                    case vm_power_state.Halted:
                        Description = Messages.CONVERSION_VPX_START;
                        VM.start(Connection.Session, ConversionVm.opaque_ref, false, false);
                        break;
                    case vm_power_state.Paused:
                        Description = Messages.CONVERSION_VPX_UNPAUSE;
                        VM.unpause(Connection.Session, ConversionVm.opaque_ref);
                        break;
                    case vm_power_state.Suspended:
                        Description = Messages.CONVERSION_VPX_RESUME;
                        VM.resume(Connection.Session, ConversionVm.opaque_ref, false, false);
                        break;
                }
            }
            catch (Exception e)
            {
                if (e is Failure f && f.ErrorDescription.Count > 0 && f.ErrorDescription[0] == Failure.VM_BAD_POWER_STATE)
                {
                    //ignore
                }
                else
                {
                    log.Error($"Failed to activate conversion VPX {ConversionVm.uuid}", e);
                    throw new Exception(Messages.CONVERSION_INITIALIZING_VPX_FAILURE);
                }
            }

            Description = Messages.CONVERSION_VPX_OBTAIN_IP;

            string ipAddress = null;
            var tries = TIMEOUT / SLEEP;

            while (tries > 0)
            {
                if (Cancelling)
                    throw new CancelledException();

                if (Helper.IsNullOrEmptyOpaqueRef(ConversionVm.guest_metrics.opaque_ref))
                {
                    ConversionVm = Connection.Resolve(new XenRef<VM>(ConversionVm.opaque_ref));
                }
                else
                {
                    var metrics = Connection.Resolve(ConversionVm.guest_metrics);
                    if (metrics != null)
                    {
                        // device 0 is the internal network for the VM; find an external one
                        var vif = Connection.ResolveAll(ConversionVm.VIFs).FirstOrDefault(v =>
                            v.device != "0" && metrics.networks.TryGetValue($"{v.device}/ip", out ipAddress));

                        if (vif != null)
                            break;
                    }
                }

                Thread.Sleep(SLEEP);
                tries--;
            }

            if (string.IsNullOrEmpty(ipAddress))
            {
                log.Error($"Cannot obtain an IP address for conversion VPX {ConversionVm.uuid}.");
                throw new Exception(Messages.CONVERSION_CANNOT_OBTAIN_VPX_IP);
            }

            ServiceIp = ipAddress;
        }
    }
}
