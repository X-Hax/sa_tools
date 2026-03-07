using System.Collections.Generic;
using TextureLib;

// Global variables and parameters used by Texture Editor

namespace TextureEditor
{
	/// <summary>Archive file types handled by Texture Editor.</summary>
	public enum TextureArchiveFormat { PVM, GVM, PVMX, PAK, XVM }

	static partial class Program
	{
		/// <summary>User settings stored in the INI file. These should be checked when performing various operations.</summary>
		public static SAModel.SAEditorCommon.SettingsFile.Settings_TextureEditor settingsfile;
		/// <summary>Set to true to make the program ask to save unsaved changes before quitting.</summary>
		public static bool unsaved;
		/// <summary>Currently selected texture, only used for refreshing the UI.</summary>
		public static int currentTextureID = -1;
		/// <summary>Currently opened archive format. PVM, GVM, PAK etc.</summary>
		public static TextureArchiveFormat currentFormat;
		/// <summary>Current name of the PVM, GVM, PAK etc.</summary>
		public static string archiveFilename;
		/// <summary>List of loaded textures.</summary>
		public static List<GenericTexture> textures = [];
		/// <summary>True when UI updates are suppressed.</summary>
		public static bool suppress = false;
		/// <summary>True when an SOC PAK without an INF file is open.</summary>
		public static bool usingSocPak = false;
		/// <summary>Currently loaded texture palette.</summary>
		public static TexturePalette currentPalette = TexturePalette.CreateDefaultPalette(true);
		/// <summary>Current palette bank ID for Chao textures.</summary>
		public static int paletteSet;
	}
}