using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PuyoTools.Modules.Archive;
using VrSharp.Gvr;
using VrSharp.Pvr;
using ArchiveLib;
using static ArchiveLib.GenericArchive;
using SonicRetro.SAModel.SAEditorCommon;

namespace TextureEditor
{
    public partial class MainForm : Form
    {
        readonly Properties.Settings Settings = Properties.Settings.Default;
        bool unsaved;
        TextureFormat currentFormat;
        string archiveFilename;
        List<TextureInfo> textures = new List<TextureInfo>();
        bool suppress = false;

        public MainForm()
        {
            Application.ThreadException += Application_ThreadException;
            InitializeComponent();
        }

        void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            string log = "Texture Editor: New log entry on " + DateTime.Now.ToString("G") + System.Environment.NewLine + 
                "Build date:" +  File.GetLastWriteTimeUtc(Application.ExecutablePath).ToString(System.Globalization.CultureInfo.InvariantCulture) + 
                System.Environment.NewLine + e.Exception.ToString();
            string errDesc = "Texture Editor has crashed with the following error:\n" + e.Exception.GetType().Name + ".\n\n" +
                "If you wish to report a bug, please include the following in your report:";
            ErrorDialog report = new ErrorDialog("Texture Editor", errDesc, log);
            DialogResult dgresult = report.ShowDialog();
            switch (dgresult)
            {
                case DialogResult.OK:
                case DialogResult.Abort:
                    Application.Exit();
                    break;
                case DialogResult.Cancel:
                    listBox1.EndUpdate();
                    suppress = false;
                    UpdateTextureCount();
                    UpdateTextureInformation();
                    break;
            }
        }

