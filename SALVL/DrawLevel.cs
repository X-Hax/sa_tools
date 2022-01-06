using SAModel.Direct3D;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.UI;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;

namespace SAModel.SALVL
{
	public partial class MainForm
	{
		internal void DrawLevel()
		{
			if (!isStageLoaded || !Enabled || DeviceResizing)
				return;

			cam.FOV = (float)(Math.PI / 4);
			cam.Aspect = RenderPanel.Width / (float)RenderPanel.Height;
			cam.DrawDistance = 100000;
			UpdateTitlebar();

			#region D3D Parameters
			Matrix projection = Matrix.PerspectiveFovRH(cam.FOV, cam.Aspect, 1, cam.DrawDistance);
			Matrix view = cam.ToMatrix();
			d3ddevice.SetTransform(TransformState.Projection, projection);
			d3ddevice.SetTransform(TransformState.View, view);
			d3ddevice.SetRenderState(RenderState.FillMode, EditorOptions.RenderFillMode);
			d3ddevice.SetRenderState(RenderState.CullMode, EditorOptions.RenderCullMode);
			d3ddevice.Material = new Material { Ambient = System.Drawing.Color.White.ToRawColor4() };
			d3ddevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, EditorOptions.FillColor.ToRawColorBGRA(), 1, 0);
			d3ddevice.SetRenderState(RenderState.ZEnable, true);
			#endregion

			d3ddevice.BeginScene();
			// all drawings after this line
			MatrixStack transform = new MatrixStack();
			EditorOptions.RenderStateCommonSetup(d3ddevice);
			if (LevelData.leveleff != null & viewSkyboxToolStripMenuItem.Checked)
			{
				d3ddevice.SetRenderState(RenderState.FogEnable, false);
				d3ddevice.SetRenderState(RenderState.ZWriteEnable, false);
				LevelData.leveleff.Render(d3ddevice, cam);
				d3ddevice.SetRenderState(RenderState.ZWriteEnable, true);
			}

			#region Fog settings
			if (currentStageFog != null && fogButton.Checked)
			{
				d3ddevice.SetRenderState(RenderState.FogColor, System.Drawing.Color.FromArgb(currentStageFog.A, currentStageFog.R, currentStageFog.G, currentStageFog.B).ToArgb());
				EditorOptions.SetStageFogData(currentStageFog);
				d3ddevice.SetRenderState(RenderState.FogEnable, currentStageFog.FogEnabled);
			}
			#endregion

			List<RenderInfo> renderlist_death = new List<RenderInfo>();
			List<RenderInfo> renderlist_geo = new List<RenderInfo>();
			List<RenderInfo> renderlist_char = new List<RenderInfo>();
			List<RenderInfo> renderlist_set = new List<RenderInfo>();

			#region Adding Level Geometry

			if (LevelData.LevelItems != null)
			{
				foreach (LevelItem item in LevelData.LevelItems)
				{
					bool display = false;

					if (visibleToolStripMenuItem.Checked && item.Visible)
						display = true;
					else if (invisibleToolStripMenuItem.Checked && !item.Visible)
						display = true;
					else if (allToolStripMenuItem.Checked)
						display = true;

					if (display)
						renderlist_geo.AddRange(item.Render(d3ddevice, cam, transform));
				}
			}

			if (LevelData.LevelAnims != null)
			{
				foreach (LevelAnim item in LevelData.LevelAnims)
				{
					bool display = false;

					if (visibleToolStripMenuItem.Checked || allToolStripMenuItem.Checked)
						display = true;

					if (display)
						renderlist_geo.AddRange(item.Render(d3ddevice, cam, transform));
				}
			}
			#endregion

			if (LevelData.StartPositions != null) renderlist_char.AddRange(LevelData.StartPositions[LevelData.Character].Render(d3ddevice, cam, transform));

			#region Adding Death Zones
			if (LevelData.DeathZones != null & viewDeathZonesToolStripMenuItem.Checked)
			{
				foreach (DeathZoneItem item in LevelData.DeathZones)
				{
					if (item.Visible)
						renderlist_death.AddRange(item.Render(d3ddevice, cam, transform));
				}
			}
			#endregion

			#region Adding SET Layout
			if (!LevelData.SETItemsIsNull() && viewSETItemsToolStripMenuItem.Checked)
			{
				foreach (SETItem item in LevelData.SETItems(LevelData.Character))
				{
					bool render = false;
					if (item.ClipSetting == ClipSetting.All)
						render = true;
					else if (item.ClipSetting == ClipSetting.HighOnly && editorDetailSetting == ClipLevel.Far)
						render = true;
					else if (item.ClipSetting == ClipSetting.MediumAndHigh && (editorDetailSetting == ClipLevel.Medium || editorDetailSetting == ClipLevel.Far))
						render = true;
					if (render)
						renderlist_set.AddRange(item.Render(d3ddevice, cam, transform));
				}
			}
			#endregion

