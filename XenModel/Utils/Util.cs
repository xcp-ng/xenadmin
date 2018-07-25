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
using System.Collections;
using System.Collections.Generic;
using System.Xml;


namespace XenAdmin
{
    public enum RoundingBehaviour
    {
        Up, Down, Nearest, None
    }

    /// <summary>
    /// Miscellaneous utility functions
    /// </summary>
    public static class Util
    {
        public const long BINARY_KILO = 1024;
        public const long BINARY_MEGA = BINARY_KILO * BINARY_KILO;
        public const long BINARY_GIGA = BINARY_KILO * BINARY_MEGA;
        public const long BINARY_TERA = BINARY_KILO * BINARY_GIGA;

        public const long DEC_KILO = 1000;
        public const long DEC_MEGA = DEC_KILO * DEC_KILO;
        public const long DEC_GIGA = DEC_KILO * DEC_MEGA;
        public const long DEC_TERA = DEC_KILO * DEC_GIGA;

        /// <summary>
        /// The default iSCSI filer port.
        /// </summary>
        public const UInt16 DEFAULT_ISCSI_PORT = 3260;

        public static string MemorySizeStringSuitableUnits(double bytes, bool showPoint0Decimal)
        {
            if (bytes >= 1 * BINARY_GIGA)
            {
                string format = Messages.VAL_GB_ONE_DECIMAL;
                int dp = 1;
                double valGB = bytes / BINARY_GIGA;
                if (valGB > 100)
                {
                    dp = 0;
                    format = Messages.VAL_GB;
                }
                if(!showPoint0Decimal)
                {
                    format = Messages.VAL_GB;
                }
                return string.Format(format, Math.Round(valGB, dp, MidpointRounding.AwayFromZero));       
            }
            else if (bytes >= 1 * BINARY_MEGA)
            {
                return string.Format(Messages.VAL_MB, Math.Round(bytes / BINARY_MEGA));
            }
            else if (bytes >= 1 * BINARY_KILO)
            {
                return string.Format(Messages.VAL_KB, Math.Round(bytes / BINARY_KILO));
            }
            
            if(bytes == 0)
            {
                return bytes.ToString();
            }
            else
            {
                return string.Format(Messages.VAL_B, bytes);
            }
        }

        /// <summary>
        /// Returns the string in suitable units and when the number of bytes is 0 the units are the ones given in formatStringWhenZero.
        /// E.g. : if bytes = 0, showPoint0Decimal = false, formatStringWhenZero = Messages.VAL_MB, then the function will return "0 MB".
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="showPoint0Decimal"></param>
        /// <param name="formatStringWhenZero"></param>
        /// <returns></returns>
        public static string MemorySizeStringSuitableUnits(double bytes, bool showPoint0Decimal, string formatStringWhenZero)
        {
            if(bytes == 0)
            {
                return string.Format(formatStringWhenZero, bytes);
            }
            else
            {
                return MemorySizeStringSuitableUnits(bytes, showPoint0Decimal);
            }
        }

        public static string DataRateString(double bytesPerSec)
        {
            string unit;
            string value = ByteSizeString(bytesPerSec, 1, true, out unit);
            return string.Format(Messages.VAL_FORMAT, value, unit);
        }

        public static string DataRateValue(double bytesPerSec, out string unit)
        {
            return ByteSizeString(bytesPerSec, 1, true, out unit);
        }

        public static string DiskSizeString(ulong bytes)
        {
            string unit;
            string value = ByteSizeString(bytes, 1, false, out unit);
            return string.Format(Messages.VAL_FORMAT, value, unit);
        }       
	
        public static string DiskSizeString(long bytes, string format = null)
        {
            return DiskSizeString(bytes, 1, format);
        }

        public static string DiskSizeString(long bytes, int dp, string format = null)
		{
			ulong abs = (ulong)Math.Abs(bytes);
            string unit;
            string value = ByteSizeString(abs, dp, false, out unit, format);
            return string.Format(Messages.VAL_FORMAT, value, unit);
		}

    	public static string DiskSizeStringWithoutUnits(long bytes)
    	{
    	    string unit;
            return ByteSizeString(bytes, 1, false, out unit);
        }

        public static string MemorySizeStringVariousUnits(double bytes)
        {
            string unit;
            string value = ByteSizeString(bytes, 0, false, out unit);
            return string.Format(Messages.VAL_FORMAT, value, unit);
        }

        public static string MemorySizeValueVariousUnits(double bytes, out string unit)
        {
            return ByteSizeString(bytes, 0, false, out unit);
        }

