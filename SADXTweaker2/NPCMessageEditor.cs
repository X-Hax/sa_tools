using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SA_Tools;

namespace SADXTweaker2
{
    public partial class NPCMessageEditor : Form
    {
        List<KeyValuePair<string, NPCText[][]>> NPCs = new List<KeyValuePair<string, NPCText[][]>>();

        NPCText[] CurrentList
        {
            get { return NPCs[level.SelectedIndex].Value[language.SelectedIndex]; }
            set { NPCs[level.SelectedIndex].Value[language.SelectedIndex] = value; }
        }
        NPCText CurrentNPC { get { return CurrentList[npcID.SelectedIndex]; } }
        NPCTextGroup CurrentGroup { get { return CurrentNPC.Groups[groupNum.SelectedIndex]; } }

        public NPCMessageEditor()
        {
            InitializeComponent();
        }

        private void NPCMessageEditor_Load(object sender, EventArgs e)
        {
            level.BeginUpdate();
			foreach (KeyValuePair<string, SA_Tools.FileInfo> item in Program.IniData.Files)
				if (item.Value.Type.Equals("npctext", StringComparison.OrdinalIgnoreCase))
				{
					NPCs.Add(new KeyValuePair<string, NPCText[][]>(item.Value.Filename, NPCTextList.Load(item.Value.Filename, int.Parse(item.Value.CustomProperties["length"], NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo))));
					level.Items.Add(item.Key);
				}
            level.EndUpdate();
            axWindowsMediaPlayer1.settings.autoStart = false;
            voiceNum.Directory = Program.IniData.VoiceFolder;
            language.SelectedIndex = 1;
            level.SelectedIndex = 0;
        }

        private void NPCMessageEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (MessageBox.Show(this, "Do you want to save?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1))
            {
                case DialogResult.Cancel:
                    e.Cancel = true;
                    break;
                case DialogResult.Yes:
                    foreach (KeyValuePair<string, NPCText[][]> item in NPCs)
                        item.Value.Save(item.Key);
                    break;
            }
        }

        private void level_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (level.SelectedIndex == -1) return;
            npcID.Items.Clear();
            if (CurrentList.Length > 0)
            {
                npcID.Items.AddRange(Array.ConvertAll(Enumerable.Range(1, CurrentList.Length).ToArray(), (a) => (object)a));
                npcID.SelectedIndex = 0;
            }
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
            List<NPCText> list = new List<NPCText>(CurrentList);
            list.Add(new NPCText());
            CurrentList = list.ToArray();
            npcID.Items.Add(npcID.Items.Count + 1);
            npcID.SelectedIndex = npcID.Items.Count - 1;
        }

        private void npcRemoveButton_Click(object sender, EventArgs e)
        {
            List<NPCText> list = new List<NPCText>(CurrentList);
            list.RemoveAt(npcID.SelectedIndex);
            CurrentList = list.ToArray();
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
                    lineRemoveButton.Enabled = line.Enabled = lineTime.Enabled = false;
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
                lineRemoveButton.Enabled = line.Enabled = lineTime.Enabled = false;
            else
            {
                lineRemoveButton.Enabled = line.Enabled = lineTime.Enabled = true;
                line.Text = CurrentGroup.Lines[lineNum.SelectedIndex].Line.Replace("\n", Environment.NewLine);
                lineTime.TotalFrames = (uint)CurrentGroup.Lines[lineNum.SelectedIndex].Time;
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

        private void lineTime_ValueChanged(object sender, EventArgs e)
        {
            CurrentGroup.Lines[lineNum.SelectedIndex].Time = (int)lineTime.TotalFrames;
        }
    }
}