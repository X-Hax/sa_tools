using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using static VMSEditor.SA1DLC;
using ArchiveLib;
using SAModel;
using System.Drawing.Imaging;
using System.Text;
using System.Collections.Generic;
using SplitTools;

namespace VMSEditor
{
    public partial class EditorDLC : Form
    {
        string currentFilename = "";
        string currentFullPath = "";
        int currentLanguage = 0;
        VMS_DLC meta = new VMS_DLC();
        VMS_DLC metaBackup;
        PuyoFile puyo;

        public EditorDLC()
        {
            Application.ThreadException += Application_ThreadException;
            InitializeComponent();
            if (Program.args.Length > 0)
                if (File.Exists(Program.args[0]))
                    LoadVMS_DLC(Program.args[0]);
        }

        void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            string errDesc = "DLC Tool has crashed with the following error:\n" + e.Exception.GetType().Name + ".\n\n" +
                "If you wish to report a bug, please include the following in your report:";
			SAModel.SAEditorCommon.ErrorDialog report = new SAModel.SAEditorCommon.ErrorDialog("DLC Tool", errDesc, e.Exception.ToString());
            DialogResult dgresult = report.ShowDialog();
            switch (dgresult)
            {
                case DialogResult.Abort:
                case DialogResult.OK:
                    Application.Exit();
                    break;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private Bitmap ResizeBitmap(Bitmap src, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics gfx = Graphics.FromImage(result))
            {
                gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                gfx.DrawImage(src, 0, 0, width, height);
            }
            return result;
        }

        private Bitmap ScalePreview(Bitmap image, PictureBox pbox, bool zoom)
        {
            if (!zoom)
                return image;
            else
                return ResizeBitmap(image, pbox.Width, pbox.Height);
        }

        private void UpdateTextures()
        {
            if (meta.TextureData == null || meta.TextureData.Length == 0)
            {
                labelTextureSectionSize.Text = "Section size: 0 bytes";
                labelNumberTextures.Text = "Number of textures: 0";
                labelTextureDimensions.Text = "";
                labelTextureSize.Text = "";
                listBoxTextures.Items.Clear();
                listBoxTextures.SelectedIndex = -1;
                pictureBoxTexture.Image = new Bitmap(32, 32);
                return;
            }
            puyo = new PuyoFile(meta.TextureData);
            listBoxTextures.Items.Clear();
            int index = 0;
            foreach (PVMEntry pvme in puyo.Entries)
            {
                listBoxTextures.Items.Add(index.ToString() + ": " + pvme.Name);
                index++;
            }
            labelTextureSectionSize.Text = "Section size: " + meta.TextureData.Length.ToString() + " bytes";
            labelNumberTextures.Text = "Number of textures: " + puyo.Entries.Count.ToString();
            listBoxTextures.SelectedIndex = 0;
        }

        public void UpdateGeneralInfo()
        {
            textBoxTitle.Text = meta.Title;
            textBoxDescription.Text = meta.Description;
            textBoxAuthor.Text = meta.AppName;
            numericUpDownDLCid.Value = (int)meta.Identifier;
            checkBoxSonic.Checked = meta.EnableSonic;
            checkBoxTails.Checked = meta.EnableTails;
            checkBoxKnuckles.Checked = meta.EnableKnuckles;
            checkBoxGamma.Checked = meta.EnableGamma;
            checkBoxAmy.Checked = meta.EnableAmy;
            checkBoxBig.Checked = meta.EnableBig;
            checkBoxUnknown1.Checked = meta.EnableWhatever1;
            checkBoxUnknown2.Checked = meta.EnableWhatever2;
            pictureBoxDLCicon.Image = ScalePreview(meta.Icon, pictureBoxDLCicon, checkBoxZoom.Checked);
            int region_index = -1;
            switch (meta.Region)
            {
                case DLCRegionLocks.Disabled:
                    region_index = 0;
                    break;
                case DLCRegionLocks.Japan:
                    region_index = 1;
                    break;
                case DLCRegionLocks.US:
                    region_index = 2;
                    break;
                case DLCRegionLocks.Europe:
                    region_index = 3;
                    break;
                case DLCRegionLocks.All:
                    region_index = 4;
                    break;
            }
            comboBoxRegionLock.SelectedIndex = region_index;
        }

        private void UpdateSize()
        {
            toolStripStatusSize.Text = "(" + meta.GetBytes().Length.ToString() + " bytes)";
        }

