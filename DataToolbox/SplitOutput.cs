using SA_Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SonicRetro.SAModel.DataToolbox
{
	public partial class SplitProgress : Form
	{
		public bool findAll = true;
		public bool pauseLog = false;
		public int splitMDL = 0; // 0 - not MDL, 1 - Little Endian, 2 - Big Endian
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

		public SplitProgress(List<string> log, List <string> files_a, string gamepath, string output_folder, bool findall, int splitMdl = 0)
		{
			InitializeComponent();
			logger = log;
			files = files_a;
			findAll = findall;
			game_path = gamepath;
			out_path = output_folder;
			splitMDL = splitMdl;
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
			if (splitMDL == 0)
			{
				Console.WriteLine("Starting batch split for " + files.Count.ToString() + " file(s)" + System.Environment.NewLine);
				foreach (string file in files)
				{
					SplitData splitdata = new SplitData();
					splitdata.dataFile = file;
					string folder_parent = Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)).ToString();
#if DEBUG
					folder_parent = Path.Combine(folder_parent, "Configuration");
					if (!Directory.Exists(Path.Combine(folder_parent, game_path))) folder_parent = Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)).ToString();
#endif
					List<string> inilist = FindRelevantINIFiles(file, Path.Combine(folder_parent, game_path));
					if (inilist.Count > 0)
					{
						foreach (string iniitem in inilist)
						{
							splitdata.iniFile = Path.Combine(folder_parent, game_path, iniitem);
							SplitDataList.Add(splitdata);
						}
					}
					else
					{
						splitdata.iniFile = Path.Combine(folder_parent, game_path, Path.GetFileNameWithoutExtension(file) + ".ini");
						SplitDataList.Add(splitdata);
					}
				}

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
						else if (Path.GetExtension(datafilename).ToLowerInvariant() == ".nb")
						{
							Console.WriteLine("Splitting NB file without INI data");
							inifilename = null;
						}
						else
						{
							Console.WriteLine(inifilename + " not found. Aborting.");
							continue;
						}
					}
					if (inifilename != null)
						Console.WriteLine("Using split data: " + inifilename);
					switch (Path.GetExtension(datafilename).ToLowerInvariant())
					{
						case ".dll":
							SA_Tools.SplitDLL.SplitDLL.SplitDLLFile(datafilename, inifilename, projectFolderName);
							break;
						case ".nb":
							SA_Tools.Split.SplitNB.SplitNBFile(datafilename, false, projectFolderName, 0, inifilename);
							break;
						default:
							SA_Tools.Split.Split.SplitFile(datafilename, inifilename, projectFolderName);
							break;
					}
				}
			}
			else
			{
				Console.WriteLine("Starting split for " + files[0] + System.Environment.NewLine);
				Console.WriteLine("Output folder: " + out_path + System.Environment.NewLine);
				SA_Tools.SAArc.sa2MDL.Split(splitMDL > 1, files[0], out_path, files.Skip(1).ToArray());
			}
			Console.WriteLine("Split job finished.");
		}

		private List<string> FindRelevantINIFiles(string file, string inilocation)
		{
			bool dllmode = false;
			string extension = Path.GetExtension(file).ToLowerInvariant();
			List<string> relevantini = new List<string>();
			if (extension == ".dll")
				dllmode = true;
			else if (extension == ".nb")
				return relevantini;
			if (findAll)
			{
				Console.WriteLine("Finding relevant split INI files for {0} in {1}", Path.GetFileName(file), inilocation);
				string[] inifiles = System.IO.Directory.GetFiles(inilocation, "*.ini", SearchOption.AllDirectories);
				for (int u = 0; u < inifiles.Length; u++)
				{
					if (inifiles[u].ToLowerInvariant().Contains("_data") || inifiles[u].ToLowerInvariant().Contains("output")) continue;
					if (dllmode)
					{
						SA_Tools.SplitDLL.IniData inifile = IniSerializer.Deserialize<SA_Tools.SplitDLL.IniData>(inifiles[u]);
						if (inifile.ModuleName != null && inifile.ModuleName.ToLowerInvariant() == Path.GetFileNameWithoutExtension(file).ToLowerInvariant())
						{
							relevantini.Add(Path.GetFullPath(inifiles[u]));
							Console.WriteLine("Found split file {0}", inifiles[u]);
						}
					}
					else
					{
						SA_Tools.IniData inifile = IniSerializer.Deserialize<SA_Tools.IniData>(inifiles[u]);
						if (inifile.DataFilename != null && inifile.DataFilename.ToLowerInvariant() == Path.GetFileName(file).ToLowerInvariant())
						{
							relevantini.Add(Path.GetFullPath(inifiles[u]));
							Console.WriteLine("Found split file {0}", inifiles[u]);
						}
						else if (inifile.DataFilename != null) Console.WriteLine("Datafilename: {0}", inifile.DataFilename);
						else Console.WriteLine("Datafilename in {0} is null", inifiles[u]);
					}
				}
			}
			return relevantini;
		}

	private void SplitProgress_Shown(object sender, EventArgs e)
		{
			SplitDataList = new List<SplitData>();
			writer = new TextBoxWriter(txtConsole);
			Console.SetOut(writer);
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