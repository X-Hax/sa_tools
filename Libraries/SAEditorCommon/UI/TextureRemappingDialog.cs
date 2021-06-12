using SAModel.Direct3D.TextureSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SAModel.SAEditorCommon.UI
{
	public partial class TextureRemappingDialog : Form
	{
		public TextureRemappingDialog(IList<BMPInfo> images)
		{
			InitializeComponent();
			TextureMap = new Dictionary<int, int>();
			imageList1.Images.AddRange(images.Select(a => a.Image).ToArray());
			SourceTileList.BeginUpdate();
			DestinationTileList.BeginUpdate();
			for (int i = 0; i < images.Count; i++)
			{
				SourceTileList.Items.Add(images[i].Name, i);
				DestinationTileList.Items.Add(images[i].Name, i);
			}
			SourceTileList.EndUpdate();
			DestinationTileList.EndUpdate();
		}

		public Dictionary<int, int> TextureMap { get; private set; }

		private void button1_Click(object sender, EventArgs e)
		{
			int src = SourceTileList.SelectedIndices[0];
			int dst = DestinationTileList.SelectedIndices[0];
			if (TextureMap.ContainsKey(src))
			{
				if (MessageBox.Show(this, "Source item is already in list! Do you want to replace it?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
					return;
				listBox1.Items[TextureMap.Keys.ToList().IndexOf(src)] = src.ToString("X") + "=" + dst.ToString("X");
			}
			else
				listBox1.Items.Add(src.ToString("X") + "=" + dst.ToString("X"));
			TextureMap[src] = dst;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			TextureMap.Remove(TextureMap.Keys.ToList()[listBox1.SelectedIndex]);
			listBox1.Items.RemoveAt(listBox1.SelectedIndex);
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			button2.Enabled = listBox1.SelectedIndex != -1;
		}

		private void TileList_SelectedIndexChanged(object sender, EventArgs e)
		{
			button1.Enabled = SourceTileList.SelectedIndices.Count > 0 && DestinationTileList.SelectedIndices.Count > 0;
		}
	}
}
