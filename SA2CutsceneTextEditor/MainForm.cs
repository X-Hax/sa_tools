using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FraGag.Compression;
using SA_Tools;

namespace SA2CutsceneTextEditor
{
	public partial class MainForm : Form
	{
		public MainForm(string[] args)
		{
			InitializeComponent();
			if (args.Length > 0)
				filename = Path.GetFullPath(args[0]);
		}

		Scene CurrentScene { get { return scenes[sceneNum.SelectedIndex]; } }
		Message CurrentMessage { get { return CurrentScene.Messages[messageNum.SelectedIndex]; } }

		static readonly string settingsPath = Path.ChangeExtension(Application.ExecutablePath, ".ini");
		static readonly Encoding jpenc = Encoding.GetEncoding(932);
		static readonly Encoding euenc = Encoding.GetEncoding(1252);
		List<string> recentFiles = new List<string>();
		string filename = null;
		List<Scene> scenes = new List<Scene>();
		bool bigEndian;
		bool useSJIS;
		SearchCriteria currentSearch;
		int searchScene, searchMessage;

		private void MainForm_Load(object sender, EventArgs e)
		{
			if (File.Exists(settingsPath))
			{
				Settings settings = Settings.Load(settingsPath);
				if (settings.RecentFiles != null)
					recentFiles = new List<string>(settings.RecentFiles.Where(a => File.Exists(a)));
				if (recentFiles.Count > 0)
					UpdateRecentFiles();
				bigEndian = settings.BigEndian;
				bigEndianGCSteamToolStripMenuItem.Checked = bigEndian;
				useSJIS = settings.UseSJIS;
				shiftJISToolStripMenuItem.Checked = useSJIS;
				windows1252ToolStripMenuItem.Checked = !useSJIS;
			}
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
			switch (MessageBox.Show(this, "Do you want to save before exiting?", "SA2 Cutscene Text Editor", MessageBoxButtons.YesNoCancel, MessageBoxIcon.None, MessageBoxDefaultButton.Button3))
			{
				case DialogResult.Yes:
					break;
				case DialogResult.Cancel:
					e.Cancel = true;
					return;
			}
			new Settings() { RecentFiles = recentFiles, BigEndian = bigEndian, UseSJIS = useSJIS }.Save(settingsPath);
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			scenes.Clear();
			sceneNum.Items.Clear();
			findNextToolStripMenuItem.Enabled = false;
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "prs", Filter = "Supported Files|*.prs;*.bin|All Files|*.*" })
				if (dlg.ShowDialog(this) == DialogResult.OK)
					LoadFile(dlg.FileName);
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
			useSJIS = Path.GetFileNameWithoutExtension(filename).Last() == '0';
			shiftJISToolStripMenuItem.Checked = useSJIS;
			windows1252ToolStripMenuItem.Checked = !useSJIS;
			Encoding encoding = useSJIS ? jpenc : euenc;
			scenes.Clear();
			int address = 0;
			int id = ByteConverter.ToInt32(fc, 0);
			while (id != -1)
			{
				scenes.Add(new Scene(fc, address, encoding));
				address += Scene.Size;
				id = ByteConverter.ToInt32(fc, address);
			}
			UpdateSceneSelect();
			scenePanel.Enabled = false;
			AddRecentFile(filename);
			Text = "SA2 Cutscene Text Editor - " + Path.GetFileName(filename);
			findNextToolStripMenuItem.Enabled = false;
		}

		private void UpdateSceneSelect()
		{
			sceneNum.Items.Clear();
			if (scenes.Count > 0)
				sceneNum.Items.AddRange(scenes.Select(a => a.GetPreview()).ToArray());
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
					Text = "SA2 Cutscene Text Editor - " + Path.GetFileName(filename);
				}
			}
		}

		private void SaveFile()
		{
			ByteConverter.BigEndian = bigEndian;
			uint addr = (uint)((scenes.Count + 1) * Scene.Size) + 0x817AFE60u;
			List<byte> fc = new List<byte>();
			Encoding encoding = useSJIS ? jpenc : euenc;
			foreach (Scene scene in scenes)
			{
				fc.AddRange(ByteConverter.GetBytes(scene.SceneNumber));
				fc.AddRange(ByteConverter.GetBytes(addr));
				fc.AddRange(ByteConverter.GetBytes(scene.Messages.Count));
				addr += (uint)(scene.Messages.Count * Message.Size);
			}
			fc.AddRange(Enumerable.Repeat(byte.MaxValue, 4));
			fc.AddRange(Enumerable.Repeat(byte.MinValue, 8));
			System.Diagnostics.Debug.Assert(fc.Count == (scenes.Count + 1) * Scene.Size);
			foreach (Scene scene in scenes)
				foreach (Message msg in scene.Messages)
				{
					fc.AddRange(ByteConverter.GetBytes(msg.Character));
					fc.AddRange(ByteConverter.GetBytes(addr));
					addr += (uint)(encoding.GetByteCount(msg.Text) + 1);
				}
			foreach (Scene scene in scenes)
				foreach (Message msg in scene.Messages)
				{
					fc.AddRange(encoding.GetBytes(msg.Text));
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
						foreach (Scene scn in scenes)
						{
							foreach (string msg in scn.Messages.Select(a => a.Text))
								sw.WriteLine(msg.Replace("\n", Environment.NewLine));
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

		private void sceneNum_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (sceneNum.SelectedIndex == -1)
				scenePanel.Enabled = sceneRemoveButton.Enabled = false;
			else
			{
				scenePanel.Enabled = sceneRemoveButton.Enabled = true;
				sceneID.Value = CurrentScene.SceneNumber;
				UpdateMessageSelect();
				messagePanel.Enabled = false;
			}
		}

		private void UpdateMessageSelect()
		{
			messageNum.Items.Clear();
			messageNum.Items.AddRange(CurrentScene.Messages.Select((a, i) => (object)((i + 1).ToString() + ": " + a.GetPreview())).ToArray());
		}

		private void sceneAddButton_Click(object sender, EventArgs e)
		{
			scenes.Add(new Scene());
			sceneNum.Items.Add("0: ");
			sceneNum.SelectedIndex = sceneNum.Items.Count - 1;
		}

		private void sceneRemoveButton_Click(object sender, EventArgs e)
		{
			int scn = sceneNum.SelectedIndex;
			scenes.RemoveAt(scn);
			if (scn == scenes.Count)
				--scn;
			UpdateSceneSelect();
			sceneNum.SelectedIndex = scn;
		}

		private void sceneID_ValueChanged(object sender, EventArgs e)
		{
			CurrentScene.SceneNumber = (int)sceneID.Value;
		}

		private void messageNum_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (messageNum.SelectedIndex == -1)
				messagePanel.Enabled = messageRemoveButton.Enabled = false;
			else
			{
				messagePanel.Enabled = true;
				messageRemoveButton.Enabled = CurrentScene.Messages.Count > 1;
				messageCharacter.Value = CurrentMessage.Character;
				messageEdit.Text = CurrentMessage.Text.Replace("\n", Environment.NewLine);
			}
		}

		private void messageAddButton_Click(object sender, EventArgs e)
		{
			CurrentScene.Messages.Add(new Message());
			messageNum.Items.Add((messageNum.Items.Count + 1) + ": ");
			messageNum.SelectedIndex = messageNum.Items.Count - 1;
		}

		private void messageRemoveButton_Click(object sender, EventArgs e)
		{
			int message = messageNum.SelectedIndex;
			CurrentScene.Messages.RemoveAt(message);
			if (message == CurrentScene.Messages.Count)
				--message;
			UpdateMessageSelect();
			messageNum.SelectedIndex = message;
		}

		private void messageCharacter_ValueChanged(object sender, EventArgs e)
		{
			CurrentMessage.Character = (int)messageCharacter.Value;
		}

		private void messageEdit_TextChanged(object sender, EventArgs e)
		{
			CurrentMessage.Text = messageEdit.Text.Replace(Environment.NewLine, "\n");
		}

		private void findToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (FindDialog dlg = new FindDialog())
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					currentSearch = dlg.GetSearchCriteria();
					searchScene = 0;
					searchMessage = 0;
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
			for (int scn = searchScene; scn < scenes.Count; scn++)
			{
				for (int msg = searchMessage; msg < scenes[scn].Messages.Count; msg++)
				{
					if (currentSearch.Character.HasValue && currentSearch.Character != scenes[scn].Messages[msg].Character)
						continue;
					if (currentSearch.Text != null && !scenes[scn].Messages[msg].Text.Contains(currentSearch.Text))
						continue;
					sceneNum.SelectedIndex = scn;
					messageNum.SelectedIndex = msg;
					searchScene = scn;
					searchMessage = msg + 1;
					if (searchMessage == scenes[scn].Messages.Count)
					{
						searchScene++;
						searchMessage = 0;
					}
					return;
				}
				searchMessage = 0;
			}
			MessageBox.Show(this, "Results not found.", "SA2 Cutscene Text Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
			findNextToolStripMenuItem.Enabled = false;
		}
	}

	public class Scene
	{
		public int SceneNumber { get; set; }
		public List<Message> Messages { get; set; } = new List<Message>();

		public static int Size => 12;

		public Scene() { }

		public Scene(byte[] file, int address, Encoding encoding)
		{
			SceneNumber = ByteConverter.ToInt32(file, address);
			int ptr = (int)(ByteConverter.ToUInt32(file, address + 4) - 0x817AFE60u);
			int cnt = ByteConverter.ToInt32(file, address + 8);
			Messages.Capacity = cnt;
			for (int i = 0; i < cnt; i++)
			{
				Messages.Add(new Message(file, ptr, encoding));
				ptr += Message.Size;
			}
		}

		public string GetPreview()
		{
			return SceneNumber + ": " + Messages.FirstOrDefault(a => !string.IsNullOrEmpty(a.Text))?.GetPreview() ?? string.Empty;
		}
	}

	public class Message
	{
		public int Character { get; set; }
		public string Text { get; set; } = string.Empty;

		public static int Size => 8;

		public Message() { }

		public Message(byte[] file, int address, Encoding encoding)
		{
			Character = ByteConverter.ToInt32(file, address);
			Text = file.GetCString((int)(ByteConverter.ToUInt32(file, address + 4) - 0x817AFE60u), encoding);
		}

		public string GetPreview()
		{
			int i = Text.IndexOf('\n');
			if (i != -1)
				return Text.Remove(i).TrimStart('\a');
			return Text.TrimStart('\a');
		}
	}

	public class SearchCriteria
	{
		public string Text { get; set; }
		public int? Character { get; set; }
	}

	public class Settings
	{
		[IniAlwaysInclude]
		public bool BigEndian { get; set; }
		[IniAlwaysInclude]
		public bool UseSJIS { get; set; }
		[IniCollection(IniCollectionMode.SingleLine, Format = ",")]
		public List<string> RecentFiles { get; set; } = new List<string>();

		public static Settings Load(string filename)
		{
			return IniSerializer.Deserialize<Settings>(filename);
		}

		public void Save(string filename)
		{
			IniSerializer.Serialize(this, filename);
		}
	}
}
