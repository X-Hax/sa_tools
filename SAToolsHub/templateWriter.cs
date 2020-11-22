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

					switch (textBox1.Text)
					{
						case "SA2PC":
							SplitInfo sa2pcInfo = new SplitInfo { GameName = textBox1.Text, GameSystemFolder = "", DataFolder = "SA2PC" };
							splitTemplateFile.GameInfo = sa2pcInfo;
							splitTemplateFile.SplitEntries = BuildSplits.sa2pc_split;
							splitTemplateFile.SplitMDLEntries = BuildSplits.sa2pc_mdlsplit;
							break;
						case "SA2":
							SplitInfo sa2Info = new SplitInfo { GameName = textBox1.Text, GameSystemFolder = "", DataFolder = "SA2" };
							splitTemplateFile.GameInfo = sa2Info;
							splitTemplateFile.SplitEntries = NonBuildSplits.sa2_final_split;
							splitTemplateFile.SplitMDLEntries = NonBuildSplits.sa2_final_mdlsplit;
							break;
						case "SA2TT":
							SplitInfo sa2ttInfo = new SplitInfo { GameName = textBox1.Text, GameSystemFolder = "", DataFolder = "SA2TheTrial" };
							splitTemplateFile.GameInfo = sa2ttInfo;
							splitTemplateFile.SplitEntries = NonBuildSplits.sa2_trial_split;
							splitTemplateFile.SplitMDLEntries = NonBuildSplits.sa2_trial_mdlsplit;
							break;
						default:
							break;
					}

					serializer.Serialize(writer, splitTemplateFile);
					projFileStream.Close();
				}
			}
		}
	}
}
