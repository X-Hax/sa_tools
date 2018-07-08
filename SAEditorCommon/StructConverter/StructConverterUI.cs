using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SA_Tools;
using SonicRetro.SAModel;

namespace SonicRetro.SAModel.SAEditorCommon.StructConverter
{
    public partial class StructConverterUI : Form
    {
        public readonly Dictionary<string, string> DataTypeList = new Dictionary<string, string>()
        {
            { "landtable", "LandTable" },
            { "model", "Model" },
            { "basicmodel", "Basic Model" },
            { "basicdxmodel", "Basic Model (SADX)" },
            { "chunkmodel", "Chunk Model" },
            { "action", "Action (animation+model)" },
            { "animation", "Animation" },
            { "objlist", "Object List" },
            { "startpos", "Start Positions" },
            { "texlist", "Texture List" },
            { "leveltexlist", "Level Texture List" },
            { "triallevellist", "Trial Level List" },
            { "bosslevellist", "Boss Level List" },
            { "fieldstartpos", "Field Start Positions" },
            { "soundtestlist", "Sound Test List" },
            { "musiclist", "Music List" },
            { "soundlist", "Sound List" },
            { "stringarray", "String Array" },
            { "nextlevellist", "Next Level List" },
            { "cutscenetext", "Cutscene Text" },
            { "recapscreen", "Recap Screen" },
            { "npctext", "NPC Text" },
            { "levelclearflags", "Level Clear Flags" },
            { "deathzone", "Death Zones" },
            { "skyboxscale", "Skybox Scale" },
            { "stageselectlist", "Stage Select List" },
            { "levelrankscores", "Level Rank Scores" },
            { "levelranktimes", "Level Rank Times" },
            { "endpos", "End Positions" },
            { "animationlist", "Animation List" },
			{ "levelpathlist", "Path List" },
			{ "stagelightdatalist", "Stage Light Data List" },
			{ "weldlist", "Weld List" },
			{ "bmitemattrlist", "BM Item Attributes List" },
			{ "creditstextlist", "Credits Text List" }
        };

        private string projectFolder = "";
        private string modFolder = "";
        private string fileName = "";

        public StructConverterUI()
        {
            InitializeComponent();
        }

        SA_Tools.IniData iniData;
        Dictionary<string, bool> itemsToExport = new Dictionary<string, bool>();

        public void SetProjectFolder(string projectFolder)
        {
            this.projectFolder = projectFolder;
        }

        public void SetModFolder(string modFolder)
        {
            this.modFolder = modFolder;
        }

        public void OpenFile(string fileName)
        {
            this.fileName = fileName;

            LoadINIFile(Path.Combine(projectFolder, fileName));
            ShowDialog();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog()
            {
                DefaultExt = "ini",
                Filter = "INI Files|*.ini|All Files|*.*"
            })
            {
                fileDialog.InitialDirectory = projectFolder;
                fileDialog.FileName = fileName + "_data.ini";
                
                if (fileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    LoadINIFile(fileDialog.FileName);
                }
            }
        }

        private void LoadINIFile(string fileName)
        {
            iniData = StructConverter.LoadINI(fileName, ref itemsToExport);
            UpdateListView();
        }

        private void UpdateListView()
        {
            listView1.BeginUpdate();
            listView1.Items.Clear();

            foreach (KeyValuePair<string, SA_Tools.FileInfo> item in iniData.Files)
            {
                KeyValuePair<string, bool> exportStatus = itemsToExport.First(export => export.Key == item.Key);

                bool modified = exportStatus.Value;

                listView1.Items.Add(new ListViewItem
                (
                new[]
                {
                    item.Key, DataTypeList[item.Value.Type],
                    (modified ? "Yes" : "No")
                })
                { Checked = modified });
            }

            listView1.EndUpdate();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void CheckAllButton_Click(object sender, EventArgs e)
        {
            listView1.BeginUpdate();
            foreach (ListViewItem item in listView1.Items)
                item.Checked = true;
            listView1.EndUpdate();
        }

        private void CheckModifiedButton_Click(object sender, EventArgs e)
        {
            listView1.BeginUpdate();
            foreach (ListViewItem item in listView1.Items)
                item.Checked = item.SubItems[2].Text != "No";
            listView1.EndUpdate();
        }

        private void UncheckAllButton_Click(object sender, EventArgs e)
        {
            listView1.BeginUpdate();
            foreach (ListViewItem item in listView1.Items)
                item.Checked = false;
            listView1.EndUpdate();
        }

		private void ExportCPPButton_Click(object sender, EventArgs e)
		{
            using (SaveFileDialog fileDialog = new SaveFileDialog() { DefaultExt = "cpp", Filter = "C++ source files|*.cpp", InitialDirectory = Environment.CurrentDirectory, RestoreDirectory = true })
            {
                fileDialog.InitialDirectory = modFolder;
                fileDialog.FileName = Path.GetFileNameWithoutExtension(fileName);

                if (fileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    StructConverter.ExportCPP(iniData, itemsToExport, fileDialog.FileName);
                }
            }
		}

		private void ExportINIButton_Click(object sender, EventArgs e)
		{
            bool autoSaveSuccess = false;
            string autoSaveName = "";
            bool preferAutoSave = true; // make this configurable later.

            if (preferAutoSave)
            {
                if (modFolder != null && modFolder.Length > 0)
                {
                    // try doing auto-export
                    Directory.CreateDirectory(modFolder);

                    if (Directory.Exists(modFolder))
                    {
                        autoSaveSuccess = true;
                        autoSaveName = Path.Combine(modFolder, fileName);
                    }
                }
            }

            if (autoSaveSuccess)
            {
                StructConverter.ExportINI(iniData, itemsToExport, autoSaveName);
                MessageBox.Show("Export Complete!");
                Hide();
            }
            else
            {
                using (SaveFileDialog fileDialog = new SaveFileDialog() { DefaultExt = "ini", Filter = "INI files|*.ini", InitialDirectory = modFolder, RestoreDirectory = true })
                {
                    fileDialog.InitialDirectory = modFolder;
                    fileDialog.FileName = fileName;

                    if (fileDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        StructConverter.ExportINI(iniData, itemsToExport, fileDialog.FileName);
                    }
                }
            }
        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            itemsToExport[e.Item.Text] = e.Item.Checked;
            //Console.WriteLine("Items to export: " + e.Item.Name + " " + ((itemsToExport[e.Item.Text]) ? "checked" : "not checked"));
        }
    }

    static class Extensions
    {
        public static string MakeIdentifier(this string name)
        {
            StringBuilder result = new StringBuilder();
            foreach (char item in name)
                if ((item >= '0' & item <= '9') | (item >= 'A' & item <= 'Z') | (item >= 'a' & item <= 'z') | item == '_')
                    result.Append(item);
            if (result[0] >= '0' & result[0] <= '9')
                result.Insert(0, '_');
            return result.ToString();
        }
    }
}