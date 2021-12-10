using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.UI;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Windows.Forms;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;

namespace SAModel.SALVL
{
	public partial class MainForm
	{
        private void panel1_KeyUp(object sender, KeyEventArgs e)
        {
            actionInputCollector.KeyUp(e.KeyCode);
        }

        private void panel1_KeyDown(object sender, KeyEventArgs e)
        {
            actionInputCollector.KeyDown(e.KeyCode);
        }

        private void ActionInputCollector_OnActionRelease(ActionInputCollector sender, string actionName)
        {
            if (!isStageLoaded)
                return;

            bool draw = false; // should the scene redraw after this action

            switch (actionName)
            {
                case ("Camera Mode"):
                    cam.mode = (cam.mode + 1) % 2;
                    string cammode = "Normal";
                    if (cam.mode == 1)
                    {
                        cammode = "Orbit";
                        if (selectedItems.GetSelection().Count > 0)
                            cam.FocalPoint = Item.CenterFromSelection(selectedItems.GetSelection()).ToVector3();
                        else
                            cam.FocalPoint = cam.Position += cam.Look * cam.Distance;
                    }
                    osd.UpdateOSDItem("Camera mode: " + cammode, RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
                    draw = true;
                    break;

                case ("Zoom to target"):
                    if (selectedItems.ItemCount > 1)
                    {
                        BoundingSphere combinedBounds = selectedItems.GetSelection()[0].Bounds;

                        for (int i = 0; i < selectedItems.ItemCount; i++)
                        {
                            combinedBounds = Direct3D.Extensions.Merge(combinedBounds, selectedItems.GetSelection()[i].Bounds);
                        }
                        cam.MoveToShowBounds(combinedBounds);
                    }
                    else if (selectedItems.ItemCount == 1)
                    {
                        cam.MoveToShowBounds(selectedItems.GetSelection()[0].Bounds);
                    }
                    osd.UpdateOSDItem("Camera zoomed to target", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
                    draw = true;
                    break;

                case ("Change Render Mode"):
                    string rendermode = "Solid";
                    if (EditorOptions.RenderFillMode == FillMode.Solid)
                    {
                        EditorOptions.RenderFillMode = FillMode.Point;
                        rendermode = "Point";
                    }
                    else
                    {
                        EditorOptions.RenderFillMode += 1;
                        if (EditorOptions.RenderFillMode == FillMode.Solid) rendermode = "Solid";
                        else rendermode = "Wireframe";
                    }
                    osd.UpdateOSDItem("Render mode: " + rendermode, RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
                    draw = true;
                    break;

                case ("Delete"):
                    foreach (Item item in selectedItems.GetSelection())
                        item.Delete(selectedItems);
                    selectedItems.Clear();
                    draw = true;
                    unsaved = true;
                    break;

                case ("Increase camera move speed"):
                    cam.MoveSpeed += 0.0625f;
                    UpdateTitlebar();
                    osd.UpdateOSDItem("Camera speed: " + cam.MoveSpeed.ToString(), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
                    draw = true;
                    break;

                case ("Decrease camera move speed"):
                    cam.MoveSpeed = Math.Max(0.0625f, cam.MoveSpeed -= 0.0625f);
                    UpdateTitlebar();
                    osd.UpdateOSDItem("Camera speed: " + cam.MoveSpeed.ToString(), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
                    draw = true;
                    break;

                case ("Reset camera move speed"):
                    cam.MoveSpeed = EditorCamera.DefaultMoveSpeed;
                    UpdateTitlebar();
                    osd.UpdateOSDItem("Camera speed: " + cam.MoveSpeed.ToString(), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
                    draw = true;
                    break;

                case ("Reset Camera Position"):
                    if (cam.mode == 0)
                    {
                        cam.Position = new Vector3();
                        osd.UpdateOSDItem("Reset camera position", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
                        draw = true;
                    }
                    break;

                case ("Reset Camera Rotation"):
                    if (cam.mode == 0)
                    {
                        cam.Pitch = 0;
                        cam.Yaw = 0;
                        draw = true;
                        osd.UpdateOSDItem("Reset camera rotation", RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
                        draw = true;
                    }
                    break;

                case ("Next Character"):
                    if (isStageLoaded)
                    {
                        LevelData.Character = (LevelData.Character + 1) % 6;

                        if (LevelData.Character < 0)
                            LevelData.Character = 5;

                        characterToolStripMenuItem.DropDownItems[LevelData.Character].PerformClick();
                    }
                    osd.UpdateOSDItem("Current character: " + LevelData.Character.ToString(), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
                    break;

                case ("Previous Character"):
                    if (isStageLoaded)
                    {
                        int chr = LevelData.Character - 1;
                        if (chr < 0) chr = 5;
                        LevelData.Character = chr;

                        characterToolStripMenuItem.DropDownItems[LevelData.Character].PerformClick();
                        osd.UpdateOSDItem("Current character: " + LevelData.Character.ToString(), RenderPanel.Width, 8, Color.AliceBlue.ToRawColorBGRA(), "gizmo", 120);
                    }
                    break;

                case ("Camera Move"):
                    cameraKeyDown = false;
                    UpdateCameraOSD();
                    break;

                case ("Camera Zoom"):
                    zoomKeyDown = false;
                    UpdateCameraOSD();
                    break;

                case ("Camera Look"):
                    lookKeyDown = false;
                    UpdateCameraOSD();
                    break;

                default:
                    break;
            }

            if (draw)
				NeedRedraw = true;
        }

        private void UpdateCameraOSD()
        {
            if (!isStageLoaded) return;
            string cameraMode = "";
            if (cameraKeyDown) cameraMode = "Move";
            else if (zoomKeyDown) cameraMode = "Zoom";
            else if (lookKeyDown) cameraMode = "Look";
            if (cameraMode != "")
                osd.UpdateOSDItem("Camera mode: " + cameraMode, RenderPanel.Width, 32, Color.AliceBlue.ToRawColorBGRA(), "camera", 120);
        }
        private void ActionInputCollector_OnActionStart(ActionInputCollector sender, string actionName)
        {
            switch (actionName)
            {
                case ("Camera Move"):
                    cameraKeyDown = true;
                    UpdateCameraOSD();
                    break;

                case ("Camera Zoom"):
                    zoomKeyDown = true;
                    UpdateCameraOSD();
                    break;

                case ("Camera Look"):
                    lookKeyDown = true;
                    UpdateCameraOSD();
                    break;

                default:
                    break;
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle) actionInputCollector.KeyDown(Keys.MButton);

            if (!isStageLoaded)
                return;

            switch (e.Button)
            {
                // If the mouse button pressed is not one we're looking for,
                // we can avoid re-drawing the scene by bailing out.
                default:
                    return;

                case MouseButtons.Left:
                    Item item;
                    if (isPointOperation)
                    {
                        HitResult res = PickItem(e.Location, out item);
                        if (item != null)
                        {
                            using (PointToDialog dlg = new PointToDialog(res.Position.ToVertex(), item.Position))
                                if (dlg.ShowDialog(this) == DialogResult.OK)
                                    foreach (SETItem it in System.Linq.Enumerable.OfType<SETItem>(selectedItems.GetSelection()))
                                        it.PointTo(dlg.SelectedLocation);
                            isPointOperation = false;

                            LevelData.InvalidateRenderState();
                        }
                        return;
                    }
                    if (transformGizmo != null)
                    {
                        if (moveModeButton.Checked)
                            transformGizmo.Mode = TransformMode.TRANFORM_MOVE;
                        else if (rotateModeButton.Checked)
                            transformGizmo.Mode = TransformMode.TRANSFORM_ROTATE;
                        else if (scaleModeButton.Checked)
                            transformGizmo.Mode = TransformMode.TRANSFORM_SCALE;
                        else
                            transformGizmo.Mode = TransformMode.NONE;
                    }
                    // If we have any helpers selected, don't execute the rest of the method!
                    if (transformGizmo.SelectedAxes != GizmoSelectedAxes.NONE) return;

                    foreach (PointHelper pointHelper in PointHelper.Instances) if (pointHelper.SelectedAxes != GizmoSelectedAxes.NONE) return;

                    PickItem(e.Location, out item);

                    if (item != null)
                    {
                        if (ModifierKeys == Keys.Control)
                        {
                            if (selectedItems.GetSelection().Contains(item))
                                selectedItems.Remove(item);
                            else
                                selectedItems.Add(item);
                        }
                        else if (!selectedItems.GetSelection().Contains(item))
                        {
                            selectedItems.Clear();
                            selectedItems.Add(item);
                        }
                    }
                    else if ((ModifierKeys & Keys.Control) == 0)
                    {
                        selectedItems.Clear();
                    }
                    break;

                case MouseButtons.Right:
                    actionInputCollector.ReleaseKeys();
                    mouseBounds = (wrapAroundScreenEdgesToolStripMenuItem.Checked) ? Screen.GetBounds(ClientRectangle) : RenderPanel.RectangleToScreen(RenderPanel.Bounds);
                    cam.UpdateCamera(new System.Drawing.Point(Cursor.Position.X, Cursor.Position.Y), new System.Drawing.Rectangle(), false, false, false, hideCursorDuringCameraMovementToolStripMenuItem.Checked);

                    if (isPointOperation)
                    {
                        isPointOperation = false;
                        return;
                    }
                    bool cancopy = false;
                    foreach (Item obj in selectedItems.GetSelection())
                    {
                        if (obj.CanCopy)
                            cancopy = true;
                    }
                    if (cancopy)
                    {
                        /*cutToolStripMenuItem.Enabled = true;
						copyToolStripMenuItem.Enabled = true;*/
                        deleteToolStripMenuItem.Enabled = true;

                        cutToolStripMenuItem.Enabled = false;
                        copyToolStripMenuItem.Enabled = false;
                    }
                    else
                    {
                        cutToolStripMenuItem.Enabled = false;
                        copyToolStripMenuItem.Enabled = false;
                        deleteToolStripMenuItem.Enabled = false;
                    }
                    pasteToolStripMenuItem.Enabled = false;
                    menuLocation = e.Location;
                    contextMenuStrip1.Show(RenderPanel, e.Location);
                    break;
            }
		}

        private HitResult PickItem(Point mouse)
        {
            return PickItem(mouse, out Item item);
        }

        private HitResult PickItem(Point mouse, out Item item)
        {
            HitResult closesthit = HitResult.NoHit;
            HitResult hit;
            item = null;
            Vector3 mousepos = new Vector3(mouse.X, mouse.Y, 0);
            Viewport viewport = d3ddevice.Viewport;
            Matrix proj = d3ddevice.GetTransform(TransformState.Projection);
            Matrix view = d3ddevice.GetTransform(TransformState.View);
            Vector3 Near, Far;
            Near = mousepos;
            Near.Z = 0;
            Far = Near;
            Far.Z = -1;

            #region Picking Level Items
            if (LevelData.LevelItems != null)
            {
                for (int i = 0; i < LevelData.LevelItemCount; i++)
                {
                    bool display = false;
                    if (visibleToolStripMenuItem.Checked && LevelData.GetLevelitemAtIndex(i).Visible)
                        display = true;
                    else if (invisibleToolStripMenuItem.Checked && !LevelData.GetLevelitemAtIndex(i).Visible)
                        display = true;
                    else if (allToolStripMenuItem.Checked)
                        display = true;
                    if (display && layer_levelItemsToolStripMenuItem.Checked)
                    {
                        hit = LevelData.GetLevelitemAtIndex(i).CheckHit(Near, Far, viewport, proj, view);
                        if (hit < closesthit)
                        {
                            closesthit = hit;
                            item = LevelData.GetLevelitemAtIndex(i);
                        }
                    }
                }
            }
            #endregion

            #region Picking Start Positions
            if (LevelData.StartPositions != null)
            {
                hit = LevelData.StartPositions[LevelData.Character].CheckHit(Near, Far, viewport, proj, view);
                if (hit < closesthit)
                {
                    closesthit = hit;
                    item = LevelData.StartPositions[LevelData.Character];
                }
            }
            #endregion

            #region Picking SET Items
            if (!LevelData.SETItemsIsNull() && viewSETItemsToolStripMenuItem.Checked && layer__SETItemsToolStripMenuItem.Checked)
                foreach (SETItem setitem in LevelData.SETItems(LevelData.Character))
                {
                    hit = setitem.CheckHit(Near, Far, viewport, proj, view);
                    if (hit < closesthit)
                    {
                        closesthit = hit;
                        item = setitem;
                    }
                }
            #endregion

            #region Picking CAM Items
            if (LevelData.CAMItems != null && viewCAMItemsToolStripMenuItem.Checked && layer_CAMItemsToolStripMenuItem.Checked)
            {
                foreach (CAMItem camItem in LevelData.CAMItems[LevelData.Character])
                {
                    hit = camItem.CheckHit(Near, Far, viewport, proj, view);
                    if (hit < closesthit)
                    {
                        closesthit = hit;
                        item = camItem;
                    }
                }
            }
            #endregion

            #region Picking Death Zones

            if (LevelData.DeathZones != null && layer_deathZonesToolStripMenuItem.Checked)
            {
                foreach (DeathZoneItem dzitem in LevelData.DeathZones)
                {
                    if (dzitem.Visible & viewDeathZonesToolStripMenuItem.Checked)
                    {
                        hit = dzitem.CheckHit(Near, Far, viewport, proj, view);
                        if (hit < closesthit)
                        {
                            closesthit = hit;
                            item = dzitem;
                        }
                    }
                }
            }

            #endregion

            #region Picking Mission SET Items
            if (LevelData.MissionSETItems != null && viewMissionSETItemsToolStripMenuItem.Checked && layer_missionSETItemsToolStripMenuItem.Checked)
                foreach (MissionSETItem setitem in LevelData.MissionSETItems[LevelData.Character])
                {
                    hit = setitem.CheckHit(Near, Far, viewport, proj, view);
                    if (hit < closesthit)
                    {
                        closesthit = hit;
                        item = setitem;
                    }
                }
            #endregion

            #region Picking Splines
            if (LevelData.LevelSplines != null && viewSplinesToolStripMenuItem.Checked && layer_splinesToolStripMenuItem.Checked)
            {
                foreach (SplineData spline in LevelData.LevelSplines)
                {
                    hit = spline.CheckHit(Near, Far, viewport, proj, view);

                    if (hit < closesthit)
                    {
                        closesthit = hit;
                        item = spline;
                    }
                }
            }
            #endregion

            return closesthit;
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle) 
				actionInputCollector.KeyUp(Keys.MButton);
			NeedPropertyRefresh = true;
		}

        private void Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isStageLoaded)
                return;
            bool draw = false;
            bool gizmo = false;
            switch (e.Button)
            {
                case MouseButtons.Middle:
                    break;

                case MouseButtons.Left:
                    if (transformGizmo.TransformGizmoMove(cam.mouseDelta, cam, selectedItems))
                    {
                        unsaved = true;
                        if (transformGizmo.SelectedAxes != GizmoSelectedAxes.NONE) gizmo = true;
                    }
                    foreach (PointHelper pointHelper in PointHelper.Instances)
                    {
                        if (pointHelper.TransformAffected(cam.mouseDelta.X / 2 * cam.MoveSpeed, cam.mouseDelta.Y / 2 * cam.MoveSpeed, cam))
                            unsaved = true;
                    }
                    draw = true;
                    break;

                case MouseButtons.None:
                    Vector3 mousepos = new Vector3(e.X, e.Y, 0);
                    Viewport viewport = d3ddevice.Viewport;
                    Matrix proj = d3ddevice.GetTransform(TransformState.Projection);
                    Matrix view = d3ddevice.GetTransform(TransformState.View);
                    Vector3 Near = mousepos;
                    Near.Z = 0;
                    Vector3 Far = Near;
                    Far.Z = -1;

                    GizmoSelectedAxes oldSelection = transformGizmo.SelectedAxes;
                    transformGizmo.SelectedAxes = transformGizmo.CheckHit(Near, Far, viewport, proj, view, cam);
                    if (oldSelection != transformGizmo.SelectedAxes)
                    {
                        draw = true;
                        break;
                    }

                    foreach (PointHelper pointHelper in PointHelper.Instances)
                    {
                        GizmoSelectedAxes oldHelperAxes = pointHelper.SelectedAxes;
                        pointHelper.SelectedAxes = pointHelper.CheckHit(Near, Far, viewport, proj, view, cam);
						if (oldHelperAxes != pointHelper.SelectedAxes)
						{
							pointHelper.Draw(d3ddevice, cam);
							draw = true;
						}
                    }

                    break;
            }
            mouseBounds = (wrapAroundScreenEdgesToolStripMenuItem.Checked) ? Screen.GetBounds(ClientRectangle) : RenderPanel.RectangleToScreen(RenderPanel.Bounds);
            EditorCamera.CameraUpdateFlags camresult = cam.UpdateCamera(new Point(Cursor.Position.X, Cursor.Position.Y), mouseBounds, lookKeyDown, zoomKeyDown, cameraKeyDown, hideCursorDuringCameraMovementToolStripMenuItem.Checked, gizmo);

            if (camresult.HasFlag(EditorCamera.CameraUpdateFlags.RefreshControls) && selectedItems != null && selectedItems.ItemCount > 0 && propertyGrid1.ActiveControl == null)
				NeedPropertyRefresh = true;
			if (camresult.HasFlag(EditorCamera.CameraUpdateFlags.Redraw) || draw)
				NeedRedraw = true;
		}

        void panel1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!isStageLoaded || !RenderPanel.Focused)
                return;

			if (cam.mode == 0)
				cam.Position += cam.Look * (cam.MoveSpeed * e.Delta * -1);
			else if (cam.mode == 1)
				cam.Distance += (cam.MoveSpeed * e.Delta * -1);
		
			DrawLevel();
			NeedRedraw = true;
        }
    }
}
