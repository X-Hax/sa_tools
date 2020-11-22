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
using SonicRetro.SAModel.SAEditorCommon;
using Ookii.Dialogs;
using System.Xml;
using System.Xml.Serialization;
using SA_Tools;
using Fclp.Internals.Extensions;
using ProjectManagement;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;

namespace SAToolsHub
{
	public enum item_icons {
		folder,
		file,
		model,
		level,
		document,
		texture,
		anim,
		audio,
		camera,
		compress,
		setobj,
		data
	}

	public partial class SAToolsHub : Form
	{
		//Additional Windows
		private newProj projectCreateDiag;
		private editProj projectEditorDiag;
		private buildWindow buildWindowDiag;
		private templateWriter templateWriter;

		//Variables
		public static string newProjFile { get; set; }
		public static string projName { get; set; }
		public static string projectDirectory { get; set; }
		public static string setGame { get; set; }
		public static string gameSystemDirectory { get; set; }
		public static List<SplitEntry> projSplitEntries { get; set; }
		public static List<SplitEntryMDL> projSplitMDLEntries { get; set; }

		//Program Paths
		string samdlPath;
		string salvlPath;
		string texeditPath;
		string sadxlvl2Path;
		string sadxsndsharpPath;
		string sadxtweakerPath;
		string sadxfonteditPath;
		string sasavePath;
		string sa2eventviewPath;
		string sa2evtexteditPath;
		string sa2streditPath;
		string sa2stgselPath;
		string datatoolboxPath;

		//Additional Code/Functions
		private void openProject(string projectFile)
		{
			var projFileSerializer = new XmlSerializer(typeof(ProjectManagement.ProjectTemplate));
			var projFileStream = File.OpenRead(projectFile);
			var projFile = (ProjectManagement.ProjectTemplate)projFileSerializer.Deserialize(projFileStream);

			projectDirectory = (projFile.GameInfo.ModSystemFolder);
			gameSystemDirectory = (projFile.GameInfo.GameSystemFolder);
			PopulateTreeView(projectDirectory);
			this.treeView1.NodeMouseClick +=
				new TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);

			setGame = projFile.GameInfo.GameName;
			projSplitEntries = projFile.SplitEntries;
			if (setGame == "SA2PC" || setGame == "SA2" || setGame == "SA2TT" || setGame == "SA2P")
			{
				projSplitMDLEntries = projFile.SplitMDLEntries;
			}

			toggleButtons(setGame);
			closeProjectToolStripMenuItem.Enabled = true;
			if (projFile.GameInfo.CanBuild)
			{
				buildToolStripMenuItem.Enabled = true;
				editProjectInfoToolStripMenuItem.Enabled = true;
				tsBuild.Enabled = true;
				tsGameRun.Enabled = true;
				tsEditProj.Enabled = true;
			}
		}
		
		private void PopulateTreeView(string directory)
		{
			TreeNode rootNode;

			DirectoryInfo info = new DirectoryInfo(directory);
			if (info.Exists)
			{
				rootNode = new TreeNode(info.Name);
				rootNode.Tag = info;
				GetDirectories(info.GetDirectories(), rootNode);
				treeView1.Nodes.Add(rootNode);
			}
		}

		private void GetDirectories(DirectoryInfo[] subDirs, TreeNode nodeToAddTo)
		{
			TreeNode aNode;
			DirectoryInfo[] subSubDirs;
			foreach (DirectoryInfo subDir in subDirs)
			{
				aNode = new TreeNode(subDir.Name, 0, 0);
				aNode.Tag = subDir;
				aNode.ImageKey = "folder";
				subSubDirs = subDir.GetDirectories();
				if (subSubDirs.Length != 0)
				{
					GetDirectories(subSubDirs, aNode);
				}
				nodeToAddTo.Nodes.Add(aNode);
			}
		}

		void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			listView1.ContextMenuStrip = contextMenuStrip1;
			TreeNode newSelected = e.Node;
			listView1.Items.Clear();
			DirectoryInfo nodeDirInfo = (DirectoryInfo)newSelected.Tag;
			ListViewItem item = null;

