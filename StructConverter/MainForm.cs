using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SADXPCTools;
using SonicRetro.SAModel;

namespace StructConverter
{
    public partial class MainForm : Form
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
			{ "levelpathlist", "Path List" }
        };

        public MainForm()
        {
            InitializeComponent();
        }

        IniData IniData;

        private void MainForm_Load(object sender, EventArgs e)
        {
            Settings = Properties.Settings.Default;
            if (Settings.MRUList == null)
                Settings.MRUList = new StringCollection();
            StringCollection mru = new StringCollection();
            foreach (string item in Settings.MRUList)
                if (File.Exists(item))
                {
                    mru.Add(item);
                    recentProjectsToolStripMenuItem.DropDownItems.Add(item.Replace("&", "&&"));
                }
            Settings.MRUList = mru;
            if (Program.Arguments.Length > 0)
                LoadINI(Program.Arguments[0]);
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

        private void recentProjectsToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            fileToolStripMenuItem.DropDown.Close();
            LoadINI(Settings.MRUList[recentProjectsToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem)]);
        }

        private void LoadINI(string filename)
        {
            IniData = IniFile.Deserialize<IniData>(filename);
            if (Settings.MRUList.Contains(filename))
            {
                recentProjectsToolStripMenuItem.DropDownItems.RemoveAt(Settings.MRUList.IndexOf(filename));
                Settings.MRUList.Remove(filename);
            }
            Settings.MRUList.Insert(0, filename);
            recentProjectsToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(filename));
            Environment.CurrentDirectory = Path.GetDirectoryName(filename);
            listView1.BeginUpdate();
            foreach (KeyValuePair<string, SADXPCTools.FileInfo> item in IniData.Files)
            {
                bool? modified = null;
                switch (item.Value.Type)
                {
                    case "cutscenetext":
                        {
                            modified = false;
                            string[] hashes = item.Value.MD5Hash.Split(',');
                            for (int i = 0; i < 5; i++)
                            {
                                string textname = Path.Combine(item.Value.Filename, ((Languages)i).ToString() + ".txt");
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
                                    string textname = Path.Combine(Path.Combine(item.Value.Filename, (i + 1).ToString(NumberFormatInfo.InvariantInfo)), ((Languages)l).ToString() + ".ini");
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
                                    string textname = Path.Combine(Path.Combine(item.Value.Filename, (i + 1).ToString(NumberFormatInfo.InvariantInfo)), ((Languages)l).ToString() + ".ini");
                                    if (HelperFunctions.FileHash(textname) != hashes[l][i])
                                    {
                                        modified = true;
                                        break;
                                    }
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
							foreach (string dir in Directory.GetDirectories(item.Value.Filename))
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
								string dir = Path.Combine(item.Value.Filename, dirinfo.Key);
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
                        if (!item.Value.NoHash)
                            modified = HelperFunctions.FileHash(item.Value.Filename) != item.Value.MD5Hash;
                        break;
                }
                listView1.Items.Add(new ListViewItem(new[] { item.Key, DataTypeList[item.Value.Type], modified.HasValue ? (modified.Value ? "Yes" : "No") : "Unknown" }) { Checked = modified ?? true });
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

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.BeginUpdate();
            foreach (ListViewItem item in listView1.Items)
                item.Checked = true;
            listView1.EndUpdate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listView1.BeginUpdate();
            foreach (ListViewItem item in listView1.Items)
                item.Checked = item.SubItems[2].Text != "No";
            listView1.EndUpdate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listView1.BeginUpdate();
            foreach (ListViewItem item in listView1.Items)
                item.Checked = false;
            listView1.EndUpdate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog fd = new SaveFileDialog() { DefaultExt = "c", Filter = "C source files|*.c;*.cpp", InitialDirectory = Environment.CurrentDirectory, RestoreDirectory = true })
                if (fd.ShowDialog(this) == DialogResult.OK)
                    using (TextWriter writer = File.CreateText(fd.FileName))
                    {
                        Dictionary<uint, string> pointers = new Dictionary<uint, string>();
                        bool SA2 = IniData.Game == Game.SA2 || IniData.Game == Game.SA2B;
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
                                modelfmt = ModelFormat.Chunk;
                                landfmt = LandTableFormat.SA2;
                                break;
                            case Game.SA2B:
                                modelfmt = ModelFormat.Chunk;
                                landfmt = LandTableFormat.SA2B;
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
                        int _i = 0;
                        foreach (KeyValuePair<string, SADXPCTools.FileInfo> item in IniData.Files)
                            if (listView1.CheckedIndices.Contains(_i++))
                            {
                                string name = item.Key.MakeIdentifier();
                                SADXPCTools.FileInfo data = item.Value;
                                switch (data.Type)
                                {
                                    case "landtable":
                                        LandTable tbl = LandTable.LoadFromFile(data.Filename);
                                        name = tbl.Name;
                                        writer.WriteLine(tbl.ToStructVariables(landfmt, new List<string>()));
                                        break;
                                    case "model":
                                        SonicRetro.SAModel.Object mdl = new ModelFile(data.Filename).Model;
                                        name = mdl.Name;
                                        writer.WriteLine(mdl.ToStructVariables(modelfmt == ModelFormat.BasicDX, new List<string>()));
                                        models.Add(item.Key, mdl.Name);
                                        break;
                                    case "basicmodel":
                                        mdl = new SonicRetro.SAModel.ModelFile(data.Filename).Model;
                                        name = mdl.Name;
                                        writer.WriteLine(mdl.ToStructVariables(false, new List<string>()));
                                        models.Add(item.Key, mdl.Name);
                                        break;
                                    case "basicdxmodel":
                                        mdl = new SonicRetro.SAModel.ModelFile(data.Filename).Model;
                                        name = mdl.Name;
                                        writer.WriteLine(mdl.ToStructVariables(true, new List<string>()));
                                        models.Add(item.Key, mdl.Name);
                                        break;
                                    case "chunkmodel":
                                        mdl = new SonicRetro.SAModel.ModelFile(data.Filename).Model;
                                        name = mdl.Name;
                                        writer.WriteLine(mdl.ToStructVariables(false, new List<string>()));
                                        models.Add(item.Key, mdl.Name);
                                        break;
                                    case "action":
                                        Animation ani = Animation.Load(data.Filename);
                                        name = "action_" + ani.Name.MakeIdentifier();
                                        writer.WriteLine(ani.ToStructVariables());
                                        writer.WriteLine("NJS_ACTION {0} = {{ &{1}, &{2} }};", name, models[data.CustomProperties["model"]], ani.Name.MakeIdentifier());
                                        break;
                                    case "animation":
                                        ani = Animation.Load(data.Filename);
                                        name = ani.Name.MakeIdentifier();
                                        writer.WriteLine(ani.ToStructVariables());
                                        break;
                                    case "objlist":
                                        {
                                            ObjectListEntry[] list = ObjectList.Load(data.Filename, SA2);
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
                                            Dictionary<SA2LevelIDs, SA2StartPosInfo> list = SA2StartPosList.Load(data.Filename);
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
                                            Dictionary<SA1LevelAct, SA1StartPosInfo> list = SA1StartPosList.Load(data.Filename);
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
                                            TextureListEntry[] list = TextureList.Load(data.Filename);
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
                                            LevelTextureList list = LevelTextureList.Load(data.Filename);
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
                                            SA1LevelAct[] list = TrialLevelList.Load(data.Filename);
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
                                            SA1LevelAct[] list = BossLevelList.Load(data.Filename);
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
                                            Dictionary<SA1LevelIDs, FieldStartPosInfo> list = FieldStartPosList.Load(data.Filename);
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
                                            SoundTestListEntry[] list = SoundTestList.Load(data.Filename);
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
                                            MusicListEntry[] list = MusicList.Load(data.Filename);
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
                                            SoundListEntry[] list = SoundList.Load(data.Filename);
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
                                            string[] strs = StringArray.Load(data.Filename);
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
                                            NextLevelListEntry[] list = NextLevelList.Load(data.Filename);
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
                                            CutsceneText texts = new CutsceneText(data.Filename);
                                            string[] langs = new string[5];
                                            for (int j = 0; j < 5; j++)
                                            {
                                                string[] strs = texts.Text[j];
                                                Languages lang = (Languages)j;
                                                writer.WriteLine("char *{0}_{1}[] = {{", name, lang);
                                                writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", strs.Select((a) => a.ToC(lang) + " " + a.ToComment()).ToArray()));
                                                writer.WriteLine("};");
                                                writer.WriteLine();
                                                langs[j] = string.Format("{0}_{1}", name, lang);
                                            }
                                            writer.WriteLine("char **{0}[] = {{ {1} }};", name, string.Join(", ", langs));
                                        }
                                        break;
                                    case "recapscreen":
                                        {
                                            RecapScreen[][] texts = RecapScreenList.Load(data.Filename, int.Parse(data.CustomProperties["length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo));
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
                                                        SADXPCTools.HelperFunctions.ToC(scr.Speed), name, (Languages)l, j));
                                                }
                                                writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", objs.ToArray()));
                                                writer.WriteLine("};");
                                                writer.WriteLine();
                                            }
                                            writer.WriteLine("RecapScreen *{0}[] = {{", name);
                                            string[] t = new string[5];
                                            for (int l = 0; l < 5; l++)
                                                t[l] = string.Format("{0}_{1}", name, (Languages)l);
                                            writer.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", t));
                                            writer.WriteLine("};");
                                        }
                                        break;
                                    case "npctext":
                                        {
                                            NPCText[][] texts = NPCTextList.Load(data.Filename, int.Parse(data.CustomProperties["length"], NumberStyles.Integer, NumberFormatInfo.InvariantInfo));
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
                                                            foreach (NPCTextLine line in group.Lines)
                                                                objs.Add(line.ToStruct((Languages)l) + " " + line.Line.ToComment());
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
                                            LevelClearFlag[] list = LevelClearFlagList.Load(data.Filename);
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
                                            DeathZoneFlags[] list = DeathZoneFlagsList.Load(data.Filename);
                                            string path = Path.GetDirectoryName(data.Filename);
                                            List<string> mdls = new List<string>(list.Length);
                                            List<string> objs = new List<string>();
                                            for (int j = 0; j < list.Length; j++)
                                            {
                                                SonicRetro.SAModel.Object obj = new ModelFile(Path.Combine(path,
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
                                            SkyboxScale[] sclini = SkyboxScaleList.Load(data.Filename);
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
                                            StageSelectLevel[] list = StageSelectLevelList.Load(data.Filename);
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
                                            Dictionary<SA2LevelIDs, LevelRankScores> list = LevelRankScoresList.Load(data.Filename);
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
                                            Dictionary<SA2LevelIDs, LevelRankTimes> list = LevelRankTimesList.Load(data.Filename);
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
                                            Dictionary<SA2LevelIDs, SA2EndPosInfo> list = SA2EndPosList.Load(data.Filename);
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
                                            SA2AnimationInfo[] list = SA2AnimationInfoList.Load(data.Filename);
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
											foreach (string dir in Directory.GetDirectories(data.Filename))
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
														name, level.ToString().MakeIdentifier(), i,	paths[i].Unknown,
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
											writer.WriteLine("LoopDataPtr {0}[] = {{", name);
											foreach (SA1LevelAct level in levels)
												writer.WriteLine("\t{{ {0}, &{1}_{2} }},", level.ToC(), name,
													level.ToString().MakeIdentifier());
											writer.WriteLine("\t{ 0xFFFF }");
											writer.WriteLine("};");
											writer.WriteLine();
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
                        writer.WriteLine("extern \"C\" __declspec(dllexport) ModInfo {0}ModInfo = {{ ModLoaderVer, NULL, NULL, 0, NULL, 0, NULL, 0, arrayptrandlength(pointers) }};", SA2 ? "SA2" : "SADX");
                    }
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