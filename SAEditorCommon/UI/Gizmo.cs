using System.Drawing;
using System.IO;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.Properties;

//using SonicRetro.SAModel.SAEditorCommon.DataTypes;
//using SonicRetro.SAModel.Direct3D;

namespace SonicRetro.SAModel.SAEditorCommon.UI
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
			#region Creating Streams From Resources
			MemoryStream x_NullStream = new MemoryStream(Resources.x_null);
			MemoryStream y_NullStream = new MemoryStream(Resources.y_null);
			MemoryStream z_NullStream = new MemoryStream(Resources.z_null);
			MemoryStream x_MoveStream = new MemoryStream(Resources.x_move);
			MemoryStream y_MoveStream = new MemoryStream(Resources.y_move);
			MemoryStream z_MoveStream = new MemoryStream(Resources.z_move);
			MemoryStream xy_MoveStream = new MemoryStream(Resources.xy_move);
			MemoryStream zx_MoveStream = new MemoryStream(Resources.zx_move);
			MemoryStream zy_MoveStream = new MemoryStream(Resources.zy_move);

			MemoryStream x_RotationStream = new MemoryStream(Resources.x_rotation);
			MemoryStream y_RotationStream = new MemoryStream(Resources.y_rotation);
			MemoryStream z_RotationStream = new MemoryStream(Resources.z_rotation);

			MemoryStream x_ScaleStream = new MemoryStream(Resources.x_scale);
			MemoryStream y_ScaleStream = new MemoryStream(Resources.y_scale);
			MemoryStream z_ScaleStream = new MemoryStream(Resources.z_scale);
			#endregion

			#region Temporary ExtendedMaterials
			ExtendedMaterial[] xMaterials;
			ExtendedMaterial[] yMaterials;
			ExtendedMaterial[] zMaterials;
			ExtendedMaterial[] doubleAxisMaterials;
			#endregion

			#region Loading Meshes and Materials from Streams
			XMoveMesh = Mesh.FromStream(x_MoveStream, MeshFlags.Managed, d3dDevice, out xMaterials);
			YMoveMesh = Mesh.FromStream(y_MoveStream, MeshFlags.Managed, d3dDevice, out yMaterials);
			ZMoveMesh = Mesh.FromStream(z_MoveStream, MeshFlags.Managed, d3dDevice, out zMaterials);

			XNullMesh = Mesh.FromStream(x_NullStream, MeshFlags.Managed, d3dDevice);
			YNullMesh = Mesh.FromStream(y_NullStream, MeshFlags.Managed, d3dDevice);
			ZNullMesh = Mesh.FromStream(z_NullStream, MeshFlags.Managed, d3dDevice);

			XYMoveMesh = Mesh.FromStream(xy_MoveStream, MeshFlags.Managed, d3dDevice, out doubleAxisMaterials);
			ZXMoveMesh = Mesh.FromStream(zx_MoveStream, MeshFlags.Managed, d3dDevice);
			ZYMoveMesh = Mesh.FromStream(zy_MoveStream, MeshFlags.Managed, d3dDevice);

			XRotateMesh = Mesh.FromStream(x_RotationStream, MeshFlags.Managed, d3dDevice);
			YRotateMesh = Mesh.FromStream(y_RotationStream, MeshFlags.Managed, d3dDevice);
			ZRotateMesh = Mesh.FromStream(z_RotationStream, MeshFlags.Managed, d3dDevice);

			XScaleMesh = Mesh.FromStream(x_ScaleStream, MeshFlags.Managed, d3dDevice);
			YScaleMesh = Mesh.FromStream(y_ScaleStream, MeshFlags.Managed, d3dDevice);
			ZScaleMesh = Mesh.FromStream(z_ScaleStream, MeshFlags.Managed, d3dDevice);

			BoxMesh = Mesh.Box(d3dDevice, 1, 1, 1);

			Mesh TexturedBox = BoxMesh.Clone(BoxMesh.Options.Value,
				VertexFormats.Position | VertexFormats.Normal | VertexFormats.Texture0 |
				VertexFormats.Texture1, BoxMesh.Device);

			//The following code makes the assumption that the vertices of the box are
			// generated the same way as they are in the April 2005 SDK
			using (VertexBuffer vb = TexturedBox.VertexBuffer)
			{
				CustomVertex.PositionNormalTextured[] verts =
					(CustomVertex.PositionNormalTextured[])vb.Lock(0,
					 typeof(CustomVertex.PositionNormalTextured), LockFlags.None,
					TexturedBox.NumberVertices);
				try
				{
					for (int i = 0; i < verts.Length; i += 4)
					{
						verts[i + 0].Tu = 0.0f;
						verts[i + 0].Tv = 0.0f;
						verts[i + 1].Tu = 1.0f;
						verts[i + 1].Tv = 0.0f;
						verts[i + 2].Tu = 1.0f;
						verts[i + 2].Tv = 1.0f;
						verts[i + 3].Tu = 0.0f;
						verts[i + 3].Tv = 1.0f;
					}
				}
				finally
				{
					vb.Unlock();
				}
			}

			BoxMesh = TexturedBox;

			XMaterial = new NJS_MATERIAL() { DiffuseColor = xMaterials[0].Material3D.Diffuse, Exponent = 0f, UseTexture = false, IgnoreLighting = true, IgnoreSpecular = true };
			YMaterial = new NJS_MATERIAL() { DiffuseColor = yMaterials[0].Material3D.Diffuse, Exponent = 0f, UseTexture = false, IgnoreLighting = true, IgnoreSpecular = true };
			ZMaterial = new NJS_MATERIAL() { DiffuseColor = zMaterials[0].Material3D.Diffuse, Exponent = 0f, UseTexture = false, IgnoreLighting = true, IgnoreSpecular = true };
			DoubleAxisMaterial = new NJS_MATERIAL() { DiffuseColor = doubleAxisMaterials[0].Material3D.Diffuse, Exponent = 0f, UseTexture = false, IgnoreLighting = true, IgnoreSpecular = true };
			HighlightMaterial = new NJS_MATERIAL() { DiffuseColor = Color.LightGoldenrodYellow, Exponent = 0f, UseTexture = false, IgnoreLighting = true, IgnoreSpecular = true };

			ATexture = Texture.FromBitmap(d3dDevice, Resources.PointATexture, Usage.AutoGenerateMipMap, Pool.Managed);
			BTexture = Texture.FromBitmap(d3dDevice, Resources.PointBTexture, Usage.AutoGenerateMipMap, Pool.Managed);
			StandardMaterial = new NJS_MATERIAL() { DiffuseColor = Color.Gray, IgnoreLighting = true, IgnoreSpecular = true, UseAlpha = false, UseTexture = true, Exponent = 100f };
			#endregion

			#region Cleanup
			x_NullStream.Close();
			y_NullStream.Close();
			z_NullStream.Close();

			x_MoveStream.Close();
			y_MoveStream.Close();
			z_MoveStream.Close();

			x_RotationStream.Close();
			y_RotationStream.Close();
			z_RotationStream.Close();
			#endregion
		}
	}
}
