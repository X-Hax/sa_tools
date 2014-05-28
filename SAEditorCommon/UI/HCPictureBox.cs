using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
    /// <summary>
    /// Poorly extended version of the PictureBox control that forces normal drawing in High Contrast mode.
    /// </summary>
    public partial class HCPictureBox : PictureBox
    {
        public HCPictureBox()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            #region Background Image Rendering
            if (BackgroundImage != null)
            {
                switch(SizeMode)
                {
                    case(PictureBoxSizeMode.AutoSize):
                        pe.Graphics.DrawImage(new Bitmap(BackgroundImage), 0, 0, this.Width, this.Height);
                        break;

                    case(PictureBoxSizeMode.CenterImage):
                        pe.Graphics.DrawImage(new Bitmap(BackgroundImage), (this.Width / 2) - (BackgroundImage.Width / 2), (this.Height / 2) - (BackgroundImage.Height / 2));
                        break;

                    case(PictureBoxSizeMode.Zoom):
                        // TODO: Add aspect ratio support
                        pe.Graphics.DrawImage(new Bitmap(BackgroundImage), 0, 0, this.Width, this.Height);
                        break;

                    case(PictureBoxSizeMode.Normal):
                        pe.Graphics.DrawImage(new Bitmap(BackgroundImage), 0, 0);
                        break;

                    case(PictureBoxSizeMode.StretchImage):
                        pe.Graphics.DrawImage(new Bitmap(BackgroundImage), 0, 0, this.Width, this.Height);
                        break;
                        
                    default:
                        pe.Graphics.DrawImage(new Bitmap(BackgroundImage), 0, 0);
                        break;
                }
            }
            #endregion

            #region Foreground Image Rendering
            if (Image != null)
            {
                switch (SizeMode)
                {
                    case (PictureBoxSizeMode.AutoSize):
                        pe.Graphics.DrawImage(new Bitmap(Image), 0, 0, this.Width, this.Height);
                        break;

                    case (PictureBoxSizeMode.CenterImage):
                        pe.Graphics.DrawImage(new Bitmap(Image), (this.Width / 2) - (Image.Width / 2), (this.Height / 2) - (Image.Height / 2));
                        break;

                    case (PictureBoxSizeMode.Zoom):
                        // TODO: Add aspect ratio support
                        pe.Graphics.DrawImage(new Bitmap(Image), 0, 0, this.Width, this.Height);
                        break;

                    case (PictureBoxSizeMode.Normal):
                        pe.Graphics.DrawImage(new Bitmap(Image), 0, 0);
                        break;

                    case (PictureBoxSizeMode.StretchImage):
                        pe.Graphics.DrawImage(new Bitmap(Image), 0, 0, this.Width, this.Height);
                        break;

                    default:
                        pe.Graphics.DrawImage(new Bitmap(Image), 0, 0);
                        break;
                }
            }
            #endregion
        }
    }
}
