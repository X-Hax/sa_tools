using SplitTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


namespace SAModel.SAEditorCommon
{
	/// <summary>
	/// User settings stored in every program's INI file. Editor-specific settings are in derived classes.
	/// </summary>
	public class SettingsFile
	{
		[DefaultValue(10000.0f)]
		public float DrawDistanceGeneral { get; set; }

		[DefaultValue(1)]
		public int CameraModifier { get; set; }

		[DefaultValue(false)]
		public bool AlternativeCamera { get; set; }

		[DefaultValue(typeof(Color), "Black")]
		public Color BackgroundColor { get; set; }

		[DefaultValue(false)]
		public bool MouseWrapScreen { get; set; }

		[DefaultValue(true)]
		public bool ShowWelcomeScreen { get; set; }

		#region Lighting
		[DefaultValue(false)]
		public bool EnableSpecular { get; set; }

		// Key Light
		[DefaultValue(typeof(Color), "0xB4ACAC")]
		public Color KeyLightDiffuse { get; set; }

		[DefaultValue(typeof(Color), "Black")]
		public Color KeyLightAmbient { get; set; }

		[DefaultValue(typeof(Color), "White")]
		public Color KeyLightSpecular { get; set; }

		[DefaultValue(typeof(Vertex), "-0.245, -1, 0.125")]
		public Vertex KeyLightDirection { get; set; }

		// Fill Light
		[DefaultValue(typeof(Color), "0x848484")]
		public Color FillLightDiffuse { get; set; }

		[DefaultValue(typeof(Color), "Black")]
		public Color FillLightAmbient { get; set; }

		[DefaultValue(typeof(Color), "0x7F7F7F")]
		public Color FillLightSpecular { get; set; }

		[DefaultValue(typeof(Vertex), "0.245, -0.4, -0.125")]
		public Vertex FillLightDirection { get; set; }

		// Back Light
		[DefaultValue(typeof(Color), "0x828E82")]
		public Color BackLightDiffuse { get; set; }

		[DefaultValue(typeof(Color), "0x282828")]
		public Color BackLightAmbient { get; set; }

		[DefaultValue(typeof(Color), "Black")]
		public Color BackLightSpecular { get; set; }

		[DefaultValue(typeof(Vertex), "-0.45, 1, 0.25")]
		public Vertex BackLightDirection { get; set; }
		#endregion

		public static string GetSettingsPath()
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SA Tools", Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ".ini");
		}

		public void Save()
		{
			string path = GetSettingsPath();
			if (!Directory.Exists(Path.GetDirectoryName(path)))
				Directory.CreateDirectory(Path.GetDirectoryName(path));
			IniSerializer.Serialize(this, path);
		}

		public class Settings_SALVL : SettingsFile
		{
			[DefaultValue(true)]
			public bool DisableModelLibrary { get; set; }

			[DefaultValue(0)]
			public int LibrarySplitterPosition { get; set; }

			[DefaultValue(0)]
			public int ItemsSplitterPosition { get; set; }

			[DefaultValue(0)]
			public int PropertiesSplitterPosition { get; set; }

			[DefaultValue(6000.0f)]
			public float DrawDistanceSET { get; set; }

			[DefaultValue(6000.0f)]
			public float DrawDistanceGeometry { get; set; }

			public Settings_SALVL()
			{
				ShowWelcomeScreen = true;
				DisableModelLibrary = true;
				LibrarySplitterPosition = 0;
				ItemsSplitterPosition = 0;
				PropertiesSplitterPosition = 0;
				DrawDistanceGeneral = 10000;
				DrawDistanceSET = 6000;
				DrawDistanceGeometry = 6000;
				CameraModifier = 1;
				AlternativeCamera = false;
				MouseWrapScreen = false;
				BackgroundColor = Color.Black;
				EnableSpecular = false;

				KeyLightDiffuse = Color.FromArgb(255, 180, 172, 172);
				KeyLightAmbient = Color.Black;
				KeyLightSpecular = Color.White;
				KeyLightDirection = new Vertex(-0.245f, -1, 0.125f);

				FillLightDiffuse = Color.FromArgb(255, 132, 132, 132);
				FillLightAmbient = Color.Black;
				FillLightSpecular = Color.FromArgb(255, 127, 127, 127);
				FillLightDirection = new Vertex(0.245f, -0.4f, -0.125f);

				BackLightDiffuse = Color.FromArgb(255, 180, 172, 172);
				BackLightAmbient = Color.FromArgb(255, 40, 40, 40);
				BackLightSpecular = Color.FromArgb(255, 127, 127, 127);
				BackLightDirection = new Vertex(-0.45f, 1f, 0.25f);
			}

			public static Settings_SALVL Load()
			{
				string path = GetSettingsPath();
				if (File.Exists(path))
					return IniSerializer.Deserialize<Settings_SALVL>(path);
				else
					return new Settings_SALVL();
			}
		}

		public class Settings_SAMDL : SettingsFile
		{
			[DefaultValue(1.125f)]
			public float CamMoveSpeed { get; set; }

			[DefaultValue(1.0f)]
			public float AnimSpeed { get; set; }

			[DefaultValue(false)]
			public bool NjbSizeLittleEndian { get; set; }

			[DefaultValue(false)]
			public bool ExportNJTL { get; set; }

