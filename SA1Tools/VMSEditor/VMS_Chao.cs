using SAModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace VMSEditor
{
    public struct ADV1_AL_IMPLESSION
    {
        public sbyte like;
        public byte meet;
};

    public struct ADV1_AL_IMPLESSION2
    {
        public uint id;
        public byte like;
        public byte meet;
    };

    public class ChaoMemoriesSA1
    {
        public ADV1_AL_IMPLESSION[] player = new ADV1_AL_IMPLESSION[6];
        public ADV1_AL_IMPLESSION2[] chao = new ADV1_AL_IMPLESSION2[32];

        public ChaoMemoriesSA1(byte[] data, int index)
        {
            for (int i = 0; i < 6; i++)
            {
                player[i].like = (sbyte)data[index + i * 2];
                player[i].meet = data[index + i * 2 + 1];
            }
            for (int u = 0; u < 32; u++)
            {
                chao[u].id = BitConverter.ToUInt32(data, index + 12 + u * 6);
                chao[u].like = data[index + 12 + u * 6 + 4];
                chao[u].meet = data[index + 12 + u * 6 + 5];
            }
        }

		public ChaoMemoriesSA1()
		{
			player = new ADV1_AL_IMPLESSION[6];
			chao = new ADV1_AL_IMPLESSION2[32];
		}

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>();
            for (int i = 0; i < 6; i++)
            {
                result.Add((byte)player[i].like);
                result.Add(player[i].meet);
            }
            for (int u = 0; u < 32; u++)
            {
                result.AddRange(BitConverter.GetBytes(chao[u].id));
                result.Add(chao[u].like);
                result.Add(chao[u].meet);
            }
            if (result.Count < 268)
            {
                do
                    result.Add(0);
                while (result.Count < 268);
            }
            return result.ToArray();
        }
    };

    public enum ChaoTypeSA1 : byte
    {
        [Description("Baby Chao")]
        Baby = 0,
        [Description("Normal Chao")]
        Normal = 1,
        [Description("Swim Chao")]
        SwimChao = 2,
        [Description("Fly Chao")]
        FlyChao = 3,
        [Description("Run Chao")]
        RunChao = 4,
        [Description("Power Chao")]
        PowerChao = 5,
        [Description("Chaos Chao")]
        ChaosChao = 6,
        [Description("Chao Egg")]
        ChaoEgg = 7,
        [Description("Empty Slot")]
        EmptySlot = 8
    }

    public enum ChaoLocationSA1 : byte
    {
        [Description("Station Square")]
        StationSquare = 0,
        [Description("Egg Carrier")]
        EggCarrier = 1,
        [Description("Mystic Ruins")]
        MysticRuins = 2
    }

    public enum ChaoFruitsSA1 : byte
    {
        None = 0,
        Lifenut = 1,
        Cherry = 2,
        Lemon = 3,
        Grape = 4,
        Plum = 5,
        Chaonut = 6,
        Hastenut = 7,
        Starnut = 8,
        Lazynut = 9,
        Coconut = 10
    }

    public enum AnimalFlagsSA1 : ushort
    {
        Deer = 0x100,
        Parrot = 0x200,
        Swallow = 0x400,
        Peacock = 0x800,

        Otter = 0x1000,
        Penguin = 0x2000,
        Seal = 0x4000,
        // 0x80 is missing

        Skunk = 0x1,
        Koala = 0x2,
        Mole = 0x4,
        Elephant = 0x8,

        Lion = 0x10,
        Gorilla = 0x20,
        Wallaby = 0x40,
        Rabbit = 0x80
    }

    public enum AnimalPartsSA1 : byte
    {
        Seal = 0,
        Penguin = 0x01,
        Otter = 0x02,
        Peacock = 0x03,
        Swallow = 0x04,
        Parrot = 0x05,
        Deer = 0x06,
        Rabbit = 0x07,
        Wallaby = 0x08,
        Gorilla = 0x09,
        Lion = 0x0A,
        Elephant = 0x0B,
        Mole = 0x0C,
        Koala = 0x0D,
        Skunk = 0x0E,
        EmptyMod = 0x0F,
        None = 0xFF
    }

    public enum ChaoJewelsSA1 : byte
    {
        Pearl = 0x1,
        Amethyst = 0x2,
        Sapphire = 0x4,
        Ruby = 0x8,
        Emerald = 0x10
    }

    public enum ChaoColorFlagsSA1 : byte
    {
        Flag1 = 0x01,
        Black = 0x02,
        Gold = 0x04,
        Silver = 0x08,
        Flag10 = 0x10,
        Flag20 = 0x20,
        Jewel = 0x40,
        Flag80 = 0x80
    }

    public enum ChaoJewelBreedsSA1 : byte
    {
        Amethyst = 0,
        Emerald = 0x1,
        Ruby = 0x2,
        Sapphire = 0x3
    }

    public class VMS_Chao
    {
        // Main
        public ChaoTypeSA1 Type;
        public ChaoLocationSA1 Garden;
        public sbyte Happiness;
        public string Name;
        // Stats
        public ushort Swim;
        public ushort Fly;
        public ushort Run;
        public ushort Power;
        public ushort HP;
        public ushort HP_Max;
        // Inventory
        public ChaoFruitsSA1[] Fruits = new ChaoFruitsSA1[8];
        // Evolution
        public float RunOrPower;
        public float FlyOrSwim;
        public float Magnitude;
        // Condition
        public ushort Affection;
        public ushort LifeLeft;
        public ushort AgingFactor;
        // Abilities
        public AnimalFlagsSA1 AnimalAbilities;
        // Jewels
        public ChaoJewelsSA1 Jewels;
        // Appearance
        public ChaoColorFlagsSA1 Flags;
        public Vertex Position;
        // Whatever
        public uint Age;
        public uint ID;
        // Animal parts
        public AnimalPartsSA1[] AnimalParts = new AnimalPartsSA1[7];
        // Stat points
        public ushort SwimPoints;
        public ushort FlyPoints;
        public ushort RunPoints;
        public ushort PowerPoints;
        // Face
        public sbyte Kindness;
        public sbyte Aggressive;
        public sbyte Curiosity;
        // Other stats
        public byte Charm;
        public byte Breed;
        public byte Sleep;
        public byte Hunger;
        public byte Tedious;
        public byte Tiredness;
        public byte Stress;
        public byte Narrow;
        public byte Pleasure;
        public byte Anger;
        public byte Sorrow;
        public byte Fear;
        public byte Loneliness;
        // Characters
        public ChaoMemoriesSA1 Memories;
        // Other stuff
        public byte Reincarnations;
        public byte Lane;
        public byte IsCPU; // Not stored in the file
        public sbyte Exists;
        public ushort CocoonTimer;
        public byte[] RaceTime = new byte[20];
        public ChaoJewelBreedsSA1 JewelBreed;
        // Verify download?
        public byte Key1;
        public byte Key2;
        public byte Key3;
        public byte Key4;

		private static char[] JapaneseChaoNameCharacters =
	    {
			'ァ', 'ア', 'ィ', 'イ', 'ゥ', 'ウ', 'ェ', 'エ', 'ォ', 'オ',
			'カ', 'ガ', 'キ', 'ギ', 'ク', 'グ', 'ケ', 'ゲ', 'コ', 'ゴ',
			'サ', 'ザ', 'シ', 'ジ', 'ス', 'ズ', 'セ', 'ゼ', 'ソ', 'ゾ',
			'タ', 'ダ', 'チ', 'ヂ', 'ッ', 'ツ', 'ヅ', 'テ', 'デ', 'ト', 'ド',
			'ナ', 'ニ', 'ヌ', 'ネ', 'ノ',
			'ハ', 'バ', 'パ', 'ヒ', 'ビ', 'ピ', 'フ', 'ブ', 'プ', 'ヘ', 'ベ', 'ペ', 'ホ', 'ボ', 'ポ',
			'マ', 'ミ', 'ム', 'メ', 'モ',
			'ャ', 'ヤ', 'ュ', 'ユ', 'ョ', 'ヨ',
			'ラ', 'リ', 'ル', 'レ', 'ロ',
			'ヮ', 'ワ', '〃', '゜', 'を', 'ん'
		};

        private static char[] SpecialChaoNameCharacters =
        {
             '。', '♥', '～', '♀', '♂', '♪', '…', '＼', '〇'
        };

        public static byte[] GetChaoNameBytes(string name)
        {
            List<byte> result = new List<byte>();
            for (int s = 0; s < name.Length; s++)
            {
                bool added = false;
                // Check Japanese characters
                for (int j = 0; j < JapaneseChaoNameCharacters.Length; j++)
                {
                    if (name[s] == JapaneseChaoNameCharacters[j])
                    {
                        result.Add((byte)(0x64 + j));
                        added = true;
                        break;
                    }
                }
                if (added)
                    continue;
                // Check Japanese special characters
                for (int j = 0; j < SpecialChaoNameCharacters.Length; j++)
                {
                    if (name[s] == SpecialChaoNameCharacters[j])
                    {
                        result.Add((byte)(0xB7 + j));
                        added = true;
                        break;
                    }
                }
                if (added)
                    continue;
                result.Add(Encoding.ASCII.GetBytes(name[s].ToString().ToUpperInvariant())[0]);
            }
            // Add extra bytes if the name is less than 8 characters
            if (result.Count < 8)
            {
                do
                    result.Add(0);
                while (result.Count < 8);
            }
            // Truncate the name if it's more than 8 characters
            if (result.Count > 8)
            {
                do
                    result.RemoveAt(result.Count - 1);
                while (result.Count > 8);
            }
            return result.ToArray();
        }

        public static string ReadChaoName(byte[] file, int index)
        {
            StringBuilder sb = new StringBuilder();
            for (int a = 0; a < 8; a++)
            {
				//System.Windows.Forms.MessageBox.Show(file[index + a].ToString("X"));
				switch (file[index + a])
                {
                    case 0:
                        break;
                    case 0x5C:
                        sb.Append("￥");
                        break;
                    default:
                        if (file[index + a] >= 0x64 && file[index + a] < 0xB7)
                        {
                            sb.Append(JapaneseChaoNameCharacters[file[index + a] - 0x64].ToString());
                        }
                        else if (file[index + a] >= 0xB7)
                        {
                            sb.Append(SpecialChaoNameCharacters[file[index + a] - 0xB7].ToString());
                        }
                        else
                            sb.Append((Convert.ToChar(file[index + a])).ToString());
                        break;
                }
            }
            return sb.ToString();
        }

        public VMS_Chao() 
		{
			Name = "";
			Position = new Vertex();
			Memories = new ChaoMemoriesSA1();
			Type = ChaoTypeSA1.EmptySlot;
		}

        public VMS_Chao(byte[] file, int index)
        {
            Type = (ChaoTypeSA1)file[index];
            Garden = (ChaoLocationSA1)file[index + 1];
            Happiness = (sbyte)file[index + 2];
            Key1 = file[index + 3];
            Name = ReadChaoName(file, index + 0x04);
            Swim = BitConverter.ToUInt16(file, index + 0x0C);
            Run = BitConverter.ToUInt16(file, index + 0x0E);
            Fly = BitConverter.ToUInt16(file, index + 0x10);
            Power = BitConverter.ToUInt16(file, index + 0x12);
            HP = BitConverter.ToUInt16(file, index + 0x14);
            HP_Max = BitConverter.ToUInt16(file, index + 0x16);
            for (int f = 0; f < 8; f++)
                Fruits[f] = (ChaoFruitsSA1)file[index + 0x18 + f];
            RunOrPower = BitConverter.ToSingle(file, index + 0x20);
            FlyOrSwim = BitConverter.ToSingle(file, index + 0x24);
            Magnitude = BitConverter.ToSingle(file, index + 0x28);
            Affection = BitConverter.ToUInt16(file, index + 0x2C);
            LifeLeft = BitConverter.ToUInt16(file, index + 0x2E);
            AgingFactor = BitConverter.ToUInt16(file, index + 0x30);
            AnimalAbilities = (AnimalFlagsSA1)BitConverter.ToUInt16(file, index + 0x32);
            Jewels = (ChaoJewelsSA1)file[index + 0x34];
            Key2 = file[index + 0x35];
            Flags = (ChaoColorFlagsSA1)BitConverter.ToUInt16(file, index + 0x36);
            Position = new Vertex(file, index + 0x38);
            Age = BitConverter.ToUInt32(file, index + 0x44);
            ID = BitConverter.ToUInt32(file, index + 0x48);
            for (int p = 0; p < 7; p++)
                AnimalParts[p] = (AnimalPartsSA1)file[index + 0x4C + p];
            Key3 = file[index + 0x53];
            SwimPoints = BitConverter.ToUInt16(file, index + 0x54);
            FlyPoints = BitConverter.ToUInt16(file, index + 0x56);
            RunPoints = BitConverter.ToUInt16(file, index + 0x58);
            PowerPoints = BitConverter.ToUInt16(file, index + 0x5A);
            Kindness = (sbyte)file[index + 0x5C];
            Aggressive = (sbyte)file[index + 0x5D];
            Curiosity = (sbyte)file[index + 0x5E];
            Charm = file[index + 0x5F];
            Breed = file[index + 0x60];
            Sleep = file[index + 0x61];
            Hunger = file[index + 0x62];
            Tedious = file[index + 0x63];
            Tiredness = file[index + 0x64];
            Stress = file[index + 0x65];
            Narrow = file[index + 0x66];
            Pleasure = file[index + 0x67];
            Anger = file[index + 0x68];
            Sorrow = file[index + 0x69];
            Fear = file[index + 0x6A];
            Loneliness = file[index + 0x6B];
            // IsCPU is at 0x17A in the struct but it's missing in the data, so everything shifts back 1 byte
            Memories = new ChaoMemoriesSA1(file, index + 0x6C);
            Reincarnations = file[index + 0x178];
            Lane = file[index + 0x179];
            Key4 = file[index + 0x17A];
            Exists = (sbyte)file[index + 0x17B];
            CocoonTimer = BitConverter.ToUInt16(file, index + 0x17C);
            for (int r = 0; r < 20; r++)
                RaceTime[r] = file[index + 0x17E + r];
            JewelBreed = (ChaoJewelBreedsSA1)BitConverter.ToUInt16(file, index + 0x192);
        }

        public byte[] GetBytes()
        {
            List<byte> result = new List<byte>();
            result.Add((byte)Type);
            result.Add((byte)Garden);
            result.Add((byte)Happiness);
            result.Add(Key1);
            result.AddRange(GetChaoNameBytes(Name));
            result.AddRange(BitConverter.GetBytes(Swim));
            result.AddRange(BitConverter.GetBytes(Run));
            result.AddRange(BitConverter.GetBytes(Fly));
            result.AddRange(BitConverter.GetBytes(Power));
            result.AddRange(BitConverter.GetBytes(HP));
            result.AddRange(BitConverter.GetBytes(HP_Max));
            for (int f = 0; f < 8; f++)
                result.Add((byte)Fruits[f]);
            result.AddRange(BitConverter.GetBytes(RunOrPower));
            result.AddRange(BitConverter.GetBytes(FlyOrSwim));
            result.AddRange(BitConverter.GetBytes(Magnitude));
            result.AddRange(BitConverter.GetBytes(Affection));
            result.AddRange(BitConverter.GetBytes(LifeLeft));
            result.AddRange(BitConverter.GetBytes(AgingFactor));
            result.AddRange(BitConverter.GetBytes((ushort)AnimalAbilities));
            result.Add((byte)Jewels);
            result.Add(Key2);
            result.AddRange(BitConverter.GetBytes((ushort)Flags));
            result.AddRange(BitConverter.GetBytes(Position.X));
            result.AddRange(BitConverter.GetBytes(Position.Y));
            result.AddRange(BitConverter.GetBytes(Position.Z));
            result.AddRange(BitConverter.GetBytes(Age));
            result.AddRange(BitConverter.GetBytes(ID));
            for (int p = 0; p < 7; p++)
                result.Add((byte)AnimalParts[p]);
            result.Add(Key3);
            result.AddRange(BitConverter.GetBytes(SwimPoints));
            result.AddRange(BitConverter.GetBytes(FlyPoints));
            result.AddRange(BitConverter.GetBytes(RunPoints));
            result.AddRange(BitConverter.GetBytes(PowerPoints));
            result.Add((byte)Kindness);
            result.Add((byte)Aggressive);
            result.Add((byte)Curiosity);
            result.Add(Charm);
            result.Add(Breed);
            result.Add(Sleep);
            result.Add(Hunger);
            result.Add(Tedious);
            result.Add(Tiredness);
            result.Add(Stress);
            result.Add(Narrow);
            result.Add(Pleasure);
            result.Add(Anger);
            result.Add(Sorrow);
            result.Add(Fear);
            result.Add(Loneliness);
            // IsCPU is at 0x17A in the struct but it's missing in the data, so everything shifts back 1 byte
            result.AddRange(Memories.GetBytes());
            result.Add(Reincarnations);
            result.Add(Lane);
            result.Add(Key4);
            result.Add((byte)Exists);
            result.AddRange(BitConverter.GetBytes(CocoonTimer));
            for (int r = 0; r < 20; r++)
                result.Add(RaceTime[r]);
            result.AddRange(BitConverter.GetBytes((ushort)JewelBreed));
            if (result.Count < 512)
            {
                do
                    result.Add(0);
                while (result.Count < 512);
            }
            return result.ToArray();
        }
    }
}
