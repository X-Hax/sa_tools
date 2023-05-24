using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using SplitTools;
using SplitTools.SplitDLL;

namespace SAModel.SAEditorCommon
{
	public partial class ManualBuildWindow : Form
	{
		public enum AssemblyType { Exe, DLL }

		private string projectFolder = "";
		private string modFolder = "";
		private string gameEXE = "";
		private SplitTools.Game game;
		private Dictionary<string, AssemblyType> assemblies = new Dictionary<string, AssemblyType>();
		private Dictionary<string, ListView> assemblyListViews = new Dictionary<string, ListView>();
		private Dictionary<string, object> assemblyIniFiles = new Dictionary<string, object>(); // gotta box ini data to object so that dll ini and ini data can exist in the same list

		private Dictionary<string, Dictionary<string, bool>> assemblyItemsToExport = new Dictionary<string, Dictionary<string, bool>>();
		private ListViewColumnSorter lvwColumnSorter;
		private ListView backupList = new();

		public ManualBuildWindow()
		{
			InitializeComponent();
		}

		public void Initalize(SplitTools.Game game, string projectName, string projectFolder,
			string modFolder, Dictionary<string, AssemblyType> assemblies, string gameEXE)
		{
			this.projectFolder = projectFolder;
			this.modFolder = modFolder;
			this.assemblies = assemblies;
			this.gameEXE = gameEXE;
			this.game = game;

			LoadingLabel.Visible = true;
		}

		private void LoadIniFiles()
		{
			assemblyIniFiles.Clear();
			assemblyItemsToExport.Clear();

			foreach (KeyValuePair<string, AssemblyType> assembly in assemblies)
			{
				string iniPath = Path.Combine(projectFolder, assembly.Key + "_data.ini");
				Dictionary<string, bool> itemsToExport = new Dictionary<string, bool>();

				switch (assembly.Value)
				{
					case AssemblyType.Exe:
						SplitTools.IniData iniData = StructConverter.StructConverter.LoadINI(iniPath, ref itemsToExport);
						assemblyItemsToExport.Add(assembly.Key, itemsToExport);
						assemblyIniFiles.Add(assembly.Key, iniData);
						break;

					case AssemblyType.DLL:
						DllIniData dllIniData = DLLModGenerator.DLLModGen.LoadINI(iniPath, ref itemsToExport);
						assemblyItemsToExport.Add(assembly.Key, itemsToExport);
						assemblyIniFiles.Add(assembly.Key, dllIniData);
						break;

					default:
						break;
				}
			}
		}

		private void SetTabsForAssemblies()
		{
			lvwColumnSorter = new ListViewColumnSorter();
			assemblyItemTabs.TabPages.Clear();

			foreach (KeyValuePair<string, ListView> listView in assemblyListViews)
			{
				listView.Value.ItemChecked -= listView1_ItemChecked;
				listView.Value.Dispose();
			}

			assemblyListViews.Clear();
			searchBox.Clear();

			foreach (KeyValuePair<string, AssemblyType> assembly in assemblies)
			{
				// create a new tab
				TabPage assemblyPage = new TabPage(assembly.Key);

				// tab needs a docked list view
				ListView tabListView = new ListView();
				tabListView.Name = string.Format("{0} listView", assembly.Key);
				tabListView.Parent = assemblyPage;
				tabListView.Dock = DockStyle.Fill;
				tabListView.ItemChecked += listView1_ItemChecked;

				tabListView.Columns.Add(new ColumnHeader() { DisplayIndex = 0, Text = "Name" });
				tabListView.Columns.Add(new ColumnHeader() { DisplayIndex = 1, Text = "Type" });
				tabListView.Columns.Add(new ColumnHeader() { DisplayIndex = 2, Text = "Changed" });

				tabListView.View = View.Details;
				tabListView.CheckBoxes = true;
				tabListView.ColumnClick += new ColumnClickEventHandler(listView1_SortChanged);
				tabListView.ListViewItemSorter = lvwColumnSorter;

				assemblyListViews.Add(assembly.Key, tabListView);
				assemblyItemTabs.TabPages.Add(assemblyPage);

				// fill list view with items to export
				object iniFile = assemblyIniFiles[assembly.Key];

				if (iniFile is SplitTools.IniData)
				{
					FillListViewIniData(tabListView, assembly.Value,
						assembly.Key, (SplitTools.IniData)iniFile, assemblyItemsToExport[assembly.Key]);

				}
				else if (iniFile is DllIniData)
				{
					FillListViewDLLIniData(tabListView, assembly.Value,
						assembly.Key, (DllIniData)iniFile, assemblyItemsToExport[assembly.Key]);
				}

				for (int i = 0; i < tabListView.Columns.Count; i++)
				{
					if (i != tabListView.Columns.Count - 1) // Don't auto resize the "Changed" column
						tabListView.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
					else
						tabListView.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.HeaderSize);
				}
			}
		}

