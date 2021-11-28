using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SAModel.SAEditorCommon
{
    public class PLPalette
    {
        public Color[,] Colors = new Color[256, 2];
        public PLPalette() { }
        public PLPalette(byte[] file, int start)
        {
            int colorindex = 0;
            Colors = new Color[256, 2];
            for (int u = start; u < file.Length; u += 8)
            {
                if (colorindex >= 256)
                    break;
                int colorvalue_d = ByteConverter.ToInt32(file, u);
                int colorvalue_s = ByteConverter.ToInt32(file, u + 4);
                Colors[colorindex, 0] = Color.FromArgb(colorvalue_d);
                Colors[colorindex, 1] = Color.FromArgb(colorvalue_s);
                colorindex++;
            }
        }

        public List<Color> GetColorList(bool specular)
        {
            List<Color> result = new List<Color>();
            for (int i = 0; i < 256; i++)
                result.Add(Colors[i, specular ? 1:0]);
            return result;
        }

        public Bitmap ToPNG(bool specular)
        {
            Bitmap result = new Bitmap(256, 32);
            for (int p = 0; p < 32; p++)
            {
                for (int i = 0; i < 256; i++)
                {
                    result.SetPixel(i, p, Colors[i, specular ? 1:0]);
                }
            }
            return result;
        }

        public PLPalette(Color[,] colors)
        {
            Colors = new Color[256, 2];
            for (int colorindex = 0; colorindex < 256; colorindex++)
            {
                Colors[colorindex, 0] = colors[colorindex, 0];
                Colors[colorindex, 1] = colors[colorindex, 1];
            }
        }

        public byte[] GetBytes(bool bigEndian = false)
        {
            List<byte> result = new List<byte>();
            for (int i = 0; i < 256; i++)
            {
                if (bigEndian)
                {
                    result.Add(Colors[i, 0].A);
                    result.Add(Colors[i, 0].R);
                    result.Add(Colors[i, 0].G);
                    result.Add(Colors[i, 0].B);
                    result.Add(Colors[i, 1].A);
                    result.Add(Colors[i, 1].R);
                    result.Add(Colors[i, 1].G);
                    result.Add(Colors[i, 1].B);
                }
                else
                {
                    result.Add(Colors[i, 0].B);
                    result.Add(Colors[i, 0].G);
                    result.Add(Colors[i, 0].R);
                    result.Add(Colors[i, 0].A);
                    result.Add(Colors[i, 1].B);
                    result.Add(Colors[i, 1].G);
                    result.Add(Colors[i, 1].R);
                    result.Add(Colors[i, 1].A);
                }
            }
            return result.ToArray();
        }
    }

    public class PLFile
	{
        public List<PLPalette> Palettes;

        public PLFile()
        {
            Palettes = new List<PLPalette>();
            for (int p = 0; p < 9; p++)
            {
                Palettes.Add(new PLPalette());
            }
        }

		public PLFile(byte[] file, bool bigEndian = false)
		{
			ByteConverter.BackupEndian();
			ByteConverter.BigEndian = bigEndian;
			Palettes = new List<PLPalette>();
			int numpalettes = file.Length / 2048;
			for (int p = 0; p < numpalettes; p++)
			{
				Palettes.Add(new PLPalette(file, p * 2048));
			}
			if (Palettes.Count < 9)
				do
					Palettes.Add(new PLPalette());
				while (Palettes.Count < 9);
			ByteConverter.RestoreEndian();
		}

        public PLFile(Bitmap png)
        {
            if (png.Width != 512)
            {
                MessageBox.Show("Palette list PNG must be 512 pixels wide.", "PL Tool Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Palettes = new List<PLPalette>();
                for (int p = 0; p < 9; p++)
                {
                    Palettes.Add(new PLPalette());
                }
                return;
            }

            Palettes = new List<PLPalette>();
            Color[,] colors = new Color[256, 2];
            for (int p = 0; p < png.Height; p++)
            {
                for (int i = 0; i < 256; i++)
                {
                    colors[i, 0] = png.GetPixel(i, p);
                    colors[i, 1] = png.GetPixel(i + 256, p);
                }
                Palettes.Add(new PLPalette(colors));
            }
            if (Palettes.Count < 9)
                do
                    Palettes.Add(new PLPalette());
                while (Palettes.Count < 9);
        }

        private bool CheckEmptyPalette(PLPalette pal)
        {
            for (int i = 0; i < 256; i++)
            {
                if (pal.Colors[i, 0].A != 0)
                    return false;
                if (pal.Colors[i, 1].A != 0)
                    return false;
            }
            return true;
        }

        public Bitmap ToPNG()
        {
            int numpalettes = 0;
            for (int p = 0; p < 8; p++)
            {
                if (CheckEmptyPalette(Palettes[p]))
                    break;
                numpalettes++;
            }
            Bitmap result = new Bitmap(512, numpalettes);
            for (int p = 0; p < numpalettes; p++)
            {
                if (CheckEmptyPalette(Palettes[p]))
                    break;
                for (int i = 0; i < 256; i++)
                {
                    result.SetPixel(i, p, Palettes[p].Colors[i, 0]);
                    result.SetPixel(i + 256, p, Palettes[p].Colors[i, 1]);
                }
            }
            return result;
        }

        public byte[] GetBytes(bool bigEndian = false)
        {
            List<byte> result = new List<byte>();
            for (int p = 0; p < Palettes.Count; p++)
            {
                if (CheckEmptyPalette(Palettes[p]))
                    break;
                result.AddRange(Palettes[p].GetBytes(bigEndian));
            }
            return result.ToArray();
        }
    }
}
