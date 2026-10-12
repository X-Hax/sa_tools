using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using System;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;
using SplitTools;
using SAModel.SAEditorCommon;

namespace SA2ObjectDefinitions.CityEscape
{
	public abstract class CityEscapePosterBase : ObjectDefinition
	{
		protected FVF_PositionTextured[] Vertices = new FVF_PositionTextured[4];
		protected Mesh posterMesh;
		protected BasicAttach posterAttach;
		protected NJS_OBJECT object_poster;
		protected Mesh[] postermeshes;
		protected BoundingSphere Bounds = new();
		internal string[] posternames =
		{
			"miu128_ce005",
			"miu128_ce008",
			"miu128_ce009",
			"miu128_ce010",
			"miu128_ce011",
			"miu128_ce013",
			"miu128_ce014",
			"miu128_ce015",
			"miu64_ce051",
			"nights1"
		};

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
			posterAttach = new BasicAttach(vertices.ToArray(), normals.ToArray(), meshes, new List<NJS_MATERIAL>());
			return posterAttach;
		}
		public void CreateMesh()
		{
			BasicAttach att = GenerateAttach(Vertices);
			att.ProcessVertexData();
			Mesh meshlist = att.CreateD3DMesh();
			Bounds = SharpDX.BoundingSphere.Merge(Bounds.ToSharpDX(), att.Bounds.ToSharpDX()).ToSAModel();
			att.Bounds = Bounds;
			posterMesh = meshlist;
		}

		protected NJS_TEXLIST texlist;
		public HitResult CheckHitNormal(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			{
				transform.NJTranslate(item.Position);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
				transform.NJScale(item.Scale.X, item.Scale.Y, 1F);
				result = HitResult.Min(result, posterMesh.CheckHit(Near, Far, Viewport, Projection, View, transform));
			}
			transform.Pop();

			return result;
		}
		public List<RenderInfo> RenderNormal(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			texlist.TextureNames[0] = posternames[Math.Min((int)item.Scale.Z, 9)];
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(item.Scale.X, item.Scale.Y, 1F);
			result.Add(new RenderInfo(posterMesh,
			0,
			transform.Top,
			item.Scale.Z == 8F ? TranslucentMaterial : OpaqueMaterial,
			ObjectHelper.GetTextures(new List<string>(["landtx13", "objtex_stg13"]), texlist, dev)[0],
			dev.GetRenderState<FillMode>(RenderState.FillMode), new BoundingSphere(item.Position.X, item.Position.Y, item.Position.Z, Math.Max(item.Scale.X, item.Scale.Y))));

			if (item.Selected)
			{
				result.Add(new RenderInfo(posterMesh,
				0,
				transform.Top,
				OpaqueMaterial,
				null,
				FillMode.Wireframe, new BoundingSphere(item.Position.X, item.Position.Y, item.Position.Z, Math.Max(item.Scale.X, item.Scale.Y))));
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
				transform.NJScale(item.Scale.X, item.Scale.Y, 1);
				result.Add(new ModelTransform(object_poster, transform.Top));
			}
			transform.Pop();

			return result;
		}
		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(item.Scale.X, item.Scale.Y, 1);
			return new BoundingSphere(item.Position.X, item.Position.Y, item.Position.Z, Math.Max(item.Scale.X, item.Scale.Y));
		}
		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}
	}

	public class Poster : CityEscapePosterBase
	{
		public override void Init(ObjectData data, string name)
		{
			CreateMesh();
			object_poster = new()
			{
				Attach = posterAttach
			};
			texlist = NJS_TEXLIST.Load("stg13_cityescape/tls/POSTER.satex");
		}
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			return CheckHitNormal(item, Near, Far, Viewport, Projection, View, transform);
		}
		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			return RenderNormal(item, dev, camera, transform);
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Poster Type", typeof(PosterType), "Extended", null, null, (o) => (PosterType)(Math.Min((int)o.Scale.Z, 9)), (o, v) => o.Scale.Z = (float)(PosterType)v),
			new PropertySpec("X Scale", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Y Scale", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Poster"; } }
	}

	public class PosterM : CityEscapePosterBase
	{
		public override void Init(ObjectData data, string name)
		{
			CreateMesh();
			object_poster = new()
			{
				Attach = posterAttach
			};
			texlist = NJS_TEXLIST.Load("stg13_cityescape/tls/POSTER.satex");
		}
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			return CheckHitNormal(item, Near, Far, Viewport, Projection, View, transform);
		}
		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			return RenderNormal(item, dev, camera, transform);
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Poster Type", typeof(PosterType), "Extended", null, null, (o) => (PosterType)(Math.Min((int)o.Scale.Z, 9)), (o, v) => o.Scale.Z = (float)(PosterType)v),
			new PropertySpec("X Scale", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Y Scale", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Poster (Type M)"; } }
	}

	public class Poster3 : CityEscapePosterBase
	{
		public override void Init(ObjectData data, string name)
		{
			CreateMesh();
			object_poster = new()
			{
				Attach = posterAttach
			};
			texlist = NJS_TEXLIST.Load("stg13_cityescape/tls/POSTER3.satex");
		}
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			{
				transform.NJTranslate(item.Position);
				if (item.Rotation.Z != 0)
					transform.NJRotateZ(item.Rotation.Z);
				if (item.Rotation.X != 0)
					transform.NJRotateX(item.Rotation.X);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
				transform.NJScale(item.Scale.X, item.Scale.Y, 1F);
				result = HitResult.Min(result, posterMesh.CheckHit(Near, Far, Viewport, Projection, View, transform));
			}
			transform.Pop();

			return result;
		}
		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			{
				transform.NJTranslate(item.Position);
				if (item.Rotation.Z != 0)
					transform.NJRotateZ(item.Rotation.Z);
				if (item.Rotation.X != 0)
					transform.NJRotateX(item.Rotation.X);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
				transform.NJScale(item.Scale.X, item.Scale.Y, 1F);
				result.Add(new RenderInfo(posterMesh,
					0,
					transform.Top,
					TranslucentMaterial,
					ObjectHelper.GetTextures(new List<string>(["landtx13", "objtex_stg13"]), texlist, dev)[0],
					dev.GetRenderState<FillMode>(RenderState.FillMode), new BoundingSphere(item.Position.X, item.Position.Y, item.Position.Z, Math.Max(item.Scale.X, item.Scale.Y))));

				if (item.Selected)
				{
					result.Add(new RenderInfo(posterMesh,
					0,
					transform.Top,
					TranslucentMaterial,
					null,
					FillMode.Wireframe, new BoundingSphere(item.Position.X, item.Position.Y, item.Position.Z, Math.Max(item.Scale.X, item.Scale.Y))));
				}
			}
			transform.Pop();

			return result;
		}
		public override string Name { get { return "Poster (Drop Shadow)"; } }
	}
	public enum PosterType
	{
		SonicTeam,
		TastyBurger,
		DigitalChoke,
		ChaosCola,
		SOAPSonicTeam,
		ChaoInSpace2,
		SA2HabitForming,
		ItemBoxExpo,
		PalmtreeShadow,
		NiGHTS
	}
}
