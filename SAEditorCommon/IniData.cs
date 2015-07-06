using System.Collections.Generic;
using System.ComponentModel;
using SA_Tools;

namespace SonicRetro.SAModel.SAEditorCommon
{
	public class IniData
	{
		public static IniData Load(string filename)
		{
			return IniSerializer.Deserialize<IniData>(filename);
		}

		public void Save(string filename)
		{
			IniSerializer.Serialize(this, filename);
		}

		public string SystemPath { get; set; }
		public string ObjectDefinitions { get; set; }
		public string ObjectTextureList { get; set; }
		public string LevelTextureLists { get; set; }
		public string MissionObjectList { get; set; }
		public string MissionTextureList { get; set; }
		public string Paths { get; set; }
		[IniName("Chars_")]
		[IniCollection(IniCollectionMode.NoSquareBrackets)]
		public Dictionary<string, IniCharInfo> Characters { get; set; }
		[IniCollection(IniCollectionMode.IndexOnly)]
		public Dictionary<string, IniLevelData> Levels { get; set; }
	}

	public class IniCharInfo
	{
		public string Model { get; set; }
		public string Textures { get; set; }
		public string TextureList { get; set; }
		public float Height { get; set; }
		public string StartPositions { get; set; }
	}

	public class IniLevelData
	{
		public string LevelGeometry { get; set; }
		[DefaultValue("0000")]
		public string LevelID { get; set; }
		public string SETName { get; set; }
		[IniCollection(IniCollectionMode.SingleLine, Format = ",")]
		public string[] Textures { get; set; }
		public string ObjectList { get; set; }
		public string ObjectTextureList { get; set; }
		public string DeathZones { get; set; }
		public string Effects { get; set; }
	}
}
