using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml.Serialization;
using SAModel.SAEditorCommon.ProjectManagement;
using System.Xml;
using SplitTools.Split;

namespace SAToolsHub
{
	public partial class resplitMenu : Form
	{
		SAModel.SAEditorCommon.UI.ProgressDialog splitProgress;
		bool overwrite = false;
		Templates.SplitTemplate template;
		List<Templates.SplitEntry> splitEntries = new List<Templates.SplitEntry>();
		List<Templates.SplitEntryMDL> splitMDLEntries = new List<Templates.SplitEntryMDL>();
		List<Templates.SplitEntryEvent> splitEventEntries = new List<Templates.SplitEntryEvent>();
		List<chkBoxData> chkBoxEntries = new List<chkBoxData>();

		public class chkBoxData
		{
			public string type { get; set; }
			public string dispName { get; set; }
			public Templates.SplitEntry splitEntry { get; set; }
			public Templates.SplitEntryMDL mdlEntry { get; set; }
			public Templates.SplitEntryEvent eventEntry { get; set; }

			public chkBoxData(string t, string n, Templates.SplitEntry split = null, Templates.SplitEntryMDL mdl = null, Templates.SplitEntryEvent ev = null)
			{
				type = t;
				dispName = n;
                if (split != null)
                {
                    splitEntry = split;
                    dispName += " (" + split.SourceFile + ")";
                }
                if (mdl != null)
                {
                    mdlEntry = mdl;
                    dispName += " (" + mdl.ModelFile + ")";
                }

				if (ev != null)
				{
					eventEntry = ev;
					dispName += " (" + ev.EventFile + ")";
				}
			}
		}

		public resplitMenu()
		{
			InitializeComponent();
			backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
			backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker1_RunWorkerCompleted);
		}

		#region General Functions
		#endregion

		#region Form Functions
		private void resplitMenu_Shown(object sender, EventArgs e)
		{
			splitEventEntries.Clear();
			splitMDLEntries.Clear();
			splitEntries.Clear();
			chkBoxEntries.Clear();
			checkedListBox1.Items.Clear();
			template = ProjectFunctions.openTemplateFile(SAToolsHub.GetTemplateFileForResplit(SAToolsHub.projType));

			foreach (Templates.SplitEntry splitEntry in template.SplitEntries)
			{
				string name;
				if (splitEntry.CmnName != null)
					name = splitEntry.CmnName;
				else
					name = splitEntry.IniFile;
				chkBoxData item = new chkBoxData("exe", name, splitEntry);
				chkBoxEntries.Add(item);

			}

			foreach (Templates.SplitEntryMDL mdlEntry in template.SplitMDLEntries)
			{
				string mdlFile;
				if (mdlEntry.CmnName != null)
					mdlFile = mdlEntry.CmnName;
				else
					mdlFile	= Path.GetFileNameWithoutExtension(mdlEntry.ModelFile);
				chkBoxData item = new chkBoxData("mdl", mdlFile, null, mdlEntry);
				chkBoxEntries.Add(item);
			}

			foreach (Templates.SplitEntryEvent eventEntry in template.SplitEventEntries)
			{
				string eventFile = Path.GetFileNameWithoutExtension(eventEntry.EventFile);
				chkBoxData item = new chkBoxData("ev", eventFile, null, null, eventEntry);
				chkBoxEntries.Add(item);
			}

			foreach (chkBoxData data in chkBoxEntries)
			{
				checkedListBox1.Items.Add(data);
				checkedListBox1.DisplayMember = "dispName";
			}
		}

