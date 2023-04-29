using SAModel.SAEditorCommon.ProjectManagement;
using SplitTools;
using SplitTools.SplitDLL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using static SAModel.SAEditorCommon.ProjectManagement.Templates;
using IniDictionary = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string>>;

namespace SAModel.SAMDL
{
	public partial class ModelSelectDialog : Form
	{
		public ModelLoadInfo ModelInfo;
		public string SelectedCategory = "";
		Dictionary<string, List<SplitEntry>> Categories = new Dictionary<string, List<SplitEntry>>();
		Dictionary<string, List<SplitEntryMDL>> MDLCategories = new Dictionary<string, List<SplitEntryMDL>>();
		List<ModelLoadInfo> Models = new List<ModelLoadInfo>();
		public string modFolder;
		public string modSystemFolder;
		public string gameSystemFolder;

		private bool CheckIfIniFileHasModels(SplitEntry split)
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

		private bool CheckLabelFile(SplitEntryMDL mdl)
		{
			// Returns true if the split INI file for the entry has models
			string inipath = Path.Combine(Path.Combine(modFolder, "figure\\bin"), mdl.LabelFile + "_data.ini");
			if (!File.Exists(inipath))
				return false;
			// Check source file extension to determine what kind of split INI data is used with it
			Dictionary<int, string> mdlFilenames = IniSerializer.Deserialize<Dictionary<int, string>>(inipath);
			foreach (var mdlitem in mdlFilenames)
			{
				string entryFilename = mdlitem.Value;
				if (mdlitem.Value.Contains("|"))
				{
					string[] mdlMeta = mdlitem.Value.Split('|');
					entryFilename = mdlMeta[0];
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
			return false;
		}

		private void AddSplitEntryToCategories(SplitEntry entry, string categoryName)
		{
			if (Categories.ContainsKey(categoryName))
				Categories[categoryName].Add(entry);
			else
			{
				List<SplitEntry> splitlist = new List<SplitEntry>();
				splitlist.Add(entry);
				Categories.Add(categoryName, splitlist);
				comboCategories.Items.Add(categoryName);
			}
		}
		private void AddMDLEntryToCategories(SplitEntryMDL entry, string categoryName)
		{
			if (MDLCategories.ContainsKey(categoryName))
				MDLCategories[categoryName].Add(entry);
			else
			{
				List<SplitEntryMDL> mdllist = new List<SplitEntryMDL>();
				mdllist.Add(entry);
				MDLCategories.Add(categoryName, mdllist);
				comboCategories.Items.Add(categoryName);
			}
		}

		public ModelSelectDialog(ProjectTemplate projFile, string lastCategory)
		{
			modFolder = projFile.GameInfo.ProjectFolder;
			modSystemFolder = Path.Combine(modFolder, projFile.GameInfo.GameDataFolder);
			gameSystemFolder = Path.Combine(ProjectFunctions.GetGamePath(projFile.GameInfo.GameName), projFile.GameInfo.GameDataFolder); // To get a path like "SADX\system" or "SA1\SONICADV"
			InitializeComponent();

			// Find valid INI files
			foreach (SplitEntry split in projFile.SplitEntries)
			{
				if (CheckIfIniFileHasModels(split))
				{
					string categoryName = split.IniFile;
					// To prevent a crash when category names aren't defined
					if (split.CmnName != null && split.CmnName != "")
						categoryName = split.CmnName;
					AddSplitEntryToCategories(split, categoryName);
				}
			}
			foreach (SplitEntryMDL mdl in projFile.SplitMDLEntries)
			{
				if (CheckLabelFile(mdl))
				{
					string categoryName = mdl.LabelFile;
					// To prevent a crash when category names aren't defined
					if (mdl.CmnName != null && mdl.CmnName != "")
						categoryName = mdl.CmnName;
					AddMDLEntryToCategories(mdl, categoryName);
				}
			}

			// Select the first category by default, or none if there are none
			comboCategories.SelectedIndex = comboCategories.Items.Count > 0 ? 0 : -1;

			// Reselect an existing category if it was selected previously
			if (lastCategory != "" && comboCategories.Items.Count > 0)
				for (int i = 0; i < comboCategories.Items.Count; i++)
				{
					if (comboCategories.GetItemText(comboCategories.Items[i]) == lastCategory)
					{
						comboCategories.SelectedIndex = i;
						break;
					}
				}
		}

		private void ListModels()
		{
			listModels.Items.Clear();
			foreach (var item in Models)
			{
				if (textBoxFilter.Text != "" && !item.ModelName.ToLowerInvariant().Contains(textBoxFilter.Text.ToLowerInvariant()))
					continue;
				listModels.Items.Add(item.ModelName);
			}
		}

		private void comboCategories_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (comboCategories.SelectedItem == null)
				return;
			if (comboCategories.GetItemText(comboCategories.SelectedItem) == SelectedCategory)
				return;
			Models.Clear();
			SelectedCategory = comboCategories.GetItemText(comboCategories.SelectedItem);
			if (!SelectedCategory.EndsWith("Model Data"))
			{ 
				foreach (SplitEntry entry in Categories[SelectedCategory])
				{
					string inipath = Path.Combine(modFolder, entry.IniFile + "_data.ini");
					// Get models from inidata by type
					switch (Path.GetExtension(entry.SourceFile).ToLowerInvariant())
					{
						case ".dll":
							IniDictionary iniFile = SplitTools.IniFile.Load(inipath);
							foreach (var dllItem in iniFile["SAMDLData"])
							{
								SAMDLMetadata meta = new SAMDLMetadata(dllItem.Value);
								Models.Add(new ModelLoadInfo(dllItem.Key, meta, modFolder));
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
										string[] textures = new string[1];
										textures[0] = entryTexture;
										Models.Add(new ModelLoadInfo(entryDescription, entryFilename, textures, null, null, null));
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
										Models.Add(new ModelLoadInfo(item.Key, item.Value, modFolder));
										break;
									default:
										break;
								}
							}
							break;
					}
				}
			}
			else
			{
				foreach (SplitEntryMDL entry in MDLCategories[SelectedCategory])
				{
					string inipath = Path.Combine(Path.Combine(modFolder, "figure\\bin"), entry.LabelFile + "_data.ini");
					// Get models from inidata by type
					Dictionary<int, string> mdlFilenames = IniSerializer.Deserialize<Dictionary<int, string>>(inipath);
					foreach (var mdlitem in mdlFilenames)
					{
						string entryFilename = mdlitem.Value;
						string entryDescription = "";
						string entryTexture = "";
						if (mdlitem.Value.Contains("|"))
						{
							string[] mdlMeta = mdlitem.Value.Split('|');
							entryFilename = mdlMeta[0];
							if (mdlMeta.Length > 1)
								entryDescription = mdlMeta[1];
							if (mdlMeta.Length > 2)
								entryTexture = mdlMeta[2];
						}
						else
							entryDescription = Path.GetFileNameWithoutExtension(mdlitem.Value);
						switch (Path.GetExtension(entryFilename).ToLowerInvariant())
						{
							case ".sa1mdl":
							case ".sa2mdl":
							case ".sa2bmdl":
								string[] textures = new string[1];
								textures[0] = entryTexture;
								if (entryTexture == "")
									textures = null;
								Models.Add(new ModelLoadInfo(entryDescription, entryFilename, textures, null, null, null));
								break;
							default:
								break;
						}
					}
				}
			}
			ListModels();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			ModelInfo = null;
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
				else if (File.Exists(Path.Combine(modSystemFolder, pvmName) + ".GVM"))
				{
					extension = ".GVM";
					modHasTexture = true;
				}
				else if (File.Exists(Path.Combine(modSystemFolder, pvmName) + ".XVM"))
				{
					extension = ".XVM";
					modHasTexture = true;
				}
				else if (File.Exists(Path.Combine(modSystemFolder, pvmName) + ".PRS"))
				{
					extension = ".PRS";
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
				else if (File.Exists(Path.Combine(modSystemFolder, pvmName) + ".GVR"))
				{
					extension = ".GVR";
					modHasTexture = true;
				}
				else if (File.Exists(Path.Combine(modSystemFolder, pvmName) + ".XVR"))
				{
					extension = ".XVR";
					modHasTexture = true;
				}
				// Fallback on the game's system folder
				if (!modHasTexture)
				{
					if (File.Exists(Path.Combine(gameSystemFolder, pvmName) + ".PVM"))
						extension = ".PVM";
					else if (File.Exists(Path.Combine(gameSystemFolder, pvmName) + ".GVM"))
						extension = ".GVM";
					else if (File.Exists(Path.Combine(gameSystemFolder, pvmName) + ".XVM"))
						extension = ".XVM";
					else if (File.Exists(Path.Combine(gameSystemFolder, pvmName) + ".PRS"))
						extension = ".PRS";
					else if (File.Exists(Path.Combine(gameSystemFolder, pvmName) + ".PAK"))
						extension = ".PAK";
					else if (File.Exists(Path.Combine(gameSystemFolder, pvmName) + ".PB"))
						extension = ".PB";
					else if (File.Exists(Path.Combine(gameSystemFolder, pvmName) + ".PVR"))
						extension = ".PVR";
					else if (File.Exists(Path.Combine(gameSystemFolder, pvmName) + ".GVR"))
						extension = ".GVR";
					else if (File.Exists(Path.Combine(gameSystemFolder, pvmName) + ".XVR"))
						extension = ".XVR";
				}
				return Path.Combine(modHasTexture ? modSystemFolder : gameSystemFolder, pvmName) + extension;
			}
		}

