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

namespace SADXObjectDefinitions.Mission
{
	class MissionEndMarker : ObjectDefinition
	{
		private NJS_OBJECT model;
		private Mesh[] meshes;

		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("mission/model/mi_target.nja.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + 0.5f, item.Position.Z);
			transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(item.Scale.X, item.Scale.Y, item.Scale.X);
			HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + 0.5f, item.Position.Z);
			transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(item.Scale.X, item.Scale.Y, item.Scale.X);
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("Mission"), meshes));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position.X, item.Position.Y + 0.5f, item.Position.Z);
			transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(item.Scale.X, item.Scale.Y, item.Scale.X);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.X, item.Position.Y + 0.5f, item.Position.Z);
			transform.NJRotateY(item.Rotation.Y);
			transform.NJScale(item.Scale.X, item.Scale.Y, item.Scale.X);
			return ObjectHelper.GetModelBounds(model, transform, Math.Max(item.Scale.X, item.Scale.Y));
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position.X, item.Position.Y + 0.5f, item.Position.Z);
			MatrixFunctions.RotateY(ref matrix, item.Rotation.Y);

			return matrix;
		}

		public override string Name { get { return "Mission End Marker"; } }

		static void SetItemList(SETItem obj, object val)
		{
			MissionSETItem item = (MissionSETItem)obj;
			MsnObjectList value = (MsnObjectList)val;
			item.PRMBytes[6] = (byte)((item.PRMBytes[6] & 0x7F) | (int)val << 7);
		}

		static object GetItemIndex(SETItem obj)
		{
			MissionSETItem item = (MissionSETItem)obj;
			short val = (short)(item.PRMBytes[6] << 8 | item.PRMBytes[7]);
			if (val == -1)
				return -1;
			else
				return val & 0x7FFF;
		}

		static void SetItemIndex(SETItem obj, object val)
		{
			MissionSETItem item = (MissionSETItem)obj;
			short value = (short)val;
			if (value == -1)
				item.PRMBytes[6] = item.PRMBytes[7] = 0xFF;
			else
			{
				item.PRMBytes[6] = (byte)((value >> 8) & 0x7F);
				item.PRMBytes[7] = (byte)value;
			}
		}

		// incomplete, further investigation required
		// specifically: PRMBytes[4] controls the mode of operation, but I can't tell what they all do
		// also probably need a selector for the Item Index property, maybe draw a line connecting the objects?
		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Visible", typeof(bool), null, null, true, (o) => ((MissionSETItem)o).PRMBytes[8] == 0, (o, v) => ((MissionSETItem)o).PRMBytes[8] = (byte)((bool)v ? 0 : 1)),
			new PropertySpec("Rings Required", typeof(byte), null, null, 0, (o) => ((MissionSETItem)o).PRMBytes[5], (o, v) => ((MissionSETItem)o).PRMBytes[5] = (byte)v),
			new PropertySpec("Item List", typeof(MsnObjectList), null, null, MsnObjectList.Mission, o => (MsnObjectList)(((MissionSETItem)o).PRMBytes[6] >> 7), SetItemList),
			new PropertySpec("Item Index", typeof(short), null, null, -1, GetItemIndex, SetItemIndex)
		};

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
	}
}