        private static string ByteSizeString(double bytes, int decPlaces, bool isRate, out string unit, string format = null)
        {
            if (bytes >= BINARY_GIGA)
            {
                unit = isRate ? Messages.VAL_GIGRATE : Messages.VAL_GIGB;
                var result = Math.Round(bytes / BINARY_GIGA, decPlaces);
                return string.IsNullOrEmpty(format) ? result.ToString() : result.ToString(format);
            }

            if (bytes >= BINARY_MEGA)
            {
                unit = isRate ? Messages.VAL_MEGRATE : Messages.VAL_MEGB;
                var result = Math.Round(bytes / BINARY_MEGA, decPlaces);
                return string.IsNullOrEmpty(format) ? result.ToString() : result.ToString(format);
            }

            if (bytes >= BINARY_KILO)
            {
                unit = isRate ? Messages.VAL_KILRATE : Messages.VAL_KILB;
                var result = Math.Round(bytes / BINARY_KILO, decPlaces);
                return string.IsNullOrEmpty(format) ? result.ToString() : result.ToString(format);
            }

            unit = isRate ? Messages.VAL_RATE : Messages.VAL_BYTE;
            return bytes.ToString();
        }

        /// <summary>
        /// nothing actually is measured in nanoseconds, we actually output microseconds
        /// </summary>
        public static string NanoSecondsString(double t)
        {
            string unit;
            string value = NanoSecondsValue(t, out unit);
            return string.Format(Messages.VAL_FORMAT_SECONDS, value, unit);
        }

        public static string NanoSecondsValue(double t, out string unit)
        {
            if (t >= DEC_GIGA)
            {
                unit = Messages.VAL_SEC;
                return (t / DEC_GIGA).ToString("0");
            }

            if (t >= DEC_MEGA)
            {
                unit = Messages.VAL_MILSEC;
                return (t / DEC_MEGA).ToString("0");
            }

            if (t >= DEC_KILO)
            {
                unit = Messages.VAL_MICSEC;
                return (t / DEC_KILO).ToString("0");
            }

            unit = Messages.VAL_NANOSEC;
            return t.ToString("0");
        }

        public static string MilliWattString(double t)
        {
            string unit;
            string value = MilliWattValue(t, out unit);
            return string.Format(Messages.VAL_FORMAT, value, unit);
        }

        public static string MilliWattValue(double t, out string unit)
        {
            if (t >= DEC_GIGA)
            {
                unit = Messages.VAL_MWATT;
                return (t / DEC_GIGA).ToString("0");
            }

            if (t >= DEC_MEGA)
            {
                unit = Messages.VAL_KILOWATT;
                return (t / DEC_MEGA).ToString("0");
            }

            if (t >= DEC_KILO)
            {
                unit = Messages.VAL_WATT;
                return (t / DEC_KILO).ToString("0");
            }

            unit = Messages.VAL_MILWATT;
            return t.ToString("0");
        }

        public static double ToGB(double bytes, int dp, RoundingBehaviour rounding)
        {
            double value = (double)bytes / BINARY_GIGA;
            int decimalsAdjustment = (int)Math.Pow(10, dp);
            switch (rounding)
            {
                case RoundingBehaviour.None:
                    return value;
                case RoundingBehaviour.Down:
                    return (Math.Floor(value * decimalsAdjustment) / decimalsAdjustment);                     
                case RoundingBehaviour.Up:
                   return (Math.Ceiling(value * decimalsAdjustment) / decimalsAdjustment);
                default:  // case RoundingBehaviour.Nearest:
                    return (Math.Round(value, 1, MidpointRounding.AwayFromZero));
            }          
        }

        public static double ToMB(double bytes, RoundingBehaviour rounding)
        {
            switch (rounding)
            {
                case RoundingBehaviour.None:
                    return bytes / BINARY_MEGA;
                case RoundingBehaviour.Down:
                    return Math.Floor(bytes / BINARY_MEGA);
                case RoundingBehaviour.Up:
                    return Math.Ceiling(bytes / BINARY_MEGA);
                default:  // case RoundingBehaviour.Nearest:
                    return Math.Round(bytes / BINARY_MEGA, MidpointRounding.AwayFromZero);
            }
        }

        public static double CorrectRoundingErrors(double amount)
        {
            // Special case to cope with choosing an amount that's a multiple of 0.1G but not 0.5G --
            // sending it to the server as the nearest byte and getting it back later --
            // and finding it's fractionally changed, messing up our spinner permitted ranges.
            double amountRounded = ToGB(amount, 1, RoundingBehaviour.Nearest) * BINARY_GIGA;
            double roundingDiff = amountRounded - amount;
            if (roundingDiff > -1.0 && roundingDiff < 1.0)  // within 1 byte: although I think it will always be positive in the case we want to correct
                return amountRounded;
            else
                return amount;
        }

        public static double ToUnixTime(DateTime time)
        {
            TimeSpan diff = time - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return diff.TotalSeconds;
        }

        public static DateTime FromUnixTime(double time)
        {
            DateTime bootTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return bootTime.AddSeconds(time);
        }

        public static string TimeString(long t)
        {
            if (t >= 120)
                return string.Format(Messages.TIME_MINUTES, t / 60);
            
            if (t > 0)
                return string.Format(Messages.TIME_SECONDS, t);
            
            return Messages.TIME_NEGLIGIBLE;
        }

        public static string TimeRangeString(long t1, long t2)
        {
            return t1 > 60 && t2 > 60 ?
                string.Format(Messages.TIME_RANGE_MINUTES, t1 / 60, t2 / 60) :
                string.Format(Messages.TIME_RANGE_SECONDS, t1, t2);
        }
       
