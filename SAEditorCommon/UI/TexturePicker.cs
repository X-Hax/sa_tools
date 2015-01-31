using System;
using System.Linq;
using System.Windows.Forms;

using SonicRetro.SAModel.Direct3D.TextureSystem;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
	public partial class TexturePicker : Form
	{
		public int SelectedValue { get { return listView.SelectedIndices[0]; } }
		private int initialSelection = 0;
		BMPInfo[] textureInfo;

		public TexturePicker(BMPInfo[] textureInfo, int initialSelection)
		{
			InitializeComponent();

			this.textureInfo = textureInfo;
			this.initialSelection = initialSelection;
		}

		private void TexturePicker_Load(object sender, EventArgs e)
		{
			for (int i = 0; i < textureInfo.Length; i++)
			{
				imageList.Images.Add(ResizeImage(textureInfo[i].Image, imageList.ImageSize));
				listView.Items.Add(i + ": " + textureInfo[i].Name, i);
			}

			// Selects the desired texture and then esures its visiblity (by scrolling)
			listView.Items[initialSelection].Selected = true;
			listView.EnsureVisible(initialSelection);
		}

		/// <summary>
		/// Resizes the <see cref="Bitmap" />, maintaining the original aspect ratio.
		/// </summary>
		Bitmap ResizeImage(Bitmap image, Size newsize)
		{
			Bitmap bmp = new Bitmap(newsize.Width, newsize.Height);
			Graphics gfx = Graphics.FromImage(bmp);
			gfx.CompositingQuality = CompositingQuality.HighQuality;
			gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
			gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
			gfx.SmoothingMode = SmoothingMode.HighQuality;
			gfx.Clear(Color.Transparent);
			int mywidth = image.Width;
			int myheight = image.Height;
			while (myheight > newsize.Height | mywidth > newsize.Width)
			{
				if (mywidth > newsize.Width)
				{
					mywidth = newsize.Width;
					myheight = (int)(image.Height * ((double)newsize.Width / image.Width));
				}
				else if (myheight > newsize.Height)
				{
					myheight = newsize.Height;
					mywidth = (int)(image.Width * ((double)newsize.Height / image.Height));
				}
			}
			gfx.DrawImage(image, (int)(((double)newsize.Width - mywidth) / 2), (int)(((double)newsize.Height - myheight) / 2), mywidth, myheight);
			return bmp;
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Close();
		}

		private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (listView.SelectedIndices[0] > -1)
			{
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
				this.Close();
			}
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			if (listView.SelectedIndices.Count > 0)
			{
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
				this.Close();
			}
			else
			{
				MessageBox.Show("Please select a texture!");
			}
		}
	}
}