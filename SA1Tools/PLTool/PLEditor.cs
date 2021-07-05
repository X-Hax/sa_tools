using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SAModel.SAEditorCommon;

namespace PLTool
{
    public partial class PLEditor : Form
    {
        public PLFile currentPLFile;
        private int selectedColorIndex;
        private int selectedPaletteIndex;
        private bool isSpecular;
        public Color[] clipboardPalette;
        public Color clipboardColor;
        bool isGamecube = false;
        public string currentFilename = "";

		public PLEditor(string filepath = "")
		{
			InitializeComponent();
			if (filepath != "" && File.Exists(filepath))
				LoadPLFile(filepath);
			else
				CreateDefaultPalettes();
		}

        private void CreateBlankPalettes()
        {
            currentPLFile = new PLFile();
            RefreshAllPalettes();
            RefreshPalettePreview();
        }

        private void CreateDefaultPalettes()
        {
            PLFile plf = new PLFile(Properties.Resources.defpalette);
            currentPLFile = new PLFile();
            currentPLFile.Palettes.Clear();
            foreach (PLPalette pl in plf.Palettes)
                currentPLFile.Palettes.Add(pl);
            RefreshAllPalettes();
            RefreshPalettePreview();
        }

        private void LoadPLFile(string filename)
        {
            currentPLFile = new PLFile(File.ReadAllBytes(filename), isGamecube);
            RefreshAllPalettes();
            RefreshPalettePreview();
            currentFilename = filename;
            this.Text = "Sonic Adventure PL Tool - " + currentFilename;
            toolStripStatusLabelFilename.Text = Path.GetFileNameWithoutExtension(currentFilename) + " ";
            toolStripStatusLabelLevelName.Text = SAModel.SAEditorCommon.LanternFilenames.GetLevelNameFromFilename(currentFilename);
            saveToolStripMenuItem.Enabled = true;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Title = "Open PL File", Filter = "PL Files|PL*.BIN|All Files|*.*" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    LoadPLFile(ofd.FileName);
                }
            }
        }

        private Bitmap DrawGradient(PLPalette palette, bool specular)
        {
            Bitmap image = new Bitmap(256, 32);
            using (Graphics gfx = Graphics.FromImage(image))
            {
                gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                for (int i = 0; i < 256; i++)
                {
                    gfx.FillRectangle(new SolidBrush(palette.Colors[i, specular ? 1 : 0]), i, 0, 1, 32);
                }
            }
            return image;
        }

        private void RefreshAllPalettes()
        {
            // Set scaled width and height, same for all palette controls
            int width = SpecularPalette0.Width;
            int height = SpecularPalette0.Height;
            DiffusePalette0.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[0], false), width, height);
            DiffusePalette1.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[1], false), width, height);
            DiffusePalette2.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[2], false), width, height);
            DiffusePalette3.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[3], false), width, height);
            DiffusePalette4.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[4], false), width, height);
            DiffusePalette5.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[5], false), width, height);
            DiffusePalette6.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[6], false), width, height);
            DiffusePalette7.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[7], false), width, height);
            SpecularPalette0.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[0], true), width, height);
            SpecularPalette1.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[1], true), width, height);
            SpecularPalette2.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[2], true), width, height);
            SpecularPalette3.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[3], true), width, height);
            SpecularPalette4.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[4], true), width, height);
            SpecularPalette5.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[5], true), width, height);
            SpecularPalette6.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[6], true), width, height);
            SpecularPalette7.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[7], true), width, height);
            // Backup palettes
            DiffusePaletteB.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[8], false), width, height);
            SpecularPaletteB.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[8], true), width, height);
        }

        private void RefreshPalette(int index)
        {
            // Set scaled width and height, same for all palette controls
            int width = SpecularPalette0.Width;
            int height = SpecularPalette0.Height;
            switch (index)
            {
                case 0:
                    DiffusePalette0.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[0], false), width, height);
                    SpecularPalette0.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[0], true), width, height);
                    break;
                case 1:
                    DiffusePalette1.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[1], false), width, height);
                    SpecularPalette1.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[1], true), width, height);
                    break;
                case 2:
                    DiffusePalette2.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[2], false), width, height);
                    SpecularPalette2.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[2], true), width, height);
                    break;
                case 3:
                    DiffusePalette3.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[3], false), width, height);
                    SpecularPalette3.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[3], true), width, height);
                    break;
                case 4:
                    DiffusePalette4.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[4], false), width, height);
                    SpecularPalette4.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[4], true), width, height);
                    break;
                case 5:
                    DiffusePalette5.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[5], false), width, height);
                    SpecularPalette5.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[5], true), width, height);
                    break;
                case 6:
                    DiffusePalette6.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[6], false), width, height);
                    SpecularPalette6.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[6], true), width, height);
                    break;
                case 7:
                    DiffusePalette7.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[7], false), width, height);
                    SpecularPalette7.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[7], true), width, height);
                    break;
                case 8:
                    DiffusePaletteB.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[8], false), width, height);
                    SpecularPaletteB.Image = DrawStretchedBitmap(DrawGradient(currentPLFile.Palettes[8], true), width, height);
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
                gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
                gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.AssumeLinear;
                gfx.DrawRectangle(new Pen(Color.Black), 1, 1, bitmap.Width - 3, bitmap.Height - 3);
                gfx.DrawRectangle(new Pen(Color.Cyan), 0, 0, bitmap.Width-1, bitmap.Height-1);
                return result;
            }
        }

        private Bitmap DrawStretchedBitmap(Bitmap bitmap, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics gfx = Graphics.FromImage(result))
            {
                gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                gfx.DrawImage(bitmap, 0, 0, width, height);
            }
            return result;
        }

        private void RefreshPalettePreview()
        {
            // Create preview bitmap
            Bitmap preview = new Bitmap(256, 36);
            using (Graphics gfx = Graphics.FromImage(preview))
            {
                gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                for (int i = 0; i < 256; i++)
                {
                    gfx.FillRectangle(new SolidBrush(currentPLFile.Palettes[selectedPaletteIndex].Colors[i, isSpecular ? 1 : 0]), i, 0, 1, 36);
                }
                gfx.FillRectangle(new SolidBrush(Color.Red), selectedColorIndex, 0, 1, 9);
                gfx.FillRectangle(new SolidBrush(Color.Black), selectedColorIndex, 27, 1, 9);

            }
            // Apply Bitmap to the scaled control
            pictureBoxPalettePreview.Image = DrawStretchedBitmap(preview, pictureBoxPalettePreview.Width, pictureBoxPalettePreview.Height);
            // Update selected palette
            // Set scaled width and height, same for all palette controls
            int width = SpecularPalette0.Width;
            int height = SpecularPalette0.Height;
            switch (selectedPaletteIndex)
            {
                case 0:
                default:
                    if (isSpecular)
                        SpecularPalette0.Image = DrawStretchedBitmap(DrawPaletteSelectionRectangle(SpecularPalette0.Image), width, height);
                    else
                        DiffusePalette0.Image = DrawStretchedBitmap(DrawPaletteSelectionRectangle(DiffusePalette0.Image), width, height);
                    break;
                case 1:
                    if (isSpecular)
                        SpecularPalette1.Image = DrawStretchedBitmap(DrawPaletteSelectionRectangle(SpecularPalette1.Image), width, height);
                    else
                        DiffusePalette1.Image = DrawStretchedBitmap(DrawPaletteSelectionRectangle(DiffusePalette1.Image), width, height);
                    break;
                case 2:
                    if (isSpecular)
                        SpecularPalette2.Image = DrawStretchedBitmap(DrawPaletteSelectionRectangle(SpecularPalette2.Image), width, height);
                    else
                        DiffusePalette2.Image = DrawStretchedBitmap(DrawPaletteSelectionRectangle(DiffusePalette2.Image), width, height);
                    break;
                case 3:
                    if (isSpecular)
                        SpecularPalette3.Image = DrawStretchedBitmap(DrawPaletteSelectionRectangle(SpecularPalette3.Image), width, height);
                    else
                        DiffusePalette3.Image = DrawStretchedBitmap(DrawPaletteSelectionRectangle(DiffusePalette3.Image), width, height);
                    break;
                case 4:
                    if (isSpecular)
                        SpecularPalette4.Image = DrawStretchedBitmap(DrawPaletteSelectionRectangle(SpecularPalette4.Image), width, height);
                    else
                        DiffusePalette4.Image = DrawStretchedBitmap(DrawPaletteSelectionRectangle(DiffusePalette4.Image), width, height);
                    break;
                case 5:
                    if (isSpecular)
                        SpecularPalette5.Image = DrawStretchedBitmap(DrawPaletteSelectionRectangle(SpecularPalette5.Image), width, height);
                    else
                        DiffusePalette5.Image = DrawStretchedBitmap(DrawPaletteSelectionRectangle(DiffusePalette5.Image), width, height);
                    break;
                case 6:
                    if (isSpecular)
                        SpecularPalette6.Image = DrawStretchedBitmap(DrawPaletteSelectionRectangle(SpecularPalette6.Image), width, height);
                    else
                        DiffusePalette6.Image = DrawStretchedBitmap(DrawPaletteSelectionRectangle(DiffusePalette6.Image), width, height);
                    break;
                case 7:
                    if (isSpecular)
                        SpecularPalette7.Image = DrawStretchedBitmap(DrawPaletteSelectionRectangle(SpecularPalette7.Image), width, height);
                    else
                        DiffusePalette7.Image = DrawStretchedBitmap(DrawPaletteSelectionRectangle(DiffusePalette7.Image), width, height);
                    break;
                case 8:
                    if (isSpecular)
                        SpecularPaletteB.Image = DrawStretchedBitmap(DrawPaletteSelectionRectangle(SpecularPaletteB.Image), width, height);
                    else
                        DiffusePaletteB.Image = DrawStretchedBitmap(DrawPaletteSelectionRectangle(DiffusePaletteB.Image), width, height);
                    break;
            }
            RefreshStatus();
            RefreshColorPreview();
        }

        private void RefreshColorPreview()
        {
            toolStripSplitButtonColor.Image = new Bitmap(16, 16);
            using (Graphics gfx = Graphics.FromImage(toolStripSplitButtonColor.Image))
            {
                gfx.Clear(currentPLFile.Palettes[selectedPaletteIndex].Colors[selectedColorIndex, isSpecular ? 1 : 0]);
            }
            RefreshStatus();
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            selectedColorIndex = trackBarColorIndex.Value;
            RefreshPalettePreview();
        }

        private void RefreshStatus()
        {
            StatusPaletteIndices.Text = (isSpecular ? "Specular " : "Diffuse ") + selectedPaletteIndex.ToString() + " / Color " + selectedColorIndex.ToString("D3");
            StatusR.Text = "R" + currentPLFile.Palettes[selectedPaletteIndex].Colors[selectedColorIndex, isSpecular ? 1 : 0].R.ToString("D3");
            StatusG.Text = "G" + currentPLFile.Palettes[selectedPaletteIndex].Colors[selectedColorIndex, isSpecular ? 1 : 0].G.ToString("D3");
            StatusB.Text = "B" + currentPLFile.Palettes[selectedPaletteIndex].Colors[selectedColorIndex, isSpecular ? 1 : 0].B.ToString("D3");
            StatusA.Text = "A" + currentPLFile.Palettes[selectedPaletteIndex].Colors[selectedColorIndex, isSpecular ? 1 : 0].A.ToString("D3");
        }

        private void SelectPalette(int index, bool specular, Control sender, MouseEventArgs e)
        {
            RefreshPalette(selectedPaletteIndex);
            selectedPaletteIndex = index;
            isSpecular = specular;
            RefreshPalettePreview();
            RefreshStatus();
            // Check for right button
            if (e.Button == MouseButtons.Right)
                contextMenuPalette.Show(sender, e.Location);
        }

        private void DiffusePalette0_Click(object sender, EventArgs e)
        {
            SelectPalette(0, false, (Control)sender, (MouseEventArgs)e);
        }

        private void DiffusePalette1_Click(object sender, EventArgs e)
        {
            SelectPalette(1, false, (Control)sender, (MouseEventArgs)e);
        }

        private void DiffusePalette2_Click(object sender, EventArgs e)
        {
            SelectPalette(2, false, (Control)sender, (MouseEventArgs)e);
        }

        private void DiffusePalette3_Click(object sender, EventArgs e)
        {
            SelectPalette(3, false, (Control)sender, (MouseEventArgs)e);
        }

        private void DiffusePalette4_Click(object sender, EventArgs e)
        {
            SelectPalette(4, false, (Control)sender, (MouseEventArgs)e);
        }

        private void DiffusePalette5_Click(object sender, EventArgs e)
        {
            SelectPalette(5, false, (Control)sender, (MouseEventArgs)e);
        }

        private void DiffusePalette6_Click(object sender, EventArgs e)
        {
            SelectPalette(6, false, (Control)sender, (MouseEventArgs)e);
        }

        private void DiffusePalette7_Click(object sender, EventArgs e)
        {
            SelectPalette(7, false, (Control)sender, (MouseEventArgs)e);
        }

        private void DiffusePaletteB_Click(object sender, EventArgs e)
        {
            SelectPalette(8, false, (Control)sender, (MouseEventArgs)e);
        }

        private void SpecularPalette0_Click(object sender, EventArgs e)
        {
            SelectPalette(0, true, (Control)sender, (MouseEventArgs)e);
        }

        private void SpecularPalette1_Click(object sender, EventArgs e)
        {
            SelectPalette(1, true, (Control)sender, (MouseEventArgs)e);
        }

        private void SpecularPalette2_Click(object sender, EventArgs e)
        {
            SelectPalette(2, true, (Control)sender, (MouseEventArgs)e);
        }

        private void SpecularPalette3_Click(object sender, EventArgs e)
        {
            SelectPalette(3, true, (Control)sender, (MouseEventArgs)e);
        }

        private void SpecularPalette4_Click(object sender, EventArgs e)
        {
            SelectPalette(4, true, (Control)sender, (MouseEventArgs)e);
        }

        private void SpecularPalette5_Click(object sender, EventArgs e)
        {
            SelectPalette(5, true, (Control)sender, (MouseEventArgs)e);
        }

        private void SpecularPalette6_Click(object sender, EventArgs e)
        {
            SelectPalette(6, true, (Control)sender, (MouseEventArgs)e);
        }

        private void SpecularPalette7_Click(object sender, EventArgs e)
        {
            SelectPalette(7, true, (Control)sender, (MouseEventArgs)e);
        }

        private void SpecularPaletteB_Click(object sender, EventArgs e)
        {
            SelectPalette(8, true, (Control)sender, (MouseEventArgs)e);
        }

        private void paletteListBlankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateBlankPalettes();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FillPaletteWithColor(Color color)
        {
            for (int i = 0; i < 256; i++)
                currentPLFile.Palettes[selectedPaletteIndex].Colors[i, isSpecular ? 1 : 0] = color;
            RefreshPalette(selectedPaletteIndex);
            RefreshPalettePreview();
        }

        private void toolStripFillWhite_Click(object sender, EventArgs e)
        {
            FillPaletteWithColor(Color.White);
        }

        private void toolStripFillBlack_Click(object sender, EventArgs e)
        {
            FillPaletteWithColor(Color.Black);
        }

        private void toolStripFillColor_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
                FillPaletteWithColor(colorDialog1.Color);
        }

        private void toolStripFillPattern_Click(object sender, EventArgs e)
        {
            using (ColorPattern cpn = new ColorPattern())
                if (cpn.ShowDialog() == DialogResult.OK)
                {
                    for (int i = 0; i < 256; i++)
                        currentPLFile.Palettes[selectedPaletteIndex].Colors[i, isSpecular ? 1 : 0] = cpn.result[i];
                    RefreshPalette(selectedPaletteIndex);
                    RefreshPalettePreview();
                }
        }

        private void SetPaletteAlpha(int alpha)
        {
            for (int i = 0; i < 256; i++)
                currentPLFile.Palettes[selectedPaletteIndex].Colors[i, isSpecular ? 1 : 0] = Color.FromArgb(alpha, currentPLFile.Palettes[selectedPaletteIndex].Colors[i, isSpecular ? 1 : 0]);
            RefreshPalette(selectedPaletteIndex);
            RefreshPalettePreview();
        }

        private void toolStripAlpha255_Click(object sender, EventArgs e)
        {
            SetPaletteAlpha(255);
        }

        private void toolStripAlpha0_Click(object sender, EventArgs e)
        {
            SetPaletteAlpha(0);
        }

        private void toolStripAlpha127_Click(object sender, EventArgs e)
        {
            SetPaletteAlpha(127);
        }

        private void toolStripFillBasicGradient_Click(object sender, EventArgs e)
        {
            using (GradientBasic gb = new GradientBasic())
                if (gb.ShowDialog() == DialogResult.OK)
                {
                    for (int i = 0; i < 256; i++)
                        currentPLFile.Palettes[selectedPaletteIndex].Colors[i, isSpecular ? 1 : 0] = gb.result[i];
                    RefreshPalette(selectedPaletteIndex);
                    RefreshPalettePreview();
                }
        }

        private void SetLabelColor(Color color)
        {
            labelDiffusePalettes.ForeColor = labelSpecularPalettes.ForeColor = label0.ForeColor = label1.ForeColor =
               label2.ForeColor = label3.ForeColor = label4.ForeColor = label5.ForeColor = label6.ForeColor = label7.ForeColor =
               labelB.ForeColor = color;
        }
        private void SetBackgroundColor(Color color)
        {
            pictureBoxPalettesBG.BackColor = pictureBoxPreviewBG.BackColor = trackBarColorIndex.BackColor =
                labelDiffusePalettes.BackColor = labelSpecularPalettes.BackColor = label0.BackColor = label1.BackColor =
                label2.BackColor = label3.BackColor = label4.BackColor = label5.BackColor = label6.BackColor = label7.BackColor =
                labelB.BackColor = color;
        }

        private void lightGreyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLabelColor(Color.Black);
            SetBackgroundColor(Color.LightGray);
            lightGreyToolStripMenuItem.Checked = true;
            darkGreyDefaultToolStripMenuItem.Checked = whiteToolStripMenuItem.Checked = blackToolStripMenuItem.Checked = false;
        }

        private void blackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLabelColor(Color.White);
            SetBackgroundColor(Color.Black);
            blackToolStripMenuItem.Checked = true;
            darkGreyDefaultToolStripMenuItem.Checked = whiteToolStripMenuItem.Checked = lightGreyToolStripMenuItem.Checked = false;
        }

        private void whiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLabelColor(Color.Black);
            SetBackgroundColor(Color.White);
            whiteToolStripMenuItem.Checked = true;
            darkGreyDefaultToolStripMenuItem.Checked = blackToolStripMenuItem.Checked = lightGreyToolStripMenuItem.Checked = false;
        }

        private void darkGreyDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLabelColor(Color.White);
            SetBackgroundColor(Color.FromArgb(36, 36, 36));
            darkGreyDefaultToolStripMenuItem.Checked = true;
            lightGreyToolStripMenuItem.Checked = whiteToolStripMenuItem.Checked = blackToolStripMenuItem.Checked = false;
        }

        private void paletteListEmeraldCoastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateDefaultPalettes();
        }

        private void CopyPalette()
        {
            clipboardPalette = new Color[256];
            for (int i = 0; i < 256; i++)
                clipboardPalette[i] = currentPLFile.Palettes[selectedPaletteIndex].Colors[i, isSpecular ? 1 : 0];
            toolStripPastePalette.Enabled = true;
        }

        private void PastePalette()
        {
            if (clipboardPalette == null)
                return;
            for (int i = 0; i < 256; i++)
                currentPLFile.Palettes[selectedPaletteIndex].Colors[i, isSpecular ? 1 : 0] = clipboardPalette[i];
            RefreshPalette(selectedPaletteIndex);
            RefreshPalettePreview();
        }

        private void toolStripCopyPalette_Click(object sender, EventArgs e)
        {
            CopyPalette();
        }

        private void toolStripPastePalette_Click(object sender, EventArgs e)
        {
            PastePalette();
        }

        private void gamecubeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gamecubeToolStripMenuItem.Checked = isGamecube = true;
            dreamcastToolStripMenuItem.Checked = false;
        }

        private void dreamcastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gamecubeToolStripMenuItem.Checked = isGamecube = false;
            dreamcastToolStripMenuItem.Checked = true;
        }

        private void pLToolHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/X-Hax/sa_tools/wiki/PL-Tool");
        }

        private void issueTrackerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/X-Hax/sa_tools/issues");
        }

        private void toolStripSplitButtonColor_MouseUp(object sender, MouseEventArgs e)
        {
            toolStripSplitButtonColor.ShowDropDown();
        }

        private void ReplaceCurrentColor(Color color)
        {
            currentPLFile.Palettes[selectedPaletteIndex].Colors[selectedColorIndex, isSpecular ? 1 : 0] = color;
            RefreshPalette(selectedPaletteIndex);
            RefreshPalettePreview();
        }

        private void toolStripColorReplace_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
                ReplaceCurrentColor(colorDialog1.Color);
        }

        private void toolStripColorReplaceBlack_Click(object sender, EventArgs e)
        {
            ReplaceCurrentColor(Color.Black);
        }

        private void toolStripColorReplaceWhite_Click(object sender, EventArgs e)
        {
            ReplaceCurrentColor(Color.White);
        }

        private void toolStripColorAlpha255_Click(object sender, EventArgs e)
        {
            ReplaceCurrentColor(Color.FromArgb(255, currentPLFile.Palettes[selectedPaletteIndex].Colors[selectedColorIndex, isSpecular ? 1 : 0]));
        }

        private void toolStripColorAlpha0_Click(object sender, EventArgs e)
        {
            ReplaceCurrentColor(Color.FromArgb(0, currentPLFile.Palettes[selectedPaletteIndex].Colors[selectedColorIndex, isSpecular ? 1 : 0]));
        }

        private void toolStripColorAlpha127_Click(object sender, EventArgs e)
        {
            ReplaceCurrentColor(Color.FromArgb(127, currentPLFile.Palettes[selectedPaletteIndex].Colors[selectedColorIndex, isSpecular ? 1 : 0]));
        }

        private void toolStripColorCopy_Click(object sender, EventArgs e)
        {
            clipboardColor = currentPLFile.Palettes[selectedPaletteIndex].Colors[selectedColorIndex, isSpecular ? 1 : 0];
            toolStripColorPaste.Enabled = true;
        }

        private void toolStripColorPaste_Click(object sender, EventArgs e)
        {
            ReplaceCurrentColor(clipboardColor);
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog() { Title = "Export PNG", Filter = "PNG Images|*.png", DefaultExt = "png", FileName = Path.GetFileNameWithoutExtension(currentFilename) })
                if (sfd.ShowDialog() == DialogResult.OK)
                    currentPLFile.ToPNG().Save(sfd.FileName);
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Title = "Import PNG", Filter = "PNG Images|*.png|All Files|*.*", DefaultExt = "png", FileName = Path.GetFileNameWithoutExtension(currentFilename) })
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Bitmap import = new Bitmap(ofd.FileName);
                    currentPLFile = new PLFile(import);
                    import.Dispose();
                }
            RefreshAllPalettes();
            RefreshPalettePreview();
        }

        private void SavePLFile(string filename)
        {
            File.WriteAllBytes(filename, currentPLFile.GetBytes(isGamecube));
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SavePLFile(currentFilename);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog() { Title = "Save PL File", Filter = "PL Files|PL*.BIN|All Files|*.*" })
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    SavePLFile(sfd.FileName);
                    currentFilename = sfd.FileName;
                    toolStripStatusLabelFilename.Text = Path.GetFileNameWithoutExtension(currentFilename);
                }
            
        }

        private void toolStripCreateGradientDX_Click(object sender, EventArgs e)
        {
            using (GradientDX gdx = new GradientDX(currentPLFile.Palettes[selectedPaletteIndex].GetColorList(isSpecular)))
                if (gdx.ShowDialog() == DialogResult.OK)
                {
                    for (int i = 0; i < 256; i++)
                        currentPLFile.Palettes[selectedPaletteIndex].Colors[i, isSpecular ? 1 : 0] = gdx.result[i];
                    RefreshPalette(selectedPaletteIndex);
                    RefreshPalettePreview();
                }
        }

        private void toolStripExportPalettePNG_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog() { FileName = Path.GetFileNameWithoutExtension(currentFilename) + "_" + (isSpecular ? "s" : "d") + selectedPaletteIndex.ToString(), Title = "Export PNG", Filter = "PNG Images|*.png", DefaultExt = "png" })
                if (sfd.ShowDialog() == DialogResult.OK)
                    currentPLFile.Palettes[selectedPaletteIndex].ToPNG(isSpecular).Save(sfd.FileName);
        }

        private void toolStripImportPalettePNG_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { FileName = Path.GetFileNameWithoutExtension(currentFilename) + "_" + (isSpecular ? "s" : "d") + selectedPaletteIndex.ToString(), Title = "Import PNG", Filter = "PNG Images|*.png|All Files|*.*", DefaultExt = "png" })
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Bitmap import = new Bitmap(ofd.FileName);
                    if (import.Width < 256)
                    {
                        MessageBox.Show(this, "Bitmap width must be 256 pixels.", "PL Tool Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    for (int i = 0; i < 256; i++)
                    {
                        Color orig = import.GetPixel(i, 0);
                        currentPLFile.Palettes[selectedPaletteIndex].Colors[i, isSpecular ? 1 : 0] = Color.FromArgb(orig.A, orig.R, orig.G, orig.B);
                    }
                    import.Dispose();
                    RefreshAllPalettes();
                    RefreshPalettePreview();
                }
        }

        private void sLEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SLEditor sL;
            if (currentFilename == "")
                sL = new SLEditor();
            else
                sL = new SLEditor(Path.GetFullPath(currentFilename).Replace("PL", "SL"));
            sL.Show();
        }
	}
}
