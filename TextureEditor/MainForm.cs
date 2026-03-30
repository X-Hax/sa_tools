using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TextureLib;
using SAModel.SAEditorCommon;
using static TextureEditor.Program;

namespace TextureEditor
{
	// This .cs file only has UI event handler functions, for actual texture related code see the .cs files in the MainForm folder.
	// Global variables, such as list of textures, settings file etc. are in Variables.cs
	public partial class MainForm : Form
	{
		/// <summary>List of recently accessed files.</summary>
		private Properties.Settings Settings = Properties.Settings.Default;

		#region Startup, shutdown and error handling
		public MainForm()
		{
			Application.ThreadException += Application_ThreadException;
			InitializeComponent();
		}

		private void MainForm_Shown(object sender, EventArgs e)
		{
			settingsfile = SAModel.SAEditorCommon.SettingsFile.Settings_TextureEditor.Load();
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

			// View menu
			textureFilteringToolStripMenuItem.Checked = settingsfile.EnableFiltering;
			// Edit menu - GVR settings
			highQualityGVMsToolStripMenuItem.Checked = settingsfile.HighQualityGVM;
			compatibleGVPToolStripMenuItem.Checked = settingsfile.SACompatiblePalettes;
			// Edit menu - Texture conversion settings
			allowCompressedFormatsToolStripMenuItem.Checked = settingsfile.TexEncodeUseCompressed;
			preferHighQualityToolStripMenuItem.Checked = settingsfile.TexEncodeAutoHighQuality;
			// Edit menu - DDS/PNG settings
			useDDSInPAKsToolStripMenuItem.Checked = settingsfile.UseDDSforPAK;
			useDDSInPVMXToolStripMenuItem.Checked = settingsfile.UseDDSforPVMX;
			useDDSInTexturePacksToolStripMenuItem.Checked = settingsfile.UseDDSforTexPack;
			// Chao Settings menu - Alignment
			neutralChaoToolStripMenuItem.Checked = settingsfile.ChaoAlignment == ChaoPalettes.ChaoAlignment.Neutral;
			heroChaoToolStripMenuItem.Checked = settingsfile.ChaoAlignment == ChaoPalettes.ChaoAlignment.Hero;
			darkChaoToolStripMenuItem.Checked = settingsfile.ChaoAlignment == ChaoPalettes.ChaoAlignment.Dark;
			// Chao Settings menu - First evolution
			childFirstToolStripMenuItem.Checked = settingsfile.ChaoFirstEvolution == ChaoPalettes.ChaoEvolution.Zero;
			normalFirstToolStripMenuItem.Checked = settingsfile.ChaoFirstEvolution == ChaoPalettes.ChaoEvolution.Normal;
			swimFirstToolStripMenuItem.Checked = settingsfile.ChaoFirstEvolution == ChaoPalettes.ChaoEvolution.Swim;
			flyFirstToolStripMenuItem.Checked = settingsfile.ChaoFirstEvolution == ChaoPalettes.ChaoEvolution.Fly;
			runFirstToolStripMenuItem.Checked = settingsfile.ChaoFirstEvolution == ChaoPalettes.ChaoEvolution.Run;
			powerFirstToolStripMenuItem.Checked = settingsfile.ChaoFirstEvolution == ChaoPalettes.ChaoEvolution.Power;
			// Chao Settings menu - Second evolution
			zeroSecondToolStripMenuItem.Checked = settingsfile.ChaoSecondEvolution == ChaoPalettes.ChaoEvolution.Zero;
			normalSecondToolStripMenuItem.Checked = settingsfile.ChaoSecondEvolution == ChaoPalettes.ChaoEvolution.Normal;
			swimSecondToolStripMenuItem.Checked = settingsfile.ChaoSecondEvolution == ChaoPalettes.ChaoEvolution.Swim;
			flySecondToolStripMenuItem.Checked = settingsfile.ChaoSecondEvolution == ChaoPalettes.ChaoEvolution.Fly;
			runSecondToolStripMenuItem.Checked = settingsfile.ChaoSecondEvolution == ChaoPalettes.ChaoEvolution.Run;
			powerSecondToolStripMenuItem.Checked = settingsfile.ChaoSecondEvolution == ChaoPalettes.ChaoEvolution.Power;
			if (Program.Arguments.Length > 0 && !LoadFile(Program.Arguments[0]))
				Close();
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

		void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			string log = "Texture Editor: New log entry on " + DateTime.Now.ToString("G") + System.Environment.NewLine +
				"Build date:" + File.GetLastWriteTime(Application.ExecutablePath).ToString(System.Globalization.CultureInfo.InvariantCulture) +
				System.Environment.NewLine + e.Exception.ToString();
			string errDesc = "Texture Editor has crashed with the following error:\n" + e.Exception.GetType().Name + ".\n\n" +
				"If you wish to report a bug, please include the following in your report:";
			ErrorDialog report = new ErrorDialog("Texture Editor", errDesc, log);
			DialogResult dgresult = report.ShowDialog(this);
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
		#endregion

		#region Drag and Drop
		// Drag and drop - adding files
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

				LoadFile(files[0]);
				listBox1_SelectedIndexChanged(sender, e);
				unsaved = false;
			}
		}

