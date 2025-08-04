using FraGag.Compression;
using SAModel;
using System.Data;
using System.Text;
using static SAModel.SAEditorCommon.SettingsFile;

namespace SA2CutsceneEffectEditor
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}
		enum CutsceneEffectFileTypes
		{
			Normal,
			LanguageOnly,
			MiniEvent,
			TheTrial
		}
		List<Size> formSizes = new List<Size>();
		List<Size> formMESizes = new List<Size>();
		public Size currentFormSize { get { return formSizes[effectDataSetComboBox.SelectedIndex]; } }
		public Size currentMEFormSize { get { return formMESizes[miniEventDataSetComboBox.SelectedIndex]; } }
		List<string> recentFiles = new List<string>();
		string filename = null;
		string currentFormatDialog = "";
		bool bigEndian;
		string bigEndianDialog = "";
		// Effect Data sets
		List<SubtitleData> subs = new List<SubtitleData>();
		List<AudioMiscData> audios = new List<AudioMiscData>();
		List<ScreenData> screens = new List<ScreenData>();
		List<SingleParticleData> singleParticles = new List<SingleParticleData>();
		List<LightData> lights = new List<LightData>();
		List<BlurData> blurs = new List<BlurData>();
		List<ParticleGeneratorData> particleGens = new List<ParticleGeneratorData>();
		List<VideoData> videos = new List<VideoData>();
		// Mini-Event Effect Data
		List<MiniEventEffectData> minieffs = new List<MiniEventEffectData>();
		SubtitleData CurrentSubData { get { return subs[(int)subInstanceNumericUpDown.Value]; } }
		AudioMiscData CurrentAudioData { get { return audios[(int)audioInstanceNumericUpDown.Value]; } }
		ScreenData CurrentScreenData { get { return screens[(int)screenInstanceNumericUpDown.Value]; } }
		SingleParticleData CurrentSingleParticleData { get { return singleParticles[(int)particleInstanceNumericUpDown.Value]; } }
		LightData CurrentLightData { get { return lights[(int)lightInstanceNumericUpDown.Value + (256 * ((int)lightSetNumericUpDown.Value - 1))]; } }
		BlurData CurrentBlurData { get { return blurs[(int)blurInstancesNumericUpDown.Value]; } }
		ParticleGeneratorData CurrentParticleGenData { get { return particleGens[(int)particleGenInstanceNumericUpDown.Value]; } }
		VideoData CurrentVideoData { get { return videos[(int)videoInstanceNumericUpDown.Value]; } }
		MiniEventEffectData CurrentMiniEventEffectData { get { return minieffs[(int)miniEventEffectInstanceNumericUpDown.Value]; } }
		MiniEventEffectFloats MiniFloats { get; set; }
		Settings_SA2CutsceneEffectEditor settingsFile;
		CutsceneEffectFileTypes currentFormat;

		private void MainForm_Load(object sender, EventArgs e)
		{
			settingsFile = Settings_SA2CutsceneEffectEditor.Load();
			formSizes.Add(new Size(groupBoxSubtitle.Width + 60, groupBoxSubtitle.Height + 200));
			formSizes.Add(new Size(groupBoxAudioMisc.Width + 60, groupBoxAudioMisc.Height + 200));
			formSizes.Add(new Size(groupBoxScreenFX.Width + 60, groupBoxScreenFX.Height + 200));
			formSizes.Add(new Size(groupBoxSingleParticle.Width + 60, groupBoxSingleParticle.Height + 200));
			formSizes.Add(new Size(groupBoxLighting.Width + 60, groupBoxLighting.Height + 200));
			formSizes.Add(new Size(groupBoxBlur.Width + 60, groupBoxBlur.Height + 200));
			formSizes.Add(new Size(groupBoxParticleGen.Width + 60, groupBoxParticleGen.Height + 200));
			formSizes.Add(new Size(groupBoxVideo.Width + 60, groupBoxVideo.Height + 200));
			formMESizes.Add(new Size(groupBoxSubtitle.Width + 60, groupBoxSubtitle.Height + 200));
			formMESizes.Add(new Size(groupBoxMiniEventEffects.Width + 60, groupBoxMiniEventEffects.Height + 200));
			formMESizes.Add(new Size(groupBoxMiniCamera.Width + 60, groupBoxMiniCamera.Height + 200));
			Size = formSizes[0];
			settingsFile = Settings_SA2CutsceneEffectEditor.Load();
			if (settingsFile.RecentFiles != null)
				recentFiles = new List<string>(settingsFile.RecentFiles.Where(a => File.Exists(a)));
			if (recentFiles.Count > 0)
				UpdateRecentFiles();
			bigEndian = settingsFile.BigEndian;
			// This is for the auto-size feature. The editor will display everything all at once.
			groupBoxAudioMisc.Location = groupBoxSubtitle.Location;
			groupBoxScreenFX.Location = groupBoxSubtitle.Location;
			groupBoxSingleParticle.Location = groupBoxSubtitle.Location;
			groupBoxLighting.Location = groupBoxSubtitle.Location;
			groupBoxBlur.Location = groupBoxSubtitle.Location;
			groupBoxParticleGen.Location = groupBoxSubtitle.Location;
			groupBoxVideo.Location = groupBoxSubtitle.Location;
			groupBoxMiniEventEffects.Location = groupBoxSubtitle.Location;
			groupBoxMiniCamera.Location = groupBoxSubtitle.Location;
			miniEventDataSetComboBox.Location = effectDataSetComboBox.Location;
			miniEventDataSetComboBox.Enabled = false;
			miniEventDataSetComboBox.Visible = false;
			switch (settingsFile.Format)
			{
				case 0:
				default:
					currentFormat = CutsceneEffectFileTypes.Normal;
					currentFormatDialog = "normal format";
					break;
				case 1:
					currentFormat = CutsceneEffectFileTypes.LanguageOnly;
					currentFormatDialog = "subtitle/audio only format";
					break;
				case 2:
					currentFormat = CutsceneEffectFileTypes.MiniEvent;
					currentFormatDialog = "mini-event format";
					break;
				case 3:
					currentFormat = CutsceneEffectFileTypes.TheTrial;
					currentFormatDialog = "beta/The Trial format";
					break;
			}
			InitializeEffectData();
			if (filename != null)
				LoadFile(filename);
		}
		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog()
			{
				DefaultExt = "prs",
				Filter = "All Cutscene Effect Files|e????_?.prs;e????_?.scr;me????_?.scr|" +
				"Main Cutscene Effect Files|e????_?.prs;e????_?.scr|" +
				"Mini-Event Effect Files|me????_?.scr|" +
				"All Files|*.*"
			})
				if (dlg.ShowDialog(this) == DialogResult.OK)
					LoadFile(dlg.FileName);
		}
		private void LoadFile(string filename)
		{
			this.filename = filename;
			byte[] fc;
			if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				fc = Prs.Decompress(filename);
			else
				fc = File.ReadAllBytes(filename);
			if (fc.Length <= 0x141C)
			{
				effectDataSetComboBox.Enabled = false;
				effectDataSetComboBox.Visible = false;
				//effectDataSetComboBox.SelectedIndex = 0;
				miniEventDataSetComboBox.Enabled = true;
				miniEventDataSetComboBox.Visible = true;
				if (fc[4] > 0 || fc[8] > 0 || fc[0x100] > 0)
				{
					bigEndian = false;
					forceBigEndianToolStripMenuItem.Checked = false;
				}
				else
				{
					bigEndian = true;
					forceBigEndianToolStripMenuItem.Checked = true;
				}
				miniEventToolStripMenuItem.Checked = true;
				normalToolStripMenuItem.Checked = false;
				normalToolStripMenuItem.Enabled = false;
				subtitleAudioOnlyToolStripMenuItem.Checked = false;
				subtitleAudioOnlyToolStripMenuItem.Enabled = false;
				dCBetaTheTrialToolStripMenuItem.Checked = false;
				dCBetaTheTrialToolStripMenuItem.Enabled = false;
				effectDataSetComboBox.Enabled = false;
				effectDataSetComboBox.Visible = false;
				miniEventDataSetComboBox.Enabled = true;
				miniEventDataSetComboBox.Visible = true;
				groupBoxScreenFX.Enabled = false;
				groupBoxSingleParticle.Enabled = false;
				groupBoxLighting.Enabled = false;
				groupBoxBlur.Enabled = false;
				groupBoxParticleGen.Enabled = false;
				groupBoxVideo.Enabled = false;
				groupBoxMiniEventEffects.Enabled = true;
				groupBoxMiniCamera.Enabled = true;
				currentFormat = CutsceneEffectFileTypes.MiniEvent;
				currentFormatDialog = "mini-event format";
				label8.Text = "Visible Time:";
				subTimeXNumericUpDown.Location = subDummyMiniNumericUpDown.Location;
			}
			else if (fc.Length == 0x2DC00)
			{
				effectDataSetComboBox.Enabled = true;
				effectDataSetComboBox.Visible = true;
				miniEventDataSetComboBox.Enabled = false;
				miniEventDataSetComboBox.Visible = false;
				bigEndian = false;
				forceBigEndianToolStripMenuItem.Checked = false;
				dCBetaTheTrialToolStripMenuItem.Checked = true;
				dCBetaTheTrialToolStripMenuItem.Enabled = true;
				subtitleAudioOnlyToolStripMenuItem.Checked = false;
				subtitleAudioOnlyToolStripMenuItem.Enabled = true;
				normalToolStripMenuItem.Checked = false;
				normalToolStripMenuItem.Enabled = true;
				miniEventToolStripMenuItem.Checked = false;
				miniEventToolStripMenuItem.Enabled = false;
				groupBoxScreenFX.Enabled = true;
				groupBoxSingleParticle.Enabled = true;
				groupBoxLighting.Enabled = true;
				groupBoxBlur.Enabled = true;
				groupBoxParticleGen.Enabled = true;
				groupBoxVideo.Enabled = true;
				groupBoxMiniEventEffects.Enabled = false;
				groupBoxMiniCamera.Enabled = false;
				currentFormat = CutsceneEffectFileTypes.TheTrial;
				currentFormatDialog = "beta/The Trial format";
				label8.Text = "Visible Time/X Displacement:";
				subTimeXNumericUpDown.Location = subDummyNumericUpDown.Location;
			}
			else
			{
				effectDataSetComboBox.Enabled = true;
				effectDataSetComboBox.Visible = true;
				miniEventDataSetComboBox.Enabled = false;
				miniEventDataSetComboBox.Visible = false;
				miniEventToolStripMenuItem.Checked = false;
				miniEventToolStripMenuItem.Enabled = false;
				dCBetaTheTrialToolStripMenuItem.Checked = false;
				dCBetaTheTrialToolStripMenuItem.Enabled = true;
				subtitleAudioOnlyToolStripMenuItem.Enabled = true;
				normalToolStripMenuItem.Enabled = true;
				groupBoxMiniEventEffects.Enabled = false;
				groupBoxMiniCamera.Enabled = false;
				label8.Text = "Visible Time/X Displacement:";
				subTimeXNumericUpDown.Location = subDummyNumericUpDown.Location;
				if (fc.Length < 0x9900)
				{
					subtitleAudioOnlyToolStripMenuItem.Checked = true;
					normalToolStripMenuItem.Checked = false;
					groupBoxScreenFX.Enabled = false;
					groupBoxSingleParticle.Enabled = false;
					groupBoxLighting.Enabled = false;
					groupBoxBlur.Enabled = false;
					groupBoxParticleGen.Enabled = false;
					groupBoxVideo.Enabled = false;
					currentFormat = CutsceneEffectFileTypes.LanguageOnly;
					currentFormatDialog = "subtitle/audio only format";
				}
				else
				{
					subtitleAudioOnlyToolStripMenuItem.Checked = false;
					normalToolStripMenuItem.Checked = true;
					groupBoxScreenFX.Enabled = true;
					groupBoxSingleParticle.Enabled = true;
					groupBoxLighting.Enabled = true;
					groupBoxBlur.Enabled = true;
					groupBoxParticleGen.Enabled = true;
					groupBoxVideo.Enabled = true;
					currentFormat = CutsceneEffectFileTypes.Normal;
					currentFormatDialog = "normal format";
				}
				if (fc[4] > 0 || fc[8] > 0 || fc[0x800] > 0 || (fc.Length > 0x9900 && (fc[0x9800] > 0 || fc[0x26800] > 0)))
				{
					bigEndian = false;
					forceBigEndianToolStripMenuItem.Checked = false;
				}
				else
				{
					bigEndian = true;
					forceBigEndianToolStripMenuItem.Checked = true;
				}
			}
			ByteConverter.BigEndian = bigEndian;
			if (forceBigEndianToolStripMenuItem.Checked)
			{
				bigEndianDialog = " in Big Endian";
			}
			else
			{
				bigEndianDialog = "";
			}
			ClearEffectData();
			AddRecentFile(filename);
			//Add Data
			int address = 0;
			if (fc.Length <= 0x141C)
			{
				while (address < 0x100)
				{
					subs.Add(new SubtitleData(fc, address));
					address += SubtitleData.Size;
				}
				address = 0x100;
				while (address < 0x1400)
				{
					minieffs.Add(new MiniEventEffectData(fc, address));
					address += MiniEventEffectData.Size;
				}
				MiniFloats = new MiniEventEffectFloats(fc, 0x1400);

				int i = 0;
				int j = 0;
				int k = 0;
				int l = 0;
				int m = 32;
				do
				{
					subs.Add(new SubtitleData());
					m++;
				}
				while (m < 256);
				do
				{
					audios.Add(new AudioMiscData());
					l++;
				}
				while (l < 512);
				do
				{
					screens.Add(new ScreenData());
					blurs.Add(new BlurData());
					particleGens.Add(new ParticleGeneratorData());
					videos.Add(new VideoData());
					i++;
				}
				while (i < 64);
				do
				{
					singleParticles.Add(new SingleParticleData());
					j++;
				}
				while (j < 2048);
				do
				{
					lights.Add(new LightData());
					k++;
				}
				while (k < 1024);
			}
			else
			{
				int mini = 0;
				do
				{
					minieffs.Add(new MiniEventEffectData());
					mini++;
				}
				while (mini < 64);
				MiniFloats = new MiniEventEffectFloats();
				while (address < 0x800)
				{
					subs.Add(new SubtitleData(fc, address));
					address += SubtitleData.Size;
				}
				address = 0x800;
				while (address < 0x9800)
				{
					audios.Add(new AudioMiscData(fc, address));
					address += AudioMiscData.Size;
				}
				if (fc.Length > 0x9900)
				{
					address = 0x9800;
					while (address < 0xA800)
					{
						screens.Add(new ScreenData(fc, address, bigEndian));
						address += ScreenData.Size;
					}
					address = 0xA800;
					while (address < 0x26800)
					{
						singleParticles.Add(new SingleParticleData(fc, address));
						address += SingleParticleData.Size;
					}

					address = 0x26800;
					if (fc.Length > 0x2DC00)
					{
						while (address < 0x37800)
						{
							lights.Add(new LightData(fc, address));
							address += LightData.Size;
						}
						address = 0x37800;
						while (address < 0x38800)
						{
							blurs.Add(new BlurData(fc, address));
							address += BlurData.Size;
						}
						address = 0x38800;
						while (address < 0x39800)
						{
							particleGens.Add(new ParticleGeneratorData(fc, address));
							address += ParticleGeneratorData.Size;
						}
						address = 0x39800;
						while (address < fc.Length)
						{
							videos.Add(new VideoData(fc, address));
							address += VideoData.Size;
						}
					}
					else
					{
						// This normalizes The Trial's light data to match final's formatting.
						int lightbuff = 0;
						while (address < 0x27900)
						{
							lights.Add(new LightData(fc, address));
							address += LightData.Size;
						}
						do
						{
							lights.Add(new LightData());
							lightbuff++;
						}
						while (lightbuff < 192);
						address = 0x27900;
						while (address < 0x28A00)
						{
							lights.Add(new LightData(fc, address));
							address += LightData.Size;
						}
						lightbuff = 0;
						do
						{
							lights.Add(new LightData());
							lightbuff++;
						}
						while (lightbuff < 192);
						address = 0x28A00;
						while (address < 0x29B00)
						{
							lights.Add(new LightData(fc, address));
							address += LightData.Size;
						}
						lightbuff = 0;
						do
						{
							lights.Add(new LightData());
							lightbuff++;
						}
						while (lightbuff < 192);
						address = 0x29B00;
						while (address < 0x2AC00)
						{
							lights.Add(new LightData(fc, address));
							address += LightData.Size;
						}
						lightbuff = 0;
						do
						{
							lights.Add(new LightData());
							lightbuff++;
						}
						while (lightbuff < 192);
						address = 0x2AC00;
						while (address < 0x2BC00)
						{
							blurs.Add(new BlurData(fc, address));
							address += BlurData.Size;
						}
						address = 0x2BC00;
						while (address < 0x2CC00)
						{
							particleGens.Add(new ParticleGeneratorData(fc, address));
							address += ParticleGeneratorData.Size;
						}
						address = 0x2CC00;
						while (address < fc.Length)
						{
							videos.Add(new VideoData(fc, address));
							address += VideoData.Size;
						}
					}
				}
				else
				{
					int i = 0;
					int j = 0;
					int k = 0;
					do
					{
						screens.Add(new ScreenData());
						blurs.Add(new BlurData());
						particleGens.Add(new ParticleGeneratorData());
						videos.Add(new VideoData());
						i++;
					}
					while (i < 64);
					do
					{
						singleParticles.Add(new SingleParticleData());
						j++;
					}
					while (j < 2048);
					do
					{
						lights.Add(new LightData());
						k++;
					}
					while (k < 1024);
				}
			}
			UpdateEffectDataOnLoad();
			Text = "SA2 Cutscene Effect Editor - " + Path.GetFileName(filename);
		}
		#region Update Effect Data
		private void UpdateEffectDataOnLoad()
		{
			UpdateGUISubtitleData();
			UpdateGUIAudioData();
			UpdateGUIScreenData();
			UpdateGUISingleParticleData();
			UpdateGUILightData();
			UpdateGUIBlurData();
			UpdateGUIParticleGenData();
			UpdateGUIVideoData();
			UpdateGUIMiniEffectData();
			UpdateGUIMiniFloats();
		}
		private void UpdateGUISubtitleData()
		{
			subFrameNumericUpDown.Value = CurrentSubData.SubtitleStart;
			subTimeXNumericUpDown.Value = CurrentSubData.SubtitleVisibleX;
		}
		private void UpdateGUIAudioData()
		{
			audioFrameNumericUpDown.Value = CurrentAudioData.AudioStart;
			initSFXCheckBox.Checked = CurrentAudioData.SFXInit == 0;
			creditsScrollNumericUpDown.Value = CurrentAudioData.CreditsScroll;
			voiceIDnumericUpDown.Value = CurrentAudioData.VoiceID;
			musicFileTextBox.Text = CurrentAudioData.MusicSetting;
			jingleFileTextBox.Text = CurrentAudioData.JingleSetting;
			vSyncNumericUpDown.Value = CurrentAudioData.VsyncMode;
		}
		private void UpdateGUIScreenData()
		{
			screenStartNumericUpDown.Value = CurrentScreenData.ScreenStart;
			screenTypeComboBox.SelectedIndex = CurrentScreenData.ScreenType;
			screenANumericUpDown.Value = CurrentScreenData.ScreenA;
			screenColorPanel.BackColor = Color.FromArgb((int)CurrentScreenData.ScreenA,
				(int)CurrentScreenData.ScreenR,
				(int)CurrentScreenData.ScreenG,
				(int)CurrentScreenData.ScreenB);
			screenTexFadeCheckBox.Checked = CurrentScreenData.Fade > 0;
			screenTexIDNumericUpDown.Value = CurrentScreenData.TexID;
			screenTexPosXNumericUpDown.Value = CurrentScreenData.TexPosX;
			screenTexPosYNumericUpDown.Value = CurrentScreenData.TexPosY;
			screenTexWidthTextBox.Text = CurrentScreenData.TexWidth.ToString();
			screenTexHeightTextBox.Text = CurrentScreenData.TexHeight.ToString();
		}
		private void UpdateGUISingleParticleData()
		{
			particleFrameNumericUpDown.Value = CurrentSingleParticleData.ParticleStart;
			particleTypeComboBox.SelectedIndex = CurrentSingleParticleData.ParticleType;
			particleMotionIDNumericUpDown.Value = CurrentSingleParticleData.ParticleMotionID;
			particleTexIDTextBox.Text = CurrentSingleParticleData.ParticleTexID.ToString();
			particlePulseXTextBox.Text = CurrentSingleParticleData.PulseTypeX.ToString();
			particlePulseConstYTextBox.Text = CurrentSingleParticleData.PulseConstY.ToString();
			particleSizeTextBox.Text = CurrentSingleParticleData.ParticleSize.ToString();
		}
		private void UpdateGUILightData()
		{
			lightFrameNumericUpDown.Value = CurrentLightData.LightStart;
			lightFadeNumericUpDown.Value = CurrentLightData.FadeType;
			lightDirXTextBox.Text = CurrentLightData.LightDirection.X.ToString();
			lightDirYTextBox.Text = CurrentLightData.LightDirection.Y.ToString();
			lightDirZTextBox.Text = CurrentLightData.LightDirection.Z.ToString();
			lightColorPanel.BackColor = Color.FromArgb(255,
				(int)(CurrentLightData.LightColor.X * 255f),
				(int)(CurrentLightData.LightColor.Y * 255f),
				(int)(CurrentLightData.LightColor.Z * 255f));
			ambientColorPanel.BackColor = Color.FromArgb(255,
				(int)(CurrentLightData.AmbientColor.X * 255f),
				(int)(CurrentLightData.AmbientColor.Y * 255f),
				(int)(CurrentLightData.AmbientColor.Z * 255f));
			singleAmbIntensityTextBox.Text = CurrentLightData.SingleAmbientIntensity.ToString();

		}
		private void UpdateGUIBlurData()
		{
			blurFrameNumericUpDown.Value = CurrentBlurData.BlurStart;
			blurDurationNumericUpDown.Value = CurrentBlurData.Duration;
			blurModel1NumericUpDown.Value = CurrentBlurData.BlurModel1;
			blurModel2NumericUpDown.Value = CurrentBlurData.BlurModel2;
			blurModel3NumericUpDown.Value = CurrentBlurData.BlurModel3;
			blurModel4NumericUpDown.Value = CurrentBlurData.BlurModel4;
			blurModel5NumericUpDown.Value = CurrentBlurData.BlurModel5;
			blurModel6NumericUpDown.Value = CurrentBlurData.BlurModel6;
			blurCountNumericUpDown.Value = CurrentBlurData.Instances;
		}
		private void UpdateGUIParticleGenData()
		{
			particleGenStartNumericUpDown.Value = CurrentParticleGenData.ParticleGenStart;
			particleGenTypeComboBox.SelectedIndex = CurrentParticleGenData.ParticleType;
			particleGenCountNumericUpDown.Value = CurrentParticleGenData.Count;
			particleGenXPosTextBox.Text = CurrentParticleGenData.Position.X.ToString();
			particleGenYPosTextBox.Text = CurrentParticleGenData.Position.Y.ToString();
			particleGenZPosTextBox.Text = CurrentParticleGenData.Position.Z.ToString();
			particleGenSpreadTextBox.Text = CurrentParticleGenData.Spread.ToString();
			particleGenVelocityXTextBox.Text = CurrentParticleGenData.Velocity.X.ToString();
			particleGenVelocityYTextBox.Text = CurrentParticleGenData.Velocity.Y.ToString();
			particleGenVelocityZTextBox.Text = CurrentParticleGenData.Velocity.Z.ToString();
			particleGenRotZNumericUpDown.Value = CurrentParticleGenData.AddRotationZ;
			particleGenYRotNumericUpDown.Value = CurrentParticleGenData.AddRotationY;
			particleGenXRotNumericUpDown.Value = CurrentParticleGenData.AddRotationX;
			particleGenUnkNumericUpDown.Value = CurrentParticleGenData.Unk;
			particleGenRotationConstNumericUpDown.Value = CurrentParticleGenData.RotationConst;
			particleGenFrameDelayNumericUpDown.Value = CurrentParticleGenData.FrameDelay;
			particleGenYScaleNumericUpDown.Value = CurrentParticleGenData.YScale;
			particleGenModelIDNumericUpDown.Value += CurrentParticleGenData.ModelID;

		}
		private void UpdateGUIVideoData()
		{
			videoFrameNumericUpDown.Value = CurrentVideoData.VideoStart;
			videoXPosNumericUpDown.Value = CurrentVideoData.VideoPosX;
			videoYPosNumericUpDown.Value = CurrentVideoData.VideoPosY;
			videoDepthTextBox.Text = CurrentVideoData.VideoDepth.ToString();
			videoTypeComboBox.SelectedIndex = Math.Min((int)CurrentVideoData.VideoOverlayType, 7);
			videoTexIDNumericUpDown.Value = CurrentVideoData.VideoOverlayTexID;
			videoFilenameTextBox.Text = CurrentVideoData.VideoName;
		}
		private void UpdateGUIMiniEffectData()
		{
			miniEventEffectStartNumericUpDown.Value = CurrentMiniEventEffectData.EffectStart;
			miniEventFadeComboBox.SelectedIndex = Math.Min((int)CurrentMiniEventEffectData.ScreenFadeType, 5);
			miniEventSFXID1NumericUpDown.Value = CurrentMiniEventEffectData.SFXEntry1;
			miniEventSFXID2NumericUpDown.Value = CurrentMiniEventEffectData.SFXEntry2;
			miniEventVoiceIDNumericUpDown.Value = CurrentMiniEventEffectData.VoiceEntry;
			miniEventMusicNameTextBox.Text = CurrentMiniEventEffectData.MusicEntry;
			miniEventJingleNameTextBox.Text = CurrentMiniEventEffectData.JingleEntry;
			miniEventRumbleNumericUpDown.Value = CurrentMiniEventEffectData.RumblePower;
		}
		private void UpdateGUIMiniFloats()
		{

			miniFloatCamPosXTextBox.Text = MiniFloats.CameraPos.X.ToString();
			miniFloatCamPosYTextBox.Text = MiniFloats.CameraPos.Y.ToString();
			miniFloatCamPosZTextBox.Text = MiniFloats.CameraPos.Z.ToString();
			miniEventPlayerXRotTextBox.Text = MiniFloats.PlayerRot.X.ToString("X");
			miniEventPlayerYRotTextBox.Text = MiniFloats.PlayerRot.Y.ToString("X");
			miniEventPlayerZRotTextBox.Text = MiniFloats.PlayerRot.Z.ToString("X");
			miniEventCameraYRotTextBox.Text = MiniFloats.CameraYRot.ToString("X");
			
		}
		private void SetScreenColorFromNumerics()
		{
			screenColorPanel.BackColor = Color.FromArgb((int)screenANumericUpDown.Value, screenColorPanel.BackColor);
		}
		private void ClearEffectData()
		{
			subInstanceNumericUpDown.Value = 0;
			audioInstanceNumericUpDown.Value = 0;
			screenInstanceNumericUpDown.Value = 0;
			particleInstanceNumericUpDown.Value = 0;
			lightInstanceNumericUpDown.Value = 0;
			lightSetNumericUpDown.Value = 1;
			blurInstancesNumericUpDown.Value = 0;
			particleGenInstanceNumericUpDown.Value = 0;
			videoInstanceNumericUpDown.Value = 0;
			subs.Clear();
			audios.Clear();
			screens.Clear();
			singleParticles.Clear();
			lights.Clear();
			blurs.Clear();
			particleGens.Clear();
			videos.Clear();
			minieffs.Clear();
			MiniFloats = new MiniEventEffectFloats();
		}
		#endregion
		#region Update File Data

		#endregion
		private void UpdateRecentFiles()
		{
			recentFilesToolStripMenuItem.DropDownItems.Clear();
			foreach (string item in recentFiles)
				recentFilesToolStripMenuItem.DropDownItems.Add(item);
		}
		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			switch (MessageBox.Show(this, "Do you want to save before exiting?", "SA2 Cutscene Effect Editor", MessageBoxButtons.YesNoCancel, MessageBoxIcon.None, MessageBoxDefaultButton.Button3))
			{
				case DialogResult.Yes:
					break;
				case DialogResult.Cancel:
					e.Cancel = true;
					return;
			}
			settingsFile.Save();
		}
		private void AddRecentFile(string filename)
		{
			if (recentFiles.Contains(filename))
				recentFiles.Remove(filename);
			recentFiles.Insert(0, filename);
			if (recentFiles.Count > 10)
				recentFiles.RemoveAt(10);
			UpdateRecentFiles();
		}
		private bool IsLanguageOnly()
		{
			if (currentFormat != CutsceneEffectFileTypes.LanguageOnly)
				return false;
			else
				return true;
		}
		private bool IsTheTrialFile()
		{
			if (currentFormat != CutsceneEffectFileTypes.TheTrial)
				return false;
			else
				return true;
		}
		private void SaveFile()
		{
			ByteConverter.BigEndian = bigEndian;
			List<byte> fc = new List<byte>();
			if (currentFormat == CutsceneEffectFileTypes.MiniEvent)
			{
				for (int i = 0; i < 32; i++)
				{
					fc.AddRange(subs[i].GetBytes());
				}
				foreach (MiniEventEffectData minifx in minieffs)
				{
					fc.AddRange(minifx.GetBytes());
				}
				fc.AddRange(MiniFloats.GetBytes());
			}
			else
			{
				foreach (SubtitleData sub in subs)
				{
					fc.AddRange(sub.GetBytes());
				}
				foreach (AudioMiscData audio in audios)
				{
					fc.AddRange(audio.GetBytes());
				}
				if (!IsLanguageOnly())
				{
					foreach (ScreenData screen in screens)
					{
						fc.AddRange(screen.GetBytes(bigEndian));
					}
					foreach (SingleParticleData sp in singleParticles)
					{
						fc.AddRange(sp.GetBytes());
					}
					if (IsTheTrialFile())
					{
						for (int i = 0; i < 64; i++)
						{
							fc.AddRange(lights[i].GetBytes());
						}
						for (int i = 256; i < 320; i++)
						{
							fc.AddRange(lights[i].GetBytes());
						}
						for (int i = 512; i < 576; i++)
						{
							fc.AddRange(lights[i].GetBytes());
						}
						for (int i = 768; i < 832; i++)
						{
							fc.AddRange(lights[i].GetBytes());
						}
					}
					else
					{
						foreach (LightData light in lights)
						{
							fc.AddRange(light.GetBytes());
						}
					}
					foreach (BlurData blur in blurs)
					{
						fc.AddRange(blur.GetBytes());
					}
					foreach (ParticleGeneratorData pg in particleGens)
					{
						fc.AddRange(pg.GetBytes());
					}
					foreach (VideoData video in videos)
					{
						fc.AddRange(video.GetBytes());
					}
				}
			}
			if (Path.GetExtension(filename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
				Prs.Compress(fc.ToArray(), filename);
			else
				File.WriteAllBytes(filename, fc.ToArray());
		}
		private void subInstanceNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			UpdateGUISubtitleData();
		}
		private void audioInstanceNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			UpdateGUIAudioData();
		}
		private void screenInstanceNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			UpdateGUIScreenData();
		}
		private void particleInstanceNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			UpdateGUISingleParticleData();
		}
		private void lightInstanceNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			UpdateGUILightData();
		}
		private void blurInstancesNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			UpdateGUIBlurData();
		}
		private void particleGenInstanceNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			UpdateGUIParticleGenData();
		}
		private void videoInstanceNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			UpdateGUIVideoData();
		}
		private void miniEventEffectInstanceNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			UpdateGUIMiniEffectData();
		}
		private void screenANumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			SetScreenColorFromNumerics();
			CurrentScreenData.ScreenA = (byte)screenANumericUpDown.Value;
		}
		private void lightColorPanel_Click(object sender, EventArgs e)
		{
			colorDialog.Color = lightColorPanel.BackColor;
			if (colorDialog.ShowDialog() == DialogResult.OK)
			{
				float lightR = colorDialog.Color.R / 255f;
				float lightG = colorDialog.Color.G / 255f;
				float lightB = colorDialog.Color.B / 255f;
				lightColorPanel.BackColor = colorDialog.Color;
				CurrentLightData.LightColor.X = lightR;
				CurrentLightData.LightColor.Y = lightG;
				CurrentLightData.LightColor.Z = lightB;
			}
		}
		private void ambientColorPanel_Click(object sender, EventArgs e)
		{
			colorDialog.Color = ambientColorPanel.BackColor;
			if (colorDialog.ShowDialog() == DialogResult.OK)
			{
				float ambientR = colorDialog.Color.R / 255f;
				float ambientG = colorDialog.Color.G / 255f;
				float ambientB = colorDialog.Color.B / 255f;
				ambientColorPanel.BackColor = colorDialog.Color;
				CurrentLightData.AmbientColor.X = ambientR;
				CurrentLightData.AmbientColor.Y = ambientG;
				CurrentLightData.AmbientColor.Z = ambientB;
			}
		}
		private void screenColorPanel_Click(object sender, EventArgs e)
		{
			colorDialog.Color = screenColorPanel.BackColor;
			if (colorDialog.ShowDialog() == DialogResult.OK)
			{
				screenColorPanel.BackColor = colorDialog.Color;
				CurrentScreenData.ScreenR = colorDialog.Color.R;
				CurrentScreenData.ScreenG = colorDialog.Color.G;
				CurrentScreenData.ScreenB = colorDialog.Color.B;
			}
		}
		private void lightSetNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			UpdateGUILightData();
		}
		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (filename == null)
			{
				saveAsToolStripMenuItem_Click(sender, e);
				return;
			}
			SaveFile();
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(this, $"File will be saved in the {currentFormatDialog}{bigEndianDialog}. Continue?", "SA2 Cutscene Effect Editor", MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button3) == DialogResult.No)
			{
				return;
			}
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "prs", Filter = "Supported Files|*.prs;*.scr|All Files|*.*" })
			{
				if (filename != null)
				{
					dlg.FileName = Path.GetFileName(filename);
					dlg.InitialDirectory = Path.GetDirectoryName(filename);
				}
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					filename = dlg.FileName;
					SaveFile();
					AddRecentFile(filename);
					Text = "SA2 Cutscene Effect Editor - " + Path.GetFileName(filename);
				}
			}
		}

		private void normalToolStripMenuItem_Click(object sender, EventArgs e)
		{
			currentFormat = CutsceneEffectFileTypes.Normal;
			currentFormatDialog = "normal format";
			miniEventDataSetComboBox.Enabled = false;
			miniEventDataSetComboBox.Visible = false;
			effectDataSetComboBox.Visible = true;
			effectDataSetComboBox.Enabled = true;
			dCBetaTheTrialToolStripMenuItem.Checked = false;
			normalToolStripMenuItem.Checked = true;
			miniEventToolStripMenuItem.Checked = false;
			subtitleAudioOnlyToolStripMenuItem.Checked = false;
			groupBoxScreenFX.Enabled = true;
			groupBoxSingleParticle.Enabled = true;
			groupBoxLighting.Enabled = true;
			groupBoxBlur.Enabled = true;
			groupBoxParticleGen.Enabled = true;
			groupBoxVideo.Enabled = true;
		}

		private void dCBetaTheTrialToolStripMenuItem_Click(object sender, EventArgs e)
		{
			currentFormat = CutsceneEffectFileTypes.TheTrial;
			currentFormatDialog = "beta/The Trial format";
			miniEventDataSetComboBox.Enabled = false;
			miniEventDataSetComboBox.Visible = false;
			effectDataSetComboBox.Visible = true;
			effectDataSetComboBox.Enabled = true;
			dCBetaTheTrialToolStripMenuItem.Checked = true;
			normalToolStripMenuItem.Checked = false;
			miniEventToolStripMenuItem.Checked = false;
			subtitleAudioOnlyToolStripMenuItem.Checked = false;
			groupBoxScreenFX.Enabled = true;
			groupBoxSingleParticle.Enabled = true;
			groupBoxLighting.Enabled = true;
			groupBoxBlur.Enabled = true;
			groupBoxParticleGen.Enabled = true;
			groupBoxVideo.Enabled = true;
		}

		private void subtitleAudioOnlyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			currentFormat = CutsceneEffectFileTypes.LanguageOnly;
			currentFormatDialog = "subtitle/audio only format";
			miniEventDataSetComboBox.Enabled = false;
			miniEventDataSetComboBox.Visible = false;
			effectDataSetComboBox.Visible = true;
			effectDataSetComboBox.Enabled = true;
			dCBetaTheTrialToolStripMenuItem.Checked = false;
			normalToolStripMenuItem.Checked = false;
			miniEventToolStripMenuItem.Checked = false;
			subtitleAudioOnlyToolStripMenuItem.Checked = true;
			groupBoxScreenFX.Enabled = false;
			groupBoxSingleParticle.Enabled = false;
			groupBoxLighting.Enabled = false;
			groupBoxBlur.Enabled = false;
			groupBoxParticleGen.Enabled = false;
			groupBoxVideo.Enabled = false;
		}
		private void miniEventToolStripMenuItem_Click(object sender, EventArgs e)
		{
			currentFormat = CutsceneEffectFileTypes.MiniEvent;
			currentFormatDialog = "mini-event format";
			miniEventDataSetComboBox.Enabled = true;
			miniEventDataSetComboBox.Visible = true;
			effectDataSetComboBox.Visible = false;
			effectDataSetComboBox.Enabled = false;
			dCBetaTheTrialToolStripMenuItem.Checked = false;
			normalToolStripMenuItem.Checked = false;
			subtitleAudioOnlyToolStripMenuItem.Checked = false;
			miniEventToolStripMenuItem.Checked = true;
			groupBoxScreenFX.Enabled = false;
			groupBoxSingleParticle.Enabled = false;
			groupBoxLighting.Enabled = false;
			groupBoxBlur.Enabled = false;
			groupBoxParticleGen.Enabled = false;
			groupBoxVideo.Enabled = false;
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			filename = null;
			Text = "SA2 Cutscene Effect Editor";
			ClearEffectData();
			currentFormat = CutsceneEffectFileTypes.Normal;
			currentFormatDialog = "normal format";
			miniEventDataSetComboBox.Enabled = false;
			miniEventDataSetComboBox.Visible = false;
			effectDataSetComboBox.Visible = true;
			effectDataSetComboBox.Enabled = true;
			dCBetaTheTrialToolStripMenuItem.Checked = false;
			normalToolStripMenuItem.Checked = true;
			subtitleAudioOnlyToolStripMenuItem.Checked = false;
			miniEventToolStripMenuItem.Checked = false;
			groupBoxScreenFX.Enabled = true;
			groupBoxSingleParticle.Enabled = true;
			groupBoxLighting.Enabled = true;
			groupBoxBlur.Enabled = true;
			groupBoxParticleGen.Enabled = true;
			groupBoxVideo.Enabled = true;
			InitializeEffectData();
		}

		private void InitializeEffectData()
		{
			int i = 0;
			int j = 0;
			int k = 0;
			int l = 0;
			int m = 0;
			do
			{
				subs.Add(new SubtitleData());
				m++;
			}
			while (m < 256);
			do
			{
				audios.Add(new AudioMiscData());
				l++;
			}
			while (l < 512);
			do
			{
				screens.Add(new ScreenData());
				blurs.Add(new BlurData());
				particleGens.Add(new ParticleGeneratorData());
				videos.Add(new VideoData());
				i++;
			}
			while (i < 64);
			do
			{
				singleParticles.Add(new SingleParticleData());
				j++;
			}
			while (j < 2048);
			do
			{
				lights.Add(new LightData());
				k++;
			}
			while (k < 1024);

			int mini = 0;
			do
			{
				minieffs.Add(new MiniEventEffectData());
				mini++;
			}
			while (mini < 64);
			MiniFloats = new MiniEventEffectFloats();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void forceBigEndianToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (forceBigEndianToolStripMenuItem.Checked)
			{
				forceBigEndianToolStripMenuItem.Checked = false;
				bigEndian = false;
				bigEndianDialog = "";
			}
			else
			{
				forceBigEndianToolStripMenuItem.Checked = true;
				bigEndian = true;
				bigEndianDialog = " in Big Endian";
			}
		}

		private void effectDataSetComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			groupBoxSubtitle.Visible = false;
			groupBoxSubtitle.Enabled = false;
			groupBoxAudioMisc.Enabled = false;
			groupBoxAudioMisc.Visible = false;
			groupBoxScreenFX.Enabled = false;
			groupBoxScreenFX.Visible = false;
			groupBoxSingleParticle.Enabled = false;
			groupBoxSingleParticle.Visible = false;
			groupBoxLighting.Enabled = false;
			groupBoxLighting.Visible = false;
			groupBoxBlur.Enabled = false;
			groupBoxBlur.Visible = false;
			groupBoxParticleGen.Enabled = false;
			groupBoxParticleGen.Visible = false;
			groupBoxVideo.Enabled = false;
			groupBoxVideo.Visible = false;
			groupBoxMiniEventEffects.Enabled = false;
			groupBoxMiniEventEffects.Visible = false;
			groupBoxMiniCamera.Enabled = false;
			groupBoxMiniCamera.Visible = false;
			label8.Text = "Visible Time/X Displacement:";
			subTimeXNumericUpDown.Location = subDummyNumericUpDown.Location;
			Size = currentFormSize;
			if (miniEventToolStripMenuItem.Checked)
			{ return; }
			switch (effectDataSetComboBox.SelectedIndex)
			{
				case 0:
				default:
					groupBoxSubtitle.Enabled = true;
					groupBoxSubtitle.Visible = true;
					break;
				case 1:
					groupBoxAudioMisc.Enabled = true;
					groupBoxAudioMisc.Visible = true;
					break;
				case 2:
					groupBoxScreenFX.Enabled = true;
					groupBoxScreenFX.Visible = true;
					break;
				case 3:
					groupBoxSingleParticle.Enabled = true;
					groupBoxSingleParticle.Visible = true;
					break;
				case 4:
					groupBoxLighting.Enabled = true;
					groupBoxLighting.Visible = true;
					break;
				case 5:
					groupBoxBlur.Enabled = true;
					groupBoxBlur.Visible = true;
					break;
				case 6:
					groupBoxParticleGen.Enabled = true;
					groupBoxParticleGen.Visible = true;
					break;
				case 7:
					groupBoxVideo.Enabled = true;
					groupBoxVideo.Visible = true;
					break;
			}
		}

		private void miniEventDataSetComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			groupBoxSubtitle.Visible = false;
			groupBoxSubtitle.Enabled = false;
			groupBoxAudioMisc.Enabled = false;
			groupBoxAudioMisc.Visible = false;
			groupBoxScreenFX.Enabled = false;
			groupBoxScreenFX.Visible = false;
			groupBoxSingleParticle.Enabled = false;
			groupBoxSingleParticle.Visible = false;
			groupBoxLighting.Enabled = false;
			groupBoxLighting.Visible = false;
			groupBoxBlur.Enabled = false;
			groupBoxBlur.Visible = false;
			groupBoxParticleGen.Enabled = false;
			groupBoxParticleGen.Visible = false;
			groupBoxVideo.Enabled = false;
			groupBoxVideo.Visible = false;
			groupBoxMiniEventEffects.Enabled = false;
			groupBoxMiniEventEffects.Visible = false;
			groupBoxMiniCamera.Enabled = false;
			groupBoxMiniCamera.Visible = false;
			label8.Text = "Visible Time:";
			subTimeXNumericUpDown.Location = subDummyMiniNumericUpDown.Location;
			Size = currentMEFormSize;
			if (!miniEventToolStripMenuItem.Checked)
			{ return; }
			switch (miniEventDataSetComboBox.SelectedIndex)
			{
				case 0:
				default:
					groupBoxSubtitle.Enabled = true;
					groupBoxSubtitle.Visible = true;
					break;
				case 1:
					groupBoxMiniEventEffects.Enabled = true;
					groupBoxMiniEventEffects.Visible = true;
					break;
				case 2:
					groupBoxMiniCamera.Enabled = true;
					groupBoxMiniCamera.Visible = true;
					break;
			}
		}
		#region Applying Changes to Raw Data
		#region Subtitles
		private void subFrameNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentSubData.SubtitleStart = (int)subFrameNumericUpDown.Value;
		}

		private void subTimeXNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentSubData.SubtitleVisibleX = (int)subTimeXNumericUpDown.Value;
		}
		#endregion
		#region Audio/Misc Data
		private void audioFrameNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentAudioData.AudioStart = (int)audioFrameNumericUpDown.Value;
		}
		private void creditsScrollNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentAudioData.CreditsScroll = (sbyte)creditsScrollNumericUpDown.Value;
		}
		private void initSFXCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			CurrentAudioData.SFXInit = initSFXCheckBox.Checked ? (sbyte)0 : (sbyte)-1;
		}
		private void voiceIDnumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentAudioData.VoiceID = (short)voiceIDnumericUpDown.Value;
		}
		private void musicFileTextBox_TextChanged(object sender, EventArgs e)
		{
			CurrentAudioData.MusicSetting = musicFileTextBox.Text;
		}
		private void jingleFileTextBox_TextChanged(object sender, EventArgs e)
		{
			CurrentAudioData.JingleSetting = jingleFileTextBox.Text;
		}
		private void vSyncNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentAudioData.VsyncMode = (int)vSyncNumericUpDown.Value;
		}
		#endregion
		#region Screen Effects
		private void screenStartNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentScreenData.ScreenStart = (int)screenStartNumericUpDown.Value;
		}

		private void screenTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			CurrentScreenData.ScreenType = (byte)screenTypeComboBox.SelectedIndex;
		}

		private void screenTexIDNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentScreenData.TexID = (short)screenTexIDNumericUpDown.Value;
		}

		private void screenTexFadeCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			CurrentScreenData.Fade = screenTexFadeCheckBox.Checked ? (short)0 : (short)1;
		}

		private void screenTexTimeNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentScreenData.VisibleTime = (int)screenTexTimeNumericUpDown.Value;
		}

		private void screenTexPosXNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentScreenData.TexPosX = (short)screenTexPosXNumericUpDown.Value;
		}

		private void screenTexPosYNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentScreenData.TexPosY = (short)screenTexPosYNumericUpDown.Value;
		}

		private void screenTexWidthTextBox_TextChanged(object sender, EventArgs e)
		{
			CurrentScreenData.TexWidth = float.Parse(screenTexWidthTextBox.Text);
		}

		private void screenTexHeightTextBox_TextChanged(object sender, EventArgs e)
		{
			CurrentScreenData.TexHeight = float.Parse(screenTexHeightTextBox.Text);
		}
		#endregion
		#region Single Particle Data
		private void particleFrameNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentSingleParticleData.ParticleStart = (int)particleFrameNumericUpDown.Value;
		}

		private void particleTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			CurrentSingleParticleData.ParticleType = (byte)particleTypeComboBox.SelectedIndex;
		}

		private void particleMotionIDNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentSingleParticleData.ParticleMotionID = (byte)particleMotionIDNumericUpDown.Value;
		}

		private void particleTexIDTextBox_TextChanged(object sender, EventArgs e)
		{
			CurrentSingleParticleData.ParticleTexID = float.Parse(particleTexIDTextBox.Text);
		}

		private void particlePulseXTextBox_TextChanged(object sender, EventArgs e)
		{
			CurrentSingleParticleData.PulseTypeX = float.Parse(particlePulseXTextBox.Text);
		}

		private void particlePulseConstYTextBox_TextChanged(object sender, EventArgs e)
		{
			CurrentSingleParticleData.PulseConstY = float.Parse(particlePulseConstYTextBox.Text);
		}

		private void particleSizeTextBox_TextChanged(object sender, EventArgs e)
		{
			CurrentSingleParticleData.ParticleSize = float.Parse(particleSizeTextBox.Text);
		}
		#endregion
		#region Lighting
		private void lightFrameNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentLightData.LightStart = (int)lightFrameNumericUpDown.Value;
		}

		private void lightFadeNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentLightData.FadeType = (int)lightFadeNumericUpDown.Value;
		}
		private void lightDirXTextBox_TextChanged(object sender, EventArgs e)
		{
			CurrentLightData.LightDirection.X = float.Parse(lightDirXTextBox.Text);
		}
		private void lightDirYTextBox_TextChanged(object sender, EventArgs e)
		{
			CurrentLightData.LightDirection.Y = float.Parse(lightDirYTextBox.Text);
		}

		private void lightDirZTextBox_TextChanged(object sender, EventArgs e)
		{
			CurrentLightData.LightDirection.Z = float.Parse(lightDirZTextBox.Text);
		}
		private void singleAmbIntensityTextBox_TextChanged(object sender, EventArgs e)
		{
			CurrentLightData.SingleAmbientIntensity = float.Parse(singleAmbIntensityTextBox.Text);
		}
		#endregion
		#region Blur Models
		private void blurFrameNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentBlurData.BlurStart = (int)blurFrameNumericUpDown.Value;
		}

		private void blurDurationNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentBlurData.Duration = (int)blurDurationNumericUpDown.Value;
		}

		private void blurModel1NumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentBlurData.BlurModel1 = (sbyte)blurModel1NumericUpDown.Value;
		}

		private void blurModel2NumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentBlurData.BlurModel2 = (sbyte)blurModel2NumericUpDown.Value;
		}

		private void blurModel3NumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentBlurData.BlurModel3 = (sbyte)blurModel3NumericUpDown.Value;
		}

		private void blurModel4NumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentBlurData.BlurModel4 = (sbyte)blurModel4NumericUpDown.Value;
		}

		private void blurModel5NumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentBlurData.BlurModel5 = (sbyte)blurModel5NumericUpDown.Value;
		}

		private void blurModel6NumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentBlurData.BlurModel6 = (sbyte)blurModel6NumericUpDown.Value;
		}

		private void blurCountNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentBlurData.Instances = (int)blurCountNumericUpDown.Value;
		}
		#endregion
		#region Particle Generator Data
		private void particleGenStartNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentParticleGenData.ParticleGenStart = (int)particleGenStartNumericUpDown.Value;
		}

		private void particleGenTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			CurrentParticleGenData.ParticleType = (int)particleGenTypeComboBox.SelectedIndex;
		}

		private void particleGenCountNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentParticleGenData.Count = (int)particleGenCountNumericUpDown.Value;
		}

		private void particleGenXPosTextBox_TextChanged(object sender, EventArgs e)
		{
			CurrentParticleGenData.Position.X = float.Parse(particleGenXPosTextBox.Text);
		}

		private void particleGenYPosTextBox_TextChanged(object sender, EventArgs e)
		{
			CurrentParticleGenData.Position.Y = float.Parse(particleGenYPosTextBox.Text);
		}

		private void particleGenZPosTextBox_TextChanged(object sender, EventArgs e)
		{
			CurrentParticleGenData.Position.Z = float.Parse(particleGenZPosTextBox.Text);
		}
		private void particleGenUnk1ATextBox_TextChanged(object sender, EventArgs e)
		{
			CurrentParticleGenData.Velocity.X = float.Parse(particleGenVelocityXTextBox.Text);
		}

		private void particleGenUnk1BTextBox_TextChanged(object sender, EventArgs e)
		{
			CurrentParticleGenData.Velocity.Y = float.Parse(particleGenVelocityYTextBox.Text);
		}

		private void particleGenUnk1CTextBox_TextChanged(object sender, EventArgs e)
		{
			CurrentParticleGenData.Velocity.Z = float.Parse(particleGenVelocityZTextBox.Text);
		}

		private void particleGenUnk1NumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentParticleGenData.AddRotationZ = (short)particleGenRotZNumericUpDown.Value;
		}

		private void particleGenUnk2NumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentParticleGenData.AddRotationY = (short)particleGenYRotNumericUpDown.Value;
		}

		private void particleGenUnk6NumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentParticleGenData.YScale = (int)particleGenYScaleNumericUpDown.Value;
		}

		private void particleGenUnk3NumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentParticleGenData.AddRotationX = (short)particleGenXRotNumericUpDown.Value;
		}

		private void particleGenUnk4NumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentParticleGenData.Unk = (short)particleGenUnkNumericUpDown.Value;
		}

		private void particleGenUnk9TextBox_TextChanged(object sender, EventArgs e)
		{
			CurrentParticleGenData.Spread = float.Parse(particleGenSpreadTextBox.Text);
		}

		private void particleGenUnk7NumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentParticleGenData.ModelID = (int)particleGenModelIDNumericUpDown.Value;
		}

		private void particleGenUnk5NumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentParticleGenData.RotationConst = (int)particleGenRotationConstNumericUpDown.Value;
		}

		private void particleGenFrameDelayNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentParticleGenData.FrameDelay = (int)particleGenFrameDelayNumericUpDown.Value;
		}
		#endregion
		#region Video Data
		private void videoFrameNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentVideoData.VideoStart = (int)videoFrameNumericUpDown.Value;
		}

		private void videoXPosNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentVideoData.VideoPosX = (short)videoXPosNumericUpDown.Value;
		}

		private void videoYPosNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentVideoData.VideoPosY = (short)videoYPosNumericUpDown.Value;
		}

		private void videoDepthTextBox_TextChanged(object sender, EventArgs e)
		{
			CurrentVideoData.VideoDepth = float.Parse(videoDepthTextBox.Text);
		}

		private void videoTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			CurrentVideoData.VideoOverlayType = (byte)videoTypeComboBox.SelectedIndex;
		}

		private void videoTexIDNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentVideoData.VideoOverlayTexID = (sbyte)videoTexIDNumericUpDown.Value;
		}

		private void videoFilenameTextBox_TextChanged(object sender, EventArgs e)
		{
			CurrentVideoData.VideoName = videoFilenameTextBox.Text;
		}
		#endregion
		#region Mini-Event Data

		private void miniEventEffectStartNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentMiniEventEffectData.EffectStart = (int)miniEventEffectStartNumericUpDown.Value;
		}
		private void miniEventFadeComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			CurrentMiniEventEffectData.ScreenFadeType = (byte)miniEventFadeComboBox.SelectedIndex;
		}

		private void miniEventSFXID1NumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentMiniEventEffectData.SFXEntry1 = (sbyte)miniEventSFXID1NumericUpDown.Value;
		}

		private void miniEventSFXID2NumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentMiniEventEffectData.SFXEntry2 = (sbyte)miniEventSFXID2NumericUpDown.Value;
		}
		private void miniEventVoiceIDNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentMiniEventEffectData.VoiceEntry = (short)miniEventVoiceIDNumericUpDown.Value;
		}

		private void miniEventMusicNameTextBox_TextChanged(object sender, EventArgs e)
		{
			CurrentMiniEventEffectData.MusicEntry = miniEventMusicNameTextBox.Text;
		}

		private void miniEventJingleNameTextBox_TextChanged(object sender, EventArgs e)
		{
			CurrentMiniEventEffectData.JingleEntry = miniEventJingleNameTextBox.Text;
		}

		private void miniEventRumbleNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			CurrentMiniEventEffectData.RumblePower = (int)miniEventRumbleNumericUpDown.Value;
		}
		private void miniFloatUnk1ATextBox_TextChanged(object sender, EventArgs e)
		{
			MiniFloats.CameraPos.X = float.Parse(miniFloatCamPosXTextBox.Text);
		}

		private void miniFloatUnk1BTextBox_TextChanged(object sender, EventArgs e)
		{
			MiniFloats.CameraPos.Y = float.Parse(miniFloatCamPosYTextBox.Text);
		}

		private void miniFloatUnk1CTextBox_TextChanged(object sender, EventArgs e)
		{
			MiniFloats.CameraPos.Z = float.Parse(miniFloatCamPosZTextBox.Text);
		}

		private void miniFloatUnk2TextBox_TextChanged(object sender, EventArgs e)
		{
			MiniFloats.PlayerRot.X = int.Parse(miniEventPlayerXRotTextBox.Text, System.Globalization.NumberStyles.HexNumber);
		}

		private void miniEventUnk3ATextBox_TextChanged(object sender, EventArgs e)
		{
			MiniFloats.PlayerRot.Y = int.Parse(miniEventPlayerYRotTextBox.Text, System.Globalization.NumberStyles.HexNumber);
		}

		private void miniEventUnk3BTextBox_TextChanged(object sender, EventArgs e)
		{
			MiniFloats.PlayerRot.Z = int.Parse(miniEventPlayerZRotTextBox.Text, System.Globalization.NumberStyles.HexNumber);
		}

		private void miniEventUnk3CTextBox_TextChanged(object sender, EventArgs e)
		{
			MiniFloats.CameraYRot = int.Parse(miniEventCameraYRotTextBox.Text, System.Globalization.NumberStyles.HexNumber);
		}
		#endregion
		#endregion
		private void recentFilesToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			LoadFile(recentFiles[recentFilesToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem)]);
		}
	}
	#region Effect Data Structs
	public class SubtitleData
	{
		public int SubtitleStart { get; set; }
		public int SubtitleVisibleX { get; set; }

		public static int Size => 8;

		public SubtitleData() { }

		public SubtitleData(byte[] file, int address)
		{
			SubtitleStart = ByteConverter.ToInt32(file, address);
			SubtitleVisibleX = ByteConverter.ToInt32(file, address + 4);
		}
		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>(Size);
			result.AddRange(ByteConverter.GetBytes(SubtitleStart));
			result.AddRange(ByteConverter.GetBytes(SubtitleVisibleX));
			return result.ToArray();
		}
	}
	public class AudioMiscData
	{
		public int AudioStart { get; set; }
		public sbyte SFXInit { get; set; }
		public sbyte CreditsScroll { get; set; }
		public short VoiceID { get; set; }
		public string MusicSetting { get; set; }
		public string JingleSetting { get; set; }
		public int VsyncMode { get; set; }

		public static int Size => 0x48;

		public AudioMiscData() 
		{
			SFXInit = -1;
			CreditsScroll = -1;
			VoiceID = -1;
		}

		public AudioMiscData(byte[] file, int address)
		{
			AudioStart = ByteConverter.ToInt32(file, address);
			SFXInit = (sbyte)file[address + 4];
			CreditsScroll = (sbyte)file[address + 5];
			VoiceID = ByteConverter.ToInt16(file, address + 6);
			MusicSetting = file.GetCString(address + 8);
			JingleSetting = file.GetCString(address + 0x18);
			VsyncMode = ByteConverter.ToInt32(file, address + 0x28);
		}
		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>(Size);
			result.AddRange(ByteConverter.GetBytes(AudioStart));
			result.Add((byte)SFXInit);
			result.Add((byte)CreditsScroll);
			result.AddRange(ByteConverter.GetBytes(VoiceID));
			if (MusicSetting != null)
				result.AddRange(Encoding.ASCII.GetBytes(MusicSetting));
			else
				result.AddRange(new byte[0x10]);
			result.Align(0x18);
			if (JingleSetting != null)
				result.AddRange(Encoding.ASCII.GetBytes(JingleSetting));
			else
				result.AddRange(new byte[0x10]);
			result.Align(0x28);
			result.AddRange(ByteConverter.GetBytes(VsyncMode));
			result.Align(0x48);
			return result.ToArray();
		}
	}
	public class ScreenData
	{
		public int ScreenStart { get; set; }
		public byte ScreenType { get; set; }
		public short Fade { get; set; }
		public byte ScreenA { get; set; }
		public byte ScreenR { get; set; }
		public byte ScreenG { get; set; }
		public byte ScreenB { get; set; }
		public short TexID { get; set; }
		public int VisibleTime { get; set; }
		public short TexPosX { get; set; }
		public short TexPosY { get; set; }
		public float TexWidth { get; set; }
		public float TexHeight { get; set; }

		public static int Size { get { return 0x40; } }

		public ScreenData() { }

		public ScreenData(byte[] file, int address, bool bigEndian)
		{
			ScreenStart = ByteConverter.ToInt32(file, address);
			ScreenType = file[address + 4];
			if (bigEndian)
			{
				ScreenA = file[address + 8];
				ScreenR = file[address + 9];
				ScreenG = file[address + 0xA];
				ScreenB = file[address + 0xB];
			}
			else
			{
				ScreenB = file[address + 8];
				ScreenG = file[address + 9];
				ScreenR = file[address + 0xA];
				ScreenA = file[address + 0xB];
			}
			Fade = ByteConverter.ToInt16(file, address + 0xC);
			TexID = ByteConverter.ToInt16(file, address + 0xE);
			VisibleTime = ByteConverter.ToInt32(file, address + 0x10);
			TexPosX = ByteConverter.ToInt16(file, address + 0x14);
			TexPosY = ByteConverter.ToInt16(file, address + 0x18);
			TexWidth = ByteConverter.ToSingle(file, address + 0x1C);
			TexHeight = ByteConverter.ToSingle(file, address + 0x20);
		}
		public byte[] GetBytes(bool bigEndian)
		{
			List<byte> result = new List<byte>(Size);
			result.AddRange(ByteConverter.GetBytes(ScreenStart));
			result.Add(ScreenType);
			result.AddRange(new byte[3]);
			if (bigEndian)
			{
				result.Add(ScreenA);
				result.Add(ScreenR);
				result.Add(ScreenG);
				result.Add(ScreenB);
			}
			else
			{
				result.Add(ScreenB);
				result.Add(ScreenG);
				result.Add(ScreenR);
				result.Add(ScreenA);
			}
			result.AddRange(ByteConverter.GetBytes(Fade));
			result.AddRange(ByteConverter.GetBytes(TexID));
			result.AddRange(ByteConverter.GetBytes(VisibleTime));
			result.AddRange(ByteConverter.GetBytes(TexPosX));
			result.AddRange(ByteConverter.GetBytes(TexPosY));
			result.AddRange(ByteConverter.GetBytes(TexWidth));
			result.AddRange(ByteConverter.GetBytes(TexHeight));
			result.Align(0x40);
			return result.ToArray();
		}
	}
	public class SingleParticleData
	{
		public int ParticleStart { get; set; }
		public byte ParticleType { get; set; }
		public byte ParticleMotionID { get; set; }
		public float ParticleTexID { get; set; }
		public float PulseTypeX { get; set; }
		public float PulseConstY { get; set; }
		public float ParticleSize { get; set; }
		public static int Size { get { return 0x38; } }
		public SingleParticleData() { }
		public SingleParticleData(byte[] file, int address)
		{
			ParticleStart = ByteConverter.ToInt32(file, address);
			ParticleType = file[address + 4];
			ParticleMotionID = file[address + 5];
			ParticleTexID = ByteConverter.ToSingle(file, address + 8);
			PulseTypeX = ByteConverter.ToSingle(file, address + 0xC);
			PulseConstY = ByteConverter.ToSingle(file, address + 0x10);
			ParticleSize = ByteConverter.ToSingle(file, address + 0x14);
		}
		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>(Size);
			result.AddRange(ByteConverter.GetBytes(ParticleStart));
			result.Add(ParticleType);
			result.Add(ParticleMotionID);
			result.AddRange(new byte[2]);
			result.AddRange(ByteConverter.GetBytes(ParticleTexID));
			result.AddRange(ByteConverter.GetBytes(PulseTypeX));
			result.AddRange(ByteConverter.GetBytes(PulseConstY));
			result.AddRange(ByteConverter.GetBytes(ParticleSize));
			result.Align(0x38);
			return result.ToArray();
		}
	}
	public class LightData
	{
		public int LightStart { get; set; }
		public int FadeType { get; set; }
		public Vertex LightDirection { get; set; }
		public Vertex LightColor { get; set; }
		public float SingleAmbientIntensity { get; set; }
		public Vertex AmbientColor { get; set; }
		public static int Size { get { return 0x44; } }
		public LightData() 
		{
			LightDirection = new Vertex(0, 0, 0);
			LightColor = new Vertex(0, 0, 0);
			AmbientColor = new Vertex(0, 0, 0);
		}
		public LightData(byte[] file, int address)
		{
			LightStart = ByteConverter.ToInt32(file, address);
			FadeType = ByteConverter.ToInt32(file, address + 4);
			LightDirection = new Vertex(file, address + 8);
			LightColor = new Vertex(file, address + 0x14);
			SingleAmbientIntensity = ByteConverter.ToSingle(file, address + 0x20);
			AmbientColor = new Vertex(file, address + 0x24);
		}
		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>(Size);
			result.AddRange(ByteConverter.GetBytes(LightStart));
			result.AddRange(ByteConverter.GetBytes(FadeType));
			result.AddRange(LightDirection.GetBytes());
			result.AddRange(LightColor.GetBytes());
			result.AddRange(ByteConverter.GetBytes(SingleAmbientIntensity));
			result.AddRange(AmbientColor.GetBytes());
			result.Align(0x44);
			return result.ToArray();
		}
	}
	public class BlurData
	{
		public int BlurStart { get; set; }
		public int Duration { get; set; }
		public sbyte BlurModel1 { get; set; }
		public sbyte BlurModel2 { get; set; }
		public sbyte BlurModel3 { get; set; }
		public sbyte BlurModel4 { get; set; }
		public sbyte BlurModel5 { get; set; }
		public sbyte BlurModel6 { get; set; }
		public int Instances { get; set; }
		public static int Size { get { return 0x40; } }
		public BlurData() { }
		public BlurData(byte[] file, int address)
		{ 
			BlurStart = ByteConverter.ToInt32(file, address);
			Duration = ByteConverter.ToInt32(file, address + 4);
			BlurModel1 = (sbyte)file[address + 8];
			BlurModel2 = (sbyte)file[address + 9];
			BlurModel3 = (sbyte)file[address + 0xA];
			BlurModel4 = (sbyte)file[address + 0xB];
			BlurModel5 = (sbyte)file[address + 0xC];
			BlurModel6 = (sbyte)file[address + 0xD];
			Instances = ByteConverter.ToInt32(file, address + 0x10);
		}
		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>(Size);
			result.AddRange(ByteConverter.GetBytes(BlurStart));
			result.AddRange(ByteConverter.GetBytes(Duration));
			result.Add((byte)BlurModel1);
			result.Add((byte)BlurModel2);
			result.Add((byte)BlurModel3);
			result.Add((byte)BlurModel4);
			result.Add((byte)BlurModel5);
			result.Add((byte)BlurModel6);
			result.AddRange(new byte[2]);
			result.AddRange(ByteConverter.GetBytes(Instances));
			result.Align(0x40);
			return result.ToArray();
		}
	}
	public class ParticleGeneratorData
	{
		public Vertex Position { get; set; }
		public Vertex Velocity { get; set; }
		public short AddRotationZ { get; set; }
		public short AddRotationY { get; set; }
		public short AddRotationX { get; set; }
		public short Unk { get; set; }
		public int ParticleGenStart { get; set; }
		public int RotationConst { get; set; }
		public float Spread { get; set; }
		public int YScale { get; set; }
		public int Count { get; set; }
		public int ModelID { get; set; }
		public int ParticleType { get; set; }
		public int FrameDelay { get; set; }

		public static int Size { get { return 0x40; } }
		public ParticleGeneratorData()
		{ 
			Position = new Vertex(0, 0, 0);
			Velocity = new Vertex(0, 0, 0);
		}
		public ParticleGeneratorData(byte[] file, int address)
		{
			Position = new Vertex(file, address);
			Velocity = new Vertex(file, address + 0xC);
			AddRotationZ = ByteConverter.ToInt16(file, address + 0x18);
			AddRotationY = ByteConverter.ToInt16(file, address + 0x1A);
			AddRotationX = ByteConverter.ToInt16(file, address + 0x1C);
			Unk = ByteConverter.ToInt16(file, address + 0x1E);
			ParticleGenStart = ByteConverter.ToInt32(file, address + 0x20);
			RotationConst = ByteConverter.ToInt32(file, address + 0x24);
			Spread = ByteConverter.ToSingle(file, address + 0x28);
			YScale = ByteConverter.ToInt32(file, address + 0x2C);
			Count = ByteConverter.ToInt32(file, address + 0x30);
			ModelID = ByteConverter.ToInt32(file, address + 0x34);
			ParticleType = ByteConverter.ToInt32(file, address + 0x38);
			FrameDelay = ByteConverter.ToInt32(file, address + 0x3C);
		}
		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>(Size);
			result.AddRange(Position.GetBytes());
			result.AddRange(Velocity.GetBytes());
			result.AddRange(ByteConverter.GetBytes(AddRotationZ));
			result.AddRange(ByteConverter.GetBytes(AddRotationY));
			result.AddRange(ByteConverter.GetBytes(AddRotationX));
			result.AddRange(ByteConverter.GetBytes(Unk));
			result.AddRange(ByteConverter.GetBytes(ParticleGenStart));
			result.AddRange(ByteConverter.GetBytes(RotationConst));
			result.AddRange(ByteConverter.GetBytes(Spread));
			result.AddRange(ByteConverter.GetBytes(YScale));
			result.AddRange(ByteConverter.GetBytes(Count));
			result.AddRange(ByteConverter.GetBytes(ModelID));
			result.AddRange(ByteConverter.GetBytes(ParticleType));
			result.AddRange(ByteConverter.GetBytes(FrameDelay));
			result.Align(0x40);
			return result.ToArray();
		}

	}
	public class VideoData
	{
		public int VideoStart { get; set; }
		public short VideoPosX {  get; set; }
		public short VideoPosY { get; set; }
		public float VideoDepth { get; set; }
		public byte VideoOverlayType { get; set; }
		public sbyte VideoOverlayTexID { get; set; }
		public string VideoName { get; set; }
		public static int Size { get { return 0x40; } }
		public VideoData() { }
		public VideoData(byte[] file, int address)
		{
			VideoStart = ByteConverter.ToInt32(file, address);
			VideoPosX = ByteConverter.ToInt16(file, address + 4);
			VideoPosY = ByteConverter.ToInt16(file, address + 6);
			VideoDepth = ByteConverter.ToSingle(file, address + 8);
			VideoOverlayType = file[address + 0xC];
			VideoOverlayTexID = (sbyte)file[address + 0xD];
			VideoName = file.GetCString(address + 0x10);
		}
		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>(Size);
			result.AddRange(ByteConverter.GetBytes(VideoStart));
			result.AddRange(ByteConverter.GetBytes(VideoPosX));
			result.AddRange(ByteConverter.GetBytes(VideoPosY));
			result.AddRange(ByteConverter.GetBytes(VideoDepth));
			result.Add(VideoOverlayType);
			result.Add((byte)VideoOverlayTexID);
			result.AddRange(new byte[2]);
			// Some files are cute and have leftover text in them. This ensures the strings aren't too long.
			if (VideoName != null)
			{
				if (VideoName.Length < 0x30)
				{
					result.AddRange(Encoding.ASCII.GetBytes(VideoName));
				}
				else
				{
					result.AddRange(Encoding.ASCII.GetBytes(VideoName.Remove(0x30)));
				}
			}
			else
				result.AddRange(new byte[0x30]);
			result.Align(0x40);
			return result.ToArray();
		}
	}
	public class MiniEventEffectData
	{
		public int EffectStart { get; set; }
		public byte ScreenFadeType { get; set; }
		public sbyte SFXEntry1 { get; set; }
		public sbyte SFXEntry2 { get; set; }
		public short VoiceEntry { get; set; }
		public string MusicEntry { get; set; }
		public string JingleEntry { get; set; }
		public int RumblePower { get; set; }
		public static int Size { get { return 0x4C; } }
		public MiniEventEffectData() 
		{
			SFXEntry1 = -1;
			SFXEntry2 = -1;
			VoiceEntry = -1;
		}
		public MiniEventEffectData(byte[] file, int address)
		{
			EffectStart = ByteConverter.ToInt32(file, address);
			ScreenFadeType = file[address + 4];
			SFXEntry1 = (sbyte)file[address + 5];
			SFXEntry2 = (sbyte)file[address + 6];
			VoiceEntry = ByteConverter.ToInt16(file, address + 8);
			MusicEntry = file.GetCString(address + 0xA);
			JingleEntry = file.GetCString(address + 0x1A);
			RumblePower = ByteConverter.ToInt32(file, address + 0x2C);
		}
		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>(Size);
			result.AddRange(ByteConverter.GetBytes(EffectStart));
			result.Add(ScreenFadeType);
			result.Add((byte)SFXEntry1);
			result.Add((byte)SFXEntry2);
			result.AddRange(new byte[1]);
			result.AddRange(ByteConverter.GetBytes(VoiceEntry));
			if (MusicEntry != null)
				result.AddRange(Encoding.ASCII.GetBytes(MusicEntry));
			else
				result.AddRange(new byte[0x10]);
			result.Align(0x1A);
			if (JingleEntry != null)
				result.AddRange(Encoding.ASCII.GetBytes(JingleEntry));
			else
				result.AddRange(new byte[0x10]);
			result.Align(0x2A);
			result.AddRange(new byte[2]);
			result.AddRange(ByteConverter.GetBytes(RumblePower));
			result.Align(0x4C);
			return result.ToArray();
		}
	}
	public class MiniEventEffectFloats
	{
		public Vertex CameraPos { get; set; }
		public Rotation PlayerRot { get; set; }
		public int CameraYRot { get; set; }

		public static int Size { get { return 0x1C; } }
		public MiniEventEffectFloats() 
		{ 
			CameraPos = new Vertex(0, 0, 0);
			PlayerRot = new Rotation(0, 0, 0);
		}
		public MiniEventEffectFloats(byte[] file, int address)
		{
			CameraPos = new Vertex(file, address);
			PlayerRot = new Rotation(file, address + 0xC);
			CameraYRot = ByteConverter.ToInt32(file, address + 0x18);
		}

		public byte[] GetBytes()
		{
			List<byte> result = new List<byte>(Size);
			result.AddRange(CameraPos.GetBytes());
			result.AddRange(PlayerRot.GetBytes());
			result.AddRange(ByteConverter.GetBytes(CameraYRot));
			result.Align(0x1C);
			return result.ToArray();
		}
	}
	#endregion
}
