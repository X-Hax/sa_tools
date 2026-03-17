using System;
using System.Collections.Generic;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using SharpDX;
using SharpDX.Direct3D9;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.StationSquare
{
	public class People : ObjectDefinition
	{
		enum SsPeopleTypes : int
		{
			ManA, ManB, ManC, ManD,
			ManStationWorker, ManBurgerShopWorker,
			WomanA, WomanB, WomanC, WomanD,
			WomanSwimsuitC, WomanSwimsuitB, WomanSwimsuitA,
			WomanBurgerShopWorker,
			GirlA, GirlB, GirlC, GirlD,
			FatManA, FatManB, FatManC, FatManD,
			BoyB, BoyA, BoyC, BoyD
		}

		private NJS_OBJECT[] models = new NJS_OBJECT[26];
		private Mesh[][] meshes = new Mesh[26][];

		SsPeopleTypes currentModelIndex;

		readonly KeyValuePair<string, string>[] SsPeopleModels =
		{
			new KeyValuePair<string, string>("adv00_stationsquare/object/oyaji_mdl_a.nja.sa1mdl", "SS_PEOPLE"),
			new KeyValuePair<string, string>("adv00_stationsquare/object/oyaji_mdl_b.nja.sa1mdl", "SS_PEOPLE"),
			new KeyValuePair<string, string>("adv00_stationsquare/object/oyaji_mdl_c.nja.sa1mdl", "SS_PEOPLE"),
			new KeyValuePair<string, string>("adv00_stationsquare/object/oyaji_mdl_d.nja.sa1mdl", "SS_PEOPLE"),

			new KeyValuePair<string, string>("adv00_stationsquare/object/oyaji_mdl2_a.nja.sa1mdl", "SS_EKIIN"),
			new KeyValuePair<string, string>("adv00_stationsquare/object/oyaji_mdl2_b.nja.sa1mdl", "SS_BURGER"),

			new KeyValuePair<string, string>("adv00_stationsquare/object/ol_mdl_a.nja.sa1mdl", "SS_PEOPLE"),
			new KeyValuePair<string, string>("adv00_stationsquare/object/ol_mdl_b.nja.sa1mdl", "SS_PEOPLE"),
			new KeyValuePair<string, string>("adv00_stationsquare/object/ol_mdl_c.nja.sa1mdl", "SS_PEOPLE"),
			new KeyValuePair<string, string>("adv00_stationsquare/object/ol_mdl_d.nja.sa1mdl", "SS_PEOPLE"),
			
			new KeyValuePair<string, string>("adv00_stationsquare/object/ol_mdl2_c.nja.sa1mdl", "SS_MIZUGI"),
			new KeyValuePair<string, string>("adv00_stationsquare/object/ol_mdl2_b.nja.sa1mdl", "SS_MIZUGI"),
			new KeyValuePair<string, string>("adv00_stationsquare/object/ol_mdl2_a.nja.sa1mdl", "SS_MIZUGI"),
			new KeyValuePair<string, string>("adv00_stationsquare/object/ol_mdl2_d.nja.sa1mdl", "SS_BURGER"),
			
			new KeyValuePair<string, string>("adv00_stationsquare/object/girl_mdl_a.nja.sa1mdl", "SS_PEOPLE"),
			new KeyValuePair<string, string>("adv00_stationsquare/object/girl_mdl_b.nja.sa1mdl", "SS_PEOPLE"),
			new KeyValuePair<string, string>("adv00_stationsquare/object/girl_mdl_c.nja.sa1mdl", "SS_PEOPLE"),
			new KeyValuePair<string, string>("adv00_stationsquare/object/girl_mdl_d.nja.sa1mdl", "SS_PEOPLE"),
			
			new KeyValuePair<string, string>("adv00_stationsquare/object/fatman_mdl_a.nja.sa1mdl", "SS_PEOPLE"),
			new KeyValuePair<string, string>("adv00_stationsquare/object/fatman_mdl_b.nja.sa1mdl", "SS_PEOPLE"),
			new KeyValuePair<string, string>("adv00_stationsquare/object/fatman_mdl_c.nja.sa1mdl", "SS_PEOPLE"),
			new KeyValuePair<string, string>("adv00_stationsquare/object/fatman_mdl_d.nja.sa1mdl", "SS_PEOPLE"),
			
			new KeyValuePair<string, string>("adv00_stationsquare/object/boy_mdl_b.nja.sa1mdl", "SS_PEOPLE"),
			new KeyValuePair<string, string>("adv00_stationsquare/object/boy_mdl_a.nja.sa1mdl", "SS_PEOPLE"),
			new KeyValuePair<string, string>("adv00_stationsquare/object/boy_mdl_c.nja.sa1mdl", "SS_PEOPLE"),
			new KeyValuePair<string, string>("adv00_stationsquare/object/boy_mdl_d.nja.sa1mdl", "SS_PEOPLE"),	
		};

		private float GetVerticalOffsetForNpcType(SsPeopleTypes type)
		{
			switch (type)
			{
				case SsPeopleTypes.ManA:
				case SsPeopleTypes.ManB:
				case SsPeopleTypes.ManC:
				case SsPeopleTypes.ManD:
				case SsPeopleTypes.ManStationWorker:
				case SsPeopleTypes.ManBurgerShopWorker:
					return 10.4f;
				case SsPeopleTypes.GirlA:
				case SsPeopleTypes.GirlB:
				case SsPeopleTypes.GirlC:
				case SsPeopleTypes.GirlD:
					return 6.2f;
				case SsPeopleTypes.BoyA:
				case SsPeopleTypes.BoyB:
				case SsPeopleTypes.BoyC:
				case SsPeopleTypes.BoyD:
					return 5.6f;
				case SsPeopleTypes.FatManA:
				case SsPeopleTypes.FatManB:
				case SsPeopleTypes.FatManC:
				case SsPeopleTypes.FatManD:
					return 8.83f;
				case SsPeopleTypes.WomanA:
				case SsPeopleTypes.WomanB:
				case SsPeopleTypes.WomanC:
				case SsPeopleTypes.WomanD:
				case SsPeopleTypes.WomanBurgerShopWorker:
				case SsPeopleTypes.WomanSwimsuitA:
				case SsPeopleTypes.WomanSwimsuitB:
				case SsPeopleTypes.WomanSwimsuitC:
				default:
					return 11.35f;
			}
		}

		public override void Init(ObjectData data, string name)
		{
			for (int i = 0; i < 26; i++)
			{
				models[i] = ObjectHelper.LoadModel(SsPeopleModels[i].Key);
				meshes[i] = ObjectHelper.GetMeshes(models[i]);
			}
		}

		private SsPeopleTypes GetCurrentModelIndex(int val)
		{
			// Clamps the value between 0 and 25
			return (SsPeopleTypes)Math.Max(0, Math.Min(val, 25));
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			currentModelIndex = GetCurrentModelIndex(item.Rotation.X);
			float YOffset = GetVerticalOffsetForNpcType(currentModelIndex);
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + YOffset, item.Position.Z);
			transform.NJRotateY(item.Rotation.Y);
			result = HitResult.Min(result, models[(int)currentModelIndex].CheckHit(Near, Far, Viewport, Projection, View, transform, meshes[(int)currentModelIndex]));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			currentModelIndex = GetCurrentModelIndex(item.Rotation.X);
			if (models[(int)currentModelIndex] == null)
				return new List<RenderInfo>();
			float YOffset = GetVerticalOffsetForNpcType(currentModelIndex);
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + YOffset, item.Position.Z);
			transform.NJRotateY(item.Rotation.Y);
			result.AddRange(models[(int)currentModelIndex].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures(SsPeopleModels[(int)currentModelIndex].Value), meshes[(int)currentModelIndex]));
			if (item.Selected)
				result.AddRange(models[(int)currentModelIndex].DrawModelTreeInvert(transform, meshes[(int)currentModelIndex]));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			currentModelIndex = GetCurrentModelIndex(item.Rotation.X);
			float YOffset = GetVerticalOffsetForNpcType(currentModelIndex);
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + YOffset, item.Position.Z);
			transform.NJRotateY(item.Rotation.Y);
			result.Add(new ModelTransform(models[(int)currentModelIndex], transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			currentModelIndex = GetCurrentModelIndex(item.Rotation.X);
			float YOffset = GetVerticalOffsetForNpcType(currentModelIndex);
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.X, item.Position.Y + YOffset, item.Position.Z);
			transform.NJRotateObject(0, item.Rotation.Y, 0);
			NJS_OBJECT model = models[(int)currentModelIndex];
			return ObjectHelper.GetModelBounds(model, transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			currentModelIndex = GetCurrentModelIndex(item.Rotation.X);
			float YOffset = GetVerticalOffsetForNpcType(currentModelIndex);
			Matrix matrix = Matrix.Identity;
			MatrixFunctions.Translate(ref matrix, item.Position.X, item.Position.Y + YOffset, item.Position.Z);
			return matrix;
		}

		public override string Name { get { return "Station Square NPC"; } }

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Model Type", typeof(SsPeopleTypes), "Extended", "Variation of the NPC model.", null, (o) => (SsPeopleTypes)Math.Min(Math.Max((int)o.Rotation.X, 0), 25), (o, v) => o.Rotation.X = (int)v),
			new PropertySpec("Conversation ID", typeof(int), "Extended", "NPC ID in SADXTweaker2's Message File Editor. It's 0-based here, so the ID in SADXTweaker2 will be this ID + 1.", null, (o) => (int)Math.Min(Math.Max((int)o.Scale.Z, 0), 999), (o, v) => o.Scale.Z = (int)v)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
	}
}