		// Drag and drop - exporting image
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
		#endregion

		#region Texture list and Add/Remove/Up/Down buttons

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (currentTextureID != listBox1.SelectedIndex)
			{
				currentTextureID = listBox1.SelectedIndex;
				UpdateTextureInformation();
			}
		}

		private void SplitContainer1_Panel2_SizeChanged(object sender, EventArgs e)
		{
			// Maybe not necessary?
			if (listBox1.SelectedIndex > -1)
				UpdateTextureView();
		}

		private void addTextureButton_Click(object sender, EventArgs e)
		{
			AddTextures();
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
		#endregion

		#region Palette settings and preview
		private void buttonLoadPalette_Click(object sender, EventArgs e)
		{
			LoadPaletteDialog(textures[listBox1.SelectedIndex].Name);
		}

		private void buttonSavePalette_Click(object sender, EventArgs e)
		{
			SavePaletteDialog(textures[listBox1.SelectedIndex].Name);
		}

		private void numericUpDownStartBank_ValueChanged(object sender, EventArgs e)
		{
			currentPalette.StartBank = (short)numericUpDownStartBank.Value;
		}

		private void buttonResetPalette_Click(object sender, EventArgs e)
		{
			ResetPalette();
		}

		private void comboBoxCurrentPaletteBank_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetPaletteBank();
		}

		private void numericUpDownStartColor_ValueChanged(object sender, EventArgs e)
		{
			SetPaletteStartColor();
		}

		private void palettePreview_Click(object sender, EventArgs e)
		{
			MouseEventArgs me = (MouseEventArgs)e;
			DisplayPaletteSelectedColorInfo(me.Location);
		}
		#endregion

		#region File menu
		private void importTexturePackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ImportTexturePack();
		}

		private void NewFile(TextureArchiveFormat newfilefmt)
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
			NewFile(TextureArchiveFormat.PVM);
			listBox1_SelectedIndexChanged(sender, e);
		}

		private void newGVMToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NewFile(TextureArchiveFormat.GVM);
			listBox1_SelectedIndexChanged(sender, e);
		}

		private void newXVMToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NewFile(TextureArchiveFormat.XVM);
			listBox1_SelectedIndexChanged(sender, e);
		}

		private void newPVMXToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NewFile(TextureArchiveFormat.PVMX);
			listBox1_SelectedIndexChanged(sender, e);
		}

		private void newPAKToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NewFile(TextureArchiveFormat.PAK);
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
					LoadFile(dlg.FileName);
					listBox1_SelectedIndexChanged(sender, e);
					unsaved = false;
				}
		}

		private void recentFilesToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			LoadFile(Settings.MRUList[recentFilesToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem)]);
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
			SaveAs(TextureArchiveFormat.PVM);
		}

		private void saveAsGVMToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveAs(TextureArchiveFormat.GVM);
		}

		private void saveAsPVMXToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveAs(TextureArchiveFormat.PVMX);
		}

		private void saveAsPAKToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveAs(TextureArchiveFormat.PAK);
		}

		private void saveXVMToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveAs(TextureArchiveFormat.XVM);
		}

		private void exportTexturePackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ExportTexturePack();
		}

		private void exportAllPVRToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ExportAllAs(TextureFileFormat.Pvr);
		}

		private void exportAllGVRToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ExportAllAs(TextureFileFormat.Gvr);
		}

		private void exportAllXVRToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ExportAllAs(TextureFileFormat.Xvr);
		}

		private void exportAllDDSToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ExportAllAs(TextureFileFormat.Dds);
		}

		private void exportAllPNGToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ExportAllAs(TextureFileFormat.Png);
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}
		#endregion

		#region View menu
		private void textureFilteringToolStripMenuItem_Click(object sender, EventArgs e)
		{
			settingsfile.EnableFiltering = textureFilteringToolStripMenuItem.Checked;
			UpdateTextureView();
		}
		#endregion

		#region Edit menu
		private void highQualityGVMsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			settingsfile.HighQualityGVM = highQualityGVMsToolStripMenuItem.Checked;
		}

		private void compatibleGVPToolStripMenuItem_Click(object sender, EventArgs e)
		{
			settingsfile.SACompatiblePalettes = compatibleGVPToolStripMenuItem.Checked;
			if (listBox1.SelectedIndex != -1)
				if (textures[listBox1.SelectedIndex] is GvrTexture && textures[listBox1.SelectedIndex].Indexed)
				{
					UpdateTextureInformation();
				}
		}

		private void preferHighQualityToolStripMenuItem_Click(object sender, EventArgs e)
		{
			settingsfile.TexEncodeAutoHighQuality = preferHighQualityToolStripMenuItem.Checked;
		}

		private void allowCompressedFormatsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			settingsfile.TexEncodeUseCompressed = allowCompressedFormatsToolStripMenuItem.Checked;
		}

		private void useDDSInPAKsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			settingsfile.UseDDSforPAK = useDDSInPAKsToolStripMenuItem.Checked;
		}

		private void useDDSInPVMXToolStripMenuItem_Click(object sender, EventArgs e)
		{
			settingsfile.UseDDSforPVMX = useDDSInPVMXToolStripMenuItem.Checked;
		}

		private void useDDSInTexturePacksToolStripMenuItem_Click(object sender, EventArgs e)
		{
			settingsfile.UseDDSforTexPack = useDDSInTexturePacksToolStripMenuItem.Checked;
		}
		#endregion

		#region Texture Panel - texture and mipmap preview
		private void texturePreviewZoomTrackBar_Scroll(object sender, EventArgs e)
		{
			UpdateTextureView();
		}

		private void trackBarMipmapLevel_Scroll(object sender, EventArgs e)
		{
			if (listBox1.SelectedIndex == -1)
				return;
			GenericTexture currentTexture = textures[listBox1.SelectedIndex];
			if (!currentTexture.HasMipmaps)
				return;
			UpdateTextureMipmap();
			UpdateTextureView();
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
			TexturePreviewCopy();
		}

		private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TexturePreviewPaste();
		}
		#endregion

		#region Texture Panel - Import/Export/Replace/Save buttons
		private void importButton_Click(object sender, EventArgs e)
		{
			if (ImportTexture())
			{
				UpdateTextureInformation();
				listBox1.Select();
			}
		}

		private void buttonReplaceImage_Click(object sender, EventArgs e)
		{
			ReplaceImage();
		}

		private void saveTextureButton_Click(object sender, EventArgs e)
		{
			SaveSingleTexture();
		}

		private void exportButton_Click(object sender, EventArgs e)
		{
			ExportImage();
		}

		#endregion

		#region Texture Panel - Editing texture information
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
				{
					ti.AddMipmaps();
					ti.Decode(); // To update mipmap images
				}
			}
			// Update surface flags for PAK textures
			if (currentFormat == TextureArchiveFormat.PAK && !usingSocPak)
			{
				if (!mipmapCheckBox.Checked)
					ti.PakMetadata.PakNinjaFlags &= ~NinjaSurfaceFlags.Mipmapped;
				else
					ti.PakMetadata.PakNinjaFlags |= NinjaSurfaceFlags.Mipmapped;
			}
			UpdateTextureInformation();
			unsaved = true;
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
			// checkBoxPAKUseAlpha_CheckedChanged(sender, e); // This is called automatically so no need to call it here
			unsaved = true;
		}
		#endregion

		#region Tools menu
		private void generateNewGbixToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (textures.Count < 1)
				return;

			DialogResult res = MessageBox.Show(this, "This will generate a random new GBIX for every texture, are you sure you wish to continue?", "Texture Editor Warning: Global Index replacement", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

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

		private void enablePAKAlphaForAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PAKEnableAlphaForAll(true);
		}

		private void disablePAKAlphaForAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PAKEnableAlphaForAll(false);
		}

		private void addMipmapsToAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool error = false;
			foreach (GenericTexture info in textures)
				if (info.CanHaveMipmaps())
				{
					info.AddMipmaps();
					info.Decode(); // To update mipmap images
					unsaved = true;
				}
				else error = true;
			if (listBox1.SelectedIndex != -1 && textures[listBox1.SelectedIndex].HasMipmaps)
				mipmapCheckBox.Checked = true;
			if (error)
				MessageBox.Show(this, "Rectangular PVR and GVR textures do not allow mipmaps. These have been skipped.", "Texture Editor Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void removeMipmapsFromAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (GenericTexture info in textures)
				info.RemoveMipmaps();
			if (listBox1.SelectedIndex != -1 && textures[listBox1.SelectedIndex].HasMipmaps)
				mipmapCheckBox.Checked = true;
			unsaved = true;
		}

		private void exportMipmapsAsPNGToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ExportMipmaps();
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
		#endregion

		#region Help menu
		private void GoToSite(string url)
		{
			System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd", $"/c start " + url) { CreateNoWindow = false });
		}

		private void textureEditorHelpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GoToSite("https://sadocs.unreliable.network/wiki/Texture_Editor");
		}

		private void textureEditingGuideToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GoToSite("https://sadocs.unreliable.network/wiki/Creating_Mods/Texture_Replacement");
		}

		private void listOfTexturesSA1SADXToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GoToSite("https://sadocs.unreliable.network/wiki/Sonic_Adventure/Texture_Files");
		}

		private void listOfTexturesSA2SA2BToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GoToSite("https://sadocs.unreliable.network/wiki/Sonic_Adventure_2/Texture_Files");
		}
		#endregion

		#region Chao Settings menu
		private void neutralChaoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			settingsfile.ChaoAlignment = ChaoPalettes.ChaoAlignment.Neutral;
			neutralChaoToolStripMenuItem.Checked = true;
			heroChaoToolStripMenuItem.Checked = false;
			darkChaoToolStripMenuItem.Checked = false;
		}

		private void heroChaoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			settingsfile.ChaoAlignment = ChaoPalettes.ChaoAlignment.Hero;
			neutralChaoToolStripMenuItem.Checked = false;
			heroChaoToolStripMenuItem.Checked = true;
			darkChaoToolStripMenuItem.Checked = false;
		}

		private void darkChaoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			settingsfile.ChaoAlignment = ChaoPalettes.ChaoAlignment.Dark;
			neutralChaoToolStripMenuItem.Checked = false;
			heroChaoToolStripMenuItem.Checked = false;
			darkChaoToolStripMenuItem.Checked = true;
		}

		private void childFirstToolStripMenuItem_Click(object sender, EventArgs e)
		{
			settingsfile.ChaoFirstEvolution = ChaoPalettes.ChaoEvolution.Zero;
			childFirstToolStripMenuItem.Checked = true;
			normalFirstToolStripMenuItem.Checked = false;
			swimFirstToolStripMenuItem.Checked = false;
			flyFirstToolStripMenuItem.Checked = false;
			runFirstToolStripMenuItem.Checked = false;
			powerFirstToolStripMenuItem.Checked = false;
		}

		private void normalFirstToolStripMenuItem_Click(object sender, EventArgs e)
		{
			settingsfile.ChaoFirstEvolution = ChaoPalettes.ChaoEvolution.Normal;
			childFirstToolStripMenuItem.Checked = false;
			normalFirstToolStripMenuItem.Checked = true;
			swimFirstToolStripMenuItem.Checked = false;
			flyFirstToolStripMenuItem.Checked = false;
			runFirstToolStripMenuItem.Checked = false;
			powerFirstToolStripMenuItem.Checked = false;
		}

		private void swimFirstToolStripMenuItem_Click(object sender, EventArgs e)
		{
			settingsfile.ChaoFirstEvolution = ChaoPalettes.ChaoEvolution.Swim;
			childFirstToolStripMenuItem.Checked = false;
			normalFirstToolStripMenuItem.Checked = false;
			swimFirstToolStripMenuItem.Checked = true;
			flyFirstToolStripMenuItem.Checked = false;
			runFirstToolStripMenuItem.Checked = false;
			powerFirstToolStripMenuItem.Checked = false;
		}

		private void flyFirstToolStripMenuItem_Click(object sender, EventArgs e)
		{
			settingsfile.ChaoFirstEvolution = ChaoPalettes.ChaoEvolution.Fly;
			childFirstToolStripMenuItem.Checked = false;
			normalFirstToolStripMenuItem.Checked = false;
			swimFirstToolStripMenuItem.Checked = false;
			flyFirstToolStripMenuItem.Checked = true;
			runFirstToolStripMenuItem.Checked = false;
			powerFirstToolStripMenuItem.Checked = false;
		}

		private void runFirstToolStripMenuItem_Click(object sender, EventArgs e)
		{
			settingsfile.ChaoFirstEvolution = ChaoPalettes.ChaoEvolution.Run;
			childFirstToolStripMenuItem.Checked = false;
			normalFirstToolStripMenuItem.Checked = false;
			swimFirstToolStripMenuItem.Checked = false;
			flyFirstToolStripMenuItem.Checked = false;
			runFirstToolStripMenuItem.Checked = true;
			powerFirstToolStripMenuItem.Checked = false;
		}

		private void powerFirstToolStripMenuItem_Click(object sender, EventArgs e)
		{
			settingsfile.ChaoFirstEvolution = ChaoPalettes.ChaoEvolution.Power;
			childFirstToolStripMenuItem.Checked = false;
			normalFirstToolStripMenuItem.Checked = false;
			swimFirstToolStripMenuItem.Checked = false;
			flyFirstToolStripMenuItem.Checked = false;
			runFirstToolStripMenuItem.Checked = false;
			powerFirstToolStripMenuItem.Checked = true;
		}

		private void zeroSecondToolStripMenuItem_Click(object sender, EventArgs e)
		{
			settingsfile.ChaoSecondEvolution = ChaoPalettes.ChaoEvolution.Zero;
			zeroSecondToolStripMenuItem.Checked = true;
			normalSecondToolStripMenuItem.Checked = false;
			swimSecondToolStripMenuItem.Checked = false;
			flySecondToolStripMenuItem.Checked = false;
			runSecondToolStripMenuItem.Checked = false;
			powerSecondToolStripMenuItem.Checked = false;
		}

		private void normalSecondToolStripMenuItem_Click(object sender, EventArgs e)
		{
			settingsfile.ChaoSecondEvolution = ChaoPalettes.ChaoEvolution.Normal;
			zeroSecondToolStripMenuItem.Checked = false;
			normalSecondToolStripMenuItem.Checked = true;
			swimSecondToolStripMenuItem.Checked = false;
			flySecondToolStripMenuItem.Checked = false;
			runSecondToolStripMenuItem.Checked = false;
			powerSecondToolStripMenuItem.Checked = false;
		}

		private void swimSecondToolStripMenuItem_Click(object sender, EventArgs e)
		{
			settingsfile.ChaoSecondEvolution = ChaoPalettes.ChaoEvolution.Swim;
			zeroSecondToolStripMenuItem.Checked = false;
			normalSecondToolStripMenuItem.Checked = false;
			swimSecondToolStripMenuItem.Checked = true;
			flySecondToolStripMenuItem.Checked = false;
			runSecondToolStripMenuItem.Checked = false;
			powerSecondToolStripMenuItem.Checked = false;
		}

		private void flySecondToolStripMenuItem_Click(object sender, EventArgs e)
		{
			settingsfile.ChaoSecondEvolution = ChaoPalettes.ChaoEvolution.Fly;
			zeroSecondToolStripMenuItem.Checked = false;
			normalSecondToolStripMenuItem.Checked = false;
			swimSecondToolStripMenuItem.Checked = false;
			flySecondToolStripMenuItem.Checked = true;
			runSecondToolStripMenuItem.Checked = false;
			powerSecondToolStripMenuItem.Checked = false;
		}

		private void runSecondToolStripMenuItem_Click(object sender, EventArgs e)
		{
			settingsfile.ChaoSecondEvolution = ChaoPalettes.ChaoEvolution.Run;
			zeroSecondToolStripMenuItem.Checked = false;
			normalSecondToolStripMenuItem.Checked = false;
			swimSecondToolStripMenuItem.Checked = false;
			flySecondToolStripMenuItem.Checked = false;
			runSecondToolStripMenuItem.Checked = true;
			powerSecondToolStripMenuItem.Checked = false;
		}

		private void powerSecondToolStripMenuItem_Click(object sender, EventArgs e)
		{
			settingsfile.ChaoSecondEvolution = ChaoPalettes.ChaoEvolution.Power;
			zeroSecondToolStripMenuItem.Checked = false;
			normalSecondToolStripMenuItem.Checked = false;
			swimSecondToolStripMenuItem.Checked = false;
			flySecondToolStripMenuItem.Checked = false;
			runSecondToolStripMenuItem.Checked = false;
			powerSecondToolStripMenuItem.Checked = true;
		}
		#endregion
	}
}