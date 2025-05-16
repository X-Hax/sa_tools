using System.Collections.Generic;

// Stuff for paletted Chao textures, will be moved over to project support eventually.

namespace TextureEditor
{
	public static class ChaoStuff
	{
		public static Dictionary<string, string> ChaoTypeLetter = new Dictionary<string, string>()
		{
			{  "Z", "Child" },
			{  "N", "Neutral" },
			{  "S", "Swim" },
			{  "F", "Fly" },
			{  "R", "Run" },
			{  "P", "Power" },
		};

		public static List<string> ChaoPalettedTextures = new List<string>()
		{
			"al_child02",
			"al_child03",
			"al_child04",
			
			"al_hn00",
			"al_hn01",
			"al_hn02",
			
			"al_hs00",
			"al_hs01",
			"al_hs02",
			
			"al_hf00",
			"al_hf01",
			"al_hf02",

			"al_hr00",
			"al_hr01",
			"al_hr02",

			"al_hp00",
			"al_hp01",
			"al_hp02",
		};
	}
}
