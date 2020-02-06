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
using XenAPI;

namespace XenAdmin.Mappings
{
	public class VmMapping
	{
		public VmMapping()
		{
			Storage = new Dictionary<string, SR>();
            StorageToAttach = new Dictionary<string, VDI>();
			Networks = new Dictionary<string, XenAPI.Network>();
		}

		public string VmNameLabel { get; set; }

		/// <summary>
		/// OpaqueRef of the target pool or host
		/// </summary>
		public object XenRef { get; set; }

		/// <summary>
		/// Name of the target pool or host
		/// </summary>
		public string TargetName { get; set; }

		/// <summary>
		/// Keyed on the id in the ovf file
		/// </summary>
		public Dictionary<string, SR> Storage { get; set; }

        public Dictionary<string, VDI> StorageToAttach { get; set; }

		/// <summary>
		/// Keyed on the id in the ovf file
		/// </summary>
		public Dictionary<string, XenAPI.Network> Networks { get; set; }
	}
}
