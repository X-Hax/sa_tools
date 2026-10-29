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
	public class ParticleGenerator : ObjectDefinition
	{
		protected FVF_PositionTextured[] Vertices = new FVF_PositionTextured[4];
		protected Mesh particleMesh;
		protected BasicAttach particleAttach;
		protected NJS_OBJECT object_particle;
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
			particleAttach = new BasicAttach(vertices.ToArray(), normals.ToArray(), meshes, new List<NJS_MATERIAL>());
			return particleAttach;
		}
		public void CreateMesh()
		{
			BasicAttach att = GenerateAttach(Vertices);
			att.ProcessVertexData();
			Mesh meshlist = att.CreateD3DMesh();
			Bounds = SharpDX.BoundingSphere.Merge(Bounds.ToSharpDX(), att.Bounds.ToSharpDX()).ToSAModel();
			att.Bounds = Bounds;
			particleMesh = meshlist;
		}

		protected NJS_TEXLIST texlist;
		public override void Init(ObjectData data, string name)
		{
			CreateMesh();
			object_particle = new()
			{
				Attach = particleAttach
			};
			string[] names = new string[1];
			names[0] = "lens01";
			//texlist = new NJS_TEXLIST { TextureNames = names, NumTextures = 1 };

		}
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{ 
			HitResult result = HitResult.NoHit;
			transform.Push();
			{
				transform.NJTranslate(item.Position);
				transform.NJScale(item.Scale.Z, item.Scale.Z, 1F);
				result = HitResult.Min(result, particleMesh.CheckHit(Near, Far, Viewport, Projection, View, transform));
			}
			transform.Pop();

			return result;
		}
		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			int texID = 0;
			switch (item.Rotation.X & 0xFF)
			{
				case 0:
				default:
					texID = 111;
					break;
				case 1:
					texID = 21;
					break;
				case 2:
					texID = 62;
					break;
				case 3:
				case 6:
				case 12:
				case 14:
					texID = 36;
					break;
				case 4:
					texID = 87;
					break;
				case 5:
					texID = 95;
					break;
				case 7:
				case 13:
				case 17:
					texID = 103;
					break;
				case 8:
					texID = 119;
					break;
				case 9:
				case 10:
				case 15:
					texID = 82;
					break;
				case 11:
					texID = 52;
					break;
			}
			var scalefromdiff = Math.Max(item.Position.X - camera.Position.X, item.Position.Y - camera.Position.Y);
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (camera.Yaw != 0)
				transform.NJRotateY(camera.Yaw);
			if (camera.Pitch != 0)
				transform.NJRotateX(camera.Pitch);
			transform.NJScale(item.Scale.Z, item.Scale.Z, 1F);
			result.Add(new RenderInfo(particleMesh,
			0,
			transform.Top,
			TranslucentMaterial,
			(item.Rotation.X & 0xFF) != 16 ? ObjectHelper.GetTextures("efftex_common", null, dev)[texID] : ObjectHelper.GetTextures("objtex_common", null, dev)[55],
			dev.GetRenderState<FillMode>(RenderState.FillMode), new BoundingSphere(item.Position.X, item.Position.Y, item.Position.Z, item.Scale.Z)));

			if (item.Selected)
			{
				result.Add(new RenderInfo(particleMesh,
				0,
				transform.Top,
				OpaqueMaterial,
				null,
				FillMode.Wireframe, new BoundingSphere(item.Position.X, item.Position.Y, item.Position.Z, item.Scale.Z)));
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
				//transform.NJRotateY(item.Rotation.Y);
				transform.NJScale(item.Scale.Z, item.Scale.Z, 1F);
				result.Add(new ModelTransform(object_particle, transform.Top));
			}
			transform.Pop();

			return result;
		}
		public override BoundingSphere GetBounds(SETItem item)
		{
			var scalefromdiff = Math.Max(item.Position.X - specialcam.Position.X, item.Position.Y - specialcam.Position.Y);
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.ToVector3());
			//transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(item.Scale.Z, item.Scale.Z, 1F);
			return new BoundingSphere(item.Position.X, item.Position.Y, item.Position.Z, item.Scale.Z);
		}
		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Y Velocity", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Spread", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Scale", typeof(float), "Extended", null, null, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v),
			new PropertySpec("Particle Spawn Rate", typeof(float), "Extended", null, null, (o) => (o.Rotation.X >> 12) & 0xFF,
				(o, v) => { o.Rotation.X &= 0x00FF; o.Rotation.X |= (byte)v << 12; }),
			new PropertySpec("Particle Type", typeof(ParticleGeneratorType), "Extended", null, null, (o) => o.Rotation.X & 0xFF,
				(o, v) => { o.Rotation.X &= 0xFF00; o.Rotation.X |= (byte)(ParticleGeneratorType)v; }),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Particle Generator"; } }
	}
	public enum ParticleGeneratorType
	{
		WaterSplash,
		Explosion,
		Sparkle,
		Smoke,
		MovingSparkle,
		RocketExhaust,
		SteamPuff,
		Exhaust,
		Dirt,
		LineSpark,
		Spark,
		FlamePuff,
		DustPuff,
		Snow,
		SmokeCloud,
		SparkDown,
		Test,
		PoisonSmoke
	}
}
