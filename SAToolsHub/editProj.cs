using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SA_Tools;
using SonicRetro.SAModel.SAEditorCommon.ModManagement;
using Fclp.Internals.Extensions;

namespace SAToolsHub
{
	public partial class editProj : Form
	{

		string projDir;
		string projModFile;

		void setVariables()
		{
			if (!SAToolsHub.projectDirectory.FullName.IsNullOrWhiteSpace())
			{
				projDir = SAToolsHub.projectDirectory.FullName;
				projModFile = Path.Combine(projDir, "mod.ini");
			}
		}



		public editProj()
		{
			InitializeComponent();
		}

		private void editProj_Shown(object sender, EventArgs e)
		{
			setVariables();
			if (SAToolsHub.setGame == "SADX")
			{
				SADXModInfo modInfo = SA_Tools.IniSerializer.Deserialize<SADXModInfo>(projModFile);
				txtName.Text = modInfo.Name;
				txtAuth.Text = modInfo.Author;
				txtDesc.Text = modInfo.Description;
				txtVerNum.Text = modInfo.Version;

				if (!modInfo.DLLFile.IsNullOrWhiteSpace())
				{
					chkDLLFile.Checked = true;
					txtDLLName.Text = modInfo.DLLFile;
				}
				if (modInfo.RedirectMainSave)
					chkMainRedir.Checked = true;
				if (modInfo.RedirectChaoSave)
					chkChaoRedir.Checked = true;
			}

			if (SAToolsHub.setGame == "SA2PC")
			{
				SA2ModInfo modInfo = SA_Tools.IniSerializer.Deserialize<SA2ModInfo>(projModFile);
				txtName.Text = modInfo.Name;
				txtAuth.Text = modInfo.Author;
				txtDesc.Text = modInfo.Description;
				txtVerNum.Text = modInfo.Version;
			}
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			//setVariables();
			if (SAToolsHub.setGame == "SADX")
			{
				SADXModInfo modInfo = new SADXModInfo
				{
					Name = txtName.Text,
					Author = txtAuth.Text,
					Description = txtDesc.Text,
					Version = txtVerNum.Text,
				};
				if (chkDLLFile.Checked)
					modInfo.DLLFile = txtDLLName.Text;
				if (chkMainRedir.Checked)
					modInfo.RedirectMainSave = true;
				if (chkChaoRedir.Checked)
					modInfo.RedirectChaoSave = true;

				SA_Tools.IniSerializer.Serialize(modInfo, projModFile);
			}

			this.Close();
		}
	}
}
