using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Linq;

namespace SASave
{
	internal partial class MainForm : Form
	{
		private string[] args;

		public MainForm(string[] args)
		{
			InitializeComponent();
			this.args = args;
		}

		int slotnum = 0;
		SaveSlot[] slots = { new SaveSlot(), new SaveSlot(), new SaveSlot() };

		SaveSlot CurrentSlot
		{
			get { return slots[slotnum]; }
			set { slots[slotnum] = value; }
		}

		SaveData CurrentData
		{
			get { return CurrentSlot.Data; }
			set { CurrentSlot.Data = value; }
		}

		static readonly string[] levels = {
			"Hedgehog Hammer",
			"Emerald Coast",
			"Windy Valley",
			"Twinkle Park",
			"Speed Highway",
			"Red Mountain",
			"Sky Deck",
			"Lost World",
			"Ice Cap",
			"Casinopolis",
			"Final Egg",
			"Unused",
			"Hot Shelter",
			"Unused",
			"Unused",
			"Chaos 0",
			"Chaos 2",
			"Chaos 4",
			"Chaos 6",
			"Perfect Chaos",
			"Egg Hornet",
			"Egg Walker",
			"Egg Viper",
			"ZERO",
			"E-101 Beta",
			"E-101mkII",
			"Station Square",
			"Unused",
			"Unused",
			"Egg Carrier Outside",
			"Unused",
			"Unused",
			"Egg Carrier Inside",
			"Mystic Ruins",
			"The Past",
			"Twinkle Circuit",
			"Sky Chase Act 1",
			"Sky Chase Act 2",
			"Sand Hill",
			"Station Square Chao Garden",
			"Egg Carrier Chao Garden",
			"Mystic Ruins Chao Garden",
			"Chao Race" };

		static readonly List<Dictionary<string, int>> ActionStages = new List<Dictionary<string, int>>() {
			new Dictionary<string, int>() {
			{ "Emerald Coast", 0 },
			{ "Windy Valley", 1 },
			{ "Casinopolis", 8 },
			{ "Ice Cap", 7 },
			{ "Twinkle Park", 2 },
			{ "Speed Highway", 3 },
			{ "Red Mountain", 4 },
			{ "Sky Deck", 5 },
			{ "Lost World", 6 },
			{ "Final Egg", 9 }
			},
			new Dictionary<string, int>() {
			{ "Windy Valley", 10 },
			{ "Casinopolis", 14 },
			{ "Ice Cap", 13 },
			{ "Sky Deck", 12 },
			{ "Speed Highway",11 }
			},
			new Dictionary<string, int>() {
			{ "Speed Highway", 15 },
			{ "Casinopolis", 19 },
			{ "Red Mountain", 16 },
			{ "Lost World", 18 },
			{ "Sky Deck", 17 }
			},
			new Dictionary<string, int>() {
			{ "Twinkle Park", 20 },
			{ "Hot Shelter", 22 },
			{ "Final Egg", 21 }
			},
			new Dictionary<string, int>() {
			{ "Final Egg", 26 },
			{ "Emerald Coast", 23 },
			{ "Windy Valley", 24 },
			{ "Red Mountain", 25 },
			{ "Hot Shelter", 27 }
			},
			new Dictionary<string, int>() {
			{ "Twinkle Park", 29 },
			{ "Ice Cap", 30 },
			{ "Emerald Coast", 28 },
			{ "Hot Shelter", 31 }
			}};

		static readonly int[] AdventureEmblemLives = { 0, 0, 1, 2, 6, 3, 4, 5 };

		const int SA1Size = 0x4A0;
		const int SADXSize = 0x570;
		const int SA2010Size = 0x580;
		const int DCHeader = 0x480;
		const int GCHeader = 0x1480;
		const int X360Header = 4;
		const int DCSlot1 = DCHeader;
		const int DCSlot2 = DCSlot1 + SA1Size;
		const int DCSlot3 = DCSlot2 + SA1Size;
		static readonly int[] DCSlots = { DCSlot1, DCSlot2, DCSlot3 };

		ushort CalcSaveChecksum(byte[] file)
		{
			int j;
			ushort v3;
			int i;

			v3 = unchecked((ushort)-1);
			for (i = 4; (uint)i < file.Length; ++i)
			{
				v3 ^= file[i];
				for (j = 0; j < 8; ++j)
				{
					if ((v3 & 1) == 1)
						v3 = (ushort)(((int)v3 >> 1) ^ 0x8408);
					else
						v3 >>= 1;
				}
			}
			return (ushort)~v3;
		}

		private ushort CalcVMSCRC(byte[] file)
		{
			byte[] copy = file.CastClone();
			copy[0x46] = 0;
			copy[0x47] = 0;
			int i, c, n = 0;
			for (i = 0; i < copy.Length; i++)
			{
				n ^= (copy[i] << 8);
				for (c = 0; c < 8; c++)
					if ((n & 0x8000) == 0x8000)
						n = (n << 1) ^ 4129;
					else
						n = (n << 1);
			}
			return (ushort)n;
		}

		private void Byteswap(byte[] data)
		{
			for (int i = 0; i < 34; i++)
				Array.Reverse(data, i * 4, 4);
			for (int i = 0; i < 12; i++)
				Array.Reverse(data, 0xDC + (i * 2), 2);
			for (int i = 0; i < 32; i++)
				Array.Reverse(data, 0x104 + (i * 2), 2);
			for (int i = 0; i < 27; i++)
				Array.Reverse(data, 0x144 + (i * 4), 4);
			Array.Reverse(data, 0x25C, 2);
			for (int i = 0; i < 8; i++)
				for (int j = 1; j < 5; j++)
					Array.Reverse(data, 0x2E8 + (i * 0xC) + (j * 2), 2);
			if (data.Length > SA1Size)
			{
				for (int i = 0; i < 11; i++)
					Array.Reverse(data, 0x4DC + (i * 4), 4);
				for (int i = 0; i < 10; i++)
					Array.Reverse(data, 0x526 + (i * 2), 2);
				for (int i = 0; i < 6; i++)
					Array.Reverse(data, 0x53C + (i * 4), 4);
				Array.Reverse(data, 0x56C, 4);
			}
		}

