using SAModel.SAEditorCommon.ProjectManagement;
using SplitTools;
using SplitTools.Split;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SAModel.DataToolbox
{
	public partial class SplitProgress : Form
	{
        private Templates.SplitTemplate splitTemplate;
        private string templateFolder;

        public bool pauseLog = false;
		public int splitMDL = 0; // 0 - not MDL, 1 - Little Endian, 2 - Big Endian
        public int labelMode = 0; // 0 - Address labels, 1 - Full labels, 2 - Strip labels
		public TextBoxWriter writer;
		public List<SplitData> splitDataList;
		public struct SplitData
		{
			public string dataFile;
			public string iniFile;
		}

		public List<string> logger;
		public List<string> files;
		public string out_path;

		public SplitProgress(List<string> log, List <string> files_a, Templates.SplitTemplate template, string output_folder, int splitMdl = 0, int labelmode = 1)
		{
			InitializeComponent();
			logger = log;
			files = files_a;
            splitTemplate = template;
            out_path = output_folder;
			splitMDL = splitMdl;
            labelMode = labelmode;
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
			SplitFlags splitFlags = SplitFlags.Log | SplitFlags.Overwrite;
			if (splitMDL == 0)
			{
				Console.WriteLine("Starting batch split for " + files.Count.ToString() + " file(s)" + System.Environment.NewLine);
                foreach (string file in files)
                {
                    SplitData splitdata = new SplitData();
                    splitdata.dataFile = file;
                    string folder_parent = Directory.GetParent(Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)).ToString();
                    templateFolder = Path.Combine(folder_parent, "GameConfig");
                    if (!Directory.Exists(templateFolder))
                        templateFolder = Path.Combine(Directory.GetParent(folder_parent).ToString(), "GameConfig");
                    List<string> inilist = FindRelevantINIFiles(file, splitTemplate);
                    foreach (string iniitem in inilist)
                    {
                        splitdata.iniFile = iniitem;
                        splitDataList.Add(splitdata);
                    }
                }

				foreach (SplitData CurrentSplitData in splitDataList)
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
					if (labelMode == 2) // If labels are stripped, run split with the nometa parameter
						splitFlags |= SplitFlags.NoMeta;
					if (labelMode != 1) // If labels are address-based or stripped, prevent split from loading the labels file if it exists
						splitFlags |= SplitFlags.NoLabels;
                    Console.WriteLine("Skip full labels: " + splitFlags.HasFlag(SplitFlags.NoLabels).ToString());
                    Console.WriteLine("Strip metadata: " + splitFlags.HasFlag(SplitFlags.NoMeta).ToString());
                    switch (Path.GetExtension(datafilename).ToLowerInvariant())
					{
						case ".dll":
							SplitTools.SplitDLL.SplitDLL.SplitDLLFile(datafilename, inifilename, projectFolderName, splitFlags);
							break;
						case ".nb":
							SplitTools.Split.SplitNB.SplitNBFile(datafilename, false, projectFolderName, 0, inifilename);
							break;
						default:
							SplitTools.Split.SplitBinary.SplitFile(datafilename, inifilename, projectFolderName, splitFlags);
							break;
					}
				}
			}
			else
			{
				Console.WriteLine("Starting split for " + files[0] + System.Environment.NewLine);
				Console.WriteLine("Output folder: " + out_path + System.Environment.NewLine);
				SplitTools.SAArc.SA2MDL.Split(files[0], out_path, files.Skip(1).ToArray());
			}
			Console.WriteLine("Split job finished.");
		}

        private List<string> FindRelevantINIFiles(string file, Templates.SplitTemplate splitTemplate)
        {
            bool dllmode = false;
            string extension = Path.GetExtension(file).ToLowerInvariant();
            List<string> relevantini = new List<string>();
            if (extension == ".dll")
            {
                if (file.ToLowerInvariant().Contains("_orig"))
                    file = file.ToLowerInvariant().Replace("_orig", "");
                dllmode = true;
            }
            else if (extension == ".nb")
                return relevantini;
            Console.WriteLine("Finding relevant split INI files for {0} in {1}", Path.GetFileName(file), splitTemplate.GameInfo.DataFolder);
            foreach (Templates.SplitEntry entry in splitTemplate.SplitEntries)
            {
                if ((Path.GetFileName(entry.SourceFile).ToLowerInvariant() == Path.GetFileName(file).ToLowerInvariant()) || (Path.GetFileName(entry.SourceFile).ToLowerInvariant() == Path.GetFileName(Path.ChangeExtension(file, ".prs")).ToLowerInvariant()))
                {
                    string inifilename = Path.Combine(templateFolder, splitTemplate.GameInfo.DataFolder, entry.IniFile + ".ini");
                    if (dllmode)
                    {
                        SplitTools.SplitDLL.IniDataSplitDLL inifile = IniSerializer.Deserialize<SplitTools.SplitDLL.IniDataSplitDLL>(inifilename);
                        if (inifile.ModuleName != null)
                        {
                            relevantini.Add(Path.GetFullPath(inifilename));
                            Console.WriteLine("Found split file {0}", inifilename);
                        }
                    }
                    else
                    {
						string filewithoutprs = Path.ChangeExtension(file, ".bin");
                        SplitTools.IniData inifile = IniSerializer.Deserialize<SplitTools.IniData>(inifilename);
						if (inifile.DataFilename != null && (inifile.DataFilename.ToLowerInvariant() == Path.GetFileName(file).ToLowerInvariant() || inifile.DataFilename.ToLowerInvariant() == Path.GetFileName(filewithoutprs).ToLowerInvariant()))
                        {
                            relevantini.Add(Path.GetFullPath(inifilename));
                            Console.WriteLine("Found split file {0}", inifilename);
                        }
                        else if (inifile.DataFilename != null) Console.WriteLine("Datafilename: {0}", inifile.DataFilename);
                        else Console.WriteLine("Datafilename in {0} is null", inifilename);
                    }
                }
            }
            return relevantini;
        }

	private void SplitProgress_Shown(object sender, EventArgs e)
		{
			splitDataList = new List<SplitData>();
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