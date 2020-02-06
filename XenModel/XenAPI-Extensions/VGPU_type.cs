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
using XenAdmin;

namespace XenAPI
{
    partial class VGPU_type : IComparable<VGPU_type>, IEquatable<VGPU_type>
    {
        public override string Name()
        {
            if (IsPassthrough())
                return Messages.VGPU_PASSTHRU_TOSTRING;

            return string.Format(Messages.VGPU_TOSTRING, model_name, Capacity());
        }

        public override string Description()
        {
            if (IsPassthrough())
                return Messages.VGPU_PASSTHRU_TOSTRING;

            bool multipleVgpuSupport = compatible_types_in_vm.Count > 0;

            var maxRes = MaxResolution();
            if ((maxRes == "0x0" || string.IsNullOrEmpty(maxRes)) && max_heads < 1)
                return multipleVgpuSupport ? string.Format(Messages.VGPU_DESCRIPTION_ZEROES_MULTIPLE_VGPU_SUPPORT, model_name, Capacity()) : 
                    string.Format(Messages.VGPU_DESCRIPTION_ZEROES, model_name, Capacity());

            return string.Format(max_heads == 1 ? 
                (multipleVgpuSupport ? Messages.VGPU_DESCRIPTION_ONE_MULTIPLE_VGPU_SUPPORT : Messages.VGPU_DESCRIPTION_ONE) : 
                (multipleVgpuSupport ? Messages.VGPU_DESCRIPTION_MANY_MULTIPLE_VGPU_SUPPORT : Messages.VGPU_DESCRIPTION_MANY),
                model_name, Capacity(), maxRes, max_heads);
        }

        public string MaxResolution()
        {
            return max_resolution_x + "x" + max_resolution_y;
        }

        #region IEquatable<VGPU_type> Members

        /// <summary>
        /// Indicates whether the current object is equal to the specified object.
        /// This calls the implementation from XenObject.
        /// This implementation is required for ToStringWrapper.
        /// </summary>
        public bool Equals(VGPU_type other)
        {
            return base.Equals(other);
        }

        #endregion

        // CA-166091: Default sort order for VGPU types is:
        // * Pass-through is "biggest"
        // * Then others in capacity order, lowest capacity is biggest
        // * In case of ties, highest resolution is biggest
        public override int CompareTo(VGPU_type other)
        {
            bool thisPassthrough = this.IsPassthrough();
            bool otherPassthrough = other.IsPassthrough();
            if (thisPassthrough != otherPassthrough)
                return thisPassthrough ? 1 : -1;

            long thisCapacity = this.Capacity();
            long otherCapacity = other.Capacity();
            if (thisCapacity != otherCapacity)
                return (int)(otherCapacity - thisCapacity);

            long thisResolution = this.max_resolution_x * this.max_resolution_y;
            long otherResolution = other.max_resolution_x * other.max_resolution_y;
            if (thisResolution != otherResolution)
                return (int)(thisResolution - otherResolution);

            return base.CompareTo(other);
        }

        /// <summary>
        /// vGPUs per GPU
        /// </summary>
        public long Capacity()
        {
            long capacity = 0;
            if (supported_on_PGPUs != null && supported_on_PGPUs.Count > 0)
            {
                PGPU pgpu = Connection.Resolve(supported_on_PGPUs[0]);
                if (pgpu != null && pgpu.supported_VGPU_max_capacities != null && pgpu.supported_VGPU_max_capacities.Count > 0)
                {
                    var vgpuTypeRef = new XenRef<VGPU_type>(this);
                    if (pgpu.supported_VGPU_max_capacities.ContainsKey(vgpuTypeRef))
                        capacity = pgpu.supported_VGPU_max_capacities[vgpuTypeRef];
                }
            }
            return capacity;
        }

        public bool IsPassthrough()
        {
            return model_name == "passthrough";
        }
    }
}