        internal static string LThanTime(long max)
        {
            return string.Format(Messages.LESS_THAN, TimeString(max));
        }

        internal static string GThanSize(long min)
        {
            return string.Format(Messages.GREATER_THAN, DiskSizeString(min));
        }

        internal static string GThanTime(long min)
        {
            return string.Format(Messages.GREATER_THAN, TimeString(min));
        }

        internal static string LThanSize(long max)
        {
            return string.Format(Messages.LESS_THAN, DiskSizeString(max));
        }

        public static string CountsPerSecondString(double p)
        {
            return string.Format(Messages.VAL_FORMAT, p, Messages.COUNTS_PER_SEC_UNIT);
        }

        public static string MegaHertzString(double t)
        {
            string unit;
            string value = MegaHertzValue(t, out unit);
            return string.Format(Messages.VAL_FORMAT, value, unit);
        }

        public static string MegaHertzValue(double t, out string unit)
        {
            return MegaHertzValue(t, 4, out unit);
        }

        /// <summary>
        /// Converts the input value from MHz to GHz if needed, rounding it to the specified decimal places
        /// </summary>
        private static string MegaHertzValue(double t, int decPlaces, out string unit)
        {
            if (t >= DEC_KILO)
            {
                unit = Messages.VAL_GIGHZ;
                return Math.Round(t / DEC_KILO, decPlaces).ToString();
            }
            
            unit = Messages.VAL_MEGHZ;
            return Math.Round(t, decPlaces).ToString();
        }

        public static string PercentageString(double fraction)
        {
            return string.Format("{0}%", (fraction * 100d).ToString("0.0"));
        }

        public static void ThrowIfParameterNull(object obj, string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (name.Length == 0)
            {
                ThrowBecauseZeroLength("name");
            }

            if (obj == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        public static void ThrowIfStringParameterNullOrEmpty(string value, string name)
        {
            ThrowIfParameterNull(value, name);

            if (value.Length == 0)
            {
                ThrowBecauseZeroLength(name);
            }
        }

        public static void ThrowIfEnumerableParameterNullOrEmpty(IEnumerable value, string name)
        {
            ThrowIfParameterNull(value, name);

#pragma warning disable 0168
            foreach (object _ in value)
            {
                return;
            }
#pragma warning restore 0168

            ThrowBecauseZeroLength(name);
        }

        private static void ThrowBecauseZeroLength(string name)
        {
            throw new ArgumentException(string.Format("{0} cannot have 0 length.", name), name);
        }

        /// <summary>
        /// Loads the specified non-generic IEnumerable into a generic List&lt;T&gt;.
        /// </summary>
        /// <typeparam name="T">The type to convert each element to</typeparam>
        /// <param name="input">The input non-generic IEnumerable.</param>
        /// <returns>Generic List&lt;T&gt;</returns>
        public static List<T> PopulateList<T>(IEnumerable input)
        {
            ThrowIfParameterNull(input, "input");
            List<T> output = new List<T>();
            foreach (T t in input)
            {
                output.Add(t);
            }
            return output;
        }

        /// <summary>
        /// Gets a List that represents the specified IEnumerable. If the input isn't a List then one gets created by passing the input into
        /// List's constructor. If the input is already a List then it is returned directly.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">The input.</param>
        /// <returns>A list for the specified enumerable</returns>
        public static List<T> GetList<T>(IEnumerable<T> input)
        {
            if (input == null)
            {
                return null;
            }

            var list = input as List<T>;

            if (list != null)
            {
                return list;
            }
            return new List<T>(input);
        }

        /// <summary>
        /// Matches 1-65535 inclusive.
        /// </summary>
        /// <param name="s">The string to be parsed</param>
        /// <returns>True if the specified string contains only a valid port, otherwise false.</returns>
        public static bool IsValidPort(string s)
        {
            int port;

            if (!int.TryParse(s, out port))
                return false;

            return 0 < port && port <= 65535;
        }

        public static string GetXmlNodeInnerText(XmlNode node, string xPath)
        {
            ThrowIfParameterNull(node, "node");
            ThrowIfStringParameterNullOrEmpty(xPath, "xPath");

            XmlNodeList nodes = node.SelectNodes(xPath);

            if (nodes == null || nodes.Count == 0)
            {
                throw new InvalidOperationException("Node not found: " + xPath);
            }

            return nodes[0].InnerText;
        }

        /// <summary>
        /// Get the first node with name 'value' and returns its innerText. Used for gettings results of CGSL async actions.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>The contents of the first node with name 'value'.</returns>
        public static string GetContentsOfValueNode(string xml)
        {
            ThrowIfStringParameterNullOrEmpty(xml, "xml");

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            // If we've got this from an async task result, then it will be wrapped
            // in a <value> element.
            foreach (XmlNode node in doc.GetElementsByTagName("value"))
            {
                return node.InnerText;
            }

            return null;
        }

    }
}
