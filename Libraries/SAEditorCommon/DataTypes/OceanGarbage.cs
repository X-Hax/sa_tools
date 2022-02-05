using SAModel.Direct3D;
using System.Collections.Generic;

// Class for rendering SADX style water (Emerald Coast, Adventure Fields, Chao Gardens)
namespace SAModel.SAEditorCommon
{
	public static class SADXOceanData
	{
		public static List<WaterSurfaceData> WaterSurfaces;
		public static readonly NJS_MATERIAL Material = new()
		{
			IgnoreLighting = true,
			SourceAlpha = AlphaInstruction.SourceAlpha,
			DestinationAlpha = AlphaInstruction.One
		};

		public static void Initialize()
		{
			WaterSurfaces = new List<WaterSurfaceData>();
		}

		public static void InitWaterSurface(int surfaceID)
		{
			uint primitiveIndex = 0; // r9
			float stepZ = 0; // fp13
			float sizeX; // fp10
			float sizeZ; // fp11
			if (WaterSurfaces[surfaceID].WrapZ == 0)
				return;
			do
			{
				sizeX = WaterSurfaces[surfaceID].WrapXZ;
				sizeZ = WaterSurfaces[surfaceID].WrapXZ + stepZ;
				WaterSurfaces[surfaceID].Primitives[primitiveIndex].Vertices[0] = new FVF_PositionTextured(new SharpDX.Vector3(0, 0, stepZ), new SharpDX.Vector2(0, 0));
				WaterSurfaces[surfaceID].Primitives[primitiveIndex].Vertices[1] = new FVF_PositionTextured(new SharpDX.Vector3(sizeX, 0, stepZ), new SharpDX.Vector2(0, 1.0f));
				WaterSurfaces[surfaceID].Primitives[primitiveIndex].Vertices[2] = new FVF_PositionTextured(new SharpDX.Vector3(0, 0, sizeZ), new SharpDX.Vector2(1.0f, 0.0f));
				WaterSurfaces[surfaceID].Primitives[primitiveIndex].Vertices[3] = new FVF_PositionTextured(new SharpDX.Vector3(sizeX, 0, sizeZ), new SharpDX.Vector2(1.0f, 1.0f));
				++primitiveIndex;
				stepZ = sizeZ;
			}
			while (primitiveIndex < WaterSurfaces[surfaceID].WrapZ);
			WaterSurfaces[surfaceID].CreateMeshes();
		}

		private static BasicAttach GenerateAttach(FVF_PositionTextured[] fvf)
		{
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
			return new BasicAttach(vertices.ToArray(), normals.ToArray(), meshes, new List<NJS_MATERIAL>());
		}

		public class WaterPrimitive
		{
			public FVF_PositionTextured[] Vertices = new FVF_PositionTextured[4];
		}

		public class WaterSurfaceData
		{
			public Vertex Center;
			public byte WrapX;
			public byte WrapZ;
			public float WrapXZ;
			public ushort TextureWaves;
			public ushort TextureSea;
			public WaterPrimitive[] Primitives = new WaterPrimitive[35];
			public BoundingSphere Bounds = new();
			public Mesh[] Meshes;

			public WaterSurfaceData()
			{
				Center = new Vertex(0, 0, 0);
				Primitives = new WaterPrimitive[35];
				for (int i = 0; i < 35; i++)
					Primitives[i] = new WaterPrimitive();
			}

			public void CreateMeshes()
			{
				List<Mesh> meshlist = new List<Mesh>();
				for (int i = 0; i < WrapZ; i++)
				{
					BasicAttach att = GenerateAttach(Primitives[i].Vertices);
					att.ProcessVertexData();
					meshlist.Add(att.CreateD3DMesh());
					Bounds = SharpDX.BoundingSphere.Merge(Bounds.ToSharpDX(), att.Bounds.ToSharpDX()).ToSAModel(); // Awkward...
				}
				Meshes = meshlist.ToArray();
			}
		};
	}
}