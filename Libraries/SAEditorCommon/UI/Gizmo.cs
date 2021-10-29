using System.Drawing;
using SharpDX.Direct3D9;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.Properties;
using Mesh = SAModel.Direct3D.Mesh;

//using SAModel.SAEditorCommon.DataTypes;

namespace SAModel.SAEditorCommon.UI
{
	/// <summary>
	/// Houses elements common to several kinds of gizmos / helpers.
	/// </summary>
	public static class Gizmo
	{
		// 'null' meshes - for when no transforms are available, but users still want to know that something *is* selected
		public static Mesh XNullMesh { get; set; }
		public static Mesh YNullMesh { get; set; }
		public static Mesh ZNullMesh { get; set; }

		// movement meshes - used for both display and mouse picking
		public static Mesh XMoveMesh { get; set; }
		public static Mesh YMoveMesh { get; set; }
		public static Mesh ZMoveMesh { get; set; }
		public static Mesh XYMoveMesh { get; set; }
		public static Mesh ZXMoveMesh { get; set; }
		public static Mesh ZYMoveMesh { get; set; }

		// rotation meshes - used for both display and mouse picking
		public static Mesh XRotateMesh { get; set; }
		public static Mesh YRotateMesh { get; set; }
		public static Mesh ZRotateMesh { get; set; }

		// scale meshes - used for both display and mouse picking
		public static Mesh XScaleMesh { get; set; }
		public static Mesh YScaleMesh { get; set; }
		public static Mesh ZScaleMesh { get; set; }

		// box mesh - you know, just in case you need one.
		public static Mesh BoxMesh { get; set; }

		// materials for rendering the above meshes
		public static NJS_MATERIAL XMaterial { get; set; }
		public static NJS_MATERIAL YMaterial { get; set; }
		public static NJS_MATERIAL ZMaterial { get; set; }
		public static NJS_MATERIAL DoubleAxisMaterial { get; set; }
		public static NJS_MATERIAL HighlightMaterial { get; set; }

		public static NJS_MATERIAL StandardMaterial { get; set; }

		public static Texture ATexture { get; set; }
		public static Texture BTexture { get; set; }

		public static void InitGizmo(Device d3dDevice)
		{
			Attach attach = new ModelFile(Resources.x_null).Model.Attach;
			attach.ProcessVertexData();
			XNullMesh = attach.CreateD3DMesh();

			attach = new ModelFile(Resources.y_null).Model.Attach;
			attach.ProcessVertexData();
			YNullMesh = attach.CreateD3DMesh();

			attach = new ModelFile(Resources.z_null).Model.Attach;
			attach.ProcessVertexData();
			ZNullMesh = attach.CreateD3DMesh();

			attach = new ModelFile(Resources.x_move).Model.Attach;
			attach.ProcessVertexData();
			XMoveMesh = attach.CreateD3DMesh();
			XMaterial = ((BasicAttach)attach).Material[0];

			attach = new ModelFile(Resources.y_move).Model.Attach;
			attach.ProcessVertexData();
			YMoveMesh = attach.CreateD3DMesh();
			YMaterial = ((BasicAttach)attach).Material[0];

			attach = new ModelFile(Resources.z_move).Model.Attach;
			attach.ProcessVertexData();
			ZMoveMesh = attach.CreateD3DMesh();
			ZMaterial = ((BasicAttach)attach).Material[0];

			attach = new ModelFile(Resources.xy_move).Model.Attach;
			attach.ProcessVertexData();
			XYMoveMesh = attach.CreateD3DMesh();
			DoubleAxisMaterial = ((BasicAttach)attach).Material[0];

			attach = new ModelFile(Resources.zx_move).Model.Attach;
			attach.ProcessVertexData();
			ZXMoveMesh = attach.CreateD3DMesh();

			attach = new ModelFile(Resources.zy_move).Model.Attach;
			attach.ProcessVertexData();
			ZYMoveMesh = attach.CreateD3DMesh();

			attach = new ModelFile(Resources.x_rotation).Model.Attach;
			attach.ProcessVertexData();
			XRotateMesh = attach.CreateD3DMesh();

			attach = new ModelFile(Resources.y_rotation).Model.Attach;
			attach.ProcessVertexData();
			YRotateMesh = attach.CreateD3DMesh();

			attach = new ModelFile(Resources.z_rotation).Model.Attach;
			attach.ProcessVertexData();
			ZRotateMesh = attach.CreateD3DMesh();

			attach = new ModelFile(Resources.x_scale).Model.Attach;
			attach.ProcessVertexData();
			XScaleMesh = attach.CreateD3DMesh();

			attach = new ModelFile(Resources.y_scale).Model.Attach;
			attach.ProcessVertexData();
			YScaleMesh = attach.CreateD3DMesh();

			attach = new ModelFile(Resources.z_scale).Model.Attach;
			attach.ProcessVertexData();
			ZScaleMesh = attach.CreateD3DMesh();

			BoxMesh = Mesh.Box(1, 1, 1);

			HighlightMaterial = new NJS_MATERIAL() { DiffuseColor = Color.LightGoldenrodYellow, Exponent = 0f, UseTexture = false, IgnoreLighting = true, IgnoreSpecular = true };

			ATexture = Resources.PointATexture.ToTexture(d3dDevice);
			BTexture = Resources.PointBTexture.ToTexture(d3dDevice);
			StandardMaterial = new NJS_MATERIAL() { DiffuseColor = Color.Gray, IgnoreLighting = true, IgnoreSpecular = true, UseAlpha = false, UseTexture = true, Exponent = 100f };
		}
	}
}
