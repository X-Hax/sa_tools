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
	public class Stonehead : ObjectDefinition
	{
		private NJS_OBJECT stonemodel;
		private Mesh[] stonemesh;
		private NJS_OBJECT pathmodel;
		private Mesh[] pathmesh;
		private NJS_TEXLIST texarrstone;
		private Texture[] texsstone;
		private NJS_TEXLIST texarrpath;
		private Texture[] texspath;
		protected List<string> texpacks = [];

		public Vertex ModifyPosition(SETItem item, out Vertex pos)
		{
			float scaleconst = item.Scale.X + 1F;
			float scaleconststone = scaleconst * -12F;
			float stonesine = ObjectHelper.NJSin(item.Rotation.Y) * scaleconststone;
			float stonecos = ObjectHelper.NJCos(item.Rotation.Y) * scaleconststone;
			pos = item.Position;
			pos.X = stonesine + item.Position.X;
			pos.Y = item.Position.Y;
			pos.Z = stonecos + item.Position.Z;

			return pos;
		}
		public override void Init(ObjectData data, string name)
		{

			stonemodel = ObjectHelper.LoadModel("stg16_WildCanyon/models/STONEHEAD.sa2mdl");
			pathmodel = ObjectHelper.LoadModel("stg16_WildCanyon/models/STONEHEAD_PATH.sa2mdl");
			
			stonemesh = ObjectHelper.GetMeshes(stonemodel);
			pathmesh = ObjectHelper.GetMeshes(pathmodel);

			texarrstone = NJS_TEXLIST.Load("stg16_WildCanyon/tls/STONEHEAD.satex");
			texarrpath = NJS_TEXLIST.Load("stg16_WildCanyon/tls/STONEHEAD_PATH.satex");

			texpacks.Add("objtex_stg16");
			texpacks.Add("landtx16");
		}
		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result = HitResult.NoHit;
			//MATH
			float scaleconst = item.Scale.X + 1F;
			float scaleconstY = item.Scale.Y + 1F;
			float scaleconst1 = scaleconst * 12F;
			float scaleconstminus = scaleconst * 20F;
			float scaleconst2 = (scaleconstY * scaleconstminus) - (scaleconstminus * 0.5F);
			float sine1 = ObjectHelper.NJSin(item.Rotation.Y) * scaleconst1;
			float sine2 = ObjectHelper.NJSin(item.Rotation.Y) * scaleconst2;
			float cos1 = ObjectHelper.NJCos(item.Rotation.Y) * scaleconst1;
			float cos2 = ObjectHelper.NJCos(item.Rotation.Y) * scaleconst2;

			float newX1 = item.Position.X + sine1 + sine2;
			float newZ1 = item.Position.Z + cos1 + cos2;

			float newX2 = item.Position.X + sine1 - sine2;
			float newZ2 = item.Position.Z + cos1 - cos2;

			float fixedX = newX2 - newX1;
			float fixedZ = newZ2 - newZ1;

			float newposX = (fixedX / 255F) * (float)(item.Rotation.Z & 0xFF) + newX1;
			float newposZ = (float)(item.Rotation.Z & 0xFF) * (fixedZ / 255F) + newZ1;

			float scaleconststone = scaleconst * -12F;
			float stonesine = ObjectHelper.NJSin(item.Rotation.Y) * scaleconststone;
			float stonecos = ObjectHelper.NJCos(item.Rotation.Y) * scaleconststone;

			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(scaleconst, 1F, (item.Scale.Y + 1F) * scaleconst);
			result = HitResult.Min(result, pathmodel.CheckHit(Near, Far, Viewport, Projection, View, transform, pathmesh));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(newposX + stonesine, item.Position.Y, newposZ + stonecos);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(scaleconst, scaleconst, scaleconst);
			result = HitResult.Min(result, stonemodel.CheckHit(Near, Far, Viewport, Projection, View, transform, stonemesh));
			transform.Pop();
			return result;
		}



		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			//MATH
			float scaleconst = item.Scale.X + 1F;
			float scaleconstY = item.Scale.Y + 1F;
			float scaleconst1 = scaleconst * 12F;
			float scaleconstminus = scaleconst * 20F;
			float scaleconst2 = (scaleconstY * scaleconstminus) - (scaleconstminus * 0.5F);
			float sine1 = ObjectHelper.NJSin(item.Rotation.Y) * scaleconst1;
			float sine2 = ObjectHelper.NJSin(item.Rotation.Y) * scaleconst2;
			float cos1 = ObjectHelper.NJCos(item.Rotation.Y) * scaleconst1;
			float cos2 = ObjectHelper.NJCos(item.Rotation.Y) * scaleconst2;

			float newX1 = item.Position.X + sine1 + sine2;
			float newZ1 = item.Position.Z + cos1 + cos2;

			float newX2 = item.Position.X + sine1 - sine2;
			float newZ2 = item.Position.Z + cos1 - cos2;

			float fixedX = newX2 - newX1;
			float fixedZ = newZ2 - newZ1;

			float newposX = (fixedX / 255F) * (float)(item.Rotation.Z & 0xFF) + newX1;
			float newposZ = (float)(item.Rotation.Z & 0xFF) * (fixedZ / 255F) + newZ1;

			float scaleconststone = scaleconst * -12F;
			float stonesine = ObjectHelper.NJSin(item.Rotation.Y) * scaleconststone;
			float stonecos = ObjectHelper.NJCos(item.Rotation.Y) * scaleconststone;

			if (texspath == null)
				texspath = ObjectHelper.GetTextures(texpacks, texarrpath, dev);
			if (texsstone == null)
				texsstone = ObjectHelper.GetTextures(texpacks, texarrstone, dev);
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(scaleconst, 1F, (item.Scale.Y + 1F) * scaleconst);
			result.AddRange(pathmodel.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texspath, pathmesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
			{
				result.AddRange(pathmodel.DrawModelTreeInvert(transform, pathmesh));
			}
			transform.Pop();
			transform.Push();
			transform.NJTranslate(newposX + stonesine, item.Position.Y, newposZ + stonecos);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(scaleconst, scaleconst, scaleconst);
			result.AddRange(stonemodel.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texsstone, stonemesh, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
			if (item.Selected)
			{
				result.AddRange(stonemodel.DrawModelTreeInvert(transform, stonemesh));
			}
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			//MATH
			float scaleconst = item.Scale.X + 1F;
			float scaleconstY = item.Scale.Y + 1F;
			float scaleconst1 = scaleconst * 12F;
			float scaleconstminus = scaleconst * 20F;
			float scaleconst2 = (scaleconstY * scaleconstminus) - (scaleconstminus * 0.5F);
			float sine1 = ObjectHelper.NJSin(item.Rotation.Y) * scaleconst1;
			float sine2 = ObjectHelper.NJSin(item.Rotation.Y) * scaleconst2;
			float cos1 = ObjectHelper.NJCos(item.Rotation.Y) * scaleconst1;
			float cos2 = ObjectHelper.NJCos(item.Rotation.Y) * scaleconst2;

			float newX1 = item.Position.X + sine1 + sine2;
			float newZ1 = item.Position.Z + cos1 + cos2;

			float newX2 = item.Position.X + sine1 - sine2;
			float newZ2 = item.Position.Z + cos1 - cos2;

			float fixedX = newX2 - newX1;
			float fixedZ = newZ2 - newZ1;

			float newposX = (fixedX / 255F) * (float)(item.Rotation.Z & 0xFF) + newX1;
			float newposZ = (float)(item.Rotation.Z & 0xFF) * (fixedZ / 255F) + newZ1;

			float scaleconststone = scaleconst * -12F;
			float stonesine = ObjectHelper.NJSin(item.Rotation.Y) * scaleconststone;
			float stonecos = ObjectHelper.NJCos(item.Rotation.Y) * scaleconststone;
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(scaleconst, 1F, (item.Scale.Y + 1F) * scaleconst);
			result.Add(new ModelTransform(pathmodel, transform.Top));
			transform.Pop();
			transform.Push();
			transform.NJTranslate(newposX + stonesine, item.Position.Y, newposZ + stonecos);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(scaleconst, scaleconst, scaleconst);
			result.Add(new ModelTransform(stonemodel, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			//MATH
			float scaleconst = item.Scale.X + 1F;
			float scaleconstY = item.Scale.Y + 1F;
			float scaleconst1 = scaleconst * 12F;
			float scaleconstminus = scaleconst * 20F;
			float scaleconst2 = (scaleconstY * scaleconstminus) - (scaleconstminus * 0.5F);
			float sine1 = ObjectHelper.NJSin(item.Rotation.Y) * scaleconst1;
			float sine2 = ObjectHelper.NJSin(item.Rotation.Y) * scaleconst2;
			float cos1 = ObjectHelper.NJCos(item.Rotation.Y) * scaleconst1;
			float cos2 = ObjectHelper.NJCos(item.Rotation.Y) * scaleconst2;

			float newX1 = item.Position.X + sine1 + sine2;
			float newZ1 = item.Position.Z + cos1 + cos2;

			float newX2 = item.Position.X + sine1 - sine2;
			float newZ2 = item.Position.Z + cos1 - cos2;

			float fixedX = newX2 - newX1;
			float fixedZ = newZ2 - newZ1;

			float newposX = (fixedX / 255F) * (float)(item.Rotation.Z & 0xFF) + newX1;
			float newposZ = (float)(item.Rotation.Z & 0xFF) * (fixedZ / 255F) + newZ1;

			float scaleconststone = scaleconst * -12F;
			float stonesine = ObjectHelper.NJSin(item.Rotation.Y) * scaleconststone;
			float stonecos = ObjectHelper.NJCos(item.Rotation.Y) * scaleconststone;
			MatrixStack transform = new MatrixStack();
			transform.Push();
			transform.NJTranslate(item.Position);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(scaleconst, 1F, (item.Scale.Y + 1F) * scaleconst);
			BoundingSphere pathbounds = ObjectHelper.GetModelBounds(pathmodel, transform);
			transform.Pop();
			transform.Push();
			transform.NJTranslate(newposX + stonesine, item.Position.Y, newposZ + stonecos);
			if (item.Rotation.Y != 0)
				transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(scaleconst, scaleconst, scaleconst);
			BoundingSphere stonebounds = ObjectHelper.GetModelBounds(stonemodel, transform);
			transform.Pop();
			return SAModel.Direct3D.Extensions.Merge(stonebounds, pathbounds);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Scale", typeof(float), "Extended", null, null, (o) => o.Scale.X, (o, v) => o.Scale.X = (float)v),
			new PropertySpec("Path Length", typeof(float), "Extended", null, null, (o) => o.Scale.Y, (o, v) => o.Scale.Y = (float)v),
			new PropertySpec("Stone Initial Position", typeof(int), "Extended", "Valid values are between 0 and 255, with a movement pattern similar to a volume slider.", null, (o) => o.Rotation.Z, (o, v) => o.Rotation.Z = (int)v),
		};
		public override PropertySpec[] CustomProperties { get { return customProperties; } }
		public override string Name { get { return "Large Movable Block"; } }
	}
}