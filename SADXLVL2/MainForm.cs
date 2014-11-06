using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SA_Tools;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.Direct3D.TextureSystem;

using SonicRetro.SAModel.SAEditorCommon;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;
using SonicRetro.SAModel.SAEditorCommon.UI;

namespace SonicRetro.SAModel.SADXLVL2
{
	public partial class MainForm : Form
	{
		Properties.Settings Settings = Properties.Settings.Default;

		public MainForm()
		{
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
			InitializeComponent();
		}

		void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			using (ErrorDialog ed = new ErrorDialog(e.Exception, true))
				if (ed.ShowDialog(this) == System.Windows.Forms.DialogResult.Cancel)
					Close();
		}

		internal Device d3ddevice;
		Dictionary<string, Dictionary<string, string>> ini;
		EditorCamera cam = new EditorCamera(EditorOptions.RenderDrawDistance);
		string levelID;
		internal string levelName;
		bool loaded;
		internal List<Item> SelectedItems;
		Dictionary<string, ToolStripMenuItem> levelMenuItems;
		bool lookKeyDown;
		bool zoomKeyDown;

		// helpers / ui stuff
		TransformGizmo transformGizmo;
		PointHelper cameraPointA;
		PointHelper cameraPointB;
		PointHelper miscHelper; // use this for anything you like, maybe for SET things like rocket / dash ring destinations?

		private void MainForm_Load(object sender, EventArgs e)
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
			d3ddevice = new Device(0, DeviceType.Hardware, panel1.Handle, CreateFlags.SoftwareVertexProcessing, new PresentParameters[] { new PresentParameters() { Windowed = true, SwapEffect = SwapEffect.Discard, EnableAutoDepthStencil = true, AutoDepthStencilFormat = DepthFormat.D24X8 } });
			EditorOptions.InitializeDefaultLights(d3ddevice);
			Gizmo.InitGizmo(d3ddevice);
			ObjectHelper.Init(d3ddevice, Properties.Resources.UnknownImg);
			if (Settings.MRUList == null)
				Settings.MRUList = new System.Collections.Specialized.StringCollection();
			System.Collections.Specialized.StringCollection mru = new System.Collections.Specialized.StringCollection();
			foreach (string item in Settings.MRUList)
				if (File.Exists(item))
				{
					mru.Add(item);
					recentProjectsToolStripMenuItem.DropDownItems.Add(item.Replace("&", "&&"));
				}
			Settings.MRUList = mru;
			if (mru.Count > 0) recentProjectsToolStripMenuItem.DropDownItems.Remove(noneToolStripMenuItem2);
			if (Program.args.Length > 0)
				LoadINI(Program.args[0]);

			LevelData.StateChanged += new LevelData.LevelStateChangeHandler(LevelData_StateChanged);

