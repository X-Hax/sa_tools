using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SA_Tools;

namespace SADXTweaker2
{
	public partial class MessageFileEditor : Form, ITextSearch
	{
		static readonly string[] fields = { "SS", "MR", "PAST" };
		static readonly string[] chars = { "S", "M", "K", "A", "E", "B", "L" };
		static readonly string[] langs = { "J", "E", "F", "S", "G" };
		static readonly string filepattern = "{0}_MES_{1}_{2}.BIN";
		static readonly uint[] baseaddrs = { 0xCB46000, 0xCB4A000, 0xCB4A000 };
		static readonly int[] basecounts = { 36, 5, 20 };

		string CurrentFile
		{
			get { return Path.Combine(Program.IniData.SystemFolder, string.Format(filepattern, fields[field.SelectedIndex], chars[character.SelectedIndex], langs[language.SelectedIndex])); }
		}

		List<NPCText> NPCs = new List<NPCText>();

		NPCText CurrentNPC { get { return NPCs[npcID.SelectedIndex]; } }
		NPCTextGroup CurrentGroup { get { return CurrentNPC.Groups[groupNum.SelectedIndex]; } }

		public MessageFileEditor()
		{
			InitializeComponent();
		}

		private void MessageFileEditor_Load(object sender, EventArgs e)
		{
			axWindowsMediaPlayer1.settings.autoStart = false;
			voiceNum.Directory = Program.IniData.VoiceFolder;
			field.SelectedIndex = character.SelectedIndex = 0;
			language.SelectedIndex = 1;
		}

		private void field_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (field.SelectedIndex == -1 | character.SelectedIndex == -1 | language.SelectedIndex == -1) return;
			loadButton.Enabled = File.Exists(CurrentFile);
		}

		private void loadButton_Click(object sender, EventArgs e)
		{
			byte[] file = File.ReadAllBytes(CurrentFile);
			NPCs = new List<NPCText>(NPCTextList.Load(file, file.GetPointer(4, baseaddrs[field.SelectedIndex]), baseaddrs[field.SelectedIndex],
				BitConverter.ToInt32(file, 0) + basecounts[field.SelectedIndex], (Languages)language.SelectedIndex, false));
			npcID.Items.Clear();
			if (NPCs.Count > 0)
			{
				npcID.Items.AddRange(Array.ConvertAll(Enumerable.Range(1, NPCs.Count).ToArray(), (a) => (object)a));
				npcID.SelectedIndex = 0;
			}
		}

		private void saveButton_Click(object sender, EventArgs e)
		{
			List<byte> file = new List<byte>();
			List<byte> headers = new List<byte>(NPCs.Count * 8);
			foreach (NPCText item in NPCs)
			{
				if (item.Groups.Count == 0)
				{
					headers.AddRange(new byte[8]);
					continue;
				}
				if (item.HasControl)
				{
					headers.AddRange(BitConverter.GetBytes(baseaddrs[field.SelectedIndex] + (uint)file.Count + 8u));
					bool first = true;
					foreach (NPCTextGroup group in item.Groups)
					{
						if (!first)
							file.AddRange(BitConverter.GetBytes((short)NPCTextControl.NewGroup));
						else
							first = false;
						foreach (ushort flag in group.EventFlags)
						{
							file.AddRange(BitConverter.GetBytes((short)NPCTextControl.EventFlag));
							file.AddRange(BitConverter.GetBytes(flag));
						}
						foreach (ushort flag in group.NPCFlags)
						{
							file.AddRange(BitConverter.GetBytes((short)NPCTextControl.NPCFlag));
							file.AddRange(BitConverter.GetBytes(flag));
						}
						if (group.Character != (SA1CharacterFlags)0xFF)
						{
							file.AddRange(BitConverter.GetBytes((short)NPCTextControl.Character));
							file.AddRange(BitConverter.GetBytes((ushort)group.Character));
						}
						if (group.Voice.HasValue)
						{
							file.AddRange(BitConverter.GetBytes((short)NPCTextControl.Voice));
							file.AddRange(BitConverter.GetBytes(group.Voice.Value));
						}
						if (group.SetEventFlag.HasValue)
						{
							file.AddRange(BitConverter.GetBytes((short)NPCTextControl.SetEventFlag));
							file.AddRange(BitConverter.GetBytes(group.SetEventFlag.Value));
						}
					}
					file.AddRange(BitConverter.GetBytes((short)NPCTextControl.End));
				}
				else
					headers.AddRange(new byte[4]);
				if (item.HasText)
				{
					List<byte> textptrs = new List<byte>();
					foreach (NPCTextGroup group in item.Groups)
					{
						foreach (NPCTextLine line in group.Lines)
						{
							textptrs.AddRange(BitConverter.GetBytes(baseaddrs[field.SelectedIndex] + (uint)file.Count + 8u));
							file.AddRange(HelperFunctions.GetEncoding((Languages)language.SelectedIndex).GetBytes(line.Line));
							file.Add(0);
							int off = file.Count % 4;
							if (off != 0)
								file.AddRange(new byte[4 - off]);
						}
						textptrs.AddRange(new byte[4]);
					}
					headers.AddRange(BitConverter.GetBytes(baseaddrs[field.SelectedIndex] + (uint)file.Count + 8u));
					file.AddRange(textptrs);
				}
				else
					headers.AddRange(new byte[4]);
			}
			file.InsertRange(0, BitConverter.GetBytes(baseaddrs[field.SelectedIndex] + (uint)file.Count + 8u));
			file.InsertRange(0, BitConverter.GetBytes(NPCs.Count - basecounts[field.SelectedIndex]));
			file.AddRange(headers);
			File.WriteAllBytes(CurrentFile, file.ToArray());
		}