		private void chkAll_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < checkedListBox1.Items.Count; i++)
			{
				checkedListBox1.SetItemChecked(i, true);
			}
		}

		private void unchkAll_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < checkedListBox1.Items.Count; i++)
			{
				checkedListBox1.SetItemChecked(i, false);
			}
		}

		private void btnSplit_Click(object sender, EventArgs e)
		{
			foreach (chkBoxData item in checkedListBox1.CheckedItems)
			{
				switch (item.type)
				{
					case "exe":
						splitEntries.Add(item.splitEntry);
						break;
					case "mdl":
						splitMDLEntries.Add(item.mdlEntry);
						break;
					case "ev":
						splitEventEntries.Add(item.eventEntry);
						break;
				}
			}


#if !DEBUG
			backgroundWorker1.RunWorkerAsync();
#endif
#if DEBUG
			backgroundWorker1_DoWork(null, null);
			BackgroundWorker1_RunWorkerCompleted(null, null);
#endif
		}

		private void chkOverwrite_CheckedChanged(object sender, EventArgs e)
		{
			if (chkOverwrite.Checked)
				overwrite = true;
			else
				overwrite = false;
		}
		#endregion

		void splitGame(string game, SAModel.SAEditorCommon.UI.ProgressDialog progress)
		{
			SplitFlags splitFlags = SplitFlags.Log | SplitFlags.Overwrite;
			string appPath = Path.GetDirectoryName(Application.ExecutablePath);
			string dataFolder = template.GameInfo.DataFolder;
			string gamePath = SAToolsHub.gameDirectory;
			string projFolder = SAToolsHub.projectDirectory;
			string iniFolder;

			progress.SetMaxSteps(splitEntries.Count + splitMDLEntries.Count + splitEventEntries.Count + 1);

			if (Directory.Exists(Path.Combine(appPath, "GameConfig", dataFolder)))
				iniFolder = Path.Combine(appPath, "GameConfig", dataFolder);
			else
				iniFolder = Path.Combine(appPath, "..\\GameConfig", dataFolder);

			progress.SetTask("Splitting Game Content");
			foreach (Templates.SplitEntry splitEntry in splitEntries)
				ProjectFunctions.SplitTemplateEntry(splitEntry, progress, gamePath, iniFolder, projFolder, splitFlags);
			// Split MDL files for SA2
			if (splitMDLEntries.Count > 0)
			{
				progress.SetTask("Splitting Character Models");
				foreach (Templates.SplitEntryMDL splitMDL in splitMDLEntries)
					ProjectFunctions.SplitTemplateMDLEntry(splitMDL, progress, gamePath, projFolder, iniFolder, splitFlags.HasFlag(SplitFlags.Overwrite));
			}
			// Split Event files for SA2
			if (splitEventEntries.Count > 0)
			{
				progress.SetTask("Splitting Event Data");
				foreach (Templates.SplitEntryEvent splitEvent in splitEventEntries)
					ProjectFunctions.SplitTemplateEventEntry(splitEvent, progress, gamePath, projFolder, iniFolder, splitFlags.HasFlag(SplitFlags.Overwrite));
			}
			// Project folders for buildable PC games
			progress.SetTask("Updating Project File");
			UpdateProjectFile(progress);
			progress.StepProgress();
		}

		void UpdateProjectFile(SAModel.SAEditorCommon.UI.ProgressDialog progress)
		{
			bool needsUpdate = false;

			if (splitEntries.Count > 0 || splitMDLEntries.Count > 0 || splitEventEntries.Count > 0)
			{
				Templates.ProjectTemplate projFile = ProjectFunctions.openProjectFileString(Path.GetFullPath(SAToolsHub.projXML));
				Templates.ProjectInfo projInfo = projFile.GameInfo;
				List<Templates.SplitEntry> projEntries = projFile.SplitEntries;
				List<Templates.SplitEntryMDL> projMdlEntries = projFile.SplitMDLEntries;
				List<Templates.SplitEntryEvent> projEventEntries = projFile.SplitEventEntries;


				foreach (Templates.SplitEntry entry in splitEntries)
				{
					if (!projEntries.Exists(x => x.IniFile == entry.IniFile))
					{
						projEntries.Add(entry);
						needsUpdate = true;
					}
				}

				if (projMdlEntries.Count > 0)
				{
					foreach (Templates.SplitEntryMDL entry in splitMDLEntries)
					{
						if (!projMdlEntries.Exists(x => x.ModelFile == entry.ModelFile))
						{
							projMdlEntries.Add(entry);
							needsUpdate = true;
						}
					}
				}
				if (projEventEntries.Count > 0)
				{
					foreach (Templates.SplitEntryEvent entry in splitEventEntries)
					{
						if (!projEventEntries.Exists(x => x.EventFile == entry.EventFile))
						{
							projEventEntries.Add(entry);
							needsUpdate = true;
						}
					}
				}
				if (needsUpdate)
				{
					XmlSerializer serializer = new XmlSerializer(typeof(Templates.ProjectTemplate));
                    XmlWriter writer = XmlWriter.Create(SAToolsHub.projXML, new XmlWriterSettings() { Indent = true });
					Templates.ProjectTemplate updProjFile = new Templates.ProjectTemplate();


					updProjFile.GameInfo = projInfo;
					updProjFile.SplitEntries = projEntries;
					if (splitMDLEntries.Count > 0)
						updProjFile.SplitMDLEntries = projMdlEntries;
					if (splitEventEntries.Count > 0)
						updProjFile.SplitEventEntries = projEventEntries;

					serializer.Serialize(writer, updProjFile);
					writer.Close();
				}
			}
		}

		#region Background Worker
		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			using (splitProgress = new SAModel.SAEditorCommon.UI.ProgressDialog("Creating project"))
			{
				Invoke((Action)splitProgress.Show);

				splitGame(SAToolsHub.setGame, splitProgress);

				Invoke((Action)splitProgress.Close);
			}
		}

		private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e != null && e.Error != null)
			{
				MessageBox.Show("Item failed to split: " + e.Error.Message, "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				DialogResult successDiag = MessageBox.Show("Selected items have been resplit successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.None);
				if (successDiag == DialogResult.OK)
				{
					SAToolsHub.resplit = true;
					this.Close();
				}
			}
		}
		#endregion
	}
}
