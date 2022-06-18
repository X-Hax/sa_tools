using SharpDX;
using SharpDX.Direct3D9;
using SAModel;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using System.Collections.Generic;
using BoundingSphere = SAModel.BoundingSphere;
using Mesh = SAModel.Direct3D.Mesh;

namespace SADXObjectDefinitions.WindyValleyAutodemo
{
	public abstract class DynCol : ObjectDefinition
	{
		protected NJS_OBJECT model;
		protected Mesh[] meshes;

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			model.Position = item.Position;
			model.Rotation = item.Rotation;
			transform.Push();
			HitResult result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			model.Position = item.Position;
			model.Rotation = item.Rotation;
			transform.Push();
			result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, ObjectHelper.GetTextures("OBJ_WINDY"), meshes));
			if (item.Selected)
				result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			List<ModelTransform> result = new List<ModelTransform>();
			model.Position = item.Position;
			model.Rotation = item.Rotation;
			transform.Push();
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();

			model.Position = item.Position;
			model.Rotation = item.Rotation;

			return ObjectHelper.GetModelBounds(model, transform);
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;

			MatrixFunctions.Translate(ref matrix, item.Position);

			return matrix;
		}
	}

	public class Sirusi2 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_sign2.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Quadruple Blue Arrow Rock Sign"; } }
	}

	public class Sirusi3 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_sign3.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Double Blue Arrow Rock Sign"; } }
	}

	public class Sirusi4 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_sign4.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Single Blue Arrow Rock Sign"; } }
	}

	public class Sirusi11 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_sign11.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Triple Yellow Arrow Sign"; } }
	}

	public class Sirusi12 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_sign12.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Single Yellow Arrow Sign"; } }
	}

	public class Sirusi13 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_sign13.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Small Rock Bumper"; } }
	}

	public class IHas14 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_has14.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Large Pillar w/Platform"; } }
	}

	public class IHas15 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_has15.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Broken Pillar in Rock"; } }
	}

	public class IHas16 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_has16.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Broken Pillar"; } }
	}

	public class IHas19 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_has19.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Pillar Top 3"; } }
	}

	public class LRock1 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_largerock1.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Large Rock"; } }
	}

	public class Rock1 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_rock1.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Little Rock (Oval)"; } }
	}

	public class IwaB : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_iwab.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Small Square Rock"; } }
	}

	public class Ioiwa1 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_oiwa01.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Hilly Rock"; } }
	}

	public class Ioiwa2 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_oiwa02.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Hilly Rock (Flattened)"; } }
	}

	public class Ioiwa3 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_oiwa03.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Hilly Rock (Small)"; } }
	}

	public class SaraB1 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_sarabig1.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Big Rock Plate 1"; } }
	}

	public class SaraB2 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_sarabig2.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Big Rock Plate 2"; } }
	}

	public class SaraM1 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_saramid1.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Medium Rock Plate 1"; } }
	}

	public class SaraM2 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_saramid2.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Medium Rock Plate 2"; } }
	}

	public class SaraS1 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_sarasmall1.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Small Rock Plate 1"; } }
	}

	public class SaraS2 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_sarasmall2.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Small Rock Plate 2"; } }
	}

	public class IBou1 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_bou01.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Rock Pole (Standing)"; } }
	}

	public class IBou2 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_bou02.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Rock Pole (Sideways)"; } }
	}

	public class IBou03 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_bou03.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Rock Pole (Base)"; } }
	}

	public class IHah1 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_hah01.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Large Broken Ruin"; } }
	}

	public class IHah2 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_hah02.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Medium Broken Ruin"; } }
	}

	public class IHah3 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_hah03.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Small Broken Ruin"; } }
	}

	public class IDai1 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_dai1.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Small Rock Stand (L Shaped)"; } }
	}

	public class IDai2 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_dai2.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Medium Rock Stand (L Shaped)"; } }
	}

	public class IDai3 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_dai3.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Large Rock Stand (L Shaped)"; } }
	}

	public class IDai4 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_dai4.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Flat Rock Stand 1 (Square Shaped)"; } }
	}

	public class IDai5 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_dai5.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Flat Rock Stand 2 (Square Shaped)"; } }
	}

	public class IDai6 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_dai6.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Angled Rock Stand 1 (Square Shaped)"; } }
	}

	public class IDai7 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_dai7.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Angled Rock Stand 2 (Square Shaped)"; } }
	}

	public class IDai8 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_dai8.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Flat Rock Stand 3 (Square Shaped)"; } }
	}

	public class IDai9 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_dai9.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Angled Rock Stand 3 (Square Shaped)"; } }
	}

	public class IDai10 : DynCol
	{
		public override void Init(ObjectData data, string name)
		{
			model = ObjectHelper.LoadModel("stg02_windy/common/models/wvobj_dai10.sa1mdl");
			meshes = ObjectHelper.GetMeshes(model);
		}
		public override string Name { get { return "Flat Rock Stand 4 (Square Shaped)"; } }
	}
}