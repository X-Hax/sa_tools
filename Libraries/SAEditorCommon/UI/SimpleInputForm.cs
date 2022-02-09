using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAModel.SAEditorCommon.UI
{
	public partial class SimpleInputForm : Form
	{
		public string outputText { get; set; }
		public bool useOK = false;

		public SimpleInputForm(string label = "Input", string window = "Input Window")
		{
			InitializeComponent();
			this.Text = window;
			lblText.Text = label;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (txtBox.Text != null || txtBox.Text != "")
				outputText = txtBox.Text;
			else
				outputText = "";
			useOK = true;
			this.Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			useOK = false;
			this.Close();
		}
	}
}