		private void BackupList(ListView listView)
		{
			backupList.Items.Clear();

			backupList.Items.AddRange((from ListViewItem item in listView.Items
									   select (ListViewItem)item.Clone()
				   ).ToArray());
		}

		private void FillListViewIniData(ListView listView, AssemblyType assemblyType, string assemblyname,
			SplitTools.IniData iniData, Dictionary<string, bool> itemsToExport)
		{
			listView.BeginUpdate();
			listView.Items.Clear();

			foreach (KeyValuePair<string, SplitTools.FileInfo> item in iniData.Files)
			{
				KeyValuePair<string, bool> exportStatus = itemsToExport.First(export => export.Key == item.Key);

				if (!StructConverter.StructConverter.DataTypeList.ContainsKey(item.Value.Type))
				{
					continue;
				}

				bool modified = exportStatus.Value;

				listView.Items.Add(new ListViewItem
				(
				new[]
				{
				item.Key, StructConverter.StructConverter.DataTypeList[item.Value.Type],
				(modified ? "Yes" : "No")
				})
				{ Checked = modified });
			}

			BackupList(listView);
			listView.EndUpdate();
		}

		private void FillListViewDLLIniData(ListView listView, AssemblyType assemblyType, string assemblyName,
			DllIniData iniData, Dictionary<string, bool> itemsToExport)
		{
			listView.BeginUpdate();
			listView.Items.Clear();

			bool oneModified = false;

			foreach (KeyValuePair<string, FileTypeHash> item in iniData.Files)
			{
				bool modified = itemsToExport[item.Key];

				if (!oneModified)
					oneModified = modified;

				//unused for now as replacing path with name breaks export if there is no edit on the file.
				
				/*string name = Path.GetFileNameWithoutExtension(item.Key);

				foreach (var samdlItem in iniData.SAMDLData)
				{
					if (Path.GetFileNameWithoutExtension(samdlItem.Key) == name)
					{
						name = samdlItem.Value.ModelName;
					}
				}*/

				listView.Items.Add(new ListViewItem
				(
				new[]
				{
					item.Key,
					StructConverter.StructConverter.DataTypeList[item.Value.Type],
					(modified ? "Yes" : "No"),
				})
				{ Checked = modified });
			}

			foreach (var item in iniData.DataItems)
			{
				bool modified = itemsToExport[item.Filename];

				listView.Items.Add(new ListViewItem
				(
				new[]
				{
				item.Filename, StructConverter.StructConverter.DataTypeList[item.Type],
				(modified ? "Yes" : "No")
				})
				{ Checked = modified });
			}

			//if at least one file got edited, sort by "descending" to show them at the beginning of the list.
			if (oneModified)
			{
				for (int i = 0; i < listView.Columns.Count; i++)
				{
					lvwColumnSorter.SortColumn = listView.Columns[i].Index;
					lvwColumnSorter.Order = SortOrder.Descending;
				}

				listView.Sort();
			}

			BackupList(listView);
			listView.EndUpdate();
		}

		private void listView1_SortChanged(object o, ColumnClickEventArgs e)
		{
			// Determine if clicked column is already the column that is being sorted.
			if (e.Column == lvwColumnSorter.SortColumn)
			{
				// Reverse the current sort direction for this column.
				lvwColumnSorter.Order = (lvwColumnSorter.Order == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending;
			}
			else
			{
				// Set the column number that is to be sorted; default to descending.
				lvwColumnSorter.SortColumn = e.Column;
				lvwColumnSorter.Order = SortOrder.Descending;
			}

			// Perform the sort with these new sort options.
			TabPage page = assemblyItemTabs.SelectedTab;
			ListView listView = assemblyListViews[page.Text];
			listView.Sort();
		}

		private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			// first get reference to our list box
			// then reference to assembly
			// then reference to export-list
			string name = assemblyItemTabs.SelectedTab.Text;

			assemblyItemsToExport[name][e.Item.Text] = e.Item.Checked;
		}

		private void CheckAllButton_Click(object sender, EventArgs e)
		{
			TabPage page = assemblyItemTabs.SelectedTab;
			ListView listView = assemblyListViews[page.Text];

			listView.BeginUpdate();
			foreach (ListViewItem item in listView.Items)
			{
				item.Checked = true;
			}
			listView.EndUpdate();
		}

		private void CheckModifiedButton_Click(object sender, EventArgs e)
		{
			TabPage page = assemblyItemTabs.SelectedTab;
			ListView listView = assemblyListViews[page.Text];

			int modifiedIndex = (assemblies[page.Text] == AssemblyType.Exe) ? 2 : 1;

			listView.BeginUpdate();
			foreach (ListViewItem item in listView.Items)
			{
				item.Checked = item.SubItems[modifiedIndex].Text != "No"; // we need to handle this differently depending on if we're exe or dll
			}
			listView.EndUpdate();
		}

