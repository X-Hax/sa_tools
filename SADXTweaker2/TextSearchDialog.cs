using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SADXTweaker2
{
	public partial class TextSearchDialog : Form
	{
		ITextSearch searcher;

		public TextSearchDialog(ITextSearch searcher)
		{
			InitializeComponent();
			this.searcher = searcher;
		}

		private void searchText_TextChanged(object sender, EventArgs e)
		{
			searchButton.Enabled = searchText.TextLength > 0;
		}

		private void searchButton_Click(object sender, EventArgs e)
		{
			listBox1.Items.Clear();
			listBox1.Items.AddRange(searcher.GetSearchResults(searchText.Text));
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			okButton.Enabled = listBox1.SelectedIndex != -1;
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		public object Result { get { return listBox1.SelectedItem; } }
	}

	public interface ITextSearch
	{
		object[] GetSearchResults(string searchText);
	}
}
