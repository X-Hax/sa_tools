using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml.Serialization;
using SAEditorCommon.ProjectManagement;

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
		data,
		cfile,
		json
	}

	public partial class SAToolsHub : Form
	{
		//Additional Windows
		private newProj projectCreateDiag;
		private editProj projectEditorDiag;
		private buildWindow buildWindowDiag;
		private templateWriter templateWriter;
		private gameOptions gameOptionsDiag;
		private projConv projectConverter;
		private formAbout abtWindow;

		//Variables
		public static string newProjFile { get; set; }
		public static string projXML { get; set; }
		public static string projectDirectory { get; set; }
		public static string setGame { get; set; }
		public static string gameDirectory { get; set; }
		string gameDir;
		public static List<SplitEntry> projSplitEntries { get; set; }
		public static List<SplitEntryMDL> projSplitMDLEntries { get; set; }
		public static ProjectSettings hubSettings { get; set; }
		List<string> copyPaths;
		List<string> backPaths;
		List<string> forwardPaths;
		class itemTags
		{
			public string Type { get; set; }
			public string Path { get; set; }
		}

		//Program Paths
		ProcessStartInfo samdlStartInfo;
		ProcessStartInfo salvlStartInfo;
		ProcessStartInfo texeditStartInfo;
		ProcessStartInfo sadxlvl2StartInfo;
		ProcessStartInfo sadxsndsharpStartInfo;
		ProcessStartInfo sadxtweakerStartInfo;
		ProcessStartInfo sadxfonteditStartInfo;
		ProcessStartInfo sasaveStartInfo;
		ProcessStartInfo sa2eventviewStartInfo;
		ProcessStartInfo sa2evtexteditStartInfo;
		ProcessStartInfo sa2streditStartInfo;
		ProcessStartInfo sa2stgselStartInfo;
		ProcessStartInfo datatoolboxStartInfo;

		class ListViewItemComparer : IComparer
		{
			private int col;
			public ListViewItemComparer()
			{
				col = 0;
			}
			public ListViewItemComparer(int column)
			{
				col = column;
			}
			public int Compare(object x, object y)
			{
				itemTags xTags = (itemTags)((ListViewItem)x).Tag;
				itemTags yTags = (itemTags)((ListViewItem)y).Tag;
				if (xTags.Type == yTags.Type)
					return String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
				else if (xTags.Type == "dir")
					return -1;
				else
					return 1;
			}
		}

		public SAToolsHub()
		{
			InitializeComponent();

			hubSettings = ProjectSettings.Load();

			projectCreateDiag = new newProj();
			projectEditorDiag = new editProj();
			buildWindowDiag = new buildWindow();
			templateWriter = new templateWriter();
			gameOptionsDiag = new gameOptions();
			projectConverter = new projConv();
			abtWindow = new formAbout();
		}

		//Additional Code/Functions
		private void openProject(string projectFile)
		{
			var projFileSerializer = new XmlSerializer(typeof(ProjectTemplate));
			var projFileStream = File.OpenRead(projectFile);
			var projFile = (ProjectTemplate)projFileSerializer.Deserialize(projFileStream);

			setGame = projFile.GameInfo.GameName;
			projectDirectory = (projFile.GameInfo.ModSystemFolder);
			gameDirectory = (projFile.GameInfo.GameSystemFolder);
			projXML = projectFile;

			switch (setGame)
			{
				case "SADXPC":
					gameDir = gameDirectory + "\\system\\";
					break;
				case "SA2PC":
					gameDir = gameDirectory + "\\resource\\gd_PC\\";
					break;
				default:
					gameDir = gameDirectory;
					break;
			}

			PopulateTreeView(projectDirectory);
			PopulateTreeView(gameDir);
			this.treeView1.NodeMouseClick +=
				new TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);

			projSplitEntries = projFile.SplitEntries;
			if (setGame.Contains("SA2"))
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

			editToolStripMenuItem.Enabled = true;
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
				if (subDir.Name != "dllcache")
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
		}

		private TreeNode SearchTreeView(string p_sSearchTerm, TreeNodeCollection p_Nodes)
		{
			foreach (TreeNode node in p_Nodes)
			{
				if (node.Text == p_sSearchTerm)
				{
					return node;
				}

				if (node.Nodes.Count > 0)
				{
					var result = SearchTreeView(p_sSearchTerm, node.Nodes);
					if (result != null)
					{
						return result;
					}
				}
			}

			return null;
		}

		void SelectListViewNode(TreeNode node)
		{
			listView1.SelectedItems.Clear();
			if (treeView1.SelectedNode != node)
			{
				TreeNode newSelected = node;
				listView1.Items.Clear();
				DirectoryInfo nodeDirInfo = (DirectoryInfo)newSelected.Tag;
				ListViewItem item = null;

				LoadFiles(nodeDirInfo, item);
			}
		}

		void LoadFiles(DirectoryInfo nodeDirInfo, ListViewItem item)
		{
			ListViewItem.ListViewSubItem[] subItems;
			foreach (DirectoryInfo dir in nodeDirInfo.GetDirectories())
			{
				if (dir.Name != "dllcache")
				{
					item = new ListViewItem(dir.Name, (int)item_icons.folder);
					subItems = new ListViewItem.ListViewSubItem[]
						{new ListViewItem.ListViewSubItem(item, "Directory"),
						new ListViewItem.ListViewSubItem(item,
						dir.LastAccessTime.ToShortDateString())};
					itemTags dirTag = new itemTags();
					dirTag.Type = "dir";
					dirTag.Path = dir.FullName;
					item.Tag = dirTag;
					item.SubItems.AddRange(subItems);
					listView1.Items.Add(item);
				}
			}
			foreach (System.IO.FileInfo file in nodeDirInfo.GetFiles())
			{
				item = new ListViewItem(file.Name, (int)item_icons.file);
				string fileName = (file.Name.ToLower());
				string fileType = (file.Extension.ToLower());
				string filePath = (file.FullName);

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
							item.ToolTipText = "Double click to open in SAMDL";
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
							item.ToolTipText = "Double click to open in SALVL";
							break;
						}
					case ".ini":
						{
							subItems = new ListViewItem.ListViewSubItem[]
								{ new ListViewItem.ListViewSubItem(item, "Data File"),
							new ListViewItem.ListViewSubItem(item,
								file.LastAccessTime.ToShortDateString())};
							item.ImageIndex = (int)item_icons.data;
							switch (fileName)
							{
								case "mod":
									item.ToolTipText = "Double click to open Mod Info Editor";
									break;
								case "sadxlvl":
									item.ToolTipText = "Double click to open SADXLVL2";
									break;
								default:
									item.ToolTipText = "Double click to open in the default text editor.";
									break;
							}
							break;
						}
					case ".txt":
						{
							subItems = new ListViewItem.ListViewSubItem[]
								{ new ListViewItem.ListViewSubItem(item, "Text File"),
							new ListViewItem.ListViewSubItem(item,
								file.LastAccessTime.ToShortDateString())};
							item.ImageIndex = (int)item_icons.document;
							item.ToolTipText = "Double click to open in the default text editor.";
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
							}
							else if (fileName.Contains("set"))
							{
								subItems = new ListViewItem.ListViewSubItem[]
									{ new ListViewItem.ListViewSubItem(item, "Object Layout"),
								new ListViewItem.ListViewSubItem(item,
									file.LastAccessTime.ToShortDateString())};
								item.ImageIndex = (int)item_icons.setobj;
							}
							else
							{
								subItems = new ListViewItem.ListViewSubItem[]
									{ new ListViewItem.ListViewSubItem(item, "Binary File"),
								new ListViewItem.ListViewSubItem(item,
									file.LastAccessTime.ToShortDateString())};
							}
							break;
						}
					case ".pvm":
					case ".pvmx":
					case ".gvm":
					case ".pak":
						{
							subItems = new ListViewItem.ListViewSubItem[]
								{ new ListViewItem.ListViewSubItem(item, "Texture Archive"),
							new ListViewItem.ListViewSubItem(item,
								file.LastAccessTime.ToShortDateString())};
							item.ImageIndex = (int)item_icons.texture;
							item.ToolTipText = "Double click to open in Texture Editor";
							break;
						}
					case ".pvr":
					case ".gvr":
						{
							subItems = new ListViewItem.ListViewSubItem[]
								{ new ListViewItem.ListViewSubItem(item, "Image File"),
							new ListViewItem.ListViewSubItem(item,
								file.LastAccessTime.ToShortDateString())};
							item.ImageIndex = (int)item_icons.texture;
							break;
						}
					case ".pvp":
					case ".gvp":
					case ".plt":
						{
							subItems = new ListViewItem.ListViewSubItem[]
								{ new ListViewItem.ListViewSubItem(item, "Palette File"),
							new ListViewItem.ListViewSubItem(item,
								file.LastAccessTime.ToShortDateString())};
							item.ImageIndex = (int)item_icons.texture;
							break;
						}
					case ".dds":
					case ".jpg":
					case ".png":
					case ".bmp":
					case ".gif":
						{
							subItems = new ListViewItem.ListViewSubItem[]
								{ new ListViewItem.ListViewSubItem(item, "Image File"),
							new ListViewItem.ListViewSubItem(item,
								file.LastAccessTime.ToShortDateString())};
							item.ImageIndex = (int)item_icons.texture;
							item.ToolTipText = "Double click to open in default image program";
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
								item.ToolTipText = "Double click to open in SAMDL";
							}
							else if (fileName.Contains("tex") || fileName.Contains("tx") || fileName.Contains("bg"))
							{
								subItems = new ListViewItem.ListViewSubItem[]
								{ new ListViewItem.ListViewSubItem(item, "Compressed Texture Archive"),
							new ListViewItem.ListViewSubItem(item,
								file.LastAccessTime.ToShortDateString())};
								item.ToolTipText = "Double click to open in Texture Editor";
							}
							else if (fileName.Contains("mtn"))
							{
								subItems = new ListViewItem.ListViewSubItem[]
								{ new ListViewItem.ListViewSubItem(item, "Compressed Animations Archive"),
							new ListViewItem.ListViewSubItem(item,
								file.LastAccessTime.ToShortDateString())};
							}
							else
							{
								subItems = new ListViewItem.ListViewSubItem[]
									{ new ListViewItem.ListViewSubItem(item, "Compressed File"),
							new ListViewItem.ListViewSubItem(item,
								file.LastAccessTime.ToShortDateString())};
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
							break;
						}
					case ".adx":
					case ".mlt":
					case ".csb":
					case ".wma":
					case ".dat":
						{
							subItems = new ListViewItem.ListViewSubItem[]
									{ new ListViewItem.ListViewSubItem(item, "Audio File"),
							new ListViewItem.ListViewSubItem(item,
								file.LastAccessTime.ToShortDateString())};
							item.ImageIndex = (int)item_icons.audio;
							break;
						}
					case ".wmv":
					case ".sfd":
					case ".m1v":
						{
							subItems = new ListViewItem.ListViewSubItem[]
									{ new ListViewItem.ListViewSubItem(item, "Video File"),
							new ListViewItem.ListViewSubItem(item,
								file.LastAccessTime.ToShortDateString())};
							item.ImageIndex = (int)item_icons.camera;
							break;
						}
					case ".c":
						{
							subItems = new ListViewItem.ListViewSubItem[]
									{ new ListViewItem.ListViewSubItem(item, "Code File"),
							new ListViewItem.ListViewSubItem(item,
								file.LastAccessTime.ToShortDateString())};
							item.ImageIndex = (int)item_icons.cfile;
							break;
						}
					case ".json":
						{
							subItems = new ListViewItem.ListViewSubItem[]
									{ new ListViewItem.ListViewSubItem(item, "Javascript File"),
							new ListViewItem.ListViewSubItem(item,
								file.LastAccessTime.ToShortDateString())};
							item.ImageIndex = (int)item_icons.json;
							break;
						}
					default:
						{
							subItems = new ListViewItem.ListViewSubItem[]
								{ new ListViewItem.ListViewSubItem(item, "File"),
							new ListViewItem.ListViewSubItem(item,
								file.LastAccessTime.ToShortDateString())};
							break;
						}
				}
				itemTags itemTag = new itemTags();
				itemTag.Type = "file";
				itemTag.Path = file.FullName;
				item.Tag = itemTag;
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

		void SetProgramPaths()
		{
			string rootPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "tools");
