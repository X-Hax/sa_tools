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
using System.Linq;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SA2ObjectDefinitions.Common
{
	public abstract class MissileBase : ObjectDefinition
	{
		protected NJS_OBJECT[] models;
		protected Mesh[][] meshes;
		protected NJS_OBJECT endpoint;
		protected Mesh[] endpointmesh;
		protected NJS_TEXLIST[] texlists;
		protected Texture[][] texs;
		[NonSerialized]
		protected FVF_PositionColored[] vertices;
		[NonSerialized]
		protected short[] faceIndeces;

		[NonSerialized]
		protected BoundingSphere pathbounds;
		protected Mesh pathmesh;

		protected static NJS_MATERIAL SelectedMaterial { get; set; }
		protected static NJS_MATERIAL UnSelectedMaterial { get; set; }

		[NonSerialized]
		protected static Mesh vertexHandleMesh;
		[NonSerialized]
		protected Sprite textSprite;

		public void DefineSpline(Vertex rocketpos, Vertex endpoint, Device dev = null)
		{
			List<FVF_PositionColored> vertList = new List<FVF_PositionColored>();
			List<short> faceIndexList = new List<short>();

			float splineMeshRadius = 2F;

			Vector3 up = new Vector3(0.5F, 1, 0);

			#region Segment vert/face creation
			short highestFaceIndex = 0;

			Vector3 thisKnot = rocketpos.ToVector3();
			Vector3 nextKnot = endpoint.ToVector3();

			Vector3 directionToNextKnot = Vector3.Normalize(nextKnot - thisKnot);
			Vector3 perpendicularDirection = Vector3.Cross(directionToNextKnot, up);

			// verts for knot 1
			FVF_PositionColored vert1_1; // top vert 1 (0)
			FVF_PositionColored vert1_2; // top vert 2 (1)
			FVF_PositionColored vert1_3; // bottom vert 1 (2)
			FVF_PositionColored vert1_4; // bottom vert 2 (3)

			// verts for knot 2
			FVF_PositionColored vert2_1; // top vert 1 (4)
			FVF_PositionColored vert2_2; // top vert 2 (5)
			FVF_PositionColored vert2_3; // bottom vert 1 (6)
			FVF_PositionColored vert2_4; // bottom vert 2 (7)

			// move top verts
			vert1_1 = new FVF_PositionColored((thisKnot + (perpendicularDirection * splineMeshRadius)), System.Drawing.Color.White);
			vert1_2 = new FVF_PositionColored((thisKnot + (perpendicularDirection * (splineMeshRadius * -1))), System.Drawing.Color.White);

			vert2_1 = new FVF_PositionColored((nextKnot + (perpendicularDirection * splineMeshRadius)), System.Drawing.Color.White);
			vert2_2 = new FVF_PositionColored((nextKnot + (perpendicularDirection * (splineMeshRadius * -1))), System.Drawing.Color.White);

			// move bottom verts
			vert1_3 = new FVF_PositionColored(vert1_1.Position - (up * splineMeshRadius), System.Drawing.Color.White);
			vert1_4 = new FVF_PositionColored(vert1_2.Position - (up * splineMeshRadius), System.Drawing.Color.White);

			vert2_3 = new FVF_PositionColored(vert2_1.Position - (up * splineMeshRadius), System.Drawing.Color.White);
			vert2_4 = new FVF_PositionColored(vert2_2.Position - (up * splineMeshRadius), System.Drawing.Color.White);

			List<short> thisKnotFaceIndexes = new List<short>
				{
					// far side
					4,0,6,
					6,2,0,

					// bottom
					6,2,3,
					3,7,6,

					// our side
					7,3,1,
					7,5,1,

					// top
					1,5,4,
					4,0,1
				};

			for (int faceIndx = 0; faceIndx < thisKnotFaceIndexes.Count(); faceIndx++)
			{
				thisKnotFaceIndexes[faceIndx] += (short)vertList.Count(); // this is the wrong approach because it's the verts we're indexing, not the faces!
				if (thisKnotFaceIndexes[faceIndx] > highestFaceIndex) highestFaceIndex = thisKnotFaceIndexes[faceIndx];
			}

			// add verts to vert list and faces to face list
			vertList.Add(vert1_1);
			vertList.Add(vert1_2);
			vertList.Add(vert1_3);
			vertList.Add(vert1_4);
			vertList.Add(vert2_1);
			vertList.Add(vert2_2);
			vertList.Add(vert2_3);
			vertList.Add(vert2_4);

			faceIndexList.AddRange(thisKnotFaceIndexes);
			#endregion

			vertices = vertList.ToArray();
			faceIndeces = faceIndexList.ToArray();

			// build bounding sphere
			pathbounds = SharpDX.BoundingSphere.FromPoints(vertices.Select(a => a.Position).ToArray()).ToSAModel();

			// build actual mesh from face index array and vbuf
			//if (pathmesh == null)
			pathmesh = new Mesh<FVF_PositionColored>(vertices, new short[][] { faceIndeces });

			// create a vertexHandle
			if (vertexHandleMesh == null)
				vertexHandleMesh = Mesh.Box(1, 1, 1);

			if (vertexHandleMesh == null) vertexHandleMesh = Mesh.Box(1, 1, 1);

			if (dev != null)
				textSprite = new Sprite(dev); // todo: do we really have to create this so often? Look into storing a cache list statically?
		}
		public override void Init(ObjectData data, string name)
		{
			SelectedMaterial = new NJS_MATERIAL
			{
				DiffuseColor = System.Drawing.Color.White,
				SpecularColor = System.Drawing.Color.Black,
				UseAlpha = false,
				DoubleSided = true,
				Exponent = 10,
				IgnoreSpecular = false,
				UseTexture = false,
				IgnoreLighting = true
			};

			UnSelectedMaterial = new NJS_MATERIAL
			{
				DiffuseColor = System.Drawing.Color.Maroon,
				SpecularColor = System.Drawing.Color.Black,
				UseAlpha = false,
				DoubleSided = true,
				Exponent = 10,
				IgnoreSpecular = false,
				UseTexture = false,
				IgnoreLighting = true
			};
			models = new NJS_OBJECT[4];
			meshes = new Mesh[4][];
			texlists = new NJS_TEXLIST[4];
			texs = new Texture[4][];

			models[0] = ObjectHelper.LoadModel("object/GC/OBJECT_MISSILEPANEL.sa2bmdl");
			models[1] = ObjectHelper.LoadModel("object/OBJECT_MISSILESWITCH.sa2mdl");
			models[2] = ObjectHelper.LoadModel("object/OBJECT_MISSILE.sa2mdl");
			models[3] = ObjectHelper.LoadModel("object/OBJECT_MISSILETIP.sa2mdl");

			endpoint = ObjectHelper.LoadModel("OBJECT/SmallAnimalBubbleCollision.sa2mdl");
			endpointmesh = ObjectHelper.GetMeshes(endpoint);

			texlists[0] = NJS_TEXLIST.Load("object/tls/MISSILEPANEL.satex");
			texlists[1] = NJS_TEXLIST.Load("object/tls/MISSILESWITCH.satex");
			texlists[2] = NJS_TEXLIST.Load("object/tls/MISSILE.satex");
			texlists[3] = NJS_TEXLIST.Load("object/tls/MISSILETIP.satex");

			for (int i = 0; i < 4; i++)
				meshes[i] = ObjectHelper.GetMeshes(models[i]);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			int inverse = (item.Rotation.X >> 4) & 0xF;
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			if (inverse != 0)
				transform.NJRotateZ(0x8000);
			result = HitResult.Min(result, models[0].CheckHit(Near, Far, Viewport, Projection, View, transform, meshes[0]));
			result = HitResult.Min(result, models[1].CheckHit(Near, Far, Viewport, Projection, View, transform, meshes[1]));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (inverse != 0)
			{
				transform.NJTranslate(0, -9.0F, 0);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
				transform.NJRotateZ(0x8000);
			}
			else
			{
				transform.NJTranslate(0, 9.0F, 0);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
			}
			result = HitResult.Min(result, models[2].CheckHit(Near, Far, Viewport, Projection, View, transform, meshes[2]));
			result = HitResult.Min(result, models[3].CheckHit(Near, Far, Viewport, Projection, View, transform, meshes[3]));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Scale);
			transform.NJScale(0.8F, 0.8f, 0.8F);
			result = HitResult.Min(result, endpoint.CheckHit(Near, Far, Viewport, Projection, View, transform, endpointmesh));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			int inverse = (item.Rotation.X >> 4) & 0xF;
			for (int i = 0; i < 4; i++)
			{
				if (texs[i] == null)
					texs[i] = ObjectHelper.GetTextures("objtex_common", texlists[i], dev);
			}
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			if (inverse != 0)
				transform.NJRotateZ(0x8000);
			result.AddRange(models[0].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs[0], meshes[0], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result.AddRange(models[1].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs[1], meshes[1], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
			{
				result.AddRange(models[0].DrawModelTreeInvert(transform, meshes[0]));
				result.AddRange(models[1].DrawModelTreeInvert(transform, meshes[1]));
			}
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (inverse != 0)
			{
				transform.NJTranslate(0, -9.0F, 0);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
				transform.NJRotateZ(0x8000);
			}
			else
			{
				transform.NJTranslate(0, 9.0F, 0);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
			}
			result.AddRange(models[2].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs[2], meshes[2], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			result.AddRange(models[3].DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs[3], meshes[3], EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
			{
				result.AddRange(models[2].DrawModelTreeInvert(transform, meshes[2]));
				result.AddRange(models[3].DrawModelTreeInvert(transform, meshes[3]));
			}
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Scale);
			transform.NJScale(0.8F, 0.8f, 0.8F);
			result.AddRange(endpoint.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, null, endpointmesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(endpoint.DrawModelTreeInvert(transform, endpointmesh));
			transform.Pop();
			DefineSpline(new Vertex(item.Position.X, inverse != 0 ? item.Position.Y - 9F : item.Position.Y + 9F, item.Position.Z), item.Scale, dev);
			RenderInfo outputInfo = new RenderInfo(pathmesh, 0, transform.Top, (item.Selected) ? SelectedMaterial : UnSelectedMaterial, null, EditorOptions.RenderFillMode, pathbounds);
			result.Add(outputInfo);
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			int inverse = (item.Rotation.X >> 4) & 0xF;
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			if (inverse != 0)
				transform.NJRotateZ(0x8000);
			result.Add(new ModelTransform(models[0], transform.Top));
			result.Add(new ModelTransform(models[1], transform.Top));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (inverse != 0)
			{
				transform.NJTranslate(0, -9.0F, 0);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
				transform.NJRotateZ(0x8000);
			}
			else
			{
				transform.NJTranslate(0, 9.0F, 0);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
			}
			result.Add(new ModelTransform(models[2], transform.Top));
			result.Add(new ModelTransform(models[3], transform.Top));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Scale);
			transform.NJScale(0.8F, 0.8f, 0.8F);
			result.Add(new ModelTransform(endpoint, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			int inverse = (item.Rotation.X >> 4) & 0xF;
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			if (inverse != 0)
				transform.NJRotateZ(0x8000);
			BoundingSphere panelbounds = ObjectHelper.GetModelBounds(models[0], transform);
			panelbounds = SAModel.Direct3D.Extensions.Merge(panelbounds, ObjectHelper.GetModelBounds(models[1], transform));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (inverse != 0)
			{
				transform.NJTranslate(0, -9.0F, 0);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
				transform.NJRotateZ(0x8000);
			}
			else
			{
				transform.NJTranslate(0, 9.0F, 0);
				if (item.Rotation.Y != 0)
					transform.NJRotateY(item.Rotation.Y);
			}
			BoundingSphere missilebounds = ObjectHelper.GetModelBounds(models[2], transform);
			missilebounds = SAModel.Direct3D.Extensions.Merge(missilebounds, ObjectHelper.GetModelBounds(models[3], transform));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(item.Scale);
			transform.NJScale(0.8F, 0.8f, 0.8F);
			BoundingSphere pointbounds = ObjectHelper.GetModelBounds(endpoint, transform, 200F);
			transform.Pop();
			BoundingSphere objectbounds = SAModel.Direct3D.Extensions.Merge(panelbounds, missilebounds);
			return SAModel.Direct3D.Extensions.Merge(pointbounds, objectbounds);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, 0, item.Rotation.Y - 0x8000, 0);
			return matrix;
		}

		//public override void SetOrientation(SETItem item, Vertex direction)
		//{
		//	int x; int z; direction.GetRotation(out x, out z);
		//	item.Rotation.X = x + 0x4000;
		//	item.Rotation.Z = -z;
		//}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Invert Platform", typeof(bool), "Extended", null, null, (o) => Convert.ToBoolean((o.Rotation.X >> 4) & 0xF),
				(o, v) => { o.Rotation.X &= 0xFF0F; o.Rotation.X |= Convert.ToByte((bool)v) << 4; }),
			new PropertySpec("Missile Type", typeof(MissileType), "Extended", null, null, (o) => (MissileType)(o.Rotation.X & 0xF),
				(o, v) => { o.Rotation.X &= 0xFFF0; o.Rotation.X |= (byte)(MissileType)v; }),
			new PropertySpec("X Endpoint", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Y Endpoint", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Z Endpoint", typeof(float), "Extended", null, null, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v),
			new PropertySpec("Missile ID", typeof(int), "Extended", "If there is no compatible item with this ID, the missile will not appear.", null, (o) => o.Rotation.Z, (o, v) => o.Rotation.Z = (int)v)
		};

		public enum MissileType
		{
			Normal,
			SingleMissile,
			SwitchActivation,
			EnemyActivation
		}

		public override PropertySpec[] CustomProperties { get { return customProperties; } }

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
	public class RocketMissile : MissileBase
	{
		public override string Name { get { return "Missile Platform"; } }
	}
	public class RMissile : MissileBase
	{
		public override string Name { get { return "Final Missile Station"; } }
	}
}