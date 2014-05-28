using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SonicRetro.SAModel;
using SonicRetro.SAModel.Direct3D.TextureSystem;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
    public partial class TexturePicker : Form
    {
        public int SelectedValue { get { return listView1.SelectedIndices[0]; } }
        private int initialSelection = 0;
        private List<Bitmap> textureList;

        public TexturePicker(BMPInfo[] textureInfo, int InitialSelection)
        {
            InitializeComponent();

            this.textureList = new List<Bitmap>();
            initialSelection = InitialSelection;

            for (int bmpIndx = 0; bmpIndx < textureInfo.Length; bmpIndx++)
            {
                textureList.Add(textureInfo[bmpIndx].Image);
            }
        }

        private void TexturePicker_Load(object sender, EventArgs e)
        {
            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(128, 128);
            foreach (Bitmap map in textureList)
            {
                try
                {
                    imageList.Images.Add(map);
                }
                catch (Exception ex)
                {
                    string errorMessage = ex.Message;
                }
            }

            listView1.LargeImageList = imageList;

            for (int texid = 0; texid < textureList.Count; texid++)
            {
                listView1.Items.Add(texid.ToString(), texid);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedIndices[0] > -1)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
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