#if DEBUG
			toolStripMenuItem1.Visible = true;
			toolStripMenuItem1.Enabled = true;
#endif
#if !DEBUG
			toolStripMenuItem1.Visible = false;
			toolStripMenuItem1.Enabled = false;
#endif

			samdlStartInfo = new ProcessStartInfo(Path.GetFullPath(Path.Combine(rootPath, "SAMDL.exe")));
			salvlStartInfo = new ProcessStartInfo(Path.GetFullPath(Path.Combine(rootPath, "SALVL.exe")));
			texeditStartInfo = new ProcessStartInfo(Path.GetFullPath(Path.Combine(rootPath, "TextureEditor.exe")));
			sadxlvl2StartInfo = new ProcessStartInfo(Path.GetFullPath(Path.Combine(rootPath, "SADXLVL2.exe")));
			sadxsndsharpStartInfo = new ProcessStartInfo(Path.GetFullPath(Path.Combine(rootPath, "SADXsndSharp.exe")));
			sadxtweakerStartInfo = new ProcessStartInfo(Path.GetFullPath(Path.Combine(rootPath, "SADXTweaker2.exe")));
			sadxfonteditStartInfo = new ProcessStartInfo(Path.GetFullPath(Path.Combine(rootPath, "SADXFontEdit.exe")));
			sasaveStartInfo = new ProcessStartInfo(Path.GetFullPath(Path.Combine(rootPath, "SASave.exe")));
			sa2eventviewStartInfo = new ProcessStartInfo(Path.GetFullPath(Path.Combine(rootPath, "SA2EventViewer.exe")));
			sa2evtexteditStartInfo = new ProcessStartInfo(Path.GetFullPath(Path.Combine(rootPath, "SA2CutsceneTextEditor.exe")));
			sa2streditStartInfo = new ProcessStartInfo(Path.GetFullPath(Path.Combine(rootPath, "SA2MessageFileEditor.exe")));
			sa2stgselStartInfo = new ProcessStartInfo(Path.GetFullPath(Path.Combine(rootPath, "SA2StageSelEdit.exe")));
			datatoolboxStartInfo = new ProcessStartInfo(Path.GetFullPath(Path.Combine(rootPath, "DataToolbox.exe")));
		}

		private void toolsHub_Shown(object sender, EventArgs e)
		{
			SetProgramPaths();

			switch (hubSettings.AutoUpdate)
			{
				case true:
					autoUpdateToolStripMenuItem.Checked = true;
					frequencyToolStripMenuItem.Enabled = true;
					break;
				case false:
					autoUpdateToolStripMenuItem.Checked = false;
					frequencyToolStripMenuItem.Enabled = false;
					break;
			}

			switch (hubSettings.UpdateFrequency)
			{
				case ProjectSettings.Frequency.daily:
					dailyToolStripMenuItem.Checked = true;
					weeklyToolStripMenuItem.Checked = false;
					monthlyToolStripMenuItem.Checked = false;
					break;
				case ProjectSettings.Frequency.weekly:
					dailyToolStripMenuItem.Checked = false;
					weeklyToolStripMenuItem.Checked = true;
					monthlyToolStripMenuItem.Checked = false;
					break;
				case ProjectSettings.Frequency.monthly:
					dailyToolStripMenuItem.Checked = false;
					weeklyToolStripMenuItem.Checked = false;
					monthlyToolStripMenuItem.Checked = true;
					break;
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

			if (newProjFile != null)
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

		private void buildRunGameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			gameOptionsDiag.ShowDialog();
		}

		//Tools
		//General Tools Initializers
		private void sAMDLToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process samdlProcess = Process.Start(samdlStartInfo);
		}

		private void sALVLToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process salvlProcess = Process.Start(salvlStartInfo);
		}

		private void textureEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process texEditProcess = Process.Start(texeditStartInfo);
		}

		//SADX Tools Initializers
		private void sADXLVL2ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (projectDirectory != null && setGame == "SADXPC")
			{
				string projectArgumentsPath = $"\"{Path.Combine(projectDirectory, "sadxlvl.ini")}\" \"{gameDirectory}\"";

				sadxlvl2StartInfo.Arguments = projectArgumentsPath;
			}

			Process sadxlvl2Process = Process.Start(sadxlvl2StartInfo);
		}

		private void sADXTweakerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process tweakerProcess = Process.Start(sadxtweakerStartInfo);
		}

		private void sADXsndSharpToolStripMenuItem_Click(object sender, EventArgs e)
		{ 
			Process sndSharpProcess = Process.Start(sadxsndsharpStartInfo);
		}

		private void sAFontEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{ 
			Process saFontProcess = Process.Start(sadxfonteditStartInfo);
		}

		private void sASaveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process saSaveProcess = Process.Start(sasaveStartInfo);
		}

		//SA2 Tools Initializers
		private void sA2EventViewerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process saSaveProcess = Process.Start(sa2eventviewStartInfo);
		}

		private void sA2CutsceneTextEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process saSaveProcess = Process.Start(sa2evtexteditStartInfo);
		}

		private void sA2MessageEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process saSaveProcess = Process.Start(sa2streditStartInfo);
		}

		private void sA2StageSelectEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process saSaveProcess = Process.Start(sa2stgselStartInfo);
		}

		//Data Extractor/Convert (new Split UI)
		private void splitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process saSaveProcess = Process.Start(datatoolboxStartInfo);
		}

		private void projectConverterToolStripMenuItem_Click(object sender, EventArgs e)
		{
			projectConverter.ShowDialog();
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

		private void toolStripMenuItem2_Click(object sender, EventArgs e)
		{
			abtWindow.Show();
		}

		//Additiional Links
		private void blenderSASupportAddonToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				Process.Start("https://github.com/Justin113D/BlenderSASupport");
			}
			catch
			{
				MessageBox.Show("Something went wrong, could not open link in browser.");
			}
		}

		private void sonicAudioToolsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				Process.Start("https://github.com/blueskythlikesclouds/SonicAudioTools");
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

		//Edit/Context Menu
		void reloadFolder()
		{
			DirectoryInfo info = (DirectoryInfo)treeView1.SelectedNode.Tag;
			ListViewItem item = null;
			listView1.Items.Clear();
			LoadFiles(info, item);
		}

		private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			if (listView1.SelectedItems.Count < 1)
			{
				editOpen.Enabled = false;
				editCopy.Enabled = false;
				editDel.Enabled = false;
				editConvert.Enabled = false;
				editToData.Enabled = false;
				editToJson.Enabled = false;
				cmsOpen.Enabled = false;
				cmsCopy.Enabled = false;
				cmsDelete.Enabled = false;
				cmsConvert.Enabled = false;
				cmsToData.Enabled = false;
				cmsToJson.Enabled = false;
			}
			else if (((itemTags)listView1.SelectedItems[0].Tag).Type == "dir")
			{
				editOpen.Enabled = true;
				cmsOpen.Enabled = true;
			}
			else
			{
				string itemPath = ((itemTags)listView1.SelectedItems[0].Tag).Path;
				string itemExt = Path.GetExtension(itemPath);
				switch (itemExt)
				{
					case ".sa1mdl":
					case ".sa2mdl":
					case ".sa2bmdl":
					case ".sa1lvl":
					case ".sa2lvl":
					case ".sa2blvl":
						editOpen.Enabled = true;
						editCopy.Enabled = true;
						editDel.Enabled = true;
						editConvert.Enabled = true;
						editToData.Enabled = false;
						cmsOpen.Enabled = true;
						cmsCopy.Enabled = true;
						cmsDelete.Enabled = true;
						cmsConvert.Enabled = true;
						cmsToData.Enabled = true;
						break;
					case ".saanim":
						editOpen.Enabled = true;
						editCopy.Enabled = true;
						editDel.Enabled = true;
						editConvert.Enabled = true;
						editToData.Enabled = true;
						editToJson.Enabled = true;
						cmsOpen.Enabled = true;
						cmsCopy.Enabled = true;
						cmsDelete.Enabled = true;
						cmsConvert.Enabled = true;
						cmsToData.Enabled = true;
						cmsToJson.Enabled = true;
						break;
					default:
						editOpen.Enabled = true;
						editCopy.Enabled = true;
						editDel.Enabled = true;
						cmsOpen.Enabled = true;
						cmsCopy.Enabled = true;
						cmsDelete.Enabled = true;
						break;
				}
			}

			if (copyPaths != null)
			{
				editPaste.Enabled = true;
				cmsPaste.Enabled = true;
			}
		}

		void openListItem(ListViewItem item)
		{
			TreeNode selNode = new TreeNode();
			string itemName = item.Text;
			string itemPath = ((itemTags)item.Tag).Path;
			string itemExt;

			if (((itemTags)listView1.SelectedItems[0].Tag).Type == "dir")
			{
				selNode = SearchTreeView(itemName, treeView1.SelectedNode.Nodes);
				itemExt = "dir";
			}
			else
			{
				itemExt = Path.GetExtension(itemPath);
			}

			if (listView1.SelectedItems.Count > 0)
			{
				switch (itemExt.ToLower())
				{
					case "dir":
						SelectListViewNode(selNode);
						treeView1.SelectedNode = selNode;
						break;
					case ".sa1mdl":
					case ".sa2mdl":
					case ".sa2bmdl":
						samdlStartInfo.Arguments = $"\"{itemPath}\"";
						Process.Start(samdlStartInfo);
						break;
					case ".sa1lvl":
					case ".sa2lvl":
					case ".sa2blvl":
						salvlStartInfo.Arguments = $"\"{itemPath}\"";
						Process.Start(salvlStartInfo);
						break;
					case ".pvm":
					case ".pvmx":
					case ".gvm":
					case ".pak":
						texeditStartInfo.Arguments = $"\"{itemPath}\"";
						Process.Start(texeditStartInfo);
						break;
					case ".txt":
					case ".dds":
					case ".jpg":
					case ".png":
					case ".bmp":
					case ".gif":
						Process.Start($"\"{itemPath}\"");
						break;
					case ".prs":
						if (itemName.Contains("mdl"))
						{
							samdlStartInfo.Arguments = $"\"{itemPath}\"";
							Process.Start(samdlStartInfo);
						}
						else if (itemName.Contains("tex") || itemName.Contains("tx") || itemName.Contains("bg"))
						{
							texeditStartInfo.Arguments = $"\"{itemPath}\"";
							Process.Start(texeditStartInfo);
						}
						break;
					case ".ini":
						switch (itemName)
						{
							case "sadxlvl.ini":
								sadxlvl2StartInfo.Arguments = $"\"{Path.Combine(projectDirectory, "sadxlvl.ini")}\" \"{gameDirectory}\"";

								Process.Start(sadxlvl2StartInfo);
								break;
							case "mod.ini":
								projectEditorDiag.ShowDialog();
								break;
							default:
								if (itemName.Contains("_data"))
								{
									sadxtweakerStartInfo.Arguments = $"\"{Path.Combine(projectDirectory, itemName)}\"";
									Process.Start(sadxtweakerStartInfo);
								}
								else
									Process.Start($"\"{itemPath}\"");
								break;
						}
						break;
				}
			}
		}

		private void editOpen_Click(object sender, EventArgs e)
		{
			if (listView1.SelectedItems[0] != null)
				openListItem(listView1.SelectedItems[0]);
		}

		private void editToData_Click(object sender, EventArgs e)
		{
			if (listView1.SelectedItems.Count > 0)
			{
				foreach (ListViewItem selItem in listView1.SelectedItems)
				{
					SonicRetro.SAModel.DataToolbox.StructConversion.ConvertFileToText(
						((itemTags)selItem.Tag).Path,
						SonicRetro.SAModel.DataToolbox.StructConversion.TextType.CStructs,
						Path.Combine(projectDirectory, "Source", (Path.GetFileNameWithoutExtension(((itemTags)selItem.Tag).Path + ".c"))));
				}
			}
		}

		private void editToJson_Click(object sender, EventArgs e)
		{
			if (listView1.SelectedItems != null)
			{
				foreach (ListViewItem selItem in listView1.SelectedItems)
				{
					if (Path.GetExtension(((itemTags)selItem.Tag).Path) == ".saanim")
					{
						SonicRetro.SAModel.DataToolbox.StructConversion.ConvertFileToText(
							((itemTags)selItem.Tag).Path,
							SonicRetro.SAModel.DataToolbox.StructConversion.TextType.JSON);
					}
					
				}
			}
			reloadFolder();
		}

		private void editCopy_Click(object sender, EventArgs e)
		{
			if (listView1.SelectedItems.Count > 0)
			{
				copyPaths = new List<string>();
				foreach (ListViewItem selItem in listView1.SelectedItems)
				{
					if (selItem != null && ((itemTags)selItem.Tag).Type != "dir")
						copyPaths.Add(((itemTags)selItem.Tag).Path);
				}
			}
		}

		private void editPaste_Click(object sender, EventArgs e)
		{
			if (copyPaths.Count > 0)
			{
				DirectoryInfo info = (DirectoryInfo)treeView1.SelectedNode.Tag;
				foreach (string file in copyPaths)
				{
					if (File.Exists(file))
					{
						string destFile = Path.Combine(info.FullName, Path.GetFileName(file));
						File.Copy(file, destFile, true);
					}
				}
				reloadFolder();
			}
		}

		private void editDel_Click(object sender, EventArgs e)
		{
			if (listView1.SelectedItems.Count > 0)
			{
				DialogResult delCheck;
				if (listView1.SelectedItems.Count == 1)
					delCheck = MessageBox.Show(("You are about to delete this file.\n\nAre you sure you want to delete this file?"), "File Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
				else
					delCheck = MessageBox.Show(("You are about to delete " + listView1.SelectedItems.Count.ToString() + " files\n\nAre you sure you want to delete these files?"), "File Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
				if (delCheck == DialogResult.Yes)
				{
					foreach (ListViewItem selItem in listView1.SelectedItems)
					{
						File.Delete(((itemTags)selItem.Tag).Path);
					}
					reloadFolder();
				}
			}
		}

		private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			editOpen_Click(sender, e);
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			editCopy_Click(sender, e);
		}

		private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			editPaste_Click(sender, e);
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			editDel_Click(sender, e);
		}

		private void cmsToData_Click(object sender, EventArgs e)
		{
			editToData_Click(sender, e);
		}

		private void cmsToJson_Click(object sender, EventArgs e)
		{
			editToJson_Click(sender, e);
		}

		//Navigation
		private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			SelectListViewNode(e.Node);
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			browseCurDirectory.Text = treeView1.SelectedNode.FullPath;
			if (treeView1.SelectedNode.Parent != null)
				browseBack.Enabled = true;
			else
				browseBack.Enabled = false;
		}

		private void listView1_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				if (listView1.FocusedItem.Bounds.Contains(e.Location) && listView1.SelectedItems[0] != null)
				{
					contextMenuStrip1.Show(Cursor.Position);
				}
			}
		}

		private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				openListItem(listView1.SelectedItems[0]);
			}
		}

		private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			listView1.ListViewItemSorter = new ListViewItemComparer(e.Column);
		}

		private void browseBack_Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode.Parent != null)
			{
				SelectListViewNode(treeView1.SelectedNode.Parent);
				treeView1.SelectedNode = treeView1.SelectedNode.Parent;
			}
		}

		private void browseOpenExplorer_Click(object sender, EventArgs e)
		{
			Process.Start($"{((DirectoryInfo)treeView1.SelectedNode.Tag).FullName}");
		}

		//Settings Handles
		private void autoUpdateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool curState = autoUpdateToolStripMenuItem.Checked;

			if (curState == false)
			{
				autoUpdateToolStripMenuItem.Checked = true;
				frequencyToolStripMenuItem.Enabled = true;

				hubSettings.AutoUpdate = true;
			}
			else
			{
				autoUpdateToolStripMenuItem.Checked = false;
				frequencyToolStripMenuItem.Enabled = false;

				hubSettings.AutoUpdate = false;
			}

			hubSettings.Save();
		}

		private void dailyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			dailyToolStripMenuItem.Checked = true;
			weeklyToolStripMenuItem.Checked = false;
			monthlyToolStripMenuItem.Checked = false;

			hubSettings.UpdateFrequency = ProjectSettings.Frequency.daily;
			hubSettings.Save();
		}

		private void weeklyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			dailyToolStripMenuItem.Checked = false;
			weeklyToolStripMenuItem.Checked = true;
			monthlyToolStripMenuItem.Checked = false;

			hubSettings.UpdateFrequency = ProjectSettings.Frequency.weekly;
			hubSettings.Save();
		}

		private void monthlyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			dailyToolStripMenuItem.Checked = false;
			weeklyToolStripMenuItem.Checked = false;
			monthlyToolStripMenuItem.Checked = true;

			hubSettings.UpdateFrequency = ProjectSettings.Frequency.monthly;
			hubSettings.Save();
		}

		//Debug/Developer Only Options
		private void toolStripMenuItem1_Click(object sender, EventArgs e)
		{
			templateWriter.ShowDialog();
		}
	}
}