			panel1.MouseWheel += new MouseEventHandler(panel1_MouseWheel);
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (loaded)
				switch (MessageBox.Show(this, "Do you want to save?", "SADXLVL2", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
				{
					case DialogResult.Yes:
						saveToolStripMenuItem_Click(this, EventArgs.Empty);
						break;
					case DialogResult.Cancel:
						return;
				}
			OpenFileDialog a = new OpenFileDialog()
			{
				DefaultExt = "ini",
				Filter = "INI Files|*.ini|All Files|*.*"
			};
			if (a.ShowDialog(this) == DialogResult.OK)
				LoadINI(a.FileName);
		}

		private void LoadINI(string filename)
		{
			loaded = false;
			ini = IniFile.Load(filename);
			Environment.CurrentDirectory = Path.GetDirectoryName(filename);
			changeLevelToolStripMenuItem.DropDownItems.Clear();
			levelMenuItems = new Dictionary<string, ToolStripMenuItem>();
			foreach (KeyValuePair<string, Dictionary<string, string>> item in ini)
				if (!string.IsNullOrEmpty(item.Key))
				{
					string[] itempath = item.Key.Split('\\');
					ToolStripMenuItem parent = changeLevelToolStripMenuItem;
					for (int i = 0; i < itempath.Length - 1; i++)
					{
						string curpath = string.Empty;
						if (i - 1 >= 0)
							curpath = string.Join(@"\", itempath, 0, i - 1);
						if (!string.IsNullOrEmpty(curpath))
							parent = levelMenuItems[curpath];
						curpath += itempath[i];
						if (!levelMenuItems.ContainsKey(curpath))
						{
							ToolStripMenuItem it = new ToolStripMenuItem(itempath[i].Replace("&", "&&")) { Tag = curpath };
							levelMenuItems.Add(curpath, it);
							parent.DropDownItems.Add(it);
							parent = it;
						}
						else
							parent = levelMenuItems[curpath];
					}
					ToolStripMenuItem ts = new ToolStripMenuItem(itempath[itempath.Length - 1], null, new EventHandler(LevelToolStripMenuItem_Clicked)) { Tag = item.Key };
					levelMenuItems.Add(item.Key, ts);
					parent.DropDownItems.Add(ts);
				}
			if (Settings.MRUList.Count == 0)
				recentProjectsToolStripMenuItem.DropDownItems.Remove(noneToolStripMenuItem2);
			if (Settings.MRUList.Contains(filename))
			{
				recentProjectsToolStripMenuItem.DropDownItems.RemoveAt(Settings.MRUList.IndexOf(filename));
				Settings.MRUList.Remove(filename);
			}
			Settings.MRUList.Insert(0, filename);
			recentProjectsToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(filename));
		}

		private void LevelToolStripMenuItem_Clicked(object sender, EventArgs e)
		{
			fileToolStripMenuItem.HideDropDown();
			if (loaded)
				switch (MessageBox.Show(this, "Do you want to save?", "SADXLVL2", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
				{
					case DialogResult.Yes:
						saveToolStripMenuItem_Click(this, EventArgs.Empty);
						break;
					case DialogResult.Cancel:
						return;
				}
			loaded = false;
			SelectedItems = new List<Item>();
			SelectedItemChanged();
			foreach (ToolStripMenuItem item in levelMenuItems.Values)
				item.Checked = false;
			((ToolStripMenuItem)sender).Checked = true;
			levelID = (string)((ToolStripMenuItem)sender).Tag;
			UseWaitCursor = true;
			Enabled = false;
			string[] itempath = levelID.Split('\\');
			levelName = itempath[itempath.Length - 1];
			LevelData.LevelName = levelName;
			Text = "SADXLVL2 - Loading " + levelName + "...";
#if !DEBUG
			backgroundWorker1.RunWorkerAsync();
#else
			backgroundWorker1_DoWork(null, null);
			backgroundWorker1_RunWorkerCompleted(null, null);
#endif
		}

		bool initerror = false;
		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
#if !DEBUG
			try
			{
#endif
			LevelData.Character = 0;
			Dictionary<string, string> group = ini[levelID];
			string syspath = Path.Combine(Environment.CurrentDirectory, ini[string.Empty]["syspath"]);
			string modpath = null;
			if (ini[string.Empty].ContainsKey("modpath"))
				modpath = ini[string.Empty]["modpath"];
			SA1LevelAct levelact = new SA1LevelAct(group.GetValueOrDefault("LevelID", "0000"));
			LevelData.leveltexs = null;
			cam = new EditorCamera(EditorOptions.RenderDrawDistance);
			if (!group.ContainsKey("LevelGeo"))
				LevelData.geo = null;
			else
			{
				LevelData.geo = LandTable.LoadFromFile(group["LevelGeo"]);
				LevelData.LevelItems = new List<LevelItem>();
				foreach (COL item in LevelData.geo.COL)
					LevelData.LevelItems.Add(new LevelItem(item, d3ddevice));
			}
			LevelData.TextureBitmaps = new Dictionary<string, BMPInfo[]>();
			LevelData.Textures = new Dictionary<string, Texture[]>();
			if (LevelData.geo != null && !string.IsNullOrEmpty(LevelData.geo.TextureFileName))
			{
				BMPInfo[] TexBmps = TextureArchive.GetTextures(System.IO.Path.Combine(syspath, LevelData.geo.TextureFileName) + ".PVM");
				Texture[] texs = new Texture[TexBmps.Length];
				for (int j = 0; j < TexBmps.Length; j++)
					texs[j] = new Texture(d3ddevice, TexBmps[j].Image, Usage.SoftwareProcessing, Pool.Managed);
				if (!LevelData.TextureBitmaps.ContainsKey(LevelData.geo.TextureFileName))
					LevelData.TextureBitmaps.Add(LevelData.geo.TextureFileName, TexBmps);
				if (!LevelData.Textures.ContainsKey(LevelData.geo.TextureFileName))
					LevelData.Textures.Add(LevelData.geo.TextureFileName, texs);
				LevelData.leveltexs = LevelData.geo.TextureFileName;
			}

			#region Start Positions
			LevelData.StartPositions = new StartPosItem[LevelData.Characters.Length];
			for (int i = 0; i < LevelData.StartPositions.Length; i++)
			{
				Dictionary<SA1LevelAct, SA1StartPosInfo> posini = SA1StartPosList.Load(ini[string.Empty][LevelData.Characters[i] + "start"]);
				Vertex pos = new Vertex();
				int rot = 0;
				if (posini.ContainsKey(levelact))
				{
					pos = posini[levelact].Position.ToSAModel();
					rot = posini[levelact].YRotation;
				}
				if (i == 0 & levelact.Level == SA1LevelIDs.PerfectChaos)
					LevelData.StartPositions[i] = new StartPosItem(new ModelFile(ini[string.Empty]["supermdl"]).Model, ini[string.Empty]["supertex"], float.Parse(ini[string.Empty]["superheight"], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo), pos, rot, d3ddevice);
				else
					LevelData.StartPositions[i] = new StartPosItem(new ModelFile(ini[string.Empty][LevelData.Characters[i] + "mdl"]).Model, ini[string.Empty][LevelData.Characters[i] + "tex"], float.Parse(ini[string.Empty][LevelData.Characters[i] + "height"], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo), pos, rot, d3ddevice);
				TextureListEntry[] texini = TextureList.Load(ini[string.Empty][LevelData.Characters[i] + "texlist"]);
				for (int ti = 0; ti < texini.Length; ti++)
				{
					string texname = texini[ti].Name;
					if (!string.IsNullOrEmpty(texname) && !LevelData.TextureBitmaps.ContainsKey(texname))
					{
						BMPInfo[] TexBmps = TextureArchive.GetTextures(System.IO.Path.Combine(syspath, texname) + ".PVM");
						Texture[] texs = new Texture[TexBmps.Length];
						for (int j = 0; j < TexBmps.Length; j++)
							texs[j] = new Texture(d3ddevice, TexBmps[j].Image, Usage.SoftwareProcessing, Pool.Managed);
						LevelData.TextureBitmaps.Add(texname, TexBmps);
						LevelData.Textures.Add(texname, texs);
					}
				}
			}
			#endregion

			#region Death Zones
			if (!group.ContainsKey("DeathZones"))
				LevelData.DeathZones = null;
			else
			{
				LevelData.DeathZones = new List<DeathZoneItem>();
				DeathZoneFlags[] dzini = DeathZoneFlagsList.Load(group["DeathZones"]);
				string path = Path.GetDirectoryName(group["DeathZones"]);
				int cnt = 0;
				for (int i = 0; i < dzini.Length; i++)
				{
					LevelData.DeathZones.Add(new DeathZoneItem(
						new ModelFile(Path.Combine(path, i.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + ".sa1mdl")).Model,
						dzini[i].Flags, d3ddevice));
					cnt++;
				}
			}
			#endregion

			#region Textures and Texture Lists
			TextureListEntry[] objtexini = TextureList.Load(ini[string.Empty]["objtexlist"]);
			for (int oti = 0; oti < objtexini.Length; oti++)
			{
				string texname = objtexini[oti].Name;
				if (!string.IsNullOrEmpty(texname) & !LevelData.TextureBitmaps.ContainsKey(texname))
				{
					BMPInfo[] TexBmps = TextureArchive.GetTextures(System.IO.Path.Combine(syspath, texname) + ".PVM");
					Texture[] texs = new Texture[TexBmps.Length];
					for (int j = 0; j < TexBmps.Length; j++)
						texs[j] = new Texture(d3ddevice, TexBmps[j].Image, Usage.SoftwareProcessing, Pool.Managed);
					LevelData.TextureBitmaps.Add(texname, TexBmps);
					LevelData.Textures.Add(texname, texs);
				}
			}
			foreach (string file in Directory.GetFiles(ini[string.Empty]["leveltexlists"]))
			{
				LevelTextureList texini = LevelTextureList.Load(file);
				if (texini.Level != levelact) continue;
				for (int ti = 0; ti < texini.TextureList.Length; ti++)
				{
					string texname = texini.TextureList[ti].Name;
					if (!string.IsNullOrEmpty(texname) && !LevelData.TextureBitmaps.ContainsKey(texname))
					{
						BMPInfo[] TexBmps = TextureArchive.GetTextures(System.IO.Path.Combine(syspath, texname) + ".PVM");
						Texture[] texs = new Texture[TexBmps.Length];
						for (int j = 0; j < TexBmps.Length; j++)
							texs[j] = new Texture(d3ddevice, TexBmps[j].Image, Usage.SoftwareProcessing, Pool.Managed);
						LevelData.TextureBitmaps.Add(texname, TexBmps);
						LevelData.Textures.Add(texname, texs);
					}
				}
			}
			objtexini = TextureList.Load(group["ObjTexs"]);
			for (int oti = 0; oti < objtexini.Length; oti++)
			{
				string texname = objtexini[oti].Name;
				if (!string.IsNullOrEmpty(texname) & !LevelData.TextureBitmaps.ContainsKey(texname))
				{
					BMPInfo[] TexBmps = TextureArchive.GetTextures(System.IO.Path.Combine(syspath, texname) + ".PVM");
					Texture[] texs = new Texture[TexBmps.Length];
					for (int j = 0; j < TexBmps.Length; j++)
						texs[j] = new Texture(d3ddevice, TexBmps[j].Image, Usage.SoftwareProcessing, Pool.Managed);
					LevelData.TextureBitmaps.Add(texname, TexBmps);
					LevelData.Textures.Add(texname, texs);
				}
			}
			if (group.ContainsKey("Textures"))
			{
				string[] textures = group["Textures"].Split(',');
				foreach (string tex in textures)
				{
					if (!LevelData.TextureBitmaps.ContainsKey(tex))
					{
						BMPInfo[] TexBmps = TextureArchive.GetTextures(System.IO.Path.Combine(syspath, tex) + ".PVM");
						Texture[] texs = new Texture[TexBmps.Length];
						for (int j = 0; j < TexBmps.Length; j++)
							texs[j] = new Texture(d3ddevice, TexBmps[j].Image, Usage.SoftwareProcessing, Pool.Managed);
						LevelData.TextureBitmaps.Add(tex, TexBmps);
						LevelData.Textures.Add(tex, texs);
					}
					if (string.IsNullOrEmpty(LevelData.leveltexs))
						LevelData.leveltexs = tex;
				}
			}
			#endregion

			#region Object Definitions / SET Layout
			LevelData.ObjDefs = new List<ObjectDefinition>();
			Dictionary<string, ObjectData> objdefini = IniSerializer.Deserialize<Dictionary<string, ObjectData>>(ini[string.Empty]["objdefs"]);
			if (File.Exists(group.GetValueOrDefault("ObjList", string.Empty)))
			{
				ObjectListEntry[] objlstini = ObjectList.Load(group["ObjList"], false);
				Directory.CreateDirectory("dllcache").Attributes |= FileAttributes.Hidden;
				for (int ID = 0; ID < objlstini.Length; ID++)
				{
					string codeaddr = objlstini[ID].CodeString;
					if (!objdefini.ContainsKey(codeaddr))
						codeaddr = "0";
					ObjectData defgroup = objdefini[codeaddr];
					ObjectDefinition def = null;
					if (!string.IsNullOrEmpty(defgroup.CodeFile))
					{
						string ty = defgroup.CodeType;
						string dllfile = System.IO.Path.Combine("dllcache", ty + ".dll");
						DateTime modDate = DateTime.MinValue;
						if (System.IO.File.Exists(dllfile))
							modDate = System.IO.File.GetLastWriteTime(dllfile);
						string fp = defgroup.CodeFile.Replace('/', System.IO.Path.DirectorySeparatorChar);
						if (modDate >= File.GetLastWriteTime(fp) && modDate > File.GetLastWriteTime(Application.ExecutablePath))
							def = (ObjectDefinition)Activator.CreateInstance(System.Reflection.Assembly.LoadFile(System.IO.Path.Combine(Environment.CurrentDirectory, dllfile)).GetType(ty));
						else
						{
							string ext = System.IO.Path.GetExtension(fp);
							CodeDomProvider pr = null;
							switch (ext.ToLowerInvariant())
							{
								case ".cs":
									pr = new Microsoft.CSharp.CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
									break;
								case ".vb":
									pr = new Microsoft.VisualBasic.VBCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
									break;
							}
							if (pr != null)
							{
								CompilerParameters para = new CompilerParameters(new string[] { "System.dll", "System.Core.dll", "System.Drawing.dll", Assembly.GetAssembly(typeof(Vector3)).Location, Assembly.GetAssembly(typeof(Texture)).Location, Assembly.GetAssembly(typeof(D3DX)).Location, Assembly.GetExecutingAssembly().Location, Assembly.GetAssembly(typeof(LandTable)).Location, Assembly.GetAssembly(typeof(EditorCamera)).Location, Assembly.GetAssembly(typeof(SA1LevelAct)).Location, Assembly.GetAssembly(typeof(ObjectDefinition)).Location });
								para.GenerateExecutable = false;
								para.GenerateInMemory = false;
								para.IncludeDebugInformation = true;
								para.OutputAssembly = System.IO.Path.Combine(Environment.CurrentDirectory, dllfile);
								CompilerResults res = pr.CompileAssemblyFromFile(para, fp);
								if (res.Errors.HasErrors)
									def = new DefaultObjectDefinition();
								else
									def = (ObjectDefinition)Activator.CreateInstance(res.CompiledAssembly.GetType(ty));
							}
							else
								def = new DefaultObjectDefinition();
						}
					}
					else
						def = new DefaultObjectDefinition();
					LevelData.ObjDefs.Add(def);
					def.Init(defgroup, objlstini[ID].Name, d3ddevice);
				}

				// Loading SET Layout
				if (LevelData.ObjDefs.Count > 0)
				{
					LevelData.SETName = group.GetValueOrDefault("SETName", ((int)levelact.Level).ToString("00") + levelact.Act.ToString("00"));
					string setstr = Path.Combine(syspath, "SET" + LevelData.SETName + "{0}.bin");
					LevelData.SETItems = new List<SETItem>[LevelData.SETChars.Length];
					for (int i = 0; i < LevelData.SETChars.Length; i++)
					{
						List<SETItem> list = new List<SETItem>();
						byte[] setfile = null;
						if (modpath != null && File.Exists(Path.Combine(modpath, string.Format(setstr, LevelData.SETChars[i]))))
							setfile = File.ReadAllBytes(Path.Combine(modpath, string.Format(setstr, LevelData.SETChars[i])));
						else if (File.Exists(string.Format(setstr, LevelData.SETChars[i])))
							setfile = File.ReadAllBytes(string.Format(setstr, LevelData.SETChars[i]));
						if (setfile != null)
						{
							int count = BitConverter.ToInt32(setfile, 0);
							int address = 0x20;
							for (int j = 0; j < count; j++)
							{
								SETItem ent = new SETItem(setfile, address);
								list.Add(ent);
								address += 0x20;
							}
						}
						LevelData.SETItems[i] = list;
					}
				}
				else
					LevelData.SETItems = null;
			}
			else
				LevelData.SETItems = null;
			#endregion

			#region CAM Layout
			LevelData.CAMName = ((int)levelact.Level).ToString("00") + levelact.Act.ToString("00");
			string camstr = Path.Combine(syspath, "CAM" + LevelData.CAMName + "{0}.bin");

			LevelData.CAMItems = new List<CAMItem>[LevelData.SETChars.Length];
			for (int i = 0; i < LevelData.SETChars.Length; i++)
			{
				List<CAMItem> list = new List<CAMItem>();
				byte[] camfile = null;
				if (modpath != null && File.Exists(Path.Combine(modpath, string.Format(camstr, LevelData.SETChars[i]))))
					camfile = File.ReadAllBytes(Path.Combine(modpath, string.Format(camstr, LevelData.SETChars[i])));
				else if (File.Exists(string.Format(camstr, LevelData.SETChars[i])))
					camfile = File.ReadAllBytes(string.Format(camstr, LevelData.SETChars[i]));
				if (camfile != null)
				{
					int count = BitConverter.ToInt32(camfile, 0);
					int address = 0x40;
					for (int j = 0; j < count; j++)
					{
						CAMItem ent = new CAMItem(camfile, address);
						list.Add(ent);
						address += 0x40;
					}
				}

				LevelData.CAMItems[i] = list;
			}

			CAMItem.Init(d3ddevice);

			#endregion

			#region Loading Level Effects
			LevelData.leveleff = null;
			if (group.ContainsKey("Effects"))
			{
				LevelDefinition def = null;
				string ty = "SADXObjectDefinitions.Level_Effects." + Path.GetFileNameWithoutExtension(group["Effects"]);
				string dllfile = Path.Combine("dllcache", ty + ".dll");
				DateTime modDate = DateTime.MinValue;
				if (File.Exists(dllfile))
					modDate = File.GetLastWriteTime(dllfile);
				string fp = group["Effects"].Replace('/', System.IO.Path.DirectorySeparatorChar);
				if (modDate >= File.GetLastWriteTime(fp) && modDate > File.GetLastWriteTime(Application.ExecutablePath))
					def = (LevelDefinition)Activator.CreateInstance(System.Reflection.Assembly.LoadFile(System.IO.Path.Combine(Environment.CurrentDirectory, dllfile)).GetType(ty));
				else
				{
					string ext = System.IO.Path.GetExtension(fp);
					CodeDomProvider pr = null;
					switch (ext.ToLowerInvariant())
					{
						case ".cs":
							pr = new Microsoft.CSharp.CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
							break;
						case ".vb":
							pr = new Microsoft.VisualBasic.VBCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
							break;
					}
					if (pr != null)
					{
						CompilerParameters para = new CompilerParameters(new string[] { "System.dll", "System.Core.dll", "System.Drawing.dll", Assembly.GetAssembly(typeof(Vector3)).Location, Assembly.GetAssembly(typeof(Texture)).Location, Assembly.GetAssembly(typeof(D3DX)).Location, Assembly.GetExecutingAssembly().Location, Assembly.GetAssembly(typeof(LandTable)).Location, Assembly.GetAssembly(typeof(EditorCamera)).Location, Assembly.GetAssembly(typeof(SA1LevelAct)).Location, Assembly.GetAssembly(typeof(Item)).Location });
						para.GenerateExecutable = false;
						para.GenerateInMemory = false;
						para.IncludeDebugInformation = true;
						para.OutputAssembly = Path.Combine(Environment.CurrentDirectory, dllfile);
						CompilerResults res = pr.CompileAssemblyFromFile(para, fp);
						if (!res.Errors.HasErrors)
							def = (LevelDefinition)Activator.CreateInstance(res.CompiledAssembly.GetType(ty));
					}
				}
				if (def != null)
					def.Init(group, levelact.Act, d3ddevice);
				LevelData.leveleff = def;
			}
			#endregion

			#region Loading Splines
			LevelData.LevelSplines = new List<SplineData>();

			if (ini[string.Empty].ContainsKey("paths"))
			{
				String splineDirectory = Path.Combine(Path.Combine(Environment.CurrentDirectory, ini[string.Empty]["paths"]), levelact.ToString());

				if (Directory.Exists(splineDirectory))
				{
					List<Dictionary<string, Dictionary<string, string>>> pathFiles = new List<Dictionary<string, Dictionary<string, string>>>();

					for (int i = 0; i < int.MaxValue; i++)
					{
						string path = string.Concat(splineDirectory, string.Format("/{0}.ini", i));
						if (File.Exists(path))
						{
							pathFiles.Add(IniFile.Load(path));
						}
						else break;
					}

					foreach (Dictionary<string, Dictionary<string, string>> pathFile in pathFiles) // looping through path files
					{
						SplineData newSpline = new SplineData();

						for (int iniEntryIndx = 0; iniEntryIndx < pathFile.Count - 1; iniEntryIndx++)
						{
							Vertex knotPosition = new Vertex();
							ushort XRot = 0, YRot = 0;
							float knotDistance = 0;

							if (pathFile[iniEntryIndx.ToString()].ContainsKey("XRotation"))
							{
								XRot = ushort.Parse(pathFile[iniEntryIndx.ToString()]["XRotation"], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
							}

							if (pathFile[iniEntryIndx.ToString()].ContainsKey("YRotation"))
							{
								YRot = ushort.Parse(pathFile[iniEntryIndx.ToString()]["YRotation"], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
							}

							if (pathFile[iniEntryIndx.ToString()].ContainsKey("Position"))
							{
								string[] vertexComponents = pathFile[iniEntryIndx.ToString()]["Position"].Split(',');

								if (vertexComponents.Length == 3)
								{
									knotPosition.X = float.Parse(vertexComponents[0]);
									knotPosition.Y = float.Parse(vertexComponents[1]);
									knotPosition.Z = float.Parse(vertexComponents[2]);
								}
							}

							if (pathFile[iniEntryIndx.ToString()].ContainsKey("Distance"))
							{
								knotDistance = float.Parse(pathFile[iniEntryIndx.ToString()]["Distance"]);
							}

							newSpline.AddKnot(new Knot(knotPosition, new Rotation(XRot, YRot, 0), knotDistance));
						}

						LevelData.LevelSplines.Add(newSpline);
					}
				}
			}
			#endregion

			transformGizmo = new TransformGizmo();

			cameraPointA = new PointHelper(); cameraPointA.BoxTexture = Gizmo.ATexture; cameraPointA.DrawCube = true;
			cameraPointB = new PointHelper(); cameraPointB.BoxTexture = Gizmo.BTexture; cameraPointB.DrawCube = true;
#if !DEBUG
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					ex.GetType().Name + ": " + ex.Message + "\nLog file has been saved to " + System.IO.Path.Combine(Environment.CurrentDirectory, "SADXLVL2.log") + ".\nSend this to MainMemory on the Sonic Retro forums.",
					"SADXLVL2 Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				File.WriteAllText("SADXLVL2.log", ex.ToString());
				initerror = true;
			}
#endif
		}

		private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (initerror)
			{
				Close();
				return;
			}
			levelPieceToolStripMenuItem.Enabled = LevelData.geo != null;
			clearLevelToolStripMenuItem.Enabled = LevelData.geo != null;
			objectToolStripMenuItem.Enabled = LevelData.SETItems != null;
			importToolStripMenuItem.Enabled = LevelData.geo != null;
			exportOBJToolStripMenuItem.Enabled = LevelData.geo != null;
			statsToolStripMenuItem.Enabled = LevelData.geo != null;
			deathZonesToolStripMenuItem.Enabled = deathZoneToolStripMenuItem.Enabled = LevelData.DeathZones != null;
			if (LevelData.DeathZones == null)
				deathZonesToolStripMenuItem.Checked = false;
			loaded = true;
			SelectedItems = new List<Item>();
			UseWaitCursor = false;
			Enabled = true;

			gizmoSpaceComboBox.Enabled = true;
			gizmoSpaceComboBox.SelectedIndex = 0;

			DrawLevel();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (loaded)
			{
				switch (MessageBox.Show(this, "Do you want to save?", "SADXLVL2", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
				{
					case DialogResult.Yes:
						saveToolStripMenuItem_Click(this, EventArgs.Empty);
						break;
					case DialogResult.Cancel:
						e.Cancel = true;
						break;
				}

				LevelData.StateChanged -= LevelData_StateChanged;
			}
			Settings.Save();
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!loaded) return;

			Dictionary<string, string> group = ini[levelID];
			string syspath = Path.Combine(Environment.CurrentDirectory, ini[string.Empty]["syspath"]);
			string modpath = null;
			if (ini[string.Empty].ContainsKey("modpath"))
				modpath = ini[string.Empty]["modpath"];
			SA1LevelAct levelact = new SA1LevelAct(group.GetValueOrDefault("LevelID", "0000"));
			if (LevelData.geo != null)
			{
				LevelData.geo.Tool = "SADXLVL2";
				LevelData.geo.SaveToFile(group["LevelGeo"], LandTableFormat.SA1);
			}
			for (int i = 0; i < LevelData.StartPositions.Length; i++)
			{
				Dictionary<SA1LevelAct, SA1StartPosInfo> posini = SA1StartPosList.Load(ini[string.Empty][LevelData.Characters[i] + "start"]);
				if (posini.ContainsKey(levelact))
					posini.Remove(levelact);
				if (LevelData.StartPositions[i].Position.X != 0 | LevelData.StartPositions[i].Position.Y != 0 | LevelData.StartPositions[i].Position.Z != 0 | LevelData.StartPositions[i].Rotation.Y != 0)
				{
					posini.Add(levelact,
						new SA1StartPosInfo()
						{
							Position = LevelData.StartPositions[i].Position.ToSA_Tools(),
							YRotation = LevelData.StartPositions[i].Rotation.Y
						});
				}
				posini.Save(ini[string.Empty][LevelData.Characters[i] + "start"]);
			}
			if (LevelData.DeathZones != null)
			{
				DeathZoneFlags[] dzini = new DeathZoneFlags[LevelData.DeathZones.Count];
				string path = Path.GetDirectoryName(group["DeathZones"]);
				for (int i = 0; i < LevelData.DeathZones.Count; i++)
					dzini[i] = LevelData.DeathZones[i].Save(path, i);
				dzini.Save(group["DeathZones"]);
			}

			#region Saving SET Items
			if (LevelData.SETItems != null)
			{
				for (int i = 0; i < LevelData.SETItems.Length; i++)
				{
					string setstr = Path.Combine(syspath, "SET" + LevelData.SETName + LevelData.SETChars[i] + ".bin");
					if (modpath != null)
						setstr = Path.Combine(modpath, setstr);

					if (File.Exists(setstr))
						File.Delete(setstr);
					if (LevelData.SETItems[i].Count == 0)
						continue;
					List<byte> file = new List<byte>(LevelData.SETItems[i].Count * 0x20 + 0x20);
					file.AddRange(BitConverter.GetBytes(LevelData.SETItems[i].Count));
					file.Align(0x20);
					foreach (SETItem item in LevelData.SETItems[i])
						file.AddRange(item.GetBytes());
					File.WriteAllBytes(setstr, file.ToArray());
				}
			}
			#endregion

			#region Saving CAM Items
			if (LevelData.CAMItems != null)
			{
				for (int i = 0; i < LevelData.CAMItems.Length; i++)
				{
					string camString = Path.Combine(syspath, "CAM" + LevelData.SETName + LevelData.SETChars[i] + ".bin");
					if (modpath != null)
						camString = Path.Combine(modpath, camString);
					if (File.Exists(camString))
						File.Delete(camString);

					if (LevelData.CAMItems[i].Count == 0)
						continue;

					List<byte> file = new List<byte>(LevelData.CAMItems[i].Count * 0x40 + 0x40); // setting up file size and header
					file.AddRange(BitConverter.GetBytes(LevelData.CAMItems[i].Count));
					file.Align(0x40);


					foreach (CAMItem item in LevelData.CAMItems[i]) // outputting individual components
						file.AddRange(item.GetBytes());

					File.WriteAllBytes(camString, file.ToArray());
				}
			}
			#endregion
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		internal void DrawLevel()
		{
			if (!loaded) return;
			cam.FOV = (float)(Math.PI / 4);
			cam.Aspect = panel1.Width / (float)panel1.Height;
			cam.DrawDistance = 100000;
			d3ddevice.SetTransform(TransformType.Projection, Matrix.PerspectiveFovRH(cam.FOV, cam.Aspect, 1, cam.DrawDistance));
			d3ddevice.SetTransform(TransformType.View, cam.ToMatrix());
			Text = "SADXLVL2 - " + levelName + " (" + cam.Position.X + ", " + cam.Position.Y + ", " + cam.Position.Z + " Pitch=" + cam.Pitch.ToString("X") + " Yaw=" + cam.Yaw.ToString("X") + " Speed=" + cam.MoveSpeed + (cam.mode == 1 ? " Distance=" + cam.Distance : "") + ")";
			d3ddevice.SetRenderState(RenderStates.FillMode, (int)EditorOptions.RenderFillMode);
			d3ddevice.SetRenderState(RenderStates.CullMode, (int)EditorOptions.RenderCullMode);
			d3ddevice.Material = new Microsoft.DirectX.Direct3D.Material { Ambient = Color.White };
			d3ddevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black.ToArgb(), 1, 0);
			d3ddevice.RenderState.ZBufferEnable = true;
			d3ddevice.BeginScene();
			//all drawings after this line
			EditorOptions.RenderStateCommonSetup(d3ddevice);

			MatrixStack transform = new MatrixStack();
			if (LevelData.leveleff != null & backgroundToolStripMenuItem.Checked)
				LevelData.leveleff.Render(d3ddevice, cam);

			cam.DrawDistance = EditorOptions.RenderDrawDistance;
			d3ddevice.SetTransform(TransformType.Projection, Matrix.PerspectiveFovRH(cam.FOV, cam.Aspect, 1, cam.DrawDistance));
			d3ddevice.SetTransform(TransformType.View, cam.ToMatrix());
			cam.BuildFrustum(d3ddevice.Transform.View, d3ddevice.Transform.Projection);

			List<RenderInfo> renderlist = new List<RenderInfo>();

			#region Adding Level Geometry
			if (LevelData.LevelItems != null)
			{
				for (int i = 0; i < LevelData.LevelItems.Count; i++)
				{
					bool display = false;
					if (visibleToolStripMenuItem.Checked && LevelData.LevelItems[i].Visible)
						display = true;
					else if (invisibleToolStripMenuItem.Checked && !LevelData.LevelItems[i].Visible)
						display = true;
					else if (allToolStripMenuItem.Checked)
						display = true;
					if (display)
					{
						renderlist.AddRange(LevelData.LevelItems[i].Render(d3ddevice, cam, transform, SelectedItems.Contains(LevelData.LevelItems[i])));
					}
				}
			}
			renderlist.AddRange(LevelData.StartPositions[LevelData.Character].Render(d3ddevice, cam, transform, SelectedItems.Contains(LevelData.StartPositions[LevelData.Character])));
			#endregion

			#region Adding SET Layout
			if (LevelData.SETItems != null && sETITemsToolStripMenuItem.Checked)
			{
				foreach (SETItem item in LevelData.SETItems[LevelData.Character])
				{
					renderlist.AddRange(item.Render(d3ddevice, cam, transform, SelectedItems.Contains(item)));
				}
			}
			#endregion

			#region Adding Death Zones
			if (LevelData.DeathZones != null & deathZonesToolStripMenuItem.Checked)
				foreach (DeathZoneItem item in LevelData.DeathZones)
					if (item.Visible)
						renderlist.AddRange(item.Render(d3ddevice, cam, transform, SelectedItems.Contains(item)));
			#endregion

			#region Adding CAM Layout
			if (LevelData.CAMItems != null && cAMItemsToolStripMenuItem.Checked)
			{
				foreach (CAMItem item in LevelData.CAMItems[LevelData.Character]) renderlist.AddRange(item.Render(d3ddevice, cam, transform, SelectedItems.Contains(item)));
			}
			#endregion

			RenderInfo.Draw(renderlist, d3ddevice, cam);

			if (splinesToolStripMenuItem.Checked)
			{
				foreach (SplineData spline in LevelData.LevelSplines)
				{
					spline.Draw(d3ddevice);
				}
			}

			d3ddevice.EndScene(); // scene drawings go before this line
			d3ddevice.Present();

			// draw helper cubes before clearing depth buffer
			cameraPointA.DrawBox(d3ddevice, cam);
			cameraPointB.DrawBox(d3ddevice, cam);

			transformGizmo.Draw(d3ddevice, cam);
			cameraPointA.Draw(d3ddevice, cam);
			cameraPointB.Draw(d3ddevice, cam);
		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{
			DrawLevel();
		}

		#region User Keyboard / Mouse Methods
		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.O:
					if (e.Control)
						openToolStripMenuItem_Click(sender, EventArgs.Empty);
					break;
				case Keys.S:
					if (!loaded) return;
					if (e.Control)
						saveToolStripMenuItem_Click(sender, EventArgs.Empty);
					break;
			}
		}

		private void panel1_MouseDown(object sender, MouseEventArgs e)
		{
			if (!loaded) return;

			switch (e.Button)
			{
				case MouseButtons.Left:
					float mindist = cam.DrawDistance; // initialize to max distance, because it will get smaller on each check
					HitResult dist;
					Item item = null;
					Vector3 mousepos = new Vector3(e.X, e.Y, 0);
					Viewport viewport = d3ddevice.Viewport;
					Matrix proj = d3ddevice.Transform.Projection;
					Matrix view = d3ddevice.Transform.View;
					Vector3 Near, Far;
					Near = mousepos;
					Near.Z = 0;
					Far = Near;
					Far.Z = -1;

					if (cameraPointA.SelectedAxes != GizmoSelectedAxes.NONE) return;
					if (cameraPointB.SelectedAxes != GizmoSelectedAxes.NONE) return;
					if (transformGizmo.SelectedAxes != GizmoSelectedAxes.NONE) return;

					#region Picking Level Items
					if (LevelData.LevelItems != null)
					{
						for (int i = 0; i < LevelData.LevelItems.Count; i++)
						{
							bool display = false;
							if (visibleToolStripMenuItem.Checked && LevelData.LevelItems[i].Visible)
								display = true;
							else if (invisibleToolStripMenuItem.Checked && !LevelData.LevelItems[i].Visible)
								display = true;
							else if (allToolStripMenuItem.Checked)
								display = true;
							if (display)
							{
								dist = LevelData.LevelItems[i].CheckHit(Near, Far, viewport, proj, view);
								if (dist.IsHit & dist.Distance < mindist)
								{
									mindist = dist.Distance;
									item = LevelData.LevelItems[i];
								}
							}
						}
					}
					#endregion

					#region Picking Start Positions
					dist = LevelData.StartPositions[LevelData.Character].CheckHit(Near, Far, viewport, proj, view);
					if (dist.IsHit & dist.Distance < mindist)
					{
						mindist = dist.Distance;
						item = LevelData.StartPositions[LevelData.Character];
					}
					#endregion

					#region Picking SET Items
					if (LevelData.SETItems != null && sETITemsToolStripMenuItem.Checked)
						foreach (SETItem setitem in LevelData.SETItems[LevelData.Character])
						{
							dist = setitem.CheckHit(Near, Far, viewport, proj, view);
							if (dist.IsHit & dist.Distance < mindist)
							{
								mindist = dist.Distance;
								item = setitem;
							}
						}
					#endregion

					#region Picking CAM Items
					if ((LevelData.CAMItems != null) && (cAMItemsToolStripMenuItem.Checked))
					{
						foreach (CAMItem camItem in LevelData.CAMItems[LevelData.Character])
						{
							dist = camItem.CheckHit(Near, Far, viewport, proj, view);
							if (dist.IsHit & dist.Distance < mindist)
							{
								mindist = dist.Distance;
								item = camItem;
							}
						}
					}
					#endregion

					#region Picking Death Zones
					if (LevelData.DeathZones != null)
						foreach (DeathZoneItem dzitem in LevelData.DeathZones)
							if (dzitem.Visible & deathZonesToolStripMenuItem.Checked)
							{
								dist = dzitem.CheckHit(Near, Far, viewport, proj, view);
								if (dist.IsHit & dist.Distance < mindist)
								{
									mindist = dist.Distance;
									item = dzitem;
								}
							}
					#endregion

					if (item != null)
					{
						if (ModifierKeys == Keys.Control)
						{
							if (SelectedItems.Contains(item))
								SelectedItems.Remove(item);
							else
								SelectedItems.Add(item);
						}
						else if (!SelectedItems.Contains(item))
						{
							SelectedItems.Clear();
							SelectedItems.Add(item);
						}
					}
					else if ((ModifierKeys & Keys.Control) == 0)
					{
						SelectedItems.Clear();
					}
					break;

				case MouseButtons.Right:
					bool cancopy = false;
					foreach (Item obj in SelectedItems)
						if (obj.CanCopy)
							cancopy = true;
					if (cancopy)
					{
						/*cutToolStripMenuItem.Enabled = true;
						copyToolStripMenuItem.Enabled = true;*/
						deleteToolStripMenuItem.Enabled = true;

						cutToolStripMenuItem.Enabled = false;
						copyToolStripMenuItem.Enabled = false;
					}
					else
					{
						cutToolStripMenuItem.Enabled = false;
						copyToolStripMenuItem.Enabled = false;
						deleteToolStripMenuItem.Enabled = false;
					}
					pasteToolStripMenuItem.Enabled = false;
					contextMenuStrip1.Show(panel1, e.Location);
					break;
			}
			SelectedItemChanged();
			DrawLevel();
		}

		private void panel1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Down:
				case Keys.Left:
				case Keys.Right:
				case Keys.Up:
					e.IsInputKey = true;
					break;
			}
		}

		private void panel1_KeyUp(object sender, KeyEventArgs e)
		{
			if (!e.Alt) lookKeyDown = false;
			if (!e.Control) zoomKeyDown = false;
		}

		private void panel1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (!loaded) return;
			if (cam.mode == 0)
			{
				if (e.KeyCode == Keys.E)
				{
					cam.Position = new Vector3();
					DrawLevel();
				}

				if (e.KeyCode == Keys.R)
				{
					cam.Pitch = 0;
					cam.Yaw = 0;
					DrawLevel();
				}
			}

			if (e.Alt) { lookKeyDown = true; if (panel1.ContainsFocus) e.Handled = false; }
			if (e.Control) zoomKeyDown = true;

			if (e.KeyCode == Keys.X)
			{
				cam.mode = (cam.mode + 1) % 2;

				if (cam.mode == 1)
				{
					if (SelectedItems.Count > 0) cam.FocalPoint = Item.CenterFromSelection(SelectedItems).ToVector3();
					else
					{
						cam.FocalPoint = cam.Position += cam.Look * cam.Distance;
					}
				}

				DrawLevel();
			}
			if (e.KeyCode == Keys.N)
			{
				if (EditorOptions.RenderFillMode == FillMode.Solid)
					EditorOptions.RenderFillMode = FillMode.Point;
				else
					EditorOptions.RenderFillMode += 1;

				DrawLevel();
			}
			if (e.KeyCode == Keys.Delete)
			{
				foreach (Item item in SelectedItems)
					item.Delete();
				SelectedItems.Clear();
				SelectedItemChanged();
				DrawLevel();
			}
		}