			#region Adding CAM Layout
			if (LevelData.CAMItems != null && viewCAMItemsToolStripMenuItem.Checked)
			{
				foreach (CAMItem item in LevelData.CAMItems[LevelData.Character])
					renderlist_set.AddRange(item.Render(d3ddevice, cam, transform));
			}
			#endregion

			#region Adding Mission SET Layout
			if (LevelData.MissionSETItems != null && viewMissionSETItemsToolStripMenuItem.Checked)
			{
				foreach (MissionSETItem item in LevelData.MissionSETItems[LevelData.Character])
					renderlist_set.AddRange(item.Render(d3ddevice, cam, transform));
			}
			#endregion

			#region Adding splines
			if (viewSplinesToolStripMenuItem.Checked)
			{
				foreach (SplineData spline in LevelData.LevelSplines)
					renderlist_set.AddRange(spline.Render(d3ddevice, cam, transform));
			}
			#endregion

			#region Debug Bounds Drawing
			if (boundsToolStripMenuItem.Checked)
			{
				MatrixStack debugBoundsStack = new MatrixStack();
				List<Item> selection = selectedItems.GetSelection();
				debugBoundsStack.Push();
				foreach (Item item in selection)
				{
					if (item is LevelItem)
					{
						LevelItem lvlItem = (LevelItem)item;
						boundsMesh = Direct3D.Mesh.Sphere(lvlItem.CollisionData.Bounds.Radius, 9, 9);
						debugBoundsStack.NJTranslate(lvlItem.CollisionData.Bounds.Center);
						RenderInfo info = new RenderInfo(boundsMesh, 0, debugBoundsStack.Top, boundsMaterial, null, FillMode.Solid, item.Bounds);
						renderlist_geo.Add(info);
					}
					else if (item is SETItem)
					{
						SETItem setitem = (SETItem)item;
						boundsMesh = Direct3D.Mesh.Sphere(setitem.Bounds.Radius, 9, 9);
						debugBoundsStack.NJTranslate(setitem.Bounds.Center);
						RenderInfo info = new RenderInfo(boundsMesh, 0, debugBoundsStack.Top, boundsMaterial, null, FillMode.Solid, item.Bounds);
						renderlist_set.Add(info);
					}
				}
				debugBoundsStack.Pop();
			}
			#endregion

			if (dragType == DragType.Model)
			{
				if (dragPlaceLevelModel != null && dragPlaceLevelMesh != null)
				{
					renderlist_set.AddRange(dragPlaceLevelModel.DrawModel(
						d3ddevice.GetRenderState<FillMode>(RenderState.FillMode),
						transform,
						LevelData.Textures[LevelData.leveltexs],
						dragPlaceLevelMesh,
						true));
				}
			}

			List<RenderInfo> drawqueue = new List<RenderInfo>();

			cam.DrawDistance = Math.Min(EditorOptions.RenderDrawDistance, EditorOptions.LevelDrawDistance);
			drawqueue.AddRange(RenderInfo.Queue(renderlist_geo, cam));

			cam.DrawDistance = Math.Min(EditorOptions.SetItemDrawDistance, EditorOptions.SetItemDrawDistance);
			drawqueue.AddRange(RenderInfo.Queue(renderlist_set, cam));

			cam.DrawDistance = Math.Min(EditorOptions.RenderDrawDistance, EditorOptions.RenderDrawDistance);
			EditorOptions.SetLightType(d3ddevice, EditorOptions.SADXLightTypes.Level);
			RenderInfo.Draw(drawqueue, d3ddevice, cam);

			d3ddevice.SetRenderState(RenderState.FogEnable, false);
			EditorOptions.SetLightType(d3ddevice, EditorOptions.SADXLightTypes.Character);
			RenderInfo.Draw(renderlist_char, d3ddevice, cam);

			d3ddevice.SetRenderState(RenderState.ZWriteEnable, false);
			RenderInfo.Draw(renderlist_death, d3ddevice, cam);
			d3ddevice.SetRenderState(RenderState.ZWriteEnable, true);

			d3ddevice.EndScene(); // scene drawings go before this line

			#region Draw Helper Objects
			cam.DrawDistance = 100000;
			projection = Matrix.PerspectiveFovRH(cam.FOV, cam.Aspect, 1, cam.DrawDistance);
			d3ddevice.SetTransform(TransformState.Projection, projection);
			cam.BuildFrustum(view, projection);

			foreach (PointHelper pointHelper in PointHelper.Instances)
			{
				pointHelper.DrawBox(d3ddevice, cam);
			}

			transformGizmo.Draw(d3ddevice, cam);

			foreach (PointHelper pointHelper in PointHelper.Instances)
			{
				pointHelper.Draw(d3ddevice, cam);
			}

			osd.ProcessMessages();
			#endregion

			d3ddevice.Present();
		}
	}
}