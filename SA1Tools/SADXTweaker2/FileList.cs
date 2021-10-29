using System;
using System.IO;
using System.Windows.Forms;

namespace SADXTweaker2
{
	public partial class FileList : Form
	{
		string directory, extension, selected;

		public FileList(string directory, string extension, string title)
		{
			InitializeComponent();
			this.directory = directory;
			this.extension = extension;
			Text = title;
			this.selected = string.Empty;
		}

		public FileList(string directory, string extension, string title, string selected)
			: this(directory, extension, title)
		{
			this.selected = selected;
		}

		private void FileList_Load(object sender, EventArgs e)
		{
			listBox1.BeginUpdate();
			foreach (string item in Directory.GetFiles(directory, "*." + extension))
				listBox1.Items.Add(Path.GetFileNameWithoutExtension(item));
			listBox1.EndUpdate();
			listBox1.SelectedIndex = Math.Max(listBox1.Items.IndexOf(selected), 0);
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		public string SelectedItem { get { return (string)listBox1.SelectedItem; } }
	}
}