		private void UncheckAllButton_Click(object sender, EventArgs e)
		{
			TabPage page = assemblyItemTabs.SelectedTab;
			ListView listView = assemblyListViews[page.Text];

			listView.BeginUpdate();
			foreach (ListViewItem item in listView.Items)
			{
				item.Checked = false;
			}
			listView.EndUpdate();
		}

		private void CPPExportButton_Click(object sender, EventArgs e)
		{
			var folderDialog = new FolderBrowserDialog();
			if (folderDialog.ShowDialog() == DialogResult.OK)
			{
				string outputFolder = folderDialog.SelectedPath;

				foreach (KeyValuePair<string, AssemblyType> assembly in assemblies)
				{
					switch (assembly.Value)
					{
						case AssemblyType.Exe:
							StructConverter.StructConverter.ExportCPP((SplitTools.IniData)assemblyIniFiles[assembly.Key],
								assemblyItemsToExport[assembly.Key], Path.Combine(outputFolder, assembly.Key + ".cpp"));
							break;

						case AssemblyType.DLL:
							DLLModGenerator.DLLModGen.ExportCPP((DllIniData)assemblyIniFiles[assembly.Key],
								assemblyItemsToExport[assembly.Key], Path.Combine(outputFolder, assembly.Key + ".cpp"));
							break;

						default:
							break;
					}
				}

				MessageBox.Show(string.Format("Source code files exported to {0}", outputFolder), "Success", MessageBoxButtons.OK);
			}
			else
				MessageBox.Show("No folder was provided.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

		}

		private void IniExportButton_Click(object sender, EventArgs e)
		{
			searchBox.Clear();
			var folderDialog = new FolderBrowserDialog();
			if (folderDialog.ShowDialog() == DialogResult.OK)
			{
				string outputFolder = folderDialog.SelectedPath;
				List<string> listIni_exe = new List<string>();
				Dictionary<string, bool> itemsEXEToExport = new Dictionary<string, bool>();
				foreach (KeyValuePair<string, AssemblyType> assembly in assemblies)
				{
					string iniPath = Path.Combine(projectFolder, assembly.Key + "_data.ini");
					switch (assembly.Value)
					{
						case AssemblyType.Exe:
							listIni_exe.Add(iniPath);
							foreach (var item in assemblyItemsToExport[assembly.Key])
								if (item.Value)
									itemsEXEToExport.Add(item.Key, item.Value);
							break;

						case AssemblyType.DLL:
							DLLModGenerator.DLLModGen.ExportINI((DllIniData)assemblyIniFiles[assembly.Key],
								assemblyItemsToExport[assembly.Key], Path.Combine(folderDialog.SelectedPath, assembly.Key + "_data.ini"));
							break;

						default:
							break;
					}
				}
				if (listIni_exe.Count > 0)
				{
					SplitTools.IniData EXEiniData = SAModel.SAEditorCommon.StructConverter.StructConverter.LoadMultiINI(listIni_exe, ref itemsEXEToExport, true);
					SAModel.SAEditorCommon.StructConverter.StructConverter.ExportINI(EXEiniData,
						itemsEXEToExport, game, Path.Combine(outputFolder, gameEXE + "_data.ini"));
				}
				MessageBox.Show(string.Format("INI Files have been exported to {0}", outputFolder), "Success", MessageBoxButtons.OK);
			}
			else
				MessageBox.Show("No folder was provided.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private void ManualBuildWindow_Shown(object sender, EventArgs e)
		{
			CPPExportButton.Enabled = false;
			IniExportButton.Enabled = false;
			CheckAllButton.Enabled = false;
			CheckModifiedButton.Enabled = false;
			UncheckAllButton.Enabled = false;

			LoadingLabel.Visible = false;

			LoadIniFiles();

			SetTabsForAssemblies();

			LoadingLabel.Visible = false;
			CPPExportButton.Enabled = true;
			IniExportButton.Enabled = true;
			CheckAllButton.Enabled = true;
			CheckModifiedButton.Enabled = true;
			UncheckAllButton.Enabled = true;
		}

		private void searchBox_TextChanged(object sender, EventArgs e)
		{
			TabPage page = assemblyItemTabs.SelectedTab;

			if (page is null)
				return;

			ListView listView = assemblyListViews[page.Text];

			listView.BeginUpdate();
			listView.Items.Clear();

			foreach (ListViewItem item in backupList.Items)
			{
				if (searchBox.Text != "" && !item.Text.ToLowerInvariant().Contains(searchBox.Text.ToLowerInvariant()))
					continue;

				listView.Items.Add(((ListViewItem)item.Clone()));
			}

			listView.EndUpdate();
		}
	}
}