        public void LoadVMS_DLC(string filename)
        {
            byte[] file = File.ReadAllBytes(filename);
            // Get metadata and icon
            meta = new VMS_DLC(file);
            metaBackup = new VMS_DLC(meta);
            meta.Icon = GetIconFromFile(file);
            // Get PVM pointer
            uint pvm_pointer = BitConverter.ToUInt32(file, 0x290);
            int pvm_value = BitConverter.ToInt32(file, 0x294);
            int pvm_count = BitConverter.ToInt32(file, 0x298);
            if (pvm_value != 0)
                Console.WriteLine("PVM at {0}, number of textures {1}", pvm_pointer.ToString("X"), pvm_count);
            checkBoxEnableTextures.Checked = pvm_value != 0;
            // Get MLT pointer
            uint mlt_pointer = BitConverter.ToUInt32(file, 0x29C);
            int mlt_value = BitConverter.ToInt32(file, 0x2A0);
            if (mlt_value != 0)
                Console.WriteLine("MLT at {0}", mlt_pointer.ToString("X"));
            // Get PRS pointer
            uint prs_pointer = BitConverter.ToUInt32(file, 0x2A4);
            int prs_value = BitConverter.ToInt32(file, 0x2A8);
            if (prs_value != 0)
                Console.WriteLine("PRS at {0}", prs_pointer.ToString("X"));
            // Checksum
            uint crc = CalculateChecksum(ref file, 0x2C0, file.Length);
            Console.WriteLine("Checksum file / calculated: {0} ({1}) / {2} ({3})", BitConverter.ToInt32(file, 0x2AC).ToString("X"), BitConverter.ToInt32(file, 0x2AC), crc.ToString("X"), (int)crc);
            // Retrieve sections
            // Get PVM
            int pvm_size = (int)mlt_pointer - (int)pvm_pointer;
            if (pvm_size > 0 && pvm_size + pvm_pointer <= file.Length)
            {
                meta.TextureData = new byte[pvm_size];
                Array.Copy(file, pvm_pointer, meta.TextureData, 0, pvm_size);
                UpdateTextures();
            }
            // Get MLT
            int mlt_size = (int)prs_pointer - (int)mlt_pointer;
            if (mlt_size > 0 && mlt_pointer + mlt_size <= file.Length)
            {
                buttonLoadMLT.Enabled = buttonSaveMLT.Enabled = checkBoxEnableSound.Checked = true;
                meta.SoundData = new byte[mlt_size];
                Array.Copy(file, mlt_pointer, meta.SoundData, 0, mlt_size);
                //File.WriteAllBytes(Path.Combine(dir, fname + ".mlt"), mltdata);
                labelSoundSectionSize.Text = "Section size: " + meta.SoundData.Length.ToString() + " bytes";
            }
            uint sectionsize = BitConverter.ToUInt32(file, 0x48);
            int text_count = BitConverter.ToInt32(file, 0x28C);
            int item_count = BitConverter.ToInt32(file, 0x284);
            int item_size = (item_count * 30 + 12); // 12-byte header
            do
            {
                item_size++;
            }
            while (item_size % 16 != 0);
            int prs_size = 0;
            if (prs_value != 0) 
                prs_size = file.Length - (int)prs_pointer;
            Console.WriteLine("Headerless size {0}, item size {1}, text size {2}, PVM size {3}, MLT size {4}, PRS size {5}", sectionsize, item_size, text_count * 64, pvm_size, mlt_size, prs_size);
            // Get Model
            if (prs_size > 0 && prs_pointer + prs_size <= file.Length)
            {
                byte[] prsdata = new byte[prs_size];
                //Console.WriteLine("Copy from array of size {0} from {1} to array size {2}", file.Length, prs_pointer, prsdata.Length);
                Array.Copy(file, prs_pointer, prsdata, 0, prs_size);
                prsdata = FraGag.Compression.Prs.Decompress(prsdata);
                // Model pointer
                uint modelpointer = BitConverter.ToUInt32(prsdata, 0) - 0xCCA4000;
                //Console.WriteLine("Model pointer: {0}", modelpointer.ToString("X"));
                labelModelSectionSize.Text = "Section size: " + prsdata.Length.ToString() + " bytes";
                try
                {
                    NJS_OBJECT mdl = new NJS_OBJECT(prsdata, (int)modelpointer, 0xCCA4000, ModelFormat.Basic, null);
                    labelModelInfo.Text = GetModelInfo(mdl);
                }
                catch (Exception)
                {
                    labelModelInfo.Text = "Error getting model information.";
                }
                checkBoxEnableModel.Checked = true;
            }
            UpdateGeneralInfo();
            currentFilename = toolStripStatusFile.Text = Path.GetFileName(filename);
            currentFullPath = filename;
            saveToolStripMenuItem.Enabled = true;
            // Add data for empty strings
            for (int i = 0; i < 16; i++)
            {
                if (meta.JapaneseStrings[i] == null)
                    meta.JapaneseStrings[i] = "";
                if (meta.EnglishStrings[i] == null)
                    meta.EnglishStrings[i] = "";
                if (meta.FrenchStrings[i] == null)
                    meta.FrenchStrings[i] = "";
                if (meta.GermanStrings[i] == null)
                    meta.GermanStrings[i] = "";
                if (meta.SpanishStrings[i] == null)
                    meta.SpanishStrings[i] = "";
            }
            UpdateMessage();
            UpdateSize();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog od = new OpenFileDialog() { DefaultExt = "vms", Filter = "VMS Files|*.vms|All Files|*.*" })
                if (od.ShowDialog(this) == DialogResult.OK)
                    LoadVMS_DLC(od.FileName);
        }

