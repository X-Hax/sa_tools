using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAModel.SAMDL
{
	public partial class AssimpImportDialog : Form
	{
		public ModelFormat Format = ModelFormat.BasicDX;
		public bool ImportAsNodes = true;
		public bool ImportColladaRootNode = false;
		public bool LegacyOBJImport = false;

		public AssimpImportDialog(ModelFormat deffmt, bool enableOBJ)
		{
			InitializeComponent();
			checkBoxLegacyOBJ.Enabled = enableOBJ;
			switch (deffmt)
			{
				case ModelFormat.Chunk:
					radioButtonChunkModel.Checked = true;
					radioButtonBasicModel.Checked = radioButtonGCModel.Checked = false;
					break;
				case ModelFormat.GC:
					radioButtonGCModel.Checked = true;
					radioButtonBasicModel.Checked = radioButtonChunkModel.Checked = false;
					break;
				case ModelFormat.Basic:
				case ModelFormat.BasicDX:
				default:
					radioButtonBasicModel.Checked = true;
					radioButtonChunkModel.Checked = radioButtonGCModel.Checked = false;
					break;
			}
		}

		private void AssimpImport_HelpButtonClicked(object sender, CancelEventArgs e)
		{
			System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd", $"/c start " + "https://github.com/X-Hax/sa_tools/wiki/SAMDL#model-import") { CreateNoWindow = false });
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if (radioButtonBasicModel.Checked)
				Format = ModelFormat.BasicDX;
			else if (radioButtonChunkModel.Checked)
				Format = ModelFormat.Chunk;
			else
				Format = ModelFormat.GC;
			ImportAsNodes = radioButtonNodes.Checked;
			ImportColladaRootNode = checkBoxImportColladaRootNode.Checked;
			LegacyOBJImport = checkBoxLegacyOBJ.Checked;
		}

		private void checkBoxLegacyOBJ_CheckedChanged(object sender, EventArgs e)
		{
			if (checkBoxLegacyOBJ.Checked)
			{
				radioButtonBasicModel.Checked = radioButtonSingleModel.Checked = true;
				radioButtonChunkModel.Checked = radioButtonGCModel.Checked = groupBoxModelFormat.Enabled = checkBoxImportColladaRootNode.Enabled = checkBoxImportColladaRootNode.Checked = radioButtonNodes.Enabled = radioButtonNodes.Checked = radioButtonSingleModel.Enabled = false;
			}
			else
			{
				groupBoxModelFormat.Enabled = checkBoxImportColladaRootNode.Enabled = radioButtonNodes.Enabled = radioButtonSingleModel.Enabled = true;
			}
		}
	}
}
