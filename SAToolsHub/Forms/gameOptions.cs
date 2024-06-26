﻿using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using SAModel.SAEditorCommon.ModManagement;

namespace SAToolsHub
{
	public partial class gameOptions : Form
	{
		buildWindow buildWindowDiag;
		string modManager;
		string gameEXE;
		string modLoaderINI;
		bool gameLaunch;

		public gameOptions()
		{
			InitializeComponent();

			buildWindowDiag = new buildWindow();
		}

		#region Additional Functions
		void RunGame()
		{
			string gamePath = Path.Combine(SAToolsHub.gameDirectory, gameEXE);

			if (!File.Exists(gamePath))
			{
				DialogResult gameError = MessageBox.Show((gameEXE + " was not located."), "Game Not Located", MessageBoxButtons.OK);
				if (gameError == DialogResult.OK)
					this.Close();
			}
			else
			{
				Environment.CurrentDirectory = Path.GetDirectoryName(gamePath);

				Process gameProcess = Process.Start(gamePath);
			}
		}

		void RunModManager()
		{
			string managerPath = Path.Combine(SAToolsHub.gameDirectory, modManager);

			if (!File.Exists(managerPath))
			{
				DialogResult managerError = MessageBox.Show((modManager + " was not located."), "Game Not Located", MessageBoxButtons.OK);
				if (managerError == DialogResult.OK)
					this.Close();
			}
			else
			{
				Environment.CurrentDirectory = Path.GetDirectoryName(managerPath);

				Process process = Process.Start(managerPath);
			}
		}

		void UpdateModLoader(string modName)
		{
			string modLoaderIniPath = Path.Combine(SAToolsHub.gameDirectory, modLoaderINI);

			switch (SAToolsHub.setGame)
			{
				case ("SADXPC"):
					SADXLoaderInfo dxLoaderIni = SplitTools.IniSerializer.Deserialize<SADXLoaderInfo>(modLoaderIniPath);

					if (!dxLoaderIni.Mods.Contains(modName))
						dxLoaderIni.Mods.Add(modName);

					SplitTools.IniSerializer.Serialize(dxLoaderIni, modLoaderIniPath);
					break;

				case ("SA2PC"):
					SA2LoaderInfo sa2LoaderIni = SplitTools.IniSerializer.Deserialize<SA2LoaderInfo>(modLoaderIniPath);

					if (!sa2LoaderIni.Mods.Contains(modName))
						sa2LoaderIni.Mods.Add(modName);

					SplitTools.IniSerializer.Serialize(sa2LoaderIni, modLoaderIniPath);
					break;
			}
		}
		#endregion

		#region Form Functions
		private void btnOK_Click(object sender, EventArgs e)
		{
			string modName;
			switch (SAToolsHub.setGame)
			{
				case ("SADXPC"):
					SADXModInfo modInfoDX = SplitTools.IniSerializer.Deserialize<SADXModInfo>(Path.Combine(SAToolsHub.projectDirectory,"mod.ini"));
					modName = modInfoDX.Name;
					break;

				case ("SA2PC"):
					SA2ModInfo modInfoSA2 = SplitTools.IniSerializer.Deserialize<SA2ModInfo>(Path.Combine(SAToolsHub.projectDirectory, "mod.ini"));
					modName = modInfoSA2.Name;
					break;

				default:
					modName = "";
					break;
			}

			if (!Directory.Exists(Path.Combine(SAToolsHub.gameDirectory, "mods", modName)))
			{
				DialogResult buildCheckWindow = MessageBox.Show("No folder for this mod exists in " + SAToolsHub.setGame + "'s mod directory.\n\nWould you like to build a mod?", "Build Mod", MessageBoxButtons.YesNo);
				if (buildCheckWindow == DialogResult.Yes)
				{
					buildWindowDiag.Show();
				}
				if (buildCheckWindow == DialogResult.No)
				{
					if (gameLaunch == true)
						RunGame();
					else
						RunModManager();

					this.Close();
				}
			}
			else
			{
				UpdateModLoader(modName);
				if (gameLaunch == true)
					RunGame();
				else
					RunModManager();

				this.Close();
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void radModManager_CheckedChanged(object sender, EventArgs e)
		{
			gameLaunch = false;
		}

		private void radRunGame_CheckedChanged(object sender, EventArgs e)
		{
			gameLaunch = true;
		}

		private void gameOptions_Shown(object sender, EventArgs e)
		{
			radModManager.Checked = true;

			switch (SAToolsHub.setGame)
			{
				case ("SADXPC"):
					modManager = "SADXModManager.exe";
					gameEXE = "sonic.exe";
					radRunGame.Text = "Launch SADX";
					modLoaderINI = SAToolsHub.gameDirectory + "\\mods\\SADXModLoader.ini";
					break;
				case ("SA2PC"):
					modManager = "SA2ModManager.exe";
					gameEXE = "sonic2app.exe";
					radRunGame.Text = "Launch SA2PC";
					modLoaderINI = SAToolsHub.gameDirectory + "\\mods\\SA2ModLoader.ini";
					break;
			}
		}
		#endregion
	}
}
