using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;

namespace StructConverter
{
	public partial class NewProject : Form
	{
		// todo: formclosed for this form doesn't end the program.
		List<char> invalidChars;
		bool hidden = false;

		public NewProject()
		{
			InitializeComponent();

			invalidChars = new List<char>(Path.GetInvalidPathChars());
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Hide();
		}

		private void browseButton_Click(object sender, EventArgs e)
		{
			DialogResult dialogResult = folderBrowserDialog1.ShowDialog();

			if (dialogResult == System.Windows.Forms.DialogResult.OK)
			{
				gamePathTextBox.Text = folderBrowserDialog1.SelectedPath;
			}

			SetContinue();
		}

		/// <summary>
		/// Activates the OK button, if it is valid to do so.
		/// </summary>
		private void SetContinue()
		{
			if (Directory.Exists(gamePathTextBox.Text) && projectNameLabel.Text.Length > 0 && ProjectPathValid()) // todo: this needs to check and see if there are any invalid folder characters in the project name.
			{
				acceptButton.Enabled = true;
			}
		}

		private bool ProjectPathValid()
		{
			foreach (char currentCharacter in projectNameLabel.Text)
			{
				if (invalidChars.Contains(currentCharacter)) return false;
			}

			return true;
		}

		private void acceptButton_Click(object sender, EventArgs e)
		{
			// validate our selection before continuing.
			if (Directory.Exists(gamePathTextBox.Text))
			{
				if (SADXRadioButton.Checked)
				{
					//we need to detect if our game path is really a game path
					// look for Sonic.exe
					string sonicExePath = Path.Combine(gamePathTextBox.Text, "sonic.exe");

					if (!File.Exists(sonicExePath)) MessageBox.Show("No sonic.exe found - are you sure this is a real game folder?");

					// look for modloader specific files
					string modLoaderDLLPath = string.Concat(gamePathTextBox.Text, "\\mods\\SADXModLoader.dll");
					if (File.Exists(modLoaderDLLPath))
					{
						// look for system folder
						string systemFolder = string.Concat(gamePathTextBox.Text, "\\system\\");
						if (Directory.Exists(systemFolder))
						{
							if (File.Exists(string.Concat(gamePathTextBox.Text, "\\split.exe")))
							{
								if (File.Exists(string.Concat(gamePathTextBox.Text, "\\splitDLL.exe")))
								{
									// emulate splitall.bat's behavior here.

									// move sadxlvl.ini to the proper folder
								}
								else MessageBox.Show("Error - couldn't find splitDLL.exe in game folder.");
							}
							else MessageBox.Show("Error - couldn't find split.exe in game folder.");
						}
						else MessageBox.Show("Error - no system folder found.");
					}
					else MessageBox.Show("Error - Modloader not installed.");
				}
				else if (SA2PCButton.Checked)
				{

				}
			}
			else MessageBox.Show("Game directory is invalid!");
		}

		private void NewProject_FormClosed(object sender, FormClosedEventArgs e)
		{
			int deb = 0; // this does get closed on hiding, not just being closed
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			// todo: check to see if project name entered exists already. Show a warning if it does.

			SetContinue();
		}
	}
}
