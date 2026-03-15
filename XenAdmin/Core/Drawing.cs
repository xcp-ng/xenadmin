/* Copyright (c) Cloud Software Group, Inc. 
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
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Drawing.Text;
using System.Windows.Forms;

namespace XenAdmin.Core
{
    class Drawing
    {
        private static readonly ColorMatrix GreyScaleColorMatrix = new ColorMatrix(new float[][]
              {
                 new float[] {0.2125f, 0.2125f, 0.2125f, 0, 0},
                 new float[] {0.2577f, 0.2577f, 0.2577f, 0, 0},
                 new float[] {0.0361f, 0.0361f, 0.0361f, 0, 0},
                 new float[] {0, 0, 0, 1, 0},
                 new float[] {0.38f, 0.38f, 0.38f, 0, 1}
              });

        private static readonly ColorMatrix AlphaColorMatrix = new ColorMatrix(new float[][]
              {
                 new float[] {1, 0, 0, 0, 0},
                 new float[] {0, 1, 0, 0, 0},
                 new float[] {0, 0, 1, 0, 0},
                 new float[] {0, 0, 0, .75f, 0},
                 new float[] {0, 0, 0, 0, 1}
              });

        public static readonly ImageAttributes GreyScaleAttributes = new ImageAttributes();
        public static readonly ImageAttributes AlphaAttributes = new ImageAttributes();
        static Drawing()
        {
            GreyScaleAttributes.SetColorMatrix(GreyScaleColorMatrix);
            AlphaAttributes.SetColorMatrix(AlphaColorMatrix);
        }

        /// <summary>
        /// Kerpow!
        /// </summary>
        public static void QuickDraw(Graphics gTarget, Bitmap buffer)
        {
            gTarget.DrawImage(buffer, new Rectangle(0, 0, buffer.Width, buffer.Height));
        }

        /// <summary>
        /// Kerpow!
        /// </summary>
        public static void QuickDraw(Graphics gTarget, Bitmap buffer, Point source, Rectangle dest)
        {
            IntPtr pTarget = gTarget.GetHdc();
            IntPtr pSource = CreateCompatibleDC(pTarget);
            IntPtr pNew = buffer.GetHbitmap();
            SelectObject(pSource, pNew);
            BitBlt(pTarget, dest.X, dest.Y, dest.Width, dest.Height, pSource, source.X, source.Y, TernaryRasterOperations.SRCCOPY);
            DeleteObject(pNew);
            DeleteDC(pSource);
            gTarget.ReleaseHdc(pTarget);
        }

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth,
           int nHeight, IntPtr hObjSource, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteObject(IntPtr hObject);

        public enum TernaryRasterOperations : uint
        {
            SRCCOPY = 0x00CC0020,
            SRCPAINT = 0x00EE0086,
            SRCAND = 0x008800C6,
            SRCINVERT = 0x00660046,
            SRCERASE = 0x00440328,
            NOTSRCCOPY = 0x00330008,
            NOTSRCERASE = 0x001100A6,
            MERGECOPY = 0x00C000CA,
            MERGEPAINT = 0x00BB0226,
            PATCOPY = 0x00F00021,
            PATPAINT = 0x00FB0A09,
            PATINVERT = 0x005A0049,
            DSTINVERT = 0x00550009,
            BLACKNESS = 0x00000042,
            WHITENESS = 0x00FF0062
        }

        public static TextRenderingHint TextRenderingHint
        {
            get
            {
                return TextRenderingHint.SystemDefault;
            }
        }

        //
        // CA-23037: & character in name, custom field (anything on general panel) 
        //           renders as a keyboard shortcut
        //

        public static void DrawText(IDeviceContext dc, String text, Font font,
            Point point, Color color)
        {
            TextRenderer.DrawText(dc, text, font, point, color, TextFormatFlags.NoPrefix);
        }

        public static void DrawText(IDeviceContext dc, String text, Font font,
            Point point, Color color, Color backColor)
        {
            DrawText(dc, text, font, point, color, backColor, TextFormatFlags.NoPrefix);
        }

        public static void DrawText(IDeviceContext dc, String text, Font font,
            Point point, Color color, Color backColor, TextFormatFlags flags)
        {
            TextRenderer.DrawText(dc, text, font, point, color, backColor, flags | TextFormatFlags.NoPrefix);
        }

        public static void DrawText(IDeviceContext dc, String text, Font font,
            Rectangle bounds, Color color)
        {
            DrawText(dc, text, font, bounds, color, TextFormatFlags.NoPrefix);
        }

        public static void DrawText(IDeviceContext dc, String text, Font font, 
            Rectangle bounds, Color color, TextFormatFlags textFormatFlags)
        {
            TextRenderer.DrawText(dc, text, font, bounds, color, textFormatFlags | TextFormatFlags.NoPrefix);
        }

        public static void DrawText(IDeviceContext dc, String text, Font font,
            Rectangle bounds, Color foreColor, Color backColor)
        {
            DrawText(dc, text, font, bounds, foreColor, backColor, TextFormatFlags.NoPrefix);
        }

        public static void DrawText(IDeviceContext dc, String text, Font font,
            Rectangle bounds, Color foreColor, Color backColor, TextFormatFlags flags)
        {
            TextRenderer.DrawText(dc, text, font, bounds, foreColor, backColor, flags | TextFormatFlags.NoPrefix);
        }

        public static Size MeasureText(String text, Font font)
        {
            return MeasureText(text, font, new Size(Int32.MaxValue, Int32.MaxValue), TextFormatFlags.NoPrefix);
        }

        public static Size MeasureText(String text, Font font, Size proposedSize, TextFormatFlags flags)
        {
            return TextRenderer.MeasureText(text, font, proposedSize, flags | TextFormatFlags.NoPrefix);
        }

        public static Size MeasureText(IDeviceContext dc, String text, Font font)
        {
            return MeasureText(dc, text, font, new Size(Int32.MaxValue, Int32.MaxValue), TextFormatFlags.NoPrefix);
        }

        public static Size MeasureText(IDeviceContext dc, String text, Font font, TextFormatFlags flags)
        {
            return MeasureText(dc, text, font, new Size(Int32.MaxValue, Int32.MaxValue), flags);
        }

        public static Size MeasureText(IDeviceContext dc, String text, Font font, Size proposedSize, TextFormatFlags flags)
        {
            return TextRenderer.MeasureText(dc, text, font, proposedSize, flags | TextFormatFlags.NoPrefix);
        }

        /// <summary>
        /// Draw me a progress bar.  Copes with visual styles being on or off
        /// </summary>
        /// <param name="g">Graphics to draw to</param>
        /// <param name="r">where to draw the progress bar</param>
        /// <param name="progress">how much progress (0.0-1.0)</param>
        /// 
        public static void DrawProgressBar(Graphics g, Rectangle r, double progress)
        {
            if (Application.RenderWithVisualStyles)
            {
                ProgressBarRenderer.DrawHorizontalBar(g, r);

                Rectangle s = new Rectangle(
                    r.X + 4,
                    r.Y + 3,
                    (int)((r.Width - 8) * progress),
                    r.Height - 5);

                ProgressBarRenderer.DrawHorizontalChunks(g, s);
            }
            else
            {
                g.FillRectangle(SystemBrushes.ButtonShadow, r);

                Rectangle s = new Rectangle(
                    r.X + 1,
                    r.Y + 1,
                    r.Width - 1,
                    r.Height - 1);

                g.FillRectangle(SystemBrushes.ButtonHighlight, s);

                Rectangle t = new Rectangle(
                    r.X + 1,
                    r.Y + 1,
                    r.Width - 2,
                    r.Height - 2);

                g.FillRectangle(SystemBrushes.ButtonFace, t);

                int barwidth = (int)(progress * (r.Width - 4));
                int chunkwidth = 7;
                int chunkgap = 2;
                int progleft = 0;
                while (progleft + chunkwidth + chunkgap < barwidth)
                {
                    Rectangle u = new Rectangle(
                        r.X + 2 + progleft,
                        r.Y + 2,
                        chunkwidth,
                        13);

                    g.FillRectangle(SystemBrushes.ActiveCaption, u);
                    progleft += chunkwidth + chunkgap;
                }

                Rectangle v = new Rectangle(
                    r.X + 2 + progleft,
                    r.Y + 2,
                    chunkwidth - progleft,
                    13);

                g.FillRectangle(SystemBrushes.ActiveCaption, v);
            }
        }

        public enum SimpleProgressBarColor { Green, Red, Blue };

        /// <summary>
        /// Draws a simple progress bar (just the chunks with a grey 1 pixel border).
        /// </summary>
        /// <param name="g">The device context used for drawing.</param>
        /// <param name="r">The rectangle that specifies where it should be drawn.</param>
        /// <param name="progress">The value of the progress. Should be between 0.0 and 1.0.</param>
        /// <param name="color">The color of the progress bar.</param>
        public static void DrawSimpleProgressBar(Graphics g, Rectangle r, double progress, SimpleProgressBarColor color)
        {
            using (Bitmap bitmap = new Bitmap(r.Width, r.Height))
            {
                using (Graphics gg = Graphics.FromImage(bitmap))
                {
                    //fill background in white
                    gg.FillRectangle(Brushes.White, new Rectangle(0, 0, r.Width, r.Height));
                    Rectangle chunks = new Rectangle(0, 0, (int)(r.Width * progress), r.Height);

                    if (Application.RenderWithVisualStyles)
                    {
                        ProgressBarRenderer.DrawHorizontalChunks(gg, chunks);
                    }
                    else
                    {
                        var brush = Brushes.LightGreen;

                        if (color == SimpleProgressBarColor.Red)
                        {
                            brush = Brushes.Red;
                        }
                        else if (color == SimpleProgressBarColor.Blue)
                        {
                            brush = Brushes.Blue;
                        }

                        gg.FillRectangle(brush, chunks);
                    }
                }

                if (Application.RenderWithVisualStyles && color != SimpleProgressBarColor.Green)
                {
                    for (int i = 0; i < bitmap.Width * progress; i++)
                    {
                        for (int j = 0; j < bitmap.Height; j++)
                        {
                            Color c = bitmap.GetPixel(i, j);

                            if (color == SimpleProgressBarColor.Blue)
                            {
                                bitmap.SetPixel(i, j, Color.FromArgb(c.R, c.B, c.G));
                            }
                            else
                            {
                                //red
                                bitmap.SetPixel(i, j, Color.FromArgb(c.G, c.R, c.B));
                            }
                        }
                    }
                }

                // draw progress bar
                g.DrawImage(bitmap, r);

                // draw rectangle around progress bar.
                g.DrawRectangle(SystemPens.ControlDark, r);
            }
        }

        /// <summary>
        /// Converts input image to grey scale using Drawing.GreyScaleAttributes
        /// </summary>
        /// <param name="source">Image to convert</param>
        /// <returns>Image converted to grey scale</returns>
        public static Image ConvertToGreyScale(Image source)
        {
            var bitmap = new Bitmap(source.Width, source.Height);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                var rectangle = new Rectangle(0, 0, source.Width, source.Height);
                graphics.DrawImage(source, rectangle, 0, 0, source.Width, source.Height, GraphicsUnit.Pixel,
                                   Drawing.GreyScaleAttributes);
                return bitmap;
            }
        }
    }
}
