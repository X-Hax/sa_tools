using SAModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PLTool
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
    }

    public class PLFile
	{
        public List<PLPalette> Palettes;

        public PLFile(byte[] file)
        {
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
        }
    }
}
