using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAModel.SAEditorCommon;
using PSO.PRS;
using ArchiveLib;
using TextureLib;

using static ArchiveLib.GenericArchive;
using static SAModel.SAEditorCommon.SettingsFile;
using static TextureEditor.TextureFunctions;
using Application = System.Windows.Forms.Application;

namespace TextureEditor
{
	enum TextureFormat { PVM, GVM, PVMX, PAK, XVM }
	enum TextureFiles { PVR, GVR, PNG, DDS, XVR, Default }

	public partial class MainForm : Form
	{
		readonly Properties.Settings Settings = Properties.Settings.Default; // MRU list
		Settings_TextureEditor settingsfile; // User settings
		bool unsaved; // Unsaved changes
		int currentTextureID = -1; // Currently selected texture, only used in the UI
		TextureFormat currentFormat; // PVM, GVM, PAK etc.
		string archiveFilename; // Current name of the PVM, GVM, PAK etc.
		List<GenericTexture> textures = new List<GenericTexture>(); // List of textures
		bool suppress = false; // Suppress UI updates
		bool nonIndexedPAK = false; // SOC PAK
		TexturePalette currentPalette = TexturePalette.CreateDefaultPalette(true);
		TexturePalette defaultPalette = TexturePalette.CreateDefaultPalette(true);
		int paletteSet; // Current palette bank ID
		public MainForm()
		{
			Application.ThreadException += Application_ThreadException;
			InitializeComponent();
		}

		void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			string log = "Texture Editor: New log entry on " + DateTime.Now.ToString("G") + System.Environment.NewLine +
				"Build date:" + File.GetLastWriteTime(Application.ExecutablePath).ToString(System.Globalization.CultureInfo.InvariantCulture) +
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
			// Remove archive filename for individual PVR/GVR textures because only archives can be saved
			if (Path.GetExtension(archiveFilename).ToLowerInvariant() == ".pvr" || Path.GetExtension(archiveFilename).ToLowerInvariant() == ".gvr" || Path.GetExtension(archiveFilename).ToLowerInvariant() == ".xvr")
				archiveFilename = null;
		}

		private void UpdateTextureCount()
		{
			if (textures.Count == 1)
				toolStripStatusLabel1.Text = "1 texture";
			else
				toolStripStatusLabel1.Text = textures.Count + " textures";
			alphaSortingToolStripMenuItem.Enabled = currentFormat == TextureFormat.PAK;
		}

		private void MainForm_Shown(object sender, EventArgs e)
		{
			settingsfile = Settings_TextureEditor.Load();
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

			highQualityGVMsToolStripMenuItem.Checked = settingsfile.HighQualityGVM;
			textureFilteringToolStripMenuItem.Checked = settingsfile.EnableFiltering;
			compatibleGVPToolStripMenuItem.Checked = settingsfile.SACompatiblePalettes;
			ddsPakCompressToolStripMenuItem.Checked = settingsfile.UseDDSforPAK;
			ddsPvmxHighQualityToolStripMenuItem.Checked = settingsfile.UseHQDDS;
			ddsPvmxToolStripMenuItem.Checked = settingsfile.UseDDSforPVMX;
			ddsPakHighQualityToolStripMenuItem.Checked = settingsfile.UseDDSUncompressedForPAK;
			ddsTexturePacksToolStripMenuItem.Checked = settingsfile.UseDDSforTexPack;
			pngPakToolStripMenuItem.Checked = settingsfile.UsePNGforPAK;
			ddsPakBestFitToolStripMenuItem.Checked = settingsfile.UseDDSColorSpace;

			System.Windows.Forms.ToolTip toolTip = new System.Windows.Forms.ToolTip();
			toolTip.AutoPopDelay = 5000;
			toolTip.InitialDelay = 1000;
			toolTip.ReshowDelay = 500;
			toolTip.SetToolTip(checkBoxPAKUseAlpha, "Disables Alpha Test and Z Write for the selected texture.\nThis can make some transparent textures blend better or worse. Use with caution.");
			if (Program.Arguments.Length > 0 && !LoadArchive(Program.Arguments[0]))
				Close();
		}

		private void Textures_DragEnter(object sender, DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

			if (files.Length == 1)
			{
				foreach (string file in files)
				{
					switch (Path.GetExtension(file).ToLowerInvariant())
					{
						// Add textures from archives
						case ".prs":
						case ".pvm":
						case ".pb":
						case ".gvm":
						case ".xvm":
						case ".pak":
						case ".pvmx":
							e.Effect = DragDropEffects.Copy;
							return;
					}
				}
			}

			e.Effect = DragDropEffects.None;
		}

		private void Textures_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
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

				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

