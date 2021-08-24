using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using SAEditorCommon.ProjectManagement;

namespace SAToolsHub
{
	public partial class projConv : Form
	{
		//Data Strings
		string sonic = "sonic";
		string chrmodels = "chrmodels";
		string adv00models = "adv00models";
		string adv01models = "adv01models";
		string adv01cmodels = "adv01cmodels";
		string adv02models = "adv02models";
		string adv03models = "adv03models";
		string bosschaos0models = "bosschaos0models";
		string chaostggarden02mr_daytime = "chaostggarden02mr_daytime";
		string chaostggarden02mr_evening = "chaostggarden02mr_evening";
		string chaostggarden02mr_night = "chaostggarden02mr_night";

		string sonic2app = "sonic2app";
		string data_dll = "Data_DLL";

		string game;
		string gamePath;
		string projPath;
		List<Templates.SplitEntryMDL> splitMdlEntries = new List<Templates.SplitEntryMDL>();

		public projConv()
		{
			InitializeComponent();
		}

		#region Additional Functions
		void setGame()
		{
			if (File.Exists(Path.Combine(txtProjFolder.Text, "sonic_data.ini")))
				game = "sadx";
			else
				game = "sa2";
		}

		string GetSystemPath()
		{
			Templates.SplitTemplate templateFile;
			string template;

			switch (game)
			{
				case "sadx":
					template = "GameConfig/PC - SADX.xml";
					break;
				case "sa2":
					template = "GameConfig/PC - SA2.xml";
					break;
				default:
					template = "";
					break;
			}

			templateFile = ProjectFunctions.openTemplateFile(template);

			return ProjectFunctions.GetGamePath(templateFile.GameInfo.GameName);
		}
		#endregion

		#region Form Functions
		private void btnBrowse_Click(object sender, EventArgs e)
		{
			var fsd = new FolderSelect.FolderSelectDialog();
			fsd.Title = "Please select an old project folder.";
			if (fsd.ShowDialog(IntPtr.Zero))
			{
				txtProjFolder.Text = fsd.FileName;
				projPath = txtProjFolder.Text;
				setGame();
			}
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			Stream projFileStream;
			Templates.ProjectTemplate projectFile = new Templates.ProjectTemplate();

			SaveFileDialog saveFileDialog1 = new SaveFileDialog();
			saveFileDialog1.Filter = "Project File (*.sap)|*.sap";
			saveFileDialog1.RestoreDirectory = true;

			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if ((projFileStream = saveFileDialog1.OpenFile()) != null)
				{
					gamePath = GetSystemPath();

					XmlSerializer serializer = new XmlSerializer(typeof(Templates.ProjectTemplate));
					TextWriter writer = new StreamWriter(projFileStream);

					Templates.ProjectInfo projInfo = new Templates.ProjectInfo();

					switch (game)
					{
						case "sadx":
							projInfo.GameName = "SADXPC";
							projInfo.CanBuild = true;
							projInfo.GameSystemFolder = gamePath;
							projInfo.ModSystemFolder = projPath;

							List<Templates.SplitEntry> dxsplitEntries = new List<Templates.SplitEntry>()
							{
								new Templates.SplitEntry { SourceFile=(sonic + ".exe"), IniFile=(sonic + "_data.ini"), CmnName="Executable Data"},
								new Templates.SplitEntry { SourceFile=(chrmodels + "_orig.dll"), IniFile=(chrmodels + "_orig_data.ini"), CmnName="Chrmodels Data"},
								new Templates.SplitEntry { SourceFile=(adv00models + ".dll"), IniFile=(adv00models + "_data.ini"), CmnName="Station Square Data"},
								new Templates.SplitEntry { SourceFile=(adv01models + ".dll"), IniFile=(adv01models + "_data.ini"), CmnName="Egg Carrier (Exterior) Data"},
								new Templates.SplitEntry { SourceFile=(adv01cmodels + ".dll"), IniFile=(adv01cmodels + "_data.ini"), CmnName="Egg Carrier (Interior) Data"},
								new Templates.SplitEntry { SourceFile=(adv02models + ".dll"), IniFile=(adv02models + "_data.ini"), CmnName="Mystic Ruins Data"},
								new Templates.SplitEntry { SourceFile=(adv03models + ".dll"), IniFile=(adv03models + "_data.ini"), CmnName="The Past Data"},
								new Templates.SplitEntry { SourceFile=(bosschaos0models + ".dll"), IniFile=(bosschaos0models + "_data.ini"), CmnName="Boss Chaos 0 Data"},
								new Templates.SplitEntry { SourceFile=(chaostggarden02mr_daytime + ".dll"), IniFile=(chaostggarden02mr_daytime + "_data.ini"), CmnName="MR Garden (Daytime) Data"},
								new Templates.SplitEntry { SourceFile=(chaostggarden02mr_evening + ".dll"), IniFile=(chaostggarden02mr_evening + "_data.ini"), CmnName="MR Garden (Evening) Data"},
								new Templates.SplitEntry { SourceFile=(chaostggarden02mr_night + ".dll"), IniFile=(chaostggarden02mr_night + "_data.ini"), CmnName="MR Garden (Night) Data"},
							};

							projectFile.GameInfo = projInfo;
							projectFile.SplitEntries = dxsplitEntries;

							break;
						case "sa2":
							projInfo.GameName = "SA2PC";
							projInfo.CanBuild = true;
							projInfo.GameSystemFolder = gamePath;
							projInfo.ModSystemFolder = projPath;

							List<Templates.SplitEntry> sa2splitEntries = new List<Templates.SplitEntry>()
							{
								new Templates.SplitEntry { SourceFile=(sonic2app + ".exe"), IniFile=(sonic2app + "_data.ini"), CmnName="Executable Data"},
								new Templates.SplitEntry { SourceFile=(data_dll + "_orig.dll"), IniFile=(data_dll + "_orig_data.ini"), CmnName="Data DLL Data"},
							};

							projectFile.GameInfo = projInfo;
							projectFile.SplitEntries = sa2splitEntries;
							projectFile.SplitMDLEntries = splitMdlEntries;

							break;
					}

					serializer.Serialize(writer, projectFile);
					projFileStream.Close();
				}
			}

			DialogResult xmlCreated = MessageBox.Show(("Project XML Successfully Created!"), "Project XML Created", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			if (xmlCreated == DialogResult.OK)
				this.Close();
		}
		#endregion
	}
}
