using System;
using System.IO;
using System.Windows.Forms;
using SonicRetro.SAModel.SAEditorCommon.ModManagement;
using Fclp.Internals.Extensions;

namespace SAToolsHub
{
	public partial class editProj : Form
	{

		string projDir;
		string projModFile;

		string[] GBItems = new string[]
		{
			"Map",
			"Skin",
			"Sound",
			"GUI",
			"Gamefile",
			"Effect",
			"Texture",
			"Script"
		};

		void setVariables()
		{
			if (!SAToolsHub.projectDirectory.IsNullOrWhiteSpace())
			{
				projDir = SAToolsHub.projectDirectory;
				projModFile = Path.Combine(projDir, "mod.ini");
			}
		}

		void clearVariables()
		{
			txtAuth.Clear();
			txtDesc.Clear();
			txtDLLName.Clear();
			txtName.Clear();
			txtVerNum.Clear();
			txtAsset.Clear();
			txtUpdateURL.Clear();
		}

		public editProj()
		{
			InitializeComponent();
			txtDLLName.Enabled = false;
			foreach (string item in GBItems)
			{
				lstGBItems.Items.Add(item);
			}
		}

		private void editProj_Shown(object sender, EventArgs e)
		{
			setVariables();
			clearVariables();
			string game = SAToolsHub.setGame;
			radGitHub.Checked = true;

			switch (game)
			{
				case "SADXPC":
					SADXModInfo modInfoSADX = SA_Tools.IniSerializer.Deserialize<SADXModInfo>(projModFile);
					txtName.Text = modInfoSADX.Name;
					txtAuth.Text = modInfoSADX.Author;
					txtDesc.Text = modInfoSADX.Description;
					txtVerNum.Text = modInfoSADX.Version;

					if (!modInfoSADX.DLLFile.IsNullOrWhiteSpace())
					{
						chkDLLFile.Checked = true;
						txtDLLName.Text = modInfoSADX.DLLFile;
					}
					if (modInfoSADX.RedirectMainSave)
						chkMainRedir.Checked = true;
					if (modInfoSADX.RedirectChaoSave)
						chkChaoRedir.Checked = true;
					if (!modInfoSADX.GitHubRepo.IsNullOrWhiteSpace() || modInfoSADX.GameBananaItemId.HasValue || !modInfoSADX.UpdateUrl.IsNullOrWhiteSpace())
					{
						chkUpdates.Checked = true;
						if (!modInfoSADX.GitHubRepo.IsNullOrWhiteSpace())
						{
							radGitHub.Checked = true;
							txtUpdateURL.Text = modInfoSADX.GitHubRepo;
							txtAsset.Text = modInfoSADX.GitHubAsset;
						}
						if (modInfoSADX.GameBananaItemId.HasValue)
						{
							radGamebanana.Checked = true;
							txtUpdateURL.Text = modInfoSADX.GameBananaItemId.ToString();
							lstGBItems.Text = modInfoSADX.GameBananaItemType;
						}
						if (!modInfoSADX.UpdateUrl.IsNullOrWhiteSpace())
						{
							radManual.Checked = true;
							txtUpdateURL.Text = modInfoSADX.UpdateUrl;
							txtAsset.Text = modInfoSADX.ChangelogUrl;
						}
					}
					break;

				case "SA2PC":
					SA2ModInfo modInfoSA2PC = SA_Tools.IniSerializer.Deserialize<SA2ModInfo>(projModFile);
					txtName.Text = modInfoSA2PC.Name;
					txtAuth.Text = modInfoSA2PC.Author;
					txtDesc.Text = modInfoSA2PC.Description;
					txtVerNum.Text = modInfoSA2PC.Version;
					if (!modInfoSA2PC.DLLFile.IsNullOrWhiteSpace())
					{
						chkDLLFile.Checked = true;
						txtDLLName.Text = modInfoSA2PC.DLLFile;
					}
					if (modInfoSA2PC.RedirectMainSave)
						chkMainRedir.Checked = true;
					if (modInfoSA2PC.RedirectChaoSave)
						chkChaoRedir.Checked = true;
					if (!modInfoSA2PC.GitHubRepo.IsNullOrWhiteSpace() || modInfoSA2PC.GameBananaItemId.HasValue || !modInfoSA2PC.UpdateUrl.IsNullOrWhiteSpace())
					{
						chkUpdates.Checked = true;
						if (!modInfoSA2PC.GitHubRepo.IsNullOrWhiteSpace())
						{
							radGitHub.Checked = true;
							txtUpdateURL.Text = modInfoSA2PC.GitHubRepo;
							txtAsset.Text = modInfoSA2PC.GitHubAsset;
						}
						if (modInfoSA2PC.GameBananaItemId.HasValue)
						{
							radGamebanana.Checked = true;
							txtUpdateURL.Text = modInfoSA2PC.GameBananaItemId.ToString();
							txtAsset.Text = modInfoSA2PC.GameBananaItemType;
						}
						if (!modInfoSA2PC.UpdateUrl.IsNullOrWhiteSpace())
						{
							radManual.Checked = true;
							txtUpdateURL.Text = modInfoSA2PC.UpdateUrl;
							txtAsset.Text = modInfoSA2PC.ChangelogUrl;
						}
					}
					break;

				default:
					break;
			}
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			string game = SAToolsHub.setGame;

			switch (game)
			{
				case "SADXPC":
					SADXModInfo modInfoSADX = new SADXModInfo();
					modInfoSADX.Name = txtName.Text;
					modInfoSADX.Author = txtAuth.Text;
					modInfoSADX.Description = txtDesc.Text;
					modInfoSADX.Version = txtVerNum.Text;
					if (chkDLLFile.Checked)
						modInfoSADX.DLLFile = txtDLLName.Text;
					if (chkMainRedir.Checked)
						modInfoSADX.RedirectMainSave = true;
					if (chkChaoRedir.Checked)
						modInfoSADX.RedirectChaoSave = true;
					if (chkUpdates.Checked && radGitHub.Checked)
					{
						modInfoSADX.GitHubRepo = txtUpdateURL.Text;
						modInfoSADX.GitHubAsset = txtAsset.Text;
					}
					if (chkUpdates.Checked && radGamebanana.Checked)
					{
						modInfoSADX.GameBananaItemId = Convert.ToInt64(txtUpdateURL.Text);
						modInfoSADX.GameBananaItemType = lstGBItems.Text;
					}
					if (chkUpdates.Checked && radManual.Checked)
					{
						modInfoSADX.UpdateUrl = txtUpdateURL.Text;
						modInfoSADX.ChangelogUrl = txtAsset.Text;
					}

					SA_Tools.IniSerializer.Serialize(modInfoSADX, projModFile);
					break;

				case "SA2PC":
					SADXModInfo modInfoSA2PC = new SADXModInfo();
					modInfoSA2PC.Name = txtName.Text;
					modInfoSA2PC.Author = txtAuth.Text;
					modInfoSA2PC.Description = txtDesc.Text;
					modInfoSA2PC.Version = txtVerNum.Text;
					if (chkDLLFile.Checked)
						modInfoSA2PC.DLLFile = txtDLLName.Text;
					if (chkMainRedir.Checked)
						modInfoSA2PC.RedirectMainSave = true;
					if (chkChaoRedir.Checked)
						modInfoSA2PC.RedirectChaoSave = true;
					if (chkUpdates.Checked && radGitHub.Checked)
					{
						modInfoSA2PC.GitHubRepo = txtUpdateURL.Text;
						modInfoSA2PC.GitHubAsset = txtAsset.Text;
					}
					if (chkUpdates.Checked && radGamebanana.Checked)
					{
						modInfoSA2PC.GameBananaItemId = Convert.ToInt64(txtUpdateURL.Text);
						modInfoSA2PC.GameBananaItemType = lstGBItems.Text;
					}
					if (chkUpdates.Checked && radManual.Checked)
					{
						modInfoSA2PC.UpdateUrl = txtUpdateURL.Text;
						modInfoSA2PC.ChangelogUrl = txtAsset.Text;
					}

					SA_Tools.IniSerializer.Serialize(modInfoSA2PC, projModFile);
					break;

				default:
					break;
			}

			this.Close();
		}

		private void chkUpdates_CheckedChanged(object sender, EventArgs e)
		{
			if (chkUpdates.Checked)
				groupBox1.Enabled = true;
			else
				groupBox1.Enabled = false;
		}

		private void radManual_CheckedChanged(object sender, EventArgs e)
		{
			lblUpperBox.Text = "Update URL";
			lblLower.Text = "Changelog URL";
			lstGBItems.Visible = false;
			txtAsset.Visible = true;
		}

		private void radGitHub_CheckedChanged(object sender, EventArgs e)
		{
			lblUpperBox.Text = "GitHub Repository URL";
			lblLower.Text = "GitHub Asset";
			lstGBItems.Visible = false;
			txtAsset.Visible = true;
		}

		private void radGamebanana_CheckedChanged(object sender, EventArgs e)
		{
			lblUpperBox.Text = "Gamebanana ID";
			lblLower.Text = "Gamebanana Mod Type";
			lstGBItems.Visible = true;
			txtAsset.Visible = false;
		}

		private void chkDLLFile_CheckedChanged(object sender, EventArgs e)
		{
			if (chkDLLFile.Checked)
				txtDLLName.Enabled = true;
			else
				txtDLLName.Enabled = false;
		}
	}
}
