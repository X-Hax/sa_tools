using System.Collections.Generic;
using System.IO;

namespace SAModel.SAEditorCommon
{
	public class LanternFilenames
	{
        public class LanternLevelEntry
        {
            public string FileID;
            public string Level;
            public string Act;

            public LanternLevelEntry(string file = "", string level = "", string act = "")
            {
                FileID = file;
                Level = level;
                Act = act;
            }
        }

        static List<LanternLevelEntry> LevelList;

        public static string GetLevelNameFromFilename(string name)
        {
            if (LevelList == null)
                BuildLevelList();
            string originalName = Path.GetFileNameWithoutExtension(name);
			if (originalName.Length < 3)
				return "Unknown file";
            string trimmedName = originalName.Substring(2, originalName.Length - 2);
            string result = "";
            foreach (var item in LevelList)
            {
                if (item.FileID == trimmedName)
                    result=string.Join(" ", item.Level, item.Act != "" ? (" / " + item.Act) : "");
            }
            return result;
        }

        public static List<LanternLevelEntry> GetLevelList()
        {
            if (LevelList == null)
                BuildLevelList();
            return LevelList;
        }

        public static void BuildLevelList()
        {
            LevelList = new List<LanternLevelEntry>();

            LevelList.Add(new LanternLevelEntry("_10B", "Emerald Coast / Egg Carrier (sunk)"));
            LevelList.Add(new LanternLevelEntry("_20B", "Windy Valley", "Act 1 Windy Hill"));
            LevelList.Add(new LanternLevelEntry("_21B", "Windy Valley", "Act 2 Tornado"));
            LevelList.Add(new LanternLevelEntry("_22B", "Windy Valley", "Act 3 The Air"));
            LevelList.Add(new LanternLevelEntry("_30B", "Twinkle Park", "Act 1 Twinkle Kart"));
            LevelList.Add(new LanternLevelEntry("_31B", "Twinkle Park", "Act 2 Pleasure Castle"));
            LevelList.Add(new LanternLevelEntry("_32B", "Twinkle Park", "Act 3 Mirror Room"));
            LevelList.Add(new LanternLevelEntry("_40B", "Speed Highway", "Act 1"));
            LevelList.Add(new LanternLevelEntry("_41B", "Speed Highway", "Act 2 Goin' Down"));
            LevelList.Add(new LanternLevelEntry("_42B", "Speed Highway", "Act 3 At Dawn"));
            LevelList.Add(new LanternLevelEntry("_50B", "Red Mountain", "Act 1 Mt. Red"));
            LevelList.Add(new LanternLevelEntry("_51B", "Red Mountain", "Act 2 Red Hot Skull"));
            LevelList.Add(new LanternLevelEntry("_52B", "Red Mountain", "Act 3 (Knuckles)"));
            LevelList.Add(new LanternLevelEntry("_60B", "Sky Deck", "Light / Character Select"));
            LevelList.Add(new LanternLevelEntry("_61B", "Sky Deck", "Dark"));
            LevelList.Add(new LanternLevelEntry("_70B", "Lost World", "Act 1 Tricky Maze"));
            LevelList.Add(new LanternLevelEntry("_71B", "Lost World", "Act 2/3 Danger! Chased by Rock"));
            LevelList.Add(new LanternLevelEntry("_80B", "Ice Cap", "Act 1 Snowy Mountain"));
            LevelList.Add(new LanternLevelEntry("_81B", "Ice Cap", "Act 2 Limestone Cave"));
            LevelList.Add(new LanternLevelEntry("_82B", "Ice Cap", "Act 3 Snowboard"));
            LevelList.Add(new LanternLevelEntry("_83B", "Ice Cap", "Act 4 (Big)"));
            LevelList.Add(new LanternLevelEntry("_90B", "Casinopolis", "Act 1 Main Hall"));
            LevelList.Add(new LanternLevelEntry("_91B", "Casinopolis", "Act 2 Sewers"));
            LevelList.Add(new LanternLevelEntry("_92B", "Casinopolis", "Act 3 Sonic Pinball"));
            LevelList.Add(new LanternLevelEntry("_93B", "Casinopolis", "Act 4 NiGHTS Pinball"));
            LevelList.Add(new LanternLevelEntry("_9MB", "Casinopolis", "Sewers alt. (Unused)"));
            LevelList.Add(new LanternLevelEntry("_A0B", "Final Egg", "Act 1 Mechanical Resonance"));
            LevelList.Add(new LanternLevelEntry("_A1B", "Final Egg", "Act 2 Crank the Heat Up"));
            LevelList.Add(new LanternLevelEntry("_A2B", "Final Egg", "Act 3 Gamma's Training Area"));
            LevelList.Add(new LanternLevelEntry("_C0B", "Hot Shelter", "Act 1 Bad Taste Aquarium"));
            LevelList.Add(new LanternLevelEntry("_C1B", "Hot Shelter", "Act 2 Red Barrage Area"));
            LevelList.Add(new LanternLevelEntry("_C2B", "Hot Shelter", "Act 3 Gamma's Hot Shelter"));
            LevelList.Add(new LanternLevelEntry());

            LevelList.Add(new LanternLevelEntry("_F0B", "Bosses", "Chaos 0"));
            LevelList.Add(new LanternLevelEntry("_G0B", "Bosses", "Chaos 2"));
            LevelList.Add(new LanternLevelEntry("_H0B", "Bosses", "Chaos 4"));
            LevelList.Add(new LanternLevelEntry("_I0B", "Bosses", "Chaos 6"));
            LevelList.Add(new LanternLevelEntry("_J0B", "Bosses", "Perfect Chaos Phase 1"));
            LevelList.Add(new LanternLevelEntry("_J1B", "Bosses", "Perfect Chaos Phase 2"));
            LevelList.Add(new LanternLevelEntry("_K0B", "Bosses", "Egg Hornet"));
            LevelList.Add(new LanternLevelEntry("_M0B", "Bosses", "Egg Viper"));
            LevelList.Add(new LanternLevelEntry("_N0B", "Bosses", "ZERO"));
            LevelList.Add(new LanternLevelEntry("_O0B", "Bosses", "E-101"));
            LevelList.Add(new LanternLevelEntry("_P0B", "Bosses", "E-101-R"));
            LevelList.Add(new LanternLevelEntry());

            LevelList.Add(new LanternLevelEntry("_Q1B", "Station Square", "Evening"));
            LevelList.Add(new LanternLevelEntry("_Q3B", "Station Square", "Night + Egg Walker"));
            LevelList.Add(new LanternLevelEntry("_Q4B", "Station Square", "Day"));

            LevelList.Add(new LanternLevelEntry("_T0B", "Egg Carrier Outside", "Airborne"));
            LevelList.Add(new LanternLevelEntry("_T1B", "Egg Carrier Outside", "Unused 1"));
            LevelList.Add(new LanternLevelEntry("_T2B", "Egg Carrier Outside", "Private Room"));
            LevelList.Add(new LanternLevelEntry("_T3B", "Egg Carrier Outside", "Captain's Room"));
            LevelList.Add(new LanternLevelEntry("_T4B", "Egg Carrier Outside", "Unused 2"));
            LevelList.Add(new LanternLevelEntry("_T5B", "Egg Carrier Outside", "Pool"));

            LevelList.Add(new LanternLevelEntry("_W0B", "Egg Carrier Inside", "Ammunition Room"));
            LevelList.Add(new LanternLevelEntry("_W1B", "Egg Carrier Inside", "Bridge"));
            LevelList.Add(new LanternLevelEntry("_W2B", "Egg Carrier Inside", "Hedgehog Hammer Room"));
            LevelList.Add(new LanternLevelEntry("_W3B", "Egg Carrier Inside", "Prison"));
            LevelList.Add(new LanternLevelEntry("_W4B", "Egg Carrier Inside", "Water Reservoir"));
            LevelList.Add(new LanternLevelEntry("_W5B", "Egg Carrier Inside", "Garden Teleporter Room"));

            LevelList.Add(new LanternLevelEntry("_X0B", "Mystic Ruins", "Day"));
            LevelList.Add(new LanternLevelEntry("_X1B", "Mystic Ruins", "Evening"));
            LevelList.Add(new LanternLevelEntry("_X2B", "Mystic Ruins", "Night"));
            LevelList.Add(new LanternLevelEntry("_X3B", "Mystic Ruins", "Base"));
            LevelList.Add(new LanternLevelEntry("_MRD", "Mystic Ruins", "Day alt. (Unused)"));
            LevelList.Add(new LanternLevelEntry("_MRE", "Mystic Ruins", "Evening alt. (Unused)"));
            LevelList.Add(new LanternLevelEntry("_MRN", "Mystic Ruins", "Night alt. (Unused)"));

            LevelList.Add(new LanternLevelEntry("_Y0B", "The Past", "Echidna City"));
            LevelList.Add(new LanternLevelEntry("_Y1B", "The Past", "Master Emerald Altar"));
            LevelList.Add(new LanternLevelEntry("_Y2B", "The Past", "Master Emerald on Fire"));

            LevelList.Add(new LanternLevelEntry("_Z0B", "Minigames", "Twinkle Circuit"));
            LevelList.Add(new LanternLevelEntry("1A0B", "Minigames", "Sky Chase Part 1"));
            LevelList.Add(new LanternLevelEntry("1B0B", "Minigames", "Sky Chase Part 2"));
            LevelList.Add(new LanternLevelEntry("1C0B", "Minigames", "Sand Hill"));
            LevelList.Add(new LanternLevelEntry());

            LevelList.Add(new LanternLevelEntry("1D0B", "Station Square Garden", "Day"));
            LevelList.Add(new LanternLevelEntry("1D1B", "Station Square Garden", "Unused"));
            LevelList.Add(new LanternLevelEntry("1E0B", "Egg Carrier Garden", "Day"));
            LevelList.Add(new LanternLevelEntry("1E1B", "Egg Carrier Garden", "Evening (Unused)"));
            LevelList.Add(new LanternLevelEntry("1E2B", "Egg Carrier Garden", "Night (Unused)"));
            LevelList.Add(new LanternLevelEntry("1F0B", "Mystic Ruins Garden", "Day"));
            LevelList.Add(new LanternLevelEntry("1F1B", "Mystic Ruins Garden", "Evening (Unused)"));
            LevelList.Add(new LanternLevelEntry("1F2B", "Mystic Ruins Garden", "Night (Unused)"));
            LevelList.Add(new LanternLevelEntry("1G0B", "Chao Stadium"));
            LevelList.Add(new LanternLevelEntry("1G1B", "Chao Race"));
        }
    }
}
