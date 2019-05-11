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
using System.Xml;
using System.Collections;
using System.Text;
using System.Security.Cryptography;
using YYProject.XXHash;

/* Thrown if we fail to verify a block (ie sha1) checksum */
public class BlockChecksumFailed : ApplicationException
{
    private string block;
    private string recomputed;
    private string original;

    public BlockChecksumFailed(string block, string recomputed, string original)
    {
        this.block = block;
        this.recomputed = recomputed;
        this.original = original;
    }

    public override string ToString()
    {
        return "Failed to verify the block checksum: block = " + block + "; recomputed = " + recomputed + "; original = " + original;
    }
}

public class Export
{
    public static bool verbose_debugging = false;
    public static void debug(string x)
    {
        if (verbose_debugging)
            Console.WriteLine(x);
    }

    private readonly SHA1 sha = new SHA1CryptoServiceProvider();
    private XXHash64 xxhash = new XXHash64();

    private string checksum_sha1(byte[] data)
    {
        byte[] result = sha.ComputeHash(data);
        return hex(result).ToLower();
    }

    private string hex(byte[] bytes)
    {
        char[] chars = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};
        char[] output = new char[bytes.Length * 2];

        for(uint i = 0; i < bytes.Length; i++)
        {
            uint b = (uint) bytes[i];
            output[i * 2] = chars[b >> 4];
            output[i * 2 + 1] = chars[b & 0x0F];
        }

        return new string(output);
    }

    private string checksum_xxhash(byte[] data)
    {
        xxhash.Initialize();
        return hex(xxhash.ComputeHash(data));
    }

    private static Hashtable parse_checksum_table(string checksum_xml)
    {
        Hashtable table = new Hashtable();

        XmlDocument xmlDoc = new System.Xml.XmlDocument();
        xmlDoc.LoadXml(checksum_xml);
        XmlNodeList members = xmlDoc.GetElementsByTagName("member");
        string name;
        string value;
        foreach (XmlNode member in members)
        {
            name = ""; value = "";
            foreach (XmlNode child in member.ChildNodes)
            {
                XmlNode v = child.FirstChild;
                if (child.Name.Equals("name"))
                    name = v.Value;
                if (child.Name.Equals("value"))
                    value = v.Value;
            }
            debug(String.Format("adding {0} = {1}", name, value));
            table.Add(name, value);
        }
        return table;
    }

    private static void compare_tables(Hashtable recomputed, Hashtable original)
    {
        foreach (DictionaryEntry x in recomputed)
        {
            string ours = (string)x.Value;
            string theirs = (string)original[x.Key];
            if (!ours.Equals(theirs))
            {
                throw new BlockChecksumFailed((string)x.Key, ours, theirs);
            }
            else
            {
                debug(String.Format("{0} hash OK", (string)x.Key));
            }
        }
    }

    private static string string_of_byte_array(byte[] payload)
    {
        Decoder decoder = Encoding.UTF8.GetDecoder();
        char[] chars = new char[decoder.GetCharCount(payload, 0, (int)payload.Length)];
        decoder.GetChars(payload, 0, (int)payload.Length, chars, 0);
        return new string(chars);
    }

    public delegate void verifyCallback(uint read);
    public delegate bool cancellingCallback();

    /* 'input' is the source of the export data, if 'output' is not null then
       a perfect copy should be echoed there. */
    public void verify(Stream input, Stream output, cancellingCallback cancelling)
    {
        verify(input, output, cancelling, null);
    }

    private Header nextHeader(Stream input, Stream output, verifyCallback callback)
    { // Interperate the next bytes from the stream as a Tar header
        byte[] bytes = nextData(input, output, callback, Header.length);

        if (Header.all_zeroes(bytes)) // Tar headers are 512-byte blocks in size
        {
            bytes = nextData(input, output, callback, Header.length);

            if (Header.all_zeroes(bytes))
            {
                // Tars end with an End-Of-Archive marker, which is two consecutive 512-byte blocks of zero bytes
                throw new EndOfArchive();
            }
        }

        return new Header(bytes);
    }

    private byte[] nextData(Stream input, Stream output, verifyCallback callback, uint size)
    { // Returns the next given number of bytes from the input
        byte[] bytes = IO.unmarshal_n(input, size);
        callback?.Invoke(size);
        if (output != null) output.Write(bytes, 0, bytes.Length);
        return bytes;
    }

    public void verify(Stream input, Stream output, cancellingCallback cancelling, verifyCallback callback)
    {
        Hashtable recomputed_checksums = new Hashtable();
        Hashtable original_checksums = null;

        try
        {
            while (!cancelling())
            {
                Header header_data = nextHeader(input, output, callback);
                debug(header_data.ToString());

                byte[] bytes_data = nextData(input, output, callback, header_data.file_size);

                if (header_data.file_name.Equals("ova.xml"))
                {
                    debug("Skipping ova.xml");
                }
                else if (header_data.file_name.EndsWith("checksum.xml"))
                {
                    string xml = string_of_byte_array(bytes_data);
                    original_checksums = parse_checksum_table(xml);
                }
                else
                { // The file has no extension (must be a data file) so will have a .checksum or .xxhash file right after it
                    Header header_checksum = nextHeader(input, output, callback);
                    byte[] bytes_checksum = nextData(input, output, callback, header_checksum.file_size);
                    string csum_compare = string_of_byte_array(bytes_checksum);

                    string csum = header_checksum.file_name.EndsWith(".xxhash") ? checksum_xxhash(bytes_data) : checksum_sha1(bytes_data);

                    if (!csum.Equals(csum_compare))
                        throw new BlockChecksumFailed(header_data.file_name, csum, csum_compare);

                    debug(String.Format(" has checksum: {0}", csum));
                    recomputed_checksums.Add(header_data.file_name, csum);

                    nextData(input, output, callback, header_checksum.paddingLength()); // Eat the padding for the checksum file
                }
                nextData(input, output, callback, header_data.paddingLength()); // Eat the padding for the data file
            }
        }
        catch (EndOfArchive)
        {
            debug("EOF");
            if (original_checksums != null)
                compare_tables(recomputed_checksums, original_checksums);
        }
    }
}