		private ModelLoadInfo LoadModelIndex(int index)
		{
			if (index == -1)
				return null;

			string modelFilePath;
			string[] textureArchives = null;
			List<string> textureArchiveList = new List<string>();
			string modelName = listModels.Items[listModels.SelectedIndex].ToString();
			// Loop through the model list to find the selected model
			foreach (var model in Models)
			{
				if (model.ModelName == modelName)
				{
					// Set model filename
					modelFilePath = Path.Combine(modFolder, model.ModelFilePath);
					// Get PVM name(s) or texlist INI file
					if (model.TextureArchives != null)
					{
						// Check if there are multiple entries
						if (model.TextureArchives.Length > 1)
						{
							for (int p = 0; p < model.TextureArchives.Length; p++)
								textureArchiveList.Add(GetTextureFilename(model.TextureArchives[p]));
						}
						// If not, use the whole field as texlist INI or PVM name + optional texture IDs
						else
						{
							// If this is an INI texlist, get PVM filenames from it
							string tspath = Path.Combine(modFolder, model.TextureArchives[0]);
							if (Path.GetExtension(tspath).ToLowerInvariant() == ".ini")
							{
								TextureListEntry[] ts = TextureList.Load(tspath);
								for (int t = 0; t < ts.Length; t++)
								{
									if (ts[t].Name != null && ts[t].Name != "")
										textureArchiveList.Add(GetTextureFilename(ts[t].Name));
								}
							}
							// If not, use it as a single texture archive
							else
								textureArchiveList.Add(GetTextureFilename(model.TextureArchives[0]));
						}
					}
					// Set the list of texture archive names
					if (textureArchiveList.Count > 0)
						textureArchives = textureArchiveList.ToArray();
					return new ModelLoadInfo(model.ModelName, modelFilePath, textureArchives, model.TextureNames, model.TextureIDs, model.TexturePalettePath);
				}
			}
			return null;
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			ModelInfo = LoadModelIndex(listModels.SelectedIndex);
		}

