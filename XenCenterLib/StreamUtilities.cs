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
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace XenCenterLib
{
    public static class StreamUtilities
    {
        /// <summary>
        /// Perform a copy of the contents of one stream class to another in a buffered fashion
        /// 
        /// Buffer size is a hard-coded 2Mb
        /// </summary>
        /// <param name="inputData">Source data</param>
        /// <param name="outputData">Target stream</param>
        public static void BufferedStreamCopy(Stream inputData, Stream outputData)
        {
            if( inputData == null)
                throw new ArgumentNullException("inputData", "BufferedStreamCopy argument cannot be null");

            if (outputData == null)
                throw new ArgumentNullException("outputData", "BufferedStreamCopy argument cannot be null");

            const long bufferSize = 2*1024*1024;

            byte[] buffer = new byte[bufferSize];
            int n;
            while ((n = inputData.Read(buffer, 0, buffer.Length)) > 0)
            {
                outputData.Write(buffer, 0, n);
            }

            outputData.Flush();
        }

        public static bool VerifyAgainstDigest(Stream stream, long limit, string algorithmName, byte[] digest, RSACryptoServiceProvider cryptoServiceProvider = null)
        {
            int bytesRead = 0;
            long offset = 0;
            byte[] buffer = new byte[2 * 1024 * 1024];

            using (var hashAlgorithm = HashAlgorithm.Create(algorithmName))
            {
                // Validate the algorithm.
                if (hashAlgorithm == null)
                    throw new NotSupportedException($"{algorithmName} is not a valid hash algorithm");

                while (offset < limit)
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);

                    if (bytesRead <= 0)
                        break;

                    if (buffer.Any(b => b != 0x0))
                    {
                        if (offset + bytesRead < limit)
                        {
                            // This is not the last block. Compute the partial hash.
                            hashAlgorithm.TransformBlock(buffer, 0, bytesRead, buffer, 0);
                        }
                    }

                    offset += bytesRead;
                }

                // It is necessary to call TransformBlock at least once and TransformFinalBlock only once before getting the hash.
                // If only the last buffer had content, then TransformBlock would not have been called at least once.
                // So, split the last buffer and hash it even if it is empty.
                // Note: TransformBlock will accept an "inputCount" that is zero.
                hashAlgorithm.TransformBlock(buffer, 0, bytesRead / 2, buffer, 0);

                // Compute the final hash.
                hashAlgorithm.TransformFinalBlock(buffer, bytesRead / 2, bytesRead / 2 + bytesRead % 2);

                return cryptoServiceProvider == null
                    ? digest.SequenceEqual(hashAlgorithm.Hash)
                    : cryptoServiceProvider.VerifyHash(hashAlgorithm.Hash, CryptoConfig.MapNameToOID(algorithmName), digest);
            }
        }
    }
}
