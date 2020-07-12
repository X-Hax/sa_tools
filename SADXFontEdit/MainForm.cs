using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using SA_Tools;
using System.Drawing.Imaging;
using System.Text;
using System.ComponentModel;

namespace SADXFontEdit
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
			tileGfx = TilePicture.CreateGraphics();
			tileGfx.SetOptions();
			TilePicture.Cursor = new Cursor(new System.IO.MemoryStream(Properties.Resources.pencilcur));
			TilePicture.Size = new Size(24 * (int)trackBar1.Value, 24 * (int)trackBar1.Value);
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
					if (File.Exists(ftd.customfile)) customstring = File.ReadAllText(ftd.customfile);
					if (ftd.fontdata)
					{
						itemlength = 0x58;
						oldformat = false;
					}
					filename = filename_a;
					byte[] file = System.IO.File.ReadAllBytes(filename);
					int count = file.Length / itemlength;
					files = new List<FontItem>(count);
					listBox1.Items.Clear();
					listBox1.BeginUpdate();
					for (int i = 0; i < count; i++)
					{

						files.Add(new FontItem(file, i * itemlength, oldformat, (ushort)i));
						if (!ftd.custom)
						{
							if (ftd.charset == 50220)
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
							else if (ftd.charset == 1200)
							{
								origtext = new byte[2];
								origtext[0] = (byte)(files[i].ID >> 0);
								origtext[1] = (byte)(files[i].ID >> 8);
								utf16String = Encoding.GetEncoding(1200).GetString(origtext);
							}
							else
							{
								origtext = new byte[1];
								origtext[0] = (byte)files[i].ID;
								utf16String = Encoding.GetEncoding(ftd.charset).GetString(origtext);
							}
						}
						else
						{
							utf16String = customstring[Math.Min(customstring.Length - 1, i)] + "";
						}
						files[i].character = utf16String;
						listBox1.Items.Add(files[i].ID.ToString("X4") + ' ' + utf16String);
					}
					listBox1.EndUpdate();
					extractAllToolStripMenuItem.Enabled = exportIndividualCharactersToolStripMenuItem.Enabled = saveAsToolStripMenuItem.Enabled = true;
					Text = "SADXFontEdit - " + filename + " / Total characters: " + files.Count.ToString();
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

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
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

		private void extractAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "", Filter = "", FileName = "font" })
			{
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					Directory.CreateDirectory(dlg.FileName);
					string dir = Path.Combine(Path.GetDirectoryName(dlg.FileName), Path.GetFileName(dlg.FileName));
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
							bitmap.Save(System.IO.Path.Combine(dir, "fontsheet" + i.ToString() + ".png"));
						}
					}
					IniSerializer.Serialize(ini, System.IO.Path.Combine(dir, "fontdata.txt"));
				}
			}
		}
		private void TilePicture_Paint(object sender, PaintEventArgs e)
		{
			DrawTile();
		}

		private void TilePicture_MouseDown(object sender, MouseEventArgs e)
		{
			if (listBox1.SelectedIndex > -1)
				if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
				{
					int x = e.X / (int)trackBar1.Value;
					int y = e.Y / (int)trackBar1.Value;
					files[listBox1.SelectedIndex].bits[x, y] = e.Button == MouseButtons.Left;
					lastpoint = new Point(x, y);
					DrawTile();
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
					files[listBox1.SelectedIndex].bits.DrawLine(e.Button == MouseButtons.Left, lastpoint, new Point(x, y));
					files[listBox1.SelectedIndex].bits[x, y] = e.Button == MouseButtons.Left;
					lastpoint = new Point(x, y);
					DrawTile();
				}
		}

		private Graphics tileGfx;
		private void DrawTile()
		{
			if (listBox1.SelectedIndex > -1)
			{
				tileGfx.DrawImage(files[listBox1.SelectedIndex].bits.Scale((int)trackBar1.Value).ToBitmap(pal), 0, 0, 24 * (int)trackBar1.Value, 24 * (int)trackBar1.Value);
			}
			else
				tileGfx.Clear(Color.Black);
		}

		private void TilePicture_Resize(object sender, EventArgs e)
		{
			tileGfx = TilePicture.CreateGraphics();
			tileGfx.SetOptions();
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			DrawTile();
		}

		private void trackBar1_Scroll(object sender, EventArgs e)
		{
			TilePicture.Size = new Size(24 * (int)trackBar1.Value, 24 * (int)trackBar1.Value);
			DrawTile();
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
					for (int i = 0; i < (ini.chars.Count) / 256 + Math.Min(1 * (ini.chars.Count) % 256, 1); i++)
					{
						if (!File.Exists(Path.GetDirectoryName(a.FileName) + "\\fontsheet" + i.ToString() + ".png")) break;
						Bitmap sheetbmp = new Bitmap(Image.FromFile(Path.GetDirectoryName(a.FileName) + "\\fontsheet" + i.ToString() + ".png"));
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
								item.bits = new BitmapBits(charbmp.Clone(new Rectangle(0, 0, 24, 24), PixelFormat.Format1bppIndexed));
								item.ID = ushort.Parse(ini.chars[i*256+u].id, System.Globalization.NumberStyles.HexNumber);
								item.miscdata = StrToByteArray(ini.chars[i * 256 + u].md);
								item.character = ini.chars[i * 256 + u].ch;
								files.Add(item);
							}
						}
						
					}
					for (int z = 0; z < files.Count; z++)
					{
						listBox1.Items.Add(files[z].ID.ToString("X4") + ' ' + files[z].character);
					}
					listBox1.EndUpdate();
					Text = "SADXFontEdit - " + a.FileName + " / Total characters: " + files.Count.ToString();
					extractAllToolStripMenuItem.Enabled = saveAsToolStripMenuItem.Enabled = exportIndividualCharactersToolStripMenuItem.Enabled = true;
				}
			}
		}

		private void exportIndividualCharactersToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "", Filter = "", FileName = "font" })
			{
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					Directory.CreateDirectory(dlg.FileName);
					string dir = Path.Combine(Path.GetDirectoryName(dlg.FileName), Path.GetFileName(dlg.FileName));
					foreach (FontItem item in files)
					{
						item.bits.ToBitmap(pal).Save(System.IO.Path.Combine(dir, item.ID.ToString("X4") + ".png"));
					}
				}
			}
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

        public FontItem(byte[] file, int address, bool oldformat, ushort index)
        {
			if (!oldformat)
			{
				ID = BitConverter.ToUInt16(file, address);
				miscdata = new byte[0xE];
				Array.Copy(file, address + 2, miscdata, 0, miscdata.Length);
				bits = new BitmapBits(file, address + 0x10, false);
			}
			else
			{
				ID = index;
				miscdata = new byte[0xE];
				bits = new BitmapBits(file, address, true);
			}
        }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>();
            result.AddRange(BitConverter.GetBytes(ID));
            result.AddRange(miscdata);
            result.AddRange(bits.GetBytes());
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