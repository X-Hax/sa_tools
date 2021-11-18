using SplitTools;
using SAModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SADXTweaker2
{
	public partial class ChaoMessageFileEditor : Form
	{
		static readonly string[] langs = { "J", "E", "F", "S", "G" };
		static readonly string filepattern = "{0}_{1}.BIN";

		string CurrentFile
		{
			get { return string.Format(filepattern, filename.SelectedItem, langs[language.SelectedIndex]); }
		}

		List<string> Messages = new List<string>();

		string CurrentMessage { get { return Messages[messageNum.SelectedIndex]; } set { Messages[messageNum.SelectedIndex] = value; } }

		public ChaoMessageFileEditor()
		{
			InitializeComponent();
		}

		private void ChaoMessageFileEditor_Load(object sender, EventArgs e)
		{
			filename.SelectedIndex = 0;
			language.SelectedIndex = 1;
		}

		private void field_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (filename.SelectedIndex == -1 | language.SelectedIndex == -1) return;
			loadButton.Enabled = File.Exists(Path.Combine(Program.project.GameInfo.ProjectFolder, Program.project.GameInfo.GameDataFolder, CurrentFile)) || File.Exists(Path.Combine(SAModel.SAEditorCommon.ProjectManagement.ProjectFunctions.GetGamePath(Program.project.GameInfo.GameName), Program.project.GameInfo.GameDataFolder, CurrentFile));
		}

		private void loadButton_Click(object sender, EventArgs e)
		{
			Messages = new List<string>();
			byte[] file;
			if (File.Exists(Path.Combine(Program.project.GameInfo.ProjectFolder, Program.project.GameInfo.GameDataFolder, CurrentFile)))
				file = File.ReadAllBytes(Path.Combine(Program.project.GameInfo.ProjectFolder, Program.project.GameInfo.GameDataFolder, CurrentFile));
			else
				file = File.ReadAllBytes(Path.Combine(SAModel.SAEditorCommon.ProjectManagement.ProjectFunctions.GetGamePath(Program.project.GameInfo.GameName), Program.project.GameInfo.GameDataFolder, CurrentFile));
			int address = 0;
			int off = ReadInt32BE(file, 0);
			while (off != -1)
			{
				Messages.Add(file.GetCString(off, HelperFunctions.GetEncoding((Languages)language.SelectedIndex)));
				address += 4;
				off = ReadInt32BE(file, address);
			}
			messageNum.Items.Clear();
			if (Messages.Count > 0)
			{
				messageNum.Items.AddRange(Array.ConvertAll(Enumerable.Range(1, Messages.Count).ToArray(), (a) => (object)a));
				messageNum.SelectedIndex = 0;
			}
		}

		private int ReadInt32BE(byte[] data, int offset)
		{
			byte[] val = new byte[4];
			Array.Copy(data, offset, val, 0, 4);
			Array.Reverse(val);
			return BitConverter.ToInt32(val, 0);
		}

		private void saveButton_Click(object sender, EventArgs e)
		{
			int baseaddr = (Messages.Count + 1) * 4;
			List<byte> file = new List<byte>(baseaddr);
			List<byte> strings = new List<byte>();
			foreach (string item in Messages)
			{
				file.AddRange(Int32BEToBytes(baseaddr + strings.Count));
				strings.AddRange(HelperFunctions.GetEncoding((Languages)language.SelectedIndex).GetBytes(item));
				strings.Add(0);
				int off = file.Count % 4;
				if (off != 0)
					strings.AddRange(new byte[4 - off]);
			}
			file.AddRange(BitConverter.GetBytes(-1));
			file.AddRange(strings);
			File.WriteAllBytes(Path.Combine(Program.project.GameInfo.ProjectFolder, Program.project.GameInfo.GameDataFolder, CurrentFile), file.ToArray());
		}

		private byte[] Int32BEToBytes(int value)
		{
			byte[] result = BitConverter.GetBytes(value);
			Array.Reverse(result);
			return result;
		}

		private void messageNum_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (messageNum.SelectedIndex == -1)
				line.Enabled = messageRemoveButton.Enabled = false;
			else
			{
				line.Enabled = messageRemoveButton.Enabled = true;
				line.Text = CurrentMessage.Replace("\n", Environment.NewLine);
			}
		}

		private void messageAddButton_Click(object sender, EventArgs e)
		{
			Messages.Add(string.Empty);
			messageNum.Items.Add(messageNum.Items.Count + 1);
			messageNum.SelectedIndex = messageNum.Items.Count - 1;
		}

		private void messageRemoveButton_Click(object sender, EventArgs e)
		{
			Messages.RemoveAt(messageNum.SelectedIndex);
			messageNum.Items.RemoveAt(messageNum.Items.Count - 1);
			messageNum_SelectedIndexChanged(messageRemoveButton, EventArgs.Empty);
		}

		private void line_TextChanged(object sender, EventArgs e)
		{
			CurrentMessage = line.Text.Replace(Environment.NewLine, "\n");
		}
	}
}