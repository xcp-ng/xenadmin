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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace XenAdmin.Controls
{
    /// <summary>
    /// Lightweight control to support easy display of the rotating spinning icon and a configurable success icon.
    /// </summary>
    public class SpinnerIcon : PictureBox
    {
        #region Fields

        private readonly Timer spinningTimer = new Timer();
        private const int SPEED = 150;
        private int currentPosition;
        private Image succeededImage;

        private readonly Image[] spinningImageFrames =
        {
            Images.StaticImages.SpinningFrame0,
            Images.StaticImages.SpinningFrame1,
            Images.StaticImages.SpinningFrame2,
            Images.StaticImages.SpinningFrame3,
            Images.StaticImages.SpinningFrame4,
            Images.StaticImages.SpinningFrame5,
            Images.StaticImages.SpinningFrame6,
            Images.StaticImages.SpinningFrame7
        };

        #endregion

        #region Properties

        /// <summary>
        /// Image to be displayed when DisplaySucceededImage() was called.
        /// </summary>
        [Browsable(true), Category("Appearance"), Description("SucceededImage")]
        [DefaultValue(typeof(int), "Image that is displayed after DisplaySucceededImage() was invoked.")]
        public Image SucceededImage
        {
            get { return succeededImage; }
            set
            {
                succeededImage = value;
                Invalidate();
            }
        }

        #endregion

        public SpinnerIcon()
        {
            spinningTimer.Interval = SPEED;
            spinningTimer.Tick += spinningTimer_Tick;
        }

        private void spinningTimer_Tick(object sender, EventArgs e)
        {
            if (currentPosition == spinningImageFrames.Length)
                currentPosition = 0;

            Image = spinningImageFrames[currentPosition++];
        }

        /// <summary>
        /// Displays the spinning icon and sets Visible=true on the control.
        /// </summary>
        public void StartSpinning()
        {
            currentPosition = 0;
            Image = null;
            Visible = true;
            spinningTimer.Start();
        }

        /// <summary>
        /// Stops the spinning and sets Visible=false on the control.
        /// </summary>
        public void StopSpinning()
        {
            Visible = false;
            spinningTimer.Stop();
        }

        /// <summary>
        /// Displays SucceededImage image instead of the spinning icon. Sets Visible=true on the control.
        /// </summary>
        /// <remarks>It is not necessary to call StopSpinning() before calling this method.</remarks>
        public void DisplaySucceededImage()
        {
            StopSpinning();
            Image = succeededImage;
            Visible = true;
        }

        protected override void Dispose(bool disposing)
        {
            spinningTimer.Stop();
            spinningTimer.Tick -= spinningTimer_Tick;
            spinningTimer.Dispose();

            base.Dispose(disposing);
        }
    }
}