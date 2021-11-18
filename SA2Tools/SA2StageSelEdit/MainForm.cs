using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using ArchiveLib;
using SAModel.Direct3D.TextureSystem;
using SplitTools;
using Pfim;
using System.Linq;
using SAModel.SAEditorCommon.ProjectManagement;

namespace SA2StageSelEdit
{
	public partial class MainForm : Form
	{
		static readonly int chaoicon = 6;
		static readonly int questionicon = 9;
		static readonly int selecticon = 15;
		static readonly Dictionary<SA2Characters, int> charicons = new Dictionary<SA2Characters, int>() {
			{ SA2Characters.Sonic, 12 },
			{ SA2Characters.Shadow, 11 },
			{ SA2Characters.Tails, 13 },
			{ SA2Characters.Eggman, 7 },
			{ SA2Characters.Knuckles, 8 },
			{ SA2Characters.Rouge, 10 },
			{ SA2Characters.MechTails, 13 },
			{ SA2Characters.MechEggman, 7 }
		};

		public MainForm()
		{
			InitializeComponent();
		}

		string filename;
		List<StageSelectLevel> levels;
		Bitmap bgtex;
		Bitmap[] uitexs;
		int selected;

		private void MainForm_Load(object sender, EventArgs e)
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
			if (Program.sapFile != null)
				openFile(Program.sapFile);
		}

		void openFile(string projFileString)
		{
			if (projFileString != null)
			{
				Templates.ProjectTemplate projFile = ProjectFunctions.openProjectFileString(projFileString);
				if (projFile.GameInfo.GameName == "SA2PC")
				{
					string projFolder = projFile.GameInfo.ProjectFolder;
					string gameFolder = "";
					if (projFile.GameInfo.GameDataFolder != null)
						gameFolder = Path.Combine(ProjectFunctions.GetGamePath(projFile.GameInfo.GameName), projFile.GameInfo.GameDataFolder);
					else
						gameFolder = Path.Combine(ProjectFunctions.GetGamePath(projFile.GameInfo.GameName), "resource", "gd_PC");

					if (File.Exists(Path.Combine(projFolder, "ADVERTISE_data.ini")))
					{
						IniData ini = IniSerializer.Deserialize<IniData>(Path.Combine(projFolder, "ADVERTISE_data.ini"));
						filename = Path.Combine(projFolder, ini.Files.First((item) => item.Value.Type == "stageselectlist").Value.Filename);
						levels = new List<StageSelectLevel>(StageSelectLevelList.Load(filename));
						string socFilePath = "";
						string prsFilePath = "";

                        if (projFile.GameInfo.GameName == "SA2PC")
                        {
                            if (File.Exists(Path.Combine(projFolder, "gd_PC", "SOC", "stageMapBG.pak")))
                                socFilePath = Path.Combine(projFolder, "gd_PC", "SOC", "stageMapBG.pak");
                            else
                                socFilePath = Path.Combine(gameFolder, "SOC", "stageMapBG.pak");

                            if (File.Exists(Path.Combine(projFolder, "gd_PC", "PRS", "stageMap.pak")))
                                prsFilePath = Path.Combine(projFolder, "gd_PC", "PRS", "stageMap.pak");
                            else
                                prsFilePath = Path.Combine(gameFolder, "PRS", "stageMap.pak");
                        }
                        else
                        {
                            socFilePath = Path.Combine(gameFolder, "stageMapBG.prs");
                            prsFilePath = Path.Combine(gameFolder, "stageMap.prs");
                        }

						PAKFile socpak = new PAKFile(socFilePath);
						using (MemoryStream str = new MemoryStream(socpak.Entries.Find((a) => a.Name.Equals("stagemap.dds")).Data))
							bgtex = LoadDDS(str);

						if (File.Exists(prsFilePath))
						{
							PAKFile pakf = new PAKFile(prsFilePath);
							byte[] inf = pakf.Entries.Find((a) => a.Name.Equals("stagemap.inf")).Data;
							uitexs = new Bitmap[inf.Length / 0x3C];
							for (int i = 0; i < uitexs.Length; i++)
								using (MemoryStream str = new MemoryStream(pakf.Entries.Find(
									(a) => a.Name.Equals(Encoding.ASCII.GetString(inf, i * 0x3C, 0x1c).TrimEnd('\0') + ".dds")).Data))
									uitexs[i] = LoadDDS(str);
						}
						else
							uitexs = TextureArchive.GetTextures(prsFilePath).Select((tex) => tex.Image).ToArray();
						saveToolStripMenuItem.Enabled = panel1.Enabled = panel2.Enabled = true;
						selected = 0;
						level.SelectedIndex = (int)levels[selected].Level;
						character.SelectedIndex = (int)levels[selected].Character;
						column.Value = levels[selected].Column;
						row.Value = levels[selected].Row;
						DrawLevel();
					}
				}
				else
					MessageBox.Show("The opened project file is not ", "Incorrect File", MessageBoxButtons.OK);
			}
			Program.sapFile = "";
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog fd = new OpenFileDialog() { DefaultExt = "sap", Filter = "SA Project File|*.sap" };
			if (fd.ShowDialog(this) == DialogResult.OK)
				openFile(fd.FileName);
		}