			LoadFiles(nodeDirInfo, item);
		}

		void LoadFiles(DirectoryInfo nodeDirInfo, ListViewItem item)
		{
			ListViewItem.ListViewSubItem[] subItems;
			foreach (DirectoryInfo dir in nodeDirInfo.GetDirectories())
			{
				item = new ListViewItem(dir.Name, (int)item_icons.folder);
				subItems = new ListViewItem.ListViewSubItem[]
					{new ListViewItem.ListViewSubItem(item, "Directory"),
						new ListViewItem.ListViewSubItem(item,
						dir.LastAccessTime.ToShortDateString())};
				item.Tag = "dir";
				item.SubItems.AddRange(subItems);
				listView1.Items.Add(item);
			}
			foreach (System.IO.FileInfo file in nodeDirInfo.GetFiles())
			{
				item = new ListViewItem(file.Name, (int)item_icons.file);
				string fileName = (file.Name.ToLower());
				string fileType = (file.Extension.ToLower());

				switch (fileType)
				{
					case ".sa1mdl":
					case ".sa2mdl":
					case ".sa2bmdl":
						{
							subItems = new ListViewItem.ListViewSubItem[]
								{ new ListViewItem.ListViewSubItem(item, "Model File"),
							new ListViewItem.ListViewSubItem(item,
								file.LastAccessTime.ToShortDateString())};
							item.ImageIndex = (int)item_icons.model;
							item.Tag = "mdl";
							break;
						}
					case ".sa1lvl":
					case ".sa2lvl":
					case ".sa2blvl":
						{
							subItems = new ListViewItem.ListViewSubItem[]
								{ new ListViewItem.ListViewSubItem(item, "Level File"),
							new ListViewItem.ListViewSubItem(item,
								file.LastAccessTime.ToShortDateString())};
							item.ImageIndex = (int)item_icons.level;
							item.Tag = "lvl";
							break;
						}
					case ".ini":
						{
							subItems = new ListViewItem.ListViewSubItem[]
								{ new ListViewItem.ListViewSubItem(item, "Data File"),
							new ListViewItem.ListViewSubItem(item,
								file.LastAccessTime.ToShortDateString())};
							item.ImageIndex = (int)item_icons.data;
							item.Tag = "ini";
							break;
						}
					case ".txt":
						{
							subItems = new ListViewItem.ListViewSubItem[]
								{ new ListViewItem.ListViewSubItem(item, "Text File"),
							new ListViewItem.ListViewSubItem(item,
								file.LastAccessTime.ToShortDateString())};
							item.ImageIndex = (int)item_icons.document;
							item.Tag = "txt";
							break;
						}
					case ".bin":
						{
							if (fileName.Contains("cam"))
							{
								subItems = new ListViewItem.ListViewSubItem[]
									{ new ListViewItem.ListViewSubItem(item, "Camera Layout"),
								new ListViewItem.ListViewSubItem(item,
									file.LastAccessTime.ToShortDateString())};
								item.ImageIndex = (int)item_icons.camera;
								item.Tag = "cam";
							}
							else if (fileName.Contains("set"))
							{
								subItems = new ListViewItem.ListViewSubItem[]
									{ new ListViewItem.ListViewSubItem(item, "Object Layout"),
								new ListViewItem.ListViewSubItem(item,
									file.LastAccessTime.ToShortDateString())};
								item.ImageIndex = (int)item_icons.setobj;
								item.Tag = "set";
							}
							else
							{
								subItems = new ListViewItem.ListViewSubItem[]
									{ new ListViewItem.ListViewSubItem(item, "Binary File"),
								new ListViewItem.ListViewSubItem(item,
									file.LastAccessTime.ToShortDateString())};
								item.Tag = "bin";
							}
							break;
						}
					case ".pvm":
					case ".pvmx":
					case ".gvm":
						{
							subItems = new ListViewItem.ListViewSubItem[]
								{ new ListViewItem.ListViewSubItem(item, "Texture Archive"),
							new ListViewItem.ListViewSubItem(item,
								file.LastAccessTime.ToShortDateString())};
							item.ImageIndex = (int)item_icons.texture;
							item.Tag = "tex";
							break;
						}
					case ".prs":
						{
							if (fileName.Contains("mdl"))
							{
								subItems = new ListViewItem.ListViewSubItem[]
								{ new ListViewItem.ListViewSubItem(item, "Compressed Model"),
							new ListViewItem.ListViewSubItem(item,
								file.LastAccessTime.ToShortDateString())};
								item.Tag = "mdl";
							}
							else if (fileName.Contains("tex"))
							{
								subItems = new ListViewItem.ListViewSubItem[]
								{ new ListViewItem.ListViewSubItem(item, "Compressed Texture Archive"),
							new ListViewItem.ListViewSubItem(item,
								file.LastAccessTime.ToShortDateString())};
								item.Tag = "tex";
							}
							else if (fileName.Contains("mtn"))
							{
								subItems = new ListViewItem.ListViewSubItem[]
								{ new ListViewItem.ListViewSubItem(item, "Compressed Animations Archive"),
							new ListViewItem.ListViewSubItem(item,
								file.LastAccessTime.ToShortDateString())};
								item.Tag = "anm";
							}
							else
							{
								subItems = new ListViewItem.ListViewSubItem[]
									{ new ListViewItem.ListViewSubItem(item, "Compressed File"),
							new ListViewItem.ListViewSubItem(item,
								file.LastAccessTime.ToShortDateString())};
								item.Tag = "prs";
							}
							item.ImageIndex = (int)item_icons.compress;
							break;
						}
					case ".saanim":
						{
							subItems = new ListViewItem.ListViewSubItem[]
								{ new ListViewItem.ListViewSubItem(item, "Animation File"),
							new ListViewItem.ListViewSubItem(item,
								file.LastAccessTime.ToShortDateString())};
							item.ImageIndex = (int)item_icons.anim;
							item.Tag = "anm";
							break;
						}
					case ".adx":
					case ".mlt":
					case ".csb":
					case ".wma":
					case ".dat":
						subItems = new ListViewItem.ListViewSubItem[]
								{ new ListViewItem.ListViewSubItem(item, "Audio File"),
							new ListViewItem.ListViewSubItem(item,
								file.LastAccessTime.ToShortDateString())};
						item.ImageIndex = (int)item_icons.audio;
						item.Tag = "snd";
						break;
					default:
						{
							subItems = new ListViewItem.ListViewSubItem[]
								{ new ListViewItem.ListViewSubItem(item, "File"),
							new ListViewItem.ListViewSubItem(item,
								file.LastAccessTime.ToShortDateString())};
							item.Tag = "msc";
							break;
						}
				}

				item.SubItems.AddRange(subItems);
				listView1.Items.Add(item);
			}

			listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
		}

		void toggleButtons(string game)
		{
			switch (game)
			{
				case "SADXPC":
					tsSADXLVL2.Visible = true;
					tsSADXTweaker.Visible = true;
					tsSADXsndSharp.Visible = true;
					tsSADXFontEdit.Visible = true;
					break;
				case "SA2PC":
					tsSA2EvView.Visible = true;
					tsSA2EvTxt.Visible = true;
					tsSA2MsgEdit.Visible = true;
					tsSA2StgSel.Visible = true;
					break;
				default:
					break;
			}
		}

		public void resetOpenProject()
		{
			treeView1.Nodes.Clear();
			listView1.Items.Clear();

			//reset Tools Hub Buttons
			tsBuild.Enabled = false;
			tsGameRun.Enabled = false;
			tsEditProj.Enabled = false;
			closeProjectToolStripMenuItem.Enabled = false;
			editProjectInfoToolStripMenuItem.Enabled = false;
			buildToolStripMenuItem.Enabled = false;

			//reset DX game buttons
			tsSADXLVL2.Visible = false;
			tsSADXTweaker.Visible = false;
			tsSADXsndSharp.Visible = false;
			tsSADXFontEdit.Visible = false;

			//reset SA2 game buttons
			tsSA2EvView.Visible = false;
			tsSA2EvTxt.Visible = false;
			tsSA2MsgEdit.Visible = false;
			tsSA2StgSel.Visible = false;
		}

		public SAToolsHub()
		{
			InitializeComponent();

			projectCreateDiag = new newProj();
			projectEditorDiag = new editProj();
			buildWindowDiag = new buildWindow();
			templateWriter = new templateWriter();
		}

		private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			string itemPath = "";
			string itemName = listView1.SelectedItems[0].Text;
			string itemTag = (string)listView1.SelectedItems[0].Tag;
			if (itemTag != "dir")
				itemPath = treeView1.SelectedNode.FullPath;

			string filePath = string.Format("\"{0}\"", projectDirectory + "/../" + (Path.Combine(itemPath, itemName)));

			if (listView1.SelectedItems.Count > 0)
			{
				switch (itemTag)
				{
					case "dir":
						//treeView1.SelectedNode = treeView1.Nodes.Find(itemName, true)[0];
						break;
					case "mdl":
						Process samdlProcess = Process.Start(samdlPath, filePath);
						break;
					case "lvl":
						//Process salvlProcess = Process.Start(salvlPath, filePath);
						break;
					case "tex":
						Process texEditProcess = Process.Start(texeditPath, filePath);
						break;
				}
			}
		}

		private void toolsHub_Shown(object sender, EventArgs e)
		{
			string build;
#if DEBUG
			build = "Debug/";
#endif
#if !DEBUG
			build = "Release/";
#endif
			string rootPath;
			string appPath = Path.GetDirectoryName(Application.ExecutablePath);

			if (Directory.Exists(appPath + "/../../bin/"))
			{
				rootPath = appPath + "/../../../";

				samdlPath = rootPath + "/SAMDL/bin/" + build + "SAMDL.exe";
				salvlPath = rootPath + "/SALVL/bin/" + build + "SALVL.exe";
				texeditPath = rootPath + "/TextureEditor/bin/" + build + "TextureEditor.exe";
				sadxlvl2Path = rootPath + "/SADXLVL2/bin/" + build + "SADXLVL2.exe";
				sadxsndsharpPath = rootPath + "/SADXsndSharp/bin/" + build + "SADXsndSharp.exe";
				sadxtweakerPath = rootPath + "/SADXTweaker2/bin/" + build + "SADXTweaker2.exe";
				sadxfonteditPath = rootPath + "/SADXFontEdit/bin/" + build + "SADXFontEdit.exe";
				sasavePath = rootPath + "/SASave/bin/" + build + "SASave.exe";
				sa2eventviewPath = rootPath + "/SA2EventViewer/bin/" + build + "SA2EventViewer.exe";
				sa2evtexteditPath = rootPath + "/SA2CutsceneTextEditor/bin/" + build + "SA2CutsceneTextEditor.exe";
				sa2streditPath = rootPath + "/SA2MessageFileEditor/bin/" + build + "SA2MessageFileEditor.exe";
				sa2stgselPath = rootPath + "/SA2StageSelEdit/bin/" + build + "SA2StageSelEdit.exe";
				datatoolboxPath = rootPath + "/DataToolbox/bin/" + build + "DataToolbox.exe";
			}
			else
			{
				rootPath = appPath + "/../";

				samdlPath = rootPath + "/SAMDL/SAMDL.exe";
				salvlPath = rootPath + "/SALVL/SALVL.exe";
				texeditPath = rootPath + "/TextureEditor/TextureEditor.exe";
				sadxlvl2Path = rootPath + "/SADXPC/SADXLVL2/SADXLVL2.exe";
				sadxsndsharpPath = rootPath + "/SADXPC/SADXsndSharp/SADXsndSharp.exe";
				sadxtweakerPath = rootPath + "/SADXPC/SADXTweaker2/SADXTweaker2.exe";
				sadxfonteditPath = rootPath + "/SADXPC/SADXFontEdit/SADXFontEdit.exe";
				sasavePath = rootPath + "/SASave/SASave.exe";
				sa2eventviewPath = rootPath + "/SA2PC/SA2EventViewer/SA2EventViewer.exe";
				sa2evtexteditPath = rootPath + "/SA2PC/SA2CutsceneTextEditor/SA2CutsceneTextEditor.exe";
				sa2streditPath = rootPath + "/SA2PC/SA2MessageFileEditor/SA2MessageFileEditor.exe";
				sa2stgselPath = rootPath + "/SA2PC/SA2StageSelEdit/SA2StageSelEdit.exe";
				datatoolboxPath = rootPath + "/DataToolbox/DataToolbox.exe";
			}
		}

		//Tool Strip Functions
		//Settings
		private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		//Projects
		private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (treeView1.Nodes.Count > 0)
			{
				DialogResult newProjWarning = MessageBox.Show(("A project is currently open.\n\nAre you sure you wish to create a new project?"), "Project Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				if (newProjWarning == DialogResult.Yes)
				{
					resetOpenProject();
					projectCreateDiag.ShowDialog();
				}
			}
			else
			{
				projectCreateDiag.ShowDialog();
			}

			if (!newProjFile.IsNullOrWhiteSpace())
			{
				openProject(newProjFile);
			}
		}

		private void openProjectToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog1 = new OpenFileDialog();
			openFileDialog1.Filter = "Project File (*.xml)|*.xml";
			openFileDialog1.RestoreDirectory = true;

			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				resetOpenProject();
				string projectFile = openFileDialog1.FileName;
				openProject(projectFile);
			}
		}

		private void editProjectInfoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			projectEditorDiag.ShowDialog();
		}

		private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			projectDirectory = null;
			resetOpenProject();
		}

		//Project Build
		private void manualBuildToolStripMenuItem_Click(object sender, EventArgs e)
		{
			buildWindowDiag.ShowDialog();
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
			ProcessStartInfo samdlStartInfo = new ProcessStartInfo(Path.GetFullPath(samdlPath));

			Process samdlProcess = Process.Start(samdlStartInfo);

		}

		private void sALVLToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ProcessStartInfo salvlStartInfo = new ProcessStartInfo(
				Path.GetFullPath(salvlPath)//,
				/*Path.GetFullPath(projectFolder)*/);

			Process salvlProcess = Process.Start(salvlStartInfo);
		}

		private void textureEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ProcessStartInfo texEditStartInfo = new ProcessStartInfo(
				Path.GetFullPath(texeditPath)//,
				/*Path.GetFullPath(projectFolder)*/);

			Process texEditProcess = Process.Start(texEditStartInfo);
		}

		//SADX Tools Initializers
		private void sADXLVL2ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ProcessStartInfo sadxlvl2StartInfo;

			if (projectDirectory != null)
			{
				string projectArgumentsPath = string.Format("\"{0}\"", Path.Combine(projectDirectory, "sadxlvl.ini"));

				sadxlvl2StartInfo = new ProcessStartInfo(
					Path.GetFullPath(sadxlvl2Path), projectArgumentsPath);
			}
			else
			{
				sadxlvl2StartInfo = new ProcessStartInfo(
					Path.GetFullPath(sadxlvl2Path));
			}

			Process sadxlvl2Process = Process.Start(sadxlvl2StartInfo);
		}

		private void sADXTweakerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ProcessStartInfo tweakerStartInfo;

			if (projectDirectory != null)
			{
				string projectArgumentsPath = string.Format("\"{0}\"", Path.Combine(projectDirectory, "sonic_data.ini"));

				tweakerStartInfo = new ProcessStartInfo(
					Path.GetFullPath(sadxtweakerPath), projectArgumentsPath);
			}
			else
			{
				tweakerStartInfo = new ProcessStartInfo(
					Path.GetFullPath(sadxtweakerPath));
			}

			Process sadxlvl2Process = Process.Start(tweakerStartInfo);
		}

		private void sADXsndSharpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ProcessStartInfo sndSharpStartInfo = new ProcessStartInfo(
				Path.GetFullPath(sadxsndsharpPath)//,
				/*Path.GetFullPath(projectFolder)*/);

			Process sndSharpProcess = Process.Start(sndSharpStartInfo);
		}

		private void sAFontEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ProcessStartInfo saFontStartInfo = new ProcessStartInfo(
				Path.GetFullPath(sadxfonteditPath)//,
				/*Path.GetFullPath(projectFolder)*/);

			Process saFontProcess = Process.Start(saFontStartInfo);
		}

		private void sASaveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ProcessStartInfo saSaveStartInfo = new ProcessStartInfo(
				Path.GetFullPath(sasavePath)//,
				/*Path.GetFullPath(projectFolder)*/);

			Process saSaveProcess = Process.Start(saSaveStartInfo);
		}

		//SA2 Tools Initializers
		private void sA2EventViewerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ProcessStartInfo sa2EventStartInfo = new ProcessStartInfo(
				Path.GetFullPath(sa2eventviewPath)//,
				/*Path.GetFullPath(projectFolder)*/);

			Process saSaveProcess = Process.Start(sa2EventStartInfo);
		}

		private void sA2CutsceneTextEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ProcessStartInfo sa2EvTextStartInfo = new ProcessStartInfo(
				Path.GetFullPath(sa2evtexteditPath)//,
				/*Path.GetFullPath(projectFolder)*/);

			Process saSaveProcess = Process.Start(sa2EvTextStartInfo);
		}

		private void sA2MessageEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ProcessStartInfo sa2MsgTextStartInfo = new ProcessStartInfo(
				Path.GetFullPath(sa2streditPath)//,
				/*Path.GetFullPath(projectFolder)*/);

			Process saSaveProcess = Process.Start(sa2MsgTextStartInfo);
		}

		private void sA2StageSelectEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ProcessStartInfo sa2StgSelStartInfo = new ProcessStartInfo(
				Path.GetFullPath(sa2stgselPath)//,
				/*Path.GetFullPath(projectFolder)*/);

			Process saSaveProcess = Process.Start(sa2StgSelStartInfo);
		}

		//Data Extractor/Convert (new Split UI)
		private void splitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ProcessStartInfo dataToolStartInfo = new ProcessStartInfo(
				Path.GetFullPath(datatoolboxPath)//,
				/*Path.GetFullPath(projectFolder)*/);

			Process saSaveProcess = Process.Start(dataToolStartInfo);
		}

		//Help Links
		//Resources
		private void sAToolsWikiToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				Process.Start("https://github.com/sonicretro/sa_tools/wiki");
			}
			catch
			{
				MessageBox.Show("Something went wrong, could not open link in browser.");
			}
		}

		private void retrosSCHGForSADXToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				Process.Start("https://info.sonicretro.org/SCHG:Sonic_Adventure_DX:_PC");
			}
			catch
			{
				MessageBox.Show("Something went wrong, could not open link in browser.");
			}
		}

		private void retrosSCHGForSA2ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				Process.Start("https://info.sonicretro.org/SCHG:Sonic_Adventure_2_(PC)");
			}
			catch
			{
				MessageBox.Show("Something went wrong, could not open link in browser.");
			}
		}

		private void sADXGitWikiToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				Process.Start("https://github.com/kellsnc/sadx-modding-guide/wiki");
			}
			catch
			{
				MessageBox.Show("Something went wrong, could not open link in browser.");
			}
		}

		//GitHub Issue Tracker
		private void gitHubIssueTrackerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				Process.Start("https://github.com/sonicretro/sa_tools/issues");
			}
			catch
			{
				MessageBox.Show("Something went wrong, could not open link in browser.");
			}
		}

		//Toolstrip Buttons
		private void tsNewProj_Click(object sender, EventArgs e)
		{
			newProjectToolStripMenuItem_Click(sender, e);
		}

		private void tsOpenProj_Click(object sender, EventArgs e)
		{
			openProjectToolStripMenuItem1_Click(sender, e);
		}

		private void tsEditProj_Click(object sender, EventArgs e)
		{
			editProjectInfoToolStripMenuItem_Click(sender, e);
		}

		private void tsBuildManual_Click(object sender, EventArgs e)
		{
			manualBuildToolStripMenuItem_Click(sender, e);
		}

		private void tsBuildRun_Click(object sender, EventArgs e)
		{
			buildRunGameToolStripMenuItem_Click(sender, e);
		}

		private void tsUpdate_Click(object sender, EventArgs e)
		{
			checkForUpdatesToolStripMenuItem_Click(sender, e);
		}

		private void tsSAMDL_Click(object sender, EventArgs e)
		{
			sAMDLToolStripMenuItem_Click(sender, e);
		}

		private void tsSALVL_Click(object sender, EventArgs e)
		{
			sALVLToolStripMenuItem_Click(sender, e);
		}

		private void tsTexEdit_Click(object sender, EventArgs e)
		{
			textureEditorToolStripMenuItem_Click(sender, e);
		}

		private void tsSADXLVL2_Click(object sender, EventArgs e)
		{
			sADXLVL2ToolStripMenuItem_Click(sender, e);
		}

		private void tsSADXTweaker_Click(object sender, EventArgs e)
		{
			sADXTweakerToolStripMenuItem_Click(sender, e);
		}

		private void tsSADXsndSharp_Click(object sender, EventArgs e)
		{
			sADXsndSharpToolStripMenuItem_Click(sender, e);
		}

		private void tsSADXFontEdit_Click(object sender, EventArgs e)
		{
			sAFontEditorToolStripMenuItem_Click(sender, e);
		}

		private void tsSA2EvView_Click(object sender, EventArgs e)
		{
			sA2EventViewerToolStripMenuItem_Click(sender, e);
		}

		private void tsSA2EvTxt_Click(object sender, EventArgs e)
		{
			sA2CutsceneTextEditorToolStripMenuItem_Click(sender, e);
		}

		private void tsSA2MsgEdit_Click(object sender, EventArgs e)
		{
			sA2MessageEditorToolStripMenuItem_Click(sender, e);
		}

		private void tsSA2StgSel_Click(object sender, EventArgs e)
		{
			sA2StageSelectEditorToolStripMenuItem_Click(sender, e);
		}

		//Context Menu
		private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void toolStripMenuItem1_Click(object sender, EventArgs e)
		{
			templateWriter.ShowDialog();
		}
	}
}
