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

namespace SA2ObjectDefinitions.WildCanyon
{
	public class Rock : ObjectDefinition
	{
		private NJS_OBJECT model;
		private Mesh[] meshes;
		private NJS_TEXLIST texarr;
		private Texture[] texs;
		protected List<string> texpacks = [];

		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg16_WildCanyon/models/ROCK.sa2mdl");
			meshes = ObjectHelper.GetMeshes(model);

			texarr = NJS_TEXLIST.Load("stg16_WildCanyon/tls/ROCK.satex");

			texpacks.Add("objtex_stg16");
			texpacks.Add("landtx16");
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			float scaleconst = (item.Scale.X / 10F) + 1F;
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJScale(scaleconst, scaleconst, scaleconst);
			result = HitResult.Min(result, model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes));
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			if (texs == null)
				texs = ObjectHelper.GetTextures(texpacks, texarr, dev);
			float scaleconst = (item.Scale.X / 10F) + 1F;
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJScale(scaleconst, scaleconst, scaleconst);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
			{
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			}
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			float scaleconst = (item.Scale.X / 10F) + 1F;
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			transform.NJScale(scaleconst, scaleconst, scaleconst);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			float scaleconst = (item.Scale.X / 10F) + 1F;
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position);
			transform.NJScale(scaleconst, scaleconst, scaleconst);
			BoundingSphere bounds = ObjectHelper.GetModelBounds(model, transform);
			return bounds;
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Spin Speed", typeof(int), "Extended", null, null, (o) => o.Rotation.X, (o, v) => o.Rotation.X = (int)v),
			new PropertySpec("Emerald ID", typeof(byte), "Extended", "A value of 255 denotes that the rock will not spawn an emerald piece when destroyed.", null, (o) => o.Rotation.Y & 0xFF,
			(o, v) => { o.Rotation.Y &= 0xFF00; o.Rotation.Y |= (byte)v; }),
			new PropertySpec("Scale", typeof(float), "Extended", "Value is divided by 10 and then 1 is added at runtime.", null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Oscillation Strength", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Oscillation Speed", typeof(float), "Extended", null, null, (o) => o.Scale.Z, (o, v) => o.Scale.Z = (float)v),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Floating Rock"; } }
	}
}