		private Regions CheckSA1Region(byte[] file)
		{
			bool match = true;
			for (int i = 0x30; i < 0x10; i++)
				if (file[i] != Properties.Resources.DC_JP[i])
				{
					match = false;
					break;
				}
			if (match) return Regions.Japan;
			match = true;
			for (int i = 0x30; i < 0x10; i++)
				if (file[i] != Properties.Resources.DC_INT[i])
				{
					match = false;
					break;
				}
			if (match) return Regions.International;
			match = true;
			for (int i = 0x30; i < 0x10; i++)
				if (file[i] != Properties.Resources.DC_US[i])
				{
					match = false;
					break;
				}
			if (match) return Regions.US;
			return Regions.US;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			lastCharacter.SelectedIndex = 0;
			lastLevel.SelectedIndex = 0;
			adventure_character.SelectedIndex = 0;
			action_stage_character.SelectedIndex = 0;
			action_stage.SelectedIndex = 0;
			skychase1_character.SelectedIndex = 0;
			skychase2_character.SelectedIndex = 0;
			icecap_character.SelectedIndex = 0;
			sandhill_character.SelectedIndex = 0;
			circuit_character.SelectedIndex = 0;
			boss_character.SelectedIndex = 0;
			metal_level_select.DataSource = new List<KeyValuePair<string, int>>(ActionStages[0]);
			metal_level_select.SelectedIndex = 0;
			Dictionary<int, string> events;
			if (System.IO.File.Exists("Events.ini"))
				events = IniFile.Deserialize<Dictionary<int, string>>("Events.ini");
			else
				events = new Dictionary<int, string>();
			int i = 0;
			events_unused.BeginUpdate();
			for (int k = 0; k < 64; k++)
			{
				events_unused.Items.Add(events.ContainsKey(i) ? events[i] : "Unused");
				i++;
			}
			events_unused.EndUpdate();
			events_general.BeginUpdate();
			for (int k = 0; k < 64; k++)
			{
				events_general.Items.Add(events.ContainsKey(i) ? events[i] : "Unused", i == 65);
				i++;
			}
			events_general.EndUpdate();
			events_sonic.BeginUpdate();
			for (int k = 0; k < 64; k++)
			{
				events_sonic.Items.Add(events.ContainsKey(i) ? events[i] : "Unused");
				i++;
			}
			events_sonic.EndUpdate();
			events_tails.BeginUpdate();
			for (int k = 0; k < 64; k++)
			{
				events_tails.Items.Add(events.ContainsKey(i) ? events[i] : "Unused");
				i++;
			}
			events_tails.EndUpdate();
			events_knuckles.BeginUpdate();
			for (int k = 0; k < 64; k++)
			{
				events_knuckles.Items.Add(events.ContainsKey(i) ? events[i] : "Unused");
				i++;
			}
			events_knuckles.EndUpdate();
			events_amy.BeginUpdate();
			for (int k = 0; k < 64; k++)
			{
				events_amy.Items.Add(events.ContainsKey(i) ? events[i] : "Unused");
				i++;
			}
			events_amy.EndUpdate();
			events_gamma.BeginUpdate();
			for (int k = 0; k < 64; k++)
			{
				events_gamma.Items.Add(events.ContainsKey(i) ? events[i] : "Unused");
				i++;
			}
			events_gamma.EndUpdate();
			events_big.BeginUpdate();
			for (int k = 0; k < 64; k++)
			{
				events_big.Items.Add(events.ContainsKey(i) ? events[i] : "Unused");
				i++;
			}
			events_big.EndUpdate();
			Dictionary<int, string> npcs;
			if (System.IO.File.Exists("NPCs.ini"))
				npcs = IniFile.Deserialize<Dictionary<int, string>>("NPCs.ini");
			else
				npcs = new Dictionary<int, string>();
			this.npcs.BeginUpdate();
			for (i = 0; i < 512; i++)
				this.npcs.Items.Add(npcs.ContainsKey(i) ? npcs[i] : "Unknown");
			this.npcs.EndUpdate();
			i = 0;
			level_clear_table.SuspendLayout();
			foreach (string item in levels)
			{
				level_clear_table.Controls.Add(new Label() { Text = item, TextAlign = ContentAlignment.MiddleCenter, Anchor = AnchorStyles.None });
				NumericUpDown ctrl = new NumericUpDown() { Name = "levelclear_" + i.ToInvariantString(), Width = 45, Maximum = byte.MaxValue, Anchor = AnchorStyles.None };
				ctrl.ValueChanged += new EventHandler(levelclear_ValueChanged);
				level_clear_table.Controls.Add(ctrl);
				i++;
			}
			level_clear_table.ResumeLayout();
			levelclear_character.SelectedIndex = 0;
			MissionTable.SuspendLayout();
			for (i = 0; i < 60; i++)
			{
				string istr = i.ToInvariantString();
				MissionTable.Controls.Add(new Label() { Anchor = AnchorStyles.None, TextAlign = ContentAlignment.MiddleCenter, Text = istr }, 0, i + 1);
				CheckBox ctrl = new CheckBox() { Name = "mission_" + istr + "_unlocked", Anchor = AnchorStyles.None, AutoSize = true, Text = string.Empty };
				ctrl.CheckedChanged += new EventHandler(mission_unlocked_CheckedChanged);
				MissionTable.Controls.Add(ctrl, 1, i + 1);
				ctrl = new CheckBox() { Name = "mission_" + istr + "_active", Anchor = AnchorStyles.None, AutoSize = true, Text = string.Empty };
				ctrl.CheckedChanged += new EventHandler(mission_active_CheckedChanged);
				MissionTable.Controls.Add(ctrl, 2, i + 1);
				ctrl = new CheckBox() { Name = "mission_" + istr + "_complete", Anchor = AnchorStyles.None, AutoSize = true, Text = string.Empty };
				ctrl.CheckedChanged += new EventHandler(mission_complete_CheckedChanged);
				MissionTable.Controls.Add(ctrl, 3, i + 1);
			}
			MissionTable.ResumeLayout();
			if (args.Length > 0)
				ReadSaveFile(args[0], -1);
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private Control FindControl(string name)
		{
			Control[] ctrls = Controls.Find(name, true);
			if (ctrls.Length > 0)
				return ctrls[0];
			return null;
		}

		private void UpdateEmblems()
		{
			if (action_stage.SelectedIndex != 1)
			{
				level_emblem_a.Checked = CurrentData.Emblems[(int)action_stage.SelectedValue];
				level_emblem_b.Checked = CurrentData.Emblems[32 + (int)action_stage.SelectedValue];
				level_emblem_c.Checked = CurrentData.Emblems[64 + (int)action_stage.SelectedValue];
			}
			for (int i = 96; i < 111; i++)
				((EmblemControl)FindControl("emblem_" + i.ToInvariantString())).Checked = CurrentData.Emblems[i];
			if (adventure_character.SelectedIndex != -1)
				adventure_emblem.Checked = CurrentData.Emblems[AdventureEmblemLives[adventure_character.SelectedIndex]];
			for (int i = 118; i < 130; i++)
				((EmblemControl)FindControl("emblem_" + i.ToInvariantString())).Checked = CurrentData.Emblems[i];
		}

		private void UpdateEventFlags()
		{
			int e = 0;
			for (int i = 0; i < 64; i++)
				events_unused.SetItemChecked(i, CurrentData.EventFlags[e + i]);
			e += 64;
			for (int i = 0; i < 64; i++)
				events_general.SetItemChecked(i, CurrentData.EventFlags[e + i]);
			e += 64;
			for (int i = 0; i < 64; i++)
				events_sonic.SetItemChecked(i, CurrentData.EventFlags[e + i]);
			e += 64;
			for (int i = 0; i < 64; i++)
				events_tails.SetItemChecked(i, CurrentData.EventFlags[e + i]);
			e += 64;
			for (int i = 0; i < 64; i++)
				events_knuckles.SetItemChecked(i, CurrentData.EventFlags[e + i]);
			e += 64;
			for (int i = 0; i < 64; i++)
				events_amy.SetItemChecked(i, CurrentData.EventFlags[e + i]);
			e += 64;
			for (int i = 0; i < 64; i++)
				events_gamma.SetItemChecked(i, CurrentData.EventFlags[e + i]);
			e += 64;
			for (int i = 0; i < 64; i++)
				events_big.SetItemChecked(i, CurrentData.EventFlags[e + i]);
		}

		private void UpdateNPCFlags()
		{
			for (int i = 0; i < 512; i++)
				npcs.SetItemChecked(i, CurrentData.NPCFlags[i]);
		}

		private void UpdateMetalEmblems()
		{
			if (metal_level_select.SelectedIndex != 1)
			{
				metal_stage_emblem_a.Checked = CurrentData.Emblems[(int)action_stage.SelectedValue * 3];
				metal_stage_emblem_b.Checked = CurrentData.Emblems[(int)action_stage.SelectedValue * 3 + 1];
				metal_stage_emblem_c.Checked = CurrentData.Emblems[(int)action_stage.SelectedValue * 3 + 2];
			}
		}

		private void giveAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CurrentData.Emblems.SetAll(true);
			UpdateEmblems();
		}

		private void takeAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CurrentData.Emblems.SetAll(false);
			UpdateEmblems();
		}

