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

﻿using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace XenAdmin.Core
{
	internal static class ExtensionMethods
	{
		/// <summary>
		/// Internationalization of True/False
		/// </summary>
		public static string ToStringI18n(this bool value)
		{
			return value ? Messages.TRUE : Messages.FALSE;
		}

		/// <summary>
		/// Turns a bool to internationalized Yes/No (on occasion it's user friendlier than True/False)
		/// </summary>
		public static string ToYesNoStringI18n(this bool value)
		{
			return value ? Messages.YES : Messages.NO;
		}

        /// <summary>
        /// This has the same bahvoiur as the standard ellipsise extension but this uses graphics
        /// objects to scale the text. This performs a binary chop on the string to get the correct length
        /// </summary>
        /// <param name="text"></param>
        /// <param name="rectangle"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public static string Ellipsise(this string text, Rectangle rectangle, Font font)
        {
            int width = TextRenderer.MeasureText(text, font).Width;
            if (width <= rectangle.Width)
                return text;

            int widthel = TextRenderer.MeasureText(Messages.ELLIPSIS, font).Width;
            if (widthel > rectangle.Width)
                return ".";

            // Binary chop to set the string to the right size
            int a = 0;
            int b = text.Length;
            int c;
            for (c = (a + b) / 2; c > a; c = (a + b) / 2)
            {
                string sr = text.Ellipsise(c);
                int srWidth = TextRenderer.MeasureText(sr, font).Width;
                if (srWidth > rectangle.Width)
                    b = c;
                else
                    a = c;
            }
            return text.Ellipsise(c);
        }

        public static StringBuilder AppendIndented(this StringBuilder builder, string value, int indent = 2)
        {
            var indentString = "";
            var i = 0;
            while (i++ < indent)
                indentString += " ";
            return builder.Append(string.Format("{0}{1}", indentString, value));
        }

        public static StringBuilder AppendIndented(this StringBuilder builder, StringBuilder value, int indent = 2)
        {
            var indentString = "";
            var i = 0;
            while (i++ < indent)
                indentString += " ";
            var newvalue = value.Replace(System.Environment.NewLine, string.Format("{0}{1}", System.Environment.NewLine, indentString));
            return builder.Append(string.Format("{0}{1}", indentString, newvalue));
        }
	}
}
