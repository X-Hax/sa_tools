using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAToolsHub
{
	public partial class editProj : Form
	{
		public editProj(int mode)
		{
			InitializeComponent();

			if (mode == 1)
			{
				btnCreateSave.Text = ("Save Project");
				groupBox1.SuspendLayout();
			}


		}
	}
}
