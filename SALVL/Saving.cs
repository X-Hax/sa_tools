using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.UI;
using SplitTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAModel.SALVL
{
	public partial class MainForm
	{
		/// <summary>
		/// Saves changes made to the currently loaded stage.
		/// </summary>
		/// <param name="autoCloseDialog">Defines whether or not the progress dialog should close on completion.</param>
		private void SaveStage(bool autoCloseDialog)
		{
			if (!isStageLoaded)
				return;

			ProgressDialog progress = new ProgressDialog("Saving stage: " + levelName, 6, true, autoCloseDialog);
			progress.Show(this);
			Application.DoEvents();

			IniLevelData level = sadxlvlini.Levels[levelID];

			Directory.CreateDirectory(modSystemFolder);

			SA1LevelAct levelact = new SA1LevelAct(level.LevelID);

			progress.SetTaskAndStep("Saving:", "Geometry...");

			if (LevelData.geo != null)
				LevelData.geo.SaveToFile(level.LevelGeometry, LandTableFormat.SA1);

			progress.StepProgress();

			progress.Step = "Start positions...";
			Application.DoEvents();

			for (int i = 0; i < LevelData.StartPositions.Length; i++)
			{
				if (!File.Exists(sadxlvlini.Characters[LevelData.Characters[i]].StartPositions))
				{
					log.Add("Error saving start positions for character " + i.ToString());
					osd.AddMessage("Error saving start positions for character " + i.ToString(), 180);
					break;
				}
				Dictionary<SA1LevelAct, SA1StartPosInfo> posini =
					SA1StartPosList.Load(sadxlvlini.Characters[LevelData.Characters[i]].StartPositions);

				if (posini.ContainsKey(levelact))
					posini.Remove(levelact);

				if (LevelData.StartPositions[i].Position.X != 0 || LevelData.StartPositions[i].Position.Y != 0 ||
					LevelData.StartPositions[i].Position.Z != 0 || LevelData.StartPositions[i].Rotation.Y != 0)
				{
					posini.Add(levelact,
						new SA1StartPosInfo()
						{
							Position = LevelData.StartPositions[i].Position,
							YRotation = LevelData.StartPositions[i].Rotation.Y
						});
				}
				posini.Save(sadxlvlini.Characters[LevelData.Characters[i]].StartPositions);
			}

			progress.StepProgress();

			progress.Step = "Death zones...";
			Application.DoEvents();

			if (LevelData.DeathZones != null)
			{
				DeathZoneFlags[] dzini = new DeathZoneFlags[LevelData.DeathZones.Count];
				string path = Path.GetDirectoryName(level.DeathZones);
				for (int i = 0; i < LevelData.DeathZones.Count; i++)
					dzini[i] = LevelData.DeathZones[i].Save(path, i.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + ".sa1mdl");
				dzini.Save(level.DeathZones);
			}

			progress.StepProgress();

			#region Saving SET Items

			progress.Step = "SET items...";
			Application.DoEvents();

			if (!LevelData.SETItemsIsNull())
			{
				for (int i = 0; i < LevelData.SETChars.Length; i++)
				{
					string setstr = Path.Combine(modSystemFolder, "SET" + LevelData.SETName + LevelData.SETChars[i] + ".bin");

					// blank the set file
					if (File.Exists(setstr) || LevelData.GetSetItemCount(i) == 0)
					{
						byte[] emptyBytes = new byte[0x20];
						File.WriteAllBytes(setstr, emptyBytes);
					}

					List<byte> file = new List<byte>(LevelData.GetSetItemCount(i) * 0x20 + 0x20);
					file.AddRange(BitConverter.GetBytes(LevelData.GetSetItemCount(i)));
					file.Align(0x20);

					foreach (SETItem item in LevelData.SETItems(i))
						file.AddRange(item.GetBytes());

					File.WriteAllBytes(setstr, file.ToArray());
				}
			}

			progress.StepProgress();

			#endregion

			#region Saving CAM Items

			progress.Step = "CAM items...";
			Application.DoEvents();

			if (LevelData.CAMItems != null)
			{
				for (int i = 0; i < LevelData.CAMItems.Length; i++)
				{
					string camString = Path.Combine(modSystemFolder, "CAM" + LevelData.SETName + LevelData.SETChars[i] + ".bin");
					/*if (modpath != null)
						camString = Path.Combine(modpath, camString);*/

					// TODO: Handle this differently. File stream? If the user is using a symbolic link for example, we defeat the purpose by deleting it.
					if (File.Exists(camString))
						File.Delete(camString);

					if (LevelData.CAMItems[i].Count == 0)
						continue;

					List<byte> file = new List<byte>(LevelData.CAMItems[i].Count * 0x40 + 0x40); // setting up file size and header
					file.AddRange(BitConverter.GetBytes(LevelData.CAMItems[i].Count));
					file.Align(0x40);


					foreach (CAMItem item in LevelData.CAMItems[i]) // outputting individual components
						file.AddRange(item.GetBytes());

					File.WriteAllBytes(camString, file.ToArray());
				}
			}

			#endregion

			#region Saving Mission SET Items

			progress.Step = "Mission SET items...";
			Application.DoEvents();

			if (LevelData.MissionSETItems != null)
			{
				for (int i = 0; i < LevelData.MissionSETItems.Length; i++)
				{
					string setstr = Path.Combine(modSystemFolder, "SETMI" + level.LevelID + LevelData.SETChars[i] + ".bin");
					string prmstr = Path.Combine(modSystemFolder, "PRMMI" + level.LevelID + LevelData.SETChars[i] + ".bin");
					/*if (modpath != nullmodSystemPath
					{
						setstr = Path.Combine(modpath, setstr);
						prmstr = Path.Combine(modpath, prmstr);
					}*/

					// TODO: Consider simply blanking the SET file instead of deleting it.
					// Completely deleting it might be undesirable since Sonic's layout will be loaded
					// in place of the missing file. And where mods are concerned, you could have conflicts
					// with other mods if the file is deleted.
					if (File.Exists(setstr))
						File.Delete(setstr);
					if (File.Exists(prmstr))
						File.Delete(prmstr);
					if (LevelData.MissionSETItems[i].Count == 0)
						continue;

					List<byte> setfile = new List<byte>(LevelData.MissionSETItems[i].Count * 0x20 + 0x20);
					setfile.AddRange(BitConverter.GetBytes(LevelData.MissionSETItems[i].Count));
					setfile.Align(0x20);

					List<byte> prmfile = new List<byte>(LevelData.MissionSETItems[i].Count * 0xC + 0x20);
					prmfile.AddRange(new byte[] { 0, 0, 0x30, 0x56 });
					prmfile.Align(0x20);

					foreach (MissionSETItem item in LevelData.MissionSETItems[i])
					{
						setfile.AddRange(item.GetBytes());
						prmfile.AddRange(item.GetPRMBytes());
					}

					File.WriteAllBytes(setstr, setfile.ToArray());
					File.WriteAllBytes(prmstr, prmfile.ToArray());
				}
			}

			progress.StepProgress();
			#endregion

			#region Saving Splines
			progress.Step = "Splines...";
			Application.DoEvents();
			if (LevelData.LevelSplines != null)
			{
				string splineDirectory = Path.Combine(Path.Combine(modFolder, sadxlvlini.Paths),
							levelact.ToString());
				int s = 0;
				foreach (SplineData spline in LevelData.LevelSplines)
				{
					string path = Path.Combine(splineDirectory, string.Format("{0}.ini", s));
					spline.Save(path);
					s++;
				}
			}
			#endregion

			#region Saving Lights
			progress.Step = "Lights...";
			Application.DoEvents();
			if (stageLightList != null)
			{
				string stageLightPath = Path.Combine(modFolder, "Misc", "Stage Lights.ini");
				IniSerializer.Serialize(stageLightList, stageLightPath);
			}
			if (characterLightList != null)
			{
				string charLightPath = Path.Combine(modFolder, "Misc", "Character Lights.ini");
				IniSerializer.Serialize(characterLightList, charLightPath);
			}
			#endregion

			progress.StepProgress();
			progress.SetTaskAndStep("Save complete!");
			unsaved = false;
			Application.DoEvents();
		}

		private void SaveSETFile(bool bigendian)
		{
			using (SaveFileDialog a = new SaveFileDialog
			{
				DefaultExt = "bin",
				Filter = "SET files|SET*.bin",
			})
			{
				if (a.ShowDialog() == DialogResult.OK)
				{
					{
						SETItem.Save(LevelData.SETItems(LevelData.Character).ToList(), a.FileName, bigendian);
					}
				}
			}
		}

	}
}