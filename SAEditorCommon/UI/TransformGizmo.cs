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
    /// <summary>Describes which axes the Gizmo will operate upon.</summary>
    public enum GizmoSelectedAxes 
    { 
        /// <summary>Transform will operate on the X axis.</summary>
        X_AXIS,
        /// <summary>Transform will operate on the Y axis.</summary>
        Y_AXIS,
        /// <summary>Transform will operate on the Z axis.</summary>
        Z_AXIS,
        /// <summary>Transform will operate on the XY axis. This is only valid for movement, not rotation.</summary>
        XY_AXIS,
        /// <summary>Transform will operate on the XZ axis. This is only valid for movement, not rotation.</summary>
        XZ_AXIS,
        /// <summary>Transform will operate on the ZY axis. This is only valid for movement, not rotation.</summary>
        ZY_AXIS,
        /// <summary>No operations can be performed.</summary>
        NONE 
    }

    /// <summary>Describes how the Gizmo should behave.</summary>
    public enum TransformMode 
    {
        /// <summary>No operations can be performed, but the user should still see an axis display.</summary>
        NONE,
        /// <summary>Used to move items. Axis handles will have arrow tips to suggest movement.</summary>
        TRANFORM_MOVE,
        /// <summary>Used to rotate items. Axis handles will be toroids to suggest a continuum.</summary>
        TRANSFORM_ROTATE 
    }

    /// <summary>
    /// This gives the user a handle to click on for movement and rotation operations.
    /// </summary>
    public class TransformGizmo
    {
        #region Private Variables
        private Vector3 position;
        private Rotation rotation;

        private bool enabled = false;
        private bool isTransformnLocal = false; // if TRUE,  the gizmo is in Local mode.
        private GizmoSelectedAxes selectedAxes = GizmoSelectedAxes.NONE; // if this value is not NONE and enabled is true, you've gotten yourself into an invalid state.
        private TransformMode mode = TransformMode.NONE;

        // 'null' meshes - for when no transform mode is selected, but users still want to know that something *is* selected
        private Microsoft.DirectX.Direct3D.Mesh xNullMesh;
        private Microsoft.DirectX.Direct3D.Mesh yNullMesh;
        private Microsoft.DirectX.Direct3D.Mesh zNullMesh;

        // movement meshes - used for both display and mouse picking
        private Microsoft.DirectX.Direct3D.Mesh xMoveMesh;
        private Microsoft.DirectX.Direct3D.Mesh yMoveMesh;
        private Microsoft.DirectX.Direct3D.Mesh zMoveMesh;
        private Microsoft.DirectX.Direct3D.Mesh xyMoveMesh;
        private Microsoft.DirectX.Direct3D.Mesh zxMoveMesh;
        private Microsoft.DirectX.Direct3D.Mesh zyMoveMesh;

        // rotation meshes - used for both display and mouse picking
        private Microsoft.DirectX.Direct3D.Mesh xRotateMesh;
        private Microsoft.DirectX.Direct3D.Mesh yRotateMesh;
        private Microsoft.DirectX.Direct3D.Mesh zRotateMesh;

        // materials for rendering the above meshes
        Material xMaterial;
        Material yMaterial;
        Material zMaterial;
        Material doubleAxisMaterial;
        Material highlightMaterial;

        private List<Item> affectedItems; // these are the items that will be affected by any transforms we are given.
        #endregion

        #region Public Accesors
        public Vector3 Position { get { return position; } set { position = value; } }
        public Rotation GizRotation { get { return rotation; } set { rotation = value; } }
        public bool Enabled { get { return enabled; } set { enabled = value; } }

        public List<Item> AffectedItems { get { return affectedItems; } 
            set 
            { 
                affectedItems = value;
                SetGizmo();
            }
        }

        public GizmoSelectedAxes SelectedAxes { get { return selectedAxes; } set { selectedAxes = value; } }

        public TransformMode Mode { get { return mode; } set { mode = value; } }

        public bool LocalTransform
        {
            get { return isTransformnLocal; }
            set
            {
                isTransformnLocal = value;
                SetGizmo();
            }
        }
        #endregion

        public TransformGizmo(Device d3dDevice)
        {
            #region Creating Streams From Resources
            MemoryStream x_NullStream = new MemoryStream(Properties.Resources.x_null);
            MemoryStream y_NullStream = new MemoryStream(Properties.Resources.y_null);
            MemoryStream z_NullStream = new MemoryStream(Properties.Resources.z_null);
            MemoryStream x_MoveStream = new MemoryStream(Properties.Resources.x_move);
            MemoryStream y_MoveStream = new MemoryStream(Properties.Resources.y_move);
            MemoryStream z_MoveStream = new MemoryStream(Properties.Resources.z_move);
            MemoryStream xy_MoveStream = new MemoryStream(Properties.Resources.xy_move);
            MemoryStream zx_MoveStream = new MemoryStream(Properties.Resources.zx_move);
            MemoryStream zy_MoveStream = new MemoryStream(Properties.Resources.zy_move);

            MemoryStream x_RotationStream = new MemoryStream(Properties.Resources.x_rotation);
            MemoryStream y_RotationStream = new MemoryStream(Properties.Resources.y_rotation);
            MemoryStream z_RotationStream = new MemoryStream(Properties.Resources.z_rotation);
            #endregion

            #region Temporary ExtendedMaterials
            Microsoft.DirectX.Direct3D.ExtendedMaterial[] xMaterials;
            Microsoft.DirectX.Direct3D.ExtendedMaterial[] yMaterials;
            Microsoft.DirectX.Direct3D.ExtendedMaterial[] zMaterials;
            Microsoft.DirectX.Direct3D.ExtendedMaterial[] doubleAxisMaterials;
            #endregion

            #region Loading Meshes and Materials from Streams
            xMoveMesh = Microsoft.DirectX.Direct3D.Mesh.FromStream(x_MoveStream, MeshFlags.Managed, d3dDevice, out xMaterials);
            yMoveMesh = Microsoft.DirectX.Direct3D.Mesh.FromStream(y_MoveStream, MeshFlags.Managed, d3dDevice, out yMaterials);
            zMoveMesh = Microsoft.DirectX.Direct3D.Mesh.FromStream(z_MoveStream, MeshFlags.Managed, d3dDevice, out zMaterials);

            xNullMesh = Microsoft.DirectX.Direct3D.Mesh.FromStream(x_NullStream, MeshFlags.Managed, d3dDevice);
            yNullMesh = Microsoft.DirectX.Direct3D.Mesh.FromStream(y_NullStream, MeshFlags.Managed, d3dDevice);
            zNullMesh = Microsoft.DirectX.Direct3D.Mesh.FromStream(z_NullStream, MeshFlags.Managed, d3dDevice);

            xyMoveMesh = Microsoft.DirectX.Direct3D.Mesh.FromStream(xy_MoveStream, MeshFlags.Managed, d3dDevice, out doubleAxisMaterials);
            zxMoveMesh = Microsoft.DirectX.Direct3D.Mesh.FromStream(zx_MoveStream, MeshFlags.Managed, d3dDevice);
            zyMoveMesh = Microsoft.DirectX.Direct3D.Mesh.FromStream(zy_MoveStream, MeshFlags.Managed, d3dDevice);

            xRotateMesh = Microsoft.DirectX.Direct3D.Mesh.FromStream(x_RotationStream, MeshFlags.Managed, d3dDevice);
            yRotateMesh = Microsoft.DirectX.Direct3D.Mesh.FromStream(y_RotationStream, MeshFlags.Managed, d3dDevice);
            zRotateMesh = Microsoft.DirectX.Direct3D.Mesh.FromStream(z_RotationStream, MeshFlags.Managed, d3dDevice);

            xMaterial = new SAModel.Material() { DiffuseColor = xMaterials[0].Material3D.Diffuse, Exponent = 0f, UseTexture = false, IgnoreLighting = true, IgnoreSpecular = true };
            yMaterial = new SAModel.Material() { DiffuseColor = yMaterials[0].Material3D.Diffuse, Exponent = 0f, UseTexture = false, IgnoreLighting = true, IgnoreSpecular = true };
            zMaterial = new SAModel.Material() { DiffuseColor = zMaterials[0].Material3D.Diffuse, Exponent = 0f, UseTexture = false, IgnoreLighting = true, IgnoreSpecular = true };
            doubleAxisMaterial = new SAModel.Material() { DiffuseColor = doubleAxisMaterials[0].Material3D.Diffuse, Exponent = 0f, UseTexture = false, IgnoreLighting = true, IgnoreSpecular = true };
            highlightMaterial = new Material() { DiffuseColor = System.Drawing.Color.LightGoldenrodYellow, Exponent = 0f, UseTexture = false, IgnoreLighting = true, IgnoreSpecular = true };
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

        /// <summary>
        /// Determines which axes have been selected by a hovering mouse.
        /// </summary>
        /// <param name="Near"></param>
        /// <param name="Far"></param>
        /// <param name="Viewport"></param>
        /// <param name="Projection"></param>
        /// <param name="View"></param>
        /// <param name="cam">viewport camera</param>
        /// <returns></returns>
        public GizmoSelectedAxes CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, EditorCamera cam)
        {
            if (!enabled) return GizmoSelectedAxes.NONE;

            MatrixStack transform = new MatrixStack();
            transform.Push();

            //float dist = SonicRetro.SAModel.Direct3D.Extensions.Distance(cam.Position, position) * ((mode == TransformMode.TRANSFORM_ROTATE) ? 1f : 0.0825f);
            float dist = SonicRetro.SAModel.Direct3D.Extensions.Distance(cam.Position, position) * 0.0825f;

            transform.TranslateLocal(position.X, position.Y, position.Z);
            transform.RotateXYZLocal(rotation.X, rotation.Y, rotation.Z);
            transform.ScaleLocal(Math.Abs(dist), Math.Abs(dist), Math.Abs(dist));

            Vector3 pos = Vector3.Unproject(Near, Viewport, Projection, View, transform.Top);
            Vector3 dir = Vector3.Subtract(pos, Vector3.Unproject(Far, Viewport, Projection, View, transform.Top));
            IntersectInformation info;

            switch(mode)
            {
                case(TransformMode.TRANFORM_MOVE):
                    if (xMoveMesh.Intersect(pos, dir, out info)) return GizmoSelectedAxes.X_AXIS;
                    else if (yMoveMesh.Intersect(pos, dir, out info)) return GizmoSelectedAxes.Y_AXIS;
                    else if (zMoveMesh.Intersect(pos, dir, out info)) return GizmoSelectedAxes.Z_AXIS;
                    else if (xyMoveMesh.Intersect(pos, dir, out info)) return GizmoSelectedAxes.XY_AXIS;
                    else if (zxMoveMesh.Intersect(pos, dir, out info)) return GizmoSelectedAxes.XZ_AXIS;
                    else if (zyMoveMesh.Intersect(pos, dir, out info)) return GizmoSelectedAxes.ZY_AXIS;
                    break;

                case(TransformMode.TRANSFORM_ROTATE):
                    if (xRotateMesh.Intersect(pos, dir, out info)) return GizmoSelectedAxes.X_AXIS;
                    if (yRotateMesh.Intersect(pos, dir, out info)) return GizmoSelectedAxes.Y_AXIS;
                    if (zRotateMesh.Intersect(pos, dir, out info)) return GizmoSelectedAxes.Z_AXIS;
                    break;

                default:
                    selectedAxes = GizmoSelectedAxes.NONE;
                    break;
            }

            return GizmoSelectedAxes.NONE;
        }

        /// <summary>
        /// Draws the gizmo onscreen.
        /// </summary>
        /// <param name="d3ddevice"></param>
        /// <param name="cam"></param>
        public void Draw(Device d3ddevice, EditorCamera cam)
        {
            d3ddevice.RenderState.ZBufferEnable = true;
            d3ddevice.BeginScene();

            RenderInfo.Draw(Render(d3ddevice, new MatrixStack(), cam), d3ddevice, cam);

            d3ddevice.EndScene(); //all drawings before this line
            d3ddevice.Present();
        }

        private RenderInfo[] Render(Device dev, MatrixStack transform, EditorCamera cam)
        {
            List<RenderInfo> result = new List<RenderInfo>();

            if (!enabled) return result.ToArray();

            float dist = SonicRetro.SAModel.Direct3D.Extensions.Distance(cam.Position, position) * 0.0825f;

            #region Setting Render States
            dev.SetSamplerState(0, SamplerStageStates.MinFilter, (int)TextureFilter.Point); // no fancy filtering is required because no textures are even being used
            dev.SetSamplerState(0, SamplerStageStates.MagFilter, (int)TextureFilter.Point);
            dev.SetSamplerState(0, SamplerStageStates.MipFilter, (int)TextureFilter.Point);
            dev.SetRenderState(RenderStates.Lighting, true);
            dev.SetRenderState(RenderStates.SpecularEnable, false);
            dev.SetRenderState(RenderStates.Ambient, System.Drawing.Color.White.ToArgb());
            dev.SetRenderState(RenderStates.AlphaBlendEnable, false);
            dev.SetRenderState(RenderStates.ColorVertex, false);
            dev.Clear(ClearFlags.ZBuffer, 0, 1, 0);
            #endregion

            transform.Push();
            transform.TranslateLocal(position.X, position.Y, position.Z);
            transform.RotateXYZLocal(rotation.X, rotation.Y, rotation.Z);
            transform.ScaleLocal(Math.Abs(dist), Math.Abs(dist), Math.Abs(dist));

            #region Handling Transform Modes
            if (mode == TransformMode.TRANFORM_MOVE)
            {
                BoundingSphere gizmoSphere = new BoundingSphere() { Center = new Vertex(position.X, position.Y, position.Z), Radius = (1.0f * Math.Abs(dist)) };
                Texture gizmoTexture = new Texture(dev, new System.Drawing.Bitmap(2, 2), 0, Pool.Managed);
                RenderInfo xRenderInfo = new RenderInfo(xMoveMesh, 0, transform.Top, (selectedAxes == GizmoSelectedAxes.X_AXIS) ? highlightMaterial : xMaterial, gizmoTexture, FillMode.Solid, gizmoSphere);
                result.Add(xRenderInfo);
                RenderInfo yRenderInfo = new RenderInfo(yMoveMesh, 0, transform.Top, (selectedAxes == GizmoSelectedAxes.Y_AXIS) ? highlightMaterial : yMaterial, gizmoTexture, FillMode.Solid, gizmoSphere);
                result.Add(yRenderInfo);
                RenderInfo zRenderInfo = new RenderInfo(zMoveMesh, 0, transform.Top, (selectedAxes == GizmoSelectedAxes.Z_AXIS) ? highlightMaterial : zMaterial, gizmoTexture, FillMode.Solid, gizmoSphere);
                result.Add(zRenderInfo);

                if(!isTransformnLocal) // this is such a cop-out. I'll try to find a better solution.
                {
                RenderInfo xyRenderInfo = new RenderInfo(xyMoveMesh, 0, transform.Top, (selectedAxes == GizmoSelectedAxes.XY_AXIS) ? highlightMaterial : doubleAxisMaterial, gizmoTexture, FillMode.Solid, gizmoSphere);
                result.Add(xyRenderInfo);
                RenderInfo zxRenderInfo = new RenderInfo(zxMoveMesh, 0, transform.Top, (selectedAxes == GizmoSelectedAxes.XZ_AXIS) ? highlightMaterial : doubleAxisMaterial, gizmoTexture, FillMode.Solid, gizmoSphere);
                result.Add(zxRenderInfo);
                RenderInfo zyRenderInfo = new RenderInfo(zyMoveMesh, 0, transform.Top, (selectedAxes == GizmoSelectedAxes.ZY_AXIS) ? highlightMaterial : doubleAxisMaterial, gizmoTexture, FillMode.Solid, gizmoSphere);
                result.Add(zyRenderInfo);
                }
            }
            else if (mode == TransformMode.TRANSFORM_ROTATE)
            {
                BoundingSphere gizmoSphere = new BoundingSphere() { Center = new Vertex(position.X, position.Y, position.Z), Radius = (1.0f * Math.Abs(dist)) };
                Texture gizmoTexture = new Texture(dev, new System.Drawing.Bitmap(2, 2), 0, Pool.Managed);
                RenderInfo xRenderInfo = new RenderInfo(xRotateMesh, 0, transform.Top, (selectedAxes == GizmoSelectedAxes.X_AXIS) ? highlightMaterial : xMaterial, gizmoTexture, FillMode.Solid, gizmoSphere);
                result.Add(xRenderInfo);
                RenderInfo yRenderInfo = new RenderInfo(yRotateMesh, 0, transform.Top, (selectedAxes == GizmoSelectedAxes.Y_AXIS) ? highlightMaterial : yMaterial, gizmoTexture, FillMode.Solid, gizmoSphere);
                result.Add(yRenderInfo);
                RenderInfo zRenderInfo = new RenderInfo(zRotateMesh, 0, transform.Top, (selectedAxes == GizmoSelectedAxes.Z_AXIS) ? highlightMaterial : zMaterial, gizmoTexture, FillMode.Solid, gizmoSphere);
                result.Add(zRenderInfo);
            }
            else if (mode == TransformMode.NONE)
            {
                BoundingSphere gizmoSphere = new BoundingSphere() { Center = new Vertex(position.X, position.Y, position.Z), Radius = (1.0f * Math.Abs(dist)) };
                Texture gizmoTexture = new Texture(dev, new System.Drawing.Bitmap(2, 2), 0, Pool.Managed);
                RenderInfo xRenderInfo = new RenderInfo(xNullMesh, 0, transform.Top, xMaterial, gizmoTexture, FillMode.Solid, gizmoSphere);
                result.Add(xRenderInfo);
                RenderInfo yRenderInfo = new RenderInfo(yNullMesh, 0, transform.Top, yMaterial, gizmoTexture, FillMode.Solid, gizmoSphere);
                result.Add(yRenderInfo);
                RenderInfo zRenderInfo = new RenderInfo(zNullMesh, 0, transform.Top, zMaterial, gizmoTexture, FillMode.Solid, gizmoSphere);
                result.Add(zRenderInfo);
            }
            #endregion

            transform.Pop();
            return result.ToArray();
        }

        /// <summary>
        /// Sets the Gizmo's transform to the expected values, given the current state.
        /// </summary>
        private void SetGizmo()
        {
            position = Item.CenterFromSelection(affectedItems).ToVector3();
            if ((affectedItems.Count == 1) && isTransformnLocal) rotation = new Rotation(affectedItems[0].Rotation.X, affectedItems[0].Rotation.Y, affectedItems[0].Rotation.Z);
            else rotation = new Rotation();

            if (affectedItems.Count > 0) enabled = true;
            else enabled = false;
        }

        /// <summary>
        /// Transforms the Items that belong to this Gizmo.
        /// </summary>
        /// <param name="xChange">Input for x axis.</param>
        /// <param name="yChange">Input for y axis.</param>
        public void TransformAffected(float xChange, float yChange, EditorCamera camera)
        {
            // don't operate with an invalid axis seleciton, or invalid mode
            if ((selectedAxes == GizmoSelectedAxes.NONE) || (mode == TransformMode.NONE)) return;

            float yFlip = -1; // I don't think we'll ever need to mess with this
            float xFlip = 1; // this though, should get flipped depending on the camera's orientation to the object.

            for (int i = 0; i < affectedItems.Count; i++) // loop through operands
            {
                Item currentItem = affectedItems[i];
                if (mode == TransformMode.TRANFORM_MOVE) // first, check what operation should be performed
                {
                    if (isTransformnLocal) // then check operant space.
                    {
                        Vector3 Up = new Vector3(), Look = new Vector3(), Right = new Vector3();
                        //float xOff = 0.0f, yOff = 0.0f, zOff = 0.0f;
                        Matrix itemMatrix = affectedItems[i].Transform(out Up, out Right, out Look);

                        Vector3 currentPosition = currentItem.Position.ToVector3();
                        Vector3 destination = new Vector3();

                        if (selectedAxes == GizmoSelectedAxes.X_AXIS) destination = (currentPosition + Look * ((Math.Abs(xChange) > Math.Abs(yChange)) ? xChange : yChange));
                        else if (selectedAxes == GizmoSelectedAxes.Y_AXIS) destination = (currentPosition + Right * ((Math.Abs(xChange) > Math.Abs(yChange)) ? xChange : yChange));
                        else if (selectedAxes == GizmoSelectedAxes.Z_AXIS) destination = (currentPosition + Up * ((Math.Abs(xChange) > Math.Abs(yChange)) ? xChange : yChange));

                        currentItem.Position = SonicRetro.SAModel.Direct3D.Extensions.ToVertex(destination);
                    }
                    else
                    {
                        float xOff = 0.0f, yOff = 0.0f, zOff = 0.0f;

                        if (selectedAxes == GizmoSelectedAxes.X_AXIS) xOff = xChange;
                        else if (selectedAxes == GizmoSelectedAxes.Y_AXIS) yOff = yChange * yFlip;
                        else if (selectedAxes == GizmoSelectedAxes.Z_AXIS) zOff = xChange;
                        else if (selectedAxes == GizmoSelectedAxes.XY_AXIS) { xOff = xChange; yOff = yChange * yFlip; }
                        else if (selectedAxes == GizmoSelectedAxes.XZ_AXIS) { xOff = xChange; zOff = yChange; }
                        else if (selectedAxes == GizmoSelectedAxes.ZY_AXIS) { zOff = xChange; yOff = yChange * yFlip; }

                        currentItem.Position = new Vertex(currentItem.Position.X + xOff, currentItem.Position.Y + yOff, currentItem.Position.Z + zOff);
                    }
                }
                else if (mode == TransformMode.TRANSFORM_ROTATE) // first, check what operation should be performed
                {
                    if (currentItem is StartPosItem)
                    {
                        currentItem.Rotation.Y += (int)xChange;
                        continue;
                    }

                    if (isTransformnLocal) // then check operant space.
                    {
                        if (selectedAxes == GizmoSelectedAxes.X_AXIS) currentItem.Rotation.XDeg += (int)xChange;
                        else if (selectedAxes == GizmoSelectedAxes.Y_AXIS) currentItem.Rotation.ZDeg += (int)xChange;
                        else if (selectedAxes == GizmoSelectedAxes.Z_AXIS) currentItem.Rotation.YDeg += (int)xChange;
                    }
                    else
                    {
                        // This code doesn't work - we need to find another way to do global rotations
                        int xOff = 0, yOff = 0, zOff = 0;
                        MatrixStack objTransform = new MatrixStack();

                        if (selectedAxes == GizmoSelectedAxes.X_AXIS) xOff = (int)xChange;
                        else if (selectedAxes == GizmoSelectedAxes.Y_AXIS) yOff = (int)xChange;
                        else if (selectedAxes == GizmoSelectedAxes.Z_AXIS) zOff = (int)xChange;

                        objTransform.Push();
                        //objTransform.RotateXYZLocal(xOff, yOff, zOff);
                        objTransform.RotateXYZLocal(currentItem.Rotation.X, currentItem.Rotation.Y, currentItem.Rotation.Z);

                        Rotation oldRotation = currentItem.Rotation;
                        Rotation newRotation = SAModel.Direct3D.Extensions.FromMatrix(objTransform.Top);

                        // todo: Fix Matrix->Euler conversion, then uncomment the line with the rotatexyzlocal call. Then uncomment the call below and the gizmo should work.
                        //currentItem.Rotation = newRotation;
                    }
                }
            }

            SetGizmo();
        } // end of TransformAffected()
    } // end of TransformGizmo class
} // end of namespace
