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

namespace XenAdmin
{
    public static class StringExtensions
    {

        /// <summary>
        /// Ellipsise string by appending the ellipsis - no truncation is performed
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string AddEllipsis(this string input)
        {
            return String.Concat(input, Messages.ELLIPSIS);
        }

        /// <summary>
        /// If input is longer than maxLength, returns a string of length maxLength consisting of a truncated
        /// version of the input string with an i18n'd ellipsis character appended. Otherwise returns input.
        /// </summary>
        /// <param name="input">May be null, in which case the empty string is returned.</param>
        /// <param name="maxLength">Must be >= Messages.ELLIPSIS.Length.</param>
        /// <returns></returns>
        public static string Ellipsise(this string input, int maxLength)
        {
            if (maxLength < Messages.ELLIPSIS.Length)
                return ".";
            if (input == null)
                return "";
            if (input.Length < maxLength)
                return input;
            else
                return input.Truncate(maxLength - Messages.ELLIPSIS.Length) + Messages.ELLIPSIS;
        }

        /// <summary>
        /// Truncate string to a fixed number of characters, but not cutting it in the middle of a UTF-16 surrogate pair:
        /// shorten it by one if necessary. The "number of characters" is actually "number of UTF-16 double bytes", not
        /// the number of true Unicode characters. In other words, this a safe replacement for s.Substring(0, len).
        /// </summary>
        /// <reference>CA-89876</reference>
        /// <param name="s">The string to be truncated</param>
        /// <param name="len">The length of the resultant string</param>
        /// <returns>A truncated copy of the string</returns>
        public static string Truncate(this string s, int len)
        {
            if (len > 0 && Char.IsSurrogatePair(s, len - 1))
                return s.Substring(0, len - 1);
            else
                return s.Substring(0, len);
        }

        /// <summary>
        /// Escapes ampersands by doubling them up. Will return null if passed null.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string EscapeAmpersands(this string s)
        {
            if (s == null)
                return null;
            else
                return s.Replace("&", "&&");
        }

        public static string EscapeQuotes(this string s)
        {
            return s == null ? null : s.Replace("\"", "\"\"");
        }
    }
}
