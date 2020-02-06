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
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Tar;

namespace XenCenterLib.Archive
{
    public static class TarSanitization
    {
        /// <remarks>
        /// See https://docs.microsoft.com/en-us/windows/desktop/fileio/naming-a-file
        /// </remarks>
        private static readonly string[] ReservedNames = { "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };

        private static readonly char[] ForbiddenChars = { '<', '>', ':', '"', '/', '\\', '|', '?', '*' };

        /// <summary>
        /// SharpZipLib doesn't account for illegal file names or characters in
        /// file names (e.g. ':' in Windows), so first we stream the tar to a
        /// new tar, sanitizing any of the contained bad file names as we go.
        /// We also need to record the modification times of all the files, so
        /// that we can restore them into the final zip.
        /// </summary>
        public static void SanitizeTarForWindows(string inputTar, string outputTar, Action cancellingDelegate)
        {
            using (var fsIn = File.OpenRead(inputTar))
            using (var inputStream = new TarInputStream(fsIn))
            using (var fsOut = File.OpenWrite(outputTar))
            using (var outputStream = new TarOutputStream(fsOut))
            {
                TarEntry entry;
                byte[] buf = new byte[8 * 1024];
                var usedNames = new Dictionary<string, string>();

                while ((entry = inputStream.GetNextEntry()) != null)
                {
                    cancellingDelegate?.Invoke();

                    entry.Name = SanitizeTarName(entry.Name, usedNames);
                    outputStream.PutNextEntry(entry);

                    long bytesToCopy = entry.Size;
                    while (bytesToCopy > 0)
                    {
                        int bytesRead = inputStream.Read(buf, 0, Math.Min(bytesToCopy > int.MaxValue ? int.MaxValue : (int)bytesToCopy, buf.Length));
                        outputStream.Write(buf, 0, bytesRead);
                        bytesToCopy -= bytesRead;
                    }

                    outputStream.CloseEntry();
                }
            }
        }

        public static string SanitizeTarPathMember(string member)
        {
            // Strip any leading/trailing whitespace, or Windows will do it for us, and we might generate non-unique names
            member = member.Trim();

            // Don't allow empty filename
            if (member.Length == 0)
                return "_";

            foreach (string reserved in ReservedNames)
            {
                if (member.ToUpperInvariant() == reserved.ToUpperInvariant()
                    || member.ToUpperInvariant().StartsWith(reserved.ToUpperInvariant() + "."))
                {
                    member = "_" + member;
                }
            }

            // Allow only 31 < c < 127, excluding any of the forbidden characters
            var sb = new StringBuilder(member.Length);
            foreach (char c in member)
            {
                if (c <= 31 || c >= 127 || ForbiddenChars.Contains(c))
                    sb.Append("_");
                else
                    sb.Append(c);
            }
            member = sb.ToString();

            // Windows also strips trailing '.' potentially generating non-unique names
            if (member.EndsWith("."))
                member = member.Substring(0, member.Length - 1) + "_";

            return member;
        }

        /// <summary>
        /// Maps file/directory names that are illegal under Windows to 'sanitized' versions. The usedNames
        /// parameter ensures this is done consistently within a directory tree.
        /// 
        /// The dictionary is used by SanitizeTarName() to ensure names are consistently sanitized. e.g.:
        /// dir1: -> dir1_
        /// dir1? -> dir1_ (1)
        /// dir1_ -> dir1_ (2)
        /// dir1:/file -> dir1_/file
        /// dir1?/file -> dir1_ (1)/file
        ///
        /// Pass the same dictionary to each invocation to get unique outputs within the same tree.
        /// </summary>
        private static string SanitizeTarName(string path, Dictionary<string, string> usedNames)
        {
            string sanitizedPath = "";
            Stack<string> bitsToEscape = new Stack<string>();
            // Trim any trailing slashes (usually indicates path is a directory)
            path = path.TrimEnd('/');
            // Take members off the end of the path until we have a name that already is
            // a key in our dictionary, or until we have the empty string.
            while (!usedNames.ContainsKey(path) && path.Length > 0)
            {
                string[] bits = path.Split('/');
                string lastBit = bits[bits.Length - 1];
                int lengthOfLastBit = lastBit.Length;
                bitsToEscape.Push(lastBit);
                path = path.Substring(0, path.Length - lengthOfLastBit);
                path = path.TrimEnd('/');
            }

            if (usedNames.ContainsKey(path))
            {
                sanitizedPath = usedNames[path];
            }

            // Now for each member in the path, look up the escaping of that member if it exists; otherwise
            // generate a new, unique escaping. Then append the escaped member to the end of the sanitized
            // path and continue.
            foreach (string member in bitsToEscape)
            {
                System.Diagnostics.Trace.Assert(member.Length > 0);
                string sanitizedMember = SanitizeTarPathMember(member);
                sanitizedPath = Path.Combine(sanitizedPath, sanitizedMember);
                path = path + Path.DirectorySeparatorChar + member;

                // Note: even if sanitizedMember == member, we must add it to the dictionary, since
                // tar permits names that differ only in case, while Windows does not. We must e.g.:
                // abc -> abc
                // aBC -> aBC (1)

                if (usedNames.ContainsKey(path))
                {
                    // We have already generated an escaping for this path prefix: use it
                    sanitizedPath = usedNames[path];
                    continue;
                }

                // Generate the unique mapping
                string pre = sanitizedPath;
                int i = 1;
                while (usedNames.Values.Any(v => v.ToUpperInvariant() == sanitizedPath.ToUpperInvariant()))
                {
                    sanitizedPath = $"{pre} ({i})";
                    i++;
                }

                usedNames.Add(path, sanitizedPath);
            }
            return sanitizedPath;
        }
    }
}
