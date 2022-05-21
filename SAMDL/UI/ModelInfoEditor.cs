using SplitTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SAModel.SAMDL.UI
{
	public partial class ModelInfoEditor : Form
	{
		public string ModelAuthor { get; set; }
		public string ModelDescription { get; set; }
		public string[] ModelAnimations { get; set; }
	
		private readonly string ModelFilename;

		public ModelInfoEditor()
		{
			InitializeComponent();
		}

		public ModelInfoEditor(NJS_OBJECT model, string author = null, string desc = null, string modelFilename = null)
		{
			InitializeComponent();
			ModelFilename = modelFilename;
			if (!string.IsNullOrEmpty(author))
				textBoxAuthor.Text = author;
			if (!string.IsNullOrEmpty(desc))
				textBoxDescription.Text = desc;
			if (!string.IsNullOrEmpty(ModelFilename) && File.Exists(Path.ChangeExtension(ModelFilename, ".action")))
			{
				string[] animfiles = File.ReadAllLines(Path.ChangeExtension(ModelFilename, ".action"));
				listBoxAnimations.Items.Clear();
				foreach (string animname in animfiles)
					listBoxAnimations.Items.Add(animname);
			}
		}

		private void textBoxAuthor_TextChanged(object sender, EventArgs e)
		{
			ModelAuthor = textBoxAuthor.Text;
		}

		private void textBoxDescription_TextChanged(object sender, EventArgs e)
		{
			ModelDescription = textBoxDescription.Text;
		}

		private void listBoxAnimations_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBoxAnimations.SelectedIndex != -1)
				textBoxAnimation.Text = listBoxAnimations.SelectedItem.ToString();
		}

		private void buttonAddAnimation_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(textBoxAnimation.Text))
			{
				if (listBoxAnimations.Items.Contains(textBoxAnimation.Text))
				{
					MessageBox.Show(this, "Cannot add duplicate animation name.", "Metadata Editor", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				else
				{
					listBoxAnimations.Items.Add(textBoxAnimation.Text);
					listBoxAnimations.SelectedItem = listBoxAnimations.Items[listBoxAnimations.Items.Count - 1];
				}
			}
			else
				MessageBox.Show(this, "No animation name specified.", "Metadata Editor", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			buttonRemoveAnimation.Enabled = listBoxAnimations.Items.Count > 0;
		}

		private void buttonRemoveAnimation_Click(object sender, EventArgs e)
		{
			int index = listBoxAnimations.SelectedIndex;
			listBoxAnimations.Items.RemoveAt(listBoxAnimations.SelectedIndex);
			if (listBoxAnimations.Items.Count > 0)
				listBoxAnimations.SelectedItem = index != 0 ? listBoxAnimations.Items[listBoxAnimations.Items.Count - 1] : listBoxAnimations.Items[0];
			buttonRemoveAnimation.Enabled = listBoxAnimations.Items.Count > 0;
		}

		private void buttonApplyAnimation_Click(object sender, EventArgs e)
		{
			List<string> animlist = new List<string>();
			for (int i = 0; i < listBoxAnimations.Items.Count; i++)
				animlist.Add(listBoxAnimations.Items[i].ToString());
			ModelAnimations = animlist.ToArray();
		}

		private void buttonClearAnimations_Click(object sender, EventArgs e)
		{
			listBoxAnimations.Items.Clear();
			buttonRemoveAnimation.Enabled = false;
		}
	}
}