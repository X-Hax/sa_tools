using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using SplitTools;
using System.Drawing.Imaging;
using System.Text;
using System.ComponentModel;

namespace SAFontEdit
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}
		private List<FontItem> files;
		private readonly Color[] pal = new Color[] { Color.Black, Color.White };
		private string filename;
		bool argb;

		private void Form1_Load(object sender, EventArgs e)
		{
			string[] args = Environment.GetCommandLineArgs();
			if (args.Length == 1)
			{
				files = new List<FontItem>();
				filename = null;
			}
			else
			{
				LoadFile(args[1]);
			}
			characterTileControl1.Cursor = new Cursor(new System.IO.MemoryStream(Properties.Resources.pencilcur));
			characterTileControl1.Size = new Size(24 * (int)trackBar1.Value, 24 * (int)trackBar1.Value);
			characterTileControl1.Palette = pal;
			buttonExportCharacter.Enabled = buttonImportCharacter.Enabled = false;
		}

		private void LoadFile(string filename_a)
		{
			using (FileTypeDialog ftd = new FileTypeDialog(filename_a))
			{
				Encoding enc = Encoding.GetEncoding(1252);
				byte[] origtext;
				bool oldformat = true;
				int itemlength = 0x48;
				string utf16String = "";
				string customstring = "";
				ftd.ShowDialog();
				if (ftd.DialogResult == DialogResult.OK)
				{
					if (ftd.charmap)
						customstring = ftd.customstring;
					if (ftd.fontdata)
					{
						itemlength = 0x58;
						oldformat = false;
					}
					argb = ftd.argb;
					bool socansi = ftd.ansiTrimmed;
					if (argb)
						itemlength = 2304;
					filename = filename_a;
					byte[] file = System.IO.File.ReadAllBytes(filename);
					int count = file.Length / itemlength;
					//MessageBox.Show("Item count: " + count.ToString());
					files = new List<FontItem>(count);
					listBox1.Items.Clear();
					listBox1.BeginUpdate();
					for (int i = 0; i < count; i++)
					{
						//MessageBox.Show("Item at " + i * itemlength + " (" + i.ToString() + ")");
						files.Add(new FontItem(file, i * itemlength, oldformat, (ushort)i, argb));
						if (!ftd.charmap)
						{
							if (ftd.codepage == 50220)
							{
								origtext = new byte[8];
								origtext[0] = 0x1B;
								origtext[1] = 0x24;
								origtext[2] = 0x42;
								origtext[3] = (byte)(files[i].ID >> 8);
								origtext[4] = (byte)(files[i].ID >> 0);
								origtext[5] = 0x1B;
								origtext[6] = 0x28;
								origtext[7] = 0x42;
								utf16String = Encoding.GetEncoding(50220).GetString(origtext);
							}
							else if (ftd.codepage == 1200)
							{
								origtext = new byte[2];
								origtext[0] = (byte)(files[i].ID >> 0);
								origtext[1] = (byte)(files[i].ID >> 8);
								utf16String = Encoding.GetEncoding(1200).GetString(origtext);
							}
							else if (ftd.codepage == 1252)
							{
								origtext = new byte[2];
								ushort id_final = (ushort)(files[i].ID + (socansi ? 32 : 0));
								origtext[0] = (byte)(id_final >> 0);
								origtext[1] = (byte)(id_final >> 8);
								if (files[i].ID <= 255)
									utf16String = Encoding.GetEncoding(1252).GetString(origtext);
								else
									utf16String = " ";
							}
							else if (!oldformat)
							{
								origtext = new byte[1];
								origtext[0] = (byte)files[i].ID;
								utf16String = Encoding.GetEncoding(ftd.codepage).GetString(origtext);
							}
							else
							{
								// This is for EFMSGFONT files with custom codepages based on the original "trimmed 1252"
								origtext = new byte[2];
								ushort id_final = (ushort)(files[i].ID + (socansi ? 32 : 0));
								origtext[0] = (byte)(id_final >> 0);
								origtext[1] = (byte)(id_final >> 8);
								if (files[i].ID <= 255)
									utf16String = Encoding.GetEncoding(ftd.codepage).GetString(origtext);
								else
									utf16String = " ";
							}
						}
						else
						{
							if (i >= customstring.Length)
								utf16String = "　";
							else
								utf16String = customstring[i] + "";
						}
						files[i].character = utf16String;
						listBox1.Items.Add(files[i].ID.ToString("X4") + ' ' + utf16String);
					}
					listBox1.EndUpdate();
					listBox1.SelectedIndex = 0;
					extractAllToolStripMenuItem.Enabled = exportIndividualCharactersToolStripMenuItem.Enabled = saveAsToolStripMenuItem.Enabled = true;
					Text = "SAFontEdit - " + filename + " / Total characters: " + files.Count.ToString();
					pictureBoxFontColor.Visible = labelA.Visible = labelR.Visible = labelB.Visible = labelG.Visible = labelColor.Visible = numericUpDownAlpha.Visible = numericUpDownGreen.Visible = numericUpDownRed.Visible = numericUpDownBlue.Visible = argb;
					fONTDATAFileToolStripMenuItem.Enabled = !argb;
				}
			}
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog a = new OpenFileDialog()
			{
				DefaultExt = "bin",
				Filter = "Font files|*.bin|All Files|*.*"
			};
			if (a.ShowDialog() == DialogResult.OK)
			{
				LoadFile(a.FileName);
			}

		}

		private void extractAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (FolderBrowserDialog dlg = new FolderBrowserDialog() { Description = "Select the folder to export font files", ShowNewFolderButton = true, UseDescriptionForTitle = true })
			{
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					FontINI ini = new FontINI();
					ini.chars = new List<FontCharacter>();
					for (int i = 0; i < (files.Count) / 256 + Math.Min(1 * (files.Count) % 256, 1); i++)
					{
						var bitmap = new Bitmap(384, 384);
						using (var canvas = Graphics.FromImage(bitmap))
						{
							canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
							for (int u = 0; u < 256; u++)
							{
								if ((256 * i + u) >= files.Count) break;
								FontCharacter newitem = new FontCharacter();
								newitem.id = (files[256 * i + u].ID.ToString("X"));
								newitem.md = BitConverter.ToString(files[256 * i + u].miscdata).Replace("-", "");
								newitem.ch = files[256 * i + u].character;
								ini.chars.Add(newitem);
								canvas.DrawImage(files[256 * i + u].bits.ToBitmap(), new Rectangle(24 * (u % 16), 24 * (u / 16), 24, 24), new Rectangle(0, 0, 24, 24), GraphicsUnit.Pixel);
								canvas.Save();
							}
							bitmap.Save(System.IO.Path.Combine(dlg.SelectedPath, "fontsheet" + i.ToString() + ".png"));
						}
					}
					IniSerializer.Serialize(ini, System.IO.Path.Combine(dlg.SelectedPath, "fontdata.txt"));
				}
			}
		}

		private void TilePicture_MouseDown(object sender, MouseEventArgs e)
		{
			if (listBox1.SelectedIndex > -1)
				if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
				{
					int x = e.X / (int)trackBar1.Value;
					int y = e.Y / (int)trackBar1.Value;
					BitmapBits bits = files[listBox1.SelectedIndex].bits;
					if (bits is BitmapBits1bpp bits1)
					{
						bits1[x, y] = e.Button == MouseButtons.Left;
					}
					else if (bits is BitmapBits32bpp bits32)
					{
						bits32[x, y] = e.Button == MouseButtons.Left ? pictureBoxFontColor.BackColor : Color.Transparent;
					}
					lastpoint = new Point(x, y);
					DrawTile();
					characterTileControl1.Invalidate();
				}
		}

		Point lastpoint;
		private void TilePicture_MouseMove(object sender, MouseEventArgs e)
		{
			if (listBox1.SelectedIndex > -1)
				if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
				{
					int x = e.X / (int)trackBar1.Value;
					x = Math.Min(Math.Max(x, 0), 23);
					int y = e.Y / (int)trackBar1.Value;
					y = Math.Min(Math.Max(y, 0), 23);
					BitmapBits bits = files[listBox1.SelectedIndex].bits;
					if (bits is BitmapBits1bpp bits1)
					{
						bits1.DrawLine(e.Button == MouseButtons.Left, lastpoint, new Point(x, y));
						bits1[x, y] = e.Button == MouseButtons.Left;
					}
					else if (bits is BitmapBits32bpp bits32)
					{
						bits32.DrawLine(e.Button == MouseButtons.Left ? pictureBoxFontColor.BackColor : Color.Transparent, lastpoint, new Point(x, y));
						bits32[x, y] = e.Button == MouseButtons.Left ? pictureBoxFontColor.BackColor : Color.Transparent;
					}
					lastpoint = new Point(x, y);
					DrawTile();
					characterTileControl1.Invalidate();
				}
		}

		private void DrawTile()
		{
			if (listBox1.SelectedIndex == -1)
				characterTileControl1.Clear = true;
			else
			{
				BitmapBits bits = files[listBox1.SelectedIndex].bits;
				characterTileControl1.Bits = bits;
				characterTileControl1.Palette = pal;
				characterTileControl1.Clear = false;
				characterTileControl1.ImgScale = trackBar1.Value;
				characterTileControl1.Clear = false;
			}
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			buttonExportCharacter.Enabled = buttonImportCharacter.Enabled = listBox1.SelectedIndex != -1;
			DrawTile();
			characterTileControl1.Invalidate();
		}

		private void trackBar1_Scroll(object sender, EventArgs e)
		{
			characterTileControl1.Size = new Size(24 * (int)trackBar1.Value, 24 * (int)trackBar1.Value);
			DrawTile();
			characterTileControl1.Invalidate();
		}

		static void FromShort(ushort number, out byte byte1, out byte byte2)
		{
			byte2 = (byte)(number >> 8);
			byte1 = (byte)(number & 255);
		}

		private void quitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		public static byte[] StrToByteArray(string str)
		{
			Dictionary<string, byte> hexindex = new Dictionary<string, byte>();
			for (int i = 0; i <= 255; i++)
				hexindex.Add(i.ToString("X2"), (byte)i);

			List<byte> hexres = new List<byte>();
			for (int i = 0; i < str.Length; i += 2)
				hexres.Add(hexindex[str.Substring(i, 2)]);

			return hexres.ToArray();
		}

		private void importToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog a = new OpenFileDialog()
			{
				DefaultExt = "txt",
				Filter = "Font data|fontdata.txt|All Files|*.*",
				Multiselect = false
			})
			{
				if (a.ShowDialog(this) == DialogResult.OK)
				{
					FontINI ini = new FontINI();
					ini = IniSerializer.Deserialize<FontINI>(a.FileName);
					files = new List<FontItem>();
					listBox1.Items.Clear();
					listBox1.BeginUpdate();
					bool usealpha = false;
					for (int i = 0; i < (ini.chars.Count) / 256 + Math.Min(1 * (ini.chars.Count) % 256, 1); i++)
					{
						if (!File.Exists(Path.GetDirectoryName(a.FileName) + "\\fontsheet" + i.ToString() + ".png")) break;
						Bitmap sheetbmp = new Bitmap(Image.FromFile(Path.GetDirectoryName(a.FileName) + "\\fontsheet" + i.ToString() + ".png"));
						if (i == 0 && sheetbmp.PixelFormat == PixelFormat.Format32bppArgb)
						{
							DialogResult res = MessageBox.Show(this, "Import with full transparency?\n\nSelect 'Yes' for SA2 PC or 'SOC' files, 'No' for Dreamcast, Gamecube and SADX PC 2004 files.", "SA Font Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
							usealpha = res == DialogResult.Yes;
						}
						for (int u = 0; u < 256; u++)
						{
							if (i * 256 + u >= ini.chars.Count) break;
							FontItem item = new FontItem();
							Bitmap charbmp = new Bitmap(24, 24);
							using (var canvas = Graphics.FromImage(charbmp))
							{
								Text = "Importing character " + u.ToString() + " of " + ini.chars.Count.ToString();
								canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
								canvas.DrawImage(sheetbmp, new Rectangle(0, 0, 24, 24), new Rectangle((u % 16) * 24, (u / 16) * 24, 24, 24), GraphicsUnit.Pixel);
								canvas.Save();
								item.bits = usealpha ? new BitmapBits32bpp(charbmp.Clone(new Rectangle(0, 0, 24, 24), PixelFormat.Format32bppArgb)) : new BitmapBits1bpp(charbmp.Clone(new Rectangle(0, 0, 24, 24), PixelFormat.Format1bppIndexed));
								item.ID = ushort.Parse(ini.chars[i * 256 + u].id, System.Globalization.NumberStyles.HexNumber);
								item.miscdata = StrToByteArray(ini.chars[i * 256 + u].md);
								item.character = ini.chars[i * 256 + u].ch;
								files.Add(item);
							}
						}
						sheetbmp.Dispose();

					}
					for (int z = 0; z < files.Count; z++)
					{
						listBox1.Items.Add(files[z].ID.ToString("X4") + ' ' + files[z].character);
					}
					listBox1.EndUpdate();
					listBox1.SelectedIndex = 0;
					Text = "SAFontEdit - " + a.FileName + " / Total characters: " + files.Count.ToString();
					extractAllToolStripMenuItem.Enabled = saveAsToolStripMenuItem.Enabled = exportIndividualCharactersToolStripMenuItem.Enabled = true;
				}
			}
		}

		private void exportIndividualCharactersToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (FolderBrowserDialog dlg = new FolderBrowserDialog() { Description = "Select the folder to export font files", ShowNewFolderButton = true, UseDescriptionForTitle = true })
			{
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					foreach (FontItem item in files)
					{
						if (item.bits is BitmapBits1bpp bits1)
							bits1.ToBitmap(pal).Save(System.IO.Path.Combine(dlg.SelectedPath, item.ID.ToString("X4") + ".png"));
						else
							item.bits.ToBitmap().Save(System.IO.Path.Combine(dlg.SelectedPath, item.ID.ToString("X4") + ".png"));
					}
				}
			}
		}

		private void fONTDATAFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveFileDialog a = new SaveFileDialog()
			{
				DefaultExt = "bin",
				Filter = "BIN Files|*.bin|All Files|*.*"
			};
			if (a.ShowDialog() == DialogResult.OK)
			{
				List<byte> file = new List<byte>();
				foreach (FontItem item in files)
					file.AddRange(item.GetBytes());
				System.IO.File.WriteAllBytes(a.FileName, file.ToArray());
			}
		}

		private void simpleFormatFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveFileDialog a = new SaveFileDialog()
			{
				DefaultExt = "bin",
				Filter = "BIN Files|*.bin|All Files|*.*"
			};
			if (a.ShowDialog() == DialogResult.OK)
			{
				List<byte> file = new List<byte>();
				foreach (FontItem item in files)
				{
					if (item.bits is BitmapBits32bpp bitmapBits1)
					{
						item.bits = new BitmapBits1bpp(bitmapBits1.ToBitmap());
						file.AddRange(item.bits.GetBytes(true));
					}
					else
						file.AddRange(item.bits.GetBytes(true));
				}
				System.IO.File.WriteAllBytes(a.FileName, file.ToArray());
			}
		}

		private void simpleFormatFile32bppToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveFileDialog a = new SaveFileDialog()
			{
				DefaultExt = "bin",
				Filter = "BIN Files|*.bin|All Files|*.*"
			};
			if (a.ShowDialog() == DialogResult.OK)
			{
				List<byte> file = new List<byte>();
				foreach (FontItem item in files)
				{
					if (item.bits is BitmapBits1bpp bitmapBits1)
					{
						item.bits = new BitmapBits32bpp(bitmapBits1.ToBitmap());
						file.AddRange(item.bits.GetBytes(false));
					}
					else
						file.AddRange(item.bits.GetBytes(false));
				}
				System.IO.File.WriteAllBytes(a.FileName, file.ToArray());
			}
		}

		private void pictureBoxFontColor_Click(object sender, EventArgs e)
		{
			using (ColorDialog dlg = new ColorDialog { FullOpen = true, AnyColor = true, Color = pictureBoxFontColor.BackColor })
			{
				if (dlg.ShowDialog(Owner) == DialogResult.OK)
				{
					numericUpDownRed.Value = dlg.Color.R;
					numericUpDownGreen.Value = dlg.Color.G;
					numericUpDownBlue.Value = dlg.Color.B;
					pictureBoxFontColor.BackColor = Color.FromArgb((int)numericUpDownAlpha.Value, (int)numericUpDownRed.Value, (int)numericUpDownGreen.Value, (int)numericUpDownBlue.Value);
				}
			}
		}

		private void numericUpDownAlpha_ValueChanged(object sender, EventArgs e)
		{
			pictureBoxFontColor.BackColor = Color.FromArgb((int)numericUpDownAlpha.Value, (int)numericUpDownRed.Value, (int)numericUpDownGreen.Value, (int)numericUpDownBlue.Value);
		}

		private void buttonImportCharacter_Click(object sender, EventArgs e)
		{
			if (listBox1.SelectedIndex == -1)
				return;
			FontItem item = files[listBox1.SelectedIndex];
			using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "png", Filter = "PNG Files|*.png", FileName = item.ID.ToString("X4") + ".png" })
			{
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					Bitmap bmp = new Bitmap(dlg.FileName);
					if (item.bits is BitmapBits1bpp)
						item.bits = new BitmapBits1bpp(bmp);
					else if (item.bits is BitmapBits32bpp)
						item.bits = new BitmapBits32bpp(bmp);
					DrawTile();
					characterTileControl1.Invalidate();
					bmp.Dispose();
				}
			}
		}

		private void buttonExportCharacter_Click(object sender, EventArgs e)
		{
			if (listBox1.SelectedIndex == -1)
				return;
			FontItem item = files[listBox1.SelectedIndex];
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "png", Filter = "PNG Files|*.png", FileName = item.ID.ToString("X4") + ".png" })
			{
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					{
						if (item.bits is BitmapBits1bpp bits1)
							bits1.ToBitmap(pal).Save(dlg.FileName);
						else
							item.bits.ToBitmap().Save(dlg.FileName);
					}
				}
			}
		}

		private void toolStripTextBox1_Click(object sender, EventArgs e)
		{
			toolStripTextBox1.Text = "";
			toolStripTextBox1.ForeColor = Color.Black;
		}

		private void toolStripMenuItemSearch_Click(object sender, EventArgs e)
		{
			bool found = false;
			int start = Math.Max(0, listBox1.SelectedIndex);
		search:
			for (int i = start; i < listBox1.Items.Count; i++)
			{
				FontItem item = files[i];
				if (item.ID.ToString("X4").ToLowerInvariant() == toolStripTextBox1.Text.ToLowerInvariant() || item.character.Substring(0, 1) == toolStripTextBox1.Text.Substring(0, 1))
				{
					listBox1.SelectedIndex = i;
					found = true;
				}
			}
			if (!found)
			{
				if (start != 0)
				{
					start = 0;
					goto search;
				}
				else
					MessageBox.Show(this, "Item not found.", "SAFontEdit", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}

		}

		private void toolStripTextBox1_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == 13)
				toolStripMenuItemSearch_Click(sender, e);
		}

		private void sAFontEditGuideToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd", $"/c start " + "https://github.com/X-Hax/sa_tools/wiki/SAFontEdit") { CreateNoWindow = false });
		}

		private void bugReportToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd", $"/c start " + "https://github.com/X-Hax/sa_tools/issues/new/choose") { CreateNoWindow = false });
		}
	}

	internal class FontItem
    {
        public ushort ID;
        public byte[] miscdata;
        public BitmapBits bits;
		public string character;

        public FontItem()
        {
        }

        public FontItem(byte[] file, int address, bool oldformat, ushort index, bool argb)
        {
			if (!oldformat)
			{
				ID = BitConverter.ToUInt16(file, address);
				miscdata = new byte[0xE];
				Array.Copy(file, address + 2, miscdata, 0, miscdata.Length);
					bits = new BitmapBits1bpp(file, address + 0x10, false);
			}
			else
			{
				ID = index;
				miscdata = new byte[0xE];
				if (argb)
					bits = new BitmapBits32bpp(file, address, false);
				else
					bits = new BitmapBits1bpp(file, address, true);
			}
        }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>();
            result.AddRange(BitConverter.GetBytes(ID));
            result.AddRange(miscdata);
            result.AddRange(bits.GetBytes(false));
            return result.ToArray();
        }
    }

	public class FontCharacter
	{
		public string id { get; set; }
		[DefaultValue("0000000000000000000000000000")]
		public string md { get; set; } = "0000000000000000000000000000";
		[DefaultValue("")]
		public string ch { get; set; } = "";
	}

	internal class FontINI
	{
		public List<FontCharacter> chars;
	}

}