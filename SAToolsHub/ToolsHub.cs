using System;
using System.IO;
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
	public partial class SAToolsHub : Form
	{
		//Additional Windows
		private GamePaths gamePathsDiag;

		public SAToolsHub()
		{
			InitializeComponent();

			gamePathsDiag = new GamePaths();
		}

		//Tool Strip Functions
		//Settings
		private void setGamePathsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			gamePathsDiag.ShowDialog();
		}

		private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		//Projects
		private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void openProjectToolStripMenuItem1_Click(object sender, EventArgs e)
		{

		}

		private void editProjectInfoToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		//Project Build
		private void autoBuildToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void manualBuildToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void configureRunOptionsToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void buildRunGameToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}


		//Tools
		//General Tools Initializers
		private void sAMDLToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// launch samdl
			string samdlPath = "";

#if DEBUG
			samdlPath = Path.GetDirectoryName(Application.ExecutablePath) + "/../../../SAMDL/bin/Debug/SAMDL.exe";
#endif
#if !DEBUG
			samdlPath = Path.GetDirectoryName(Application.ExecutablePath) + "/../SAMDL/SAMDL.exe";
#endif

			Console.WriteLine(samdlPath);

			System.Diagnostics.ProcessStartInfo samdlStartInfo = new System.Diagnostics.ProcessStartInfo(
				Path.GetFullPath(samdlPath)//,
				/*Path.GetFullPath(projectFolder)*/);

			System.Diagnostics.Process samdlProcess = System.Diagnostics.Process.Start(samdlStartInfo);

		}

		private void sALVLToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// launch salvl
			string salvlPath = "";

#if DEBUG
			salvlPath = Path.GetDirectoryName(Application.ExecutablePath) + "/../../../SALVL/bin/Debug/SALVL.exe";
#endif
#if !DEBUG
			salvlPath = Path.GetDirectoryName(Application.ExecutablePath) + "/../SALVL/SALVL.exe";
#endif

			Console.WriteLine(salvlPath);

			System.Diagnostics.ProcessStartInfo salvlStartInfo = new System.Diagnostics.ProcessStartInfo(
				Path.GetFullPath(salvlPath)//,
				/*Path.GetFullPath(projectFolder)*/);

			System.Diagnostics.Process salvlProcess = System.Diagnostics.Process.Start(salvlStartInfo);
		}

		private void textureEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// launch TextureEditor
			string texEditPath = "";

#if DEBUG
			texEditPath = Path.GetDirectoryName(Application.ExecutablePath) + "/../../../TextureEditor/bin/Debug/TextureEditor.exe";
#endif
#if !DEBUG
			texEditPath = Path.GetDirectoryName(Application.ExecutablePath) + "/../TextureEditor/TextureEditor.exe";
#endif

			Console.WriteLine(texEditPath);

			System.Diagnostics.ProcessStartInfo texEditStartInfo = new System.Diagnostics.ProcessStartInfo(
				Path.GetFullPath(texEditPath)//,
				/*Path.GetFullPath(projectFolder)*/);

			System.Diagnostics.Process texEditProcess = System.Diagnostics.Process.Start(texEditStartInfo);
		}

		//SADX Tools Initializers
		private void sADXLVL2ToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void sADXTweakerToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void sADXsndSharpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// launch TextureEditor
			string sndSharpPath = "";

#if DEBUG
			sndSharpPath = Path.GetDirectoryName(Application.ExecutablePath) + "/../../../SADXsndSharp/bin/Debug/SADXsndSharp.exe";
#endif
#if !DEBUG
			sndSharpPath = Path.GetDirectoryName(Application.ExecutablePath) + "/../SADXsndSharp/SADXsndSharp.exe";
#endif

			Console.WriteLine(sndSharpPath);

			System.Diagnostics.ProcessStartInfo sndSharpStartInfo = new System.Diagnostics.ProcessStartInfo(
				Path.GetFullPath(sndSharpPath)//,
				/*Path.GetFullPath(projectFolder)*/);

			System.Diagnostics.Process sndSharpProcess = System.Diagnostics.Process.Start(sndSharpStartInfo);
		}

		private void sAFontEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// launch TextureEditor
			string saFontPath = "";

#if DEBUG
			saFontPath = Path.GetDirectoryName(Application.ExecutablePath) + "/../../../SADXFontEdit/bin/Debug/SADXFontEdit.exe";
#endif
#if !DEBUG
			sndSharpPath = Path.GetDirectoryName(Application.ExecutablePath) + "/../SADXFontEdit/SADXFontEdit.exe";
#endif

			Console.WriteLine(saFontPath);

			System.Diagnostics.ProcessStartInfo saFontStartInfo = new System.Diagnostics.ProcessStartInfo(
				Path.GetFullPath(saFontPath)//,
				/*Path.GetFullPath(projectFolder)*/);

			System.Diagnostics.Process saFontProcess = System.Diagnostics.Process.Start(saFontStartInfo);
		}

		private void sASaveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// launch SA Save
			string saSavePath = "";

#if DEBUG
			saSavePath = Path.GetDirectoryName(Application.ExecutablePath) + "/../../../SASave/bin/Debug/SASave.exe";
#endif
#if !DEBUG
			sndSharpPath = Path.GetDirectoryName(Application.ExecutablePath) + "/../SASave/SASave.exe";
#endif

			Console.WriteLine(saSavePath);

			System.Diagnostics.ProcessStartInfo saSaveStartInfo = new System.Diagnostics.ProcessStartInfo(
				Path.GetFullPath(saSavePath)//,
				/*Path.GetFullPath(projectFolder)*/);

			System.Diagnostics.Process saSaveProcess = System.Diagnostics.Process.Start(saSaveStartInfo);
		}

		//SA2 Tools Initializers
		private void sA2EventViewerToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		//Data Extractor/Convert (new Split UI)
		private void splitToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		//Help Links
		//Resources
		private void sAToolsWikiToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("https://github.com/sonicretro/sa_tools/wiki");
			}
			catch (Exception ex)
			{
				MessageBox.Show("Something went wrong, could not open link in browser.");
			}
		}

		private void retrosSCHGForSADXToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("https://info.sonicretro.org/SCHG:Sonic_Adventure_DX:_PC");
			}
			catch (Exception ex)
			{
				MessageBox.Show("Something went wrong, could not open link in browser.");
			}
		}

		private void retrosSCHGForSA2ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("https://info.sonicretro.org/SCHG:Sonic_Adventure_2_(PC)");
			}
			catch (Exception ex)
			{
				MessageBox.Show("Something went wrong, could not open link in browser.");
			}
		}

		private void sADXGitWikiToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("https://github.com/kellsnc/sadx-modding-guide/wiki");
			}
			catch (Exception ex)
			{
				MessageBox.Show("Something went wrong, could not open link in browser.");
			}
		}

		//GitHub Issue Tracker
		private void gitHubIssueTrackerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("https://github.com/sonicretro/sa_tools/issues");
			}
			catch (Exception ex)
			{
				MessageBox.Show("Something went wrong, could not open link in browser.");
			}
		}
	}
}