		private void checkAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CurrentData.EventFlags.SetAll(true);
			UpdateEventFlags();
		}

		private void uncheckAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CurrentData.EventFlags.SetAll(false);
			UpdateEventFlags();
		}

		private void checkAllToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			CurrentData.NPCFlags.SetAll(true);
			UpdateNPCFlags();
		}

		private void uncheckAllToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			CurrentData.NPCFlags.SetAll(false);
			UpdateNPCFlags();
		}

		private void markAllUnlockedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < 60; i++)
				((CheckBox)FindControl("mission_" + i.ToInvariantString() + "_unlocked")).Checked = true;
		}

		private void markAllLockedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < 60; i++)
				((CheckBox)FindControl("mission_" + i.ToInvariantString() + "_unlocked")).Checked = false;
		}

		private void markAllCompletedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < 60; i++)
				((CheckBox)FindControl("mission_" + i.ToInvariantString() + "_complete")).Checked = true;
		}

		private void markAllUncompletedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < 60; i++)
				((CheckBox)FindControl("mission_" + i.ToInvariantString() + "_complete")).Checked = false;
		}

		private void giveAllToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			CurrentData.MetalEmblems.SetAll(true);
			UpdateMetalEmblems();
		}

		private void takeAllToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			CurrentData.MetalEmblems.SetAll(false);
			UpdateMetalEmblems();
		}

		private void LoadSaveData()
		{
			playTime.Frames = CurrentData.PlayTime;
			switch (CurrentData.MessageOption)
			{
				case MessageOptions.VoiceAndText:
					messagesVoiceAndText.Checked = true;
					break;
				case MessageOptions.VoiceOnly:
					messagesVoiceOnly.Checked = true;
					break;
			}
			switch (CurrentData.VoiceLanguage)
			{
				case VoiceLanguages.Default:
					voiceDefault.Checked = true;
					break;
				case VoiceLanguages.Japanese:
					voiceJapanese.Checked = true;
					break;
				case VoiceLanguages.English:
					voiceEnglish.Checked = true;
					break;
			}
			switch (CurrentData.TextLanguage)
			{
				case TextLanguages.Default:
					textDefault.Checked = true;
					break;
				case TextLanguages.Japanese:
					textJapanese.Checked = true;
					break;
				case TextLanguages.English:
					textEnglish.Checked = true;
					break;
				case TextLanguages.French:
					textFrench.Checked = true;
					break;
				case TextLanguages.Spanish:
					textSpanish.Checked = true;
					break;
				case TextLanguages.German:
					textGerman.Checked = true;
					break;
			}
			rumble.Checked = CurrentData.Rumble;
			lastCharacter.SelectedIndex = CurrentData.LastCharacter;
			if (CurrentData.LastLevel == 100)
				lastLevel.SelectedIndex = 0;
			else
				lastLevel.SelectedIndex = CurrentData.LastLevel + 1;
			adventure_character.SelectedIndex = -1;
			action_stage_character.SelectedIndex = -1;
			skychase1_character.SelectedIndex = -1;
			skychase2_character.SelectedIndex = -1;
			icecap_character.SelectedIndex = -1;
			sandhill_character.SelectedIndex = -1;
			hedgehoghammer_score_1.Value = CurrentData.HedgehogHammerScores[0];
			hedgehoghammer_score_2.Value = CurrentData.HedgehogHammerScores[1];
			hedgehoghammer_score_3.Value = CurrentData.HedgehogHammerScores[2];
			circuit_character.SelectedIndex = -1;
			boss_character.SelectedIndex = -1;
			UpdateEmblems();
			UpdateEventFlags();
			UpdateNPCFlags();
			levelclear_character.SelectedIndex = -1;
			blackmarket_rings.Value = CurrentData.BlackMarketRings;
			for (int i = 0; i < 60; i++)
			{
				((CheckBox)FindControl("mission_" + i.ToInvariantString() + "_unlocked")).Checked = CurrentData.Missions[i].Unlocked;
				((CheckBox)FindControl("mission_" + i.ToInvariantString() + "_active")).Checked = CurrentData.Missions[i].Active;
				((CheckBox)FindControl("mission_" + i.ToInvariantString() + "_complete")).Checked = CurrentData.Missions[i].Complete;
			}
			metal_level_select.SelectedIndex = -1;
			icecap_metal_sonic_score_1.Value = CurrentData.MetalIceCapScores[0];
			icecap_metal_sonic_score_2.Value = CurrentData.MetalIceCapScores[1];
			icecap_metal_sonic_score_3.Value = CurrentData.MetalIceCapScores[2];
			sandhill_metal_sonic_score_1.Value = CurrentData.MetalSandHillScores[0];
			sandhill_metal_sonic_score_2.Value = CurrentData.MetalSandHillScores[1];
			sandhill_metal_sonic_score_3.Value = CurrentData.MetalSandHillScores[2];
			circuit_metal_time_1.CircuitTime = CurrentData.MetalTwinkleCircuitTimes.BestTimes[0];
			circuit_metal_time_2.CircuitTime = CurrentData.MetalTwinkleCircuitTimes.BestTimes[1];
			circuit_metal_time_3.CircuitTime = CurrentData.MetalTwinkleCircuitTimes.BestTimes[2];
			circuit_metal_lap_1.CircuitTime = CurrentData.MetalTwinkleCircuitTimes.Lap1Time;
			circuit_metal_lap_2.CircuitTime = CurrentData.MetalTwinkleCircuitTimes.Lap2Time;
			boss_metal_sonic_time_1.LevelTime = CurrentData.MetalBossTimes[0];
			boss_metal_sonic_time_2.LevelTime = CurrentData.MetalBossTimes[1];
			boss_metal_sonic_time_3.LevelTime = CurrentData.MetalBossTimes[2];
			saveToolStripMenuItem.Enabled = CurrentSlot.Filename != null;
		}

		private void newSlot1ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			slots[0] = new SaveSlot();
			if (slotnum == 0)
				LoadSaveData();
		}

		private void newSlot2ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			slots[1] = new SaveSlot();
			if (slotnum == 1)
				LoadSaveData();
		}

		private void newSlot3ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			slots[2] = new SaveSlot();
			if (slotnum == 2)
				LoadSaveData();
		}

		private void newAllSlotsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < 3; i++)
				slots[i] = new SaveSlot();
			LoadSaveData();
		}

		private static Regex PCFilename = new Regex(@"SonicDX([0-9][0-9])\.snc", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
		private static Regex GCFilename = new Regex(@".*SONICADVENTURE_DX_PLAYRECORD_[123]\.gci", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

		private void OpenSaveFile(int slot)
		{
			using (OpenFileDialog fd = new OpenFileDialog()
			{
				DefaultExt = "snc",
				Filter = "Save Files|*.vms;*.gci;*.snc;*.bin|All Files|*.*"
			})
				if (fd.ShowDialog(this) == DialogResult.OK)
					ReadSaveFile(fd.FileName, slot);
		}

		private void ReadSaveFile(string filename, int slot)
		{
			switch (Path.GetExtension(filename).ToLowerInvariant())
			{
				case ".vms":
					if (slot == -1)
					{
						byte[] vms = File.ReadAllBytes(filename);
						for (int i = 0; i < 3; i++)
						{
							slots[i] = new SaveSlot() { System = Systems.Dreamcast };
							slots[i].Filename = filename;
							slots[i].Region = CheckSA1Region(vms);
							byte[] sl = new byte[SA1Size];
							Array.Copy(vms, DCSlots[i], sl, 0, SA1Size);
							int calc = CalcSaveChecksum(sl);
							int stored = BitConverter.ToInt32(sl, 0);
							byte[] data = Properties.Resources.PC.CastClone();
							Array.Copy(sl, 0, data, 0, SA1Size);
							slots[i].Data = new SaveData(data);
							if (calc != stored)
								MessageBox.Show(this, "Checksum in file \"" + filename + "\" slot " + (i + 1).ToInvariantString() + " incorrect!\nCalculated value: 0x" + calc.ToString("X4") + "\nStored value: 0x" + stored.ToString("X4"));
						}
						LoadSaveData();
					}
					else
						using (SA1SlotDialog sa1 = new SA1SlotDialog())
							if (sa1.ShowDialog(this) == DialogResult.OK)
							{
								slots[slot] = new SaveSlot() { System = Systems.Dreamcast };
								slots[slot].Filename = filename;
								byte[] vms = File.ReadAllBytes(filename);
								slots[slot].Region = CheckSA1Region(vms);
								byte[] sl = new byte[SA1Size];
								Array.Copy(vms, DCSlots[sa1.Selection], sl, 0, SA1Size);
								int calc = CalcSaveChecksum(sl);
								int stored = BitConverter.ToInt32(sl, 0);
								byte[] data = Properties.Resources.PC.CastClone();
								Array.Copy(sl, 0, data, 0, SA1Size);
								slots[slot].Data = new SaveData(data);
								if (calc != stored)
									MessageBox.Show(this, "Checksum in file \"" + filename + "\" slot " + (slot + 1).ToInvariantString() + " incorrect!\nCalculated value: 0x" + calc.ToString("X4") + "\nStored value: 0x" + stored.ToString("X4"));
								if (slotnum == slot)
									LoadSaveData();
							}
					break;
				case ".gci":
					if (slot == -1)
					{
						if (GCFilename.IsMatch(Path.GetFileName(filename)))
						{
							string file = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename).TrimEnd('1', '2', '3') + "{0}" + Path.GetExtension(filename));
							for (int i = 0; i < 3; i++)
							{
								string fn = string.Format(CultureInfo.InvariantCulture, file, i + 1);
								if (File.Exists(fn))
									LoadGCNSlot(i, fn);
							}
							LoadSaveData();
						}
						else
						{
							LoadGCNSlot(0, filename);
							if (slotnum == 0)
								LoadSaveData();
						}
					}
					else
					{
						LoadGCNSlot(slot, filename);
						if (slotnum == slot)
							LoadSaveData();
					}
					break;
				case ".snc":
					if (slot == -1)
					{
						if (PCFilename.IsMatch(Path.GetFileName(filename)))
						{
							string file = Path.Combine(Path.GetDirectoryName(filename), "SonicDX{0:00}.snc");
							Match match = PCFilename.Match(Path.GetFileName(filename));
							byte num = byte.Parse(match.Groups[1].Value, NumberStyles.None, NumberFormatInfo.InvariantInfo);
							for (int i = 0; i < 3; i++)
							{
								string fn = string.Format(CultureInfo.InvariantCulture, file, num + i);
								if (File.Exists(fn))
									LoadPCSlot(i, fn);
							}
							LoadSaveData();
						}
						else
						{
							LoadPCSlot(0, filename);
							if (slotnum == 0)
								LoadSaveData();
						}
					}
					else
					{
						LoadPCSlot(slot, filename);
						if (slotnum == slot)
							LoadSaveData();
					}
					break;
				case ".bin":
					if (slot == -1) slot = 0;
					slots[slot] = new SaveSlot() { System = Systems.PC };
					slots[slot].Filename = filename;
					byte[] bin = File.ReadAllBytes(filename);
					byte[] datab = new byte[SADXSize + 16];
					Array.Copy(bin, X360Header, datab, 0, SADXSize + 16);
					int calcb = CalcSaveChecksum(datab);
					Byteswap(datab);
					int storedb = BitConverter.ToInt32(datab, 0);
					slots[slot].Data = new SaveData(datab);
					if (calcb != storedb)
						MessageBox.Show(this, "Checksum in file \"" + filename + "\" incorrect!\nCalculated value: 0x" + calcb.ToString("X4") + "\nStored value: 0x" + storedb.ToString("X4"));
					if (slotnum == slot)
						LoadSaveData();
					break;
				default:
					throw new ApplicationException("Unknown save type " + Path.GetExtension(filename) + "!");
			}
		}

		private void LoadPCSlot(int i, string fn)
		{
			slots[i] = new SaveSlot();
			slots[i].Filename = fn;
			byte[] data = File.ReadAllBytes(fn);
			int calc = CalcSaveChecksum(data);
			int stored = BitConverter.ToInt32(data, 0);
			slots[i].Data = new SaveData(data);
			if (calc != stored)
				MessageBox.Show(this, "Checksum in file \"" + fn + "\" incorrect!\nCalculated value: 0x" + calc.ToString("X4") + "\nStored value: 0x" + stored.ToString("X4"));
		}

		private void LoadGCNSlot(int i, string fn)
		{
			slots[i] = new SaveSlot() { System = Systems.GameCube };
			slots[i].Filename = fn;
			byte[] gci = File.ReadAllBytes(fn);
			if (System.Text.Encoding.ASCII.GetString(gci, 0, 6) == "RELSAB")
			{
				slots[i].System = Systems.GameCubePreview;
				slots[i].Region = Regions.Japan;
				byte[] sl = new byte[SA1Size];
				Array.Copy(gci, GCHeader, sl, 0, SA1Size);
				int calc = CalcSaveChecksum(sl);
				Byteswap(sl);
				int stored = BitConverter.ToInt32(sl, 0);
				byte[] data = Properties.Resources.PC.CastClone();
				Array.Copy(sl, 0, data, 0, SA1Size);
				slots[i].Data = new SaveData(data);
				if (calc != stored)
					MessageBox.Show(this, "Checksum in file \"" + fn + "\" incorrect!\nCalculated value: 0x" + calc.ToString("X4") + "\nStored value: 0x" + stored.ToString("X4"));
			}
			else
			{
				switch (char.ToLowerInvariant((char)gci[3]))
				{
					case 'p':
						slots[i].Region = Regions.Europe;
						break;
					case 'j':
						slots[i].Region = Regions.Japan;
						break;
				}
				byte[] data = new byte[SADXSize];
				Array.Copy(gci, GCHeader, data, 0, SADXSize);
				int calc = CalcSaveChecksum(data);
				Byteswap(data);
				int stored = BitConverter.ToInt32(data, 0);
				slots[i].Data = new SaveData(data);
				if (calc != stored)
					MessageBox.Show(this, "Checksum in file \"" + fn + "\" incorrect!\nCalculated value: 0x" + calc.ToString("X4") + "\nStored value: 0x" + stored.ToString("X4"));
			}
		}

		private void openSlot1ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenSaveFile(0);
		}

		private void openSlot2ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenSaveFile(1);
		}

		private void openSlot3ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenSaveFile(2);
		}

		private void openAllSlotsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenSaveFile(-1);
		}

		private void slotToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			int slot = slotToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem);
			if (slot == slotnum) return;
			foreach (ToolStripItem item in slotToolStripMenuItem.DropDownItems)
				((ToolStripMenuItem)item).Checked = false;
			((ToolStripMenuItem)e.ClickedItem).Checked = true;
			slotnum = slot;
			LoadSaveData();
		}

		private static readonly DateTime GCDate = new DateTime(2000, 1, 1);
		private void WriteSaveFile()
		{
			switch (CurrentSlot.System)
			{
				case Systems.Dreamcast:
					byte[] vms = Properties.Resources.DC_US.CastClone();
					switch (CurrentSlot.Region)
					{
						case Regions.Japan:
							vms = Properties.Resources.DC_JP.CastClone();
							break;
						case Regions.International:
							vms = Properties.Resources.DC_INT.CastClone();
							break;
					}
					for (int i = 0; i < 3; i++)
					{
						byte[] dcsave = slots[i].Data.GetBytes();
						Array.Resize(ref dcsave, SA1Size);
						BitConverter.GetBytes((int)CalcSaveChecksum(dcsave)).CopyTo(dcsave, 0);
						dcsave.CopyTo(vms, DCSlots[i]);
					}
					BitConverter.GetBytes(CalcVMSCRC(vms)).CopyTo(vms, 0x46);
					File.WriteAllBytes(CurrentSlot.Filename, vms);
					break;
				case Systems.GameCube:
					byte[] gcsave = CurrentData.GetBytes();
					Byteswap(gcsave);
					byte[] check = BitConverter.GetBytes((int)CalcSaveChecksum(gcsave));
					Array.Reverse(check);
					check.CopyTo(gcsave, 0);
					byte[] gci = Properties.Resources.GC.CastClone();
					gcsave.CopyTo(gci, GCHeader);
					switch (CurrentSlot.Region)
					{
						case Regions.Japan:
							gci[3] = (byte)'J';
							break;
						case Regions.Europe:
							gci[3] = (byte)'P';
							break;
					}
					gci[0x25] = gci[0x59] = (byte)(0x31 + slotnum);
					int emblemcnt = CurrentData.Emblems.Cast<bool>().Count((val) => val);
					System.Text.Encoding.ASCII.GetBytes(emblemcnt.ToInvariantString().PadLeft(3)).CopyTo(gci, 0x72);
					check = BitConverter.GetBytes((uint)(DateTime.Now - GCDate).TotalSeconds);
					Array.Reverse(check);
					check.CopyTo(gci, 0x28);
					File.WriteAllBytes(CurrentSlot.Filename, gci);
					break;
				case Systems.GameCubePreview:
					gcsave = CurrentData.GetBytes();
					Byteswap(gcsave);
					Array.Resize(ref gcsave, SA1Size);
					check = BitConverter.GetBytes((int)CalcSaveChecksum(gcsave));
					Array.Reverse(check);
					check.CopyTo(gcsave, 0);
					gci = Properties.Resources.GCPreview.CastClone();
					gcsave.CopyTo(gci, GCHeader);
					gci[0x25] = (byte)(0x31 + slotnum);
					emblemcnt = CurrentData.Emblems.Cast<bool>().Count((val) => val);
					System.Text.Encoding.ASCII.GetBytes(emblemcnt.ToInvariantString().PadLeft(3)).CopyTo(gci, 0x70);
					check = BitConverter.GetBytes((uint)(DateTime.Now - GCDate).TotalSeconds);
					Array.Reverse(check);
					check.CopyTo(gci, 0x28);
					File.WriteAllBytes(CurrentSlot.Filename, gci);
					break;
				case Systems.PC:
					byte[] pcsave = CurrentData.GetBytes();
					BitConverter.GetBytes((int)CalcSaveChecksum(pcsave)).CopyTo(pcsave, 0);
					File.WriteAllBytes(CurrentSlot.Filename, pcsave);
					break;
			}
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			WriteSaveFile();
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog fd = new SaveFileDialog()
			{
				DefaultExt = "snc",
				Filter = "Save Files|*.vms;*.gci;*.snc|All Files|*.*"
			})
				if (fd.ShowDialog(this) == DialogResult.OK)
				{
					Systems system = Systems.PC;
					switch (Path.GetExtension(fd.FileName).ToLowerInvariant())
					{
						case ".vms":
							system = Systems.Dreamcast;
							break;
						case ".gci":
							system = Systems.GameCube;
							break;
						case ".snc":
							system = Systems.PC;
							break;
					}
					using (SaveFormatDialog fmt = new SaveFormatDialog(system))
						if (fmt.ShowDialog(this) == DialogResult.OK)
						{
							CurrentSlot.System = fmt.SelectedSystem;
							CurrentSlot.Region = fmt.SelectedRegion;
							CurrentSlot.Filename = fd.FileName;
							WriteSaveFile();
						}
				}
		}

		private void playTime_ValueChanged(object sender, EventArgs e)
		{
			CurrentData.PlayTime = playTime.Frames;
		}

		private void messages_CheckedChanged(object sender, EventArgs e)
		{
			if (messagesVoiceOnly.Checked)
				CurrentData.MessageOption = MessageOptions.VoiceOnly;
			else if (messagesVoiceAndText.Checked)
				CurrentData.MessageOption = MessageOptions.VoiceAndText;
		}

		private void voice_CheckedChanged(object sender, EventArgs e)
		{
			if (voiceDefault.Checked)
				CurrentData.VoiceLanguage = VoiceLanguages.Default;
			else if (voiceJapanese.Checked)
				CurrentData.VoiceLanguage = VoiceLanguages.Japanese;
			else if (voiceEnglish.Checked)
				CurrentData.VoiceLanguage = VoiceLanguages.English;
		}

		private void text_CheckedChanged(object sender, EventArgs e)
		{
			if (textDefault.Checked)
				CurrentData.TextLanguage = TextLanguages.Default;
			else if (textJapanese.Checked)
				CurrentData.TextLanguage = TextLanguages.Japanese;
			else if (textEnglish.Checked)
				CurrentData.TextLanguage = TextLanguages.English;
			else if (textFrench.Checked)
				CurrentData.TextLanguage = TextLanguages.French;
			else if (textSpanish.Checked)
				CurrentData.TextLanguage = TextLanguages.Spanish;
			else if (textGerman.Checked)
				CurrentData.TextLanguage = TextLanguages.German;
		}

		private void rumble_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Rumble = rumble.Checked;
		}

		private void lastCharacter_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lastCharacter.SelectedIndex == -1) return;
			CurrentData.LastCharacter = (byte)lastCharacter.SelectedIndex;
		}

		private void lastLevel_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lastLevel.SelectedIndex == -1) return;
			if (lastLevel.SelectedIndex == 0)
				CurrentData.LastLevel = 100;
			else
				CurrentData.LastLevel = (short)(lastLevel.SelectedIndex - 1);
		}

		private void adventure_character_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (adventure_character.SelectedIndex == -1)
			{
				adventure_character.SelectedIndex = 0;
				return;
			}
			int index = adventure_character.SelectedIndex;
			switch (index)
			{
				case 2:
					lifeIcon.Image = Properties.Resources.Tails;
					lives.Value = CurrentData.Lives[1];
					adventure_emblem.Checked = CurrentData.Emblems[112];
					break;
				case 3:
					lifeIcon.Image = Properties.Resources.Knuckles;
					lives.Value = CurrentData.Lives[2];
					adventure_emblem.Checked = CurrentData.Emblems[113];
					break;
				case 4:
					lifeIcon.Image = Properties.Resources.Super_Sonic;
					lives.Value = CurrentData.Lives[6];
					adventure_emblem.Checked = CurrentData.Emblems[117];
					break;
				case 5:
					lifeIcon.Image = Properties.Resources.Amy;
					lives.Value = CurrentData.Lives[3];
					adventure_emblem.Checked = CurrentData.Emblems[114];
					break;
				case 6:
					lifeIcon.Image = Properties.Resources.Gamma;
					lives.Value = CurrentData.Lives[4];
					adventure_emblem.Checked = CurrentData.Emblems[115];
					break;
				case 7:
					lifeIcon.Image = Properties.Resources.Big;
					lives.Value = CurrentData.Lives[5];
					adventure_emblem.Checked = CurrentData.Emblems[116];
					break;
				default:
					lifeIcon.Image = Properties.Resources.Sonic;
					lives.Value = CurrentData.Lives[0];
					adventure_emblem.Checked = CurrentData.Emblems[111];
					break;
			}
			adventure_tod.Value = CurrentData.AdventureModeData[index].TimeOfDay;
			adventure_unk1.Value = CurrentData.AdventureModeData[index].Unknown1;
			adventure_unk2.Value = CurrentData.AdventureModeData[index].Unknown2;
			adventure_entrance.Value = CurrentData.AdventureModeData[index].Entrance;
			adventure_levelact.LevelAct = CurrentData.AdventureModeData[index].LevelAct;
			adventure_unk3.Value = CurrentData.AdventureModeData[index].Unknown3;
		}

		private void lives_ValueChanged(object sender, EventArgs e)
		{
			if (adventure_character.SelectedIndex == -1) return;
			switch (adventure_character.SelectedIndex)
			{
				case 2:
					CurrentData.Lives[1] = (sbyte)lives.Value;
					break;
				case 3:
					CurrentData.Lives[2] = (sbyte)lives.Value;
					break;
				case 4:
					CurrentData.Lives[6] = (sbyte)lives.Value;
					break;
				case 5:
					CurrentData.Lives[3] = (sbyte)lives.Value;
					break;
				case 6:
					CurrentData.Lives[4] = (sbyte)lives.Value;
					break;
				case 7:
					CurrentData.Lives[5] = (sbyte)lives.Value;
					break;
				default:
					CurrentData.Lives[0] = (sbyte)lives.Value;
					break;
			}
		}

		private void adventure_emblem_CheckedChanged(object sender, EventArgs e)
		{
			if (adventure_character.SelectedIndex == -1) return;
			switch (adventure_character.SelectedIndex)
			{
				case 2:
					CurrentData.Emblems[112] = adventure_emblem.Checked;
					break;
				case 3:
					CurrentData.Emblems[113] = adventure_emblem.Checked;
					break;
				case 4:
					CurrentData.Emblems[117] = adventure_emblem.Checked;
					break;
				case 5:
					CurrentData.Emblems[114] = adventure_emblem.Checked;
					break;
				case 6:
					CurrentData.Emblems[115] = adventure_emblem.Checked;
					break;
				case 7:
					CurrentData.Emblems[116] = adventure_emblem.Checked;
					break;
				default:
					CurrentData.Emblems[111] = adventure_emblem.Checked;
					break;
			}
		}

		private void adventure_tod_ValueChanged(object sender, EventArgs e)
		{
			if (adventure_character.SelectedIndex == -1) return;
			CurrentData.AdventureModeData[adventure_character.SelectedIndex].TimeOfDay = adventure_tod.Value;
		}

		private void adventure_unk1_ValueChanged(object sender, EventArgs e)
		{
			if (adventure_character.SelectedIndex == -1) return;
			CurrentData.AdventureModeData[adventure_character.SelectedIndex].Unknown1 = (short)adventure_unk1.Value;
		}

		private void adventure_unk2_ValueChanged(object sender, EventArgs e)
		{
			if (adventure_character.SelectedIndex == -1) return;
			CurrentData.AdventureModeData[adventure_character.SelectedIndex].Unknown2 = (short)adventure_unk2.Value;
		}

		private void adventure_entrance_ValueChanged(object sender, EventArgs e)
		{
			if (adventure_character.SelectedIndex == -1) return;
			CurrentData.AdventureModeData[adventure_character.SelectedIndex].Entrance = (short)adventure_entrance.Value;
		}

		private void adventure_levelact_ValueChanged(object sender, EventArgs e)
		{
			if (adventure_character.SelectedIndex == -1) return;
			CurrentData.AdventureModeData[adventure_character.SelectedIndex].LevelAct = adventure_levelact.LevelAct;
		}

		private void adventure_unk3_ValueChanged(object sender, EventArgs e)
		{
			if (adventure_character.SelectedIndex == -1) return;
			CurrentData.AdventureModeData[adventure_character.SelectedIndex].Unknown3 = (short)adventure_unk3.Value;
		}

		private void action_stage_character_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (action_stage_character.SelectedIndex == -1)
			{
				action_stage.DataSource = null;
				action_stage_character.SelectedIndex = 0;
				return;
			}
			action_stage.DisplayMember = "Key";
			action_stage.ValueMember = "Value";
			action_stage.DataSource = new List<KeyValuePair<string, int>>(ActionStages[action_stage_character.SelectedIndex]);
		}

		private void action_stage_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (action_stage.SelectedIndex == -1)
			{
				if (action_stage.DataSource != null)
					action_stage.SelectedIndex = 0;
				return;
			}
			int stagenum = (int)action_stage.SelectedValue;
			level_emblem_a.Checked = CurrentData.Emblems[stagenum];
			level_emblem_b.Checked = CurrentData.Emblems[32 + stagenum];
			level_emblem_c.Checked = CurrentData.Emblems[64 + stagenum];
			level_score.Value = CurrentData.LevelScores[stagenum];
			if (action_stage_character.SelectedIndex == 5)
			{
				level_timeweight_label.Text = "Best Weights:";
				level_timeweight.Mode = WeightControlModes.Weight;
				level_timeweight.Weights = CurrentData.LevelWeights[stagenum - 28];
			}
			else
			{
				level_timeweight_label.Text = "Best Time:";
				level_timeweight.Mode = WeightControlModes.Time;
				level_timeweight.Time = CurrentData.LevelTimes[stagenum];
			}
			level_rings.Value = CurrentData.LevelRings[stagenum];
		}

		private void level_emblem_a_CheckedChanged(object sender, EventArgs e)
		{
			if (action_stage.SelectedIndex == -1) return;
			CurrentData.Emblems[(int)action_stage.SelectedValue] = level_emblem_a.Checked;
		}

		private void level_emblem_b_CheckedChanged(object sender, EventArgs e)
		{
			if (action_stage.SelectedIndex == -1) return;
			CurrentData.Emblems[32 + (int)action_stage.SelectedValue] = level_emblem_b.Checked;
		}

		private void level_emblem_c_CheckedChanged(object sender, EventArgs e)
		{
			if (action_stage.SelectedIndex == -1) return;
			CurrentData.Emblems[64 + (int)action_stage.SelectedValue] = level_emblem_c.Checked;
		}

		private void level_score_ValueChanged(object sender, EventArgs e)
		{
			if (action_stage.SelectedIndex == -1) return;
			CurrentData.LevelScores[(int)action_stage.SelectedValue] = (int)level_score.Value;
		}

		private void level_timeweight_ValueChanged(object sender, EventArgs e)
		{
			if (action_stage.SelectedIndex == -1) return;
			switch (level_timeweight.Mode)
			{
				case WeightControlModes.Time:
					CurrentData.LevelTimes[(int)action_stage.SelectedValue] = level_timeweight.Time;
					break;
				case WeightControlModes.Weight:
					CurrentData.LevelWeights[(int)action_stage.SelectedValue - 28] = level_timeweight.Weights;
					break;
			}
		}

		private void level_rings_ValueChanged(object sender, EventArgs e)
		{
			if (action_stage.SelectedIndex == -1) return;
			CurrentData.LevelRings[(int)action_stage.SelectedValue] = (short)level_rings.Value;
		}

		private void skychase1_character_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (skychase1_character.SelectedIndex == -1)
			{
				skychase1_character.SelectedIndex = 0;
				return;
			}
			int index = skychase1_character.SelectedIndex;
			skychase1_score_1.Value = CurrentData.SkyChase1Scores[index][0];
			skychase1_score_2.Value = CurrentData.SkyChase1Scores[index][1];
			skychase1_score_3.Value = CurrentData.SkyChase1Scores[index][2];
		}

		private void skychase1_score_1_ValueChanged(object sender, EventArgs e)
		{
			if (skychase1_character.SelectedIndex == -1) return;
			CurrentData.SkyChase1Scores[skychase1_character.SelectedIndex][0] = (int)skychase1_score_1.Value;
		}

		private void skychase1_score_2_ValueChanged(object sender, EventArgs e)
		{
			if (skychase1_character.SelectedIndex == -1) return;
			CurrentData.SkyChase1Scores[skychase1_character.SelectedIndex][1] = (int)skychase1_score_2.Value;
		}

		private void skychase1_score_3_ValueChanged(object sender, EventArgs e)
		{
			if (skychase1_character.SelectedIndex == -1) return;
			CurrentData.SkyChase1Scores[skychase1_character.SelectedIndex][2] = (int)skychase1_score_3.Value;
		}

		private void emblem_102_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Emblems[102] = emblem_102.Checked;
		}

		private void emblem_97_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Emblems[97] = emblem_97.Checked;
		}

		private void skychase2_character_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (skychase2_character.SelectedIndex == -1)
			{
				skychase2_character.SelectedIndex = 0;
				return;
			}
			int index = skychase2_character.SelectedIndex;
			skychase2_score_1.Value = CurrentData.SkyChase2Scores[index][0];
			skychase2_score_2.Value = CurrentData.SkyChase2Scores[index][1];
			skychase2_score_3.Value = CurrentData.SkyChase2Scores[index][2];
		}

		private void skychase2_score_1_ValueChanged(object sender, EventArgs e)
		{
			if (skychase2_character.SelectedIndex == -1) return;
			CurrentData.SkyChase2Scores[skychase2_character.SelectedIndex][0] = (int)skychase2_score_1.Value;
		}

		private void skychase2_score_2_ValueChanged(object sender, EventArgs e)
		{
			if (skychase2_character.SelectedIndex == -1) return;
			CurrentData.SkyChase2Scores[skychase2_character.SelectedIndex][1] = (int)skychase2_score_2.Value;
		}

		private void skychase2_score_3_ValueChanged(object sender, EventArgs e)
		{
			if (skychase2_character.SelectedIndex == -1) return;
			CurrentData.SkyChase2Scores[skychase2_character.SelectedIndex][2] = (int)skychase2_score_3.Value;
		}

		private void emblem_103_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Emblems[103] = emblem_103.Checked;
		}

		private void emblem_98_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Emblems[98] = emblem_98.Checked;
		}

		private void icecap_character_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (icecap_character.SelectedIndex == -1)
			{
				icecap_character.SelectedIndex = 0;
				return;
			}
			int index = icecap_character.SelectedIndex;
			icecap_score_1.Value = CurrentData.IceCapScores[index][0];
			icecap_score_2.Value = CurrentData.IceCapScores[index][1];
			icecap_score_3.Value = CurrentData.IceCapScores[index][2];
		}

		private void icecap_score_1_ValueChanged(object sender, EventArgs e)
		{
			if (icecap_character.SelectedIndex == -1) return;
			CurrentData.IceCapScores[icecap_character.SelectedIndex][0] = (int)icecap_score_1.Value;
		}

		private void icecap_score_2_ValueChanged(object sender, EventArgs e)
		{
			if (icecap_character.SelectedIndex == -1) return;
			CurrentData.IceCapScores[icecap_character.SelectedIndex][1] = (int)icecap_score_2.Value;
		}

		private void icecap_score_3_ValueChanged(object sender, EventArgs e)
		{
			if (icecap_character.SelectedIndex == -1) return;
			CurrentData.IceCapScores[icecap_character.SelectedIndex][2] = (int)icecap_score_3.Value;
		}

		private void sandhill_character_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (sandhill_character.SelectedIndex == -1)
			{
				sandhill_character.SelectedIndex = 0;
				return;
			}
			int index = sandhill_character.SelectedIndex;
			sandhill_score_1.Value = CurrentData.SandHillScores[index][0];
			sandhill_score_2.Value = CurrentData.SandHillScores[index][1];
			sandhill_score_3.Value = CurrentData.SandHillScores[index][2];
		}

		private void sandhill_score_1_ValueChanged(object sender, EventArgs e)
		{
			if (sandhill_character.SelectedIndex == -1) return;
			CurrentData.SandHillScores[sandhill_character.SelectedIndex][0] = (int)sandhill_score_1.Value;
		}

		private void sandhill_score_2_ValueChanged(object sender, EventArgs e)
		{
			if (sandhill_character.SelectedIndex == -1) return;
			CurrentData.SandHillScores[sandhill_character.SelectedIndex][1] = (int)sandhill_score_2.Value;
		}

		private void sandhill_score_3_ValueChanged(object sender, EventArgs e)
		{
			if (sandhill_character.SelectedIndex == -1) return;
			CurrentData.SandHillScores[sandhill_character.SelectedIndex][2] = (int)sandhill_score_3.Value;
		}

		private void emblem_104_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Emblems[104] = emblem_104.Checked;
		}

		private void emblem_99_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Emblems[99] = emblem_99.Checked;
		}

		private void hedgehoghammer_score_1_ValueChanged(object sender, EventArgs e)
		{
			CurrentData.HedgehogHammerScores[0] = (int)hedgehoghammer_score_1.Value;
		}

		private void hedgehoghammer_score_2_ValueChanged(object sender, EventArgs e)
		{
			CurrentData.HedgehogHammerScores[1] = (int)hedgehoghammer_score_2.Value;
		}

		private void hedgehoghammer_score_3_ValueChanged(object sender, EventArgs e)
		{
			CurrentData.HedgehogHammerScores[2] = (int)hedgehoghammer_score_3.Value;
		}

		private void emblem_105_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Emblems[105] = emblem_105.Checked;
		}

		private void emblem_100_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Emblems[100] = emblem_100.Checked;
		}

		private void circuit_character_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (circuit_character.SelectedIndex == -1)
			{
				circuit_character.SelectedIndex = 0;
				return;
			}
			int index = circuit_character.SelectedIndex;
			circuit_time_1.CircuitTime = CurrentData.TwinkleCircuitTimes[index].BestTimes[0];
			circuit_time_2.CircuitTime = CurrentData.TwinkleCircuitTimes[index].BestTimes[1];
			circuit_time_3.CircuitTime = CurrentData.TwinkleCircuitTimes[index].BestTimes[2];
			circuit_lap_1.CircuitTime = CurrentData.TwinkleCircuitTimes[index].Lap1Time;
			circuit_lap_2.CircuitTime = CurrentData.TwinkleCircuitTimes[index].Lap2Time;
		}

		private void circuit_time_1_ValueChanged(object sender, EventArgs e)
		{
			if (circuit_character.SelectedIndex == -1) return;
			CurrentData.TwinkleCircuitTimes[circuit_character.SelectedIndex].BestTimes[0] = circuit_time_1.CircuitTime;
		}

		private void circuit_time_2_ValueChanged(object sender, EventArgs e)
		{
			if (circuit_character.SelectedIndex == -1) return;
			CurrentData.TwinkleCircuitTimes[circuit_character.SelectedIndex].BestTimes[1] = circuit_time_2.CircuitTime;
		}

		private void circuit_time_3_ValueChanged(object sender, EventArgs e)
		{
			if (circuit_character.SelectedIndex == -1) return;
			CurrentData.TwinkleCircuitTimes[circuit_character.SelectedIndex].BestTimes[2] = circuit_time_3.CircuitTime;
		}

		private void circuit_lap_1_ValueChanged(object sender, EventArgs e)
		{
			if (circuit_character.SelectedIndex == -1) return;
			CurrentData.TwinkleCircuitTimes[circuit_character.SelectedIndex].Lap1Time = circuit_lap_1.CircuitTime;
		}

		private void circuit_lap_2_ValueChanged(object sender, EventArgs e)
		{
			if (circuit_character.SelectedIndex == -1) return;
			CurrentData.TwinkleCircuitTimes[circuit_character.SelectedIndex].Lap2Time = circuit_lap_2.CircuitTime;
		}

		private void emblem_101_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Emblems[101] = emblem_101.Checked;
		}

		private void emblem_96_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Emblems[96] = emblem_96.Checked;
		}

		private void boss_character_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (boss_character.SelectedIndex == -1)
			{
				boss_character.SelectedIndex = 0;
				return;
			}
			int index = boss_character.SelectedIndex;
			boss_time_1.LevelTime = CurrentData.BossTimes[index][0];
			boss_time_2.LevelTime = CurrentData.BossTimes[index][1];
			boss_time_3.LevelTime = CurrentData.BossTimes[index][2];
		}

		private void boss_time_1_ValueChanged(object sender, EventArgs e)
		{
			if (boss_character.SelectedIndex == -1) return;
			CurrentData.BossTimes[boss_character.SelectedIndex][0] = boss_time_1.LevelTime;
		}

		private void boss_time_2_ValueChanged(object sender, EventArgs e)
		{
			if (boss_character.SelectedIndex == -1) return;
			CurrentData.BossTimes[boss_character.SelectedIndex][1] = boss_time_2.LevelTime;
		}

		private void boss_time_3_ValueChanged(object sender, EventArgs e)
		{
			if (boss_character.SelectedIndex == -1) return;
			CurrentData.BossTimes[boss_character.SelectedIndex][2] = boss_time_3.LevelTime;
		}

		private void emblem_106_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Emblems[106] = emblem_106.Checked;
		}

		private void emblem_107_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Emblems[107] = emblem_107.Checked;
		}

		private void emblem_108_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Emblems[108] = emblem_108.Checked;
		}

		private void emblem_109_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Emblems[109] = emblem_109.Checked;
		}

		private void emblem_110_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Emblems[110] = emblem_110.Checked;
		}

		private void emblem_118_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Emblems[118] = emblem_118.Checked;
		}

		private void emblem_119_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Emblems[119] = emblem_119.Checked;
		}

		private void emblem_120_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Emblems[120] = emblem_120.Checked;
		}

		private void emblem_121_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Emblems[121] = emblem_121.Checked;
		}

		private void emblem_122_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Emblems[122] = emblem_122.Checked;
		}

		private void emblem_123_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Emblems[123] = emblem_123.Checked;
		}

		private void emblem_124_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Emblems[124] = emblem_124.Checked;
		}

		private void emblem_125_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Emblems[125] = emblem_125.Checked;
		}

		private void emblem_126_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Emblems[126] = emblem_126.Checked;
		}

		private void emblem_127_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Emblems[127] = emblem_127.Checked;
		}

		private void emblem_128_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Emblems[128] = emblem_128.Checked;
		}

		private void emblem_129_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Emblems[129] = emblem_129.Checked;
		}

		private void events_unused_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			CurrentData.EventFlags[e.Index] = e.NewValue == CheckState.Checked;
		}

		private void events_general_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			CurrentData.EventFlags[e.Index + 64] = e.NewValue == CheckState.Checked;
		}

		private void events_sonic_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			CurrentData.EventFlags[e.Index + 128] = e.NewValue == CheckState.Checked;
		}

		private void events_tails_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			CurrentData.EventFlags[e.Index + 192] = e.NewValue == CheckState.Checked;
		}

		private void events_knuckles_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			CurrentData.EventFlags[e.Index + 256] = e.NewValue == CheckState.Checked;
		}

		private void events_amy_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			CurrentData.EventFlags[e.Index + 320] = e.NewValue == CheckState.Checked;
		}

		private void events_gamma_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			CurrentData.EventFlags[e.Index + 384] = e.NewValue == CheckState.Checked;
		}

		private void events_big_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			CurrentData.EventFlags[e.Index + 448] = e.NewValue == CheckState.Checked;
		}

		private void npcs_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			CurrentData.NPCFlags[e.Index] = e.NewValue == CheckState.Checked;
		}

		private void levelclear_character_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (levelclear_character.SelectedIndex == -1)
			{
				levelclear_character.SelectedIndex = 0;
				return;
			}
			for (int i = 0; i < 43; i++)
				((NumericUpDown)level_clear_table.GetControlFromPosition(1, i)).Value = CurrentData.LevelClearCounts[levelclear_character.SelectedIndex][i];
		}

		private void levelclear_ValueChanged(object sender, EventArgs e)
		{
			if (levelclear_character.SelectedIndex == -1) return;
			CurrentData.LevelClearCounts[levelclear_character.SelectedIndex][level_clear_table.GetPositionFromControl((Control)sender).Row] = (byte)((NumericUpDown)sender).Value;
		}

		private void blackmarket_rings_ValueChanged(object sender, EventArgs e)
		{
			CurrentData.BlackMarketRings = (int)blackmarket_rings.Value;
		}

		private void mission_unlocked_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Missions[MissionTable.GetPositionFromControl((Control)sender).Row - 1].Unlocked = ((CheckBox)sender).Checked;
		}

		private void mission_active_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Missions[MissionTable.GetPositionFromControl((Control)sender).Row - 1].Active = ((CheckBox)sender).Checked;
		}

		private void mission_complete_CheckedChanged(object sender, EventArgs e)
		{
			CurrentData.Missions[MissionTable.GetPositionFromControl((Control)sender).Row - 1].Complete = ((CheckBox)sender).Checked;
		}

		private void metal_level_select_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (metal_level_select.SelectedIndex == -1)
			{
				if (metal_level_select.DataSource != null)
					metal_level_select.SelectedIndex = 0;
				return;
			}
			int index = (int)metal_level_select.SelectedValue;
			metal_stage_emblem_a.Checked = CurrentData.MetalEmblems[index * 3];
			metal_stage_emblem_b.Checked = CurrentData.MetalEmblems[index * 3 + 1];
			metal_stage_emblem_c.Checked = CurrentData.MetalEmblems[index * 3 + 2];
			metal_level_score.Value = CurrentData.MetalLevelScores[index];
			metal_level_time.LevelTime = CurrentData.MetalLevelTimes[index];
			metal_level_rings.Value = CurrentData.MetalLevelRings[index];
		}

		private void metal_stage_emblem_a_CheckedChanged(object sender, EventArgs e)
		{
			if (metal_level_select.SelectedIndex == -1) return;
			CurrentData.MetalEmblems[(int)metal_level_select.SelectedValue * 3] = metal_stage_emblem_a.Checked;
		}

		private void metal_stage_emblem_b_CheckedChanged(object sender, EventArgs e)
		{
			if (metal_level_select.SelectedIndex == -1) return;
			CurrentData.MetalEmblems[(int)metal_level_select.SelectedValue * 3 + 1] = metal_stage_emblem_b.Checked;
		}

		private void metal_stage_emblem_c_CheckedChanged(object sender, EventArgs e)
		{
			if (metal_level_select.SelectedIndex == -1) return;
			CurrentData.MetalEmblems[(int)metal_level_select.SelectedValue * 3 + 2] = metal_stage_emblem_c.Checked;
		}

		private void metal_level_score_ValueChanged(object sender, EventArgs e)
		{
			if (metal_level_select.SelectedIndex == -1) return;
			CurrentData.MetalLevelScores[(int)metal_level_select.SelectedValue] = (int)metal_level_score.Value;
		}

		private void metal_level_time_ValueChanged(object sender, EventArgs e)
		{
			if (metal_level_select.SelectedIndex == -1) return;
			CurrentData.MetalLevelTimes[(int)metal_level_select.SelectedValue] = metal_level_time.LevelTime;
		}

		private void metal_level_rings_ValueChanged(object sender, EventArgs e)
		{
			if (metal_level_select.SelectedIndex == -1) return;
			CurrentData.MetalLevelRings[(int)metal_level_select.SelectedValue] = (short)metal_level_rings.Value;
		}

		private void icecap_metal_sonic_score_1_ValueChanged(object sender, EventArgs e)
		{
			CurrentData.MetalIceCapScores[0] = (int)icecap_metal_sonic_score_1.Value;
		}

		private void icecap_metal_sonic_score_2_ValueChanged(object sender, EventArgs e)
		{
			CurrentData.MetalIceCapScores[1] = (int)icecap_metal_sonic_score_2.Value;
		}

		private void icecap_metal_sonic_score_3_ValueChanged(object sender, EventArgs e)
		{
			CurrentData.MetalIceCapScores[2] = (int)icecap_metal_sonic_score_3.Value;
		}

		private void sandhill_metal_sonic_score_1_ValueChanged(object sender, EventArgs e)
		{
			CurrentData.MetalSandHillScores[0] = (int)sandhill_metal_sonic_score_1.Value;
		}

		private void sandhill_metal_sonic_score_2_ValueChanged(object sender, EventArgs e)
		{
			CurrentData.MetalSandHillScores[1] = (int)sandhill_metal_sonic_score_2.Value;
		}

		private void sandhill_metal_sonic_score_3_ValueChanged(object sender, EventArgs e)
		{
			CurrentData.MetalSandHillScores[2] = (int)sandhill_metal_sonic_score_3.Value;
		}

		private void circuit_metal_time_1_ValueChanged(object sender, EventArgs e)
		{
			CurrentData.MetalTwinkleCircuitTimes.BestTimes[0] = circuit_metal_time_1.CircuitTime;
		}

		private void circuit_metal_time_2_ValueChanged(object sender, EventArgs e)
		{
			CurrentData.MetalTwinkleCircuitTimes.BestTimes[1] = circuit_metal_time_2.CircuitTime;
		}

		private void circuit_metal_time_3_ValueChanged(object sender, EventArgs e)
		{
			CurrentData.MetalTwinkleCircuitTimes.BestTimes[2] = circuit_metal_time_3.CircuitTime;
		}

		private void circuit_metal_lap_1_ValueChanged(object sender, EventArgs e)
		{
			CurrentData.MetalTwinkleCircuitTimes.Lap1Time = circuit_metal_lap_1.CircuitTime;
		}

		private void circuit_metal_lap_2_ValueChanged(object sender, EventArgs e)
		{
			CurrentData.MetalTwinkleCircuitTimes.Lap2Time = circuit_metal_lap_2.CircuitTime;
		}

		private void boss_metal_sonic_time_1_ValueChanged(object sender, EventArgs e)
		{
			CurrentData.MetalBossTimes[0] = boss_metal_sonic_time_1.LevelTime;
		}

		private void boss_metal_sonic_time_2_ValueChanged(object sender, EventArgs e)
		{
			CurrentData.MetalBossTimes[1] = boss_metal_sonic_time_2.LevelTime;
		}

		private void boss_metal_sonic_time_3_ValueChanged(object sender, EventArgs e)
		{
			CurrentData.MetalBossTimes[2] = boss_metal_sonic_time_3.LevelTime;
		}
	}

	class SaveSlot
	{
		public string Filename { get; set; }
		public SaveData Data { get; set; }
		public Systems System { get; set; }
		public Regions Region { get; set; }

		public SaveSlot()
		{
			Data = new SaveData(Properties.Resources.PC);
			System = Systems.PC;
			Region = Regions.US;
		}
	}

	enum Systems
	{
		Dreamcast,
		GameCube,
		PC,
		XBox360,
		Steam,
		GameCubePreview
	}

	enum Regions
	{
		US,
		Europe,
		Japan,
		International
	}
}