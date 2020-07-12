using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SADXFontEdit
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        /// <summary>
        /// Sets options to enable faster rendering.
        /// </summary>
        public static void SetOptions(this System.Drawing.Graphics gfx)
        {
            gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
            gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
        }
    }
}
