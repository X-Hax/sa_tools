using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLTool
{
	class LanternFilenames
	{
        static Dictionary<string, string> LevelList;

        public static string GetLevelNameFromFilename(string name)
        {
            if (LevelList == null)
                BuildLevelList();
            string originalName = Path.GetFileNameWithoutExtension(name);
            string trimmedName = originalName.Substring(2, originalName.Length - 2);
            foreach (var item in LevelList)
            {
                if (item.Key == trimmedName)
                    return item.Value;
            }
            return "";
        }

        public static void BuildLevelList()
        {
            LevelList = new Dictionary<string, string>();

            LevelList.Add("_10B", "Emerald Coast + Egg Carrier (sunk)");

            LevelList.Add("_20B", "Windy Valley / Act 1 Windy Hill");
            LevelList.Add("_21B", "Windy Valley / Act 1 Tornado");
            LevelList.Add("_22B", "Windy Valley / Act 1 The Air");

            LevelList.Add("_30B", "Twinkle Park / Act 1 Twinkle Kart");
            LevelList.Add("_31B", "Twinkle Park / Act 2 Pleasure Castle");
            LevelList.Add("_32B", "Twinkle Park / Act 3 Mirror Room");

            LevelList.Add("_40B", "Speed Highway / Act 1");
            LevelList.Add("_41B", "Speed Highway / Act 2 Goin' Down");
            LevelList.Add("_42B", "Speed Highway / Act 3 At Down");

            LevelList.Add("_50B", "Red Mountain / Act 1 Mt. Red");
            LevelList.Add("_51B", "Red Mountain / Act 2 Red Hot Skull");
            LevelList.Add("_52B", "Red Mountain / Act 3 (Knuckles)");

            LevelList.Add("_60B", "Sky Deck (Light) + Character Select");
            LevelList.Add("_61B", "Sky Deck (Dark)");

            LevelList.Add("_70B", "Lost World / Act 1 Tricky Maze");
            LevelList.Add("_71B", "Lost World / Act 2/3 Danger! Chased by Rock");

            LevelList.Add("_80B", "Ice Cap / Act 1 Snowy Mountain");
            LevelList.Add("_81B", "Ice Cap / Act 2 Limestone Cave");
            LevelList.Add("_82B", "Ice Cap / Act 3 Snowboard");
            LevelList.Add("_83B", "Ice Cap / Act 4 (Big)");

            LevelList.Add("_90B", "Casinopolis / Act 1 Main Hall");
            LevelList.Add("_91B", "Casinopolis / Act 2 Sewers");
            LevelList.Add("_92B", "Casinopolis / Act 3 Sonic Pinball");
            LevelList.Add("_93B", "Casinopolis / Act 4 NiGHTS Pinball");

            LevelList.Add("_A0B", "Final Egg / Act 1 Mechanical Resonance");
            LevelList.Add("_A1B", "Final Egg / Act 2 Crank the Heat Up");
            LevelList.Add("_A2B", "Final Egg / Act 3 Gamma's Training Area");

            LevelList.Add("_C0B", "Hot Shelter / Act 1 Bad Taste Aquarium");
            LevelList.Add("_C1B", "Hot Shelter / Act 2 Red Barrage Area");
            LevelList.Add("_C2B", "Hot Shelter / Act 3 Gamma's Hot Shelter");

            LevelList.Add("_F0B", "Chaos 0");
            LevelList.Add("_G0B", "Chaos 2");
            LevelList.Add("_H0B", "Chaos 4");
            LevelList.Add("_I0B", "Chaos 6");
            LevelList.Add("_J0B", "Perfect Chaos / Act 1");
            LevelList.Add("_J1B", "Perfect Chaos / Act 2");
            LevelList.Add("_K0B", "Egg Hornet");
            LevelList.Add("_M0B", "Egg Viper");
            LevelList.Add("_N0B", "ZERO");
            LevelList.Add("_O0B", "E-101");
            LevelList.Add("_P0B", "E-101-R");

            LevelList.Add("_Q1B", "Station Square (Evening)");
            LevelList.Add("_Q3B", "Station Square (Night) + Egg Walker");
            LevelList.Add("_Q4B", "Station Square (Day)");

            LevelList.Add("_T0B", "Egg Carrier Outside (Airborne)");
            LevelList.Add("_T1B", "Egg Carrier Outside (Unused)");
            LevelList.Add("_T2B", "Egg Carrier Private Room");
            LevelList.Add("_T3B", "Egg Carrier Captain's Room");
            LevelList.Add("_T4B", "Egg Carrier Outsude (Unused)");
            LevelList.Add("_T5B", "Egg Carrier Pool");

            LevelList.Add("_W0B", "Egg Carrier Ammunition Room");
            LevelList.Add("_W1B", "Egg Carrier Bridge");
            LevelList.Add("_W2B", "Egg Carrier Hedgehog Hammer Room");
            LevelList.Add("_W3B", "Egg Carrier Prison");
            LevelList.Add("_W4B", "Egg Carrier Water Reservoir");
            LevelList.Add("_W5B", "Egg Carrier Garden Teleporter Room");

            LevelList.Add("_X0B", "Mystic Ruins (Day)");
            LevelList.Add("_X1B", "Mystic Ruins (Evening)");
            LevelList.Add("_X2B", "Mystic Ruins (Night)");
            LevelList.Add("_X3B", "Mystic Ruins Base");

            LevelList.Add("_Y0B", "The Past / Echidna City");
            LevelList.Add("_Y1B", "The Past / Master Emerald Altar");
            LevelList.Add("_Y2B", "The Past / Master Emerald on Fire");

            LevelList.Add("_Z0B", "Twinkle Circuit");
            LevelList.Add("1A0B", "Sky Chase / Act 1");
            LevelList.Add("1B0B", "Sky Chase / Act 2");
            LevelList.Add("1C0B", "Sand Hill");

            LevelList.Add("1D0B", "Station Square Garden (Day)");
            LevelList.Add("1D1B", "Station Square Garden (Unused)");
            LevelList.Add("1E0B", "Egg Carrier Garden (Day)");
            LevelList.Add("1E1B", "Egg Carrier Garden (Unused)");
            LevelList.Add("1E2B", "Egg Carrier Garden (Unused)");
            LevelList.Add("1F0B", "Mystic Ruins Garden (Day)");
            LevelList.Add("1F1B", "Mystic Ruins Garden (Evening / Unused)");
            LevelList.Add("1F2B", "Mystic Ruins Garden (Night / Unused)");
            LevelList.Add("1G0B", "Chao Stadium");
            LevelList.Add("1G1B", "Chao Race");

            LevelList.Add("_9MB", "Casinopolis Sewers (Unused)");
            LevelList.Add("_MRD", "Mystic Ruins Day alt. (Unused)");
            LevelList.Add("_MRE", "Mystic Ruins Evening alt. (Unused)");
            LevelList.Add("_MRN", "Mystic Ruins Night alt. (Unused)");
        }
    }
}
