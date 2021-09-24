using SplitTools;
using SplitTools.SplitDLL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using static SAEditorCommon.ProjectManagement.Templates;
using IniDictionary = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string>>;

namespace SAModel.SAMDL
{
    public partial class ModelSelectDialog : Form
    {
        public string ModelFilename;
        public string[] TextureArchiveNames;
        public TexnameArray TextureNames;
        public int[] TextureIDs;
        public int CategoryIndex = -1;
        List<SplitEntry> Categories = new List<SplitEntry>();
        Dictionary<string, SplitTools.FileInfo> Models = new Dictionary<string, SplitTools.FileInfo>();
        public string modFolder;
        public string modSystemFolder;
        public string gameSystemFolder;

        public bool CheckIfIniFileHasModels(SplitEntry split)
        {
            // Returns true if the split INI file for the entry has models
            string inipath = Path.Combine(modFolder, split.IniFile + "_data.ini");
            if (!File.Exists(inipath))
                return false;
            // Check source file extension to determine what kind of split INI data is used with it
            switch (Path.GetExtension(split.SourceFile).ToLowerInvariant())
            {
                case ".nb":
                    Dictionary<int, string> nbFilenames = IniSerializer.Deserialize<Dictionary<int, string>>(inipath);
                    foreach (var nbitem in nbFilenames)
                    {
                        string entryFilename = nbitem.Value;
                        if (nbitem.Value.Contains("|"))
                        {
                            string[] nbMeta = nbitem.Value.Split('|');
                            entryFilename = nbMeta[0];
                        }
                        switch (Path.GetExtension(entryFilename).ToLowerInvariant())
                        {
                            case ".sa1mdl":
                            case ".sa2mdl":
                            case ".sa2bmdl":
                                return true;
                            default:
                                break;
                        }
                    }
                    break;
                default:
                    IniDictionary iniFile = SplitTools.IniFile.Load(inipath);
                    foreach (var key in iniFile)
                    {
                        // If this section exists in the file, it's a DLL split
                        if (key.Key == "SAMDLData")
                            if (key.Value.Count > 0)
                                return true;
                        // Regular binary split
                        if (key.Value.ContainsKey("type"))
                            switch (key.Value["type"])
                            {
                                case "model":
                                case "basicmodel":
                                case "basicdxmodel":
                                case "chunkmodel":
                                case "gcmodel":
                                    return true;
                                default:
                                    break;
                            }
                    }
                    break;
            }
            return false;
        }

        public ModelSelectDialog(ProjectTemplate projFile, int index)
        {
            modFolder = projFile.GameInfo.ProjectFolder;
            modSystemFolder = Path.Combine(modFolder, projFile.GameInfo.GameDataFolder);
            gameSystemFolder = Path.Combine(projFile.GameInfo.GameFolder, projFile.GameInfo.GameDataFolder); // To get a path like "SADX\system" or "SA1\SONICADV"
            InitializeComponent();

            // Find valid INI files
            foreach (SplitEntry split in projFile.SplitEntries)
            {
                if (CheckIfIniFileHasModels(split))
                {
                    Categories.Add(split);
                    string categoryName = split.IniFile;
                    // To prevent a crash when category names aren't defined
                    if (split.CmnName != null && split.CmnName != "")
                        categoryName = split.CmnName;
                    comboCategories.Items.Add(categoryName);
                }
            }
            if (comboCategories.Items.Count - 1 >= index)
                comboCategories.SelectedIndex = index;
            else if (comboCategories.Items.Count > 0)
                comboCategories.SelectedIndex = 0;
            else 
                comboCategories.SelectedIndex = -1;
        }

