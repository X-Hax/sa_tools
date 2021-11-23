using FraGag.Compression;
using SplitTools;
using SAModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAModel.SAEditorCommon;

namespace SA2MessageFileEditor
{
	public partial class MainForm : Form
	{
		public MainForm(string[] args)
		{
			InitializeComponent();
			if (args.Length > 0)
				filename = Path.GetFullPath(args[0]);
		}

		List<Message> CurrentMessage { get { return messages[messageNum.SelectedIndex]; } }
		Message CurrentLine { get { return CurrentMessage[lineNum.SelectedIndex]; } }

		static readonly Encoding jpenc = Encoding.GetEncoding(932);
		static readonly Encoding euenc = Encoding.GetEncoding(1252);
		List<string> recentFiles = new List<string>();
		string filename = null;
		List<List<Message>> messages = new List<List<Message>>();
		bool bigEndian;
		bool useSJIS;
		SearchCriteria currentSearch;
		int searchMessage, searchLine;
		SettingsFile settingsFile;

		private void MainForm_Load(object sender, EventArgs e)
		{
			settingsFile = SettingsFile.Load();
			if (settingsFile.SA2MessageFileEditor.RecentFiles != null)
				recentFiles = new List<string>(settingsFile.SA2MessageFileEditor.RecentFiles.Where(a => File.Exists(a)));
			if (recentFiles.Count > 0)
				UpdateRecentFiles();
			bigEndian = settingsFile.SA2MessageFileEditor.BigEndian;
			bigEndianGCSteamToolStripMenuItem.Checked = bigEndian;
			useSJIS = settingsFile.SA2MessageFileEditor.UseSJIS;
			shiftJISToolStripMenuItem.Checked = useSJIS;
			windows1252ToolStripMenuItem.Checked = !useSJIS;
			if (filename != null)
				LoadFile(filename);
		}