        private void UpdateMRUList(string filename)
        {
            if (Settings.MRUList.Count > 10)
            {
                for (int i = 9; i < Settings.MRUList.Count; i++)
                {
                    Settings.MRUList.RemoveAt(i);
                }
            }
            archiveFilename = filename;
            if (Settings.MRUList.Contains(filename))
            {
                int i = Settings.MRUList.IndexOf(filename);
                Settings.MRUList.RemoveAt(i);
                recentFilesToolStripMenuItem.DropDownItems.RemoveAt(i);
            }
            Settings.MRUList.Insert(0, filename);
            recentFilesToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(filename.Replace("&", "&&")));
            Text = currentFormat.ToString() + " Editor - " + filename;
        }

        private void UpdateTextureCount()
        {
            if (textures.Count == 1)
                toolStripStatusLabel1.Text = "1 texture";
            else
                toolStripStatusLabel1.Text = textures.Count + " textures";
            alphaSortingToolStripMenuItem.Enabled = currentFormat == TextureFormat.PAK;
        }

        private void UpdateTextureInformation()
        {
            indexTextBox.Text = hexIndexCheckBox.Checked ? listBox1.SelectedIndex.ToString("X") : listBox1.SelectedIndex.ToString();
            bool en = listBox1.SelectedIndex != -1;
            removeTextureButton.Enabled = textureName.Enabled = globalIndex.Enabled = importButton.Enabled = exportButton.Enabled = en;
            if (en)
            {
                suppress = true;
                textureUpButton.Enabled = listBox1.SelectedIndex > 0;
                textureDownButton.Enabled = listBox1.SelectedIndex < textures.Count - 1;
                textureName.Text = textures[listBox1.SelectedIndex].Name;
                globalIndex.Value = textures[listBox1.SelectedIndex].GlobalIndex;
                if (textures[listBox1.SelectedIndex].CheckMipmap())
                {
                    mipmapCheckBox.Enabled = true;
                    mipmapCheckBox.Checked = textures[listBox1.SelectedIndex].Mipmap;
                }
                else
                    mipmapCheckBox.Checked = mipmapCheckBox.Enabled = false;
                UpdateTextureView(textures[listBox1.SelectedIndex].Image);
                textureSizeLabel.Text = $"Actual Size: {textures[listBox1.SelectedIndex].Image.Width}x{textures[listBox1.SelectedIndex].Image.Height}";
                switch (textures[listBox1.SelectedIndex])
                {
                    case PakTextureInfo pak:
                        dataFormatLabel.Text = $"Data Format: {pak.DataFormat}";
                        pixelFormatLabel.Text = $"Surface Flags: {pak.GetSurfaceFlags()}";
                        dataFormatLabel.Show();
                        pixelFormatLabel.Show();
                        checkBoxPAKUseAlpha.Enabled = true;
                        checkBoxPAKUseAlpha.Show();
                        checkBoxPAKUseAlpha.Checked = pak.DataFormat == GvrDataFormat.Rgb5a3;
                        break;
                    case PvrTextureInfo pvr:
                        dataFormatLabel.Text = $"Data Format: {pvr.DataFormat}";
                        pixelFormatLabel.Text = $"Pixel Format: {pvr.PixelFormat}";
                        dataFormatLabel.Show();
                        pixelFormatLabel.Show();
                        numericUpDownOrigSizeX.Enabled = numericUpDownOrigSizeY.Enabled = false;
                        numericUpDownOrigSizeX.Value = pvr.Image.Width;
                        numericUpDownOrigSizeY.Value = pvr.Image.Height;
                        checkBoxPAKUseAlpha.Enabled = false;
                        checkBoxPAKUseAlpha.Hide();
                        break;
                    case GvrTextureInfo gvr:
                        dataFormatLabel.Text = $"Data Format: {gvr.DataFormat}";
                        pixelFormatLabel.Text = $"Pixel Format: {gvr.PixelFormat}";
                        dataFormatLabel.Show();
                        pixelFormatLabel.Show();
                        numericUpDownOrigSizeX.Enabled = numericUpDownOrigSizeY.Enabled = false;
                        numericUpDownOrigSizeX.Value = gvr.Image.Width;
                        numericUpDownOrigSizeY.Value = gvr.Image.Height;
                        checkBoxPAKUseAlpha.Enabled = false;
                        checkBoxPAKUseAlpha.Hide();
                        break;
                    case PvmxTextureInfo pvmx:
                        numericUpDownOrigSizeX.Enabled = numericUpDownOrigSizeY.Enabled = true;
                        if (pvmx.Dimensions.HasValue)
                        {
                            numericUpDownOrigSizeX.Value = pvmx.Dimensions.Value.Width;
                            numericUpDownOrigSizeY.Value = pvmx.Dimensions.Value.Height;
                        }
                        else
                        {
                            numericUpDownOrigSizeX.Value = pvmx.Image.Width;
                            numericUpDownOrigSizeY.Value = pvmx.Image.Height;
                        }
                        dataFormatLabel.Hide();
                        pixelFormatLabel.Hide();
                        checkBoxPAKUseAlpha.Enabled = false;
                        checkBoxPAKUseAlpha.Hide();
                        break;
                    default:
                        dataFormatLabel.Hide();
                        pixelFormatLabel.Hide();
                        numericUpDownOrigSizeX.Enabled = numericUpDownOrigSizeY.Enabled = false;
                        numericUpDownOrigSizeX.Value = textures[listBox1.SelectedIndex].Image.Width;
                        numericUpDownOrigSizeY.Value = textures[listBox1.SelectedIndex].Image.Height;
                        checkBoxPAKUseAlpha.Enabled = false;
                        checkBoxPAKUseAlpha.Hide();
                        break;
                }
                suppress = false;
            }
            else
            {
                textureUpButton.Enabled = false;
                textureDownButton.Enabled = false;
                mipmapCheckBox.Enabled = false;
                dataFormatLabel.Text = $"Data Format: Unknown";
                pixelFormatLabel.Text = $"Pixel Format: Unknown";
                textureSizeLabel.Text = $"Actual Size: ---";
                indexTextBox.Text = "-1";
                UpdateTextureView(new Bitmap(64, 64));
            }
        }

        private List<TextureInfo> GetTexturesFromFile(string fname)
        {
            byte[] datafile = File.ReadAllBytes(fname);
            List<TextureInfo> newtextures;
            if (PVMXFile.Identify(datafile))
            {
                PVMXFile pvmx = new PVMXFile(datafile);
                newtextures = new List<TextureInfo>();
                foreach (PVMXFile.PVMXEntry pvmxe in pvmx.Entries)
                {
                    PvmxTextureInfo texinfo = new PvmxTextureInfo(Path.GetFileNameWithoutExtension(pvmxe.Name), pvmxe.GBIX, pvmxe.GetBitmap());
                    if (pvmxe.HasDimensions())
                        texinfo.Dimensions = new Size(pvmxe.Width, pvmxe.Height);
                    newtextures.Add(texinfo);
                }
            }
            else if (PAKFile.Identify(fname))
            {
                PAKFile pak = new PAKFile(fname);
                string filenoext = Path.GetFileNameWithoutExtension(fname).ToLowerInvariant();
                bool hasIndex = false;
                foreach (PAKFile.PAKEntry fl in pak.Entries)
                {
                    if (fl.Name.Equals(filenoext + ".inf", StringComparison.OrdinalIgnoreCase))
                        hasIndex = true;
                }
                if (!hasIndex)
                {
                    MessageBox.Show("This PAK archive does not contain an index file, and Texture Editor cannot open it. Use ArchiveTool to open such files.", "Texture Editor");
                    textures.Clear();
                    listBox1.Items.Clear();
                    archiveFilename = null;
                    Text = "PAK Editor";
                    UpdateTextureCount();
                    return new List<TextureInfo>();
                }
                byte[] inf = pak.Entries.Single((file) => file.Name.Equals(filenoext + ".inf", StringComparison.OrdinalIgnoreCase)).Data;
                newtextures = new List<TextureInfo>(inf.Length / 0x3C);
                for (int i = 0; i < inf.Length; i += 0x3C)
                {
                    // Load a PAK INF entry
                    byte[] pakentry = new byte[0x3C];
                    Array.Copy(inf, i, pakentry, 0, 0x3C);
                    PAKInfEntry entry = new PAKInfEntry(pakentry);
                    // Load texture data
                    byte[] dds = pak.Entries.First((file) => file.Name.Equals(entry.GetFilename() + ".dds", StringComparison.OrdinalIgnoreCase)).Data;
                    newtextures.Add(new PakTextureInfo(entry.GetFilename(), entry.globalindex, CreateBitmapFromStream(new MemoryStream(dds)), entry.Type, entry.fSurfaceFlags));
                }
            }
            else
            {
				PuyoArchiveType idResult = PuyoFile.Identify(datafile);
                if (idResult == PuyoArchiveType.Unknown)
                {
                    MessageBox.Show(this, "Unknown archive type: " + fname + ".", "Texture Editor Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return new List<TextureInfo>();
                }
                PuyoFile arc = new PuyoFile(datafile);
                if (arc.PaletteRequired)
                    arc.AddPalette(Path.GetDirectoryName(fname));
                newtextures = new List<TextureInfo>(arc.Entries.Count);
                foreach (GenericArchiveEntry file in arc.Entries)
                {
                    MemoryStream str = new MemoryStream(file.Data);
                    str.Seek(0, SeekOrigin.Begin);
                    if (file is PVMEntry pvme)
                        newtextures.Add(new PvrTextureInfo(Path.GetFileNameWithoutExtension(file.Name), str, pvme.Palette));
                    else if (file is GVMEntry gvme)
                        newtextures.Add(new GvrTextureInfo(Path.GetFileNameWithoutExtension(file.Name), str, gvme.Palette));
                }
            }
            // Check if TextureInfo match the current format and convert if necessary
            for (int i = 0; i < newtextures.Count; i++)
            {
                switch (currentFormat)
                {
                    case TextureFormat.PVM:
                        if (!(newtextures[i] is PvrTextureInfo))
                            newtextures[i] = new PvrTextureInfo(newtextures[i]);
                        break;
                    case TextureFormat.GVM:
                        if (!(newtextures[i] is GvrTextureInfo))
                            newtextures[i] = new GvrTextureInfo(newtextures[i]);
                        break;
                    case TextureFormat.PVMX:
                        if (!(newtextures[i] is PvmxTextureInfo))
                            newtextures[i] = new PvmxTextureInfo(newtextures[i]);
                        break;
                    case TextureFormat.PAK:
                        if (!(newtextures[i] is PakTextureInfo))
                            newtextures[i] = new PakTextureInfo(newtextures[i]);
                        break;
                }
            }
            return newtextures;
        }

        private bool LoadArchive(string filename)
        {
            byte[] datafile = File.ReadAllBytes(filename);
            if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
                datafile = FraGag.Compression.Prs.Decompress(datafile);
            // Identify format and set program mode
            if (PVMXFile.Identify(datafile))
                currentFormat = TextureFormat.PVMX;
            else if (PAKFile.Identify(filename))
                currentFormat = TextureFormat.PAK;
            else
            {
                PuyoArchiveType identifyResult = PuyoFile.Identify(datafile);
                switch (identifyResult)
                {
                    case PuyoArchiveType.PVMFile:
                        currentFormat = TextureFormat.PVM;
                        break;
                    case PuyoArchiveType.GVMFile:
                        currentFormat = TextureFormat.GVM;
                        break;
                    case PuyoArchiveType.Unknown:
                        MessageBox.Show(this, "Unknown archive type: \"" + filename + "\".", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                }
            }

            // Load textures
            List<TextureInfo> newtextures = GetTexturesFromFile(filename);
            if (newtextures == null || newtextures.Count == 0)
                return false;
            textures.Clear();
            textures.AddRange(newtextures);
            listBox1.Items.Clear();
            listBox1.Items.AddRange(textures.Select((item) => item.Name).ToArray());
            UpdateTextureCount();
            UpdateMRUList(Path.GetFullPath(filename));
            return true;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            System.Collections.Specialized.StringCollection newlist = new System.Collections.Specialized.StringCollection();

            if (Settings.MRUList != null)
            {
                foreach (string file in Settings.MRUList)
                {
                    if (File.Exists(file))
                    {
                        newlist.Add(file);
                        recentFilesToolStripMenuItem.DropDownItems.Add(file.Replace("&", "&&"));
                    }
                }
            }

            Settings.MRUList = newlist;

            highQualityGVMsToolStripMenuItem.Checked = Settings.HighQualityGVM;

            if (Program.Arguments.Length > 0 && !LoadArchive(Program.Arguments[0]))
                Close();
        }

        private void SplitContainer1_Panel2_SizeChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > -1)
                UpdateTextureView(textures[listBox1.SelectedIndex].Image);
        }

        private void NewFile(TextureFormat newfilefmt)
        {
            if (unsaved)
            {
                DialogResult res = MessageBox.Show(this, "There are unsaved changes. Would you like to save them?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (res)
                {
                    case DialogResult.Yes:
                        SaveTextures();
                        break;
                    case DialogResult.Cancel:
                        return;
                    case DialogResult.No:
                        break;
                }
            }
            textures.Clear();
            listBox1.Items.Clear();
            archiveFilename = null;
            currentFormat = newfilefmt;
            Text = newfilefmt.ToString() + " Editor";
            UpdateTextureCount();
            listBox1.SelectedIndex = -1;
        }

        private void newPVMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewFile(TextureFormat.PVM);
            listBox1_SelectedIndexChanged(sender, e);
        }

        private void newGVMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewFile(TextureFormat.GVM);
            listBox1_SelectedIndexChanged(sender, e);
        }

        private void newPVMXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewFile(TextureFormat.PVMX);
            listBox1_SelectedIndexChanged(sender, e);
        }

        private void newPAKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewFile(TextureFormat.PAK);
            listBox1_SelectedIndexChanged(sender, e);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (unsaved)
            {
                DialogResult res = MessageBox.Show(this, "There are unsaved changes. Would you like to save them?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (res)
                {
                    case DialogResult.Yes:
                        SaveTextures();
                        break;
                    case DialogResult.Cancel:
                        return;
                    case DialogResult.No:
                        break;
                }
            }
            using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "pvm", Filter = "Texture Files|*.pvm;*.gvm;*.prs;*.pvmx;*.pak" })
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    LoadArchive(dlg.FileName);
                    listBox1_SelectedIndexChanged(sender, e);
                    unsaved = false;
                }
        }

        private MemoryStream EncodePVR(PvrTextureInfo tex)
        {
            if (tex.TextureData != null)
                return TextureFunctions.UpdateGBIX(tex.TextureData, tex.GlobalIndex);
            switch (tex.DataFormat)
            {
                case PvrDataFormat.Index8:
                case PvrDataFormat.Index4:
                    tex.Image = new Bitmap(TextureEditor.Properties.Resources.error);
                    tex.PixelFormat = PvrPixelFormat.Rgb565;
                    tex.DataFormat = PvrDataFormat.SquareTwiddled;
                    break;
                default:
                    tex.PixelFormat = TextureFunctions.GetPvrPixelFormatFromBitmap(tex.Image);
                    tex.DataFormat = TextureFunctions.GetPvrDataFormatFromBitmap(tex.Image, tex.Mipmap);
                    break;
            }
            PvrTextureEncoder encoder = new PvrTextureEncoder(tex.Image, tex.PixelFormat, tex.DataFormat);
            encoder.GlobalIndex = tex.GlobalIndex;
            MemoryStream pvr = new MemoryStream();
            encoder.Save(pvr);
            pvr.Seek(0, SeekOrigin.Begin);
            return pvr;
        }

        private MemoryStream EncodeGVR(GvrTextureInfo tex)
        {
            if (tex.TextureData != null)
                return TextureFunctions.UpdateGBIX(tex.TextureData, tex.GlobalIndex, true);
            if (tex.DataFormat != GvrDataFormat.Index4 && tex.DataFormat != GvrDataFormat.Index8)
                tex.DataFormat = TextureFunctions.GetGvrDataFormatFromBitmap(tex.Image, Settings.HighQualityGVM);
            else // Cannot encode a paletted texture
            {
                tex.Image = new Bitmap(TextureEditor.Properties.Resources.error);
                tex.PixelFormat = GvrPixelFormat.Unknown;
                tex.DataFormat = GvrDataFormat.Dxt1;
            }
            GvrTextureEncoder encoder = new GvrTextureEncoder(tex.Image, tex.PixelFormat, tex.DataFormat);
            encoder.GlobalIndex = tex.GlobalIndex;
            MemoryStream gvr = new MemoryStream();
            encoder.Save(gvr);
            gvr.Seek(0, SeekOrigin.Begin);
            return gvr;
        }

        private void SaveTextures()
        {
            byte[] data;
            using (MemoryStream str = new MemoryStream())
            {
                ArchiveWriter writer = null;
                bool paletteerror = false;
                switch (currentFormat)
                {
                    case TextureFormat.PVM:
                        writer = new PvmArchiveWriter(str);
                        foreach (PvrTextureInfo tex in textures)
                        {
                            MemoryStream pvr = EncodePVR(tex);
                            if (pvr == null)
                            {
                                if (!paletteerror)
                                {
                                    MessageBox.Show("Unable to encode textures with an external palette.\nUse ArchiveTool to convert archives with paletted textures.", "Texture Editor Error");
                                    paletteerror = true;
                                }
                                continue;
                            }
                            writer.CreateEntry(pvr, tex.Name);
                        }
                        break;
                    case TextureFormat.GVM:
                        writer = new GvmArchiveWriter(str);
                        foreach (GvrTextureInfo tex in textures)
                        {
                            MemoryStream gvr = EncodeGVR(tex);
                            if (gvr == null)
                            {
                                if (!paletteerror)
                                {
                                    MessageBox.Show("Unable to encode textures with an external palette.\nUse ArchiveTool to convert archives with paletted textures.", "Texture Editor Error");
                                    paletteerror = true;
                                }
                                continue;
                            }
                            writer.CreateEntry(gvr, tex.Name);
                        }
                        break;
                    case TextureFormat.PVMX:
                        PVMXFile pvmx = new PVMXFile();
                        foreach (PvmxTextureInfo tex in textures)
                        {
                            Size size = new Size(tex.Image.Width, tex.Image.Height);
                            if (tex.Dimensions.HasValue)
                                size = new Size(tex.Dimensions.Value.Width, tex.Dimensions.Value.Height);
                            MemoryStream ds = new MemoryStream();
                            tex.Image.Save(ds, System.Drawing.Imaging.ImageFormat.Png);
                            pvmx.Entries.Add(new PVMXFile.PVMXEntry(tex.Name, tex.GlobalIndex, ds.ToArray(), size.Width, size.Height));
                        }
                        File.WriteAllBytes(archiveFilename, pvmx.GetBytes());
                        return;
                    case TextureFormat.PAK:
                        PAKFile pak = new PAKFile();
                        string filenoext = Path.GetFileNameWithoutExtension(archiveFilename).ToLowerInvariant();
                        pak.FolderName = filenoext;
                        string longdir = "..\\..\\..\\sonic2\\resource\\gd_pc\\prs\\" + filenoext;
                        List<byte> inf = new List<byte>(textures.Count * 0x3C);
                        foreach (TextureInfo item in textures)
                        {
                            using (MemoryStream tex = new MemoryStream())
                            {
                                item.Image.Save(tex, ImageFormat.Png);
                                byte[] tb = tex.ToArray();
                                string name = item.Name.ToLowerInvariant();
                                if (name.Length > 0x1C)
                                    name = name.Substring(0, 0x1C);
                                pak.Entries.Add(new PAKFile.PAKEntry(name + ".dds", longdir + '\\' + name + ".dds", tb));
                                // Create a new PAK INF entry
                                PAKInfEntry entry = new PAKInfEntry();
                                byte[] namearr = Encoding.ASCII.GetBytes(name);
                                Array.Copy(namearr, entry.filename, namearr.Length);
                                entry.globalindex = item.GlobalIndex;
                                entry.nWidth = (uint)item.Image.Width;
                                entry.nHeight = (uint)item.Image.Height;
                                // Salvage GVR data if available
                                if (item is PakTextureInfo pk)
                                {
                                    entry.Type = entry.PixelFormat = pk.DataFormat;
                                    entry.fSurfaceFlags = pk.SurfaceFlags;
                                }
                                if (item.Mipmap)
                                    entry.fSurfaceFlags |= NinjaSurfaceFlags.Mipmapped;
                                inf.AddRange(entry.GetBytes());
                            }
                        }
                        pak.Entries.Insert(0, new PAKFile.PAKEntry(filenoext + ".inf", longdir + '\\' + filenoext + ".inf", inf.ToArray()));
                        pak.Save(archiveFilename);
                        return;
                }
                writer?.Flush();
                data = str.ToArray();
                str.Close();
            }
            if (Path.GetExtension(archiveFilename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
                FraGag.Compression.Prs.Compress(data, archiveFilename);
            else
                File.WriteAllBytes(archiveFilename, data);
            unsaved = false;
        }

        private void ConvertTextures(TextureFormat newfmt)
        {
            switch (newfmt)
            {
                case TextureFormat.PVM:
                    switch (currentFormat)
                    {
                        case TextureFormat.GVM:
                            textures = new List<TextureInfo>(textures.Cast<GvrTextureInfo>().Select(a => new PvrTextureInfo(a)).Cast<TextureInfo>());
                            break;
                        case TextureFormat.PVMX:
                        case TextureFormat.PAK:
                            textures = new List<TextureInfo>(textures.Select(a => new PvrTextureInfo(a)).Cast<TextureInfo>());
                            break;
                    }
                    break;
                case TextureFormat.GVM:
                    switch (currentFormat)
                    {
                        case TextureFormat.PVM:
                            textures = new List<TextureInfo>(textures.Cast<PvrTextureInfo>().Select(a => new GvrTextureInfo(a)).Cast<TextureInfo>());
                            break;
                        case TextureFormat.PVMX:
                        case TextureFormat.PAK:
                            textures = new List<TextureInfo>(textures.Select(a => new GvrTextureInfo(a)).Cast<TextureInfo>());
                            break;
                    }
                    break;
                case TextureFormat.PVMX:
                    switch (currentFormat)
                    {
                        case TextureFormat.PVM:
                        case TextureFormat.GVM:
                        case TextureFormat.PAK:
                            textures = new List<TextureInfo>(textures.Select(a => new PvmxTextureInfo(a)).Cast<TextureInfo>());
                            break;
                    }
                    break;
                case TextureFormat.PAK:
                    switch (currentFormat)
                    {
                        case TextureFormat.PVM:
                        case TextureFormat.GVM:
                        case TextureFormat.PVMX:
                            textures = new List<TextureInfo>(textures.Select(a => new PakTextureInfo(a)).Cast<TextureInfo>());
                            break;
                    }
                    break;
            }
            currentFormat = newfmt;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (archiveFilename == null)
                saveAsToolStripMenuItem_Click(sender, e);
            else
                SaveTextures();
        }

        private void SaveAs(TextureFormat savefmt)
        {
            string ext;
            switch (savefmt)
            {
                case TextureFormat.GVM:
                    ext = "gvm";
                    break;
                case TextureFormat.PVMX:
                    ext = "pvmx";
                    break;
                case TextureFormat.PAK:
                    ext = "pak";
                    break;
                case TextureFormat.PVM:
                default:
                    ext = "pvm";
                    break;
            }
            using (SaveFileDialog dlg = new SaveFileDialog() { FilterIndex = (int)savefmt + 1, DefaultExt = ext, Filter = "PVM Files|*.pvm;*.prs|GVM Files|*.gvm;*.prs|PVMX Files|*.pvmx|PAK Files|*.pak" })
            {
                if (archiveFilename != null)
                {
                    dlg.InitialDirectory = Path.GetDirectoryName(archiveFilename);
                    dlg.FileName = Path.ChangeExtension(Path.GetFileName(archiveFilename), ext);
                }
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    ConvertTextures((TextureFormat)dlg.FilterIndex - 1);
                    UpdateMRUList(dlg.FileName);
                    SaveTextures();
                    unsaved = false;
                }
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileToolStripMenuItem.HideDropDown();
            SaveAs(currentFormat);
        }

        private void saveAsPVMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs(TextureFormat.PVM);
        }

        private void saveAsGVMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs(TextureFormat.GVM);
        }

        private void saveAsPVMXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs(TextureFormat.PVMX);
        }

        private void saveAsPAKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs(TextureFormat.PAK);
        }

        private void importTexturePackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "txt", Filter = "index.txt|index.txt", FileName = "index.txt" })
            {
                if (archiveFilename != null)
                    dlg.InitialDirectory = Path.GetDirectoryName(archiveFilename);
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    using (TextReader texList = File.OpenText(dlg.FileName))
                    {
                        string dir = Path.GetDirectoryName(dlg.FileName);
                        listBox1.BeginUpdate();
                        string line = texList.ReadLine();
                        while (line != null)
                        {
                            string[] split = line.Split(',');
                            if (split.Length > 1)
                            {
                                uint gbix = uint.Parse(split[0]);
                                string name = Path.ChangeExtension(split[1], null);
                                Bitmap bmp;
                                MemoryStream str = new MemoryStream(File.ReadAllBytes(Path.Combine(dir, split[1])));
                                bmp = CreateBitmapFromStream(str);
                                switch (currentFormat)
                                {
                                    case TextureFormat.PVM:
                                        textures.Add(new PvrTextureInfo(name, gbix, bmp));
                                        break;
                                    case TextureFormat.GVM:
                                        textures.Add(new GvrTextureInfo(name, gbix, bmp));
                                        break;
                                    case TextureFormat.PVMX:
                                        PvmxTextureInfo pvmx = new PvmxTextureInfo(name, gbix, bmp);
                                        if (split.Length > 2)
                                        {
                                            string[] dim = split[2].Split('x');
                                            if (dim.Length > 1)
                                                pvmx.Dimensions = new Size(int.Parse(dim[0]), int.Parse(dim[1]));
                                        }
                                        textures.Add(pvmx);
                                        break;
                                    case TextureFormat.PAK:
                                        textures.Add(new PakTextureInfo(name, gbix, bmp));
                                        break;
                                }
                                listBox1.Items.Add(name);

                            }
                            line = texList.ReadLine();
                        }
                        listBox1.EndUpdate();
                        listBox1.SelectedIndex = textures.Count - 1;
                        UpdateTextureCount();
                        unsaved = true;
                    }
            }
        }

        private void exportTexturePackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog() { Title = "Save Folder", DefaultExt = "", Filter = "Folder|", FileName = "texturepack" })
            {

                if (archiveFilename != null)
                {
                    dlg.InitialDirectory = Path.GetDirectoryName(archiveFilename);
                    dlg.FileName = Path.GetFileNameWithoutExtension(archiveFilename);
                }

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    Directory.CreateDirectory(dlg.FileName);
                    string dir = Path.Combine(Path.GetDirectoryName(dlg.FileName), Path.GetFileName(dlg.FileName));
                    using (TextWriter texList = File.CreateText(Path.Combine(dir, "index.txt")))
                    {
                        foreach (TextureInfo tex in textures)
                        {
                            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(tex.Image);
                            if (tex is PvrTextureInfo pvri)
                            {
                                if (pvri.DataFormat == PvrDataFormat.Index4 && exportDefaultPaletteToolStripMenuItem.Checked)
                                {
                                    PvrTexture temp = new PvrTexture(pvri.TextureData.ToArray());
                                    temp.SetPalette(new PvpPalette(TextureEditor.Properties.Resources.defaultPVP));
                                    bmp = TextureFunctions.ExportPalettedTexture(temp.ToBitmap());
                                }
                            }
                            else if (tex is GvrTextureInfo gvri)
                            {
                                if (gvri.DataFormat == GvrDataFormat.Index4 && exportDefaultPaletteToolStripMenuItem.Checked)
                                {
                                    GvrTexture temp = new GvrTexture(gvri.TextureData.ToArray());
                                    temp.SetPalette(new GvpPalette(TextureEditor.Properties.Resources.defaultGVP));
                                    bmp = TextureFunctions.ExportPalettedTexture(temp.ToBitmap());
                                }
                            }
                            bmp.Save(Path.Combine(dir, tex.Name + ".png"));
                            if (tex is PvmxTextureInfo xtex && xtex.Dimensions.HasValue)
                                texList.WriteLine("{0},{1},{2}x{3}", xtex.GlobalIndex, xtex.Name + ".png", xtex.Dimensions.Value.Width, xtex.Dimensions.Value.Height);
                            else
                                texList.WriteLine("{0},{1},{2}x{3}", tex.GlobalIndex, tex.Name + ".png", tex.Image.Width, tex.Image.Height);
                        }
                    }
                }
            }
        }

        private void recentFilesToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            LoadArchive(Settings.MRUList[recentFilesToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem)]);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void UpdateTextureView(Image image)
        {
            int width = image.Width;
            int height = image.Height;
            int maxwidth = splitContainer1.Panel2.Width - 20;
            int maxheight = splitContainer1.Panel2.Height - (tableLayoutPanel1.Top + (tableLayoutPanel1.Height - textureImage.Height)) - 20;
            if (width > maxwidth || height > maxheight)
            {
                double widthpct = width / (double)maxwidth;
                double heightpct = height / (double)maxheight;
                switch (Math.Sign(widthpct - heightpct))
                {
                    case -1: // height > width
                        maxwidth = (int)(width / heightpct);
                        break;
                    case 1:
                        maxheight = (int)(height / widthpct);
                        break;
                }
                Bitmap bmp = new Bitmap(maxwidth, maxheight);
                using (Graphics gfx = Graphics.FromImage(bmp))
                {
                    gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    gfx.DrawImage(image, 0, 0, maxwidth, maxheight);
                }
                textureImage.Image = bmp;
            }
            else
                textureImage.Image = image;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTextureInformation();
        }

        private void addTextureButton_Click(object sender, EventArgs e)
        {
            string defext = null;
            string filter = "Supported Files|*.prs;*.pvm;*.gvm;*.pvmx;*.pak;*.pvr;*.gvr;*.png;*.jpg;*.jpeg;*.gif;*.bmp;*.dds";
            switch (currentFormat)
            {
                case TextureFormat.PVM:
                    defext = "pvr";
                    break;
                case TextureFormat.GVM:
                    defext = "gvr";
                    break;
                case TextureFormat.PVMX:
                case TextureFormat.PAK:
                    defext = "png";
                    break;
            }
            using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = defext, Filter = filter, Multiselect = true })
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    uint gbix = textures.Count == 0 ? 0 : textures.Max((item) => item.GlobalIndex);
                    if (gbix != uint.MaxValue)
                        gbix++;
                    listBox1.BeginUpdate();
                    foreach (string file in dlg.FileNames)
                    {
                        switch (Path.GetExtension(file).ToLowerInvariant())
                        {
                            case ".prs":
                            case ".pvm":
                            case ".gvm":
                            case ".pak":
                            case ".pvmx":
                                foreach (TextureInfo tex in GetTexturesFromFile(file))
                                {
                                    textures.Add(tex);
                                    listBox1.Items.Add(tex.Name);
                                }
                                break;
                            default:
                                {
                                    string name = Path.GetFileNameWithoutExtension(file);
                                    MemoryStream str = new MemoryStream(File.ReadAllBytes(file));
                                    if (currentFormat == TextureFormat.PVM && PvrTexture.Is(file))
                                        textures.Add(new PvrTextureInfo(name, str));
                                    else if (currentFormat == TextureFormat.GVM && GvrTexture.Is(file))
                                        textures.Add(new GvrTextureInfo(name, str));
                                    else
                                    {
                                        switch (currentFormat)
                                        {
                                            case TextureFormat.PVM:
                                                textures.Add(new PvrTextureInfo(name, gbix, CreateBitmapFromStream(str)));
                                                break;
                                            case TextureFormat.GVM:
                                                textures.Add(new GvrTextureInfo(name, gbix, CreateBitmapFromStream(str)));
                                                break;
                                            case TextureFormat.PVMX:
                                                textures.Add(new PvmxTextureInfo(name, gbix, CreateBitmapFromStream(str)));
                                                break;
                                            case TextureFormat.PAK:
                                                textures.Add(new PakTextureInfo(name, gbix, CreateBitmapFromStream(str)));
                                                break;
                                        }
                                        if (gbix != uint.MaxValue)
                                            gbix++;
                                    }
                                    listBox1.Items.Add(name);
                                }
                                break;
                        }
                    }
                    listBox1.EndUpdate();
                    UpdateTextureCount();
                    listBox1.SelectedIndex = textures.Count - 1;
                    unsaved = true;
                }
            }
        }

        private void removeTextureButton_Click(object sender, EventArgs e)
        {
            int i = listBox1.SelectedIndex;
            textures.RemoveAt(i);
            listBox1.Items.RemoveAt(i);
            if (i == textures.Count)
                --i;
            listBox1.SelectedIndex = i;
            UpdateTextureCount();
            unsaved = true;
        }

        private void TextureUpButton_Click(object sender, EventArgs e)
        {
            int i = listBox1.SelectedIndex;
            TextureInfo ti = textures[i];
            textures.RemoveAt(i);
            textures.Insert(i - 1, ti);
            listBox1.BeginUpdate();
            listBox1.Items.RemoveAt(i);
            listBox1.Items.Insert(i - 1, ti.Name);
            listBox1.EndUpdate();
            listBox1.SelectedIndex = i - 1;
            unsaved = true;
        }

        private void TextureDownButton_Click(object sender, EventArgs e)
        {
            int i = listBox1.SelectedIndex;
            TextureInfo ti = textures[i];
            textures.RemoveAt(i);
            textures.Insert(i + 1, ti);
            listBox1.BeginUpdate();
            listBox1.Items.RemoveAt(i);
            listBox1.Items.Insert(i + 1, ti.Name);
            listBox1.EndUpdate();
            listBox1.SelectedIndex = i + 1;
            unsaved = true;
        }

        private void HexIndexCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            indexTextBox.Text = hexIndexCheckBox.Checked ? listBox1.SelectedIndex.ToString("X") : listBox1.SelectedIndex.ToString();
        }

        private void textureName_TextChanged(object sender, EventArgs e)
        {
            bool focus = textureName.Focused;
            int st = textureName.SelectionStart;
            int len = textureName.SelectionLength;
            string name = textureName.Text;
            foreach (char c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            listBox1.Items[listBox1.SelectedIndex] = textureName.Text = textures[listBox1.SelectedIndex].Name = name;
            if (focus)
                textureName.Focus();
            textureName.SelectionStart = st;
            textureName.SelectionLength = len;
        }

        private void globalIndex_ValueChanged(object sender, EventArgs e)
        {
            if (textures[listBox1.SelectedIndex].GlobalIndex != (uint)globalIndex.Value)
            {
                textures[listBox1.SelectedIndex].GlobalIndex = (uint)globalIndex.Value;
                unsaved = true;
            }
        }

        private void mipmapCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (textures[listBox1.SelectedIndex].Mipmap != mipmapCheckBox.Checked)
            {
                textures[listBox1.SelectedIndex].Mipmap = mipmapCheckBox.Checked;
                // Update surface flags for PAK textures
                if (textures[listBox1.SelectedIndex] is PakTextureInfo pk)
                {
                    if (!mipmapCheckBox.Checked)
                        pk.SurfaceFlags &= ~NinjaSurfaceFlags.Mipmapped;
                    else
                        pk.SurfaceFlags |= NinjaSurfaceFlags.Mipmapped;
                }
                UpdateTextureInformation();
                unsaved = true;
            }
        }

        private void textureImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedIndex != -1 && e.Button == MouseButtons.Left)
            {
                Bitmap bmp = new Bitmap(textures[listBox1.SelectedIndex].Image);
                DataObject dobj = new DataObject();
                dobj.SetImage(bmp);
                string fn = Path.Combine(Path.GetTempPath(), textures[listBox1.SelectedIndex].Name + ".png");
                bmp.Save(fn);
                dobj.SetFileDropList(new System.Collections.Specialized.StringCollection() { fn });
                DoDragDrop(dobj, DragDropEffects.Copy);
            }
        }

        private void textureImage_DragEnter(object sender, DragEventArgs e)
        {
            if (listBox1.SelectedIndex != -1 && (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(DataFormats.Bitmap)))
                e.Effect = DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;
        }

        private void textureImage_DragDrop(object sender, DragEventArgs e)
        {
            if (listBox1.SelectedIndex == -1) return;
            Bitmap tex = null;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {   
                MemoryStream ms = new MemoryStream(File.ReadAllBytes(((string[])e.Data.GetData(DataFormats.FileDrop, true))[0]));
                tex = CreateBitmapFromStream(ms);
            }
            else if (e.Data.GetDataPresent(DataFormats.Bitmap))
                tex = new Bitmap((Image)e.Data.GetData(DataFormats.Bitmap));
            else
                return;
            textures[listBox1.SelectedIndex].Image = tex;
            UpdateTextureView(textures[listBox1.SelectedIndex].Image);
            UpdateTextureInformation();
            unsaved = true;
        }

        private void textureImage_MouseClick(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedIndex != -1 && e.Button == MouseButtons.Right)
            {
                pasteToolStripMenuItem.Enabled = Clipboard.ContainsImage();
                contextMenuStrip1.Show(textureImage, e.Location);
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetImage(textures[listBox1.SelectedIndex].Image);
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap tex = new Bitmap(Clipboard.GetImage());
            textures[listBox1.SelectedIndex].Image = tex;
            UpdateTextureView(textures[listBox1.SelectedIndex].Image);
            UpdateTextureInformation();
            unsaved = true;
        }

        private Bitmap CreateBitmapFromStream(MemoryStream texmemstr)
        {
            Bitmap bitmap_temp;
            Bitmap textureimg;
            if (PvrTexture.Is(texmemstr))
            {
                PvrTexture pvrt = new PvrTexture(texmemstr);
                bitmap_temp = pvrt.ToBitmap();
            }
            else if (GvrTexture.Is(texmemstr))
            {
                GvrTexture gvrt = new GvrTexture(texmemstr);
                bitmap_temp = gvrt.ToBitmap();
            }
            else
            {
                // Check if the texture is DDS
                uint check = BitConverter.ToUInt32(texmemstr.ToArray(), 0);
                if (check == 0x20534444) // DDS header
                {
                    PixelFormat pxformat;
                    var image = Pfim.Pfim.FromStream(texmemstr, new Pfim.PfimConfig());
                    switch (image.Format)
                    {
                        case Pfim.ImageFormat.Rgba32:
                            pxformat = PixelFormat.Format32bppArgb;
                            break;
                        default:
                            MessageBox.Show("Unsupported image format.");
                            throw new NotImplementedException();
                    }
                    bitmap_temp = new Bitmap(image.Width, image.Height, pxformat);
                    BitmapData bmpData = bitmap_temp.LockBits(new Rectangle(0, 0, bitmap_temp.Width, bitmap_temp.Height), ImageLockMode.WriteOnly, pxformat);
                    Marshal.Copy(image.Data, 0, bmpData.Scan0, image.DataLen);
                    bitmap_temp.UnlockBits(bmpData);
                }
                else
                    bitmap_temp = new Bitmap(texmemstr);
            }
            // The bitmap is cloned to avoid access error on the original file
            textureimg = new Bitmap(bitmap_temp);
            bitmap_temp.Dispose();
            return textureimg;
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "pvr", Filter = "Texture Files|*.pvr;*.gvr;*.png;*.jpg;*.jpeg;*.gif;*.bmp" };
            if (!string.IsNullOrEmpty(listBox1.GetItemText(listBox1.SelectedItem)))
                dlg.FileName = listBox1.GetItemText(listBox1.SelectedItem);
            DialogResult res = dlg.ShowDialog(this);
            if (res == DialogResult.OK)
            {
                string name = Path.GetFileNameWithoutExtension(dlg.FileName);
                MemoryStream texmemstr = new MemoryStream(File.ReadAllBytes(dlg.FileName));
                switch (currentFormat)
                {
                    case TextureFormat.PVM:
                        PvrTextureInfo oldpvr = (PvrTextureInfo)textures[listBox1.SelectedIndex];
                        if (PvrTexture.Is(dlg.FileName))
                            textures[listBox1.SelectedIndex] = new PvrTextureInfo(oldpvr.Name, texmemstr);
                        else
                        {
                            PvrTextureInfo newpvr = new PvrTextureInfo(oldpvr.Name, oldpvr.GlobalIndex, CreateBitmapFromStream(texmemstr));
                            newpvr.TextureData = null;
                            newpvr.PixelFormat = TextureFunctions.GetPvrPixelFormatFromBitmap(newpvr.Image);
                            newpvr.DataFormat = TextureFunctions.GetPvrDataFormatFromBitmap(newpvr.Image, newpvr.Mipmap);
                            textures[listBox1.SelectedIndex] = newpvr;
                        }
                        break;
                    case TextureFormat.GVM:
                        GvrTextureInfo oldgvr = (GvrTextureInfo)textures[listBox1.SelectedIndex];
                        if (GvrTexture.Is(dlg.FileName))
                            textures[listBox1.SelectedIndex] = new GvrTextureInfo(oldgvr.Name, texmemstr);
                        else
                        {
                            GvrTextureInfo newgvr = new GvrTextureInfo(oldgvr.Name, oldgvr.GlobalIndex, CreateBitmapFromStream(texmemstr));
                            newgvr.TextureData = null;
                            newgvr.DataFormat = TextureFunctions.GetGvrDataFormatFromBitmap(newgvr.Image, Settings.HighQualityGVM);
                            textures[listBox1.SelectedIndex] = newgvr;
                        }
                        break;
                    case TextureFormat.PVMX:
                        PvmxTextureInfo oldpvmx = (PvmxTextureInfo)textures[listBox1.SelectedIndex];
                        textures[listBox1.SelectedIndex] = new PvmxTextureInfo(oldpvmx.Name, oldpvmx.GlobalIndex, CreateBitmapFromStream(texmemstr));
                        break;
                    case TextureFormat.PAK:
                        PakTextureInfo oldpak = (PakTextureInfo)textures[listBox1.SelectedIndex];
                        textures[listBox1.SelectedIndex] = new PakTextureInfo(name, oldpak.GlobalIndex, CreateBitmapFromStream(texmemstr), oldpak.DataFormat, oldpak.SurfaceFlags);
                        break;
                    default:
                        break;
                }
                UpdateTextureView(textures[listBox1.SelectedIndex].Image);
                UpdateTextureInformation();
                unsaved = true;
            }
            listBox1.Select();
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "png", FileName = textures[listBox1.SelectedIndex].Name + ".png", Filter = "PNG Files|*.png" })
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    Bitmap bmp = new Bitmap(textures[listBox1.SelectedIndex].Image);
                    if (textures[listBox1.SelectedIndex] is PvrTextureInfo pvri)
                    {
                        if (pvri.DataFormat == PvrDataFormat.Index4 && exportDefaultPaletteToolStripMenuItem.Checked)
                        {
                            PvrTexture temp = new PvrTexture(pvri.TextureData.ToArray());
                            temp.SetPalette(new PvpPalette(TextureEditor.Properties.Resources.defaultPVP));
                            bmp = TextureFunctions.ExportPalettedTexture(temp.ToBitmap());
                        }
                    }
                    else if (textures[listBox1.SelectedIndex] is GvrTextureInfo gvri)
                    {
                        if (gvri.DataFormat == GvrDataFormat.Index4 && exportDefaultPaletteToolStripMenuItem.Checked)
                        {
                            GvrTexture temp = new GvrTexture(gvri.TextureData.ToArray());
                            temp.SetPalette(new GvpPalette(TextureEditor.Properties.Resources.defaultGVP));
                            bmp = TextureFunctions.ExportPalettedTexture(temp.ToBitmap());
                        }
                    }
                    bmp.Save(dlg.FileName);
                }

            listBox1.Select();
        }

        private void addMipmapsToAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (TextureInfo info in textures)
                if (info.CheckMipmap())
                {
                    info.Mipmap = true;
                    if (info is PakTextureInfo pk)
                        pk.SurfaceFlags |= NinjaSurfaceFlags.Mipmapped;
                }
            if (listBox1.SelectedIndex != -1 && textures[listBox1.SelectedIndex].CheckMipmap())
                mipmapCheckBox.Checked = true;
            unsaved = true;
        }

        private void highQualityGVMsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            Settings.HighQualityGVM = highQualityGVMsToolStripMenuItem.Checked;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Save();
            if (unsaved)
            {
                DialogResult res = MessageBox.Show(this, "There are unsaved changes. Would you like to save them?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (res)
                {
                    case DialogResult.Yes:
                        SaveTextures();
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                    case DialogResult.No:
                        break;
                }
            }
        }

        private void numericUpDownOrigSizeX_ValueChanged(object sender, EventArgs e)
        {
            if (!suppress && textures[listBox1.SelectedIndex] is PvmxTextureInfo tex)
            {
                if (!tex.Dimensions.HasValue || (tex.Dimensions.Value.Width != (int)numericUpDownOrigSizeX.Value || tex.Dimensions.Value.Height != (int)numericUpDownOrigSizeY.Value))
                {
                    tex.Dimensions = new Size((int)numericUpDownOrigSizeX.Value, (int)numericUpDownOrigSizeY.Value);
                    unsaved = true;
                }
            }
        }

        private void numericUpDownOrigSizeY_ValueChanged(object sender, EventArgs e)
        {
            if (!suppress && textures[listBox1.SelectedIndex] is PvmxTextureInfo tex)
            {
                if (!tex.Dimensions.HasValue || (tex.Dimensions.Value.Width != (int)numericUpDownOrigSizeX.Value || tex.Dimensions.Value.Height != (int)numericUpDownOrigSizeY.Value))
                {
                    tex.Dimensions = new Size((int)numericUpDownOrigSizeX.Value, (int)numericUpDownOrigSizeY.Value);
                    unsaved = true;
                }
            }
        }

        private void checkBoxPAKUseAlpha_CheckedChanged(object sender, EventArgs e)
        {
            if (!(textures[listBox1.SelectedIndex] is PakTextureInfo))
                return;
            PakTextureInfo pk = (PakTextureInfo)textures[listBox1.SelectedIndex];
            // Use alpha
            if (checkBoxPAKUseAlpha.Checked)
                pk.DataFormat = GvrDataFormat.Rgb5a3;
            // Don't use alpha (palettized)
            else if ((pk.SurfaceFlags & NinjaSurfaceFlags.Palettized) != 0)
                pk.DataFormat = GvrDataFormat.Index4;
            // Don't use alpha (regular)
            else
                pk.DataFormat = GvrDataFormat.Dxt1;
            UpdateTextureInformation();
        }

        private void PAKEnableAlphaForAll(bool enable)
        {
            if (textures == null || textures.Count == 0)
                return;
            foreach (PakTextureInfo paktxt in textures)
            {
                if (enable)
                    paktxt.DataFormat = GvrDataFormat.Rgb5a3;
                else
                {
                    if ((paktxt.SurfaceFlags & NinjaSurfaceFlags.Palettized) != 0)
                        paktxt.DataFormat = GvrDataFormat.Index4;
                    else paktxt.DataFormat = GvrDataFormat.Dxt1;
                }
            }
            if (listBox1.SelectedIndex != -1)
                UpdateTextureInformation();
            unsaved = true;
        }

        private void enablePAKAlphaForAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PAKEnableAlphaForAll(true);
        }

        private void disablePAKAlphaForAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PAKEnableAlphaForAll(false);
        }

		private void checkBoxPAKUseAlpha_Click(object sender, EventArgs e)
		{
            checkBoxPAKUseAlpha_CheckedChanged(sender, e);
            unsaved = true;
        }
	}
}