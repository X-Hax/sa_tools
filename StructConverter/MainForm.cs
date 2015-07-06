using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StructConverter
{
	public partial class MainForm : Form
	{
		NewProject newProjectForm;

		public MainForm()
		{
			InitializeComponent();

			newProjectForm = new NewProject();
		}

		#region New Project methods
		private void NewProjectButton_Click(object sender, EventArgs e)
		{
			this.Hide();
			DialogResult dialogResult = newProjectForm.ShowDialog();
			if (dialogResult == System.Windows.Forms.DialogResult.Cancel) this.Show();
		}
		#endregion

		#region Build Existing Methods
		private void buildExistingButton_Click(object sender, EventArgs e)
		{
			ModBuilder newModbuilderForm = new ModBuilder();
			this.Hide();
			newModbuilderForm.ShowDialog();
			this.Close();
		}
		#endregion
	}
}
