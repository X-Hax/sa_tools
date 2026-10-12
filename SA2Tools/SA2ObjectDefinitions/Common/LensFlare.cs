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
	public class LensFlare : ObjectDefinition
	{
		protected FVF_PositionTextured[] Vertices = new FVF_PositionTextured[4];
		protected Mesh sunMesh;
		protected BasicAttach sunAttach;
		protected NJS_OBJECT object_sun;
		protected BoundingSphere Bounds = new();
		protected EditorCamera specialcam = new EditorCamera(100000F);

		public static readonly NJS_MATERIAL OpaqueMaterial = new()
		{
			IgnoreLighting = true,
			SourceAlpha = AlphaInstruction.SourceAlpha,
			DestinationAlpha = AlphaInstruction.InverseSourceAlpha,
			UseAlpha = false
		};

		public static readonly NJS_MATERIAL TranslucentMaterial = new()
		{
			IgnoreLighting = true,
			SourceAlpha = AlphaInstruction.SourceAlpha,
			DestinationAlpha = AlphaInstruction.InverseSourceAlpha,
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
			for (int f = 0; f < fvf.Length; f++)
			{
				vertices.Add(new Vertex(fvf[f].Position.X, fvf[f].Position.Y, fvf[f].Position.Z));
				normals.Add(new Vertex(0, 0, 0));
			}
			sunAttach = new BasicAttach(vertices.ToArray(), normals.ToArray(), meshes, new List<NJS_MATERIAL>());
			return sunAttach;
		}
		public void CreateMesh()
		{
			BasicAttach att = GenerateAttach(Vertices);
			att.ProcessVertexData();
			Mesh meshlist = att.CreateD3DMesh();
			Bounds = SharpDX.BoundingSphere.Merge(Bounds.ToSharpDX(), att.Bounds.ToSharpDX()).ToSAModel();
			att.Bounds = Bounds;
			sunMesh = meshlist;
		}

		protected NJS_TEXLIST texlist;
		public override void Init(ObjectData data, string name)
		{
			CreateMesh();
			object_sun = new()
			{
				Attach = sunAttach
			};
			string[] names = new string[1];
			names[0] = "lens01";
			//texlist = new NJS_TEXLIST { TextureNames = names, NumTextures = 1 };

		}
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{ 
			HitResult result = HitResult.NoHit;
			var scalefromdiff = Math.Max(item.Position.X - specialcam.Position.X, item.Position.Y - specialcam.Position.Y);
			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJScale(scalefromdiff, scalefromdiff, 1F);
				result = HitResult.Min(result, sunMesh.CheckHit(Near, Far, Viewport, Projection, View, transform));
			}
			transform.Pop();

			return result;
		}
		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			var scalefromdiff = Math.Max(item.Position.X - camera.Position.X, item.Position.Y - camera.Position.Y);
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (camera.Yaw != 0)
				transform.NJRotateY(camera.Yaw);
			if (camera.Pitch != 0)
				transform.NJRotateX(camera.Pitch);
			transform.NJScale(scalefromdiff / 20, scalefromdiff / 20, 1F);
			result.Add(new RenderInfo(sunMesh,
			0,
			transform.Top,
			TranslucentMaterial,
			ObjectHelper.GetTextures("efftex_common", null, dev)[66],
			dev.GetRenderState<FillMode>(RenderState.FillMode), new BoundingSphere(item.Position.X, item.Position.Y, item.Position.Z, 1F)));

			if (item.Selected)
			{
				result.Add(new RenderInfo(sunMesh,
				0,
				transform.Top,
				OpaqueMaterial,
				null,
				FillMode.Wireframe, new BoundingSphere(item.Position.X, item.Position.Y, item.Position.Z, 1F)));
			}
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();

			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJRotateY(item.Rotation.Y);
				transform.NJScale(20F, 20F, 1F);
				result.Add(new ModelTransform(object_sun, transform.Top));
			}
			transform.Pop();

			return result;
		}
		public override BoundingSphere GetBounds(SETItem item)
		{
			var scalefromdiff = Math.Max(item.Position.X - specialcam.Position.X, item.Position.Y - specialcam.Position.Y);
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(scalefromdiff, scalefromdiff, 1F);
			return new BoundingSphere(item.Position.X, item.Position.Y, item.Position.Z, 1F);
		}
		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}
		public override string Name { get { return "Sun with Lens Flare"; } }
	}
}