		private void listModels_SelectedIndexChanged(object sender, EventArgs e)
		{
			buttonOK.Enabled = listModels.SelectedIndex != -1;
		}

		private void listModels_DoubleClick(object sender, EventArgs e)
		{
			if (listModels.SelectedIndex != -1)
			{
				ModelInfo = LoadModelIndex(listModels.SelectedIndex);
			}
			this.DialogResult = DialogResult.OK;
			Close();
		}

		private void textBoxFilter_TextChanged(object sender, EventArgs e)
		{
			ListModels();
		}

		private void ModelSelectDialog_Load(object sender, EventArgs e)
		{

		}
	}

	public class ModelLoadInfo
	{
		public string ModelName;
		public string ModelFilePath;
		public string[] TextureArchives;
		public NJS_TEXLIST TextureNames;
		public int[] TextureIDs;
		public string TexturePalettePath;

		public ModelLoadInfo(string name, string modelFile, string[] textures, NJS_TEXLIST texnames, int[] texids, string texturePaletteFile)
		{
			ModelName = name;
			ModelFilePath = modelFile;
			TextureArchives = textures;
			TextureNames = texnames;
			TextureIDs = texids;
			TexturePalettePath = texturePaletteFile;
		}

		public ModelLoadInfo(string name, SplitTools.FileInfo split, string modFolder)
		{
			ModelName = name;
			ModelFilePath = split.Filename;
			if (split.CustomProperties.ContainsKey("texture"))
				TextureArchives = split.CustomProperties["texture"].Split(',');
			if (split.CustomProperties.ContainsKey("texturepalette"))
				TexturePalettePath = split.CustomProperties["texturepalette"];
			if (split.CustomProperties.ContainsKey("texids"))
			{
				string[] texids_s = split.CustomProperties["texids"].Split(',');
				List<int> texid_list = new List<int>();
				for (int i = 0; i < texids_s.Length; i++)
					texid_list.Add(int.Parse(texids_s[i], System.Globalization.NumberStyles.Integer));
				TextureIDs = texid_list.ToArray();
			}
			else if (split.CustomProperties.ContainsKey("texnames"))
			{
				string texnamefile = Path.Combine(modFolder, split.CustomProperties["texnames"]);
				TextureNames = NJS_TEXLIST.Load(texnamefile);
			}
		}

		public ModelLoadInfo(string modelFilePath, SAMDLMetadata meta, string modFolder)
		{
			ModelFilePath = modelFilePath;
			ModelName = meta.ModelName;
			TextureArchives = meta.TextureArchives;
			TextureIDs = meta.TextureIDs;
			TexturePalettePath = meta.TexturePalette;
			if (meta.TextureNameFile != null && meta.TextureNameFile != "")
			{
				string texnamefile = Path.Combine(modFolder, meta.TextureNameFile);
				TextureNames = NJS_TEXLIST.Load(texnamefile);
			}
		}
	}
}