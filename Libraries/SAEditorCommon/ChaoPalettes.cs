using System.Collections.Generic;
using System.Text;

// Functionality to retrieve palette filenames and bank IDs for palettized Chao textures.

namespace SAModel.SAEditorCommon
{
	public static class ChaoPalettes
	{
		public enum ChaoEvolution { Zero, Normal, Swim, Fly, Run, Power }

		public enum ChaoAlignment { Neutral, Hero, Dark }

		private static readonly List<string> ChaoPalettedTextures =
		[
			// Child (Neutral, Hero and Dark)
			"al_child02", // Bank 0
			"al_child03", // Bank 1
			"al_child04", // Bank 2
			// Hero Normal
			"al_hn00",  // Bank 0
			"al_hn01", // Bank 1
			"al_hn02", // Bank 2
			// Hero Swim
			"al_hs00", // Bank 0
			"al_hs01", // Bank 1
			"al_hs02", // Bank 2
			// Hero Fly
			"al_hf00", // Bank 0
			"al_hf01", // Bank 1
			"al_hf02", // Bank 2
			// Hero Run
			"al_hr00", // Bank 0
			"al_hr01", // Bank 1
			"al_hr02", // Bank 2
			// Hero Power
			"al_hp00", // Bank 0
			"al_hp01", // Bank 1
			"al_hp02", // Bank 2
		];

		private static string GetChaoEvolutionLetter(ChaoEvolution type)
		{
			return type switch
			{
				ChaoEvolution.Normal => "n",
				ChaoEvolution.Swim => "s",
				ChaoEvolution.Fly => "f",
				ChaoEvolution.Run => "r",
				ChaoEvolution.Power => "p",
				_ => "z"
			};
		}

		private static string GetChaoPaletteName(ChaoAlignment align, ChaoEvolution evolution1, ChaoEvolution evolution2)
		{
			switch (align)
			{
				// Hero Chao
				case ChaoAlignment.Hero:
					StringBuilder sb = new StringBuilder();
					sb.Append("al_h");
					// Hero Child
					if (evolution1 == ChaoEvolution.Zero)
						sb.Append("c");
					// Hero Adult
					else
						sb.Append(GetChaoEvolutionLetter(evolution1));
					// Second evolution
					sb.Append(GetChaoEvolutionLetter(evolution2));
					return sb.ToString();
				// Dark Chao
				case ChaoAlignment.Dark:
					// Child dark Chao need AL_DC, Dark adult Chao don't use palettes
					return "al_dc";
				// Neutral Chao
				case ChaoAlignment.Neutral:
				default:
					// Neutral Child Chao need palettes depending on their second evolution, Neutral adult Chao don't use palettes
					return evolution2 switch
					{
						ChaoEvolution.Zero => "al_nc00",
						ChaoEvolution.Normal => "al_nc01",
						_ => string.Empty,
					};					
			}
		}

		/// <summary>Retrieves a pair of palette filename and bank ID matching the specified Chao texture, alignment and second evolution.</summary>
		/// <param name="textureName">Texture filename without extension, such as "al_hn00".</param>
		/// <param name="align">Chao alignment: Neutral, Hero or Dark.</param>
		/// <param name="evolution2">Chao second evolution: Normal, Swim, Fly, Run, Power or Zero.</param>
		/// <returns>A KeyValuePair of palette filename (lowercase), such as "al_hpf" for Hero Power Fly, and bank ID in the range of 0-2.</returns>
		public static KeyValuePair<string, int> GetChaoPaletteAndBankFromTextureName(string textureName, ChaoAlignment align, ChaoEvolution evolution2)
		{
			return textureName.ToLowerInvariant() switch
			{
				// Child
				"al_child02" => new KeyValuePair<string, int>(GetChaoPaletteName(align, ChaoEvolution.Zero, evolution2), 0),
				"al_child03" => new KeyValuePair<string, int>(GetChaoPaletteName(align, ChaoEvolution.Zero, evolution2), 1),
				"al_child04" => new KeyValuePair<string, int>(GetChaoPaletteName(align, ChaoEvolution.Zero, evolution2), 2),
				// Hero Normal
				"al_hn00" => new KeyValuePair<string, int>(GetChaoPaletteName(ChaoAlignment.Hero, ChaoEvolution.Normal, evolution2), 0),
				"al_hn01" => new KeyValuePair<string, int>(GetChaoPaletteName(ChaoAlignment.Hero, ChaoEvolution.Normal, evolution2), 1),
				"al_hn02" => new KeyValuePair<string, int>(GetChaoPaletteName(ChaoAlignment.Hero, ChaoEvolution.Normal, evolution2), 2),
				// Hero Swim
				"al_hs00" => new KeyValuePair<string, int>(GetChaoPaletteName(ChaoAlignment.Hero, ChaoEvolution.Swim, evolution2), 0),
				"al_hs01" => new KeyValuePair<string, int>(GetChaoPaletteName(ChaoAlignment.Hero, ChaoEvolution.Swim, evolution2), 1),
				"al_hs02" => new KeyValuePair<string, int>(GetChaoPaletteName(ChaoAlignment.Hero, ChaoEvolution.Swim, evolution2), 2),
				// Hero Fly
				"al_hf00" => new KeyValuePair<string, int>(GetChaoPaletteName(ChaoAlignment.Hero, ChaoEvolution.Fly, evolution2), 0),
				"al_hf01" => new KeyValuePair<string, int>(GetChaoPaletteName(ChaoAlignment.Hero, ChaoEvolution.Fly, evolution2), 1),
				"al_hf02" => new KeyValuePair<string, int>(GetChaoPaletteName(ChaoAlignment.Hero, ChaoEvolution.Fly, evolution2), 2),
				// Hero Run
				"al_hr00" => new KeyValuePair<string, int>(GetChaoPaletteName(ChaoAlignment.Hero, ChaoEvolution.Run, evolution2), 0),
				"al_hr01" => new KeyValuePair<string, int>(GetChaoPaletteName(ChaoAlignment.Hero, ChaoEvolution.Run, evolution2), 1),
				"al_hr02" => new KeyValuePair<string, int>(GetChaoPaletteName(ChaoAlignment.Hero, ChaoEvolution.Run, evolution2), 2),
				// Hero Power
				"al_hp00" => new KeyValuePair<string, int>(GetChaoPaletteName(ChaoAlignment.Hero, ChaoEvolution.Power, evolution2), 0),
				"al_hp01" => new KeyValuePair<string, int>(GetChaoPaletteName(ChaoAlignment.Hero, ChaoEvolution.Power, evolution2), 1),
				"al_hp02" => new KeyValuePair<string, int>(GetChaoPaletteName(ChaoAlignment.Hero, ChaoEvolution.Power, evolution2), 2),
				_ => new KeyValuePair<string, int>(string.Empty, 0),
			};
		}

		/// <summary>Checks whether the texture's filename (without extension) is a known Chao paletted texture filename.</summary>
		/// <param name="textureName">Texture name, such as "al_child02".</param>
		/// <returns>True if the texture is a known Chao paletted texture.</returns>
		public static bool CheckIfTextureIsChaoPalettedTexture(string textureName)
		{
			return ChaoPalettedTextures.Contains(textureName.ToLowerInvariant());
		}
	}
}