using System;
using System.Windows.Forms;

namespace SAModel.SAEditorCommon.UI
{
	public partial class PointToDialog : Form
	{
		private Vertex clickPoint, centerPoint;

		public PointToDialog(Vertex clickPoint, Vertex centerPoint)
		{
			InitializeComponent();
			this.clickPoint = clickPoint;
			this.centerPoint = centerPoint;
		}

		private void PointToDialog_Load(object sender, EventArgs e)
		{
			radioButton1.Text += " (" + clickPoint + ")";
			radioButton2.Text += " (" + centerPoint + ")";
		}

		public Vertex SelectedLocation
		{
			get
			{
				if (radioButton1.Checked)
					return clickPoint;
				else
					return centerPoint;
			}
		}
	}
}
