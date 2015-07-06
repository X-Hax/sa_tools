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

namespace ModGenerator
{
    public partial class ModBuilder : Form
    {
        private Properties.Settings Settings;

        public Dictionary<string, string> DataTypeList = new Dictionary<string, string>()
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
			{ "stagelightdatalist", "Stage Light Data List" }
        };

        public ModBuilder()
        {
            InitializeComponent();
        }

        IniData IniData;
        string gameFolder;
        string projectName;
		string projectFolder;

        private void MainForm_Load(object sender, EventArgs e)
        {
            Settings = Properties.Settings.Default;

            sA2ToolStripMenuItem.Checked = !Settings.ModBuilderSADX;
            sADXToolStripMenuItem.Checked = Settings.ModBuilderSADX;

            SetGameFolder();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog a = new OpenFileDialog()
            {
                DefaultExt = "ini",
                Filter = "INI Files|*.ini|All Files|*.*"
            })
                if (a.ShowDialog(this) == DialogResult.OK)
                    LoadINI(a.FileName);
        }

        private void PickProject()
        {
            using (SonicRetro.SAModel.SAEditorCommon.UI.ProjectSelector projectSelector = new SonicRetro.SAModel.SAEditorCommon.UI.ProjectSelector(gameFolder))
            {
                if (projectSelector.NoProjects)
                {
                    MessageBox.Show("No projects found. You can create a new project in the Mod Generator main menu, by clicking the \"Start a new project\" button.");
                    return;
                }
                else
                {
                    projectSelector.ShowDialog();
                    projectName = projectSelector.SelectedProjectName;
                    projectFolder = projectSelector.SelectedProjectPath;
                }
            }

            // add all the files from the DataMappings folder
            string dataMappingsFolder = string.Concat(projectFolder, "\\DataMappings\\");

            string[] dataMappingFiles = Directory.GetFiles(dataMappingsFolder, "*.ini");

            foreach(string iniFile in dataMappingFiles)
            {
                if (Path.GetFileName(iniFile) == "sonic_data.ini") LoadINI(iniFile);
                else MessageBox.Show("Couldn't load DLL data mapping ini - it uses a different format.");
            }
        }

        private void LoadINI(string filename)
        {
            IniData = IniSerializer.Deserialize<IniData>(filename);

            Environment.CurrentDirectory = Path.GetDirectoryName(filename);

            string groupName = Path.GetFileName(filename);
            listView1.Groups.Add(groupName, groupName);

            listView1.BeginUpdate();
            foreach (KeyValuePair<string, SA_Tools.FileInfo> item in IniData.Files)
            {
				string projectRelativeFileLocation = Path.Combine(projectFolder, item.Value.Filename); // getting project-relative location

                bool? modified = null;
                switch (item.Value.Type)
                {
                    case "cutscenetext":
                        {
                            modified = false;
                            string[] hashes = item.Value.MD5Hash.Split(',');
                            for (int i = 0; i < 5; i++)
                            {
                                string textname = Path.Combine(projectRelativeFileLocation, ((Languages)i).ToString() + ".txt");
                                if (HelperFunctions.FileHash(textname) != hashes[i])
                                {
                                    modified = true;
                                    break;
                                }
                            }
                        }
                        break;
                    case "recapscreen":
                        {
                            modified = false;
                            int count = int.Parse(item.Value.CustomProperties["length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                            string[] hash2 = item.Value.MD5Hash.Split(':');
                            string[][] hashes = new string[hash2.Length][];
                            for (int i = 0; i < hash2.Length; i++)
                                hashes[i] = hash2[i].Split(',');
                            for (int i = 0; i < count; i++)
                                for (int l = 0; l < 5; l++)
                                {
                                    string textname = Path.Combine(Path.Combine(projectRelativeFileLocation, (i + 1).ToString(NumberFormatInfo.InvariantInfo)), ((Languages)l).ToString() + ".ini");
                                    if (HelperFunctions.FileHash(textname) != hashes[i][l])
                                    {
                                        modified = true;
                                        break;
                                    }
                                }
                        }
                        break;
                    case "npctext":
                        {
                            modified = false;
                            int count = int.Parse(item.Value.CustomProperties["length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
                            string[] hash2 = item.Value.MD5Hash.Split(':');
                            string[][] hashes = new string[hash2.Length][];
                            for (int i = 0; i < hash2.Length; i++)
                                hashes[i] = hash2[i].Split(',');
                            for (int l = 0; l < 5; l++)
                                for (int i = 0; i < count; i++)
                                {
                                    string textname = Path.Combine(Path.Combine(projectRelativeFileLocation, (i + 1).ToString(NumberFormatInfo.InvariantInfo)), ((Languages)l).ToString() + ".ini");
                                    if (HelperFunctions.FileHash(textname) != hashes[l][i])
                                    {
                                        modified = true;
                                        break;
                                    }
                                }
                        }
                        break;
					case "deathzone":
                        {
                            modified = false;
                            string[] hashes = item.Value.MD5Hash.Split(',');
							if (HelperFunctions.FileHash(projectRelativeFileLocation) != hashes[0])
							{
								modified = true;
								break;
							}
							DeathZoneFlags[] flags = DeathZoneFlagsList.Load(projectRelativeFileLocation);
							if (flags.Length != hashes.Length - 1)
							{
								modified = true;
								break;
							}
							string path = Path.GetDirectoryName(projectRelativeFileLocation);
							for (int i = 0; i < flags.Length; i++)
								if (HelperFunctions.FileHash(Path.Combine(path, i.ToString(NumberFormatInfo.InvariantInfo) + (IniData.Game == Game.SA2 || IniData.Game == Game.SA2B ? ".sa2mdl" : ".sa1mdl")))
									!= hashes[i + 1])
								{
									modified = true;
									break;
								}
						}
						break;
					case "levelpathlist":
						{
							modified = false;
							Dictionary<string, string[]> hashes = new Dictionary<string, string[]>();
							string[] hash1 = item.Value.MD5Hash.Split('|');
							foreach (string hash in hash1)
							{
								string[] hash2 = hash.Split(':');
								hashes.Add(hash2[0], hash2[1].Split(','));
							}
							foreach (string dir in Directory.GetDirectories(projectRelativeFileLocation))
							{
								string name = new DirectoryInfo(dir).Name;
								if (!hashes.ContainsKey(name))
								{
									modified = true;
									break;
								}
							}
							if (modified.Value)
								break;
							foreach (KeyValuePair<string, string[]> dirinfo in hashes)
							{
								string dir = Path.Combine(projectRelativeFileLocation, dirinfo.Key);
								if (!Directory.Exists(dir))
								{
									modified = true;
									break;
								}
								if (Directory.GetFiles(dir, "*.ini").Length != dirinfo.Value.Length)
								{
									modified = true;
									break;
								}
								for (int i = 0; i < dirinfo.Value.Length; i++)
								{
									string file = Path.Combine(dir, i.ToString(NumberFormatInfo.InvariantInfo) + ".ini");
									if (!File.Exists(file))
									{
										modified = true;
										break;
									}
									else if (HelperFunctions.FileHash(file) != dirinfo.Value[i])
									{
										modified = true;
										break;
									}
								}
								if (modified.Value)
									break;
							}
						}
						break;
                    default:
						if (!string.IsNullOrEmpty(item.Value.MD5Hash))
							modified = HelperFunctions.FileHash(projectRelativeFileLocation) != item.Value.MD5Hash;
                        break;
                }
                ListViewItem newItem = new ListViewItem(new[] { item.Key, DataTypeList[item.Value.Type], modified.HasValue ? (modified.Value ? "Yes" : "No") : "Unknown" }) { Checked = modified ?? true };
                newItem.Group = listView1.Groups[groupName];
                listView1.Items.Add(newItem);
            }
            listView1.EndUpdate();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Save();
        }

        private void CheckAll_Click(object sender, EventArgs e)
        {
            listView1.BeginUpdate();
            foreach (ListViewItem item in listView1.Items)
                item.Checked = true;
            listView1.EndUpdate();
        }

        private void CheckModified_Click(object sender, EventArgs e)
        {
            listView1.BeginUpdate();
            foreach (ListViewItem item in listView1.Items)
                item.Checked = item.SubItems[2].Text != "No";
            listView1.EndUpdate();
        }

        private void UnCheckAll_Click(object sender, EventArgs e)
        {
            listView1.BeginUpdate();
            foreach (ListViewItem item in listView1.Items)
                item.Checked = false;
            listView1.EndUpdate();
        }

		private void ExportCPP(TextWriter writer, bool SA2)
		{
			Dictionary<uint, string> pointers = new Dictionary<uint, string>();
			uint imagebase = IniData.ImageBase ?? 0x400000;
			ModelFormat modelfmt = 0;
			LandTableFormat landfmt = 0;
			switch (IniData.Game)
			{
				case Game.SA1:
					modelfmt = ModelFormat.Basic;
					landfmt = LandTableFormat.SA1;
					break;
				case Game.SADX:
					modelfmt = ModelFormat.BasicDX;
					landfmt = LandTableFormat.SADX;
					break;
				case Game.SA2:
				case Game.SA2B:
					modelfmt = ModelFormat.Chunk;
					landfmt = LandTableFormat.SA2;
					break;
			}
			writer.WriteLine("// Generated by SA Tools Struct Converter");
			writer.WriteLine();
			if (SA2)
				writer.WriteLine("#include \"SA2ModLoader.h\"");
			else
				writer.WriteLine("#include \"SADXModLoader.h\"");
			writer.WriteLine();
			Dictionary<string, string> models = new Dictionary<string, string>();
			foreach (KeyValuePair<string, SA_Tools.FileInfo> item in IniData.Files.Where((a, i) => listView1.CheckedIndices.Contains(i)))
			{
				string name = item.Key.MakeIdentifier();
				SA_Tools.FileInfo data = item.Value;

				string projectRelativeFileLocation = Path.Combine(projectFolder, data.Filename); // getting project-relative location

				switch (data.Type)
				{
					case "landtable":
						LandTable tbl = LandTable.LoadFromFile(projectRelativeFileLocation);
						name = tbl.Name;
						writer.WriteLine(tbl.ToStructVariables(landfmt, new List<string>()));
						break;
					case "model":
						SonicRetro.SAModel.NJS_OBJECT mdl = new ModelFile(projectRelativeFileLocation).Model;
						name = mdl.Name;
						writer.WriteLine(mdl.ToStructVariables(modelfmt == ModelFormat.BasicDX, new List<string>()));
						models.Add(item.Key, mdl.Name);
						break;
					case "basicmodel":
						mdl = new SonicRetro.SAModel.ModelFile(projectRelativeFileLocation).Model;
						name = mdl.Name;
						writer.WriteLine(mdl.ToStructVariables(false, new List<string>()));
						models.Add(item.Key, mdl.Name);
						break;
					case "basicdxmodel":
						mdl = new SonicRetro.SAModel.ModelFile(projectRelativeFileLocation).Model;
						name = mdl.Name;
						writer.WriteLine(mdl.ToStructVariables(true, new List<string>()));
						models.Add(item.Key, mdl.Name);
						break;
					case "chunkmodel":
						mdl = new SonicRetro.SAModel.ModelFile(projectRelativeFileLocation).Model;
						name = mdl.Name;
						writer.WriteLine(mdl.ToStructVariables(false, new List<string>()));
						models.Add(item.Key, mdl.Name);
						break;
					case "action":
						Animation ani = Animation.Load(projectRelativeFileLocation);
						name = "action_" + ani.Name.MakeIdentifier();
						writer.WriteLine(ani.ToStructVariables());
						writer.WriteLine("NJS_ACTION {0} = {{ &{1}, &{2} }};", name, models[data.CustomProperties["model"]], ani.Name.MakeIdentifier());
						break;
					case "animation":
						ani = Animation.Load(projectRelativeFileLocation);
						name = ani.Name.MakeIdentifier();
						writer.WriteLine(ani.ToStructVariables());
						break;
					case "objlist":
						{
							ObjectListEntry[] list = ObjectList.Load(projectRelativeFileLocation, SA2);
							writer.WriteLine("ObjectListEntry {0}_list[] = {{", name);
							List<string> objs = new List<string>(list.Length);
							foreach (ObjectListEntry obj in list)
								objs.Add(obj.ToStruct() + " " + obj.Name.ToComment());
							writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
							writer.WriteLine("};");
							writer.WriteLine();
							writer.WriteLine("ObjectList {0} = {{ arraylengthandptr({0}_list) }};", name);
						}
						break;
					case "startpos":
						if (SA2)
						{
							Dictionary<SA2LevelIDs, SA2StartPosInfo> list = SA2StartPosList.Load(projectRelativeFileLocation);
							writer.WriteLine("StartPosition {0}[] = {{", name);
							List<string> objs = new List<string>(list.Count + 1);
							foreach (KeyValuePair<SA2LevelIDs, SA2StartPosInfo> obj in list)
								objs.Add(obj.ToStruct());
							objs.Add("{ LevelIDs_Invalid }");
							writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
							writer.WriteLine("};");
						}
						else
						{
							Dictionary<SA1LevelAct, SA1StartPosInfo> list = SA1StartPosList.Load(projectRelativeFileLocation);
							writer.WriteLine("StartPosition {0}[] = {{", name);
							List<string> objs = new List<string>(list.Count + 1);
							foreach (KeyValuePair<SA1LevelAct, SA1StartPosInfo> obj in list)
								objs.Add(obj.ToStruct());
							objs.Add("{ LevelIDs_Invalid }");
							writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
							writer.WriteLine("};");
						}
						break;
					case "texlist":
						{
							TextureListEntry[] list = TextureList.Load(projectRelativeFileLocation);
							writer.WriteLine("PVMEntry {0}[] = {{", name);
							List<string> objs = new List<string>(list.Length + 1);
							foreach (TextureListEntry obj in list)
								objs.Add(obj.ToStruct());
							objs.Add("{ 0 }");
							writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
							writer.WriteLine("};");
						}
						break;
					case "leveltexlist":
						{
							LevelTextureList list = LevelTextureList.Load(projectRelativeFileLocation);
							writer.WriteLine("PVMEntry {0}_list[] = {{", name);
							List<string> objs = new List<string>(list.TextureList.Length);
							foreach (TextureListEntry obj in list.TextureList)
								objs.Add(obj.ToStruct());
							writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
							writer.WriteLine("};");
							writer.WriteLine();
							writer.WriteLine("LevelPVMList {0} = {{ {1}, arraylengthandptr({0}_list) }};", name, list.Level.ToC());
						}
						break;
					case "triallevellist":
						{
							SA1LevelAct[] list = TrialLevelList.Load(projectRelativeFileLocation);
							writer.WriteLine("TrialLevelListEntry {0}_list[] = {{", name);
							List<string> objs = new List<string>(list.Length);
							foreach (SA1LevelAct obj in list)
								objs.Add(string.Format("{{ {0}, {1} }}", obj.Level.ToC("LevelIDs"), obj.Act));
							writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
							writer.WriteLine("};");
							writer.WriteLine();
							writer.WriteLine("TrialLevelList {0} = {{ arrayptrandlength({0}_list) }};", name);
						}
						break;
					case "bosslevellist":
						{
							SA1LevelAct[] list = BossLevelList.Load(projectRelativeFileLocation);
							writer.WriteLine("__int16 {0}[] = {{", name);
							List<string> objs = new List<string>(list.Length + 1);
							foreach (SA1LevelAct obj in list)
								objs.Add(obj.ToC());
							objs.Add("levelact(LevelIDs_Invalid, 0)");
							writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
							writer.WriteLine("};");
						}
						break;
					case "fieldstartpos":
						{
							Dictionary<SA1LevelIDs, FieldStartPosInfo> list = FieldStartPosList.Load(projectRelativeFileLocation);
							writer.WriteLine("FieldStartPosition {0}[] = {{", name);
							List<string> objs = new List<string>(list.Count + 1);
							foreach (KeyValuePair<SA1LevelIDs, FieldStartPosInfo> obj in list)
								objs.Add(obj.ToStruct());
							objs.Add("{ LevelIDs_Invalid }");
							writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
							writer.WriteLine("};");
						}
						break;
					case "soundtestlist":
						{
							SoundTestListEntry[] list = SoundTestList.Load(projectRelativeFileLocation);
							writer.WriteLine("SoundTestEntry {0}_list[] = {{", name);
							List<string> objs = new List<string>(list.Length);
							foreach (SoundTestListEntry obj in list)
								objs.Add(obj.ToStruct() + " " + obj.Title.ToComment());
							writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
							writer.WriteLine("};");
							writer.WriteLine();
							writer.WriteLine("SoundTestCategory {0} = {{ arrayptrandlength({0}_list) }};", name);
						}
						break;
					case "musiclist":
						{
							MusicListEntry[] list = MusicList.Load(projectRelativeFileLocation);
							writer.WriteLine("MusicInfo {0}[] = {{", name);
							List<string> objs = new List<string>(list.Length);
							foreach (MusicListEntry obj in list)
								objs.Add(obj.ToStruct());
							writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
							writer.WriteLine("};");
						}
						break;
					case "soundlist":
						{
							SoundListEntry[] list = SoundList.Load(projectRelativeFileLocation);
							writer.WriteLine("SoundFileInfo {0}_list[] = {{", name);
							List<string> objs = new List<string>(list.Length);
							foreach (SoundListEntry obj in list)
								objs.Add(obj.ToStruct());
							writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
							writer.WriteLine("};");
							writer.WriteLine();
							writer.WriteLine("SoundList {0} = {{ arraylengthandptr({0}_list) }};", name);
						}
						break;
					case "stringarray":
						{
							string[] strs = StringArray.Load(projectRelativeFileLocation);
							Languages lang = Languages.Japanese;
							if (data.CustomProperties.ContainsKey("language"))
								lang = (Languages)Enum.Parse(typeof(Languages), data.CustomProperties["language"], true);
							writer.WriteLine("char *{0}[] = {{", name);
							List<string> objs = new List<string>(strs.Length);
							foreach (string obj in strs)
								objs.Add(obj.ToC(lang) + " " + obj.ToComment());
							writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
							writer.WriteLine("};");
						}
						break;
					case "nextlevellist":
						{
							NextLevelListEntry[] list = NextLevelList.Load(projectRelativeFileLocation);
							writer.WriteLine("NextLevelData {0}[] = {{", name);
							List<string> objs = new List<string>(list.Length + 1);
							foreach (NextLevelListEntry obj in list)
								objs.Add(obj.ToStruct());
							objs.Add("{ 0, -1 }");
							writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
							writer.WriteLine("};");
						}
						break;
					case "cutscenetext":
						{
							CutsceneText texts = new CutsceneText(projectRelativeFileLocation);
							uint addr = (uint)(data.Address + imagebase);
							for (int j = 0; j < 5; j++)
							{
								string[] strs = texts.Text[j];
								Languages lang = (Languages)j;
								writer.WriteLine("char *{0}_{1}[] = {{", name, lang);
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", strs.Select((a) => a.ToC(lang) + " " + a.ToComment()).ToArray()));
								writer.WriteLine("};");
								writer.WriteLine();
								pointers.Add(addr, string.Format("{0}_{1}", name, lang));
								addr += 4;
							}
						}
						break;
					case "recapscreen":
						{
							uint addr = (uint)(data.Address + imagebase);
							RecapScreen[][] texts = RecapScreenList.Load(projectRelativeFileLocation, int.Parse(data.CustomProperties["length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo));
							for (int l = 0; l < 5; l++)
								for (int j = 0; j < texts.Length; j++)
								{
									writer.WriteLine("char *{0}_{1}_{2}_Text[] = {{", name, (Languages)l, j);
									writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", texts[j][l].Text.Split('\n').Select((a) => a.ToC((Languages)l) + " " + a.ToComment()).ToArray()));
									writer.WriteLine("};");
									writer.WriteLine();
								}
							for (int l = 0; l < 5; l++)
							{
								writer.WriteLine("RecapScreen {0}_{1}[] = {{", name, (Languages)l);
								List<string> objs = new List<string>(texts.Length);
								for (int j = 0; j < texts.Length; j++)
								{
									RecapScreen scr = texts[j][l];
									objs.Add(string.Format("{{ {0}, arraylengthandptr({1}_{2}_{3}_Text) }}",
										SA_Tools.HelperFunctions.ToC(scr.Speed), name, (Languages)l, j));
								}
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
								writer.WriteLine();
								pointers.Add(addr, string.Format("{0}_{1}", name, (Languages)l));
								addr += 4;
							}
						}
						break;
					case "npctext":
						{
							NPCText[][] texts = NPCTextList.Load(projectRelativeFileLocation, int.Parse(data.CustomProperties["length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo));
							uint headaddr = (uint)(data.Address + imagebase);
							for (int l = 0; l < 5; l++)
							{
								for (int j = 0; j < texts[l].Length; j++)
								{
									if (texts[l][j].Groups.Count == 0)
										continue;
									if (texts[l][j].HasControl)
									{
										writer.WriteLine("__int16 {0}_{1}_{2}_Control[] = {{", name, (Languages)l, j);
										bool first = true;
										List<string> objs = new List<string>();
										foreach (NPCTextGroup group in texts[l][j].Groups)
										{
											if (!first)
												objs.Add(NPCTextControl.NewGroup.ToC());
											else
												first = false;
											foreach (ushort flag in group.EventFlags)
											{
												objs.Add(NPCTextControl.EventFlag.ToC());
												objs.Add(flag.ToCHex());
											}
											foreach (ushort flag in group.NPCFlags)
											{
												objs.Add(NPCTextControl.NPCFlag.ToC());
												objs.Add(flag.ToCHex());
											}
											if (group.Character != (SA1CharacterFlags)0xFF)
											{
												objs.Add(NPCTextControl.Character.ToC());
												objs.Add(group.Character.ToC("CharacterFlags"));
											}
											if (group.Voice.HasValue)
											{
												objs.Add(NPCTextControl.Voice.ToC());
												objs.Add(group.Voice.Value.ToString());
											}
											if (group.SetEventFlag.HasValue)
											{
												objs.Add(NPCTextControl.SetEventFlag.ToC());
												objs.Add(group.SetEventFlag.Value.ToCHex());
											}
										}
										objs.Add(NPCTextControl.End.ToC());
										writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
										writer.WriteLine("};");
										writer.WriteLine();
									}
									if (texts[l][j].HasText)
									{
										writer.WriteLine("HintText_Text {0}_{1}_{2}_Text[] = {{", name, (Languages)l, j);
										List<string> objs = new List<string>();
										foreach (NPCTextGroup group in texts[l][j].Groups)
										{
											foreach (NPCTextLine line in group.Lines)
												objs.Add(line.ToStruct((Languages)l) + " " + line.Line.ToComment());
											objs.Add("{ 0 }");
										}
										writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
										writer.WriteLine("};");
										writer.WriteLine();
									}
								}
							}
							for (int l = 0; l < 5; l++)
							{
								if (l > 0)
									writer.WriteLine();
								writer.WriteLine("HintText_Entry {0}_{1}[] = {{", name, (Languages)l);
								List<string> objs = new List<string>();
								for (int j = 0; j < texts[l].Length; j++)
								{
									if (texts[l][j].Groups.Count == 0)
									{
										objs.Add("{ 0 }");
										continue;
									}
									StringBuilder line = new StringBuilder("{ ");
									if (texts[l][j].HasControl)
										line.AppendFormat("{0}_{1}_{2}_Control", name, (Languages)l, j);
									else
										line.Append("NULL");
									line.Append(", ");
									if (texts[l][j].HasText)
										line.AppendFormat("{0}_{1}_{2}_Text", name, (Languages)l, j);
									else
										line.Append("NULL");
									line.Append(" }");
									objs.Add(line.ToString());
								}
								writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
								writer.WriteLine("};");
								pointers.Add(headaddr, string.Format("{0}_{1}", name, (Languages)l));
								headaddr += 4;
							}
						}
						break;
					case "levelclearflags":
						{
							LevelClearFlag[] list = LevelClearFlagList.Load(projectRelativeFileLocation);
							writer.WriteLine("LevelClearFlagData {0}[] = {{", name);
							List<string> objs = new List<string>(list.Length);
							foreach (LevelClearFlag obj in list)
								objs.Add(obj.ToStruct());
							objs.Add("{ -1 }");
							writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
							writer.WriteLine("};");
						}
						break;
					case "deathzone":
						{
							DeathZoneFlags[] list = DeathZoneFlagsList.Load(projectRelativeFileLocation);
							string path = Path.GetDirectoryName(projectRelativeFileLocation);
							List<string> mdls = new List<string>(list.Length);
							List<string> objs = new List<string>();
							for (int j = 0; j < list.Length; j++)
							{
								SonicRetro.SAModel.NJS_OBJECT obj = new ModelFile(Path.Combine(path,
									j.ToString(NumberFormatInfo.InvariantInfo) + ".sa1mdl")).Model;
								writer.WriteLine(obj.ToStructVariables(modelfmt == ModelFormat.BasicDX, objs));
								mdls.Add(obj.Name);
								objs.Clear();
							}
							writer.WriteLine("DeathZone {0}[] = {{", name);
							for (int j = 0; j < list.Length; j++)
								objs.Add(string.Format("{{ {0}, &{1} }}", list[j].Flags.ToC("CharacterFlags"), mdls[j]));
							objs.Add("{ 0 }");
							writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
							writer.WriteLine("};");
						}
						break;
					case "skyboxscale":
						{
							uint headaddr = (uint)(data.Address + imagebase);
							int cnt = int.Parse(data.CustomProperties["count"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
							SkyboxScale[] sclini = SkyboxScaleList.Load(projectRelativeFileLocation);
							for (int j = 0; j < cnt; j++)
							{
								writer.WriteLine("SkyboxScale {0}_{1} = {2};", name, j, sclini[j].ToStruct());
								pointers.Add(headaddr, string.Format("{0}_{1}", name, j));
								headaddr += 4;
							}
						}
						break;
					case "stageselectlist":
						{
							StageSelectLevel[] list = StageSelectLevelList.Load(projectRelativeFileLocation);
							writer.WriteLine("StageSelectLevel {0}[] = {{", name);
							List<string> objs = new List<string>(list.Length);
							foreach (StageSelectLevel obj in list)
								objs.Add(obj.ToStruct());
							writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
							writer.WriteLine("};");
						}
						break;
					case "levelrankscores":
						{
							Dictionary<SA2LevelIDs, LevelRankScores> list = LevelRankScoresList.Load(projectRelativeFileLocation);
							writer.WriteLine("LevelRankScores {0}[] = {{", name);
							List<string> objs = new List<string>(list.Count);
							foreach (KeyValuePair<SA2LevelIDs, LevelRankScores> obj in list)
								objs.Add(obj.ToStruct());
							objs.Add("{ LevelIDs_Invalid }");
							writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
							writer.WriteLine("};");
						}
						break;
					case "levelranktimes":
						{
							Dictionary<SA2LevelIDs, LevelRankTimes> list = LevelRankTimesList.Load(projectRelativeFileLocation);
							writer.WriteLine("LevelRankTimes {0}[] = {{", name);
							List<string> objs = new List<string>(list.Count);
							foreach (KeyValuePair<SA2LevelIDs, LevelRankTimes> obj in list)
								objs.Add(obj.ToStruct());
							objs.Add("{ LevelIDs_Invalid }");
							writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
							writer.WriteLine("};");
						}
						break;
					case "endpos":
						{
							Dictionary<SA2LevelIDs, SA2EndPosInfo> list = SA2EndPosList.Load(projectRelativeFileLocation);
							writer.WriteLine("LevelEndPosition {0}[] = {{", name);
							List<string> objs = new List<string>(list.Count);
							foreach (KeyValuePair<SA2LevelIDs, SA2EndPosInfo> obj in list)
								objs.Add(obj.ToStruct());
							objs.Add("{ LevelIDs_Invalid }");
							writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
							writer.WriteLine("};");
						}
						break;
					case "animationlist":
						{
							SA2AnimationInfo[] list = SA2AnimationInfoList.Load(projectRelativeFileLocation);
							writer.WriteLine("AnimationInfo {0}[] = {{", name);
							List<string> objs = new List<string>(list.Length);
							foreach (SA2AnimationInfo obj in list)
								objs.Add(obj.ToStruct());
							writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
							writer.WriteLine("};");
						}
						break;
					case "levelpathlist":
						{
							List<SA1LevelAct> levels = new List<SA1LevelAct>();
							foreach (string dir in Directory.GetDirectories(projectRelativeFileLocation))
							{
								SA1LevelAct level;
								try { level = new SA1LevelAct(new DirectoryInfo(dir).Name); }
								catch { continue; }
								levels.Add(level);
								List<PathData> paths = PathList.Load(dir);
								for (int i = 0; i < paths.Count; i++)
								{
									writer.WriteLine("Loop {0}_{1}_{2}_Entries[] = {{", name, level.ToString().MakeIdentifier(), i);
									List<string> objs = new List<string>(paths[i].Path.Count);
									foreach (PathDataEntry entry in paths[i].Path)
										objs.Add(entry.ToStruct());
									writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
									writer.WriteLine("};");
									writer.WriteLine();
									writer.WriteLine("LoopHead {0}_{1}_{2} = {{ {3}, LengthOfArray({0}_{1}_{2}_Entries), {4}, {0}_{1}_{2}_Entries, (ObjectFuncPtr){5} }};",
										name, level.ToString().MakeIdentifier(), i, paths[i].Unknown,
										HelperFunctions.ToC(paths[i].TotalDistance),
										HelperFunctions.ToCHex(paths[i].Code));
									writer.WriteLine();
								}
								writer.WriteLine("LoopHead *{0}_{1}[] = {{", name, level.ToString().MakeIdentifier());
								for (int i = 0; i < paths.Count; i++)
									writer.WriteLine("\t&{0}_{1}_{2},", name, level.ToString().MakeIdentifier(), i);
								writer.WriteLine("\tNULL");
								writer.WriteLine("};");
								writer.WriteLine();
							}
							writer.WriteLine("PathDataPtr {0}[] = {{", name);
							foreach (SA1LevelAct level in levels)
								writer.WriteLine("\t{{ {0}, {1}_{2} }},", level.ToC(), name,
									level.ToString().MakeIdentifier());
							writer.WriteLine("\t{ 0xFFFF }");
							writer.WriteLine("};");
							writer.WriteLine();
						}
						break;
					case "stagelightdatalist":
						{
							List<SA1StageLightData> list = SA1StageLightDataList.Load(projectRelativeFileLocation);
							writer.WriteLine("StageLightData {0}[] = {{", name);
							List<string> objs = new List<string>(list.Count + 1);
							foreach (SA1StageLightData obj in list)
								objs.Add(obj.ToStruct());
							objs.Add("{ 0xFFu }");
							writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
							writer.WriteLine("};");
						}
						break;
				}
				writer.WriteLine();
				if (data.PointerList != null && data.PointerList.Length > 0)
					foreach (int ptr in data.PointerList)
						pointers.Add((uint)(ptr + imagebase), name);
			}
			writer.WriteLine("PointerInfo pointers[] = {");
			List<string> ptrs = new List<string>(pointers.Count);
			foreach (KeyValuePair<uint, string> ptr in pointers)
				ptrs.Add(string.Format("ptrdecl({0}, &{1})", HelperFunctions.ToCHex(ptr.Key), ptr.Value));
			writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", ptrs.ToArray()));
			writer.WriteLine("};");
			writer.WriteLine();
		}

        private void ExportOldCPP_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog fd = new SaveFileDialog() { DefaultExt = "cpp", Filter = "C++ source files|*.cpp", InitialDirectory = Environment.CurrentDirectory, RestoreDirectory = true })
                if (fd.ShowDialog(this) == DialogResult.OK)
                    using (TextWriter writer = File.CreateText(fd.FileName))
                    {
						bool SA2 = IniData.Game == Game.SA2 || IniData.Game == Game.SA2B;
						ExportCPP(writer, SA2);
                        writer.WriteLine("extern \"C\" __declspec(dllexport) const ModInfo {0}ModInfo = {{ ModLoaderVer, NULL, NULL, 0, NULL, 0, NULL, 0, arrayptrandlength(pointers) }};", SA2 ? "SA2" : "SADX");
                    }
        }

		private void ExportNewCPP_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog fd = new SaveFileDialog() { DefaultExt = "cpp", Filter = "C++ source files|*.cpp", InitialDirectory = Environment.CurrentDirectory, RestoreDirectory = true })
				if (fd.ShowDialog(this) == DialogResult.OK)
					using (TextWriter writer = File.CreateText(fd.FileName))
					{
						bool SA2 = IniData.Game == Game.SA2 || IniData.Game == Game.SA2B;
						ExportCPP(writer, SA2);
						writer.WriteLine("extern \"C\" __declspec(dllexport) const PointerList Pointers = { arrayptrandlength(pointers) };");
						writer.WriteLine();
						writer.WriteLine("extern \"C\" __declspec(dllexport) const ModInfo {0}ModInfo = {{ ModLoaderVer }};", SA2 ? "SA2" : "SADX");
					}
		}

		private void CopyDirectory(DirectoryInfo src, string dst)
		{
			if (!Directory.Exists(dst))
				Directory.CreateDirectory(dst);
			foreach (DirectoryInfo dir in src.GetDirectories())
				CopyDirectory(dir, Path.Combine(dst, dir.Name));
			foreach (System.IO.FileInfo fil in src.GetFiles())
				fil.CopyTo(Path.Combine(dst, fil.Name), true);
		}

		private void INIExport_Click(object sender, EventArgs e)
		{
            using (SaveFileDialog fd = new SaveFileDialog() { DefaultExt = "ini", Filter = "INI files|*.ini", InitialDirectory = Environment.CurrentDirectory, RestoreDirectory = true })
				if (fd.ShowDialog(this) == DialogResult.OK)
				{
					string dstfol = Path.GetDirectoryName(fd.FileName);
					IniData output = new IniData();
					output.Files = new Dictionary<string, SA_Tools.FileInfo>();
					foreach (KeyValuePair<string, SA_Tools.FileInfo> item in IniData.Files.Where((a, i) => listView1.CheckedIndices.Contains(i)))
					{
						string projectRelativeFileLocation = Path.Combine(projectFolder, item.Value.Filename); // getting project-relative location

						if (Directory.Exists(projectRelativeFileLocation))
							Directory.CreateDirectory(Path.Combine(dstfol, projectRelativeFileLocation));
						else
							Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(dstfol, projectRelativeFileLocation)));
						switch (item.Value.Type)
						{
							case "deathzone":
								DeathZoneFlags[] list = DeathZoneFlagsList.Load(projectRelativeFileLocation);
								string path = Path.GetDirectoryName(projectRelativeFileLocation);
								for (int j = 0; j < list.Length; j++)
								{
									System.IO.FileInfo fil = new System.IO.FileInfo(Path.Combine(path, j.ToString(NumberFormatInfo.InvariantInfo) + ".sa1mdl"));
									fil.CopyTo(Path.Combine(Path.Combine(dstfol, path), j.ToString(NumberFormatInfo.InvariantInfo)), true);
								}
								File.Copy(projectRelativeFileLocation, Path.Combine(dstfol, projectRelativeFileLocation), true);
								break;
							default:
								if (Directory.Exists(projectRelativeFileLocation))
									CopyDirectory(new DirectoryInfo(projectRelativeFileLocation), Path.Combine(dstfol, projectRelativeFileLocation));
								else
									File.Copy(projectRelativeFileLocation, Path.Combine(dstfol, projectRelativeFileLocation), true);
								break;
						}
						item.Value.MD5Hash = null;
						output.Files.Add(item.Key, item.Value);
					}
					IniSerializer.Serialize(output, fd.FileName);
				}
		}

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This program allows you to convert split data into C code, that can be compiled into a DLL file for ModLoader.\nTo switch games, use the checkboxes in the File->Game menu.");
        }

        private void sADXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sA2ToolStripMenuItem.CheckState = CheckState.Unchecked;

            // todo: switch our game mode, then bring up the projects pop-up so the user can select.
            string errorMessage = "None supplied";
            DialogResult lookForNewPath = System.Windows.Forms.DialogResult.None;
            if (Properties.Settings.Default.SADXPath == "" || (!SonicRetro.SAModel.SAEditorCommon.UI.ProjectSelector.VerifyGamePath(SA_Tools.Game.SADX, Properties.Settings.Default.SADXPath, out errorMessage)))
            {
                // show an error message that the sadx path is invalid, ask for a new one.
                lookForNewPath = MessageBox.Show(string.Format("The on-record SADX game directory doesn't appear to be valid because: {0}\nOK to supply one, Cancel to ignore.", errorMessage), "Directory Warning", MessageBoxButtons.OKCancel);
                if (lookForNewPath == System.Windows.Forms.DialogResult.OK)
                {
                    if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK) Properties.Settings.Default.SADXPath = folderBrowser.SelectedPath;
                }
            }

            Properties.Settings.Default.ModBuilderSADX = true; 

            Properties.Settings.Default.Save();
            SetGameFolder();
        }

        private void sA2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sADXToolStripMenuItem.CheckState = CheckState.Unchecked;

            // todo: switch our game mode, then bring up the projects pop-up so the user can select.
            string errorMessage = "None supplied";
            DialogResult lookForNewPath = System.Windows.Forms.DialogResult.None;
            if (Properties.Settings.Default.SA2Path == "" || (!SonicRetro.SAModel.SAEditorCommon.UI.ProjectSelector.VerifyGamePath(SA_Tools.Game.SA2B, Properties.Settings.Default.SA2Path, out errorMessage)))
            {
                // show an error message that the sadx path is invalid, ask for a new one.
                lookForNewPath = MessageBox.Show(string.Format("The on-record SA2PC game directory doesn't appear to be valid because: {0}\nOK to supply one, Cancel to ignore.", errorMessage), "Directory Warning", MessageBoxButtons.OKCancel);
                if (lookForNewPath == System.Windows.Forms.DialogResult.OK)
                {
                    if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK) Properties.Settings.Default.SA2Path = folderBrowser.SelectedPath;
                }
            }

            Properties.Settings.Default.ModBuilderSADX = false; 

            Properties.Settings.Default.Save();
            SetGameFolder();
        }

        private void SetGameFolder()
        {
            if (sADXToolStripMenuItem.Checked) gameFolder = Properties.Settings.Default.SADXPath;
            else if (sA2ToolStripMenuItem.Checked) gameFolder = Properties.Settings.Default.SA2Path;

            PickProject();
        }

        private void projectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PickProject();
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