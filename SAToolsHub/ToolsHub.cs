using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Net;
using System.ComponentModel;
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
		private ListViewColumnSorter lvwColumnSorter;

		//Variables
		public static string newProjFile { get; set; }
		string projXML = "";
        public bool canUseSALVL { get; set; }
        public static string projectDirectory { get; set; }
		public static string setGame { get; set; }
		public static string gameDirectory { get; set; }
        public static string gameSystemDirectory { get; set; }
		public static List<Templates.SplitEntry> projSplitEntries { get; set; }
		public static List<Templates.SplitEntryMDL> projSplitMDLEntries { get; set; }
		public static ProjectSettings hubSettings { get; set; }
		List<string> copyPaths;

		public class itemTags
		{
			public string Type { get; set; }
			public string Path { get; set; }
			public long Size { get; set; }
			public DateTime Access { get; set; }
		}

		//Program Paths
		ProcessStartInfo samdlStartInfo;
		ProcessStartInfo texeditStartInfo;
		ProcessStartInfo salvlStartInfo;
		ProcessStartInfo sadxsndsharpStartInfo;
		ProcessStartInfo sadxtweakerStartInfo;
		ProcessStartInfo sadxfonteditStartInfo;
		ProcessStartInfo sasaveStartInfo;
		ProcessStartInfo sa2eventviewStartInfo;
		ProcessStartInfo sa2evtexteditStartInfo;
		ProcessStartInfo sa2streditStartInfo;
		ProcessStartInfo sa2stgselStartInfo;
		ProcessStartInfo datatoolboxStartInfo;
		ProcessStartInfo sadlctoolStartInfo;

		public SAToolsHub()
		{
			InitializeComponent();
			lvwColumnSorter = new ListViewColumnSorter();
			this.listView1.ListViewItemSorter = lvwColumnSorter;

			hubSettings = ProjectSettings.Load();

			//if (Program.Arguments.Length > 0 && Program.Arguments[0].Contains(".sap"))
				//openProject(ProjectFunctions.openProjectFileString(Program.Arguments[0]));

			projectCreateDiag = new newProj();
			projectEditorDiag = new editProj();
			buildWindowDiag = new buildWindow();
			templateWriter = new templateWriter();
			gameOptionsDiag = new gameOptions();
			projectConverter = new projConv();

			SetProgramPaths();

			switch (hubSettings.UpdateCheck)
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

			switch (hubSettings.UpdateUnit)
			{
				case ProjectSettings.UpdateUnits.Always:
					alwaysToolStripMenuItem.Checked = true;
					dailyToolStripMenuItem.Checked = false;
					weeklyToolStripMenuItem.Checked = false;
					break;
				case ProjectSettings.UpdateUnits.Days:
					alwaysToolStripMenuItem.Checked = false;
					dailyToolStripMenuItem.Checked = true;
					weeklyToolStripMenuItem.Checked = false;
					break;
				case ProjectSettings.UpdateUnits.Weeks:
					alwaysToolStripMenuItem.Checked = false;
					dailyToolStripMenuItem.Checked = false;
					weeklyToolStripMenuItem.Checked = true;
					break;
			}
		}

		// TODO: ToolsHub - Migrate some Additional Functions out.
		#region Additional Functions
		private void initProject()
		{
			Templates.ProjectTemplate projectFile;
			OpenFileDialog openFileDialog1 = new OpenFileDialog();
			openFileDialog1.Filter = "Project File (*.sap)|*.sap";
			openFileDialog1.RestoreDirectory = true;

			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				resetOpenProject();
				projXML = openFileDialog1.FileName;
				projectFile = ProjectFunctions.openProjectFileString(projXML);
				openProject(projectFile);
			}
		}

		private void openProject(Templates.ProjectTemplate projFile)
		{
			string rootFolder;
			bool validFolders = true;
			bool sapChanged = false;

			setGame = projFile.GameInfo.GameName;
			
			projectDirectory = projFile.GameInfo.ProjectFolder;
			gameDirectory = projFile.GameInfo.GameFolder;
            gameSystemDirectory = Path.Combine(projFile.GameInfo.GameFolder, projFile.GameInfo.GameDataFolder);
			// Check for valid paths just in case the user moved folders.
			if (!Directory.Exists(projectDirectory))
			{
			MissingProjectFolder:
				DialogResult projDirMissing = MessageBox.Show(("Project Directory not found. Please locate the correct folder."), "Missing Directory", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				if (projDirMissing == DialogResult.OK)
				{
					var fsd = new FolderSelect.FolderSelectDialog();
					fsd.Title = "Please select the correct project folder";
					if (fsd.ShowDialog(IntPtr.Zero))
					{
						projectDirectory = fsd.FileName;
						projFile.GameInfo.ProjectFolder = projectDirectory;
						sapChanged = true;
					}
					else
					{
						DialogResult noProjFolder = MessageBox.Show(("No folder selected."), "Missing Directory", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
						if (noProjFolder == DialogResult.Retry)
							goto MissingProjectFolder;
						else
							validFolders = false;
					}
				}
			}

			if (!Directory.Exists(gameDirectory))
			{
			MissingGameFolder:
				DialogResult gameDirMissing = MessageBox.Show(("Game Directory not found. Please locate the correct folder."), "Missing Directory", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				if (gameDirMissing == DialogResult.OK)
				{
					var fsd = new FolderSelect.FolderSelectDialog();
					fsd.Title = "Please select the correct game folder";
					if (fsd.ShowDialog(IntPtr.Zero))
					{
						if (File.Exists(Path.Combine(fsd.FileName, projFile.GameInfo.CheckFile)))
						{
							gameDirectory = fsd.FileName;
							projFile.GameInfo.GameFolder = gameDirectory;
							sapChanged = true;
						}
						else
						{
							DialogResult invalidGameFolder = MessageBox.Show(("Invalid folder selected."), "Missing Directory", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
							if (invalidGameFolder == DialogResult.Retry)
								goto MissingGameFolder;
							else
								validFolders = false;
						}
					}
					else
					{
						DialogResult noGameFolder = MessageBox.Show(("No folder selected."), "Missing Directory", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
						if (noGameFolder == DialogResult.Retry)
							goto MissingGameFolder;
						else
							validFolders = false;
					}
				}
			}

			// TODO: Add Paths update function for sap file here.
			if (sapChanged)
			{
				// If new Paths provided, update sap file
			}

			if (validFolders)
			{
                rootFolder = setGame + " Game Files";
				PopulateTreeView(projectDirectory);
				PopulateTreeView(gameSystemDirectory);
				treeView1.Nodes[1].Text = rootFolder;
				this.treeView1.NodeMouseClick +=
					new TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);

				projSplitEntries = projFile.SplitEntries;
                if (projFile.SplitMDLEntries != null)
                    projSplitMDLEntries = projFile.SplitMDLEntries;

				toggleButtons(setGame);
				closeProjectToolStripMenuItem.Enabled = true;
                canUseSALVL = projFile.GameInfo.CanUseSALVL;
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
			browseBack.Enabled = false;
			browseCurDirectory.TextBox.Clear();
			browseOpenExplorer.Enabled = false;

			//reset DX game buttons
			tsSADXTweaker.Visible = false;
			tsSADXsndSharp.Visible = false;
			tsSADXFontEdit.Visible = false;

			//reset SA2 game buttons
			tsSA2EvView.Visible = false;
			tsSA2EvTxt.Visible = false;
			tsSA2MsgEdit.Visible = false;
			tsSA2StgSel.Visible = false;

			//reset Game strings
			setGame = "";
			projectDirectory = "";
			gameDirectory = "";
		}

		void SetProgramPaths()
		{
			string rootPath = "";
			if (Directory.Exists(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "tools")))
				rootPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "tools");
			else
				rootPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "../../tools");
#if DEBUG
			toolStripMenuItem1.Visible = true;
			toolStripMenuItem1.Enabled = true;
#endif
#if !DEBUG
			toolStripMenuItem1.Visible = false;
			toolStripMenuItem1.Enabled = false;
#endif

			samdlStartInfo = new ProcessStartInfo(Path.GetFullPath(Path.Combine(rootPath, "SAMDL.exe")));
			texeditStartInfo = new ProcessStartInfo(Path.GetFullPath(Path.Combine(rootPath, "TextureEditor.exe")));
			salvlStartInfo = new ProcessStartInfo(Path.GetFullPath(Path.Combine(rootPath, "SALVL.exe")));
			sadxsndsharpStartInfo = new ProcessStartInfo(Path.GetFullPath(Path.Combine(rootPath, "SADXsndSharp.exe")));
			sadxtweakerStartInfo = new ProcessStartInfo(Path.GetFullPath(Path.Combine(rootPath, "SADXTweaker2.exe")));
			sadxfonteditStartInfo = new ProcessStartInfo(Path.GetFullPath(Path.Combine(rootPath, "SADXFontEdit.exe")));
			sasaveStartInfo = new ProcessStartInfo(Path.GetFullPath(Path.Combine(rootPath, "SASave.exe")));
			sa2eventviewStartInfo = new ProcessStartInfo(Path.GetFullPath(Path.Combine(rootPath, "SA2EventViewer.exe")));
			sa2evtexteditStartInfo = new ProcessStartInfo(Path.GetFullPath(Path.Combine(rootPath, "SA2CutsceneTextEditor.exe")));
			sa2streditStartInfo = new ProcessStartInfo(Path.GetFullPath(Path.Combine(rootPath, "SA2MessageFileEditor.exe")));
			sa2stgselStartInfo = new ProcessStartInfo(Path.GetFullPath(Path.Combine(rootPath, "SA2StageSelEdit.exe")));
			datatoolboxStartInfo = new ProcessStartInfo(Path.GetFullPath(Path.Combine(rootPath, "DataToolbox.exe")));
			sadlctoolStartInfo = new ProcessStartInfo(Path.GetFullPath(Path.Combine(rootPath, "VMSEditor.exe")));
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

		string getFileSize(long length)
		{
			if (length > 0)
			{
				return ((length / (1024.0f)).ToString("#,0.00 KBs"));
			}
			else
				return "0";
		}

		void LoadFiles(DirectoryInfo nodeDirInfo, ListViewItem item)
		{
			ListViewItem.ListViewSubItem[] subItems;
			string subItemType = "";
			string tagType = "";
			foreach (DirectoryInfo dir in nodeDirInfo.GetDirectories())
			{
				if (dir.Name != "dllcache")
				{
					item = new ListViewItem(dir.Name, (int)item_icons.folder);
					subItems = new ListViewItem.ListViewSubItem[] {
						new ListViewItem.ListViewSubItem(item, "Directory"),
						new ListViewItem.ListViewSubItem(item, dir.LastAccessTime.ToShortDateString()),
						new ListViewItem.ListViewSubItem(item, null)
					};
					itemTags dirTag = new itemTags();
					dirTag.Type = "dir";
					dirTag.Path = dir.FullName;
					dirTag.Size = 0;
					dirTag.Access = dir.LastAccessTime;
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
							subItemType = "Model File";
							tagType = "mdl";
							item.ImageIndex = (int)item_icons.model;
							item.ToolTipText = "Double click to open in SAMDL";
							break;
						}
					case ".sa1lvl":
					case ".sa2lvl":
					case ".sa2blvl":
						{
							subItemType = "Level File";
							tagType = "lvl";
							item.ImageIndex = (int)item_icons.level;
							item.ToolTipText = "Double click to open in SALVL";
							break;
						}
					case ".ini":
						{
							subItemType = "Data File";
							tagType = "txt";
							item.ImageIndex = (int)item_icons.data;
							switch (fileName)
							{
								case "mod":
									item.ToolTipText = "Double click to open Mod Info Editor";
									break;
								case "sadxlvl":
									item.ToolTipText = "Double click to open SALVL";
									break;
								default:
									item.ToolTipText = "Double click to open in the default text editor.";
									break;
							}
							break;
						}
					case ".txt":
						{
							subItemType = "Text File";
							tagType = "txt";
							item.ImageIndex = (int)item_icons.document;
							item.ToolTipText = "Double click to open in the default text editor.";
							break;
						}
					case ".bin":
						{
							tagType = "bin";
							if (fileName.Contains("cam"))
							{
								subItemType = "Camera Layout";
								item.ImageIndex = (int)item_icons.camera;
							}
							else if (fileName.Contains("set"))
							{
								subItemType = "Object Layout";
								item.ImageIndex = (int)item_icons.setobj;
							}
							else
							{
								subItemType = "Binary File";
							}
							break;
						}
					case ".pvm":
					case ".pvmx":
					case ".gvm":
					case ".pak":
						{
							subItemType = "Texture Archive";
							tagType = "tex";
							item.ImageIndex = (int)item_icons.texture;
							item.ToolTipText = "Double click to open in Texture Editor";
							break;
						}
					case ".pvr":
					case ".gvr":
						{
							subItemType = "Sega VR Image";
							tagType = "vr";
							item.ImageIndex = (int)item_icons.texture;
							break;
						}
					case ".dds":
					case ".jpg":
					case ".png":
					case ".bmp":
					case ".gif":
						{
							subItemType = "Image File";
							tagType = "img";
							item.ImageIndex = (int)item_icons.texture;
							break;
						}
					case ".pvp":
					case ".gvp":
					case ".plt":
						{
							subItemType = "Palette File";
							tagType = "plt";
							item.ImageIndex = (int)item_icons.texture;
							break;
						}
					case ".prs":
						{
							if (fileName.Contains("mdl"))
							{
								subItemType = "Compressed Model Archive";
								tagType = "mdl";
								item.ToolTipText = "Double click to open in SAMDL";
							}
							else if (fileName.Contains("tex") || fileName.Contains("tx") || fileName.Contains("bg"))
							{
								subItemType = "Compressed Texture Archive";
								tagType = "tex";
								item.ToolTipText = "Double click to open in Texture Editor";
							}
							else if (fileName.Contains("mtn"))
							{
								subItemType = "Compressed Animations Archive";
								tagType = "anm";
							}
							else
							{
								subItemType = "Compressed File";
								tagType = "file";
							}
							item.ImageIndex = (int)item_icons.compress;
							break;
						}
					case ".saanim":
						{
							subItemType = "Animation File";
							tagType = "anm";
							item.ImageIndex = (int)item_icons.anim;
							break;
						}
					case ".adx":
					case ".mlt":
					case ".csb":
					case ".wma":
					case ".dat":
						{
							subItemType = "Audio File";
							tagType = "snd";
							item.ImageIndex = (int)item_icons.audio;
							break;
						}
					case ".wmv":
					case ".sfd":
					case ".mpg":
					case ".m1v":
						{
							subItemType = "Video File";
							tagType = "vid";
							item.ImageIndex = (int)item_icons.camera;
							break;
						}
					case ".c":
					case ".cs":
						{
							subItemType = "Code File";
							tagType = "txt";
							item.ImageIndex = (int)item_icons.cfile;
							break;
						}
					case ".json":
						{
							subItemType = "Javascript File";
							tagType = "txt";
							item.ImageIndex = (int)item_icons.json;
							break;
						}
					case ".action":
						{
							subItemType = "Animation Linker";
							tagType = "txt";
							item.ImageIndex = (int)item_icons.document;
							break;
						}
					default:
						{
							subItemType = "File";
							tagType = "file";
							item.ImageIndex = (int)item_icons.file;
							break;
						}
				}
				subItems = new ListViewItem.ListViewSubItem[] {
					new ListViewItem.ListViewSubItem(item, subItemType),
					new ListViewItem.ListViewSubItem(item, file.LastAccessTime.ToShortDateString()),
					new ListViewItem.ListViewSubItem(item, SizeSuffix.GetSizeSuffix(file.Length))
				};
				itemTags itemTag = new itemTags();
				itemTag.Type = tagType;
				itemTag.Path = file.FullName;
				itemTag.Size = file.Length;
				itemTag.Access = file.LastAccessTime;
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
					tsSALVL.Visible = true;
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

		void reloadFolder()
		{
			DirectoryInfo info = (DirectoryInfo)treeView1.SelectedNode.Tag;
			ListViewItem item = null;
			listView1.Items.Clear();
			LoadFiles(info, item);
		}

		void openListItem(ListViewItem item)
		{
			TreeNode selNode = new TreeNode();
			string itemName = item.Text;
			string itemPath = ((itemTags)item.Tag).Path;
			string itemType = ((itemTags)item.Tag).Type;

			if (((itemTags)listView1.SelectedItems[0].Tag).Type == "dir")
			{
				selNode = SearchTreeView(itemName, treeView1.SelectedNode.Nodes);
			}

			if (listView1.SelectedItems.Count > 0)
			{
				switch (itemType)
				{
					case "dir":
						SelectListViewNode(selNode);
						treeView1.SelectedNode = selNode;
						break;
					case "mdl":
						samdlStartInfo.Arguments = $"\"{itemPath}\"";
						Process.Start(samdlStartInfo);
						samdlStartInfo.Arguments = "";
						break;
					case "lvl":
						salvlStartInfo.Arguments = $"\"{itemPath}\"";
						Process.Start(salvlStartInfo);
						salvlStartInfo.Arguments = "";
						break;
					case "tex":
						texeditStartInfo.Arguments = $"\"{itemPath}\"";
						Process.Start(texeditStartInfo);
						texeditStartInfo.Arguments = "";
						break;
					case "txt":
						switch (itemName)
						{
							case "sadxlvl.ini":
								salvlStartInfo.Arguments = $"\"{projXML}\"";

								Process.Start(salvlStartInfo);
								salvlStartInfo.Arguments = "";
								break;
							case "mod.ini":
								projectEditorDiag.ShowDialog();
								break;
							default:
								if (itemName.Contains("_data") && setGame == "SADX")
								{
									sadxtweakerStartInfo.Arguments = $"\"{Path.Combine(projectDirectory, itemName)}\"";
									Process.Start(sadxtweakerStartInfo);
									sadxtweakerStartInfo.Arguments = "";
								}
								else
									Process.Start($"\"{itemPath}\"");
								break;
						}
						break;
					case "img":
						Process.Start($"\"{itemPath}\"");
						break;
					case "snd":
						if (itemName.Contains("dat"))
						{
							sadxsndsharpStartInfo.Arguments = $"\"{itemPath}\"";
							Process.Start(sadxsndsharpStartInfo);
							sadxsndsharpStartInfo.Arguments = "";
						}
						else
							Process.Start($"\"{itemPath}\"");
						break;
					case "vid":
						Process.Start($"\"{itemPath}\"");
						break;
				}
			}
		}
		#endregion

		#region Menu Projects
		//Settings
		private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			checkForUpdatesToolStripMenuItem.Enabled = false;
			tsUpdate.Enabled = false;

			if (CheckForUpdates(true))
			{
				return;
			}
			else
			{
				tsUpdate.Enabled = true;
				checkForUpdatesToolStripMenuItem.Enabled = true;
			}
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
				openProject(ProjectFunctions.openProjectFileString(newProjFile));
				projXML = newProjFile;
				newProjFile = null;
			}
		}

		private void openProjectToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			initProject();
		}

		private void editProjectInfoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			projectEditorDiag.ShowDialog();
		}

		private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			projectDirectory = null;
			projXML = null;
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
		#endregion

		#region Menu Game Tools
		//General Tools Initializers
		private void sAMDLToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process samdlProcess = Process.Start(samdlStartInfo);
		}

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            string projectArgumentsPath = canUseSALVL ? $"\"{projXML}\"" : "";
            if (projectDirectory != null)
                salvlStartInfo.Arguments = projectArgumentsPath;
            Process salvlProcess = Process.Start(salvlStartInfo);
        }

		private void textureEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process texEditProcess = Process.Start(texeditStartInfo);
		}

		//SADX Tools Initializers
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

		private void sADLCToolToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process saDLCToolProcess = Process.Start(sadlctoolStartInfo);
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
			DialogResult convWarning = MessageBox.Show(("This feature will create an sap file for your projects.\n\nSome tools may not function properly with older projects., notably SALVL." +
				"\n\nWould you like to continue with the project conversion?"), "Project Conversion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			if (convWarning == DialogResult.Yes)
				projectConverter.ShowDialog();
		}
		#endregion

		#region Web Links
		//Resources
		private void sAToolsWikiToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				Process.Start("https://github.com/X-Hax/sa_tools/wiki");
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
				Process.Start("https://info.org/SCHG:Sonic_Adventure_DX:_PC");
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
				Process.Start("https://info.org/SCHG:Sonic_Adventure_2_(PC)");
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
				Process.Start("https://github.com/X-Hax/sa_tools/issues");
			}
			catch
			{
				MessageBox.Show("Something went wrong, could not open link in browser.");
			}
		}
		#endregion

		#region Toolstrip Buttons
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

		private void tsTexEdit_Click(object sender, EventArgs e)
		{
			textureEditorToolStripMenuItem_Click(sender, e);
		}

		private void tsSADXLVL2_Click(object sender, EventArgs e)
		{
			toolStripMenuItem3_Click(sender, e);
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
		#endregion

		#region Edit Menu
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
						editToData.Enabled = true;
						cmsOpen.Enabled = true;
						cmsCopy.Enabled = true;
						cmsDelete.Enabled = true;
						cmsConvert.Enabled = true;
						cmsToData.Enabled = true;
						break;
					case ".saanim":
						editCopy.Enabled = true;
						editDel.Enabled = true;
						editConvert.Enabled = true;
						editToData.Enabled = true;
						editToJson.Enabled = true;
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

		private void editOpen_Click(object sender, EventArgs e)
		{
			if (listView1.SelectedItems[0] != null)
				openListItem(listView1.SelectedItems[0]);
		}

		private void editToData_Click(object sender, EventArgs e)
		{
			string outDir = Path.Combine(projectDirectory, "Code");
			if (!Directory.Exists(outDir))
			{
				Directory.CreateDirectory(outDir);
				treeView1.Nodes.Clear();
				PopulateTreeView(projectDirectory);
				PopulateTreeView(gameDirectory);
			}

			if (listView1.SelectedItems.Count > 0)
			{
				foreach (ListViewItem selItem in listView1.SelectedItems)
				{
					string filename = ((itemTags)selItem.Tag).Path;
					string outName = Path.Combine(outDir, (Path.GetFileNameWithoutExtension(((itemTags)selItem.Tag).Path))) + ".c";

					SAModel.DataToolbox.StructConversion.ConvertFileToText(filename, SAModel.DataToolbox.StructConversion.TextType.CStructs, outName);
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
						SAModel.DataToolbox.StructConversion.ConvertFileToText(
							((itemTags)selItem.Tag).Path,
							SAModel.DataToolbox.StructConversion.TextType.JSON);
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
				DialogResult delCheck = MessageBox.Show(("You are about to delete " + listView1.SelectedItems.Count.ToString() + " file(s).\n\nAre you sure you want to delete these file(s)?"), "File Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
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
		#endregion

		#region Navigation
		private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			SelectListViewNode(e.Node);
			treeView1.SelectedNode = e.Node;
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			browseCurDirectory.Text = treeView1.SelectedNode.FullPath;
			if (treeView1.SelectedNode.Parent != null)
				browseBack.Enabled = true;
			else
				browseBack.Enabled = false;

			if (browseOpenExplorer.Enabled == false)
				browseOpenExplorer.Enabled = true;
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
			if (e.Column == lvwColumnSorter.SortColumn)
			{
				// Reverse the current sort direction for this column.
				if (lvwColumnSorter.Order == SortOrder.Ascending)
				{
					lvwColumnSorter.Order = SortOrder.Descending;
				}
				else
				{
					lvwColumnSorter.Order = SortOrder.Ascending;
				}
			}
			else
			{
				// Set the column number that is to be sorted; default to ascending.
				lvwColumnSorter.SortColumn = e.Column;
				lvwColumnSorter.Order = SortOrder.Ascending;
			}

			// Perform the sort with these new sort options.
			this.listView1.Sort();
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
		#endregion

		#region Settings
		private void autoUpdateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool curState = autoUpdateToolStripMenuItem.Checked;

			if (curState == false)
			{
				autoUpdateToolStripMenuItem.Checked = true;
				frequencyToolStripMenuItem.Enabled = true;

				hubSettings.UpdateCheck = true;
			}
			else
			{
				autoUpdateToolStripMenuItem.Checked = false;
				frequencyToolStripMenuItem.Enabled = false;

				hubSettings.UpdateCheck = false;
			}

			hubSettings.Save();
		}

		private void alwaysToolStripMenuItem_Click(object sender, EventArgs e)
		{
			alwaysToolStripMenuItem.Checked = true;
			dailyToolStripMenuItem.Checked = false;
			weeklyToolStripMenuItem.Checked = false;

			hubSettings.UpdateUnit = ProjectSettings.UpdateUnits.Always;
			hubSettings.Save();
		}

		private void dailyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			alwaysToolStripMenuItem.Checked = false;
			dailyToolStripMenuItem.Checked = true;
			weeklyToolStripMenuItem.Checked = false;

			hubSettings.UpdateUnit = ProjectSettings.UpdateUnits.Days;
			hubSettings.Save();
		}

		private void weeklyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			alwaysToolStripMenuItem.Checked = false;
			dailyToolStripMenuItem.Checked = false;
			weeklyToolStripMenuItem.Checked = true;

			hubSettings.UpdateUnit = ProjectSettings.UpdateUnits.Weeks;
			hubSettings.Save();
		}
		#endregion

		#region Update Code
		private bool checkedForUpdates;
		const string updatePath = ".updates";

		private static bool UpdateTimeElapsed(ProjectSettings.UpdateUnits unit, int amount, DateTime start)
		{
			if (unit == ProjectSettings.UpdateUnits.Always)
			{
				return true;
			}

			TimeSpan span = DateTime.UtcNow - start;

			switch (unit)
			{
				case ProjectSettings.UpdateUnits.Hours:
					return span.TotalHours >= amount;

				case ProjectSettings.UpdateUnits.Days:
					return span.TotalDays >= amount;

				case ProjectSettings.UpdateUnits.Weeks:
					return span.TotalDays / 7.0 >= amount;

				default:
					throw new ArgumentOutOfRangeException(nameof(unit), unit, null);
			}
		}

		private bool CheckForUpdates(bool force = false)
		{
			if (!force && !hubSettings.UpdateCheck)
			{
				return false;
			}

			if (!force && !UpdateTimeElapsed(hubSettings.UpdateUnit, hubSettings.UpdateFrequency, DateTime.FromFileTimeUtc(hubSettings.UpdateTime)))
			{
				return false;
			}

			checkedForUpdates = true;
			hubSettings.UpdateTime = DateTime.UtcNow.ToFileTimeUtc();

			if (!File.Exists("satoolsver.txt"))
			{
				return false;
			}

			using (var wc = new WebClient())
			{
				try
				{
					string msg = wc.DownloadString("http://mm.reimuhakurei.net/toolchangelog.php?tool=satools&rev=" + File.ReadAllText("satoolsver.txt"));
					if (msg.Length > 0)
					{
						using (var dlg = new Updater.UpdateMessageDialog("SA Tools", msg.Replace("\n", "\r\n")))
						{
							if (dlg.ShowDialog(this) == DialogResult.Yes)
							{
								DialogResult result = DialogResult.OK;
								do
								{
									try
									{
										if (!Directory.Exists(updatePath))
										{
											Directory.CreateDirectory(updatePath);
										}
									}
									catch (Exception ex)
									{
										result = MessageBox.Show(this, "Failed to create temporary update directory:\n" + ex.Message
																	   + "\n\nWould you like to retry?", "Directory Creation Failed", MessageBoxButtons.RetryCancel);
										if (result == DialogResult.Cancel) return false;
									}
								} while (result == DialogResult.Retry);

								using (var dlg2 = new Updater.LoaderDownloadDialog("http://mm.reimuhakurei.net/SA%20Tools.7z", updatePath))
									if (dlg2.ShowDialog(this) == DialogResult.OK)
									{
										Close();
										return true;
									}
							}
						}
					}
					tsUpdate.Enabled = true;
					checkForUpdatesToolStripMenuItem.Enabled = true;
				}
				catch
				{
					MessageBox.Show(this, "Unable to retrieve update information.", "SA Tools");
				}
			}

			return false;
		}
		#endregion

		#region Debug
		private void toolStripMenuItem1_Click(object sender, EventArgs e)
		{
			templateWriter.ShowDialog();
		}
		#endregion

		private void SAToolsHub_Shown(object sender, EventArgs e)
		{
			if (CheckForUpdates())
				return;
		}

		private void replitFileToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}
	}
}
