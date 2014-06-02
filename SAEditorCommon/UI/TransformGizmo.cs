using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;

using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.Direct3D;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
    public enum GizmoSelectedAxes { X_AXIS, Y_AXIS, Z_AXIS, XY_AXIS, XZ_AXIS, ZY_AXIS, NONE }

    public enum TransformMode { TRANFORM_MOVE, TRANSFORM_ROTATE }

    /// <summary>
    /// This gives the user a handle to click on 
    /// </summary>
    class TransformGizmo
    {
        private bool enabled = false;
        private bool isRotationLocal = false; // if TRUE,  the gizmo is in Local mode.
        private GizmoSelectedAxes selectedAxes = GizmoSelectedAxes.NONE; // if this value is not NONE and enabled is true, you've gotten yourself into an invalid state.

        // movement meshes - used for both display and mouse picking
        private Mesh xMoveMesh;
        private Mesh yMoveMesh;
        private Mesh zMoveMesh;

        // rotation meshes - used for both display and mouse picking
        private Mesh xRotateMesh;
        private Mesh yRotateMesh;
        private Mesh zRotateMesh;

        private List<Item> affectedItems; // these are the items that will be affected by any transforms we are given.
    }
}
