using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using System;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;
using SplitTools;

namespace SA2ObjectDefinitions.Common
{
	public abstract class Minimal : ObjectDefinition
	{
		private NJS_OBJECT[] animalmodels;
		protected NJS_OBJECT[] animalsets;
		protected Mesh[][] animalsetmeshes;
		protected Texture[] texs;

		public void InitSetup(ObjectData data, string name)
		{
			animalmodels = new NJS_OBJECT[21];

			animalsets = new NJS_OBJECT[5];
			animalsetmeshes = new Mesh[5][];

			animalmodels[0] = ObjectHelper.LoadModel("chao/animals/MINIMAL_ARA.sa2mdl");    //Raccoon
			animalmodels[1] = ObjectHelper.LoadModel("chao/animals/MINIMAL_BATMAN.sa2mdl"); //Bat
			animalmodels[2] = ObjectHelper.LoadModel("chao/animals/MINIMAL_BONE.sa2mdl");   //Skeleton Dog
			animalmodels[3] = ObjectHelper.LoadModel("chao/animals/MINIMAL_CHIT.sa2mdl");   //Cheetah
			animalmodels[4] = ObjectHelper.LoadModel("chao/animals/MINIMAL_DRA.sa2mdl");    //Dragon
			animalmodels[5] = ObjectHelper.LoadModel("chao/animals/MINIMAL_GOMA.sa2mdl");   //Seal
			animalmodels[6] = ObjectHelper.LoadModel("chao/animals/MINIMAL_GORI.sa2mdl");   //Gorilla
			animalmodels[7] = ObjectHelper.LoadModel("chao/animals/MINIMAL_HAN.sa2mdl");    //Half-Fish
			animalmodels[8] = ObjectHelper.LoadModel("chao/animals/MINIMAL_INO.sa2mdl");    //Boar
			animalmodels[9] = ObjectHelper.LoadModel("chao/animals/MINIMAL_KON.sa2mdl");    //Condor
			animalmodels[10] = ObjectHelper.LoadModel("chao/animals/MINIMAL_KUJA.sa2mdl");  //Peacock
			animalmodels[11] = ObjectHelper.LoadModel("chao/animals/MINIMAL_KUMA.sa2mdl");  //Bear
			animalmodels[12] = ObjectHelper.LoadModel("chao/animals/MINIMAL_OUM.sa2mdl");   //Parrot
			animalmodels[13] = ObjectHelper.LoadModel("chao/animals/MINIMAL_PEN.sa2mdl");   //Penguin
			animalmodels[14] = ObjectHelper.LoadModel("chao/animals/MINIMAL_PHEN.sa2mdl");  //Phoenix
			animalmodels[15] = ObjectHelper.LoadModel("chao/animals/MINIMAL_RAKO.sa2mdl");  //Sea Otter
			animalmodels[16] = ObjectHelper.LoadModel("chao/animals/MINIMAL_SHEP.sa2mdl");  //Sheep
			animalmodels[17] = ObjectHelper.LoadModel("chao/animals/MINIMAL_SKA.sa2mdl");   //Skunk
			animalmodels[18] = ObjectHelper.LoadModel("chao/animals/MINIMAL_TORA.sa2mdl");  //Tiger
			animalmodels[19] = ObjectHelper.LoadModel("chao/animals/MINIMAL_UNI.sa2mdl");   //Unicorn
			animalmodels[20] = ObjectHelper.LoadModel("chao/animals/MINIMAL_USA.sa2mdl");   //Rabbit
		}
		public void SetAnimals(int levelID)
		{
			switch (levelID)
			{
				case 3: // Green Forest
					animalsets[0] = animalmodels[7];
					animalsets[1] = animalmodels[12];
					animalsets[2] = animalmodels[0];
					animalsets[3] = animalmodels[20];
					animalsets[4] = animalmodels[3];
					break;
				case 4: // White Jungle
					animalsets[0] = animalmodels[4];
					animalsets[1] = animalmodels[11];
					animalsets[2] = animalmodels[17];
					animalsets[3] = animalmodels[10];
					animalsets[4] = animalmodels[9];
					break;
				case 5: // Pumpkin Hill
					animalsets[0] = animalmodels[7];
					animalsets[1] = animalmodels[1];
					animalsets[2] = animalmodels[2];
					animalsets[3] = animalmodels[3];
					animalsets[4] = animalmodels[8];
					break;
				case 6: // Sky Rail
					animalsets[0] = animalmodels[14];
					animalsets[1] = animalmodels[9];
					animalsets[2] = animalmodels[16];
					animalsets[3] = animalmodels[11];
					animalsets[4] = animalmodels[18];
					break;
				case 7: // Aquatic Mine
					animalsets[0] = animalmodels[4];
					animalsets[1] = animalmodels[9];
					animalsets[2] = animalmodels[17];
					animalsets[3] = animalmodels[13];
					animalsets[4] = animalmodels[5];
					break;
				case 8: // Security Hall
					animalsets[0] = animalmodels[14];
					animalsets[1] = animalmodels[18];
					animalsets[2] = animalmodels[0];
					animalsets[3] = animalmodels[12];
					animalsets[4] = animalmodels[9];
					break;
				case 9: // Prison Lane
					animalsets[0] = animalmodels[19];
					animalsets[1] = animalmodels[17];
					animalsets[2] = animalmodels[16];
					animalsets[3] = animalmodels[18];
					animalsets[4] = animalmodels[6];
					break;
				case 10: // Metal Harbor
					animalsets[0] = animalmodels[14];
					animalsets[1] = animalmodels[15];
					animalsets[2] = animalmodels[0];
					animalsets[3] = animalmodels[5];
					animalsets[4] = animalmodels[13];
					break;
				case 11: // Iron Gate
					animalsets[0] = animalmodels[4];
					animalsets[1] = animalmodels[20];
					animalsets[2] = animalmodels[17];
					animalsets[3] = animalmodels[6];
					animalsets[4] = animalmodels[18];
					break;
				case 12: // Weapons Bed
					animalsets[0] = animalmodels[14];
					animalsets[1] = animalmodels[3];
					animalsets[2] = animalmodels[16];
					animalsets[3] = animalmodels[5];
					animalsets[4] = animalmodels[15];
					break;
				case 13: // City Escape
					animalsets[0] = animalmodels[19];
					animalsets[1] = animalmodels[17];
					animalsets[2] = animalmodels[16];
					animalsets[3] = animalmodels[0];
					animalsets[4] = animalmodels[20];
					break;
				case 14: // Radical Highway
					animalsets[0] = animalmodels[19];
					animalsets[1] = animalmodels[5];
					animalsets[2] = animalmodels[0];
					animalsets[3] = animalmodels[3];
					animalsets[4] = animalmodels[8];
					break;
				case 16: // Wild Canyon
					animalsets[0] = animalmodels[4];
					animalsets[1] = animalmodels[3];
					animalsets[2] = animalmodels[16];
					animalsets[3] = animalmodels[10];
					animalsets[4] = animalmodels[9];
					break;
				case 17: // Mission Street
					animalsets[0] = animalmodels[14];
					animalsets[1] = animalmodels[6];
					animalsets[2] = animalmodels[16];
					animalsets[3] = animalmodels[8];
					animalsets[4] = animalmodels[20];
					break;
				case 18: // Dry Lagoon
					animalsets[0] = animalmodels[19];
					animalsets[1] = animalmodels[10];
					animalsets[2] = animalmodels[16];
					animalsets[3] = animalmodels[15];
					animalsets[4] = animalmodels[13];
					break;
				case 21: // Sand Ocean
					animalsets[0] = animalmodels[2];
					animalsets[1] = animalmodels[1];
					animalsets[2] = animalmodels[0];
					animalsets[3] = animalmodels[12];
					animalsets[4] = animalmodels[10];
					break;
				case 22: // Crazy Gadget
					animalsets[0] = animalmodels[14];
					animalsets[1] = animalmodels[5];
					animalsets[2] = animalmodels[17];
					animalsets[3] = animalmodels[11];
					animalsets[4] = animalmodels[18];
					break;
				case 23: // Hidden Base
					animalsets[0] = animalmodels[19];
					animalsets[1] = animalmodels[18];
					animalsets[2] = animalmodels[17];
					animalsets[3] = animalmodels[13];
					animalsets[4] = animalmodels[15];
					break;
				case 24: // Eternal Engine
					animalsets[0] = animalmodels[7];
					animalsets[1] = animalmodels[8];
					animalsets[2] = animalmodels[0];
					animalsets[3] = animalmodels[9];
					animalsets[4] = animalmodels[12];
					break;
				case 25: // Death Chamber
					animalsets[0] = animalmodels[2];
					animalsets[1] = animalmodels[20];
					animalsets[2] = animalmodels[17];
					animalsets[3] = animalmodels[18];
					animalsets[4] = animalmodels[6];
					break;
				case 26: // Egg Quarters
					animalsets[0] = animalmodels[7];
					animalsets[1] = animalmodels[12];
					animalsets[2] = animalmodels[17];
					animalsets[3] = animalmodels[6];
					animalsets[4] = animalmodels[11];
					break;
				case 27: // Lost Colony
					animalsets[0] = animalmodels[2];
					animalsets[1] = animalmodels[1];
					animalsets[2] = animalmodels[0];
					animalsets[3] = animalmodels[20];
					animalsets[4] = animalmodels[8];
					break;
				case 28: // Pyramid Cave
					animalsets[0] = animalmodels[2];
					animalsets[1] = animalmodels[1];
					animalsets[2] = animalmodels[16];
					animalsets[3] = animalmodels[9];
					animalsets[4] = animalmodels[10];
					break;
				case 30: // Final Rush
					animalsets[0] = animalmodels[4];
					animalsets[1] = animalmodels[13];
					animalsets[2] = animalmodels[16];
					animalsets[3] = animalmodels[10];
					animalsets[4] = animalmodels[9];
					break;
				case 31: // Green Hill
					animalsets[0] = animalmodels[19];
					animalsets[1] = animalmodels[13];
					animalsets[2] = animalmodels[0];
					animalsets[3] = animalmodels[20];
					animalsets[4] = animalmodels[12];
					break;
				case 32: // Meteor Herd
					animalsets[0] = animalmodels[14];
					animalsets[1] = animalmodels[20];
					animalsets[2] = animalmodels[16];
					animalsets[3] = animalmodels[13];
					animalsets[4] = animalmodels[5];
					break;
				case 34: // Cannon's Core (Sonic)
					animalsets[0] = animalmodels[4];
					animalsets[1] = animalmodels[18];
					animalsets[2] = animalmodels[17];
					animalsets[3] = animalmodels[15];
					animalsets[4] = animalmodels[13];
					break;
				case 35: // Cannon's Core (Eggman)
					animalsets[0] = animalmodels[14];
					animalsets[1] = animalmodels[11];
					animalsets[2] = animalmodels[0];
					animalsets[3] = animalmodels[5];
					animalsets[4] = animalmodels[15];
					break;
				case 36: // Cannon's Core (Tails)
					animalsets[0] = animalmodels[19];
					animalsets[1] = animalmodels[5];
					animalsets[2] = animalmodels[17];
					animalsets[3] = animalmodels[11];
					animalsets[4] = animalmodels[6];
					break;
				case 37: // Cannon's Core (Rouge)
					animalsets[0] = animalmodels[19];
					animalsets[1] = animalmodels[9];
					animalsets[2] = animalmodels[17];
					animalsets[3] = animalmodels[8];
					animalsets[4] = animalmodels[3];
					break;
				case 38: // Cannon's Core (Knuckles)
					animalsets[0] = animalmodels[4];
					animalsets[1] = animalmodels[3];
					animalsets[2] = animalmodels[0];
					animalsets[3] = animalmodels[5];
					animalsets[4] = animalmodels[15];
					break;
				case 40: // Final Chase
					animalsets[0] = animalmodels[14];
					animalsets[1] = animalmodels[18];
					animalsets[2] = animalmodels[17];
					animalsets[3] = animalmodels[15];
					animalsets[4] = animalmodels[13];
					break;
				case 43: // Cosmic Wall
					animalsets[0] = animalmodels[19];
					animalsets[1] = animalmodels[15];
					animalsets[2] = animalmodels[16];
					animalsets[3] = animalmodels[20];
					animalsets[4] = animalmodels[3];
					break;
				case 44: // Mad Space
					animalsets[0] = animalmodels[14];
					animalsets[1] = animalmodels[6];
					animalsets[2] = animalmodels[0];
					animalsets[3] = animalmodels[12];
					animalsets[4] = animalmodels[10];
					break;
				default: // All other stages
					animalsets[0] = animalmodels[13];
					animalsets[1] = animalmodels[13];
					animalsets[2] = animalmodels[13];
					animalsets[3] = animalmodels[13];
					animalsets[4] = animalmodels[13];
					break;
			}
			for (int i = 0; i < 5; i++)
				animalsetmeshes[i] = ObjectHelper.GetMeshes(animalsets[i]);
		}
		public int SetAnimalType(SETItem item, out int id)
		{
			id = 0;
			var rand = new Random();
			if ((item.Rotation.X & 0xFF) == 16)
				id = 1;
			if ((item.Rotation.X & 0xFF) == 17)
				id = 2;
			if ((item.Rotation.X & 0xFF) >= 18 && (item.Rotation.X & 0xFF) < 32)
				id = rand.Next(1, 3);
			if ((item.Rotation.X & 0xFF) == 32)
				id = 3;
			if ((item.Rotation.X & 0xFF) == 33)
				id = 4;
			if ((item.Rotation.X & 0xFF) >= 34 && (item.Rotation.X & 0xFF) < 48)
				id = rand.Next(3, 5);
			if ((item.Rotation.X & 0xFF) >= 48)
				id = rand.Next(1, 5);
			return id;
		}
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			SetAnimalType(item, out int id);
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y - 0x8000, item.Rotation.Z);
			result = animalsets[id].CheckHit(Near, Far, Viewport, Projection, View, transform, animalsetmeshes[id]);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			SetAnimalType(item, out int id);
			if (texs == null)
				texs = ObjectHelper.GetTextures("AL_MINIMAL_TEX", null, dev);
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y - 0x8000, item.Rotation.Z);
			result.AddRange(animalsets[id].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, animalsetmeshes[id], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(animalsets[id].DrawModelTreeInvert(transform, animalsetmeshes[id]));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			SetAnimalType(item, out int id);
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y - 0x8000, item.Rotation.Z);
			result.Add(new ModelTransform(animalsets[id], transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			SetAnimalType(item, out int id);
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateObject(item.Rotation.X, item.Rotation.Y - 0x8000, item.Rotation.Z);
			return ObjectHelper.GetModelBounds(animalsets[id], transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, item.Rotation.X, item.Rotation.Y - 0x8000, item.Rotation.Z);

			return matrix;
		}

		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x; int z; direction.GetRotation(out x, out z);
			item.Rotation.X = x + 0x4000;
			item.Rotation.Z = -z;
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Animal Condition", typeof(AnimalCondition), "Extended", null, null, (o) => (AnimalCondition)((o.Rotation.X >> 8) & 0xF),
				(o, v) => { o.Rotation.X &= 0xF0FF; o.Rotation.X |= ((byte)v) << 8; }),
			new PropertySpec("Animal Type", typeof(AnimalSet), "Extended", null, null, (o) => (AnimalSet)(o.Rotation.X & 0xFF),
			(o, v) => { o.Rotation.X &= 0xFF00; o.Rotation.X |= (byte)v; }),
			new PropertySpec("Movement Radius", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Speed", typeof(int), "Extended", null, null, (o) => o.Rotation.Z, (o, v) => o.Rotation.Z = (int)v),

		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override string Name { get { return "Small Animal"; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
	public class GreenForestMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(3);
		}
	}
	public class WhiteJungleMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(4);
		}
	}
	public class PumpkinHillMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(5);
		}
	}
	public class SkyRailMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(6);
		}
	}
	public class AquaticMineMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(7);
		}
	}
	public class SecurityHallMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(8);
		}
	}
	public class PrisonLaneMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(9);
		}
	}
	public class MetalHarborMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(10);
		}
	}
	public class IronGateMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(11);
		}
	}
	public class WeaponsBedMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(12);
		}
	}
	public class CityEscapeMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(13);
		}
	}
	public class RadicalHighwayMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(14);
		}
	}
	public class WildCanyonMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(16);
		}
	}
	public class MissionStreetMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(17);
		}
	}
	public class DryLagoonMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(18);
		}
	}
	public class SandOceanMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(21);
		}
	}
	public class CrazyGadgetMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(22);
		}
	}
	public class HiddenBaseMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(23);
		}
	}
	public class EternalEngineMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(24);
		}
	}
	public class DeathChamberMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(25);
		}
	}
	public class EggQuartersMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(26);
		}
	}
	public class LostColonyMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(27);
		}
	}
	public class PyramidCaveMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(28);
		}
	}
	public class FinalRushMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(30);
		}
	}
	public class GreenHillMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(31);
		}
	}
	public class MeteorHerdMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(32);
		}
	}
	public class CannonsCoreSMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(34);
		}
	}
	public class CannonsCoreEMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(35);
		}
	}
	public class CannonsCoreTMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(36);
		}
	}
	public class CannonsCoreRMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(37);
		}
	}
	public class CannonsCoreKMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(38);
		}
	}
	public class FinalChaseMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(40);
		}
	}
	public class CosmicWallMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(43);
		}
	}
	public class MadSpaceMinimal : Minimal
	{
		public override void Init(ObjectData data, string name)
		{
			InitSetup(data, name);
			SetAnimals(44);
		}
	}

	public enum AnimalCondition : byte
		{
			Roam,
			Stationary,
			Whistle
		}
		
		public enum AnimalSet : byte
		{
			Rare,
			AnimalA1 = 0x10,
			AnimalA2,
			RandomSetA0,
			RandomSetA1,
			RandomSetA2,
			RandomSetA3,
			RandomSetA4,
			RandomSetA5,
			RandomSetA6,
			RandomSetA7,
			RandomSetA8,
			RandomSetA9,
			RandomSetA10,
			RandomSetA11,
			RandomSetA12,
			RandomSetA13,
			AnimalB1,
			AnimalB2,
			RandomSetB0,
			RandomSetB1,
			RandomSetB2,
			RandomSetB3,
			RandomSetB4,
			RandomSetB5,
			RandomSetB6,
			RandomSetB7,
			RandomSetB8,
			RandomSetB9,
			RandomSetB10,
			RandomSetB11,
			RandomSetB12,
			RandomSetB13,
			Random
		}
		/*public enum Animal
		{
			Phoenix,
			Unicorn,
			Dragon,
			Bat,
			SkeletonDog,
			HalfFish,
			Raccoon,
			Sheep,
			Skunk,
			Condor,
			Parrot,
			Peacock,
			Gorilla,
			Tiger,
			Bear,
			Boar,
			Cheetah,
			Rabbit,
			Otter,
			Seal,
			Penguin,
		}*/
}