using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SADXLVL2
{
	public partial class LevelSelectDialog : Form
	{
		/// <summary>
		/// Gets the "tag" for the selected stage. (e.g "Action Stages\Emerald Coast")
		/// </summary>
		public string SelectedStage { get; private set; }

		public bool skipobjdefs { get; private set; }
		private readonly Dictionary<string, List<string>> levels;

		// TODO: Add parameter to select last loaded stage on open.

		/// <summary>
		/// Initializes a LevelSelect dialog with a dropdown list of categories and a list of levels below.
		/// </summary>
		/// <param name="levels">A dictionary containing the list of levels to populate the dialog with.</param>
		public LevelSelectDialog(Dictionary<string, List<string>> levels)
		{
			InitializeComponent();

			this.levels = levels;
			
			foreach (var i in this.levels)
				comboCategories.Items.Add(i.Key);
			
			comboCategories.SelectedIndex = 0;

		}

		private void listStages_SelectedIndexChanged(object sender, EventArgs e)
		{
			buttonOK.Enabled = (listStages.SelectedIndices.Count > 0);
		}

		private void comboCategories_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox combo = (ComboBox)sender;
			string selectedText = combo.GetItemText(combo.SelectedItem);

			listStages.BeginUpdate();

			if (levels.ContainsKey(selectedText))
			{
				listStages.Items.Clear();
				foreach (string s in levels[selectedText])
					listStages.Items.Add(s);
			}

			listStages.EndUpdate();
		}

		private void listStages_DoubleClick(object sender, EventArgs e)
		{
			if (listStages.SelectedIndices.Count > 0)
				buttonOK.PerformClick();
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			SelectedStage = comboCategories.GetItemText(comboCategories.SelectedItem) + '\\' + listStages.GetItemText(listStages.SelectedItem);
			skipobjdefs = checkBox_SkipDefs.Checked;
		}
	}
}