		Point lastmouse;
		private void Panel1_MouseMove(object sender, MouseEventArgs e)
		{
			if (!loaded) return;
			Point evloc = e.Location;
			if (lastmouse == Point.Empty)
			{
				lastmouse = evloc;
				return;
			}
			Point chg = evloc - (Size)lastmouse;
			if (e.Button == System.Windows.Forms.MouseButtons.Middle)
			{
				// all cam controls are now bound to the middle mouse button
				if (cam.mode == 0)
				{
					if (zoomKeyDown)
					{
						cam.Position += cam.Look * (chg.Y * cam.MoveSpeed);
					}
					else if (lookKeyDown)
					{
						cam.Yaw = unchecked((ushort)(cam.Yaw - chg.X * 0x10));
						cam.Pitch = unchecked((ushort)(cam.Pitch - chg.Y * 0x10));
					}
					else if (!lookKeyDown && !zoomKeyDown) // pan
					{
						cam.Position += cam.Up * (chg.Y * cam.MoveSpeed);
						cam.Position += cam.Right * (chg.X * cam.MoveSpeed) * -1;
					}
				}
				else if (cam.mode == 1)
				{
					if (zoomKeyDown)
					{
						cam.Distance += (chg.Y * cam.MoveSpeed) * 3;
					}
					else if (lookKeyDown)
					{
						cam.Yaw = unchecked((ushort)(cam.Yaw - chg.X * 0x10));
						cam.Pitch = unchecked((ushort)(cam.Pitch - chg.Y * 0x10));
					}
					else if (!lookKeyDown && !zoomKeyDown) // pan
					{
						cam.FocalPoint += cam.Up * (chg.Y * cam.MoveSpeed);
						cam.FocalPoint += cam.Right * (chg.X * cam.MoveSpeed) * -1;
					}
				}

				DrawLevel();
			}
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				cameraPointA.TransformAffected(chg.X / 2, chg.Y / 2);
				cameraPointB.TransformAffected(chg.X / 2, chg.Y / 2);
				transformGizmo.TransformAffected(chg.X / 2, chg.Y / 2);
				DrawLevel();

				Rectangle scrbnds = Screen.GetBounds(Cursor.Position);
				if (Cursor.Position.X == scrbnds.Left)
				{
					Cursor.Position = new Point(scrbnds.Right - 2, Cursor.Position.Y);
					evloc = new Point(evloc.X + scrbnds.Width - 2, evloc.Y);
				}
				else if (Cursor.Position.X == scrbnds.Right - 1)
				{
					Cursor.Position = new Point(scrbnds.Left + 1, Cursor.Position.Y);
					evloc = new Point(evloc.X - scrbnds.Width + 1, evloc.Y);
				}
				if (Cursor.Position.Y == scrbnds.Top)
				{
					Cursor.Position = new Point(Cursor.Position.X, scrbnds.Bottom - 2);
					evloc = new Point(evloc.X, evloc.Y + scrbnds.Height - 2);
				}
				else if (Cursor.Position.Y == scrbnds.Bottom - 1)
				{
					Cursor.Position = new Point(Cursor.Position.X, scrbnds.Top + 1);
					evloc = new Point(evloc.X, evloc.Y - scrbnds.Height + 1);
				}
			}
			else if (e.Button == System.Windows.Forms.MouseButtons.None)
			{
				float mindist = cam.DrawDistance; // initialize to max distance, because it will get smaller on each check
				Vector3 mousepos = new Vector3(e.X, e.Y, 0);
				Viewport viewport = d3ddevice.Viewport;
				Matrix proj = d3ddevice.Transform.Projection;
				Matrix view = d3ddevice.Transform.View;
				Vector3 Near, Far;
				Near = mousepos;
				Near.Z = 0;
				Far = Near;
				Far.Z = -1;

				GizmoSelectedAxes oldSelection = transformGizmo.SelectedAxes;
				transformGizmo.SelectedAxes = transformGizmo.CheckHit(Near, Far, viewport, proj, view, cam);
				if (oldSelection != transformGizmo.SelectedAxes) { transformGizmo.Draw(d3ddevice, cam); goto done_processing; }

				GizmoSelectedAxes oldCamA = cameraPointA.SelectedAxes;
				cameraPointA.SelectedAxes = cameraPointA.CheckHit(Near, Far, viewport, proj, view, cam);
				if (oldCamA != cameraPointA.SelectedAxes) { cameraPointA.Draw(d3ddevice, cam); goto done_processing; }

				if (cameraPointA.SelectedAxes == GizmoSelectedAxes.NONE)
				{
					GizmoSelectedAxes oldCamB = cameraPointB.SelectedAxes;
					cameraPointB.SelectedAxes = cameraPointB.CheckHit(Near, Far, viewport, proj, view, cam);
					if (oldCamB != cameraPointB.SelectedAxes) { cameraPointB.Draw(d3ddevice, cam); goto done_processing; }
				}
			}

