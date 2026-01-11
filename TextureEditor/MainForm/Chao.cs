using ArchiveLib;
using System.IO;
using TextureLib;
using static SAModel.SAEditorCommon.ChaoPalettes;
using static TextureEditor.Program;

namespace TextureEditor
{
	public partial class MainForm
	{
		private bool SetChaoPalette(GenericTexture tex)
		{
			string texName = tex.Name.ToLowerInvariant();
			if (CheckIfTextureIsChaoPalettedTexture(texName))
			{
				var result = GetChaoPaletteAndBankFromTextureName(texName, settingsfile.ChaoAlignment, settingsfile.ChaoSecondEvolution);
				string palext = ".pvp";
				if (tex is GvrTexture)
					palext = ".gvp";
				string palname = result.Key;
				int palbank = result.Value;
				if (!string.IsNullOrEmpty(palname))
				{
					string folder = !string.IsNullOrEmpty(archiveFilename) ? Path.GetDirectoryName(archiveFilename) + "\\" : "";
					string pvppath = folder + palname + palext;
					if (File.Exists(pvppath))
					{
						currentPalette = new TexturePalette(File.ReadAllBytes(pvppath));
						paletteSet = palbank;
						toolStripStatusLabelPalette.Text = "Palette loaded from " + palname + palext;
						return true;
					}
				}
			}
			return false;
		}

		private bool CheckIfArchiveHasPalettedChaoTextures(GenericArchive arc)
		{
			foreach (GenericArchive.GenericArchiveEntry file in arc.Entries)
			{
				if (CheckIfTextureIsChaoPalettedTexture(Path.GetFileNameWithoutExtension(file.Name).ToLowerInvariant()))
					return true;
			}
			return false;
		}
	}
}