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

namespace SA2ObjectDefinitions.MetalHarbor
{
	public abstract class MHContBase : ObjectDefinition
	{
		protected NJS_OBJECT model;
		protected Mesh[] meshes;
		protected NJS_TEXLIST texarr;
		protected Texture[] texs;
		[NonSerialized]
		protected FVF_PositionColored[] vertices;
		[NonSerialized]
		protected short[] faceIndeces;

		[NonSerialized]
		protected BoundingSphere pathbounds;
		protected Mesh pathmesh;

		public static NJS_MATERIAL SelectedMaterial { get; set; }
		public static NJS_MATERIAL UnSelectedMaterial { get; set; }

		[NonSerialized]
		protected static Mesh vertexHandleMesh;
		[NonSerialized]
		protected Sprite textSprite;

		protected List<string> texpacks = [];

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

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
			transform.NJRotateY(item.Rotation.Y);
			result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
			transform.NJTranslate(0, 0, item.Scale.X);
			result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			if (texs == null)
				texs = ObjectHelper.GetTextures(texpacks, texarr, dev);
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			transform.NJTranslate(0, 0, item.Scale.X);
			if (item.Scale.X != 0)
			{
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
				DefineSpline(new Vertex(item.Position.X, item.Position.Y + model.Attach.Bounds.Center.Y, item.Position.Z), Vector3.TransformCoordinate(model.Attach.Bounds.Center.ToVector3(), transform.Top).ToVertex(), dev);
				RenderInfo outputInfo = new RenderInfo(pathmesh, 0, Matrix.Identity, (item.Selected) ? SelectedMaterial : UnSelectedMaterial, null, EditorOptions.RenderFillMode, pathbounds);
				result.Add(outputInfo);
			}
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
			transform.NJTranslate(0, 0, item.Scale.X);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(item.Position.ToVector3());
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			BoundingSphere basebounds =  ObjectHelper.GetModelBounds(model, transform);
			transform.NJTranslate(0, 0, item.Scale.X);
			BoundingSphere endbounds = ObjectHelper.GetModelBounds(model, transform);
			transform.Pop();
			return SAModel.Direct3D.Extensions.Merge(basebounds, endbounds);
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

		public override float DefaultXScale { get { return 0; } }

		public override float DefaultYScale { get { return 0; } }