		private void npcID_SelectedIndexChanged(object sender, EventArgs e)
		{
			groupNum.Items.Clear();
			if (npcID.SelectedIndex == -1)
				panel2.Enabled = npcRemoveButton.Enabled = false;
			else
			{
				panel2.Enabled = npcRemoveButton.Enabled = true;
				if (CurrentNPC.Groups.Count > 0)
				{
					groupNum.Items.AddRange(Array.ConvertAll(Enumerable.Range(1, CurrentNPC.Groups.Count).ToArray(), (a) => (object)a));
					groupNum.SelectedIndex = 0;
				}
			}
		}

		private void npcAddButton_Click(object sender, EventArgs e)
		{
			NPCs.Add(new NPCText());
			npcID.Items.Add(npcID.Items.Count + 1);
			npcID.SelectedIndex = npcID.Items.Count - 1;
		}

		private void npcRemoveButton_Click(object sender, EventArgs e)
		{
			NPCs.RemoveAt(npcID.SelectedIndex);
			npcID.Items.RemoveAt(npcID.Items.Count - 1);
			npcID_SelectedIndexChanged(npcRemoveButton, EventArgs.Empty);
		}

		private void groupNum_SelectedIndexChanged(object sender, EventArgs e)
		{
			eventFlagNum.Items.Clear();
			npcFlagNum.Items.Clear();
			lineNum.Items.Clear();
			if (groupNum.SelectedIndex == -1)
				panel3.Enabled = groupRemoveButton.Enabled = false;
			else
			{
				panel3.Enabled = groupRemoveButton.Enabled = true;
				if (CurrentGroup.EventFlags.Count > 0)
				{
					eventFlagNum.Items.AddRange(Array.ConvertAll(Enumerable.Range(1, CurrentGroup.EventFlags.Count).ToArray(), (a) => (object)a));
					eventFlagNum.SelectedIndex = 0;
				}
				else
					eventRemoveButton.Enabled = eventFlag.Enabled = eventNotSet.Enabled = false;
				if (CurrentGroup.NPCFlags.Count > 0)
				{
					npcFlagNum.Items.AddRange(Array.ConvertAll(Enumerable.Range(1, CurrentGroup.NPCFlags.Count).ToArray(), (a) => (object)a));
					npcFlagNum.SelectedIndex = 0;
				}
				else
					npcFlagRemoveButton.Enabled = npcFlag.Enabled = npcNotSet.Enabled = false;
				SA1CharacterFlags flags = CurrentGroup.Character;
				sonic.Checked = (flags & SA1CharacterFlags.Sonic) == SA1CharacterFlags.Sonic;
				eggman.Checked = (flags & SA1CharacterFlags.Eggman) == SA1CharacterFlags.Eggman;
				tails.Checked = (flags & SA1CharacterFlags.Tails) == SA1CharacterFlags.Tails;
				knuckles.Checked = (flags & SA1CharacterFlags.Knuckles) == SA1CharacterFlags.Knuckles;
				tikal.Checked = (flags & SA1CharacterFlags.Tikal) == SA1CharacterFlags.Tikal;
				amy.Checked = (flags & SA1CharacterFlags.Amy) == SA1CharacterFlags.Amy;
				gamma.Checked = (flags & SA1CharacterFlags.Gamma) == SA1CharacterFlags.Gamma;
				big.Checked = (flags & SA1CharacterFlags.Big) == SA1CharacterFlags.Big;
				ushort? val = CurrentGroup.Voice;
				if (voice.Checked = val.HasValue)
					voiceNum.Value = val.Value.ToString("0000", NumberFormatInfo.InvariantInfo);
				val = CurrentGroup.SetEventFlag;
				if (setEventFlag.Checked = val.HasValue)
				{
					ushort v = val.Value;
					setEventNum.Value = v & 0x3FF;
					setEventUnset.Checked = (v & 0x4000) == 0x4000;
				}
				if (CurrentGroup.Lines.Count > 0)
				{
					lineNum.Items.AddRange(Array.ConvertAll(Enumerable.Range(1, CurrentGroup.Lines.Count).ToArray(), (a) => (object)a));
					lineNum.SelectedIndex = 0;
				}
				else
					lineRemoveButton.Enabled = line.Enabled = true;
			}
		}