				LoadArchive(files[0]);
				listBox1_SelectedIndexChanged(sender, e);
				unsaved = false;
			}

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

		private Bitmap ScaleBitmapToWindow(Bitmap image, System.Drawing.Drawing2D.InterpolationMode mode)
		{
			float scale = 1.0f;
			switch (texturePreviewZoomTrackBar.Value)
			{
				case 0:
					scale = 1.0f / 16.0f;
					break;
				case 1:
					scale = 1.0f / 8.0f;
					break;
				case 2:
					scale = 1.0f / 4.0f;
					break;
				case 3:
					scale = 1.0f / 2.0f;
					break;
				case 4:
					scale = 1.0f;
					break;
				case 5:
					scale = 2.0f;
					break;
				case 6:
					scale = 4.0f;
					break;
				case 7:
					scale = 8.0f;
					break;
				case 8:
					scale = 16.0f;
					break;
			}
			if (trackBarMipmapLevel.Enabled && trackBarMipmapLevel.Value > 0)
				scale = scale * (float)(Math.Pow(2, trackBarMipmapLevel.Value));
			float newwidth = ((float)image.Width * scale);
			float newheight = ((float)image.Height * scale);
			if (newwidth < 1)
			{
				float ratio = 1.0f / newwidth;
				newwidth = newwidth * ratio;
				newheight = newheight * ratio;
			}
			if (newheight < 1)
			{
				float ratio = 1.0f / newheight;
				newwidth = newwidth * ratio;
				newheight = newheight * ratio;
			}
			if (newwidth > 2048)
			{
				float ratio = newwidth / 2048.0f;
				newwidth = newwidth / ratio;
				newheight = newheight / ratio;
			}
			if (newheight > 2048)
			{
				float ratio = newheight / 2048.0f;
				newwidth = newwidth / ratio;
				newheight = newheight / ratio;
			}
			labelZoomInfo.Text = "Zoom: " + (newwidth / (float)image.Width).ToString() + "x (" + newwidth.ToString() + "x" + newheight.ToString() + ")";
			Bitmap bmp = new Bitmap((int)newwidth, (int)newheight);
			using (Graphics gfx = Graphics.FromImage(bmp))
			{
				gfx.InterpolationMode = mode;
				gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
				gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
				gfx.DrawImage(image, 0, 0, newwidth, newheight);
			}
			return bmp;
		}

		private void UpdateTextureView()
		{
			if (listBox1.SelectedIndex == -1)
				return;
			GenericTexture texinfo = textures[listBox1.SelectedIndex];
			Bitmap image;
			if (texinfo.HasMipmaps)
				image = texinfo.MipmapImages[trackBarMipmapLevel.Value];
			else
				image = texinfo.Image;
			textureImage.Image = ScaleBitmapToWindow(image, textureFilteringToolStripMenuItem.Checked ? System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic : System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor);
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
			GenericTexture ti = textures[i];
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
			GenericTexture ti = textures[i];
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
			if (textures[listBox1.SelectedIndex].Gbix != (uint)globalIndex.Value)
			{
				textures[listBox1.SelectedIndex].Gbix = (uint)globalIndex.Value;
				unsaved = true;
			}
		}

		private void mipmapCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			GenericTexture ti = textures[listBox1.SelectedIndex];
			if (ti.HasMipmaps == mipmapCheckBox.Checked)
				return;
			// Add or remove mipmaps
			else
			{
				if (ti.HasMipmaps)
					ti.RemoveMipmaps();
				else
					ti.AddMipmaps();
			}
			// Update surface flags for PAK textures (this should be handled in the library though...)
			if (currentFormat == TextureFormat.PAK)
			{
				if (!mipmapCheckBox.Checked)
					ti.PakMetadata.PakNinjaFlags &= ~NinjaSurfaceFlags.Mipmapped;
				else
					ti.PakMetadata.PakNinjaFlags |= NinjaSurfaceFlags.Mipmapped;
			}
			UpdateTextureInformation();
			unsaved = true;
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
				tex = GenericTexture.LoadTexture(File.ReadAllBytes(((string[])e.Data.GetData(DataFormats.FileDrop, true))[0])).Image;
			}
			else if (e.Data.GetDataPresent(DataFormats.Bitmap))
				tex = new Bitmap((System.Drawing.Image)e.Data.GetData(DataFormats.Bitmap));
			else
				return;
			textures[listBox1.SelectedIndex].ImportBitmap(tex);
			UpdateTextureView();
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
			textures[listBox1.SelectedIndex].ImportBitmap(tex);
			UpdateTextureView();
			UpdateTextureInformation();
			unsaved = true;
		}

		private void PAKEnableAlphaForAll(bool enable)
		{
			if (textures == null || textures.Count == 0)
				return;
			foreach (GenericTexture paktxt in textures)
			{
				if (enable)
					paktxt.PakMetadata.PakGvrFormat = GvrDataFormat.Rgb5a3;
				else
				{
					if ((paktxt.PakMetadata.PakNinjaFlags & NinjaSurfaceFlags.Palettized) != 0)
						paktxt.PakMetadata.PakGvrFormat = GvrDataFormat.Index4;
					else paktxt.PakMetadata.PakGvrFormat = GvrDataFormat.Dxt1;
				}
			}
			if (listBox1.SelectedIndex != -1)
				UpdateTextureInformation();
			unsaved = true;
		}

		private void exportButton_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "png", FileName = textures[listBox1.SelectedIndex].Name + ".png", Filter = "PNG Files|*.png" })
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					Bitmap bmp = new Bitmap(textures[listBox1.SelectedIndex].Image);
					bmp.Save(dlg.FileName);
				}

			listBox1.Select();
		}

		private void highQualityGVMsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			settingsfile.HighQualityGVM = highQualityGVMsToolStripMenuItem.Checked;
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (unsaved)
			{
				DialogResult res = MessageBox.Show(this, "There are unsaved changes. Would you like to save them?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				switch (res)
				{
					case DialogResult.Yes:
						if (!string.IsNullOrEmpty(archiveFilename))
							SaveTextures();
						else
							SaveAs(currentFormat);
						break;
					case DialogResult.Cancel:
						e.Cancel = true;
						break;
					case DialogResult.No:
						break;
				}
			}
			try
			{
				Settings.Save();
				settingsfile.Save();
			}
			catch { }
			;
		}

		private void numericUpDownOrigSizeX_ValueChanged(object sender, EventArgs e)
		{
			if (!suppress)
			{
				GenericTexture tex = textures[listBox1.SelectedIndex];
				if (tex.PvmxOriginalDimensions.Width != (int)numericUpDownOrigSizeX.Value || tex.PvmxOriginalDimensions.Height != (int)numericUpDownOrigSizeY.Value)
				{
					tex.PvmxOriginalDimensions = new Size((int)numericUpDownOrigSizeX.Value, (int)numericUpDownOrigSizeY.Value);
					unsaved = true;
				}
			}
		}

		private void numericUpDownOrigSizeY_ValueChanged(object sender, EventArgs e)
		{
			if (!suppress)
			{
				GenericTexture tex = textures[listBox1.SelectedIndex];
				if (tex.PvmxOriginalDimensions.Width != (int)numericUpDownOrigSizeX.Value || tex.PvmxOriginalDimensions.Height != (int)numericUpDownOrigSizeY.Value)
				{
					tex.PvmxOriginalDimensions = new Size((int)numericUpDownOrigSizeX.Value, (int)numericUpDownOrigSizeY.Value);
					unsaved = true;
				}
			}
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (archiveFilename == null)
				saveAsToolStripMenuItem_Click(sender, e);
			else
				SaveTextures();
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

		private void saveXVMToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveAs(TextureFormat.XVM);
		}

		private void SplitContainer1_Panel2_SizeChanged(object sender, EventArgs e)
		{
			// Maybe not necessary?
			if (listBox1.SelectedIndex > -1)
				UpdateTextureView();
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

		private void newXVMToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NewFile(TextureFormat.XVM);
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
			using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "pvm", Filter = "Texture Files|*.pvm;*.gvm;*.xvm;*.prs;*.pvmx;*.pak" })
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					LoadArchive(dlg.FileName);
					listBox1_SelectedIndexChanged(sender, e);
					unsaved = false;
				}
		}

		private void enablePAKAlphaForAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PAKEnableAlphaForAll(true);
		}

		private void disablePAKAlphaForAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PAKEnableAlphaForAll(false);
		}

		private void texturePreviewZoomTrackBar_Scroll(object sender, EventArgs e)
		{
			UpdateTextureInformation();
		}

		private void textureFilteringToolStripMenuItem_Click(object sender, EventArgs e)
		{
			UpdateTextureInformation();
		}

		private void textureFilteringToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			settingsfile.EnableFiltering = textureFilteringToolStripMenuItem.Checked;
		}

		private void compatibleGVPToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			settingsfile.SACompatiblePalettes = compatibleGVPToolStripMenuItem.Checked;
		}

		private void generateNewGbixToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (textures.Count < 1)
				return;

			DialogResult res = MessageBox.Show(this, "This will generate new GBIX for every texture, are you sure you wish to continue?", "Warning Gbix Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

			if (res != DialogResult.Yes)
			{
				return;
			}

			Random random = new();
			uint randomNumber = (uint)random.Next(100000, 9999999);

			foreach (var texture in textures)
			{
				texture.Gbix = randomNumber;
				randomNumber++;
			}

			listBox1.SelectedIndex = -1;
			listBox1.SelectedItem = null;
			unsaved = true;
		}

		private void useDDSInPAKsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			settingsfile.UseDDSforPAK = ddsPakCompressToolStripMenuItem.Checked;
		}

		private void useDDSInTexturePacksToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			settingsfile.UseDDSforTexPack = ddsTexturePacksToolStripMenuItem.Checked;
		}

		private void useDDSInPVMXToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			settingsfile.UseDDSforPVMX = ddsPvmxToolStripMenuItem.Checked;
		}

		private void checkBoxPAKUseAlpha_CheckedChanged(object sender, EventArgs e)
		{
			GenericTexture pk = textures[listBox1.SelectedIndex];
			// Use alpha
			if (checkBoxPAKUseAlpha.Checked)
				pk.PakMetadata.PakGvrFormat = GvrDataFormat.Rgb5a3;
			// Don't use alpha (palettized)
			else if ((pk.PakMetadata.PakNinjaFlags & NinjaSurfaceFlags.Palettized) != 0)
				pk.PakMetadata.PakGvrFormat = GvrDataFormat.Index4;
			// Don't use alpha (regular)
			else
				pk.PakMetadata.PakGvrFormat = GvrDataFormat.Dxt1;
			UpdateTextureInformation();
		}
		private void checkBoxPAKUseAlpha_Click(object sender, EventArgs e)
		{
			checkBoxPAKUseAlpha_CheckedChanged(sender, e);
			unsaved = true;
		}
		private void exportAllPVRToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ExportAllAs(TextureFiles.PVR);
		}
		private void exportAllGVRToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ExportAllAs(TextureFiles.GVR);
		}
		private void exportAllXVRToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ExportAllAs(TextureFiles.XVR);
		}
		private void exportAllDDSToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ExportAllAs(TextureFiles.DDS);
		}
		private void exportAllPNGToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ExportAllAs(TextureFiles.PNG);
		}
		private void useHQDDSToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			settingsfile.UseHQDDS = ddsPvmxHighQualityToolStripMenuItem.Checked;
		}

		private void useDDSColorSpaceToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			settingsfile.UseDDSColorSpace = ddsPakBestFitToolStripMenuItem.Checked;
		}

		private void usePNGToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			settingsfile.UsePNGforPAK = pngPakToolStripMenuItem.Checked;
		}
		private void useDDSInPAKsUncompressedLargestSizeToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			settingsfile.UseDDSUncompressedForPAK = ddsPakHighQualityToolStripMenuItem.Checked;
		}

		private void useDDSInPAKsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ddsPakCompressToolStripMenuItem.Checked = true;
			ddsPakBestFitToolStripMenuItem.Checked = false;
			ddsPakHighQualityToolStripMenuItem.Checked = false;
			pngPakToolStripMenuItem.Checked = false;
		}

		private void useDDSColorSpaceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ddsPakCompressToolStripMenuItem.Checked = false;
			ddsPakBestFitToolStripMenuItem.Checked = true;
			ddsPakHighQualityToolStripMenuItem.Checked = false;
			pngPakToolStripMenuItem.Checked = false;
		}

		private void usePNGToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ddsPakCompressToolStripMenuItem.Checked = false;
			ddsPakBestFitToolStripMenuItem.Checked = false;
			ddsPakHighQualityToolStripMenuItem.Checked = false;
			pngPakToolStripMenuItem.Checked = true;
		}

		private void useDDSInPAKsUncompressedLargestSizeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ddsPakCompressToolStripMenuItem.Checked = false;
			ddsPakBestFitToolStripMenuItem.Checked = false;
			ddsPakHighQualityToolStripMenuItem.Checked = true;
			pngPakToolStripMenuItem.Checked = false;
		}


		private void exportMaskToolStripMenuItem_Click(object sender, EventArgs e)
		{
			exportMaskToolStripMenuItem.Checked = true;
			exportPalettedIndexedToolStripMenuItem.Checked = exportPalettedFullToolStripMenuItem.Checked = false;
		}

		private void exportPalettedIndexedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			exportPalettedIndexedToolStripMenuItem.Checked = true;
			exportMaskToolStripMenuItem.Checked = exportPalettedFullToolStripMenuItem.Checked = false;
		}

		private void exportPalettedFullToolStripMenuItem_Click(object sender, EventArgs e)
		{
			exportPalettedFullToolStripMenuItem.Checked = true;
			exportMaskToolStripMenuItem.Checked = exportPalettedIndexedToolStripMenuItem.Checked = false;
		}

		private void numericUpDownStartBank_ValueChanged(object sender, EventArgs e)
		{
			if (currentPalette != null)
				currentPalette.StartBank = (short)numericUpDownStartBank.Value;
		}

		private PalettedTextureFormat GetPalettedTextureFormat(GenericTexture info)
		{
			if (info is PvrTexture pvri)
				switch (pvri.PvrDataFormat)
				{
					case PvrDataFormat.Index4:
					case PvrDataFormat.Index4Mipmaps:
						return PalettedTextureFormat.Index4;
					case PvrDataFormat.Index8:
					case PvrDataFormat.Index8Mipmaps:
						return PalettedTextureFormat.Index8;
					default:
						return PalettedTextureFormat.NotIndexed;
				}
			else if (info is GvrTexture gvri)
				switch (gvri.GvrDataFormat)
				{
					case GvrDataFormat.Index4:
						return PalettedTextureFormat.Index4;
					case GvrDataFormat.Index8:
						return PalettedTextureFormat.Index8;
					default:
						return PalettedTextureFormat.NotIndexed;
				}
			else return PalettedTextureFormat.NotIndexed;
		}

		private void addMipmapsToAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (GenericTexture info in textures)
				if (info.CanHaveMipmaps())
				{
					info.HasMipmaps = true;
					info.PakMetadata.PakNinjaFlags |= NinjaSurfaceFlags.Mipmapped;
					info.Encode();
				}
			if (listBox1.SelectedIndex != -1 && textures[listBox1.SelectedIndex].HasMipmaps)
				mipmapCheckBox.Checked = true;
			unsaved = true;
		}

		private void recentFilesToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			LoadArchive(Settings.MRUList[recentFilesToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem)]);
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (currentTextureID != listBox1.SelectedIndex)
			{
				currentTextureID = listBox1.SelectedIndex;
				UpdateTextureInformation();
			}
		}

		private void buttonLoadPalette_Click(object sender, EventArgs e)
		{
			LoadPaletteDialog();
		}

		private void buttonSavePalette_Click(object sender, EventArgs e)
		{
			SavePaletteDialog(textures[listBox1.SelectedIndex].Name);
		}

		private Bitmap GeneratePalettePreview()
		{
			int rows = currentPalette.GetNumColors() / 16;
			Bitmap result = new Bitmap(256 + 16, rows * 16 + rows);
			using (var snoop_r = new BmpPixelSnoop(result))
			{
				int offset_y = 0;
				for (int y = 0; y < rows; y++)
				{
					int offset_x = 0;
					for (int x = 0; x < 16; x++)
					{
						// Draw a 16x16 square
						for (int z = 0; z < 16; z++)
							for (int h = 0; h < 16; h++)
								snoop_r.SetPixel(offset_x + x * 16 + z, offset_y + y * 16 + h, currentPalette.GetColorAnyBank(y * 16 + x));
						offset_x += 1;
					}
					offset_y += 1;
				}
			}
			return result;
		}

		private void UpdateTextureInformation()
		{
			indexTextBox.Text = hexIndexCheckBox.Checked ? listBox1.SelectedIndex.ToString("X") : listBox1.SelectedIndex.ToString();
			int actualPaletteColors = 0;
			bool en = listBox1.SelectedIndex != -1;
			removeTextureButton.Enabled = textureName.Enabled = importButton.Enabled = exportButton.Enabled = saveTextureButton.Enabled = en;
			globalIndex.Enabled = (en && !nonIndexedPAK);
			if (en)
			{
				suppress = true;
				GenericTexture currentTexture = textures[listBox1.SelectedIndex];
				textureUpButton.Enabled = listBox1.SelectedIndex > 0;
				textureDownButton.Enabled = listBox1.SelectedIndex < textures.Count - 1;
				textureName.Text = currentTexture.Name;
				globalIndex.Value = currentTexture.Gbix;
				numericUpDownOrigSizeX.Value = currentTexture.Width;
				numericUpDownOrigSizeY.Value = currentTexture.Height;
				extraFormatLabel.Hide();
				textureSizeLabel.Hide();
				dataFormatLabel.Show();
				pixelFormatLabel.Show();
				checkBoxPAKUseAlpha.Enabled = false;
				checkBoxPAKUseAlpha.Hide();
				extraFormatLabel.Hide();
				textureSizeLabel.Hide();
				if (currentTexture.CanHaveMipmaps())
				{
					mipmapCheckBox.Enabled = true;
					mipmapCheckBox.Checked = currentTexture.HasMipmaps;
				}
				else
					mipmapCheckBox.Checked = mipmapCheckBox.Enabled = false;
				if (currentFormat == TextureFormat.PAK)
				{
					pixelFormatLabel.Text = $"Surface Flags: {GetSurfaceFlagsString(currentTexture.PakMetadata.PakNinjaFlags)}";
					extraFormatLabel.Text = $"PAK GVR Format: {currentTexture.PakMetadata.PakGvrFormat}";
					pixelFormatLabel.Show();
					extraFormatLabel.Show();
					checkBoxPAKUseAlpha.Enabled = true;
					checkBoxPAKUseAlpha.Show();
					checkBoxPAKUseAlpha.Checked = currentTexture.PakMetadata.PakGvrFormat == GvrDataFormat.Rgb5a3;
				}
				else if (currentFormat == TextureFormat.PVMX)
				{
					textureSizeLabel.Text = $"Actual Size: {textures[listBox1.SelectedIndex].Image.Width}x{textures[listBox1.SelectedIndex].Image.Height}";
					textureSizeLabel.Show();
					numericUpDownOrigSizeX.Enabled = numericUpDownOrigSizeY.Enabled = true;
					if (currentTexture.PvmxOriginalDimensions.Width != 0)
					{
						numericUpDownOrigSizeX.Value = currentTexture.PvmxOriginalDimensions.Width;
						numericUpDownOrigSizeY.Value = currentTexture.PvmxOriginalDimensions.Height;
					}
					else
					{
						numericUpDownOrigSizeX.Value = currentTexture.Image.Width;
						numericUpDownOrigSizeY.Value = currentTexture.Image.Height;
					}
				}
				switch (currentTexture)
				{
					case PvrTexture pvr:
						dataFormatLabel.Text = $"Data Format: {pvr.PvrDataFormat}";
						pixelFormatLabel.Text = $"Pixel Format: {pvr.PvrPixelFormat}";
						numericUpDownOrigSizeX.Enabled = numericUpDownOrigSizeY.Enabled = false;
						numericUpDownOrigSizeX.Value = pvr.Image.Width;
						numericUpDownOrigSizeY.Value = pvr.Image.Height;
						switch (pvr.PvrDataFormat)
						{
							case PvrDataFormat.Index4:
							case PvrDataFormat.Index4Mipmaps:
							case PvrDataFormat.Index8:
							case PvrDataFormat.Index8Mipmaps:
								string folder = !string.IsNullOrEmpty(archiveFilename) ? Path.GetDirectoryName(archiveFilename) + "\\" : "";
								string pvppath = folder + pvr.Name + ".pvp";
								if (File.Exists(pvppath))
								{
									currentPalette = new TexturePalette(File.ReadAllBytes(pvppath), settingsfile.SACompatiblePalettes);
									paletteSet = 0;//Math.Min(currentPalette.GetMaxBanks(pvr.PvrDataFormat == PvrDataFormat.Index8 || pvr.PvrDataFormat == PvrDataFormat.Index8Mipmaps), paletteSet);
								}
								actualPaletteColors = currentPalette.GetNumColors();

								// Failsafe check if the palette has a smaller number of colors than the indexed image expects
								int neededcolors = (pvr.PvrDataFormat == PvrDataFormat.Index4 | pvr.PvrDataFormat == PvrDataFormat.Index4Mipmaps) ? 16 : 256;
								if (neededcolors - actualPaletteColors > 0)
									currentPalette.AddColors(new Color[neededcolors - actualPaletteColors]);
								try
								{
									pvr.SetPalette(currentPalette, paletteSet);
								}
								catch (Exception ex)
								{
									MessageBox.Show(this, "Palette data couldn't be applied: " + ex.Message.ToString(), "Palette application error", MessageBoxButtons.OK, MessageBoxIcon.Error);
									currentPalette = defaultPalette;
									pvr.SetPalette(defaultPalette, 0);
								}
								break;
							default:
								break;
						}
						break;
					case GvrTexture gvr:
						dataFormatLabel.Text = $"GVR Data Format: {gvr.GvrDataFormat}";
						pixelFormatLabel.Text = $"GVR Palette Format: {gvr.GvrPaletteFormat}";
						numericUpDownOrigSizeX.Enabled = numericUpDownOrigSizeY.Enabled = false;
						numericUpDownOrigSizeX.Value = gvr.Image.Width;
						numericUpDownOrigSizeY.Value = gvr.Image.Height;
						checkBoxPAKUseAlpha.Enabled = false;
						checkBoxPAKUseAlpha.Hide();
						extraFormatLabel.Visible = false;
						textureSizeLabel.Hide();
						switch (gvr.GvrDataFormat)
						{
							case GvrDataFormat.Index4:
							case GvrDataFormat.Index8:
							/*
								actualPaletteColors = currentPalette.GetNumColors()();
								string folder = !string.IsNullOrEmpty(archiveFilename) ? Path.GetDirectoryName(archiveFilename) + "\\" : "";
								string gvppath = folder + gvr.Name + ".gvp";
								if (!customPaletteLoaded && File.Exists(gvppath))
								{
									currentPalette = new TexturePalette(File.ReadAllBytes(gvppath), settingsfile.SACompatiblePalettes);
									paletteSet = Math.Min(currentPalette.GetMaxBanks(gvr.DataFormat == GvrDataFormat.Index8), paletteSet);
								}
								if (!paletteApplied)
								{
									if (gvr.TextureData == null)
									{
										gvr.PixelFormat = GvrPixelFormat.IntensityA8; // Indexed GVRs seem to have this set always
										GvrTextureEncoder enc = new GvrTextureEncoder(gvr.Image, gvr.PixelFormat, gvr.DataFormat);
										gvr.TextureData = new MemoryStream();
										enc.Save(gvr.TextureData);
										gvr.TextureData.Seek(0, SeekOrigin.Begin);
									}
									GvrTexture gt = new GvrTexture(gvr.TextureData.ToArray());
									currentPalette.IsGVP = true;
									// Failsafe check if the palette has a smaller number of colors than the indexed image expects
									int neededcolors = (gvr.DataFormat == GvrDataFormat.Index4) ? 16 : 256;
									if (neededcolors - actualPaletteColors > 0)
									{
										for (int i = 0; i < neededcolors - actualPaletteColors; i++)
											currentPalette.Colors.Add(Color.Black);
									}
									try
									{
										gt.SetPalette(new GvpPalette(currentPalette.GetBytes()), paletteSet);
									}
									catch (Exception)
									{
										MessageBox.Show(this, "Palette data couldn't be applied. This can be caused by using 16-color palettes on 256-color indexed images. Select a correct palette file and try again.", "Palette application error", MessageBoxButtons.OK, MessageBoxIcon.Error);
										currentPalette = new TexturePalette(defaultPalette.GetBytes());
										gt.SetPalette(new GvpPalette(currentPalette.GetBytes()), 0);
									}
									paletteApplied = true;
									gvr.Image = gt.ToBitmap();
								}
								break;*/
							default:
								break;
						}
						break;
					case XvrTexture xvr:
						pixelFormatLabel.Text = $"XVR format: {xvr.XvrType}";
						dataFormatLabel.Text = $"Data Format: DDS";
						dataFormatLabel.Hide();
						dataFormatLabel.Show();
						pixelFormatLabel.Show();
						numericUpDownOrigSizeX.Enabled = numericUpDownOrigSizeY.Enabled = false;
						numericUpDownOrigSizeX.Value = xvr.Image.Width;
						numericUpDownOrigSizeY.Value = xvr.Image.Height;
						checkBoxPAKUseAlpha.Enabled = false;
						checkBoxPAKUseAlpha.Hide();
						extraFormatLabel.Hide();
						textureSizeLabel.Hide();
						break;
					case DdsTexture dds:
						dataFormatLabel.Text = $"DDS Data Format: {dds.DdsFormat}";
						numericUpDownOrigSizeX.Enabled = numericUpDownOrigSizeY.Enabled = false;
						break;
					default:
						break;
				}
				suppress = false;
				UpdateTextureMipmap();
				UpdateTextureView();
				ShowHidePaletteInfo(actualPaletteColors);				
			}
			else
			{
				textureUpButton.Enabled = false;
				textureDownButton.Enabled = false;
				mipmapCheckBox.Enabled = false;
				dataFormatLabel.Text = $"Data Format: Unknown";
				pixelFormatLabel.Text = $"Pixel Format: Unknown";
				textureSizeLabel.Hide();
				indexTextBox.Text = "-1";
				textureImage.Image = new Bitmap(64, 64);
				UpdateTextureMipmap();
				UpdateTextureView();
				ShowHidePaletteInfo(actualPaletteColors);
			}
		}

		private List<GenericTexture> GetTexturesFromFile(string fname)
		{
			nonIndexedPAK = false;
			byte[] datafile = File.ReadAllBytes(fname);
			if (Path.GetExtension(fname).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				datafile = PRS.Decompress(datafile);
			List<GenericTexture> newtextures;
			if (PVMXFile.Identify(datafile))
			{
				PVMXFile pvmx = new PVMXFile(datafile);
				newtextures = new List<GenericTexture>();
				foreach (PVMXFile.PVMXEntry pvmxe in pvmx.Entries)
				{
					GdiTexture texinfo = (GdiTexture)GenericTexture.LoadTexture(pvmxe.Data);
					texinfo.Name = Path.GetFileNameWithoutExtension(pvmxe.Name);
					texinfo.Gbix = pvmxe.GBIX;
					texinfo.PvmxOriginalDimensions.Width = pvmxe.Width;
					texinfo.PvmxOriginalDimensions.Height = pvmxe.Height;
					newtextures.Add(texinfo);
				}
			}
			else if (PAKFile.Identify(fname))
			{
				PAKFile pak = new PAKFile(fname);
				string filenoext = Path.GetFileNameWithoutExtension(fname).ToLowerInvariant();
				bool hasIndex = false;
				string indexName = "";
				foreach (PAKFile.PAKEntry fl in pak.Entries)
				{
					// Search for the correct index file
					if (fl.Name.Equals(filenoext + ".inf", StringComparison.OrdinalIgnoreCase))
					{
						hasIndex = true;
						indexName = filenoext + ".inf";
						break;
					}
					// Search for an incorrectly named index file
					else if (Path.GetExtension(fl.Name).ToLowerInvariant() == ".inf")
					{
						MessageBox.Show(this, "The index file name must match the PAK file name for the game to recognize it. Please resave the archive with the desired filename.", "Texture Editor Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						hasIndex = true;
						indexName = fl.Name;
					}
				}
				// Handle non-indexed PAKs in the SOC folder
				if (!hasIndex)
				{
					nonIndexedPAK = true;
					newtextures = new List<GenericTexture>(pak.Entries.Count);
					foreach (PAKFile.PAKEntry fl in pak.Entries)
					{
						GenericTexture paktex = GenericTexture.LoadTexture(fl.Data);
						fl.Name = Path.GetFileNameWithoutExtension(fl.Name);
					}
				}
				// Handle indexed PAKs
				else
				{
					nonIndexedPAK = false;
					byte[] inf = pak.Entries.Single((file) => file.Name.Equals(indexName, StringComparison.OrdinalIgnoreCase)).Data;
					newtextures = new List<GenericTexture>(inf.Length / 0x3C);
					for (int i = 0; i < inf.Length; i += 0x3C)
					{
						// Load a PAK INF entry
						byte[] pakentry = new byte[0x3C];
						Array.Copy(inf, i, pakentry, 0, 0x3C);
						PAKInfEntry entry = new PAKInfEntry(pakentry);
						// Load texture data
						try
						{
							// The substring thing removes the ".dds" extension in the entry name
							// Names are trimmed to avoid trailing spaces which the game ignores but the tools don't
							byte[] dds = pak.Entries.First((file) => Path.GetExtension(file.Name) != ".inf" && file.Name.Substring(0, file.Name.Length - 4).Trim().Equals(entry.GetFilename().Trim(), StringComparison.OrdinalIgnoreCase)).Data;
							GenericTexture paktx = GenericTexture.LoadTexture(dds);
							paktx.PakMetadata.PakGvrFormat = entry.TypeInf;
							paktx.PakMetadata.PakNinjaFlags = entry.fSurfaceFlags;
							paktx.Gbix = entry.GlobalIndex;
							paktx.Name = entry.GetFilename().Trim();
							newtextures.Add(paktx);
						}
						catch (Exception ex)
						{
							MessageBox.Show(this, $"Could not add texture {entry.GetFilename().Trim() + ".dds: " + ex.Message.ToString() + "."}", "Texture Editor Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
					}
				}
			}
			else
			{
				// If it's a PB file, convert it to PVM and load as PVM
				if (PBFile.Identify(datafile))
					datafile = new PBFile(datafile).GetPVM().GetBytes();
				// If it isn't, try loading a PVM/GVM/XVM
				PuyoArchiveType idResult = PuyoFile.Identify(datafile);
				if (idResult == PuyoArchiveType.Unknown)
				{
					MessageBox.Show(this, "Unknown archive type: " + fname + ".", "Texture Editor Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return new List<GenericTexture>();
				}
				PuyoFile arc = new PuyoFile(datafile);
				newtextures = new List<GenericTexture>(arc.Entries.Count);
				foreach (GenericArchiveEntry file in arc.Entries)
				{
					if (file is PVMEntry pvme)
						newtextures.Add(new PvrTexture(file.Data) { Name = file.Name });
					else if (file is GVMEntry gvme)
						newtextures.Add(new GvrTexture(file.Data) { Name = file.Name });
					else if (file is XVMEntry xvme)
						newtextures.Add(new XvrTexture(file.Data) { Name = file.Name });
				}
			}
			// Check if GenericTexture match the current format and convert if necessary
			for (int i = 0; i < newtextures.Count; i++)
			{
				switch (currentFormat)
				{
					case TextureFormat.PVM:
						if (!(newtextures[i] is PvrTexture))
							newtextures[i] = newtextures[i].ToPvr();
						break;
					case TextureFormat.GVM:
						if (!(newtextures[i] is GvrTexture))
							newtextures[i] = newtextures[i].ToGvr();
						break;
					case TextureFormat.PVMX:
						if (!(newtextures[i] is GdiTexture))
							newtextures[i] = newtextures[i].ToGdi();
						break;
					case TextureFormat.PAK:
						if (!(newtextures[i] is GvrTexture) && !(newtextures[i] is DdsTexture) && !(newtextures[i] is GdiTexture))
							newtextures[i] = newtextures[i].ToDds();
						break;
					case TextureFormat.XVM:
						if (!(newtextures[i] is XvrTexture))
							newtextures[i] = newtextures[i].ToXvr();
						break;
				}
			}
			return newtextures;
		}

		private bool LoadArchive(string filename)
		{
			// Load file
			byte[] datafile = File.ReadAllBytes(filename);
			if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				datafile = PRS.Decompress(datafile);

			// Check if the file is a PVR/GVR/XVR
			PuyoArchiveType puyotype = PuyoArchiveType.Unknown;
			if (PvrTexture.Identify(datafile))
			{
				puyotype = PuyoArchiveType.PVMFile;
				currentFormat = TextureFormat.PVM;
			}
			else if (GvrTexture.Identify(datafile))
			{
				puyotype = PuyoArchiveType.GVMFile;
				currentFormat = TextureFormat.GVM;
			}
			else if (XvrTexture.Identify(datafile))
			{
				puyotype = PuyoArchiveType.XVMFile;
				currentFormat = TextureFormat.XVM;
			}
			// If the file is a single PVR/GVR/XVR, create a PVM/GVM/XVM archive and add the texture to it.
			if (puyotype != PuyoArchiveType.Unknown)
			{
				string[] otherfiles = Directory.GetFiles(Path.GetDirectoryName(filename), "*" + Path.GetExtension(filename), SearchOption.TopDirectoryOnly);
				PuyoFile arc = new PuyoFile(puyotype);
				textures.Clear();
				int selIndex = 0;
				for (int i = 0; i < otherfiles.Length; i++)
				{
					try
					{
						datafile = File.ReadAllBytes(otherfiles[i]);
						if (Path.GetFileName(otherfiles[i]) == Path.GetFileName(filename))
							selIndex = i;
						switch (puyotype)
						{
							case PuyoArchiveType.PVMFile:
								arc.Entries.Add(new PVMEntry(datafile, Path.GetFileNameWithoutExtension(otherfiles[i])));
								MemoryStream strp = new MemoryStream();
								textures.Add(new PvrTexture(arc.Entries[i].Data) { Name = Path.GetFileNameWithoutExtension(otherfiles[i]).Split('.')[0] });
								break;
							case PuyoArchiveType.GVMFile:
								arc.Entries.Add(new GVMEntry(datafile, Path.GetFileNameWithoutExtension(otherfiles[i])));
								textures.Add(new GvrTexture(arc.Entries[i].Data) { Name = Path.GetFileNameWithoutExtension(otherfiles[i]).Split('.')[0] });
								break;
							case PuyoArchiveType.XVMFile:
								arc.Entries.Add(new XVMEntry(datafile, Path.GetFileNameWithoutExtension(otherfiles[i])));
								textures.Add(new XvrTexture(arc.Entries[i].Data) { Name = Path.GetFileNameWithoutExtension(otherfiles[i]).Split('.')[0] });
								break;
						}
					}
					catch (Exception ex)
					{
						MessageBox.Show(this, "Folder loading cancelled. Could not add texture " + otherfiles[i] + ": " + ex.ToString(), "Texture Editor Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						break;
					}
				}
				listBox1.Items.Clear();
				listBox1.Items.AddRange(textures.Select((item) => item.Name).ToArray());
				UpdateTextureCount();
				UpdateMRUList(Path.GetFullPath(filename));
				listBox1.SelectedIndex = listBox1.Items.Count == 0 ? -1 : selIndex;
				return true;
			}
			// Otherwise load the file as an archive
			if (PVMXFile.Identify(datafile))
				currentFormat = TextureFormat.PVMX;
			else if (PAKFile.Identify(filename))
				currentFormat = TextureFormat.PAK;
			else
			{
				if (PBFile.Identify(datafile))
					datafile = new PBFile(datafile).GetPVM().GetBytes();
				PuyoArchiveType identifyResult = PuyoFile.Identify(datafile);
				switch (identifyResult)
				{
					case PuyoArchiveType.PVMFile:
						currentFormat = TextureFormat.PVM;
						break;
					case PuyoArchiveType.GVMFile:
						currentFormat = TextureFormat.GVM;
						break;
					case PuyoArchiveType.XVMFile:
						currentFormat = TextureFormat.XVM;
						break;
					case PuyoArchiveType.Unknown:
						MessageBox.Show(this, "Unknown archive type: \"" + filename + "\".", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
				}
			}

			// Load textures
			List<GenericTexture> newtextures = GetTexturesFromFile(filename);
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

		private void SaveTextures()
		{
			byte[] data;
			using (MemoryStream str = new MemoryStream())
			{
				switch (currentFormat)
				{
					case TextureFormat.PVM:
						PuyoFile puyo = new PuyoFile(PuyoArchiveType.PVMFile);
						foreach (PvrTexture tex in textures)
							puyo.Entries.Add(new PVMEntry(tex.GetBytes(), tex.Name));
						data = puyo.GetBytes();
						if (Path.GetExtension(archiveFilename).Equals(".pb", StringComparison.OrdinalIgnoreCase))
							data = puyo.GetPB().GetBytes();
						unsaved = false;
						break;
					case TextureFormat.GVM:
						PuyoFile puyog = new PuyoFile(PuyoArchiveType.GVMFile);
						foreach (GvrTexture tex in textures)
							puyog.Entries.Add(new GVMEntry(tex.GetBytes(), tex.Name));
						data = puyog.GetBytes();
						unsaved = false;
						break;
					case TextureFormat.XVM:
						PuyoFile puyox = new PuyoFile(PuyoArchiveType.XVMFile);
						foreach (XvrTexture tex in textures)
							puyox.Entries.Add(new XVMEntry(tex.GetBytes(), tex.Name));
						data = puyox.GetBytes();
						unsaved = false;
						break;
					case TextureFormat.PVMX:
						PVMXFile pvmx = new PVMXFile();
						foreach (GenericTexture tex in textures)
						{
							Size size = new Size(tex.Image.Width, tex.Image.Height);
							if (tex.PvmxOriginalDimensions.Width != 0)
								size = new Size(tex.PvmxOriginalDimensions.Width, tex.PvmxOriginalDimensions.Height);
							pvmx.Entries.Add(new PVMXFile.PVMXEntry(tex.Name + TextureFunctions.IdentifyTextureFileExtension(tex.RawData), tex.Gbix, tex.GetBytes(), size.Width, size.Height));
						}
						File.WriteAllBytes(archiveFilename, pvmx.GetBytes());
						unsaved = false;
						return;
					case TextureFormat.PAK:
						PAKFile pak = new PAKFile();
						string filenoext = Path.GetFileNameWithoutExtension(archiveFilename).ToLowerInvariant();
						pak.FolderName = filenoext;
						string longdir = nonIndexedPAK ? "..\\..\\..\\sonic2\\resource\\gd_pc\\soc\\" + filenoext : "..\\..\\..\\sonic2\\resource\\gd_pc\\prs\\" + filenoext;
						List<byte> inf = new List<byte>(textures.Count * 0x3C);
						// Add individual PAK entries
						foreach (GenericTexture item in textures)
						{
							// TODO: PAK quality options and DDS/PNG
							string name = item.Name.ToLowerInvariant();
							if (name.Length > 0x1C)
								name = name.Substring(0, 0x1C);
							name = name.Trim();
							pak.Entries.Add(new PAKFile.PAKEntry(name + ".dds", longdir + '\\' + name + ".dds", item.GetBytes().ToArray()));
							// Create a new PAK INF entry
							PAKInfEntry entry = new PAKInfEntry();
							byte[] namearr = Encoding.ASCII.GetBytes(name);
							Array.Copy(namearr, entry.Filename, namearr.Length);
							entry.GlobalIndex = item.Gbix;
							entry.nWidth = (uint)item.Image.Width;
							entry.nHeight = (uint)item.Image.Height;
							entry.TypeInf = entry.PixelFormatInf = item.PakMetadata.PakGvrFormat;
							entry.fSurfaceFlags = item.PakMetadata.PakNinjaFlags;
							if (item.HasMipmaps)
								entry.fSurfaceFlags |= NinjaSurfaceFlags.Mipmapped;
							inf.AddRange(entry.GetBytes());
						}
						// Insert the INF file
						if (!nonIndexedPAK)
							pak.Entries.Insert(0, new PAKFile.PAKEntry(filenoext + ".inf", longdir + '\\' + filenoext + ".inf", inf.ToArray()));
						pak.Save(archiveFilename);
						unsaved = false;
						return;
					default:
						return;
				}
			}
			if (Path.GetExtension(archiveFilename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				File.WriteAllBytes(archiveFilename, PRS.Compress(data, 255));
			else
				File.WriteAllBytes(archiveFilename, data);
			unsaved = false;
		}

		// TODO: Add DDS/PNG and quality options
		private void ConvertTextures(TextureFormat newfmt)
		{
			for (int i = 0; i < textures.Count; i++)
			{
				switch (newfmt)
				{
					case TextureFormat.PVM:
						textures[i] = textures[i].ToPvr();
						break;
					case TextureFormat.GVM:
						textures[i] = textures[i].ToGvr();
						break;
					case TextureFormat.XVM:
						textures[i] = textures[i].ToXvr();
						break;
					case TextureFormat.PVMX:
						textures[i] = textures[i].ToGdi();
						break;
					case TextureFormat.PAK:
						textures[i] = textures[i].ToDds();
						break;
				}
			}
			currentFormat = newfmt;
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
				case TextureFormat.XVM:
					ext = "xvm";
					break;
				case TextureFormat.PVM:
				default:
					ext = "pvm";
					break;
			}
			using (SaveFileDialog dlg = new SaveFileDialog() { FilterIndex = (int)savefmt + 1, DefaultExt = ext, Filter = "PVM Files|*.pvm;*.prs;*.pb|GVM Files|*.gvm;*.prs|PVMX Files|*.pvmx|PAK Files|*.pak|XVM Files|*.xvm;*.prs" })
			{
				if (!string.IsNullOrEmpty(archiveFilename))
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

		private void ExportAllAs(TextureFiles expfmt)
		{
			string ext;
			switch (expfmt)
			{
				case TextureFiles.GVR:
					ext = ".gvr";
					break;
				case TextureFiles.PNG:
					ext = ".png";
					break;
				case TextureFiles.DDS:
					ext = ".dds";
					break;
				case TextureFiles.XVR:
					ext = ".xvr";
					break;
				case TextureFiles.PVR:
				default:
					ext = ".pvr";
					break;
			}
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
						foreach (GenericTexture tex in textures)
						{
							byte[] exportData;
							switch (expfmt)
							{
								case TextureFiles.PVR:
									exportData = tex.ToPvr().RawData;
									break;
								case TextureFiles.GVR:
									exportData = tex.ToGvr().RawData;
									break;
								case TextureFiles.XVR:
									exportData = tex.ToXvr().RawData;
									break;
								case TextureFiles.DDS:
									exportData = tex.ToDds().RawData;
									break;
								case TextureFiles.PNG:
								default:
									exportData = tex.ToGdi().RawData;
									break;
							}
							texList.WriteLine("{0},{1},{2}x{3}", tex.Gbix, tex.Name + TextureFunctions.IdentifyTextureFileExtension(exportData), tex.Image.Width, tex.Image.Height);
							File.WriteAllBytes(Path.Combine(dir, tex.Name + ext), exportData);
						}
					}
				}
			}
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
								byte[] file = File.ReadAllBytes(Path.Combine(dir, split[1]));
								uint check = BitConverter.ToUInt32(file, 0);
								MemoryStream str = new MemoryStream(file);
								GenericTexture tex = GenericTexture.LoadTexture(file);
								tex.Name = name;
								tex.Gbix = gbix;
								switch (currentFormat)
								{
									case TextureFormat.PVM:
										textures.Add(tex.ToPvr());
										break;
									case TextureFormat.GVM:
										textures.Add(tex.ToGvr());
										break;
									case TextureFormat.PVMX:
										if (split.Length > 2)
										{
											string[] dim = split[2].Split('x');
											if (dim.Length > 1)
												tex.PvmxOriginalDimensions = new Size(int.Parse(dim[0]), int.Parse(dim[1]));
										}
										if (tex is DdsTexture)
											textures.Add(tex);
										else
											textures.Add(tex.ToGdi());
										break;
									case TextureFormat.PAK:
										textures.Add(tex.ToDds());
										break;
									case TextureFormat.XVM:
										textures.Add(tex.ToXvr());
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
						foreach (GenericTexture tex in textures)
						{
							byte[] exportData;
							PalettedTextureFormat indexedfmt = GetPalettedTextureFormat(tex);
							// Indexed
							if (indexedfmt != PalettedTextureFormat.NotIndexed)
							{
								System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(tex.Image);
								bmp = ProcessIndexedBitmap(tex, indexedfmt);
								MemoryStream ms = new MemoryStream();
								bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
								File.WriteAllBytes(Path.Combine(dir, tex.Name + ".png"), ms.ToArray());
								exportData = tex.RawData = ms.ToArray();
							}
							// Non-indexed
							else
							{
								if (ddsTexturePacksToolStripMenuItem.Checked)
									exportData = tex.ToDds().RawData;
								else
									exportData = tex.ToGdi().RawData;
							}
							if (tex.PvmxOriginalDimensions.Width != 0)
								texList.WriteLine("{0},{1},{2}x{3}", tex.Gbix, tex.Name + TextureFunctions.IdentifyTextureFileExtension(exportData), tex.PvmxOriginalDimensions.Width, tex.PvmxOriginalDimensions.Height);
							else
								texList.WriteLine("{0},{1},{2}x{3}", tex.Gbix, tex.Name + TextureFunctions.IdentifyTextureFileExtension(exportData), tex.Image.Width, tex.Image.Height);
							File.WriteAllBytes(Path.Combine(dir, tex.Name + TextureFunctions.IdentifyTextureFileExtension(exportData)), exportData.ToArray());
						}
					}
				}
			}
		}

		private void addTextureButton_Click(object sender, EventArgs e)
		{
			string defext = null;
			string filter = "Supported Files|*.prs;*.pvm;*.pb;*.gvm;*.pvmx;*.pak;*.pvr;*.gvr;*.xvr;*.png;*.jpg;*.jpeg;*.gif;*.bmp;*.dds";
			switch (currentFormat)
			{
				case TextureFormat.PVM:
					defext = "pvr";
					break;
				case TextureFormat.GVM:
					defext = "gvr";
					break;
				case TextureFormat.PVMX:
					defext = "png";
					break;
				case TextureFormat.PAK:
					defext = "dds";
					break;
				case TextureFormat.XVM:
					defext = "xvr";
					break;
			}
			using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = defext, Filter = filter, Multiselect = true })
			{
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					uint gbix = textures.Count == 0 ? 0 : textures.Max((item) => item.Gbix);
					if (gbix != uint.MaxValue)
						gbix++;
					listBox1.BeginUpdate();
					foreach (string file in dlg.FileNames)
					{
						switch (Path.GetExtension(file).ToLowerInvariant())
						{
							// Add textures from archives
							case ".prs":
							case ".pvm":
							case ".pb":
							case ".gvm":
							case ".xvm":
							case ".pak":
							case ".pvmx":
								foreach (GenericTexture tex in GetTexturesFromFile(file))
								{
									textures.Add(tex);
									listBox1.Items.Add(tex.Name);
								}
								break;
							// Add individual textures
							default:
								{
									string name = Path.GetFileNameWithoutExtension(file);
									byte[] dt = File.ReadAllBytes(file);
									// Add textures that match current archive format
									if (currentFormat == TextureFormat.PVM && PvrTexture.Identify(dt))
										textures.Add(new PvrTexture(dt, 0, name));
									else if (currentFormat == TextureFormat.GVM && GvrTexture.Identify(dt))
										textures.Add(new GvrTexture(dt, 0, name));
									else if (currentFormat == TextureFormat.XVM && XvrTexture.Identify(dt))
										textures.Add(new XvrTexture(dt, 0, name));
									else
									{
										GenericTexture gdi = GenericTexture.LoadTexture(dt);
										gdi.Name = name;
										gdi.Gbix = gbix;
										switch (currentFormat)
										{
											case TextureFormat.PVM:
												textures.Add(gdi.ToPvr());
												break;
											case TextureFormat.GVM:
												textures.Add(gdi.ToGvr());
												break;
											case TextureFormat.XVM:
												textures.Add(gdi.ToXvr());
												break;
											case TextureFormat.PVMX:
												textures.Add(gdi);
												break;
											case TextureFormat.PAK:
												if (Path.GetFileName(file).EndsWith(".dds", StringComparison.OrdinalIgnoreCase))
												{
													textures.Add(gdi.ToDds());
												}
												else
												{
													Bitmap[] mips = null;
													NinjaSurfaceFlags mipflag = NinjaSurfaceFlags.Mipmapped;
													if (PvrTexture.Identify(dt))
													{
														mips = new PvrTexture(dt).MipmapImages;
														mipflag = new PvrTexture(dt).HasMipmaps ? NinjaSurfaceFlags.Mipmapped : 0;
													}
													if (GvrTexture.Identify(dt))
													{
														mips = new GvrTexture(dt).MipmapImages;
														mipflag = new GvrTexture(dt).HasMipmaps ? NinjaSurfaceFlags.Mipmapped : 0;
													}
													if (XvrTexture.Identify(dt))
													{
														mipflag = new XvrTexture(dt).HasMipmaps ? NinjaSurfaceFlags.Mipmapped : 0;
													}
													textures.Add(gdi.ToDds());
												}
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

		private void importButton_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "pvr", Filter = "Texture Files|*.pvr;*.gvr;*.xvr;*.png;*.jpg;*.jpeg;*.gif;*.bmp;*.dds" };
			if (!string.IsNullOrEmpty(listBox1.GetItemText(listBox1.SelectedItem)))
				dlg.FileName = listBox1.GetItemText(listBox1.SelectedItem);
			DialogResult res = dlg.ShowDialog(this);
			if (res == DialogResult.OK)
			{
				string name = Path.GetFileNameWithoutExtension(dlg.FileName);
				byte[] data = File.ReadAllBytes(dlg.FileName);
				GenericTexture tex = GenericTexture.LoadTexture(data);
				switch (currentFormat)
				{
					case TextureFormat.PVM:
						PvrTexture oldpvr = (PvrTexture)textures[listBox1.SelectedIndex];
						if (tex is PvrTexture)
							textures[listBox1.SelectedIndex] = new PvrTexture(data) { Name = oldpvr.Name, Gbix = oldpvr.Gbix };
						else
							textures[listBox1.SelectedIndex] = tex.ToPvr();
						break;
					case TextureFormat.GVM:
						GvrTexture oldgvr = (GvrTexture)textures[listBox1.SelectedIndex];
						if (tex is GvrTexture)
							textures[listBox1.SelectedIndex] = new GvrTexture(data) { Name = oldgvr.Name, Gbix = oldgvr.Gbix };
						else
							textures[listBox1.SelectedIndex] = tex.ToGvr();
						break;
					case TextureFormat.XVM:
						XvrTexture oldxvr = (XvrTexture)textures[listBox1.SelectedIndex];
						if (tex is XvrTexture)
							textures[listBox1.SelectedIndex] = new XvrTexture(data) { Name = oldxvr.Name, Gbix = oldxvr.Gbix };
						else
							textures[listBox1.SelectedIndex] = tex.ToXvr();
						break;
					case TextureFormat.PVMX:
						GenericTexture oldpvmx = textures[listBox1.SelectedIndex];
						tex.Name = oldpvmx.Name;
						tex.Gbix = oldpvmx.Gbix;
						textures[listBox1.SelectedIndex] = tex.ToGdi();
						break;
					case TextureFormat.PAK:
						GenericTexture oldpak = textures[listBox1.SelectedIndex];
						tex.Name = oldpak.Name;
						tex.Gbix = oldpak.Gbix;
						tex.PakMetadata = oldpak.PakMetadata;
						textures[listBox1.SelectedIndex] = tex.ToDds();
						break;
					default:
						break;
				}
				UpdateTextureInformation();
				unsaved = true;
			}
			listBox1.Select();
		}

		private void saveTextureButton_Click(object sender, EventArgs e)
		{
			string ext = "png";
			switch (textures[listBox1.SelectedIndex])
			{
				case PvrTexture:
					ext = ".pvr";
					break;
				case GvrTexture:
					ext = ".gvr";
					break;
				case XvrTexture:
					ext = ".xvr";
					break;
				case DdsTexture:
					ext = ".dds";
					break;
				case GdiTexture:
					ext = ".png";
					break;
			}
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = ext, FileName = textures[listBox1.SelectedIndex].Name, Filter = "All Files|*.*" })
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					File.WriteAllBytes(dlg.FileName, textures[listBox1.SelectedIndex].RawData);
				}
			listBox1.Select();
		}

		#region Palette related functions

		private void ShowHidePaletteInfo(int palcolors)
		{
			if (listBox1.SelectedIndex == -1)
			{
				labelPaletteFormat.Hide();
				palettePreview.Hide();
				buttonLoadPalette.Hide();
				buttonSavePalette.Hide();
				buttonResetPalette.Hide();
				panelPaletteInfo.Hide();
				return;
			}
			PalettedTextureFormat dataformat = GetPalettedTextureFormat(textures[listBox1.SelectedIndex]);
			switch (dataformat)
			{
				case PalettedTextureFormat.Index4:
				case PalettedTextureFormat.Index8:
					int rowsize = dataformat == PalettedTextureFormat.Index8 ? 256 : 16;
					int rowsize_stored = rowsize - currentPalette.StartColor;
					int numrows = currentPalette.GetNumColors() / rowsize_stored;
					comboBoxCurrentPaletteBank.Items.Clear();
					int maxbanks = Math.Max(1, currentPalette.GetNumColors() / rowsize);
					numericUpDownStartBank.Maximum = maxbanks - 1;
					numericUpDownStartBank.Value = Math.Max(0, Math.Min(currentPalette.StartBank, numericUpDownStartBank.Maximum));
					numericUpDownStartColor.Maximum = rowsize - 1;
					numericUpDownStartColor.Value = Math.Max(0, Math.Min(currentPalette.StartColor, numericUpDownStartColor.Maximum));
					currentPalette.StartBank = Math.Max((short)0, Math.Min((short)(maxbanks - 1), currentPalette.StartBank)); // Apparently some PVP files in PSO have broken bank IDs
					for (int i = 0; i < numrows; i++)
						if (currentPalette.StartBank + i < maxbanks)
							comboBoxCurrentPaletteBank.Items.Add((currentPalette.StartBank + i).ToString());
					paletteSet = comboBoxCurrentPaletteBank.SelectedIndex = Math.Min(comboBoxCurrentPaletteBank.Items.Count - 1, paletteSet);
					palettePreview.Image = GeneratePalettePreview();
					labelPaletteFormat.Text = currentPalette.Info() + " : " + palcolors.ToString() + "/" + currentPalette.GetNumColors().ToString() + " colors";
					labelPaletteFormat.Show();
					palettePreview.Show();
					panelPaletteInfo.Show();
					buttonLoadPalette.Show();
					buttonSavePalette.Show();
					buttonResetPalette.Show();
					textureSizeLabel.Hide();
					numericUpDownOrigSizeX.Value = textures[listBox1.SelectedIndex].Image.Width;
					numericUpDownOrigSizeY.Value = textures[listBox1.SelectedIndex].Image.Height;
					return;
				default:
					labelPaletteFormat.Hide();
					palettePreview.Hide();
					buttonLoadPalette.Hide();
					buttonSavePalette.Hide();
					buttonResetPalette.Hide();
					panelPaletteInfo.Hide();
					return;
			}
		}



		private void buttonResetPalette_Click(object sender, EventArgs e)
		{
			currentPalette = TexturePalette.CreateDefaultPalette(true);
			paletteSet = comboBoxCurrentPaletteBank.SelectedIndex = 0;
			UpdateTextureInformation();
		}


		private void comboBoxCurrentPaletteBank_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (currentPalette != null && paletteSet != comboBoxCurrentPaletteBank.SelectedIndex)
			{
				paletteSet = comboBoxCurrentPaletteBank.SelectedIndex;
				UpdateTextureInformation();
			}
		}

		private void numericUpDownStartColor_ValueChanged(object sender, EventArgs e)
		{
			if (currentPalette != null)
				currentPalette.StartColor = (short)Math.Min(numericUpDownStartColor.Value, currentPalette.GetNumColors() - numericUpDownStartColor.Value);
		}

		private void LoadPaletteDialog()
		{
			using (OpenFileDialog fd = new OpenFileDialog() { Filter = "Texture Palette|*.pvp;*.gvp;*.bmp;*.png", DefaultExt = "pvp", FilterIndex = currentFormat == TextureFormat.PVM ? 1 : 2 })
			{
				DialogResult dr = fd.ShowDialog(this);
				if (dr == DialogResult.OK)
				{
					switch (Path.GetExtension(fd.FileName).ToLowerInvariant())
					{
						case ".png":
						case ".bmp":
							Bitmap bp = new Bitmap(fd.FileName);
							currentPalette = new TexturePalette(new Bitmap(bp));
							bp.Dispose();
							break;
						case ".pvp":
						case ".gvp":
						default:
							currentPalette = new TexturePalette(File.ReadAllBytes(fd.FileName), compatibleGVPToolStripMenuItem.Checked);
							break;
					}
					paletteSet = comboBoxCurrentPaletteBank.SelectedIndex = 0;
					UpdateTextureInformation();
				}
			}
		}

		private void SavePaletteDialog(string defaultFilename)
		{
			using (SaveFileDialog fd = new SaveFileDialog() { Title = "Save Palette File", FileName = defaultFilename, Filter = "PVR Palette|*.pvp|GVR Palette|*.gvp|Bitmaps|*.bmp;*.png", DefaultExt = "pvp", FilterIndex = currentFormat == TextureFormat.PVM ? 1 : 2 })
			{
				DialogResult dr = fd.ShowDialog(this);
				if (dr == DialogResult.OK)
				{
					switch (Path.GetExtension(fd.FileName).ToLowerInvariant())
					{
						case ".pvp":
						case ".gvp":
							File.WriteAllBytes(fd.FileName, currentPalette.GetBytes(Path.GetExtension(fd.FileName).ToLowerInvariant() == ".gvp"));
							break;
						case ".png":
							currentPalette.SavePNG(fd.FileName);
							break;
					}
				}
			}
		}

		private Bitmap ProcessIndexedBitmap(GenericTexture tex, PalettedTextureFormat indxfmt)
		{
			// Prepare the mask
			Bitmap unpalettedBitmap;
			if (tex is PvrTexture pvri)
			{
				PvrTexture pvrt = new PvrTexture(pvri.RawData);
				pvrt.SetPalette(TexturePalette.CreateDefaultPalette(true));
				unpalettedBitmap = pvrt.Image;
			}
			else if (tex is GvrTexture gvri)
			{
				GvrTexture gvrt = new GvrTexture(gvri.RawData);
				gvrt.SetPalette(TexturePalette.CreateDefaultPalette(true));
				unpalettedBitmap = gvrt.Image;
			}
			else return tex.Image;
			TexturePalette applyPalette;
			int applySet;

			// Set the default palette if exporting mask, otherwise use current palette and bank
			if (exportMaskToolStripMenuItem.Checked)
			{
				applyPalette = defaultPalette;
				applySet = 0;
			}
			else
			{
				applyPalette = currentPalette;
				applySet = paletteSet;
			}

			// Palettize
			Bitmap result;
			int setsize = 16;
			switch (indxfmt)
			{
				case PalettedTextureFormat.Index4:
					result = new Bitmap(unpalettedBitmap.Width, unpalettedBitmap.Height, PixelFormat.Format4bppIndexed);
					break;
				case PalettedTextureFormat.Index8:
					setsize = 256;
					result = new Bitmap(unpalettedBitmap.Width, unpalettedBitmap.Height, PixelFormat.Format8bppIndexed);
					break;
				default:
					return unpalettedBitmap;
			}
			Color[] defaultpaletteSet = defaultPalette.ToColorArray();
			List<Color> usedPaletteSet = new List<Color>();
			Color[] colorArray = applyPalette.ToColorArray();
			for (int i = 0; i < setsize; i++)
				usedPaletteSet.Add(colorArray[applySet * setsize + i]);

			// Replace the palette in the Bitmap
			var newAliasForPalette = result.Palette;
			for (int i = 0; i < usedPaletteSet.Count; i++)
			{
				newAliasForPalette.Entries[i] = usedPaletteSet[i];
			}
			result.Palette = newAliasForPalette;

			// Set pixels in the indexed image
			int remaining = unpalettedBitmap.Height * unpalettedBitmap.Width;
			using (var snoop_u = new BmpPixelSnoop(new Bitmap(unpalettedBitmap)))
			{
				for (int y = 0; y < unpalettedBitmap.Height; y++)
					for (int x = 0; x < unpalettedBitmap.Width; x++)
					{
						for (int p = 0; p < setsize; p++)
						{
							if (snoop_u.GetPixel(x, y) == defaultpaletteSet[p])
							{
								remaining--;
								TextureFunctions.SetPixelIndex(result, x, y, p);
								break;
							}
						}
					}
			}
			if (remaining > 0)
				System.Windows.Forms.MessageBox.Show(remaining.ToString() + " pixels were not found in the palette.",
					"Texture Editor Export Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
			if (exportPalettedFullToolStripMenuItem.Checked)
				return new Bitmap(result);
			else
				return result;
		}

		private void RetrievePaletteFromIndexedBitmap(Bitmap bitmap, PalettedTextureFormat texfmt, PixelCodec codec, bool gvp = false)
		{
			if (bitmap.PixelFormat != PixelFormat.Format4bppIndexed && bitmap.PixelFormat != PixelFormat.Format8bppIndexed)
				return;
			currentPalette.Clear();

			for (int c = 0; c < bitmap.Palette.Entries.Length; c++)
				currentPalette.AddColor(bitmap.Palette.Entries[c]);

			// Add more colors if an Index4 image was imported as Index8
			int rowsize = texfmt == PalettedTextureFormat.Index8 ? 256 : 16;
			if (currentPalette.GetNumColors() < rowsize)
				do
					currentPalette.AddColor(Color.Black);
				while (currentPalette.GetNumColors() < rowsize);

			currentPalette.Encode(codec);

			// Refresh bank preview
			int rowsize_stored = rowsize - currentPalette.StartColor;
			int numrows = currentPalette.GetNumColors() / rowsize_stored;
			comboBoxCurrentPaletteBank.Items.Clear();
			for (int i = 0; i < numrows; i++)
				comboBoxCurrentPaletteBank.Items.Add((currentPalette.StartBank + i).ToString());
			comboBoxCurrentPaletteBank.SelectedIndex = paletteSet;
			paletteSet = comboBoxCurrentPaletteBank.SelectedIndex = 0;
		}

		private Bitmap RetrieveMaskFromIndexedBitmap(Bitmap bitmap, PalettedTextureFormat palettedTextureFormat)
		{
			PixelFormat pxfmt = PixelFormat.Format4bppIndexed;
			int stepsize = 16;
			if (palettedTextureFormat == PalettedTextureFormat.Index8)
			{
				stepsize = 256;
				pxfmt = PixelFormat.Format8bppIndexed;
			}
			Bitmap result = new Bitmap(bitmap.Width, bitmap.Height, pxfmt);
			using (var snoop_b = new BmpPixelSnoop(new Bitmap(bitmap)))
			{
				for (int y = 0; y < bitmap.Height; y++)
					for (int x = 0; x < bitmap.Width; x++)
					{
						for (int p = 0; p < stepsize; p++)
						{
							if (snoop_b.GetPixel(x, y) == currentPalette.GetColorAnyBank(p))
							{
								TextureFunctions.SetPixelIndex(result, x, y, p);
								break;
							}
						}
					}
			}
			// Replace the palette in the Bitmap
			var newAliasForPalette = result.Palette;
			for (int i = 0; i < currentPalette.GetNumColors(); i++)
			{
				newAliasForPalette.Entries[i] = defaultPalette.GetColorAnyBank(i);
			}
			result.Palette = newAliasForPalette;
			return result;
		}
		#endregion

		#region Mipmap preview
		private void trackBarMipmapLevel_Scroll(object sender, EventArgs e)
		{
			GenericTexture currentTexture = textures[listBox1.SelectedIndex];
			if (!currentTexture.HasMipmaps)
				return;
			UpdateTextureMipmap();
			UpdateTextureView();
		}

		private void UpdateTextureMipmap()
		{
			if (listBox1.SelectedIndex != -1)
			{
				GenericTexture currentTexture = textures[listBox1.SelectedIndex];
				if (currentTexture.HasMipmaps)
				{
					trackBarMipmapLevel.Maximum = currentTexture.MipmapImages.Length - 1;
					trackBarMipmapLevel.Value = Math.Min(trackBarMipmapLevel.Value, currentTexture.MipmapImages.Length - 1);
					trackBarMipmapLevel.Enabled = true;
					int mipmapW = currentTexture.Image.Width / (int)Math.Pow(2, trackBarMipmapLevel.Value);
					int mipmapH = currentTexture.Image.Width / (int)Math.Pow(2, trackBarMipmapLevel.Value);
					labelMipmapLevel.Text = $"Mipmap Level: {trackBarMipmapLevel.Value} ({mipmapW}x{mipmapH})";
					return;
				}
			}
			// Texture not selected or no mipmaps
			trackBarMipmapLevel.Value = 0;
			trackBarMipmapLevel.Enabled = false;
			labelMipmapLevel.Text = "Mipmap Level: N/A";
		}
		#endregion
	}
}