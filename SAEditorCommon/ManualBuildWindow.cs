using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using SA_Tools;

namespace SonicRetro.SAModel.SAEditorCommon
{
	public partial class ManualBuildWindow : Form
	{
		public enum AssemblyType { Exe, DLL }

		private string projectFolder = "";
		private string modFolder = "";

		private Dictionary<string, AssemblyType> assemblies = new Dictionary<string, AssemblyType>();
		private Dictionary<string, ListView> assemblyListViews = new Dictionary<string, ListView>();
		private Dictionary<string, object> assemblyIniFiles = new Dictionary<string, object>(); // gotta box ini data to object so that dll ini and ini data can exist in the same list

		private Dictionary<string, Dictionary<string, bool>> assemblyItemsToExport = new Dictionary<string, Dictionary<string, bool>>();

		public ManualBuildWindow()
		{
			InitializeComponent();
		}

		public void Initalize(Game game, string projectName, string projectFolder,
			string modFolder, Dictionary<string, AssemblyType> assemblies)
		{
			this.projectFolder = projectFolder;
			this.modFolder = modFolder;
			this.assemblies = assemblies;

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
						SA_Tools.IniData iniData = StructConverter.StructConverter.LoadINI(iniPath, ref itemsToExport);
						assemblyItemsToExport.Add(assembly.Key, itemsToExport);
						assemblyIniFiles.Add(assembly.Key, iniData);
						break;

					case AssemblyType.DLL:
						DLLModGenerator.DllIniData dllIniData = DLLModGenerator.DLLModGen.LoadINI(iniPath, ref itemsToExport);
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
			assemblyItemTabs.TabPages.Clear();

			foreach (KeyValuePair<string, ListView> listView in assemblyListViews)
			{
				listView.Value.ItemChecked -= listView1_ItemChecked;
				listView.Value.Dispose();
			}

			assemblyListViews.Clear();

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

				if (assembly.Value == AssemblyType.Exe)
				{
					tabListView.Columns.Add(new ColumnHeader() { DisplayIndex = 1, Text = "Type" });
					tabListView.Columns.Add(new ColumnHeader() { DisplayIndex = 2, Text = "Changed" });
				}
				else
				{
					tabListView.Columns.Add(new ColumnHeader() { DisplayIndex = 1, Text = "Changed" });
				}

				tabListView.View = View.Details;
				tabListView.CheckBoxes = true;

				assemblyListViews.Add(assembly.Key, tabListView);
				assemblyItemTabs.TabPages.Add(assemblyPage);

				// fill list view with items to export
				object iniFile = assemblyIniFiles[assembly.Key];

				if (iniFile is SA_Tools.IniData)
				{
					FillListViewIniData(tabListView, assembly.Value,
						assembly.Key, (SA_Tools.IniData)iniFile, assemblyItemsToExport[assembly.Key]);

				}
				else if (iniFile is DLLModGenerator.DllIniData)
				{
					FillListViewDLLIniData(tabListView, assembly.Value,
						assembly.Key, (DLLModGenerator.DllIniData)iniFile, assemblyItemsToExport[assembly.Key]);
				}

				for (int i = 0; i < tabListView.Columns.Count; i++)
				{
					tabListView.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
				}
			}
		}

		private void FillListViewIniData(ListView listView, AssemblyType assemblyType, string assemblyname,
			SA_Tools.IniData iniData, Dictionary<string, bool> itemsToExport)
		{
			listView.BeginUpdate();
			listView.Items.Clear();

			foreach (KeyValuePair<string, SA_Tools.FileInfo> item in iniData.Files)
			{
				KeyValuePair<string, bool> exportStatus = itemsToExport.First(export => export.Key == item.Key);

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

			listView.EndUpdate();
		}

		private void FillListViewDLLIniData(ListView listView, AssemblyType assemblyType, string assemblyName,
			DLLModGenerator.DllIniData iniData, Dictionary<string, bool> itemsToExport)
		{
			listView.BeginUpdate();
			listView.Items.Clear();

			foreach (KeyValuePair<string, DLLModGenerator.FileTypeHash> item in iniData.Files)
			{
				KeyValuePair<string, bool> exportStatus = itemsToExport.First(export => export.Key == item.Key);

				bool modified = exportStatus.Value;

				listView.Items.Add(new ListViewItem(new[] { item.Key, modified ? "Yes" : "No" }) { Checked = modified }); ;
			}

			listView.EndUpdate();
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

		private void UncheckAllButton_Click(object sender, EventArgs e)
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

		private void CheckModifiedButton_Click(object sender, EventArgs e)
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
			string outputFolder = Path.Combine(projectFolder, "source");

			Directory.CreateDirectory(outputFolder);

			// use a folder dialog here instead of a file dialog
			// todo: make this asyncronous
			foreach (KeyValuePair<string, AssemblyType> assembly in assemblies)
			{
				switch (assembly.Value)
				{
					case AssemblyType.Exe:
						StructConverter.StructConverter.ExportCPP((SA_Tools.IniData)assemblyIniFiles[assembly.Key],
							assemblyItemsToExport[assembly.Key], Path.Combine(outputFolder, assembly.Key + ".cpp"));
						break;

					case AssemblyType.DLL:
						DLLModGenerator.DLLModGen.ExportCPP((DLLModGenerator.DllIniData)assemblyIniFiles[assembly.Key],
							assemblyItemsToExport[assembly.Key], Path.Combine(outputFolder, assembly.Key + ".cpp"));
						break;

					default:
						break;
				}
			}

			MessageBox.Show(string.Format("Source code files exported to {0}", outputFolder));
		}

		private void IniExportButton_Click(object sender, EventArgs e)
		{
			using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
			{
				folderDialog.SelectedPath = modFolder;

				if (folderDialog.ShowDialog(this) == DialogResult.OK)
				{
					// use a folder dialog here instead of a file dialog
					foreach (KeyValuePair<string, AssemblyType> assembly in assemblies)
					{
						switch (assembly.Value)
						{
							case AssemblyType.Exe:
								StructConverter.StructConverter.ExportINI((SA_Tools.IniData)assemblyIniFiles[assembly.Key],
									assemblyItemsToExport[assembly.Key], Path.Combine(folderDialog.SelectedPath, assembly.Key + ".ini"));
								break;

							case AssemblyType.DLL:
								DLLModGenerator.DLLModGen.ExportINI((DLLModGenerator.DllIniData)assemblyIniFiles[assembly.Key],
									assemblyItemsToExport[assembly.Key], Path.Combine(folderDialog.SelectedPath, assembly.Key + ".ini"));
								break;

							default:
								break;
						}
					}
				}
			}

			MessageBox.Show("Export complete!");
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
	}
}