			public Settings_SAMDL()
			{
				ShowWelcomeScreen = true;
				CamMoveSpeed = 1.125f;
				AnimSpeed = 1.0f;
				DrawDistanceGeneral = 10000.0f;
				CameraModifier = 1;
				AlternativeCamera = false;
				BackgroundColor = Color.Black;
				EnableSpecular = false;
				NjbSizeLittleEndian = false;
				ExportNJTL = false;
				KeyLightDiffuse = Color.FromArgb(255, 180, 172, 172);
				KeyLightAmbient = Color.Black;
				KeyLightSpecular = Color.White;
				KeyLightDirection = new Vertex(-0.245f, -1, 0.125f);

				FillLightDiffuse = Color.FromArgb(255, 132, 132, 132);
				FillLightAmbient = Color.Black;
				FillLightSpecular = Color.FromArgb(255, 127, 127, 127);
				FillLightDirection = new Vertex(0.245f, -0.4f, -0.125f);

				BackLightDiffuse = Color.FromArgb(255, 180, 172, 172);
				BackLightAmbient = Color.FromArgb(255, 40, 40, 40);
				BackLightSpecular = Color.FromArgb(255, 127, 127, 127);
				BackLightDirection = new Vertex(-0.45f, 1f, 0.25f);
			}

			public static Settings_SAMDL Load()
			{
				string path = GetSettingsPath();
				if (File.Exists(path))
					return IniSerializer.Deserialize<Settings_SAMDL>(path);
				else
					return new Settings_SAMDL();
			}
		}

		public class Settings_SA2EventViewer : SettingsFile
		{
			[DefaultValue(55000.0f)]
			public new float DrawDistanceGeneral { get; set; }

			public Settings_SA2EventViewer()
			{
				DrawDistanceGeneral = 55000.0f;
				CameraModifier = 1;
				BackgroundColor = Color.Black;
				EnableSpecular = false;

				KeyLightDiffuse = Color.FromArgb(255, 180, 172, 172);
				KeyLightAmbient = Color.Black;
				KeyLightSpecular = Color.White;
				KeyLightDirection = new Vertex(-0.245f, -1, 0.125f);

				FillLightDiffuse = Color.FromArgb(255, 132, 132, 132);
				FillLightAmbient = Color.Black;
				FillLightSpecular = Color.FromArgb(255, 127, 127, 127);
				FillLightDirection = new Vertex(0.245f, -0.4f, -0.125f);

				BackLightDiffuse = Color.FromArgb(255, 180, 172, 172);
				BackLightAmbient = Color.FromArgb(255, 40, 40, 40);
				BackLightSpecular = Color.FromArgb(255, 127, 127, 127);
				BackLightDirection = new Vertex(-0.45f, 1f, 0.25f);
			}

			public static Settings_SA2EventViewer Load()
			{
				string path = GetSettingsPath();
				if (File.Exists(path))
					return IniSerializer.Deserialize<Settings_SA2EventViewer>(path);
				else
					return new Settings_SA2EventViewer();
			}
		}

		public class Settings_TextureEditor : SettingsFile
		{
			[DefaultValue(false)]
			public bool HighQualityGVM { get; set; }
			[DefaultValue(true)]
			public bool SACompatiblePalettes { get; set; }
			[DefaultValue(true)]
			public bool EnableFiltering { get; set; }
			[DefaultValue(false)]
			public bool UsePNGforPAK { get; set; }

			public Settings_TextureEditor()
			{
				HighQualityGVM = false;
				SACompatiblePalettes = true;
				EnableFiltering = true;
				UsePNGforPAK = false;
			}

			public static Settings_TextureEditor Load()
			{
				string path = GetSettingsPath();
				if (File.Exists(path))
					return IniSerializer.Deserialize<Settings_TextureEditor>(path);
				else
					return new Settings_TextureEditor();
			}
		}

		public class Settings_SA2CutsceneTextEditor : SettingsFile
		{
			[DefaultValue(false)]
			public bool BigEndian { get; set; }
			[DefaultValue(false)]
			public bool UseSJIS { get; set; }
			[IniCollection(IniCollectionMode.SingleLine, Format = ",")]
			public List<string> RecentFiles { get; set; } = new List<string>();

			public Settings_SA2CutsceneTextEditor()
			{
				BigEndian = false;
				UseSJIS = false;
				RecentFiles = new List<string>();
			}

			public static Settings_SA2CutsceneTextEditor Load()
			{
				string path = GetSettingsPath();
				if (File.Exists(path))
					return IniSerializer.Deserialize<Settings_SA2CutsceneTextEditor>(path);
				else
					return new Settings_SA2CutsceneTextEditor();
			}
		}

		public class Settings_SA2MessageFileEditor : SettingsFile
		{
			[DefaultValue(false)]
			public bool BigEndian { get; set; }
			[DefaultValue(false)]
			public bool UseSJIS { get; set; }
			[IniCollection(IniCollectionMode.SingleLine, Format = ",")]
			public List<string> RecentFiles { get; set; } = new List<string>();

			public Settings_SA2MessageFileEditor()
			{
				BigEndian = false;
				UseSJIS = false;
				RecentFiles = new List<string>();
			}

			public static Settings_SA2MessageFileEditor Load()
			{
				string path = GetSettingsPath();
				if (File.Exists(path))
					return IniSerializer.Deserialize<Settings_SA2MessageFileEditor>(path);
				else
					return new Settings_SA2MessageFileEditor();
			}
		}
	}
}