using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SonicRetro.SAModel.DataToolbox
{
	public partial class SplitProgress : Form
	{
		bool pauseLog = false;
		public TextBoxWriter writer;
		public List<SplitData> SplitDataList;
		public struct SplitData
		{
			public string dataFile;
			public string iniFile;
		}

		public List<string> logger;
		public List<string> files;
		public string game_path;
		public string out_path;

		public SplitProgress(List<string> log, List <string> files_a, string gamepath, string output_folder)
		{
			InitializeComponent();
			this.logger = log;
			this.files = files_a;
			game_path = gamepath;
			out_path = output_folder;
		}

		private void buttonCloseSplitProgress_Click(object sender, EventArgs e)
		{
			if (backgroundWorker1.IsBusy) backgroundWorker1.CancelAsync();
			Close();
		}

		private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			string datafilename;
			string inifilename;
			string projectFolderName;
			Console.WriteLine("Starting batch split for " + files.Count.ToString() + " file(s)" + System.Environment.NewLine);
			foreach (SplitData CurrentSplitData in SplitDataList)
			{
				datafilename = CurrentSplitData.dataFile;
				inifilename = CurrentSplitData.iniFile;

				Console.WriteLine("Splitting file: " + datafilename);
				if (out_path == "") projectFolderName = Path.GetDirectoryName(datafilename);
				else projectFolderName = out_path;

				if (projectFolderName[projectFolderName.Length - 1] != '/') projectFolderName = string.Concat(projectFolderName, '/');
				Console.WriteLine("Output folder: " + projectFolderName);

				if (!File.Exists(datafilename))
				{
					Console.WriteLine(datafilename + " not found. Aborting.");
					continue;
				}

				if (!Directory.Exists(projectFolderName))
				{
					// try creating the directory
					bool created = true;

					try
					{
						// check to see if trailing charcter closes 
						Directory.CreateDirectory(projectFolderName);
					}
					catch
					{
						created = false;
					}

					if (!created)
					{
						// couldn't create directory.
						Console.WriteLine("Output folder did not exist and couldn't be created.");
						continue;
					}
				}

				if (!File.Exists(inifilename))
				{

					if (inifilename.Length > 9 && File.Exists(inifilename.Substring(0, inifilename.Length - 9) + ".ini"))
					{
						inifilename = inifilename.Substring(0, inifilename.Length - 9) + ".ini";
					}
					else
					{
						Console.WriteLine(inifilename + " not found. Aborting.");
						continue;
					}
				}

				Console.WriteLine("Using split data: " + inifilename);

				// switch on file extension - if dll, use dll splitter
				System.IO.FileInfo fileInfo = new System.IO.FileInfo(datafilename);

				int result = (fileInfo.Extension.ToLower().Contains("dll")) ? SA_Tools.SplitDLL.SplitDLL.SplitDLLFile(datafilename, inifilename, projectFolderName) :
					SA_Tools.Split.Split.SplitFile(datafilename, inifilename, projectFolderName);
			}
			Console.WriteLine("Batch split finished.");
		}

				

	private void SplitProgress_Shown(object sender, EventArgs e)
		{
			SplitDataList = new List<SplitData>();
			writer = new TextBoxWriter(txtConsole);
			Console.SetOut(writer);
			foreach (string file in files)
			{
				SplitData splitdata = new SplitData();
				splitdata.dataFile = file;
				string folder_parent = Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)).ToString();
#if DEBUG
				folder_parent = Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)).ToString()).ToString()).ToString(), "Configuration");
#endif
				splitdata.iniFile = Path.Combine(folder_parent, game_path, Path.GetFileNameWithoutExtension(file) + ".ini");
				SplitDataList.Add(splitdata);
			}
			buttonCloseSplitProgress.Text = "Cancel";
			backgroundWorker1.RunWorkerAsync();

		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			if (writer != null && !pauseLog) writer.WriteOut();
		}

		private void checkboxPause_CheckedChanged(object sender, EventArgs e)
		{
			pauseLog = checkboxPause.Checked;
			buttonCloseSplitProgress.Text = "Close";
		}

		private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			buttonCloseSplitProgress.Text = "Close";
		}
	}
}