        private void comboCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboCategories.SelectedIndex == CategoryIndex)
                return;
            Models.Clear();
            listModels.Items.Clear();
            CategoryIndex = comboCategories.SelectedIndex;
            string inipath = Path.Combine(modFolder, Categories[CategoryIndex].IniFile + "_data.ini");
            // Get models from inidata by type
            switch (Path.GetExtension(Categories[CategoryIndex].SourceFile).ToLowerInvariant())
            {
                case ".dll":
                    IniDictionary iniFile = SplitTools.IniFile.Load(inipath);
                    foreach (var dllItem in iniFile["SAMDLData"])
					{
                        //MessageBox.Show(counter.ToString() + dllItem.Key + dllItem.Value);
                        string[] nameAndTexture = dllItem.Value.Split('|');
                        SplitTools.FileInfo fakeFileInfo = new SplitTools.FileInfo();
                        fakeFileInfo.CustomProperties = new Dictionary<string, string>();
                        fakeFileInfo.Filename = dllItem.Key;
                        //MessageBox.Show(nameAndTexture[0]);
                        if (nameAndTexture.Length > 1)
                            fakeFileInfo.CustomProperties["texture"] = nameAndTexture[1];
                        if (nameAndTexture.Length > 2)
                            fakeFileInfo.CustomProperties["texids"] = nameAndTexture[2];
                        Models.Add(nameAndTexture[0], fakeFileInfo);
                    }
                    break;
                case ".nb":
                    Dictionary<int, string> nbFilenames = IniSerializer.Deserialize<Dictionary<int, string>>(inipath);
                    foreach (var nbitem in nbFilenames)
                    {
                        string entryFilename = nbitem.Value;
                        string entryDescription = "";
                        string entryTexture = "";
                        if (nbitem.Value.Contains("|"))
                        {
                            string[] nbMeta = nbitem.Value.Split('|');
                            entryFilename = nbMeta[0];
                            if (nbMeta.Length > 1)
                                entryDescription = nbMeta[1];
                            if (nbMeta.Length > 2)
                                entryTexture = nbMeta[2];
                        }
                        else
                            entryDescription = Path.GetFileNameWithoutExtension(nbitem.Value);
                        switch (Path.GetExtension(entryFilename).ToLowerInvariant())
                        {
                            case ".sa1mdl":
                            case ".sa2mdl":
                            case ".sa2bmdl":
                                SplitTools.FileInfo fakeFileInfo = new SplitTools.FileInfo();
                                fakeFileInfo.CustomProperties = new Dictionary<string, string>();
                                fakeFileInfo.Filename = entryFilename;
                                if (entryTexture != "")
                                    fakeFileInfo.CustomProperties["texture"] = entryTexture;
                                Models.Add(entryDescription, fakeFileInfo);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                default:
                    IniData inidata = IniSerializer.Deserialize<IniData>(inipath);
                    foreach (var item in inidata.Files)
                    {
                        switch (item.Value.Type)
                        {
                            case "model":
                            case "basicmodel":
                            case "basicdxmodel":
                            case "chunkmodel":
                            case "gcmodel":
                                Models.Add(item.Key, item.Value);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
            }
            // Fill in models
            foreach (var item in Models)
            {
                listModels.Items.Add(item.Key);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            ModelFilename = "";
            TextureArchiveNames = null;
            Close();
        }

        // Get the exact filename for a PVM/GVM/PRS/whatever (i.e. OBJ_REGULAR will return the full path to OBJ_REGULAR.PVM or OBJ_REGULAR.GVM or OBJ_REGULAR.PRS etc.)
        private string GetTextureFilename(string pvmName)
        {
            string extension = ".PVM";
            // Determine whether a custom texture pack or a PVMX exists
            if (Directory.Exists(Path.Combine(modFolder, "textures", pvmName)))
                return Path.Combine(modFolder, "textures", pvmName, "index.txt");
            else if (File.Exists(Path.Combine(modFolder, "textures", pvmName + ".PVMX")))
                return Path.Combine(modFolder, "textures", pvmName + ".PVMX");
            else
            {
                bool modHasTexture = false;
                // Check if PVM/GVM/PRS/PAK exists in the mod's system folder
                if (File.Exists(Path.Combine(modSystemFolder, pvmName) + ".PVM"))
                {
                    extension = ".PVM";
                    modHasTexture = true;
                }
                else if (File.Exists(Path.Combine(modSystemFolder, pvmName) + ".PRS"))
                {
                    extension = ".PRS";
                    modHasTexture = true;
                }
                else if (File.Exists(Path.Combine(modSystemFolder, pvmName) + ".GVM"))
                {
                    extension = ".GVM";
                    modHasTexture = true;
                }
                else if (File.Exists(Path.Combine(modSystemFolder, pvmName) + ".PAK"))
                {
                    extension = ".PAK";
                    modHasTexture = true;
                }
                else if (File.Exists(Path.Combine(modSystemFolder, pvmName) + ".PB"))
                {
                    extension = ".PB";
                    modHasTexture = true;
                }
                else if (File.Exists(Path.Combine(modSystemFolder, pvmName) + ".PVR"))
                {
                    extension = ".PVR";
                    modHasTexture = true;
                }
                // Fallback on the game's system folder
                if (!modHasTexture)
                {
                    if (File.Exists(Path.Combine(gameSystemFolder, pvmName) + ".PVM"))
                        extension = ".PVM";
                    else if (File.Exists(Path.Combine(gameSystemFolder, pvmName) + ".PRS"))
                        extension = ".PRS";
                    else if (File.Exists(Path.Combine(gameSystemFolder, pvmName) + ".GVM"))
                        extension = ".GVM";
                    else if (File.Exists(Path.Combine(gameSystemFolder, pvmName) + ".PAK"))
                        extension = ".PAK";
                    else if (File.Exists(Path.Combine(gameSystemFolder, pvmName) + ".PB"))
                        extension = ".PB";
                    else if (File.Exists(Path.Combine(gameSystemFolder, pvmName) + ".PVR"))
                        extension = ".PVR";
                }
                return Path.Combine(modHasTexture ? modSystemFolder : gameSystemFolder, pvmName) + extension;
            }
        }

        private void LoadModelIndex(int index)
        {
            TextureArchiveNames = null;
            if (index == -1)
                return;
            List<string> textureNamesList = new List<string>();
            string modelName = listModels.Items[listModels.SelectedIndex].ToString();
            // Loop through the model list to find the selected model
            foreach (var model in Models)
            {
                if (model.Key == modelName)
                {
                    // Set model filename
                    ModelFilename = Path.Combine(modFolder, model.Value.Filename);
                    // Set textures...
                    // PVM name(s) specified directly
                    if (model.Value.CustomProperties.ContainsKey("texture"))
                    {
                        // Check if there are multiple entries
                        if (model.Value.CustomProperties["texture"].Contains(","))
                        {
                            string[] pvms = model.Value.CustomProperties["texture"].Split(',');
                            for (int p = 0; p < pvms.Length; p++)
                                textureNamesList.Add(GetTextureFilename(pvms[p]));
                        }
                        // If not, use the whole field as PVM name + optional texture IDs
                        else
                        {
                            string pvmName = model.Value.CustomProperties["texture"];
                            textureNamesList.Add(GetTextureFilename(pvmName));
                            // Check for specific texture IDs
                            if (model.Value.CustomProperties.ContainsKey("texids"))
                            {
                                // Multiple textures
                                if (model.Value.CustomProperties["texids"].Contains(","))
                                {
                                    string[] textureIDs_string = model.Value.CustomProperties["texids"].Split(',');
                                    List<int> texids = new List<int>();
                                    for (int i = 0; i < textureIDs_string.Length; i++)
                                        texids.Add(int.Parse(textureIDs_string[i], System.Globalization.NumberStyles.Integer));
                                    TextureIDs = texids.ToArray();
                                }
                                // Single texture
                                else
                                {
                                    TextureIDs = new int[1];
                                    TextureIDs[0] = int.Parse(model.Value.CustomProperties["texids"], System.Globalization.NumberStyles.Integer);
                                }
                            }
                            else
                                TextureIDs = null;
                        }
                    }
                    // PVM names specified in texlist INI file
                    else if (model.Value.CustomProperties.ContainsKey("texlist"))
                    {
                        // Get PVM/GVM names from texlist INI
                        string tspath = Path.Combine(modFolder, model.Value.CustomProperties["texlist"]);
                        TextureListEntry[] ts = TextureList.Load(tspath);
                        for (int t = 0; t < ts.Length; t++)
                        {
                            if (ts[t].Name != null && ts[t].Name != "")
                                textureNamesList.Add(GetTextureFilename(ts[t].Name));
                        }
                    }
                    // A list of texture names is specified for partial texlist
                    if (model.Value.CustomProperties.ContainsKey("texnames"))
                    {
                        string tapath = Path.Combine(modFolder, model.Value.CustomProperties["texnames"]);
                        TextureNames = new TexnameArray(tapath);
                    }
                    else
                        TextureNames = null;
                    // Set the list of texture archive names
                    TextureArchiveNames = textureNamesList.ToArray();
                    // Finish the loop if a match is found
                    break;
                }
            }
        }
        private void buttonOK_Click(object sender, EventArgs e)
        {
            LoadModelIndex(listModels.SelectedIndex);
        }

        private void listModels_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonOK.Enabled = listModels.SelectedIndex != -1;
        }

        private void listModels_DoubleClick(object sender, EventArgs e)
        {
            if (listModels.SelectedIndex != -1)
            {
                LoadModelIndex(listModels.SelectedIndex);
            }
            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}