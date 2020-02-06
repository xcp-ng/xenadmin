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


namespace XenAdmin.Core
{
    public class I18NException : Exception
    {
        public readonly I18NExceptionType Type;
        public readonly string[] Parameters;

        public I18NException(I18NExceptionType type, params string[] parameters)
        {
            this.Type = type;
            this.Parameters = parameters;
        }

        // ensure that we log paramters and our type
        public override string ToString()
        {
            return string.Format("I18NException of type '{0}' occurred\nException parameters: '{1}'\n{2}", Type, string.Join(", ", Parameters), base.ToString());
        }

        public string I18NMessage
        {
            get
            {
                string message = string.Format(FriendlyNameManager.GetFriendlyName(string.Format("Exception-{0}", Type)), Parameters);
                return !string.IsNullOrEmpty(message) ? message : Messages.EXCEPTION_GENERAL;
            }
        }
    }

    public enum I18NExceptionType
    {
        PluginVersionInvalid,
        PluginSearchParseFailed,
        PluginXMLNodeNotRecognised,
        PluginResourcesFileNotFound,
        PluginLabelNotDefined,
        PluginOnlyOneCommandPermitted,
        
        XmlAttributeMissing,
        XmlInvalid,

        PowerShellNotPresent,
        PowerShellExecutionPolicyRestricted,

        XenSearchFileInvalid
    }
}