		private bool CheckIfDDS(Stream input)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				input.CopyTo(ms);
				byte[] dds = ms.ToArray();
				uint check = BitConverter.ToUInt32(dds.ToArray(), 0);
				if (check == 0x20534444) // DDS header
					return true;
				else
					return false;
			}
		}

		private Bitmap LoadDDS(MemoryStream str)
		{
			// Check if the texture is DDS
			if (CheckIfDDS(str))
			{
				str.Seek(0, SeekOrigin.Begin);
				PixelFormat pxformat;
				var image = Pfim.Pfim.FromStream(str, new Pfim.PfimConfig());
				switch (image.Format)
				{
					case Pfim.ImageFormat.Rgba32:
						pxformat = PixelFormat.Format32bppArgb;
						break;
					case Pfim.ImageFormat.Rgb24:
						pxformat = PixelFormat.Format24bppRgb;
						break;
					case Pfim.ImageFormat.R5g5b5:
						pxformat = PixelFormat.Format16bppRgb555;
						break;
					case Pfim.ImageFormat.R5g5b5a1:
						pxformat = PixelFormat.Format16bppArgb1555;
						break;
					case Pfim.ImageFormat.R5g6b5:
						pxformat = PixelFormat.Format16bppRgb565;
						break;
					default:
						throw new Exception("Unsupported image format: " + image.Format.ToString());
				}
				var bitmap = new Bitmap(image.Width, image.Height, pxformat);
				BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, pxformat);
				System.Runtime.InteropServices.Marshal.Copy(image.Data, 0, bmpData.Scan0, image.DataLen);
				bitmap.UnlockBits(bmpData);
				return bitmap;
			}
			else // Not DDS
				return new Bitmap(str);
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			levels.ToArray().Save(filename);
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void panel1_Paint(object sender, PaintEventArgs e) { DrawLevel(); }

		void DrawLevel()
		{
			if (string.IsNullOrEmpty(filename)) return;
			using (Bitmap bmp = new Bitmap(bgtex))
			{
				using (Graphics gfx = Graphics.FromImage(bmp))
				{
					gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
					gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
					gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
					foreach (StageSelectLevel item in levels)
					{
						int tex = 0;
						switch (item.Level)
						{
							case SA2LevelIDs.GreenHill: // Green Hill
								tex = questionicon;
								break;
							case SA2LevelIDs.ChaoWorld: // Chao World
								tex = chaoicon;
								break;
							default:
								tex = charicons[item.Character];
								break;
						}
						gfx.DrawImage(uitexs[tex], item.Column * 60 + 110, item.Row * 60 + 10, 48, 48);
					}
					gfx.DrawImage(uitexs[selecticon], levels[selected].Column * 60 + 104, levels[selected].Row * 60 + 4, 96, 96);
				}
				using (Graphics pnlgfx = panel1.CreateGraphics())
				{
					pnlgfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
					pnlgfx.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
				}
			}
		}

		private void panel1_MouseMove(object sender, MouseEventArgs e)
		{
			if (string.IsNullOrEmpty(filename)) return;
			bool hit = false;
			foreach (StageSelectLevel item in levels)
				if (new Rectangle(item.Column * 60 + 110, item.Row * 60 + 10, 48, 48).Contains(e.Location))
					hit = true;
			panel1.Cursor = hit ? Cursors.Hand : Cursors.Default;
		}

		bool suppressEvents;

		private void panel1_MouseClick(object sender, MouseEventArgs e)
		{
			if (string.IsNullOrEmpty(filename)) return;
			foreach (StageSelectLevel item in levels)
				if (new Rectangle(item.Column * 60 + 110, item.Row * 60 + 10, 48, 48).Contains(e.Location))
				{
					selected = levels.IndexOf(item);
					suppressEvents = true;
					level.SelectedIndex = (int)item.Level;
					character.SelectedIndex = (int)item.Character;
					column.Value = item.Column;
					row.Value = item.Row;
					suppressEvents = false;
					DrawLevel();
					return;
				}
		}

		private void level_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (suppressEvents) return;
			levels[selected].Level = (SA2LevelIDs)level.SelectedIndex;
			DrawLevel();
		}

		private void character_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (suppressEvents) return;
			levels[selected].Character = (SA2Characters)character.SelectedIndex;
			DrawLevel();
		}

		private void column_ValueChanged(object sender, EventArgs e)
		{
			if (suppressEvents) return;
			levels[selected].Column = (int)column.Value;
			DrawLevel();
		}

		private void row_ValueChanged(object sender, EventArgs e)
		{
			if (suppressEvents) return;
			levels[selected].Row = (int)row.Value;
			DrawLevel();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (filename != null)
				switch (MessageBox.Show(this, "Do you want to save?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
				{
					case DialogResult.Cancel:
						e.Cancel = true;
						break;
					case DialogResult.Yes:
						saveToolStripMenuItem_Click(this, EventArgs.Empty);
						break;
				}
		}
	}
}