		done_processing:
			lastmouse = evloc;
		}

		void panel1_MouseWheel(object sender, MouseEventArgs e)
		{
			if (!loaded) return;
			if (!panel1.Focused) return;

			float detentValue = -1;

			if (e.Delta < 0) detentValue = 1;

			if (cam.mode == 0) cam.Position += cam.Look * (detentValue * cam.MoveSpeed);
			else if (cam.mode == 1) cam.Distance += (detentValue * cam.MoveSpeed);
			DrawLevel();
		}
		#endregion

		internal void SelectedItemChanged()
		{
			propertyGrid1.SelectedObjects = SelectedItems.ToArray();

			if (cam.mode == 1)
			{
				cam.FocalPoint = Item.CenterFromSelection(SelectedItems).ToVector3();
			}

			if (SelectedItems.Count > 0) // set up gizmo
			{
				transformGizmo.AffectedItems = SelectedItems;
				if (!transformGizmo.Enabled) { transformGizmo.Enabled = true; DrawLevel(); }

				if (SelectedItems.Count == 1) // single-select only cases
				{
					if (SelectedItems[0] is CAMItem)
					{
						CAMItem camItem = (CAMItem)SelectedItems[0];
						cameraPointA.SetPoint(camItem.PointA);
						cameraPointB.SetPoint(camItem.PointB);
					}
					else
					{
						cameraPointA.Enabled = false;
						cameraPointB.Enabled = false;
					}
				}
			}
			else
			{
				if (transformGizmo != null)
				{
					transformGizmo.AffectedItems = new List<Item>();
					transformGizmo.Enabled = false;
					DrawLevel();
				}

				if ((cameraPointA != null) && (cameraPointB != null))
				{
					cameraPointA.Enabled = false;
					cameraPointB.Enabled = false;
				}
			}
		}

		private void cutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<Item> selitems = new List<Item>();
			foreach (Item item in SelectedItems)
				if (item.CanCopy)
				{
					item.Delete();
					selitems.Add(item);
				}
			SelectedItems.Clear();
			SelectedItemChanged();
			DrawLevel();
			if (selitems.Count == 0) return;
			Clipboard.SetData(DataFormats.Serializable, selitems);
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<Item> selitems = new List<Item>();
			foreach (Item item in SelectedItems)
				if (item.CanCopy)
					selitems.Add(item);
			if (selitems.Count == 0) return;
			Clipboard.SetData("SADXLVLObjectList", selitems);
		}

		private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			object obj = Clipboard.GetData("SADXLVLObjectList");

			if (obj == null)
			{
				MessageBox.Show("Paste operation failed - this is a known issue and is being worked on.");
				return; // todo: finish implementing proper copy/paste
			}
			List<Item> objs = (List<Item>)obj;
			Vector3 center = new Vector3();
			foreach (Item item in objs)
				center.Add(item.Position.ToVector3());
			center = new Vector3(center.X / objs.Count, center.Y / objs.Count, center.Z / objs.Count);
			foreach (Item item in objs)
			{
				item.Position = new Vertex(item.Position.X - center.X + cam.Position.X, item.Position.Y - center.Y + cam.Position.Y, item.Position.Z - center.Z + cam.Position.Z);
				item.Paste();
			}
			SelectedItems = new List<Item>(objs);
			SelectedItemChanged();
			DrawLevel();
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Item item in SelectedItems)
				if (item.CanCopy)
					item.Delete();
			SelectedItems.Clear();
			SelectedItemChanged();
			DrawLevel();
		}

		private void characterToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			LevelData.Character = characterToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem);

			// Character view buttons
			toolSonic.Checked = false;
			toolTails.Checked = false;
			toolKnuckles.Checked = false;
			toolAmy.Checked = false;
			toolBig.Checked = false;
			toolGamma.Checked = false;

			foreach (ToolStripMenuItem item in characterToolStripMenuItem.DropDownItems)
				item.Checked = false;

			((ToolStripMenuItem)e.ClickedItem).Checked = true;

			switch (LevelData.Character)
			{
				default:
					toolSonic.Checked = true;
					break;

				case 1:
					toolTails.Checked = true;
					break;

				case 2:
					toolKnuckles.Checked = true;
					break;

				case 3:
					toolAmy.Checked = true;
					break;

				case 4:
					toolGamma.Checked = true;
					break;

				case 5:
					toolBig.Checked = true;
					break;
			}

			transformGizmo.Enabled = false;

			if (transformGizmo.AffectedItems != null)
				transformGizmo.AffectedItems.Clear();

			DrawLevel();
		}

		private void onClickCharacterButton(object sender, EventArgs e)
		{
			if (sender == toolTails)
				tailsToolStripMenuItem.PerformClick();
			else if (sender == toolKnuckles)
				knucklesToolStripMenuItem.PerformClick();
			else if (sender == toolAmy)
				amyToolStripMenuItem.PerformClick();
			else if (sender == toolBig)
				bigToolStripMenuItem.PerformClick();
			else if (sender == toolGamma)
				gammaToolStripMenuItem.PerformClick();
			else
				sonicToolStripMenuItem.PerformClick();
		}

		private void levelToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			foreach (ToolStripMenuItem item in levelToolStripMenuItem.DropDownItems)
				item.Checked = false;
			((ToolStripMenuItem)e.ClickedItem).Checked = true;
			transformGizmo.Enabled = false;
			transformGizmo.AffectedItems.Clear();
			DrawLevel();
		}

		private void levelPieceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (importFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				string filePath = importFileDialog.FileName;

				bool errorFlag = false;
				string errorMsg = "";

				LevelData.ImportFromFile(filePath, d3ddevice, cam, out errorFlag, out errorMsg);

				if (errorFlag)
				{
					MessageBox.Show(errorMsg);
				}
			}
		}

		private void importToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (importFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				string filePath = importFileDialog.FileName;
				DialogResult userClearLevelResult = MessageBox.Show("Do you want to clear the level models first?", "Clear Level?", MessageBoxButtons.YesNoCancel);

				if (userClearLevelResult == System.Windows.Forms.DialogResult.Cancel) return;
				else if (userClearLevelResult == DialogResult.Yes)
				{
					DialogResult clearAnimsResult = MessageBox.Show("Do you also want to clear any animated level models?", "Clear anims too?", MessageBoxButtons.YesNo);

					LevelData.ClearLevelGeometry();

					if (clearAnimsResult == System.Windows.Forms.DialogResult.Yes)
					{
						LevelData.ClearLevelGeoAnims();
					}
				}

				bool errorFlag = false;
				string errorMsg = "";

				LevelData.ImportFromFile(filePath, d3ddevice, cam, out errorFlag, out errorMsg);

				if (errorFlag)
				{
					MessageBox.Show(errorMsg);
				}
			}
		}

		private void objectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SETItem item = new SETItem();
			Vector3 pos = cam.Position + (-20 * cam.Look);
			item.Position = new Vertex(pos.X, pos.Y, pos.Z);
			LevelData.SETItems[LevelData.Character].Add(item);
			SelectedItems = new List<Item>() { item };
			SelectedItemChanged();
			DrawLevel();
		}

		private void cameraToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Vector3 pos = cam.Position + (-20 * cam.Look);
			CAMItem item = new CAMItem(new Vertex(pos.X, pos.Y, pos.Z));
			LevelData.CAMItems[LevelData.Character].Add(item);
			SelectedItems = new List<Item>() { item };
			SelectedItemChanged();
			DrawLevel();
		}

		private void exportOBJToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveFileDialog a = new SaveFileDialog
			{
				DefaultExt = "obj",
				Filter = "OBJ Files|*.obj"
			};
			if (a.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				using (StreamWriter objstream = new StreamWriter(a.FileName, false))
				using (StreamWriter mtlstream = new StreamWriter(Path.ChangeExtension(a.FileName, "mtl"), false))
				{
					#region Material Exporting
					string materialPrefix = LevelData.leveltexs;

					objstream.WriteLine("mtllib " + Path.GetFileNameWithoutExtension(a.FileName) + ".mtl");

					// This is admittedly not an accurate representation of the materials used in the model - HOWEVER, it makes the materials more managable in MAX
					// So we're doing it this way. In the future we should come back and add an option to do it this way or the original way.
					for (int texIndx = 0; texIndx < LevelData.TextureBitmaps[LevelData.leveltexs].Length; texIndx++)
					{
						mtlstream.WriteLine(String.Format("newmtl {0}_material_{1}", materialPrefix, texIndx));
						mtlstream.WriteLine("Ka 1 1 1");
						mtlstream.WriteLine("Kd 1 1 1");
						mtlstream.WriteLine("Ks 0 0 0");
						mtlstream.WriteLine("illum 1");

						if (!string.IsNullOrEmpty(LevelData.leveltexs))
						{
							mtlstream.WriteLine("Map_Kd " + LevelData.TextureBitmaps[LevelData.leveltexs][texIndx].Name + ".png");

							// save texture
							string mypath = System.IO.Path.GetDirectoryName(a.FileName);
							BMPInfo item = LevelData.TextureBitmaps[LevelData.leveltexs][texIndx];
							item.Image.Save(Path.Combine(mypath, item.Name + ".png"));
						}
					}
					#endregion

					int totalVerts = 0;
					int totalNorms = 0;
					int totalUVs = 0;

					bool errorFlag = false;

					for (int i = 0; i < LevelData.geo.COL.Count; i++)
						SAModel.Direct3D.Extensions.WriteModelAsObj(objstream, LevelData.geo.COL[i].Model, materialPrefix, new MatrixStack(), ref totalVerts, ref totalNorms, ref totalUVs, ref errorFlag);
					if (LevelData.geo.Anim != null)
						for (int i = 0; i < LevelData.geo.Anim.Count; i++)
							SAModel.Direct3D.Extensions.WriteModelAsObj(objstream, LevelData.geo.Anim[i].Model, materialPrefix, new MatrixStack(), ref totalVerts, ref totalNorms, ref totalUVs, ref errorFlag);

					if (errorFlag) MessageBox.Show("Error(s) encountered during export. Inspect the output file for more details.");
				}
			}
		}

		private void deathZoneToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DeathZoneItem item = new DeathZoneItem(d3ddevice);
			Vector3 pos = cam.Position + (-20 * cam.Look);
			item.Position = new Vertex(pos.X, pos.Y, pos.Z);
			switch (LevelData.Character)
			{
				case 0:
					item.Sonic = true;
					break;
				case 1:
					item.Tails = true;
					break;
				case 2:
					item.Knuckles = true;
					break;
				case 3:
					item.Amy = true;
					break;
				case 4:
					item.Gamma = true;
					break;
				case 5:
					item.Big = true;
					break;
			}
			SelectedItems = new List<Item>() { item };
			SelectedItemChanged();
			DrawLevel();
		}

		private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			DrawLevel();
		}

		private void backgroundToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DrawLevel();
		}

		private void recentProjectsToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			LoadINI(Settings.MRUList[recentProjectsToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem)]);
		}

		private void reportBugToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (BugReportDialog dlg = new BugReportDialog())
				dlg.ShowDialog(this);
		}

		void LevelData_StateChanged()
		{
			if (transformGizmo != null) transformGizmo.AffectedItems = SelectedItems;
			SelectedItemChanged();
			DrawLevel();
		}

		private void clearLevelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult clearAnimsResult = MessageBox.Show("Do you also want to clear any animated level models?", "Clear anims too?", MessageBoxButtons.YesNoCancel);

			if (clearAnimsResult == System.Windows.Forms.DialogResult.Cancel) return;

			LevelData.ClearLevelGeometry();

			if (clearAnimsResult == System.Windows.Forms.DialogResult.Yes)
			{
				LevelData.ClearLevelGeoAnims();
			}
		}

		private void statsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show(LevelData.GetStats());
		}

		private void sETITemsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			DrawLevel();
		}

		private void cAMItemsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			DrawLevel();
		}
		private void deathZonesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DrawLevel();
		}

		private void findReplaceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SETFindReplace findReplaceForm = new SETFindReplace();

			DialogResult findReplaceResult = findReplaceForm.ShowDialog();

			if (findReplaceResult == System.Windows.Forms.DialogResult.OK)
			{
				SelectedItemChanged();
				DrawLevel();
			}
		}

		private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool errorFlag = false;
			string errorMsg = "";
			LevelData.DuplicateSelection(d3ddevice, ref SelectedItems, out errorFlag, out errorMsg);

			if (errorFlag) MessageBox.Show(errorMsg);
		}

		private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			EditorOptionsEditor optionsEditor = new EditorOptionsEditor(cam);
			optionsEditor.FormUpdated += new EditorOptionsEditor.FormUpdatedHandler(optionsEditor_FormUpdated);
			optionsEditor.Show();
		}

		void optionsEditor_FormUpdated()
		{
			DrawLevel();
		}

		#region Gizmo Button Event Methods
		private void selectModeButton_Click(object sender, EventArgs e)
		{
			if (transformGizmo != null)
			{
				transformGizmo.Mode = TransformMode.NONE;
				gizmoSpaceComboBox.Enabled = true;
				moveModeButton.Checked = false;
				rotateModeButton.Checked = false;
				DrawLevel(); // possibly find a better way of doing this than re-drawing the entire scene? Possibly keep a copy of the last render w/o gizmo in memory?
			}
		}

		private void moveModeButton_Click(object sender, EventArgs e)
		{
			if (transformGizmo != null)
			{
				transformGizmo.Mode = TransformMode.TRANFORM_MOVE;
				gizmoSpaceComboBox.Enabled = true;
				selectModeButton.Checked = false;
				rotateModeButton.Checked = false;
				scaleModeButton.Checked = false;
				DrawLevel();
			}
		}

		private void rotateModeButton_Click(object sender, EventArgs e)
		{
			if (transformGizmo != null)
			{
				transformGizmo.Mode = TransformMode.TRANSFORM_ROTATE;
				transformGizmo.LocalTransform = true;
				gizmoSpaceComboBox.SelectedIndex = 1;
				gizmoSpaceComboBox.Enabled = false;
				selectModeButton.Checked = false;
				moveModeButton.Checked = false;
				scaleModeButton.Checked = false;
				DrawLevel();
			}
		}

		private void gizmoSpaceComboBox_DropDownClosed(object sender, EventArgs e)
		{
			if (transformGizmo != null)
			{
				transformGizmo.LocalTransform = (gizmoSpaceComboBox.SelectedIndex == 0) ? false : true;
				DrawLevel();
			}
		}

		private void scaleModeButton_Click(object sender, EventArgs e)
		{
			if (transformGizmo != null)
			{
				transformGizmo.Mode = TransformMode.TRANSFORM_SCALE;
				transformGizmo.LocalTransform = true;
				gizmoSpaceComboBox.SelectedIndex = 1;
				gizmoSpaceComboBox.Enabled = false;
				selectModeButton.Checked = false;
				moveModeButton.Checked = false;
				DrawLevel();
			}
		}
		#endregion

		private void duplicateToToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if ((SelectedItems == null) || (SelectedItems.Count == 0))
			{
				MessageBox.Show("To use this feature you must have a selection!");
				return;
			}

			DuplicateTo duplicateToWindow = new DuplicateTo(SelectedItems);
			duplicateToWindow.ShowDialog();
		}

		private void deleteAllOfTypeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			transformGizmo.Enabled = false;
			transformGizmo.AffectedItems = null; // just in case we wind up deleting our selection

			SETDeleteType typeDeleter = new SETDeleteType();

			typeDeleter.ShowDialog();
		}
	}
}