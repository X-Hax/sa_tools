using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Ookii.Dialogs;
using Ookii.Dialogs.Wpf;
using SA_Tools;
using SonicRetro.SAModel.SAEditorCommon;

namespace SAToolsHub
{
	public partial class newProj : Form
	{
		string sadxPath = Program.Settings.SADXPCPath;
		string sa2pcPath = Program.Settings.SA2PCPath;

		public newProj()
		{
			InitializeComponent();
		}

		private void newProj_Shown(object sender, EventArgs e)
		{
			bool sadxpcIsValid = GamePathChecker.CheckSADXPCValid(
				Program.Settings.SADXPCPath, out string sadxFailReason);

			bool sa2pcIsValid = GamePathChecker.CheckSA2PCValid(
				Program.Settings.SA2PCPath, out string sa2pcInvalidReason);

			radSADX.Enabled = sadxpcIsValid;
			radSA2PC.Enabled = sa2pcIsValid;

			txtAuthor.Text = String.Empty;
			txtDesc.Text = String.Empty;
			txtName.Text = String.Empty;
			txtProjFolder.Text = String.Empty;
		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			
			var folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
			var folderResult = folderDialog.ShowDialog();
			if (folderResult.HasValue && folderResult.Value)
			{
				txtProjFolder.Text = folderDialog.SelectedPath;
			}
		}

		private void btnCreate_Click(object sender, EventArgs e)
		{
			Stream projFileStream;
			SaveFileDialog saveFileDialog1 = new SaveFileDialog();
			saveFileDialog1.Filter = "Project File (*.xml)|*.xml";
			saveFileDialog1.RestoreDirectory = true;
			string projFolder;
			if (radSADX.Checked)
			{
				saveFileDialog1.InitialDirectory = sadxPath;
			}
			else if (radSA2PC.Checked)
			{
				saveFileDialog1.InitialDirectory = sa2pcPath;
			}


			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if ((projFileStream = saveFileDialog1.OpenFile()) != null)
				{
					XmlSerializer serializer = new XmlSerializer(typeof(ProjectTemplate.ProjectTemplate));
					TextWriter writer = new StreamWriter(projFileStream);
					if (checkBox1.Checked)
					{
						projFolder = txtProjFolder.Text;
					}
					else
					{
						projFolder = Path.Combine((Path.GetDirectoryName(saveFileDialog1.FileName)), Path.GetFileNameWithoutExtension(saveFileDialog1.FileName));
						if (!Directory.Exists(projFolder))
						{
							Directory.CreateDirectory(projFolder);
						}
					}

					ProjectTemplate.ProjectTemplate templateFile = new ProjectTemplate.ProjectTemplate();
					if (radSADX.Checked)
					{
						templateFile.GameName = "SADX";
						templateFile.CanBuild = true;
						templateFile.GameSystemFolder = sadxPath;
					}
					else if (radSA2PC.Checked)
					{
						templateFile.GameName = "SA2PC";
						templateFile.CanBuild = true;
						templateFile.GameSystemFolder = sa2pcPath;
					}
					templateFile.ModSystemFolder = projFolder;

					serializer.Serialize(writer, templateFile);
					projFileStream.Close();
				}
			}
			this.Close();
		}

		private void newProj_Load(object sender, EventArgs e)
		{

		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			if (checkBox1.Checked)
			{
				txtProjFolder.Enabled = true;
				btnBrowse.Enabled = true;
			}
			else
			{
				txtProjFolder.Enabled = false;
				btnBrowse.Enabled = false;
			}
		}
	}
}