		private void groupAddButton_Click(object sender, EventArgs e)
		{
			CurrentNPC.Groups.Add(new NPCTextGroup());
			groupNum.Items.Add(groupNum.Items.Count + 1);
			groupNum.SelectedIndex = groupNum.Items.Count - 1;
		}

		private void groupRemoveButton_Click(object sender, EventArgs e)
		{
			CurrentNPC.Groups.RemoveAt(groupNum.SelectedIndex);
			groupNum.Items.RemoveAt(groupNum.Items.Count - 1);
			groupNum_SelectedIndexChanged(groupRemoveButton, EventArgs.Empty);
		}

		private void eventFlagNum_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (eventFlagNum.SelectedIndex == -1)
				eventRemoveButton.Enabled = eventFlag.Enabled = eventNotSet.Enabled = false;
			else
			{
				eventRemoveButton.Enabled = eventFlag.Enabled = eventNotSet.Enabled = true;
				ushort v = CurrentGroup.EventFlags[eventFlagNum.SelectedIndex];
				eventFlag.Value = v & 0x3FF;
				eventNotSet.Checked = (v & 0x4000) == 0x4000;
			}
		}

		private void eventAddButton_Click(object sender, EventArgs e)
		{
			CurrentGroup.EventFlags.Add(0);
			eventFlagNum.Items.Add(eventFlagNum.Items.Count + 1);
			eventFlagNum.SelectedIndex = eventFlagNum.Items.Count - 1;
		}

		private void eventRemoveButton_Click(object sender, EventArgs e)
		{
			CurrentGroup.EventFlags.RemoveAt(eventFlagNum.SelectedIndex);
			eventFlagNum.Items.RemoveAt(eventFlagNum.Items.Count - 1);
			eventFlagNum_SelectedIndexChanged(eventRemoveButton, EventArgs.Empty);
		}

		private void eventFlag_ValueChanged(object sender, EventArgs e)
		{
			CurrentGroup.EventFlags[eventFlagNum.SelectedIndex] = (ushort)((int)eventFlag.Value | (eventNotSet.Checked ? 0x4000 : 0));
		}

		private void npcFlagNum_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (npcFlagNum.SelectedIndex == -1)
				npcFlagRemoveButton.Enabled = npcFlag.Enabled = npcNotSet.Enabled = false;
			else
			{
				npcFlagRemoveButton.Enabled = npcFlag.Enabled = npcNotSet.Enabled = true;
				ushort v = CurrentGroup.NPCFlags[npcFlagNum.SelectedIndex];
				npcFlag.Value = v & 0x3FF;
				npcNotSet.Checked = (v & 0x4000) == 0x4000;
			}
		}

		private void npcFlagAddButton_Click(object sender, EventArgs e)
		{
			CurrentGroup.NPCFlags.Add(0);
			npcFlagNum.Items.Add(npcFlagNum.Items.Count + 1);
			npcFlagNum.SelectedIndex = npcFlagNum.Items.Count - 1;
		}

		private void npcFlagRemoveButton_Click(object sender, EventArgs e)
		{
			CurrentGroup.NPCFlags.RemoveAt(npcFlagNum.SelectedIndex);
			npcFlagNum.Items.RemoveAt(npcFlagNum.Items.Count - 1);
			npcFlagNum_SelectedIndexChanged(npcRemoveButton, EventArgs.Empty);
		}

		private void npcFlag_ValueChanged(object sender, EventArgs e)
		{
			CurrentGroup.NPCFlags[npcFlagNum.SelectedIndex] = (ushort)((int)npcFlag.Value | (npcNotSet.Checked ? 0x4000 : 0));
		}

		private void character_CheckedChanged(object sender, EventArgs e)
		{
			SA1CharacterFlags flags = 0;
			if (sonic.Checked) flags |= SA1CharacterFlags.Sonic;
			if (eggman.Checked) flags |= SA1CharacterFlags.Eggman;
			if (tails.Checked) flags |= SA1CharacterFlags.Tails;
			if (knuckles.Checked) flags |= SA1CharacterFlags.Knuckles;
			if (tikal.Checked) flags |= SA1CharacterFlags.Tikal;
			if (amy.Checked) flags |= SA1CharacterFlags.Amy;
			if (gamma.Checked) flags |= SA1CharacterFlags.Gamma;
			if (big.Checked) flags |= SA1CharacterFlags.Big;
			CurrentGroup.Character = flags;
		}

		private void voice_CheckedChanged(object sender, EventArgs e)
		{
			if (voice.Checked)
			{
				CurrentGroup.Voice = 0;
				voiceNum.Enabled = axWindowsMediaPlayer1.Ctlenabled = true;
				voiceNum.Value = "0000";
			}
			else
			{
				CurrentGroup.Voice = null;
				voiceNum.Enabled = axWindowsMediaPlayer1.Ctlenabled = false;
			}
		}

		private void voiceNum_ValueChanged(object sender, EventArgs e)
		{
			CurrentGroup.Voice = ushort.Parse(voiceNum.Value, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);
			axWindowsMediaPlayer1.URL = Path.Combine(Path.Combine(Environment.CurrentDirectory, Program.IniData.VoiceFolder), voiceNum.Value + ".wma").Replace('/', Path.DirectorySeparatorChar);
		}

		private void setEventFlag_CheckedChanged(object sender, EventArgs e)
		{
			if (setEventFlag.Checked)
			{
				CurrentGroup.SetEventFlag = 0;
				setEventNum.Enabled = setEventUnset.Enabled = true;
				setEventNum.Value = 0;
				setEventUnset.Checked = false;
			}
			else
			{
				CurrentGroup.SetEventFlag = null;
				setEventNum.Enabled = setEventUnset.Enabled = false;
			}
		}

		private void setEventNum_ValueChanged(object sender, EventArgs e)
		{
			CurrentGroup.SetEventFlag = (ushort)((int)setEventNum.Value | (setEventUnset.Checked ? 0x4000 : 0));
		}

		private void lineNum_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lineNum.SelectedIndex == -1)
				lineRemoveButton.Enabled = line.Enabled = false;
			else
			{
				lineRemoveButton.Enabled = line.Enabled = true;
				line.Text = CurrentGroup.Lines[lineNum.SelectedIndex].Line.Replace("\n", Environment.NewLine);
			}
		}

		private void lineAddButton_Click(object sender, EventArgs e)
		{
			CurrentGroup.Lines.Add(new NPCTextLine());
			lineNum.Items.Add(lineNum.Items.Count + 1);
			lineNum.SelectedIndex = lineNum.Items.Count - 1;
		}

		private void lineRemoveButton_Click(object sender, EventArgs e)
		{
			CurrentGroup.Lines.RemoveAt(lineNum.SelectedIndex);
			lineNum.Items.RemoveAt(lineNum.Items.Count - 1);
			lineNum_SelectedIndexChanged(npcRemoveButton, EventArgs.Empty);
		}

		private void line_TextChanged(object sender, EventArgs e)
		{
			CurrentGroup.Lines[lineNum.SelectedIndex].Line = line.Text.Replace(Environment.NewLine, "\n");
		}

		class SearchResult
		{
			public int npc, group, line;
			public string text;

			public SearchResult(int npc, int group, int line, string text)
			{
				this.npc = npc;
				this.group = group;
				this.line = line;
				this.text = text;
			}

			public override string ToString()
			{
				return string.Format("{0}.{1}.{2}: {3}", npc + 1, group + 1, line + 1, text);
			}
		}

		public object[] GetSearchResults(string searchText)
		{
			List<object> results = new List<object>();
			for (int npci = 0; npci < NPCs.Count; npci++)
				for (int groupi = 0; groupi < NPCs[npci].Groups.Count; groupi++)
					for (int linei = 0; linei < NPCs[npci].Groups[groupi].Lines.Count; linei++)
						if (NPCs[npci].Groups[groupi].Lines[linei].Line.Replace('\n', ' ').Contains(searchText))
							results.Add(new SearchResult(npci, groupi, linei, NPCs[npci].Groups[groupi].Lines[linei].Line.Replace('\n', ' ')));
			return results.ToArray();
		}

		private void searchButton_Click(object sender, EventArgs e)
		{
			using (TextSearchDialog dlg = new TextSearchDialog(this))
				if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					SearchResult result = (SearchResult)dlg.Result;
					npcID.SelectedIndex = result.npc;
					groupNum.SelectedIndex = result.group;
					lineNum.SelectedIndex = result.line;
				}
		}
	}
}