		private void UpdateRecentFiles()
		{
			recentFilesToolStripMenuItem.DropDownItems.Clear();
			foreach (string item in recentFiles)
				recentFilesToolStripMenuItem.DropDownItems.Add(item);
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			switch (MessageBox.Show(this, "Do you want to save before exiting?", "SA2 Message File Editor", MessageBoxButtons.YesNoCancel, MessageBoxIcon.None, MessageBoxDefaultButton.Button3))
			{
				case DialogResult.Yes:
					break;
				case DialogResult.Cancel:
					e.Cancel = true;
					return;
			}
			settingsFile.Save();
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			messages.Clear();
			messageNum.Items.Clear();
			findNextToolStripMenuItem.Enabled = false;
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "prs", Filter = "Supported Files|*.prs;*.bin|All Files|*.*" })
				if (dlg.ShowDialog(this) == DialogResult.OK)
					LoadFile(dlg.FileName);
		}

        private bool CheckForEscapeCharacter(string text)
        {
            int c = 0;
            while (c < text.Length)
            {
                if (text[c] == '\f')
                    return true;
                c++;
            }
            return false;
        }

		private void LoadFile(string filename)
		{
			this.filename = filename;
			byte[] fc;
			if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				fc = Prs.Decompress(filename);
			else
				fc = File.ReadAllBytes(filename);
			ByteConverter.BigEndian = false;
			uint le = ByteConverter.ToUInt32(fc, 0);
			ByteConverter.BigEndian = true;
			uint be = ByteConverter.ToUInt32(fc, 0);
			if (be > le)
				bigEndian = false;
			else if (be < le)
				bigEndian = true;
			ByteConverter.BigEndian = bigEndian;
			bigEndianGCSteamToolStripMenuItem.Checked = bigEndian;
			useSJIS = char.ToLowerInvariant(Path.GetFileNameWithoutExtension(filename).Last()) == 'j';
			shiftJISToolStripMenuItem.Checked = useSJIS;
			windows1252ToolStripMenuItem.Checked = !useSJIS;
			Encoding encoding = useSJIS ? jpenc : euenc;
			messages.Clear();
			int address = 0;
			int off = ByteConverter.ToInt32(fc, 0);
            bool hasEscape = false;
			while (off != -1)
			{
                string str = fc.GetCString(off, encoding);
                messages.Add(Message.FromString(str));
                if (CheckForEscapeCharacter(str))
                    hasEscape = true; 
                address += 4;
				off = ByteConverter.ToInt32(fc, address);
			}
            textOnlyToolStripMenuItem.Checked = !hasEscape;
			UpdateMessageSelect();
			messagePanel.Enabled = false;
			AddRecentFile(filename);
			Text = "SA2 Message File Editor - " + Path.GetFileName(filename);
			findNextToolStripMenuItem.Enabled = false;
		}

		private void UpdateMessageSelect()
		{
			messageNum.Items.Clear();
			if (messages.Count > 0)
				messageNum.Items.AddRange(messages.Select((a, i) => (object)((i + 1).ToString() + ": " + (a.Count > 0 ? a[0].GetPreview() : string.Empty))).ToArray());
		}

		private void AddRecentFile(string filename)
		{
			if (recentFiles.Contains(filename))
				recentFiles.Remove(filename);
			recentFiles.Insert(0, filename);
			if (recentFiles.Count > 10)
				recentFiles.RemoveAt(10);
			UpdateRecentFiles();
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (filename == null)
				saveAsToolStripMenuItem_Click(sender, e);
			SaveFile();
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "prs", Filter = "Supported Files|*.prs;*.bin|All Files|*.*" })
			{
				if (filename != null)
				{
					dlg.FileName = Path.GetFileName(filename);
					dlg.InitialDirectory = Path.GetDirectoryName(filename);
				}
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					filename = dlg.FileName;
					SaveFile();
					AddRecentFile(filename);
					Text = "SA2 Message File Editor - " + Path.GetFileName(filename);
				}
			}
		}

		private void SaveFile()
		{
			ByteConverter.BigEndian = bigEndian;
			int addr = (messages.Count + 1) * 4;
			List<byte> fc = new List<byte>();
			Encoding encoding = useSJIS ? jpenc : euenc;
			List<string> strs = new List<string>(messages.Select(a => Message.ToString(a,textOnlyToolStripMenuItem.Checked)));
			foreach (string item in strs)
			{
				fc.AddRange(ByteConverter.GetBytes(addr));
				addr += encoding.GetByteCount(item) + 1;
			}
			fc.AddRange(BitConverter.GetBytes(-1));
			foreach (string item in strs)
			{
				fc.AddRange(encoding.GetBytes(item));
				fc.Add(0);
			}
			if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				Prs.Compress(fc.ToArray(), filename);
			else
				File.WriteAllBytes(filename, fc.ToArray());
		}

		private void exportToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "txt", Filter = "Text Files|*.txt|All Files|*.*" })
			{
				if (filename != null)
				{
					dlg.FileName = Path.ChangeExtension(Path.GetFileName(filename), ".txt");
					dlg.InitialDirectory = Path.GetDirectoryName(filename);
				}
				if (dlg.ShowDialog(this) == DialogResult.OK)
					using (StreamWriter sw = File.CreateText(dlg.FileName))
						foreach (List<Message> msg in messages)
						{
							foreach (string line in msg.Select(a => a.Text))
								sw.WriteLine(line.Replace("\n", Environment.NewLine));
							sw.WriteLine();
						}
			}
		}

		private void recentFilesToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			LoadFile(recentFiles[recentFilesToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem)]);
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void shiftJISToolStripMenuItem_Click(object sender, EventArgs e)
		{
			useSJIS = true;
			shiftJISToolStripMenuItem.Checked = true;
			windows1252ToolStripMenuItem.Checked = false;
		}

		private void windows1252ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			useSJIS = false;
			shiftJISToolStripMenuItem.Checked = false;
			windows1252ToolStripMenuItem.Checked = true;
		}

		private void bigEndianGCSteamToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bigEndian = !bigEndian;
		}

		private void messageNum_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (messageNum.SelectedIndex == -1)
				messagePanel.Enabled = messageRemoveButton.Enabled = false;
			else
			{
				messagePanel.Enabled = messageRemoveButton.Enabled = true;
				UpdateLineSelect();
				linePanel.Enabled = false;
			}
		}

		private void UpdateLineSelect()
		{
			lineNum.Items.Clear();
			lineNum.Items.AddRange(CurrentMessage.Select((a, i) => (object)((i + 1).ToString() + ": " + a.GetPreview())).ToArray());
		}

		private void messageAddButton_Click(object sender, EventArgs e)
		{
			messages.Add(new List<Message>() { new Message() });
			messageNum.Items.Add((messageNum.Items.Count + 1) + ": ");
			messageNum.SelectedIndex = messageNum.Items.Count - 1;
		}

		private void messageRemoveButton_Click(object sender, EventArgs e)
		{
			int msg = messageNum.SelectedIndex;
			messages.RemoveAt(msg);
			if (msg == messages.Count)
				--msg;
			UpdateMessageSelect();
			messageNum.SelectedIndex = msg;
		}

		private void lineNum_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lineNum.SelectedIndex == -1)
				linePanel.Enabled = lineRemoveButton.Enabled = false;
			else
			{
				linePanel.Enabled = true;
				lineRemoveButton.Enabled = CurrentMessage.Count > 1;
				int? audio = CurrentLine.Audio;
				if (playAudioCheckBox.Checked = audio.HasValue)
					audioSelector.Value = audio.Value;
				int? voice = CurrentLine.Voice;
				if (playVoiceCheckBox.Checked = voice.HasValue)
					voiceSelector.Value = voice.Value;
				int? wait = CurrentLine.WaitTime;
				if (waitTimeCheckBox.Checked = wait.HasValue)
					waitTimeSelector.TotalFrames = wait.Value;
				messageCentered.Checked = CurrentLine.Centered;
				lineEdit.Text = CurrentLine.Text.Replace("\n", Environment.NewLine);
			}
		}

		private void lineAddButton_Click(object sender, EventArgs e)
		{
			CurrentMessage.Add(new Message());
			lineNum.Items.Add((lineNum.Items.Count + 1) + ": ");
			lineNum.SelectedIndex = lineNum.Items.Count - 1;
		}

		private void lineRemoveButton_Click(object sender, EventArgs e)
		{
			int line = lineNum.SelectedIndex;
			CurrentMessage.RemoveAt(line);
			if (line == CurrentMessage.Count)
				--line;
			UpdateLineSelect();
			lineNum.SelectedIndex = line;
		}

		private void playAudioCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (playAudioCheckBox.Checked)
			{
				CurrentLine.Audio = 0;
				audioSelector.Enabled = true;
				audioSelector.Value = 0;
				playVoiceCheckBox.Checked = false;
			}
			else
			{
				CurrentLine.Audio = null;
				audioSelector.Enabled = false;
			}
		}

		private void audioSelector_ValueChanged(object sender, EventArgs e)
		{
			CurrentLine.Audio = (int)audioSelector.Value;
		}

		private void playVoiceCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (playVoiceCheckBox.Checked)
			{
				CurrentLine.Voice = 0;
				voiceSelector.Enabled = true;
				voiceSelector.Value = 0;
				playAudioCheckBox.Checked = false;
			}
			else
			{
				CurrentLine.Voice = null;
				voiceSelector.Enabled = false;
			}
		}

		private void voiceSelector_ValueChanged(object sender, EventArgs e)
		{
			CurrentLine.Voice = (int)voiceSelector.Value;
		}

		private void waitTimeCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (waitTimeCheckBox.Checked)
			{
				CurrentLine.WaitTime = 0;
				waitTimeSelector.Enabled = true;
				waitTimeSelector.TotalFrames = 0;
			}
			else
			{
				CurrentLine.WaitTime = null;
				waitTimeSelector.Enabled = false;
			}
		}

		private void waitTimeSelector_ValueChanged(object sender, EventArgs e)
		{
			CurrentLine.WaitTime = waitTimeSelector.TotalFrames;
		}

		private void messageCentered_CheckedChanged(object sender, EventArgs e)
		{
			CurrentLine.Centered = messageCentered.Checked;
		}

		private void lineEdit_TextChanged(object sender, EventArgs e)
		{
			CurrentLine.Text = lineEdit.Text.Replace(Environment.NewLine, "\n");
		}

		private void findToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (FindDialog dlg = new FindDialog())
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					currentSearch = dlg.GetSearchCriteria();
					searchMessage = 0;
					searchLine = 0;
					findNextToolStripMenuItem.Enabled = true;
					FindNextResult();
				}
		}

		private void findNextToolStripMenuItem_Click(object sender, EventArgs e)
		{
			FindNextResult();
		}

		private void FindNextResult()
		{
			for (int msg = searchMessage; msg < messages.Count; msg++)
			{
				for (int line = searchLine; line < messages[msg].Count; line++)
				{
					if (currentSearch.Audio.HasValue && currentSearch.Audio != messages[msg][line].Audio)
						continue;
					if (currentSearch.Voice.HasValue && currentSearch.Voice != messages[msg][line].Voice)
						continue;
					if (currentSearch.Text != null && !messages[msg][line].Text.Contains(currentSearch.Text))
						continue;
					messageNum.SelectedIndex = msg;
					lineNum.SelectedIndex = line;
					searchMessage = msg;
					searchLine = line + 1;
					if (searchLine == messages[msg].Count)
					{
						searchMessage++;
						searchLine = 0;
					}
					return;
				}
				searchLine = 0;
			}
			MessageBox.Show(this, "Results not found.", "SA2 Message File Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
			findNextToolStripMenuItem.Enabled = false;
		}
	}

	public class Message
	{
		public int? Voice { get; set; }
		public int? Audio { get; set; }
		public int? WaitTime { get; set; }
		public bool Centered { get; set; } = true;
		public string Text { get; set; } = string.Empty;

		static int ParseInt(string str, ref int c)
		{
			int st = c;
			for (; c < str.Length; c++)
				if (str[c] < '0' || str[c] > '9')
					break;
			return int.Parse(str.Substring(st, c - st));
		}

		public static List<Message> FromString(string text)
		{
			List<Message> result = new List<Message>();
			if (string.IsNullOrEmpty(text)) return result;
			int c = 0;
			Message msg = new Message();
			while (c < text.Length)
			{
				if (text[c] == '\f')
				{
                    if (++c == text.Length) break;
					char v6 = text[c];
					while (v6 != ' ' && v6 != ':')
					{
						switch (v6)
						{
							case 'A':
							case 'a':
								++c;
								msg.Audio = ParseInt(text, ref c);
								break;
							case 'S':
							case 's':
								++c;
								msg.Voice = ParseInt(text, ref c);
								break;
							case 'W':
							case 'w':
								++c;
								msg.WaitTime = ParseInt(text, ref c);
								break;
							default:
								++c;
								break;
						}
						if (c == text.Length) break;
						v6 = text[c];
					}
					if (c == text.Length) break;
					++c;
				}
				int next = text.IndexOf('\f', c);
				if (next == -1)
				{
					msg.Text = text.Substring(c);
					if (msg.Text.StartsWith("\a"))
						msg.Text = msg.Text.TrimStart('\a');
					else
						msg.Centered = false;
					break;
				}
				if (next != c)
				{
					msg.Text = text.Substring(c, next - c);
					if (msg.Text.StartsWith("\a"))
						msg.Text = msg.Text.TrimStart('\a');
					else
						msg.Centered = false;
					c = next;
				}
				result.Add(msg);
				msg = new Message();
			}
			result.Add(msg);
			return result;
		}

        public static string ToString(List<Message> messages, bool textOnly)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Message msg in messages)
                if (textOnly)
                    sb.Append(msg.Text);
                else sb.Append(msg);
            return sb.ToString();
        }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append('\f');
			if (Audio.HasValue)
			{
				sb.Append('a');
				sb.Append(Audio.Value);
			}
			if (Voice.HasValue)
			{
				sb.Append('s');
				sb.Append(Voice.Value);
			}
			if (WaitTime.HasValue)
			{
				sb.Append('w');
				sb.Append(WaitTime.Value);
			}
			sb.Append(' ');
			if (Centered)
				sb.Append('\a');
			sb.Append(Text);
			return sb.ToString();
		}

		public string GetPreview()
		{
			int i = Text.IndexOf('\n');
			if (i != -1)
				return Text.Remove(i);
			return Text;
		}
	}

	public class SearchCriteria
	{
		public string Text { get; set; }
		public int? Audio { get; set; }
		public int? Voice { get; set; }
	}
}
