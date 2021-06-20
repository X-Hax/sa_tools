using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PLTool
{
    public partial class MainForm : Form
    {
        public List<PLPalette> currentPLFile;
        private int selectedColorIndex;
        private int selectedPaletteIndex;
        private bool isSpecular;

        public MainForm()
        {
            InitializeComponent();
            CreateBlankPalettes();
        }

        private void CreateBlankPalettes()
        {
            currentPLFile = new List<PLPalette>();
            for (int i = 0; i < 9; i++)
                currentPLFile.Add(new PLPalette());
            RefreshAllPalettes();
            RefreshPalettePreview();
            RefreshColorPreview();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "PL Files|PL*.BIN|All Files|*.*"} )
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    currentPLFile = new PLFile(File.ReadAllBytes(ofd.FileName)).Palettes;
                    RefreshAllPalettes();
                    RefreshPalettePreview();
                    RefreshColorPreview();
                }
            }
		}

        private Bitmap DrawGradient(PLPalette palette, bool specular)
        {
            Bitmap image = new Bitmap(512, 32);
            using (Graphics gfx = Graphics.FromImage(image))
            {
                gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                for (int i = 0; i < 256; i++)
                {
                    gfx.FillRectangle(new SolidBrush(palette.Colors[i, specular ? 1 : 0]), i * 2, 0, 2, 32);
                }
            }
            return image;
        }

        private void RefreshAllPalettes()
        {
            DiffusePalette0.Image = DrawGradient(currentPLFile[0], false);
            DiffusePalette1.Image = DrawGradient(currentPLFile[1], false);
            DiffusePalette2.Image = DrawGradient(currentPLFile[2], false);
            DiffusePalette3.Image = DrawGradient(currentPLFile[3], false);
            DiffusePalette4.Image = DrawGradient(currentPLFile[4], false);
            DiffusePalette5.Image = DrawGradient(currentPLFile[5], false);
            DiffusePalette6.Image = DrawGradient(currentPLFile[6], false);
            DiffusePalette7.Image = DrawGradient(currentPLFile[7], false);
            SpecularPalette0.Image = DrawGradient(currentPLFile[0], true);
            SpecularPalette1.Image = DrawGradient(currentPLFile[1], true);
            SpecularPalette2.Image = DrawGradient(currentPLFile[2], true);
            SpecularPalette3.Image = DrawGradient(currentPLFile[3], true);
            SpecularPalette4.Image = DrawGradient(currentPLFile[4], true);
            SpecularPalette5.Image = DrawGradient(currentPLFile[5], true);
            SpecularPalette6.Image = DrawGradient(currentPLFile[6], true);
            SpecularPalette7.Image = DrawGradient(currentPLFile[7], true);
            // Backup palettes
            DiffusePaletteB.Image = DrawGradient(currentPLFile[8], false);
            SpecularPaletteB.Image = DrawGradient(currentPLFile[8], true);
        }

        private void RefreshPalette(int index)
        {
            switch (index)
            {
                case 0:
                    DiffusePalette0.Image = DrawGradient(currentPLFile[0], false);
                    SpecularPalette0.Image = DrawGradient(currentPLFile[0], true);
                    break;
                case 1:
                    DiffusePalette1.Image = DrawGradient(currentPLFile[1], false);
                    SpecularPalette1.Image = DrawGradient(currentPLFile[1], true);
                    break;
                case 2:
                    DiffusePalette2.Image = DrawGradient(currentPLFile[2], false);
                    SpecularPalette2.Image = DrawGradient(currentPLFile[2], true);
                    break;
                case 3:
                    DiffusePalette3.Image = DrawGradient(currentPLFile[3], false);
                    SpecularPalette3.Image = DrawGradient(currentPLFile[3], true);
                    break;
                case 4:
                    DiffusePalette4.Image = DrawGradient(currentPLFile[4], false);
                    SpecularPalette4.Image = DrawGradient(currentPLFile[4], true);
                    break;
                case 5:
                    DiffusePalette5.Image = DrawGradient(currentPLFile[5], false);
                    SpecularPalette5.Image = DrawGradient(currentPLFile[5], true);
                    break;
                case 6:
                    DiffusePalette6.Image = DrawGradient(currentPLFile[6], false);
                    SpecularPalette6.Image = DrawGradient(currentPLFile[6], true);
                    break;
                case 7:
                    DiffusePalette7.Image = DrawGradient(currentPLFile[7], false);
                    SpecularPalette7.Image = DrawGradient(currentPLFile[7], true);
                    break;
                case 8:
                    DiffusePaletteB.Image = DrawGradient(currentPLFile[8], false);
                    SpecularPaletteB.Image = DrawGradient(currentPLFile[8], true);
                    break;
            }
        }

        private Bitmap DrawPaletteSelectionRectangle(Image bitmap)
        {
            Bitmap result = new Bitmap(bitmap);
            using (Graphics gfx = Graphics.FromImage(result))
            {
                gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                gfx.DrawRectangle(new Pen(Color.Black, 3.0f), 1, 1, 511, 31);
                gfx.DrawRectangle(new Pen(Color.Cyan, 2.0f), 0, 0, 512, 32);
                return result;
            }
        }

        private void RefreshPalettePreview()
        {
            pictureBoxPalettePreview.Image = new Bitmap(1024, 64);
            using (Graphics gfx = Graphics.FromImage(pictureBoxPalettePreview.Image))
            {
                gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                for (int i = 0; i < 256; i++)
                {
                    gfx.FillRectangle(new SolidBrush(currentPLFile[selectedPaletteIndex].Colors[i, isSpecular ? 1 : 0]), i * 4, 0, 4, 64);
                }
                gfx.FillRectangle(new SolidBrush(Color.Red), selectedColorIndex * 4, 0, 4, 32);
                switch (selectedPaletteIndex)
                {
                    case 0:
                    default:
                        if (isSpecular)
                            SpecularPalette0.Image = DrawPaletteSelectionRectangle(SpecularPalette0.Image);
                        else
                            DiffusePalette0.Image = DrawPaletteSelectionRectangle(DiffusePalette0.Image);
                        break;
                    case 1:
                        if (isSpecular)
                            SpecularPalette1.Image = DrawPaletteSelectionRectangle(SpecularPalette1.Image);
                        else
                            DiffusePalette1.Image = DrawPaletteSelectionRectangle(DiffusePalette1.Image);
                        break;
                    case 2:
                        if (isSpecular)
                            SpecularPalette2.Image = DrawPaletteSelectionRectangle(SpecularPalette2.Image);
                        else
                            DiffusePalette2.Image = DrawPaletteSelectionRectangle(DiffusePalette2.Image);
                        break;
                    case 3:
                        if (isSpecular)
                            SpecularPalette3.Image = DrawPaletteSelectionRectangle(SpecularPalette3.Image);
                        else
                            DiffusePalette3.Image = DrawPaletteSelectionRectangle(DiffusePalette3.Image);
                        break;
                    case 4:
                        if (isSpecular)
                            SpecularPalette4.Image = DrawPaletteSelectionRectangle(SpecularPalette4.Image);
                        else
                            DiffusePalette4.Image = DrawPaletteSelectionRectangle(DiffusePalette4.Image);
                        break;
                    case 5:
                        if (isSpecular)
                            SpecularPalette5.Image = DrawPaletteSelectionRectangle(SpecularPalette5.Image);
                        else
                            DiffusePalette5.Image = DrawPaletteSelectionRectangle(DiffusePalette5.Image);
                        break;
                    case 6:
                        if (isSpecular)
                            SpecularPalette6.Image = DrawPaletteSelectionRectangle(SpecularPalette6.Image);
                        else
                            DiffusePalette6.Image = DrawPaletteSelectionRectangle(DiffusePalette6.Image);
                        break;
                    case 7:
                        if (isSpecular)
                            SpecularPalette7.Image = DrawPaletteSelectionRectangle(SpecularPalette7.Image);
                        else
                            DiffusePalette7.Image = DrawPaletteSelectionRectangle(DiffusePalette7.Image);
                        break;
                    case 8:
                        if (isSpecular)
                            SpecularPaletteB.Image = DrawPaletteSelectionRectangle(SpecularPaletteB.Image);
                        else
                            DiffusePaletteB.Image = DrawPaletteSelectionRectangle(DiffusePaletteB.Image);
                        break;
                }
            }
            RefreshStatus();
            RefreshColorPreview();
        }

        private void RefreshColorPreview()
        {
            toolStripSplitButtonColor.Image = new Bitmap(24, 24);
            using (Graphics gfx = Graphics.FromImage(toolStripSplitButtonColor.Image))
            {
                gfx.Clear(currentPLFile[selectedPaletteIndex].Colors[selectedColorIndex, isSpecular ? 1 : 0]);
            }
            RefreshStatus();
        }

		private void trackBar1_ValueChanged(object sender, EventArgs e)
		{
            selectedColorIndex = trackBar1.Value;
            RefreshPalettePreview();
            RefreshColorPreview();
        }

        private void RefreshStatus()
        {
            StatusPaletteIndices.Text = isSpecular ? "Specular " : "Diffuse " + selectedPaletteIndex.ToString() + " / Color " + selectedColorIndex.ToString();
            StatusR.Text = "R" + currentPLFile[selectedPaletteIndex].Colors[selectedColorIndex, isSpecular ? 1 : 0].R.ToString("D3");
            StatusG.Text = "G" + currentPLFile[selectedPaletteIndex].Colors[selectedColorIndex, isSpecular ? 1 : 0].G.ToString("D3");
            StatusB.Text = "B" + currentPLFile[selectedPaletteIndex].Colors[selectedColorIndex, isSpecular ? 1 : 0].B.ToString("D3");
            StatusA.Text = "A" + currentPLFile[selectedPaletteIndex].Colors[selectedColorIndex, isSpecular ? 1 : 0].A.ToString("D3");
        }

        private void SelectPalette(int index, bool specular)
        {
            RefreshPalette(selectedPaletteIndex);
            selectedPaletteIndex = index;
            isSpecular = specular;
            RefreshPalettePreview();
            RefreshColorPreview();
            RefreshStatus();
        }

		private void DiffusePalette0_Click(object sender, EventArgs e)
		{
            SelectPalette(0, false);
        }

		private void DiffusePalette1_Click(object sender, EventArgs e)
		{
            SelectPalette(1, false);
        }

		private void DiffusePalette2_Click(object sender, EventArgs e)
		{
            SelectPalette(2, false);
        }

		private void DiffusePalette3_Click(object sender, EventArgs e)
		{
            SelectPalette(3, false);
        }

		private void DiffusePalette4_Click(object sender, EventArgs e)
		{
            SelectPalette(4, false);
        }

		private void DiffusePalette5_Click(object sender, EventArgs e)
		{
            SelectPalette(5, false);
        }

		private void DiffusePalette6_Click(object sender, EventArgs e)
		{
            SelectPalette(6, false);
        }

		private void DiffusePalette7_Click(object sender, EventArgs e)
		{
            SelectPalette(7, false);
        }

		private void DiffusePaletteB_Click(object sender, EventArgs e)
		{
            SelectPalette(8, false);
        }

		private void SpecularPalette0_Click(object sender, EventArgs e)
		{
            SelectPalette(0, true);
        }

		private void SpecularPalette1_Click(object sender, EventArgs e)
		{
            SelectPalette(1, true);
        }

		private void SpecularPalette2_Click(object sender, EventArgs e)
		{
            SelectPalette(2, true);
        }

		private void SpecularPalette3_Click(object sender, EventArgs e)
		{
            SelectPalette(3, true);
        }

		private void SpecularPalette4_Click(object sender, EventArgs e)
		{
            SelectPalette(4, true);
        }

		private void SpecularPalette5_Click(object sender, EventArgs e)
		{
            SelectPalette(5, true);
        }

		private void SpecularPalette6_Click(object sender, EventArgs e)
		{
            SelectPalette(6, true);
        }

		private void SpecularPalette7_Click(object sender, EventArgs e)
		{
            SelectPalette(7, true);
        }

		private void SpecularPaletteB_Click(object sender, EventArgs e)
		{
            SelectPalette(8, true);
        }

		private void paletteListBlankToolStripMenuItem_Click(object sender, EventArgs e)
		{
            CreateBlankPalettes();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
            Application.Exit();
		}
	}
}
