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
using SonicRetro.SAModel.SAEditorCommon.ModManagement;
using ProjectManagement;

namespace SAToolsHub
{
	public partial class buildWindow : Form
	{
		SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow manualBuildWindow =
			new SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow();

		Dictionary<string, SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType> assemblies =
				new Dictionary<string, SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType>();

		SA_Tools.Game game;
		string modName;

		void setAssemblies()
		{
			assemblies.Clear();
			List<SplitEntry> splitEntry = SAToolsHub.projSplitEntries;
			switch (game)
			{
				case (SA_Tools.Game.SADX):
					for (int i = 0; i < checkedListBox1.Items.Count; i++)
					{
						if (checkedListBox1.GetItemChecked(i))
						{
							if (splitEntry[i].SourceFile.Contains("exe"))
							{
								assemblies.Add(splitEntry[i].IniFile, SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.Exe);
							}
							else
							{
								if (splitEntry[i].IniFile == "chrmodels")
								{
									assemblies.Add((splitEntry[i].IniFile + "_orig"), SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.DLL);
								}
								else
								{
									assemblies.Add(splitEntry[i].IniFile, SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.DLL);
								}
							}
						}
					}
					break;

				case (SA_Tools.Game.SA2B):
					for (int i = 0; i < checkedListBox1.Items.Count; i++)
					{
						if (checkedListBox1.GetItemChecked(i))
						{
							if (splitEntry[i].SourceFile.Contains("exe"))
							{
								assemblies.Add(splitEntry[i].IniFile, SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.Exe);
							}
							else
							{
								if (splitEntry[i].IniFile == "data_dll")
								{
									assemblies.Add((splitEntry[i].IniFile + "_orig"), SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.DLL);
								}
								else
								{
									assemblies.Add(splitEntry[i].IniFile, SonicRetro.SAModel.SAEditorCommon.ManualBuildWindow.AssemblyType.DLL);
								}
							}
						}
					}
					break;

				default:
					break;
			}
		}

		public buildWindow()
		{
			InitializeComponent();
		}

		private void buildWindow_Shown(object sender, EventArgs e)
		{
			if (SAToolsHub.setGame == "SADX")
			{
				game = SA_Tools.Game.SADX;
			}
			else if (SAToolsHub.setGame == "SA2PC")
			{
				game = SA_Tools.Game.SA2B;
			}

			checkedListBox1.Items.Clear();
			switch(game)
			{
				case (SA_Tools.Game.SADX):
					SADXModInfo sadxMod = SA_Tools.IniSerializer.Deserialize<SADXModInfo>(Path.Combine(SAToolsHub.projectDirectory.ToString(), "mod.ini"));
					modName = sadxMod.Name;
					break;
				case (SA_Tools.Game.SA2B):
					SA2ModInfo sa2Mod = SA_Tools.IniSerializer.Deserialize<SA2ModInfo>(Path.Combine(SAToolsHub.projectDirectory.ToString(), "mod.ini"));
					modName = sa2Mod.Name;
					break;
				default:
					break;
			}

			foreach(SplitEntry splitEntry in SAToolsHub.projSplitEntries)
			{
				checkedListBox1.Items.Add(splitEntry.IniFile);
			}
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			if (chkAll.Checked)
			{
				for (int i = 0; i < checkedListBox1.Items.Count; i++)
					checkedListBox1.SetItemCheckState(i, CheckState.Checked);
				checkedListBox1.Enabled = false;
			}
			else if (!chkAll.Checked)
			{
				checkedListBox1.Enabled = true;
			}
		}

		private void btnManual_Click(object sender, EventArgs e)
		{
			setAssemblies();
			manualBuildWindow.Initalize(game, modName, SAToolsHub.projectDirectory.ToString(),
				Path.Combine(SAToolsHub.gameSystemDirectory.ToString(), "mods"), assemblies);
			manualBuildWindow.ShowDialog();
		}

		private void btnAuto_Click(object sender, EventArgs e)
		{
			setAssemblies();
		}
	}
}
