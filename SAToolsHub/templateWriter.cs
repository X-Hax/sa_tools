using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using SA_Tools;
using Fclp.Internals.Extensions;
using ProjectManagement;
using SA_Tools.Split;

namespace SAToolsHub
{
	public partial class templateWriter : Form
	{
		public templateWriter()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Stream projFileStream;
			SaveFileDialog saveFileDialog1 = new SaveFileDialog();
			saveFileDialog1.Filter = "Template File (*.xml)|*.xml";
			saveFileDialog1.RestoreDirectory = true;

			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if ((projFileStream = saveFileDialog1.OpenFile()) != null)
				{
					XmlSerializer serializer = new XmlSerializer(typeof(SplitTemplate));
					TextWriter writer = new StreamWriter(projFileStream);

					SplitTemplate splitTemplateFile = new SplitTemplate();

					serializer.Serialize(writer, splitTemplateFile);
					projFileStream.Close();
				}
			}
		}
	}
}
