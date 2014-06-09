using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;

using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.Direct3D;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
    public enum GizmoSelectedAxes { X_AXIS, Y_AXIS, Z_AXIS, XY_AXIS, XZ_AXIS, ZY_AXIS, NONE }

    public enum TransformMode { TRANFORM_MOVE, TRANSFORM_ROTATE }

    /// <summary>
    /// This gives the user a handle to click on for movement and rotation operations
    /// </summary>
    public class TransformGizmo
    {
        private Vector3 position;
        private Rotation rotation;

        private bool enabled = false;
        private bool isTransformnLocal = false; // if TRUE,  the gizmo is in Local mode.
        private GizmoSelectedAxes selectedAxes = GizmoSelectedAxes.NONE; // if this value is not NONE and enabled is true, you've gotten yourself into an invalid state.
        private TransformMode mode = TransformMode.TRANFORM_MOVE;

        // movement meshes - used for both display and mouse picking
        private Microsoft.DirectX.Direct3D.Mesh xMoveMesh;
        private Microsoft.DirectX.Direct3D.Mesh yMoveMesh;
        private Microsoft.DirectX.Direct3D.Mesh zMoveMesh;

        // rotation meshes - used for both display and mouse picking
        private Microsoft.DirectX.Direct3D.Mesh xRotateMesh;
        private Microsoft.DirectX.Direct3D.Mesh yRotateMesh;
        private Microsoft.DirectX.Direct3D.Mesh zRotateMesh;

        // materials for rendering the above meshes
        Material xMaterial;
        Material yMaterial;
        Material zMaterial;

        private List<Item> affectedItems; // these are the items that will be affected by any transforms we are given.

        #region Public Accesors
        public Vector3 Position { get { return position; } set { position = value; } }
        public Rotation GizRotation { get { return rotation; } set { rotation = value; } }
        public bool Enabled { get { return enabled; } set { enabled = value; } }

        public List<Item> AffectedItems { get { return affectedItems; } 
            set 
            { 
                affectedItems = value;
                position = Item.CenterFromSelection(affectedItems).ToVector3();
                if ((affectedItems.Count == 1) && isTransformnLocal) rotation = new Rotation(affectedItems[0].Rotation.X, affectedItems[0].Rotation.Y, affectedItems[0].Rotation.Z);
                else rotation = new Rotation();

                if (affectedItems.Count > 0) enabled = false;
                else enabled = false;
            }
        }
        #endregion

        public TransformGizmo(Device d3dDevice)
        {
            MemoryStream x_MoveStream = new MemoryStream(Properties.Resources.x_move);
            MemoryStream y_MoveStream = new MemoryStream(Properties.Resources.y_move);
            MemoryStream z_MoveStream = new MemoryStream(Properties.Resources.z_move);

            MemoryStream x_RotationStream = new MemoryStream(Properties.Resources.x_rotation);
            MemoryStream y_RotationStream = new MemoryStream(Properties.Resources.y_rotation);
            MemoryStream z_RotationStream = new MemoryStream(Properties.Resources.z_rotation);

            Microsoft.DirectX.Direct3D.ExtendedMaterial[] xMaterials;
            Microsoft.DirectX.Direct3D.ExtendedMaterial[] yMaterials;
            Microsoft.DirectX.Direct3D.ExtendedMaterial[] zMaterials;

            xMoveMesh = Microsoft.DirectX.Direct3D.Mesh.FromStream(x_MoveStream, MeshFlags.Managed, d3dDevice, out xMaterials);
            yMoveMesh = Microsoft.DirectX.Direct3D.Mesh.FromStream(y_MoveStream, MeshFlags.Managed, d3dDevice, out yMaterials);
            zMoveMesh = Microsoft.DirectX.Direct3D.Mesh.FromStream(z_MoveStream, MeshFlags.Managed, d3dDevice, out zMaterials);

            xRotateMesh = Microsoft.DirectX.Direct3D.Mesh.FromStream(x_RotationStream, MeshFlags.Managed, d3dDevice);
            yRotateMesh = Microsoft.DirectX.Direct3D.Mesh.FromStream(y_RotationStream, MeshFlags.Managed, d3dDevice);
            zRotateMesh = Microsoft.DirectX.Direct3D.Mesh.FromStream(z_RotationStream, MeshFlags.Managed, d3dDevice);

            xMaterial = new SAModel.Material() { DiffuseColor = xMaterials[0].Material3D.Diffuse, Exponent = 10f, UseTexture = false, IgnoreLighting = true, IgnoreSpecular = true };
            yMaterial = new SAModel.Material() { DiffuseColor = yMaterials[0].Material3D.Diffuse, Exponent = 10f, UseTexture = false, IgnoreLighting = true, IgnoreSpecular = true };
            zMaterial = new SAModel.Material() { DiffuseColor = zMaterials[0].Material3D.Diffuse, Exponent = 10f, UseTexture = false, IgnoreLighting = true, IgnoreSpecular = true };

            x_MoveStream.Close();
            y_MoveStream.Close();
            z_MoveStream.Close();

            x_RotationStream.Close();
            y_RotationStream.Close();
            z_RotationStream.Close();
        }

        /*public HitResult CheckHit()
        {
            HitResult returnValue = new HitResult();

            return returnValue;
        }*/

        public RenderInfo[] Render(Device dev, MatrixStack transform, EditorCamera cam)
        {
            List<RenderInfo> result = new List<RenderInfo>();

            if (!enabled) return result.ToArray();

            float dist = SonicRetro.SAModel.Direct3D.Extensions.Distance(cam.Position, position) * 0.0825f;

            dev.SetSamplerState(0, SamplerStageStates.MinFilter, (int)TextureFilter.Point); // no fancy filtering is required because no textures are even being used
            dev.SetSamplerState(0, SamplerStageStates.MagFilter, (int)TextureFilter.Point);
            dev.SetSamplerState(0, SamplerStageStates.MipFilter, (int)TextureFilter.Point);
            dev.SetRenderState(RenderStates.Lighting, true);
            dev.SetRenderState(RenderStates.SpecularEnable, false);
            dev.SetRenderState(RenderStates.Ambient, System.Drawing.Color.White.ToArgb());
            dev.SetRenderState(RenderStates.AlphaBlendEnable, false);
            dev.SetRenderState(RenderStates.ColorVertex, false);
            dev.SetRenderState(RenderStates.ZEnable, false);

            transform.Push();
            transform.TranslateLocal(position.X, position.Y, position.Z);
            transform.RotateXYZLocal(rotation.X, rotation.Y, rotation.Z);
            transform.ScaleLocal(Math.Abs(dist), Math.Abs(dist), Math.Abs(dist));

            if (mode == TransformMode.TRANFORM_MOVE)
            {
                BoundingSphere gizmoSphere = new BoundingSphere() { Center = new Vertex(position.X, position.Y, position.Z), Radius = (1.0f * Math.Abs(dist)) };
                Texture gizmoTexture = new Texture(dev, new System.Drawing.Bitmap(2, 2), 0, Pool.Managed);
                RenderInfo xRenderInfo = new RenderInfo(xMoveMesh, 0, transform.Top, xMaterial, gizmoTexture, FillMode.Solid, gizmoSphere);
                result.Add(xRenderInfo);
                RenderInfo yRenderInfo = new RenderInfo(yMoveMesh, 0, transform.Top, yMaterial, gizmoTexture, FillMode.Solid, gizmoSphere);
                result.Add(yRenderInfo);
                RenderInfo zRenderInfo = new RenderInfo(zMoveMesh, 0, transform.Top, zMaterial, gizmoTexture, FillMode.Solid, gizmoSphere);
                result.Add(zRenderInfo);
            }
            else if (mode == TransformMode.TRANSFORM_ROTATE)
            {
                BoundingSphere gizmoSphere = new BoundingSphere() { Center = new Vertex(position.X, position.Y, position.Z), Radius = (1.0f * Math.Abs(dist)) };
                Texture gizmoTexture = new Texture(dev, new System.Drawing.Bitmap(2, 2), 0, Pool.Managed);
                RenderInfo xRenderInfo = new RenderInfo(xRotateMesh, 0, transform.Top, xMaterial, gizmoTexture, FillMode.Solid, gizmoSphere);
                result.Add(xRenderInfo);
                RenderInfo yRenderInfo = new RenderInfo(yRotateMesh, 0, transform.Top, yMaterial, gizmoTexture, FillMode.Solid, gizmoSphere);
                result.Add(yRenderInfo);
                RenderInfo zRenderInfo = new RenderInfo(zRotateMesh, 0, transform.Top, zMaterial, gizmoTexture, FillMode.Solid, gizmoSphere);
                result.Add(zRenderInfo);
            }

            transform.Pop();
            return result.ToArray();
        }
    }
}
