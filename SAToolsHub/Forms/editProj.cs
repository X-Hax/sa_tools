using System;
using System.IO;
using System.Windows.Forms;
using System.Text;
using SAModel.SAEditorCommon.ModManagement;

namespace SAToolsHub
{
	public partial class editProj : Form
	{

		string projDir;
		string projModFile;

		string[] CatItems = new string[]
		{
			"Animations",
			"Chao",
			"Custom Level",
			"Cutscene",
			"Game Overhaul",
			"Gameplay",
			"Misc",
			"Music",
			"Patch",
			"Skin",
			"Sound",
			"Textures",
			"UI"
		};

		public editProj()
		{
			InitializeComponent();
			txtDLLName.Enabled = false;

			foreach (string item2 in CatItems)
			{
				comboModCategory.Items.Add(item2);
			}
		}

		#region Additional Functions
		void setVariables()
		{
			if (SAToolsHub.projectDirectory != null)
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
		#endregion

		#region Form Functions
		private void editProj_Shown(object sender, EventArgs e)
		{
			setVariables();
			clearVariables();
			string game = SAToolsHub.setGame;
			radGitHub.Checked = true;

			switch (game)
			{
				case "SADXPC":
					SADXModInfo modInfoSADX = SplitTools.IniSerializer.Deserialize<SADXModInfo>(projModFile);
					txtName.Text = modInfoSADX.Name;
					txtAuth.Text = modInfoSADX.Author;
					txtDesc.Text = modInfoSADX.Description;
					txtVerNum.Text = modInfoSADX.Version;
					textModID.Text = modInfoSADX.ModID;
					comboModCategory.Text = modInfoSADX.Category;

					if (modInfoSADX.DLLFile != null)
					{
						chkDLLFile.Checked = true;
						txtDLLName.Text = modInfoSADX.DLLFile;
					}
					if (modInfoSADX.RedirectMainSave)
						chkMainRedir.Checked = true;
					if (modInfoSADX.RedirectChaoSave)
						chkChaoRedir.Checked = true;
					if (modInfoSADX.GitHubRepo != null || modInfoSADX.GameBananaItemId.HasValue || modInfoSADX.UpdateUrl != null)
					{
						chkUpdates.Checked = true;
						if (modInfoSADX.GitHubRepo != null)
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
						if (modInfoSADX.UpdateUrl != null)
						{
							radManual.Checked = true;
							txtUpdateURL.Text = modInfoSADX.UpdateUrl;
							txtAsset.Text = modInfoSADX.ChangelogUrl;
						}
					}
					break;

				case "SA2PC":
					SA2ModInfo modInfoSA2PC = SplitTools.IniSerializer.Deserialize<SA2ModInfo>(projModFile);
					txtName.Text = modInfoSA2PC.Name;
					txtAuth.Text = modInfoSA2PC.Author;
					txtDesc.Text = modInfoSA2PC.Description;
					txtVerNum.Text = modInfoSA2PC.Version;
					textModID.Text = modInfoSA2PC.ModID;
					comboModCategory.Text = modInfoSA2PC.Category;

					if (modInfoSA2PC.DLLFile != null)
					{
						chkDLLFile.Checked = true;
						txtDLLName.Text = modInfoSA2PC.DLLFile;
					}
					if (modInfoSA2PC.RedirectMainSave)
						chkMainRedir.Checked = true;
					if (modInfoSA2PC.RedirectChaoSave)
						chkChaoRedir.Checked = true;
					if (modInfoSA2PC.GitHubRepo != null || modInfoSA2PC.GameBananaItemId.HasValue || modInfoSA2PC.UpdateUrl != null)
					{
						chkUpdates.Checked = true;
						if (modInfoSA2PC.GitHubRepo != null)
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
						if (modInfoSA2PC.UpdateUrl != null)
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
					modInfoSADX.Category = comboModCategory.Text;
					modInfoSADX.Description = txtDesc.Text;
					modInfoSADX.Version = txtVerNum.Text;
					modInfoSADX.ModID = textModID.Text;
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

					SplitTools.IniSerializer.Serialize(modInfoSADX, projModFile);
					break;

				case "SA2PC":
					SADXModInfo modInfoSA2PC = new SADXModInfo();
					modInfoSA2PC.Name = txtName.Text;
					modInfoSA2PC.Author = txtAuth.Text;
					modInfoSA2PC.Description = txtDesc.Text;
					modInfoSA2PC.Version = txtVerNum.Text;
					modInfoSA2PC.Category = comboModCategory.Text;
					modInfoSA2PC.ModID = textModID.Text;
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

					SplitTools.IniSerializer.Serialize(modInfoSA2PC, projModFile);
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
			lblLower.Text = "";
			txtAsset.Visible = false;
		}

		private void chkDLLFile_CheckedChanged(object sender, EventArgs e)
		{
			if (chkDLLFile.Checked)
				txtDLLName.Enabled = true;
			else
				txtDLLName.Enabled = false;
		}
		#endregion

		private void editProj_Load(object sender, EventArgs e)
		{

		}

		#region ModID Generation
		static string GetGamePrefix()
		{
			string prefix = "";
			switch (SAToolsHub.setGame)
			{
				case ("SADXPC"):
					prefix = "sadx.";
					break;

				case ("SA2PC"):
					prefix = "sa2.";
					break;
			}
			return prefix;
		}

		static string GenerateModID()
		{
			return GetGamePrefix() + ((uint)DateTime.Now.GetHashCode()).ToString();
		}

		static string RemoveSpecialCharacters(string str)
		{
			StringBuilder sb = new StringBuilder();
			foreach (char c in str)
			{
				if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == '-')
				{
					sb.Append(c);
				}
			}
			return sb.ToString().ToLowerInvariant();
		}

		static bool isStringNotEmpty(string txt)
		{
			return txt.Length > 0;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			textModID.Clear();
			string name = isStringNotEmpty(txtName.Text) ? txtName.Text : null;
			string author = isStringNotEmpty(txtAuth.Text) ? txtAuth.Text : null;

			if (name != null && author != null)
			{
				string idName = RemoveSpecialCharacters(name);
				string idAuthor = RemoveSpecialCharacters(author);
				textModID.Text = String.Format(GetGamePrefix() + "{0}.{1}", idAuthor, idName);
			}
			else
				textModID.Text = GenerateModID();
		}
		#endregion
	}
}
