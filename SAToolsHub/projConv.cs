using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using Ookii.Dialogs.Wpf;
using SAEditorCommon.ProjectManagement;
using Fclp.Internals.Extensions;

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
		List<SplitEntryMDL> splitMdlEntries = new List<SplitEntryMDL>();

		public projConv()
		{
			InitializeComponent();
		}

		void setGame()
		{
			if (File.Exists(Path.Combine(txtProjFolder.Text, "sonic_data.ini")))
				game = "sadx";
			else
				game = "sa2";
		}

		string GetSystemPath()
		{
			string template;

			switch (game)
			{
				case "sadx":
					template = "Configuration/Templates/sadxpc_template.xml";
					break;
				case "sa2":
					template = "Configuration/Templates/sa2pc_template.xml";
					break;
				default:
					template = "";
					break;
			}

			var templateFileSerializer = new XmlSerializer(typeof(SplitTemplate));
			var templateFileStream = File.OpenRead(template);
			var templateFile = (SplitTemplate)templateFileSerializer.Deserialize(templateFileStream);

			gamePath = templateFile.GameInfo.GameSystemFolder;

			if (gamePath.IsNullOrWhiteSpace())
			{
				DialogResult gamePathWarning = MessageBox.Show(("No game path was found for this game's template. Please browse to the game's installation folder."), "Game Path Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				if (gamePathWarning == DialogResult.OK)
				{
					var folderDialog = new VistaFolderBrowserDialog();
					var folderResult = folderDialog.ShowDialog();
					if (folderResult.HasValue && folderResult.Value)
					{
						gamePath = folderDialog.SelectedPath;
						if (game == "sa2")
							splitMdlEntries = templateFile.SplitMDLEntries;
					}
					else
					{
						DialogResult pathWarning = MessageBox.Show(("No path was supplied."), "No Path Supplied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						if (pathWarning == DialogResult.OK)
						{
							this.Close();
						}
					}
				}
			}

			return gamePath;
		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			var folderDialog = new VistaFolderBrowserDialog();
			var folderResult = folderDialog.ShowDialog();
			if (folderResult.HasValue && folderResult.Value)
			{
				txtProjFolder.Text = folderDialog.SelectedPath;
				projPath = txtProjFolder.Text;
				setGame();
			}
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			Stream projFileStream;
			ProjectTemplate projectFile = new ProjectTemplate();

			SaveFileDialog saveFileDialog1 = new SaveFileDialog();
			saveFileDialog1.Filter = "Project File (*.xml)|*.xml";
			saveFileDialog1.RestoreDirectory = true;

			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if ((projFileStream = saveFileDialog1.OpenFile()) != null)
				{
					GetSystemPath();

					XmlSerializer serializer = new XmlSerializer(typeof(ProjectTemplate));
					TextWriter writer = new StreamWriter(projFileStream);

					ProjectInfo projInfo = new ProjectInfo();

					switch (game)
					{
						case "sadx":
							projInfo.GameName = "SADXPC";
							projInfo.CanBuild = true;
							projInfo.GameSystemFolder = gamePath;
							projInfo.ModSystemFolder = projPath;

							List<SplitEntry> dxsplitEntries = new List<SplitEntry>()
							{
								new SplitEntry { SourceFile=(sonic + ".exe"), IniFile=(sonic + "_data.ini"), CmnName="Executable Data"},
								new SplitEntry { SourceFile=(chrmodels + "_orig.dll"), IniFile=(chrmodels + "_orig_data.ini"), CmnName="Chrmodels Data"},
								new SplitEntry { SourceFile=(adv00models + ".dll"), IniFile=(adv00models + "_data.ini"), CmnName="Station Square Data"},
								new SplitEntry { SourceFile=(adv01models + ".dll"), IniFile=(adv01models + "_data.ini"), CmnName="Egg Carrier (Exterior) Data"},
								new SplitEntry { SourceFile=(adv01cmodels + ".dll"), IniFile=(adv01cmodels + "_data.ini"), CmnName="Egg Carrier (Interior) Data"},
								new SplitEntry { SourceFile=(adv02models + ".dll"), IniFile=(adv02models + "_data.ini"), CmnName="Mystic Ruins Data"},
								new SplitEntry { SourceFile=(adv03models + ".dll"), IniFile=(adv03models + "_data.ini"), CmnName="The Past Data"},
								new SplitEntry { SourceFile=(bosschaos0models + ".dll"), IniFile=(bosschaos0models + "_data.ini"), CmnName="Boss Chaos 0 Data"},
								new SplitEntry { SourceFile=(chaostggarden02mr_daytime + ".dll"), IniFile=(chaostggarden02mr_daytime + "_data.ini"), CmnName="MR Garden (Daytime) Data"},
								new SplitEntry { SourceFile=(chaostggarden02mr_evening + ".dll"), IniFile=(chaostggarden02mr_evening + "_data.ini"), CmnName="MR Garden (Evening) Data"},
								new SplitEntry { SourceFile=(chaostggarden02mr_night + ".dll"), IniFile=(chaostggarden02mr_night + "_data.ini"), CmnName="MR Garden (Night) Data"},
							};

							projectFile.GameInfo = projInfo;
							projectFile.SplitEntries = dxsplitEntries;

							break;
						case "sa2":
							projInfo.GameName = "SA2PC";
							projInfo.CanBuild = true;
							projInfo.GameSystemFolder = gamePath;
							projInfo.ModSystemFolder = projPath;

							List<SplitEntry> sa2splitEntries = new List<SplitEntry>()
							{
								new SplitEntry { SourceFile=(sonic2app + ".exe"), IniFile=(sonic2app + "_data.ini"), CmnName="Executable Data"},
								new SplitEntry { SourceFile=(data_dll + "_orig.dll"), IniFile=(data_dll + "_orig_data.ini"), CmnName="Data DLL Data"},
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
	}
}
