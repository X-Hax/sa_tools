using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using SharpDX;
using SharpDX.Direct3D9;
using SplitTools;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SA2ObjectDefinitions.Common
{
	public abstract class MysticMelody : ObjectDefinition
	{
		protected NJS_OBJECT model;
		protected Mesh[] meshes;
		protected NJS_OBJECT endpoint;
		protected Mesh[] meshendpoint;
		protected NJS_TEXLIST texarr;
		protected Texture[] texs;
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

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, 0, item.Rotation.Y - 0x8000, item.Rotation.Z);

			return matrix;
		}

		public override void SetOrientation(SETItem item, Vertex direction)
		{
			int x; int z; direction.GetRotation(out x, out z);
			item.Rotation.X = x + 0x4000;
			item.Rotation.Z = -z;
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Shrine ID", typeof(float), "Extended", null, null, (o) => o.Rotation.X, (o, v) => o.Rotation.X = (int)v),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
	
	public class Shrine : MysticMelody
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("object/OBJECT_KNUDAI.sa2mdl");
			meshes = ObjectHelper.GetMeshes(model);
			texarr = NJS_TEXLIST.Load("object/tls/KNUDAI.satex");
		}
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
			transform.NJRotateY(item.Rotation.Y);
			HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			if (texs == null)
				texs = ObjectHelper.GetTextures("objtex_common", texarr, dev);
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			return ObjectHelper.GetModelBounds(model, transform);
		}
		public override string Name { get { return "Mystic Melody Shrine"; } }
	}
	
	public class Platform : MysticMelody
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("object/OBJECT_KDASIBA.sa2mdl");
			meshes = ObjectHelper.GetMeshes(model);
			texarr = NJS_TEXLIST.Load("object/tls/KDASIBA.satex");
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			if (texs == null)
				texs = ObjectHelper.GetTextures("objtex_common", texarr, dev);
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			return ObjectHelper.GetModelBounds(model, transform);
		}

		public override string Name { get { return "Mystic Melody Platform"; } }
	}
	
	public class Door : MysticMelody
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("object/OBJECT_KDDOOR.sa2mdl");
			meshes = ObjectHelper.GetMeshes(model);
			texarr = NJS_TEXLIST.Load("object/tls/KDDOOR.satex");
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			if (texs == null)
				texs = ObjectHelper.GetTextures("objtex_common", texarr, dev);
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			return ObjectHelper.GetModelBounds(model, transform);
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Shrine ID", typeof(byte), "Extended", null, null, (o) => o.Rotation.X & 0xFF,
			(o, v) => { o.Rotation.X &= 0xFF00; o.Rotation.X |= (byte)v; }),
			new PropertySpec("Door Orientation (Debug)", typeof(MysticDoorOpen), "Extended", null, null, (o) => (MysticDoorOpen)(o.Rotation.X >> 8),
			(o, v) => { o.Rotation.X &= 0xFF; o.Rotation.X |= (byte)v << 8; }),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Mystic Melody Door (Vertical)"; } }
		public enum MysticDoorOpen : byte
		{
			Upward,
			LeftToRight,
			Upward2,
			RightToLeft
		}
	}
	public class Warp : MysticMelody
	{
		public override void Init(ObjectData data, string name)
		{
			CreateMesh();
			spriteObject = new()
			{
				Attach = spriteAttach
			};
			model = spriteObject;
			meshes = ObjectHelper.GetMeshes(model);
			endpoint = ObjectHelper.LoadModel("OBJECT/CubeCollision.sa2mdl");
			meshendpoint = ObjectHelper.GetMeshes(endpoint);
			string[] texnames = new string[1];
			texnames[0] = "sikake_33_128";
			NJS_TEXLIST ntl = NJS_TEXLIST.Load("object/tls/KNUDAI.satex");
			ntl.TextureNames = texnames;
			ntl.NumTextures = 1;
			texarr = ntl;
		}
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJScale(5F, 5F, 1F);
			result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Scale);
			transform.NJScale(10F, 10f, 10f);
			result = HitResult.Min(result, endpoint.CheckHit(Near, Far, Viewport, Projection, View, transform, meshendpoint));
			transform.Pop();
			return result;
		}
		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			if (texs == null)
				texs = ObjectHelper.GetTextures("objtex_common", texarr, dev);
			transform.Push();
			transform.NJTranslate(item.Position);
			if (camera.Yaw != 0)
				transform.NJRotateY(camera.Yaw);
			if (camera.Pitch != 0)
				transform.NJRotateX(camera.Pitch);
			transform.NJScale(5F, 5F, 1F);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
			{
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			}
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Scale);
			transform.NJScale(10F, 10f, 10f);
			result.AddRange(endpoint.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, meshendpoint, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
			{
				result.AddRange(endpoint.DrawModelTreeInvert(transform, meshendpoint));
			}
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJScale(5F, 5F, 1F);
			BoundingSphere warpbounds = ObjectHelper.GetModelBounds(model, transform);
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Scale);
			transform.NJScale(10F, 10F, 10F);
			BoundingSphere endbounds = ObjectHelper.GetModelBounds(endpoint, transform);
			transform.Pop();
			return SAModel.Direct3D.Extensions.Merge(warpbounds, endbounds);
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Shrine ID", typeof(byte), "Extended", null, null, (o) => o.Rotation.X & 0xFF,
			(o, v) => { o.Rotation.X &= 0xFF00; o.Rotation.X |= (byte)v; }),
			new PropertySpec("Warp X Endpoint", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Warp Y Endpoint", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Warp Z Endpoint", typeof(float), "Extended", null, null, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override string Name { get { return "Mystic Melody Warp"; } }
	}
}