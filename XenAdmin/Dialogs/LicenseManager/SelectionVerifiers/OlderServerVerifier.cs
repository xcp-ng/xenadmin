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
using System.Linq;
using XenAdmin.Core;

namespace XenAdmin.Dialogs.LicenseManagerSelectionVerifiers
{
    public class OlderServerVerifier : LicenseSelectionVerifier
    {
        public OlderServerVerifier(List<LicenseDataGridViewRow> rowsToVerify) :base(rowsToVerify){}

        public override string VerificationDetails()
        {
            return Status == VerificationStatus.Error && RowsToVerify.Count > 1
                ? string.Format(Messages.LICENSE_NO_MULTISELECT_LICENSE, BrandManager.ProductVersion56)
                : string.Empty;
        }


        public override void Verify()
        {
            if (RowsToVerify.Count < 1)
            {
                Status = VerificationStatus.Error;
                return;
            }

            bool allCanUseServer = RowsToVerify.TrueForAll(r => r.CanUseLicenseServer);
            int cannotUse = RowsToVerify.Where(r => !r.CanUseLicenseServer).ToList().Count;
            bool oldSelectionValid = cannotUse < 2 && cannotUse > 0 && cannotUse == RowsToVerify.Count;

            if (allCanUseServer || oldSelectionValid)
            {
                Status = VerificationStatus.OK;
                return;
            }
            
            Status = VerificationStatus.Error;
        }
    }
}
