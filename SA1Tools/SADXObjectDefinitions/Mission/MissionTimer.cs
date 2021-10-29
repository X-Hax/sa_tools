using SharpDX;
using SharpDX.Direct3D9;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;

namespace SADXObjectDefinitions.Mission
{
	class MissionTimer : ObjectDefinition
	{
		public override void Init(ObjectData data, string name) { }

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			transform.Push();
			transform.NJTranslate(item.Position);
			HitResult result = ObjectHelper.CheckSpriteHit(Near, Far, Viewport, Projection, View, transform);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			transform.NJTranslate(item.Position);
			result.AddRange(ObjectHelper.RenderSprite(dev, transform, null, item.Position.ToVector3(), item.Selected));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			return new List<ModelTransform>();
		}

		public override string Name { get { return "Mission Timer"; } }

		private readonly PropertySpec[] customProperties = new PropertySpec[] {
			new PropertySpec("Switch ID", typeof(byte), null, null, true, (o) => ((MissionSETItem)o).PRMBytes[4], (o, v) => ((MissionSETItem)o).PRMBytes[4] = (byte)v),
			new PropertySpec("Minutes", typeof(byte), null, null, true, (o) => ((MissionSETItem)o).PRMBytes[5], (o, v) => ((MissionSETItem)o).PRMBytes[5] = (byte)v),
			new PropertySpec("Seconds", typeof(byte), null, null, true, (o) => ((MissionSETItem)o).PRMBytes[6], (o, v) => ((MissionSETItem)o).PRMBytes[6] = (byte)v),
			new PropertySpec("Frames", typeof(byte), null, null, true, (o) => ((MissionSETItem)o).PRMBytes[7], (o, v) => ((MissionSETItem)o).PRMBytes[7] = (byte)v)
		};

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);

			return matrix;
		}

		public override PropertySpec[] CustomProperties { get { return customProperties; } }
	}
}