        private void checkBoxZoom_Click(object sender, EventArgs e)
        {
            pictureBoxDLCicon.Image = ScalePreview(meta.Icon, pictureBoxDLCicon, checkBoxZoom.Checked);
            toolStripStatusHint.Text = "This setting only affects the preview.";
        }

        private void buttonLoadIcon_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog od = new OpenFileDialog() { DefaultExt = "bmp", Filter = "Bitmap Files|*.bmp;*.png;*.jpg;*.gif|All Files|*.*" })
                if (od.ShowDialog(this) == DialogResult.OK)
                {
                    Bitmap bitmap = new Bitmap(od.FileName);
                    if (bitmap.PixelFormat != PixelFormat.Format4bppIndexed)
                    {
                        var quantizer = new PnnQuant.PnnQuantizer();
                        bitmap = quantizer.QuantizeImage(bitmap, PixelFormat.Format4bppIndexed, 16, true);
                    }
                    meta.Icon = bitmap;
                    pictureBoxDLCicon.Image = ScalePreview(meta.Icon, pictureBoxDLCicon, checkBoxZoom.Checked);
                }
        }

        private void UpdateMessage()
        {
            textBoxMessageTextJP.Text = meta.JapaneseStrings[(int)numericUpDownMessageID.Value].Replace("\n", "\r\n");
            textBoxMessageTextEN.Text = meta.EnglishStrings[(int)numericUpDownMessageID.Value].Replace("\n", "\r\n");
            textBoxMessageTextFR.Text = meta.FrenchStrings[(int)numericUpDownMessageID.Value].Replace("\n", "\r\n");
            textBoxMessageTextGE.Text = meta.GermanStrings[(int)numericUpDownMessageID.Value].Replace("\n", "\r\n");
            textBoxMessageTextSP.Text = meta.SpanishStrings[(int)numericUpDownMessageID.Value].Replace("\n", "\r\n");
        }

        private void numericUpDownMessageID_ValueChanged(object sender, EventArgs e)
        {
            UpdateMessage();
        }

        private void UpdateTexturePreview()
        {
            if (listBoxTextures.SelectedIndex == -1)
                return;
            Bitmap texture = puyo.Entries[listBoxTextures.SelectedIndex].GetBitmap();
            if (checkBoxRectangleTexture.Checked)
                pictureBoxTexture.Image = ScalePreview(ResizeBitmap(texture, texture.Width * 2, texture.Height), pictureBoxTexture, checkBoxZoomTexture.Checked);
            else
                pictureBoxTexture.Image = ScalePreview(texture, pictureBoxTexture, checkBoxZoomTexture.Checked);
            labelTextureDimensions.Text = texture.Width.ToString() + "x" + texture.Height.ToString();
            labelTextureSize.Text = puyo.Entries[listBoxTextures.SelectedIndex].Data.Length.ToString() + " bytes";
        }

        private void listBoxTextures_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTexturePreview();
        }

        private void checkBoxZoomTexture_Click(object sender, EventArgs e)
        {
            UpdateTexturePreview();
            toolStripStatusHint.Text = "This setting only affects the preview.";
        }

        private void checkBoxEnableSound_Click(object sender, EventArgs e)
        {
            buttonLoadMLT.Enabled = buttonSaveMLT.Enabled = checkBoxEnableSound.Checked;
            if (checkBoxEnableSound.Checked)
            {
                if (meta.SoundData == null)
                    meta.SoundData = new byte[0];
            }
            else
                meta.SoundData = null;
            labelSoundSectionSize.Text = "Section size: " + (checkBoxEnableSound.Checked ? meta.SoundData.Length.ToString() : "0") + " bytes";
        }

        private void UpdateSound()
        {
            checkBoxEnableSound.Checked = !(meta.SoundData == null || meta.SoundData.Length == 0);
            buttonLoadMLT.Enabled = buttonSaveMLT.Enabled = checkBoxEnableSound.Checked;
            labelSoundSectionSize.Text = "Section size: " + (checkBoxEnableSound.Checked ? meta.SoundData.Length.ToString() : "0") + " bytes";
            UpdateSize();
        }

        private void buttonLoadMLT_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog od = new OpenFileDialog() { DefaultExt = "mlt", Filter = "Multi-Unit Files|*.mlt|All Files|*.*" })
                if (od.ShowDialog(this) == DialogResult.OK)
                {
                    meta.SoundData = File.ReadAllBytes(od.FileName);
                    UpdateSound();
                }
        }

        private void dLCToolManualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/X-Hax/sa_tools/wiki/Dreamcast-DLC-Tool");
        }

        private void gitHubIssueTrackerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/X-Hax/sa_tools/issues");
        }

        private void checkBoxEnableTextures_Click(object sender, EventArgs e)
        {
            buttonLoadPVM.Enabled = buttonSavePVM.Enabled = checkBoxEnableTextures.Checked;
            meta.TextureData = new byte[0];
            UpdateTextures();
        }

        private void buttonEditObjects_Click(object sender, EventArgs e)
        {
            VMS_DLC original = new VMS_DLC(meta);
            EditorDLCObjectEditor obje = new EditorDLCObjectEditor(meta, puyo);
            DialogResult result = obje.ShowDialog(this);
            switch (result)
            {
                case DialogResult.Yes:
                    meta = obje.editorMeta;
                    break;
                default:
                    meta = new VMS_DLC(original);
                    break;
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sv = new SaveFileDialog() { FileName = Path.ChangeExtension(Path.GetFileName(currentFilename), "vms"), Title = "Save VMS File", Filter = "VMS Files|*.vms|All Files|*.*", DefaultExt = "vms" })
            {
                if (sv.ShowDialog() == DialogResult.OK)
                {
                    byte[] result = meta.GetBytes();
                    if (!encryptDLCFilesToolStripMenuItem.Checked)
                        ProcessVMS(ref result);
                    File.WriteAllBytes(sv.FileName, result);
                    if (createVMIFileToolStripMenuItem.Checked)
                        File.WriteAllBytes(Path.ChangeExtension(sv.FileName, ".VMI"), new VMIFile(meta, Path.GetFileNameWithoutExtension(sv.FileName)).GetBytes());
                    currentFullPath = sv.FileName;
                    currentFilename = Path.GetFileName(sv.FileName);
                }
            }
        }

        private string SetMessage(string controlText)
        {
            if (controlText == "")
                return "";
            string copytext = controlText.Replace("\r\n", "\n");
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < copytext.Length; i++)
            {
                sb.Append(copytext[i]);
            }
            return sb.ToString();
        }

        private void textBoxMessageTextJP_TextChanged(object sender, EventArgs e)
        {
            meta.JapaneseStrings[(int)numericUpDownMessageID.Value] = SetMessage(textBoxMessageTextJP.Text);
        }

        private void textBoxMessageTextEN_TextChanged(object sender, EventArgs e)
        {
            meta.EnglishStrings[(int)numericUpDownMessageID.Value] = SetMessage(textBoxMessageTextEN.Text);
        }

        private void textBoxMessageTextFR_TextChanged(object sender, EventArgs e)
        {
            meta.FrenchStrings[(int)numericUpDownMessageID.Value] = SetMessage(textBoxMessageTextFR.Text);
        }

        private void textBoxMessageTextGE_TextChanged(object sender, EventArgs e)
        {
            meta.GermanStrings[(int)numericUpDownMessageID.Value] = SetMessage(textBoxMessageTextGE.Text);
        }

        private void textBoxMessageTextSP_TextChanged(object sender, EventArgs e)
        {
            meta.SpanishStrings[(int)numericUpDownMessageID.Value] = SetMessage(textBoxMessageTextSP.Text);
        }

        public void UpdateModel()
        {
            if (meta.ModelData == null || meta.ModelData.Length == 0)
            {
                labelModelSectionSize.Text = "Section size: 0 bytes";
                labelModelInfo.Text = "";
                checkBoxEnableModel.Checked = buttonLoadModel.Enabled = buttonSaveModel.Enabled = buttonImportRawModel.Enabled = buttonSaveRawModel.Enabled = false;
                return;
            }
            labelModelSectionSize.Text = "Section size: " + meta.ModelData.Length.ToString() + " bytes";
            byte[] modeldata = FraGag.Compression.Prs.Decompress(meta.ModelData);
            uint modelpointer = BitConverter.ToUInt32(modeldata, 0) - 0xCCA4000;
            try
            {
                NJS_OBJECT mdl = new NJS_OBJECT(modeldata, (int)modelpointer, 0xCCA4000, ModelFormat.Basic, null);
                labelModelInfo.Text = GetModelInfo(mdl);
            }
            catch (Exception)
            {
                labelModelInfo.Text = "Error getting model information.";
            }
            buttonLoadModel.Enabled = buttonSaveModel.Enabled = buttonImportRawModel.Enabled = buttonSaveRawModel.Enabled = true;
            UpdateSize();
        }

        private void buttonLoadModel_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog op = new OpenFileDialog() { FileName = Path.ChangeExtension(Path.GetFileName(currentFilename), ".sa1mdl"), Title = "Select a Model", Filter = "SA1MDL Files|*.sa1mdl|All Files|*.*", DefaultExt = "sa1mdl" })
            {
                if (op.ShowDialog() == DialogResult.OK)
                {
                    meta.ModelData = ConvertModel(op.FileName);
                    checkBoxEnableModel.Checked = true;
                    UpdateModel();
                }
            }
        }

        private void buttonImportRawModel_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog op = new OpenFileDialog() { FileName = Path.ChangeExtension(Path.GetFileName(currentFilename), "prs"), Title = "Load a Binary Model", Filter = "PRS and Binary Files|*.prs;*.bin|All Files|*.*", DefaultExt = "prs" })
            {
                if (op.ShowDialog() == DialogResult.OK)
                {
                    List<byte> result = new List<byte>();
                    byte[] import = File.ReadAllBytes(op.FileName);
                    // If the imported file is not a PRS, compress it
                    if (Path.GetExtension(op.FileName).ToLowerInvariant() != ".prs")
                        import = FraGag.Compression.Prs.Compress(import);
                    result.AddRange(import);
                    if (result.Count % 16 != 0)
                    {
                        do
                        {
                            result.Add(0);
                        }
                        while (result.Count % 16 != 0);
                    }
                    meta.ModelData = result.ToArray();
                    UpdateModel();
                }
            }
        }

        private void buttonSaveRawModel_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sv = new SaveFileDialog() { FileName = Path.ChangeExtension(Path.GetFileName(currentFilename), "prs"), Title = "Export a Binary Model", Filter = "PRS Files|*.prs|Binary Files|*.bin|All Files|*.*", DefaultExt = "prs" })
            {
                if (sv.ShowDialog() == DialogResult.OK)
                {
                    byte[] export;
                    if (Path.GetExtension(sv.FileName).ToLowerInvariant() == ".prs")
                        export = meta.ModelData;
                    else
                        export = FraGag.Compression.Prs.Decompress(meta.ModelData);
                    File.WriteAllBytes(sv.FileName, export);
                }
            }
        }

        private void buttonSaveModel_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sv = new SaveFileDialog() { FileName = Path.ChangeExtension(Path.GetFileName(currentFilename), ".sa1mdl"), Title = "Save Model", Filter = "SA1MDL Files|*.sa1mdl|All Files|*.*", DefaultExt = "sa1mdl" })
            {
                if (sv.ShowDialog() == DialogResult.OK)
                {
                    byte[] modeldata = FraGag.Compression.Prs.Decompress(meta.ModelData);
                    uint modelpointer = BitConverter.ToUInt32(modeldata, 0) - 0xCCA4000;
                    NJS_OBJECT mdl = new NJS_OBJECT(modeldata, (int)modelpointer, 0xCCA4000, ModelFormat.Basic, null);
                    ModelFile.CreateFile(sv.FileName, mdl, null, null, null, new System.Collections.Generic.Dictionary<uint, byte[]>(), ModelFormat.Basic);
                }
            }
        }

        private void buttonSaveIcon_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sv = new SaveFileDialog() { FileName = Path.ChangeExtension(Path.GetFileName(currentFilename), ".bmp"), Title = "Save Icon", Filter = "Bitmap Files|*.bmp|All Files|*.*", DefaultExt = "bmp" })
            {
                if (sv.ShowDialog() == DialogResult.OK)
                {
                    meta.Icon.Save(sv.FileName);
                }
            }
        }

        private void textBoxTitle_TextChanged(object sender, EventArgs e)
        {
            meta.Title = textBoxTitle.Text;
        }

        private void textBoxDescription_TextChanged(object sender, EventArgs e)
        {
            meta.Description = textBoxDescription.Text;
        }

        private void textBoxAuthor_TextChanged(object sender, EventArgs e)
        {
            meta.AppName = textBoxAuthor.Text;
        }

        private void numericUpDownDLCid_ValueChanged(object sender, EventArgs e)
        {
            meta.Identifier = (uint)numericUpDownDLCid.Value;
        }

        private void comboBoxRegionLock_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxRegionLock.SelectedIndex)
            {
                case 0:
                    meta.Region = DLCRegionLocks.Disabled;
                    break;
                case 1:
                    meta.Region = DLCRegionLocks.Japan;
                    break;
                case 2:
                    meta.Region = DLCRegionLocks.US;
                    break;
                case 3:
                    meta.Region = DLCRegionLocks.Europe;
                    break;
                case 4:
                    meta.Region = DLCRegionLocks.All;
                    break;
            }
        }

        private void buttonSaveMLT_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sv = new SaveFileDialog() { FileName = Path.ChangeExtension(Path.GetFileName(currentFilename), ".mlt"), Title = "Save Multi-Unit", Filter = "Multi-Unit Files|*.mlt|All Files|*.*", DefaultExt = "mlt" })
            {
                if (sv.ShowDialog() == DialogResult.OK)
                    File.WriteAllBytes(sv.FileName, meta.SoundData);
            }
        }

        private void buttonTextReload_Click(object sender, EventArgs e)
        {
            if (metaBackup == null)
            {
                MessageBox.Show(this, "Original data does not exist.", "DLC Tool Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            switch (currentLanguage)
            {
                case 0:
                    textBoxMessageTextJP.Text = metaBackup.JapaneseStrings[(int)numericUpDownMessageID.Value];
                    break;
                case 1:
                    textBoxMessageTextEN.Text = metaBackup.EnglishStrings[(int)numericUpDownMessageID.Value];
                    break;
                case 2:
                    textBoxMessageTextFR.Text = metaBackup.FrenchStrings[(int)numericUpDownMessageID.Value];
                    break;
                case 3:
                    textBoxMessageTextGE.Text = metaBackup.GermanStrings[(int)numericUpDownMessageID.Value];
                    break;
                case 4:
                    textBoxMessageTextSP.Text = metaBackup.SpanishStrings[(int)numericUpDownMessageID.Value];
                    break;
            }
        }

        private void buttonTextClear_Click(object sender, EventArgs e)
        {
            switch (currentLanguage)
            {
                case 0:
                    textBoxMessageTextJP.Text = "";
                    break;
                case 1:
                    textBoxMessageTextEN.Text = "";
                    break;
                case 2:
                    textBoxMessageTextFR.Text = "";
                    break;
                case 3:
                    textBoxMessageTextGE.Text = "";
                    break;
                case 4:
                    textBoxMessageTextSP.Text = "";
                    break;
            }
        }

        private void buttonTextClearAll_Click(object sender, EventArgs e)
        {
            textBoxMessageTextJP.Text = "";
            textBoxMessageTextEN.Text = "";
            textBoxMessageTextFR.Text = "";
            textBoxMessageTextGE.Text = "";
            textBoxMessageTextSP.Text = "";
        }

		private void buttonLoadPVM_Click(object sender, EventArgs e)
		{
            using (OpenFileDialog op = new OpenFileDialog() { FileName = Path.ChangeExtension(Path.GetFileName(currentFilename), "pvm"), Title = "Load a PVM File", Filter = "PVM Files|*.pvm|All Files|*.*", DefaultExt = "pvm" })
            {
                if (op.ShowDialog() == DialogResult.OK)
                {
                    meta.TextureData = File.ReadAllBytes(op.FileName);
                    UpdateTextures();
                    UpdateSize();
                }
            }
        }

        private void buttonSavePVM_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sv = new SaveFileDialog() { FileName = Path.ChangeExtension(Path.GetFileName(currentFilename), "pvm"), Title = "Save a PVM File", Filter = "PVM Files|*.pvm|All Files|*.*", DefaultExt = "pvm" })
            {
                if (sv.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(sv.FileName, meta.TextureData);
                }
            }
        }

		private void checkBoxSonic_CheckedChanged(object sender, EventArgs e)
		{
            meta.EnableSonic = checkBoxSonic.Checked;
		}

		private void checkBoxTails_CheckedChanged(object sender, EventArgs e)
		{
            meta.EnableTails = checkBoxTails.Checked;
        }

		private void checkBoxKnuckles_CheckedChanged(object sender, EventArgs e)
		{
            meta.EnableKnuckles = checkBoxKnuckles.Checked;
        }

		private void checkBoxAmy_CheckedChanged(object sender, EventArgs e)
		{
            meta.EnableAmy = checkBoxAmy.Checked;
		}

		private void checkBoxBig_CheckedChanged(object sender, EventArgs e)
		{
            meta.EnableBig = checkBoxBig.Checked;
        }

		private void checkBoxGamma_CheckedChanged(object sender, EventArgs e)
		{
            meta.EnableGamma = checkBoxGamma.Checked;
        }

		private void checkBoxUnknown1_CheckedChanged(object sender, EventArgs e)
		{
            meta.EnableWhatever1 = checkBoxUnknown1.Checked;
        }

		private void checkBoxUnknown2_CheckedChanged(object sender, EventArgs e)
		{
            meta.EnableWhatever2 = checkBoxUnknown2.Checked;
        }

		private void textBoxMessageTextJP_Click(object sender, EventArgs e)
		{
            currentLanguage = 0;
            toolStripStatusHint.Text = "Enter Japanese message " + numericUpDownMessageID.Value.ToString() + ".";
        }

		private void textBoxMessageTextEN_Click(object sender, EventArgs e)
		{
            currentLanguage = 1;
            toolStripStatusHint.Text = "Enter English message " + numericUpDownMessageID.Value.ToString() + ".";
        }

		private void textBoxMessageTextFR_Click(object sender, EventArgs e)
		{
            currentLanguage = 2;
            toolStripStatusHint.Text = "Enter French message " + numericUpDownMessageID.Value.ToString() + ".";
        }

		private void textBoxMessageTextGE_Click(object sender, EventArgs e)
		{
            currentLanguage = 3;
            toolStripStatusHint.Text = "Enter German message " + numericUpDownMessageID.Value.ToString() + ".";
        }

		private void textBoxMessageTextSP_Click(object sender, EventArgs e)
		{
            currentLanguage = 4;
            toolStripStatusHint.Text = "Enter Spanish message " + numericUpDownMessageID.Value.ToString() + ".";
        }

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
            meta = new VMS_DLC();
            meta.Icon = new Bitmap(32, 32);
            metaBackup = new VMS_DLC(meta);
            UpdateGeneralInfo();
            UpdateMessage();
            UpdateModel();
            UpdateTextures();
            UpdateSound();
            UpdateSize();
        }

		private void checkBoxEnableModel_Click(object sender, EventArgs e)
		{
            buttonLoadModel.Enabled = buttonSaveModel.Enabled = buttonImportRawModel.Enabled = buttonSaveRawModel.Enabled = checkBoxEnableModel.Checked;
        }

        private void ExportFolder(string dir)
        {
            Directory.CreateDirectory(dir);
            string fname = Path.GetFileName(dir);
            IniSerializer.Serialize(meta, Path.Combine(dir, fname + ".ini"));
            meta.Icon.Save(Path.Combine(dir, fname + ".bmp"));
            if (meta.TextureData != null && meta.TextureData.Length > 0)
                File.WriteAllBytes(Path.Combine(dir, fname + ".pvm"), meta.TextureData);
            if (meta.SoundData != null && meta.SoundData.Length > 0)
                File.WriteAllBytes(Path.Combine(dir, fname + ".mlt"), meta.SoundData);
            if (meta.ModelData != null && meta.ModelData.Length > 0)
            {
                byte[] prsdata = FraGag.Compression.Prs.Decompress(meta.ModelData);
                uint modelpointer = BitConverter.ToUInt32(prsdata, 0) - 0xCCA4000;            
                NJS_OBJECT mdl = new NJS_OBJECT(prsdata, (int)modelpointer, 0xCCA4000, ModelFormat.Basic, null);
                ModelFile.CreateFile(Path.Combine(dir, fname + ".sa1mdl"), mdl, null, null, null, null, ModelFormat.Basic);
                if (exportBinaryDataToolStripMenuItem.Checked)
                {
                    File.WriteAllBytes(Path.Combine(dir, fname + ".prs"), meta.ModelData);
                    File.WriteAllBytes(Path.Combine(dir, fname + ".bin"), prsdata);
                }
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog op = new OpenFileDialog() { FileName = Path.ChangeExtension(Path.GetFileName(currentFilename), "ini"), Title = "Import an INI File", Filter = "INI Files|*.ini|All Files|*.*", DefaultExt = "ini" })
            {
                if (op.ShowDialog() == DialogResult.OK)
                {
                    meta = VMS_DLC.LoadFile(op.FileName);
                    // Add data for empty strings
                    List<string> StringsJ = new List<string>(16);
                    List<string> StringsE = new List<string>(16);
                    List<string> StringsF = new List<string>(16);
                    List<string> StringsG = new List<string>(16);
                    List<string> StringsS = new List<string>(16);
                    for (int i = 0; i < meta.JapaneseStrings.Length; i++)
                    {
                        StringsJ.Add(meta.JapaneseStrings[i]);
                        StringsE.Add(meta.EnglishStrings[i]);
                        StringsF.Add(meta.FrenchStrings[i]);
                        StringsG.Add(meta.GermanStrings[i]);
                        StringsS.Add(meta.SpanishStrings[i]);
                    }
                    if (StringsJ.Count < 16)
                    {
                        do
                        {
                            StringsJ.Add("");
                            StringsE.Add("");
                            StringsF.Add("");
                            StringsG.Add("");
                            StringsS.Add("");
                        }
                        while (StringsJ.Count < 16);
                    }
                    meta.JapaneseStrings = StringsJ.ToArray();
                    meta.EnglishStrings = StringsE.ToArray();
                    meta.FrenchStrings = StringsF.ToArray();
                    meta.GermanStrings = StringsG.ToArray();
                    meta.SpanishStrings = StringsS.ToArray();
                    // Add data for empty sections
                    if (meta.TextureData == null)
                        meta.TextureData = new byte[0];
                    if (meta.SoundData == null)
                        meta.SoundData = new byte[0];
                    if (meta.ModelData == null)
                        meta.ModelData = new byte[0];
                    UpdateGeneralInfo();
                    UpdateMessage();
                    UpdateModel();
                    UpdateTextures();
                    UpdateSound();
                    UpdateSize();
                    currentFilename = toolStripStatusFile.Text = Path.ChangeExtension(Path.GetFileName(op.FileName), ".VMS");
                    currentFullPath = Path.ChangeExtension(op.FileName, ".VMS");
                }
            }
        }

		private void exportToolStripMenuItem_Click(object sender, EventArgs e)
		{
            using (SaveFileDialog sv = new SaveFileDialog() { FileName = Path.GetFileNameWithoutExtension(currentFilename), Title = "Export Folder", Filter = "Folder|", DefaultExt = "" })
            {
                if (sv.ShowDialog() == DialogResult.OK)
                {
                    ExportFolder(sv.FileName);
                }
            }
        }

		private void checkBoxRectangleTexture_Click(object sender, EventArgs e)
		{
            UpdateTexturePreview();
            toolStripStatusHint.Text = "This setting only affects the preview.";
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentFullPath == "")
            {
                MessageBox.Show(this, "Invalid path.", "DLC Tool Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            File.WriteAllBytes(currentFullPath, meta.GetBytes());
            if (createVMIFileToolStripMenuItem.Checked)
                File.WriteAllBytes(Path.ChangeExtension(currentFullPath, ".VMI"), new VMIFile(meta, Path.GetFileNameWithoutExtension(currentFullPath)).GetBytes());
        }

		private void textBoxTitle_Click(object sender, EventArgs e)
		{
            toolStripStatusHint.Text = "Set DLC title that will appear in the Dreamcast's 'File' menu.";
		}

		private void textBoxDescription_Click(object sender, EventArgs e)
		{
            toolStripStatusHint.Text = "Set DLC description that will appear in the Dreamcast's 'File' menu.";
        }

		private void textBoxAuthor_Click(object sender, EventArgs e)
		{
            toolStripStatusHint.Text = "Copyright information.";
        }

		private void numericUpDownDLCid_Click(object sender, EventArgs e)
		{
            toolStripStatusHint.Text = "Unique ID. Values under 200 force the language to Japanese.";
        }

		private void comboBoxRegionLock_Click(object sender, EventArgs e)
		{
            toolStripStatusHint.Text = "Restrict the DLC to certain versions of the game.";
        }

		private void numericUpDownMessageID_Click(object sender, EventArgs e)
		{
            toolStripStatusHint.Text = "There can be up to 16 messages per language.";
        }

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
            toolStripStatusHint.Text = "";
        }

		private void checkBoxUnknown1_Click(object sender, EventArgs e)
		{
            toolStripStatusHint.Text = "Unknown.";
        }

		private void checkBoxUnknown2_Click(object sender, EventArgs e)
		{
            toolStripStatusHint.Text = "Unknown.";
        }

		private void encryptVMSFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
            using (OpenFileDialog od = new OpenFileDialog() { DefaultExt = "vms", Filter = "VMS Files|*.vms|All Files|*.*" })
                if (od.ShowDialog(this) == DialogResult.OK)
                {
                    byte[] data = File.ReadAllBytes(od.FileName);
                    ProcessVMS(ref data);
                    File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(od.FileName), Path.GetFileNameWithoutExtension(od.FileName)) + "_dec.vms", data);
                }
        }

		private void encryptRawDataToolStripMenuItem_Click(object sender, EventArgs e)
		{
            using (OpenFileDialog od = new OpenFileDialog() { DefaultExt = "bin", Filter = "All Files|*.*" })
                if (od.ShowDialog(this) == DialogResult.OK)
                {
                    byte[] data = File.ReadAllBytes(od.FileName);
                    VMSFile.DecryptData(ref data);
                    File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(od.FileName), Path.GetFileNameWithoutExtension(od.FileName)) + "_dec.bin", data);
                }
        }

		private void checkBoxSonic_Click(object sender, EventArgs e)
		{
            toolStripStatusHint.Text = "";
		}

		private void EditorDLC_FormClosing(object sender, FormClosingEventArgs e)
		{
            Application.Exit();
		}
	}
}