		public override float DefaultZScale { get { return 0; } }
	}
	public class MHCont : MHContBase
	{
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
			model = ObjectHelper.LoadModel("stg10_metalharbor/models/GC/SmallContainer.sa2bmdl");
			meshes = ObjectHelper.GetMeshes(model);
			texarr = NJS_TEXLIST.Load("stg10_metalharbor/tls/SmallContainer.satex");
			texpacks.Add("objtex_stg10");
			texpacks.Add("landtx10");
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Wall Bump", typeof(bool), "Extended", "Determines if the box is treated as a wall that knocks the player back.", null, (o) => Convert.ToBoolean(o.Rotation.X % 2), (o, v) => o.Rotation.X = Convert.ToInt32((bool)v)),
			new PropertySpec("Displacement", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Movement Speed", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Moving Red Container"; } }
	}
	public class MHContB : MHContBase
	{
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
			model = ObjectHelper.LoadModel("stg10_metalharbor/models/GC/ContainerBase.sa2bmdl");
			meshes = ObjectHelper.GetMeshes(model);
			texarr = NJS_TEXLIST.Load("stg10_metalharbor/tls/ContainerBase.satex");
			texpacks.Add("objtex_stg10");
			texpacks.Add("landtx10");
		}
		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			if (texs == null)
				texs = ObjectHelper.GetTextures(texpacks, texarr, dev);
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			transform.NJTranslate(0, 0, item.Scale.X);
			if (item.Scale.X != 0)
			{
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
				DefineSpline(new Vertex(item.Position.X, item.Position.Y + model.Attach.Bounds.Center.Y + 5F, item.Position.Z), Vector3.TransformCoordinate(new Vertex(model.Attach.Bounds.Center.X, model.Attach.Bounds.Center.Y + 5F, model.Attach.Bounds.Center.Z).ToVector3(), transform.Top).ToVertex(), dev);
				RenderInfo outputInfo = new RenderInfo(pathmesh, 0, Matrix.Identity, (item.Selected) ? SelectedMaterial : UnSelectedMaterial, null, EditorOptions.RenderFillMode, pathbounds);
				result.Add(outputInfo);
			}
			transform.Pop();
			return result;
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Displacement", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Movement Speed", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Moving Container Lift"; } }
	}
	public class MHCont0 : MHContBase
	{
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
			model = ObjectHelper.LoadModel("stg10_metalharbor/models/GC/Box.sa2bmdl");
			meshes = ObjectHelper.GetMeshes(model);
			texarr = NJS_TEXLIST.Load("stg10_metalharbor/tls/Box.satex");
			texpacks.Add("objtex_stg10");
			texpacks.Add("landtx10");
		}
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
			transform.NJTranslate(item.Scale.X, 0, 0);
			result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			if (texs == null)
				texs = ObjectHelper.GetTextures(texpacks, texarr, dev);
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			transform.NJTranslate(item.Scale.X, 0, 0);
			if (item.Scale.X != 0)
			{
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
				DefineSpline(Vector3.TransformCoordinate(new Vertex(model.Attach.Bounds.Center.X - item.Scale.X, model.Attach.Bounds.Center.Y, model.Attach.Bounds.Center.Z).ToVector3(), transform.Top).ToVertex(), Vector3.TransformCoordinate(model.Attach.Bounds.Center.ToVector3(), transform.Top).ToVertex(), dev);
				RenderInfo outputInfo = new RenderInfo(pathmesh, 0, Matrix.Identity, (item.Selected) ? SelectedMaterial : UnSelectedMaterial, null, EditorOptions.RenderFillMode, pathbounds);
				result.Add(outputInfo);
			}
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
			transform.NJTranslate(item.Scale.X, 0, 0);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(item.Position.ToVector3());
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			BoundingSphere basebounds = ObjectHelper.GetModelBounds(model, transform);
			transform.NJTranslate(item.Scale.X, 0, 0);
			BoundingSphere endbounds = ObjectHelper.GetModelBounds(model, transform);
			transform.Pop();
			return SAModel.Direct3D.Extensions.Merge(basebounds, endbounds);
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Displacement", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Movement Speed", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Moving Container Lift (Siding)"; } }
	}
	public class MHContF : MHContBase
	{
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
			model = ObjectHelper.LoadModel("stg10_metalharbor/models/GC/SmallContainer.sa2bmdl");
			meshes = ObjectHelper.GetMeshes(model);
			texarr = NJS_TEXLIST.Load("stg10_metalharbor/tls/SmallContainer.satex");
			texpacks.Add("objtex_stg10");
			texpacks.Add("landtx10");
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Wall Bump", typeof(bool), "Extended", "Determines if the box is treated as a wall that knocks the player back.", null, (o) => Convert.ToBoolean(o.Rotation.X % 2), (o, v) => o.Rotation.X = Convert.ToInt32((bool)v)),
			new PropertySpec("Displacement", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Movement Speed", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Moving Red Container (Far)"; } }
	}
	public class MHCont0F : MHContBase
	{
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
			model = ObjectHelper.LoadModel("stg10_metalharbor/models/GC/Box.sa2bmdl");
			meshes = ObjectHelper.GetMeshes(model);
			texarr = NJS_TEXLIST.Load("stg10_metalharbor/tls/Box.satex");
			texpacks.Add("objtex_stg10");
			texpacks.Add("landtx10");
		}
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
			transform.NJTranslate(item.Scale.X, 0, 0);
			result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			if (texs == null)
				texs = ObjectHelper.GetTextures(texpacks, texarr, dev);
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			transform.NJTranslate(item.Scale.X, 0, 0);
			if (item.Scale.X != 0)
			{
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
				DefineSpline(Vector3.TransformCoordinate(new Vertex(model.Attach.Bounds.Center.X - item.Scale.X, model.Attach.Bounds.Center.Y, model.Attach.Bounds.Center.Z).ToVector3(), transform.Top).ToVertex(), Vector3.TransformCoordinate(model.Attach.Bounds.Center.ToVector3(), transform.Top).ToVertex(), dev);
				RenderInfo outputInfo = new RenderInfo(pathmesh, 0, Matrix.Identity, (item.Selected) ? SelectedMaterial : UnSelectedMaterial, null, EditorOptions.RenderFillMode, pathbounds);
				result.Add(outputInfo);
			}
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
			transform.NJTranslate(item.Scale.X, 0, 0);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(item.Position.ToVector3());
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			BoundingSphere basebounds = ObjectHelper.GetModelBounds(model, transform);
			transform.NJTranslate(item.Scale.X, 0, 0);
			BoundingSphere endbounds = ObjectHelper.GetModelBounds(model, transform);
			transform.Pop();
			return SAModel.Direct3D.Extensions.Merge(basebounds, endbounds);
		}
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Displacement", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Movement Speed", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Moving Container Lift (Siding, Far)"; } }
	}
}