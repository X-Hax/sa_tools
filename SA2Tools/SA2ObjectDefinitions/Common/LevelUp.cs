using Assimp;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using SharpDX;
using SharpDX.Direct3D9;
using SplitTools;
using System;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SA2ObjectDefinitions.Common
{
	public class LevelUp : ObjectDefinition
	{
		private NJS_OBJECT ring;
		private Mesh[] meshesRing;
		private NJS_TEXLIST texarrRing;
		private Texture[] texsRing;

		private NJS_OBJECT grndglow;
		private Mesh[] meshesGrnd;
		private NJS_TEXLIST texarrGrnd;
		private Texture[] texsGrnd;

		private NJS_OBJECT glow;
		private Mesh[] meshesGlow;
		private NJS_TEXLIST texarrGlow;
		private Texture[] texsGlow;

		private NJS_OBJECT[] upgrades;
		private Mesh[][] meshesUpg;
		//private NJS_TEXLIST texarrUpg;
		private Texture[][] texsUpg;

		protected FVF_PositionTextured[] Vertices = new FVF_PositionTextured[4];
		protected Mesh spriteMesh;
		protected BasicAttach spriteAttach;
		protected NJS_OBJECT spriteObject;
		protected Mesh[] spriteMeshs;
		protected BoundingSphere Bounds = new();

		public static readonly NJS_MATERIAL OpaqueMaterial = new()
		{
			IgnoreLighting = true,
			SourceAlpha = AlphaInstruction.SourceAlpha,
			DestinationAlpha = AlphaInstruction.InverseSourceAlpha,
			TextureID = 0,
			UseTexture = true,
			UseAlpha = false
		};

		public static readonly NJS_MATERIAL TranslucentMaterial = new()
		{
			IgnoreLighting = true,
			SourceAlpha = AlphaInstruction.SourceAlpha,
			DestinationAlpha = AlphaInstruction.InverseSourceAlpha,
			TextureID = 0,
			UseTexture = true,
			UseAlpha = true
		};

		private BasicAttach GenerateAttach(FVF_PositionTextured[] fvf, SETItem item = null)
		{
			Vertices[0] = new FVF_PositionTextured(new SharpDX.Vector3(1F, 1F, -0.5F), new SharpDX.Vector2(0, 0));
			Vertices[1] = new FVF_PositionTextured(new SharpDX.Vector3(1F, -1F, -0.5F), new SharpDX.Vector2(0, 1.0f));
			Vertices[2] = new FVF_PositionTextured(new SharpDX.Vector3(-1F, 1F, -0.5F), new SharpDX.Vector2(1.0f, 0.0f));
			Vertices[3] = new FVF_PositionTextured(new SharpDX.Vector3(-1F, -1F, -0.5F), new SharpDX.Vector2(1.0f, 1.0f));
			// Poly
			ushort[] SquareInds = { 0, 1, 2, 3 };
			List<Poly> poly = new List<Poly>();
			poly.Add(new Strip(SquareInds, false));
			// UV
			List<UV> uv = new List<UV>();
			// Meshset
			List<NJS_MESHSET> meshes = new List<NJS_MESHSET>();
			NJS_MESHSET mesh = new NJS_MESHSET(poly.ToArray(), false, true, false);
			for (int i = 0; i < SquareInds.Length; i++)
			{
				mesh.UV[i] = new UV(fvf[SquareInds[i]].UV.X, fvf[SquareInds[i]].UV.Y);
			}
			meshes.Add(mesh);
			// Vertices and normals
			List<Vertex> vertices = new List<Vertex>();
			List<Vertex> normals = new List<Vertex>();
			List<NJS_MATERIAL> mats = new List<NJS_MATERIAL>();
			for (int f = 0; f < fvf.Length; f++)
			{
				vertices.Add(new Vertex(fvf[f].Position.X, fvf[f].Position.Y, fvf[f].Position.Z));
				normals.Add(new Vertex(0, 0, 0));
			}
			mats.Add(TranslucentMaterial);
			spriteAttach = new BasicAttach(vertices.ToArray(), normals.ToArray(), meshes, mats);
			return spriteAttach;
		}
		public void CreateMesh()
		{
			BasicAttach att = GenerateAttach(Vertices);
			att.ProcessVertexData();
			Mesh meshlist = att.CreateD3DMesh();
			Bounds = SharpDX.BoundingSphere.Merge(Bounds.ToSharpDX(), att.Bounds.ToSharpDX()).ToSAModel();
			att.Bounds = Bounds;
			spriteMesh = meshlist;
		}

		public override void Init(ObjectData data, string name)
		{
			CreateMesh();
			spriteObject = new()
			{
				Attach = spriteAttach
			};

			upgrades = new NJS_OBJECT[28];
			meshesUpg = new Mesh[28][];
			texsUpg = new Texture[28][];

			ring = ObjectHelper.LoadModel("object/OBJECT_LEVUPDAI_RING.sa2mdl");
			meshesRing = ObjectHelper.GetMeshes(ring);
			texarrRing = NJS_TEXLIST.Load("object/tls/LEVUPDAI_RING.satex");

			grndglow = ObjectHelper.LoadModel("object/OBJECT_LEVUPDAI_GROUNDGLOW.sa2mdl");
			meshesGrnd = ObjectHelper.GetMeshes(grndglow);
			texarrGrnd = NJS_TEXLIST.Load("object/tls/LEVUPDAI_GROUNDGLOW.satex");

			glow = ObjectHelper.LoadModel("object/OBJECT_LEVUPDAI_GLOW.sa2mdl");
			meshesGlow = ObjectHelper.GetMeshes(glow);
			texarrGlow = NJS_TEXLIST.Load("object/tls/LEVUPDAI_GLOW.satex");

			upgrades[0] = ObjectHelper.LoadModel("OBJECT/LEVUPDAI/SONIC_RUBBERUNIT.sa2mdl");
			upgrades[1] = ObjectHelper.LoadModel("OBJECT/LEVUPDAI/SONIC_SHOES.sa2mdl");
			upgrades[2] = ObjectHelper.LoadModel("OBJECT/LEVUPDAI/SONIC_FLAMERING.sa2mdl");
			upgrades[3] = ObjectHelper.LoadModel("OBJECT/LEVUPDAI/SHADOW_SHOES.sa2mdl");
			upgrades[4] = ObjectHelper.LoadModel("OBJECT/LEVUPDAI/SHADOW_FLAMERING.sa2mdl");
			upgrades[5] = ObjectHelper.LoadModel("OBJECT/LEVUPDAI/TWALKER_BOOSTER.sa2mdl");
			upgrades[6] = ObjectHelper.LoadModel("OBJECT/LEVUPDAI/TWALKER_HYPERCANNON.sa2mdl");
			upgrades[7] = ObjectHelper.LoadModel("OBJECT/LEVUPDAI/TWALKER_LAZERBALSTER.sa2mdl");
			upgrades[8] = ObjectHelper.LoadModel("OBJECT/LEVUPDAI/EWALKER_JETENGINE.sa2mdl");
			upgrades[9] = ObjectHelper.LoadModel("OBJECT/LEVUPDAI/EWALKER_EXTRASHIELD.sa2mdl");
			upgrades[10] = ObjectHelper.LoadModel("OBJECT/LEVUPDAI/EWALKER_BAZOOKA.sa2mdl");
			upgrades[11] = ObjectHelper.LoadModel("OBJECT/LEVUPDAI/EWALKER_POWERGUN.sa2mdl");
			upgrades[12] = ObjectHelper.LoadModel("OBJECT/LEVUPDAI/KNUCKLES_CLAWS.sa2mdl");
			upgrades[13] = ObjectHelper.LoadModel("OBJECT/LEVUPDAI/KNUCKLES_GROVES.sa2mdl");
			upgrades[14] = ObjectHelper.LoadModel("OBJECT/LEVUPDAI/KNUCKLES_AIR.sa2mdl");
			upgrades[15] = ObjectHelper.LoadModel("OBJECT/LEVUPDAI/ROUGE_SCOPE.sa2mdl");
			upgrades[16] = ObjectHelper.LoadModel("OBJECT/LEVUPDAI/ROUGE_NAILS.sa2mdl");
			upgrades[17] = ObjectHelper.LoadModel("OBJECT/LEVUPDAI/ROUGE_BOOTS.sa2mdl");
			upgrades[18] = upgrades[19] = upgrades[20] = upgrades[21] = upgrades[22] = upgrades[23] = upgrades[24] = upgrades[27] = spriteObject;
			upgrades[25] = ObjectHelper.LoadModel("OBJECT/LEVUPDAI/SONIC_MAGICWRIST.sa2mdl");
			upgrades[26] = ObjectHelper.LoadModel("OBJECT/LEVUPDAI/KNUCKLES_SUNGLASS.sa2mdl");

			for (int i = 0; i < 28; i++)
				meshesUpg[i] = ObjectHelper.GetMeshes(upgrades[i]);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			var upID = (int)item.Scale.X;
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Z != 0)
				transform.NJRotateZ(item.Rotation.Z);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result = HitResult.Min(result, ring.CheckHit(Near, Far, Viewport, Projection, View, transform, meshesRing));
			result = HitResult.Min(result, glow.CheckHit(Near, Far, Viewport, Projection, View, transform, meshesGlow));
			result = HitResult.Min(result, grndglow.CheckHit(Near, Far, Viewport, Projection, View, transform, meshesGrnd));
			transform.Pop();
			transform.Push();
			if ((upID > 17 && upID <= 24) || upID == 27)
			{
				transform.NJTranslate(item.Position.X, item.Position.Y + 20.0F, item.Position.Z);
				transform.NJScale(5F, 5F, 1F);
				result = HitResult.Min(result, upgrades[upID].CheckHit(Near, Far, Viewport, Projection, View, transform, meshesUpg[upID]));
			}
			else
			{
				transform.NJTranslate(item.Position);
				result = HitResult.Min(result, upgrades[upID].CheckHit(Near, Far, Viewport, Projection, View, transform, meshesUpg[upID]));
			}
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			var upID = (int)item.Scale.X;
			List<RenderInfo> result = new List<RenderInfo>();
			if (texsRing == null)
				texsRing = ObjectHelper.GetTextures("objtex_common", texarrRing, dev);
			if (texsGrnd == null)
				texsGrnd = ObjectHelper.GetTextures("objtex_common", texarrGrnd, dev);
			if (texsGlow == null)
				texsGlow = ObjectHelper.GetTextures("objtex_common", texarrGlow, dev);
			switch (item.Scale.X)
			{
				case 0:
				case 1:
				case 2:
				case 25:
				default:
					if (texsUpg[upID] == null)
						texsUpg[upID] = ObjectHelper.GetTextures("sonictex", null, dev);
					break;
				case 3:
				case 4:
					if (texsUpg[upID] == null)
						texsUpg[upID] = ObjectHelper.GetTextures("teriostex", null, dev);
					break;
				case 5:
				case 6:
				case 7:
					if (texsUpg[upID] == null)
						texsUpg[upID] = ObjectHelper.GetTextures("twalktex", null, dev);
					break;
				case 8:
				case 9:
				case 10:
				case 11:
					if (texsUpg[upID] == null)
						texsUpg[upID] = ObjectHelper.GetTextures("ewalktex", null, dev);
					break;
				case 12:
				case 13:
				case 14:
				case 26:
					if (texsUpg[upID] == null)
						texsUpg[upID] = ObjectHelper.GetTextures("knucktex", null, dev);
					break;
				case 15:
				case 16:
				case 17:
					if (texsUpg[upID] == null)
						texsUpg[upID] = ObjectHelper.GetTextures("rougetex", null, dev);
					break;
				case 18:
				case 19:
				case 20:
				case 21:
				case 22:
				case 23:
					if (texsUpg[upID] == null)
						texsUpg[upID] = ObjectHelper.GetTextures("levupdai_tmtex", null, dev);
					break;
				case 24:
				case 27:
					if (texsUpg[upID] == null)
						texsUpg[upID] = ObjectHelper.GetTextures("levupdai_thtex", null, dev);
					break;

			}
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Z != 0)
				transform.NJRotateZ(item.Rotation.Z);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result.AddRange(ring.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texsRing, meshesRing, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result.AddRange(grndglow.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texsGrnd, meshesGrnd, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result.AddRange(glow.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texsGlow, meshesGlow, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
			{
				result.AddRange(ring.DrawModelTreeInvert(transform, meshesRing));
				result.AddRange(grndglow.DrawModelTreeInvert(transform, meshesGrnd));
				result.AddRange(glow.DrawModelTreeInvert(transform, meshesGlow));
			}
			transform.Pop();
			transform.Push();
			if ((upID > 17 && upID <= 24) || upID == 27)
			{
				transform.NJTranslate(item.Position.X, item.Position.Y + 20.0F, item.Position.Z);
				if (camera.Yaw != 0)
					transform.NJRotateY(camera.Yaw);
				if (camera.Pitch != 0)
					transform.NJRotateX(camera.Pitch);
				transform.NJScale(5F, 5F, 1F);
				result.AddRange(upgrades[upID].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texsUpg[upID], meshesUpg[upID], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
				{
					result.AddRange(upgrades[upID].DrawModelTreeInvert(transform, meshesUpg[upID]));
				}
			}
			else
			{
				transform.NJTranslate(item.Position);
				result.AddRange(upgrades[upID].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texsUpg[upID], meshesUpg[upID], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				if (item.Selected)
				{
					result.AddRange(upgrades[upID].DrawModelTreeInvert(transform, meshesUpg[upID]));
				}
			}
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Z != 0)
				transform.NJRotateZ(item.Rotation.Z);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result.Add(new ModelTransform(ring, transform.Top));
			result.Add(new ModelTransform(glow, transform.Top));
			result.Add(new ModelTransform(grndglow, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			var upID = (int)item.Scale.X;
			BoundingSphere locationbounds = null;
			BoundingSphere upgradebounds = null;
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(item.Position.ToVector3());
			if (item.Rotation.Z != 0)
				transform.NJRotateZ(item.Rotation.Z);
			if (item.Rotation.X != 0)
				transform.NJRotateX(item.Rotation.X);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			BoundingSphere ringbounds = ObjectHelper.GetModelBounds(ring, transform);
			BoundingSphere glowbounds = ObjectHelper.GetModelBounds(glow, transform);
			BoundingSphere groundbounds = ObjectHelper.GetModelBounds(grndglow, transform);
			locationbounds = SAModel.Direct3D.Extensions.Merge(ringbounds, SAModel.Direct3D.Extensions.Merge(glowbounds, groundbounds));
			transform.Pop();
			transform.Push();
			if ((upID > 17 && upID <= 24) || upID == 27)
			{
				transform.NJTranslate(item.Position.X, item.Position.Y + 20.0F, item.Position.Z);
				transform.NJScale(5F, 5F, 1F);
				upgradebounds = ObjectHelper.GetModelBounds(upgrades[upID], transform);
			}
			else
			{
				transform.NJTranslate(item.Position);
				upgradebounds = ObjectHelper.GetModelBounds(upgrades[upID], transform);
			}
			transform.Pop();
			return SAModel.Direct3D.Extensions.Merge(locationbounds, upgradebounds);
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
			new PropertySpec("Upgrade", typeof(LevelUpItemType), "Extended", null, null, (o) => (LevelUpItemType)o.Scale.X, (o, v) => o.Scale.X = (int)v),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override string Name { get { return "Level-Up Item Location"; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
		public enum LevelUpItemType
		{
			BounceBracelet,
			LightShoes,
			SonicFlameRing,
			AirShoes,
			ShadowFlameRing,
			Booster,
			Bazooka,
			TailsLaserBlaster,
			JetEngine,
			ProtectionArmor,
			LargeCannon,
			EggmanLaserBlaster,
			ShovelClaws,
			HammerGloves,
			AirNecklace,
			TreasureScope,
			PickNails,
			IronBoots,
			SonicMysticMelody,
			ShadowMysticMelody,
			TailsMysticMelody,
			EggmanMysticMelody,
			KnucklesMysticMelody,
			RougeMysticMelody,
			SonicAncientLight,
			MagicGloves,
			Sunglasses,
			ShadowAncientLight
		}
	}
}