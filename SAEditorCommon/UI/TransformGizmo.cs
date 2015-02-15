using System;
using System.Collections.Generic;
using System.Drawing;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;

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
		TRANSFORM_ROTATE,
		/// <summary>Used to scale cameras and level items. Axis handles will be blocks.</summary>
		TRANSFORM_SCALE
	}

	/// <summary>
	/// This gives the user a handle to click on for movement and rotation operations.
	/// </summary>
	public class TransformGizmo
	{
		#region Private Variables
		private Vector3 position = new Vector3();
		private Rotation rotation = new Rotation();

		private bool enabled = false;
		private bool isTransformLocal = false; // if TRUE,  the gizmo is in Local mode.
		private GizmoSelectedAxes selectedAxes = GizmoSelectedAxes.NONE; // if this value is not NONE and enabled is true, you've gotten yourself into an invalid state.
		private TransformMode mode = TransformMode.NONE;

		private List<Item> affectedItems; // these are the items that will be affected by any transforms we are given.
		#endregion

		#region Public Accesors
		public Vector3 Position { get { return position; } set { position = value; } }
		public Rotation GizRotation { get { return rotation; } set { rotation = value; } }
		public bool Enabled { get { return enabled; } set { enabled = value; } }

		public List<Item> AffectedItems
		{
			get { return affectedItems; }
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
			get { return isTransformLocal; }
			set
			{
				isTransformLocal = value;
				SetGizmo();
			}
		}
		#endregion

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
			if (!enabled)
				return GizmoSelectedAxes.NONE;

			MatrixStack transform = new MatrixStack();
			transform.Push();

			float dist = Direct3D.Extensions.Distance(cam.Position, position) * 0.0825f;

			transform.TranslateLocal(position.X, position.Y, position.Z);
			transform.RotateXYZLocal(rotation.X, rotation.Y, rotation.Z);
			transform.ScaleLocal(Math.Abs(dist), Math.Abs(dist), Math.Abs(dist));

			Vector3 pos = Vector3.Unproject(Near, Viewport, Projection, View, transform.Top);
			Vector3 dir = Vector3.Subtract(pos, Vector3.Unproject(Far, Viewport, Projection, View, transform.Top));
			IntersectInformation info;

			switch (mode)
			{
				case (TransformMode.TRANFORM_MOVE):
					if (Gizmo.XMoveMesh.Intersect(pos, dir, out info))
						return GizmoSelectedAxes.X_AXIS;
					if (Gizmo.YMoveMesh.Intersect(pos, dir, out info))
						return GizmoSelectedAxes.Y_AXIS;
					if (Gizmo.ZMoveMesh.Intersect(pos, dir, out info))
						return GizmoSelectedAxes.Z_AXIS;

					if (!isTransformLocal) // don't even look for these if the transform is local.
					{
						if (Gizmo.XYMoveMesh.Intersect(pos, dir, out info))
							return GizmoSelectedAxes.XY_AXIS;
						if (Gizmo.ZXMoveMesh.Intersect(pos, dir, out info))
							return GizmoSelectedAxes.XZ_AXIS;
						if (Gizmo.ZYMoveMesh.Intersect(pos, dir, out info))
							return GizmoSelectedAxes.ZY_AXIS;
					}
					break;

				case (TransformMode.TRANSFORM_ROTATE):
					if (Gizmo.XRotateMesh.Intersect(pos, dir, out info))
						return GizmoSelectedAxes.X_AXIS;
					if (Gizmo.YRotateMesh.Intersect(pos, dir, out info))
						return GizmoSelectedAxes.Y_AXIS;
					if (Gizmo.ZRotateMesh.Intersect(pos, dir, out info))
						return GizmoSelectedAxes.Z_AXIS;
					break;

				case (TransformMode.TRANSFORM_SCALE):
					if (Gizmo.XScaleMesh.Intersect(pos, dir, out info))
						return GizmoSelectedAxes.X_AXIS;
					if (Gizmo.YScaleMesh.Intersect(pos, dir, out info))
						return GizmoSelectedAxes.Y_AXIS;
					if (Gizmo.ZScaleMesh.Intersect(pos, dir, out info))
						return GizmoSelectedAxes.Z_AXIS;
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
			//d3ddevice.Present();
		}

		// TODO: Consider returning IEnumerable<RenderInfo>
		private RenderInfo[] Render(Device dev, MatrixStack transform, EditorCamera cam)
		{
			List<RenderInfo> result = new List<RenderInfo>();

			if (!enabled)
				return result.ToArray();

			float dist = cam.Position.Distance(position) * 0.0825f;
			BoundingSphere gizmoSphere = new BoundingSphere() { Center = new Vertex(position.X, position.Y, position.Z), Radius = (1.0f * Math.Abs(dist)) };

			#region Setting Render States
			dev.SetSamplerState(0, SamplerStageStates.MinFilter, (int)TextureFilter.Point); // no fancy filtering is required because no textures are even being used
			dev.SetSamplerState(0, SamplerStageStates.MagFilter, (int)TextureFilter.Point);
			dev.SetSamplerState(0, SamplerStageStates.MipFilter, (int)TextureFilter.Point);
			dev.SetRenderState(RenderStates.Lighting, true);
			dev.SetRenderState(RenderStates.SpecularEnable, false);
			dev.SetRenderState(RenderStates.Ambient, Color.White.ToArgb());
			dev.SetRenderState(RenderStates.AlphaBlendEnable, false);
			dev.SetRenderState(RenderStates.ColorVertex, false);
			dev.Clear(ClearFlags.ZBuffer, 0, 1, 0);
			#endregion

			transform.Push();
			transform.TranslateLocal(position.X, position.Y, position.Z);
			transform.RotateXYZLocal(rotation.X, rotation.Y, rotation.Z);
			transform.ScaleLocal(Math.Abs(dist), Math.Abs(dist), Math.Abs(dist));

			#region Handling Transform Modes
			switch (mode)
			{
				case TransformMode.TRANFORM_MOVE:
					{
						RenderInfo xRenderInfo = new RenderInfo(Gizmo.XMoveMesh, 0, transform.Top, (selectedAxes == GizmoSelectedAxes.X_AXIS) ? Gizmo.HighlightMaterial : Gizmo.XMaterial, null, FillMode.Solid, gizmoSphere);
						result.Add(xRenderInfo);
						RenderInfo yRenderInfo = new RenderInfo(Gizmo.YMoveMesh, 0, transform.Top, (selectedAxes == GizmoSelectedAxes.Y_AXIS) ? Gizmo.HighlightMaterial : Gizmo.YMaterial, null, FillMode.Solid, gizmoSphere);
						result.Add(yRenderInfo);
						RenderInfo zRenderInfo = new RenderInfo(Gizmo.ZMoveMesh, 0, transform.Top, (selectedAxes == GizmoSelectedAxes.Z_AXIS) ? Gizmo.HighlightMaterial : Gizmo.ZMaterial, null, FillMode.Solid, gizmoSphere);
						result.Add(zRenderInfo);

						if (!isTransformLocal) // this is such a cop-out. I'll try to find a better solution.
						{
							RenderInfo xyRenderInfo = new RenderInfo(Gizmo.XYMoveMesh, 0, transform.Top, (selectedAxes == GizmoSelectedAxes.XY_AXIS) ? Gizmo.HighlightMaterial : Gizmo.DoubleAxisMaterial, null, FillMode.Solid, gizmoSphere);
							result.Add(xyRenderInfo);
							RenderInfo zxRenderInfo = new RenderInfo(Gizmo.ZXMoveMesh, 0, transform.Top, (selectedAxes == GizmoSelectedAxes.XZ_AXIS) ? Gizmo.HighlightMaterial : Gizmo.DoubleAxisMaterial, null, FillMode.Solid, gizmoSphere);
							result.Add(zxRenderInfo);
							RenderInfo zyRenderInfo = new RenderInfo(Gizmo.ZYMoveMesh, 0, transform.Top, (selectedAxes == GizmoSelectedAxes.ZY_AXIS) ? Gizmo.HighlightMaterial : Gizmo.DoubleAxisMaterial, null, FillMode.Solid, gizmoSphere);
							result.Add(zyRenderInfo);
						}
					}
					break;
				case TransformMode.TRANSFORM_ROTATE:
					{
						RenderInfo xRenderInfo = new RenderInfo(Gizmo.XRotateMesh, 0, transform.Top, (selectedAxes == GizmoSelectedAxes.X_AXIS) ? Gizmo.HighlightMaterial : Gizmo.XMaterial, null, FillMode.Solid, gizmoSphere);
						result.Add(xRenderInfo);
						RenderInfo yRenderInfo = new RenderInfo(Gizmo.YRotateMesh, 0, transform.Top, (selectedAxes == GizmoSelectedAxes.Y_AXIS) ? Gizmo.HighlightMaterial : Gizmo.YMaterial, null, FillMode.Solid, gizmoSphere);
						result.Add(yRenderInfo);
						RenderInfo zRenderInfo = new RenderInfo(Gizmo.ZRotateMesh, 0, transform.Top, (selectedAxes == GizmoSelectedAxes.Z_AXIS) ? Gizmo.HighlightMaterial : Gizmo.ZMaterial, null, FillMode.Solid, gizmoSphere);
						result.Add(zRenderInfo);
					}
					break;
				case TransformMode.NONE:
					{
						RenderInfo xRenderInfo = new RenderInfo(Gizmo.XNullMesh, 0, transform.Top, Gizmo.XMaterial, null, FillMode.Solid, gizmoSphere);
						result.Add(xRenderInfo);
						RenderInfo yRenderInfo = new RenderInfo(Gizmo.YNullMesh, 0, transform.Top, Gizmo.YMaterial, null, FillMode.Solid, gizmoSphere);
						result.Add(yRenderInfo);
						RenderInfo zRenderInfo = new RenderInfo(Gizmo.ZNullMesh, 0, transform.Top, Gizmo.ZMaterial, null, FillMode.Solid, gizmoSphere);
						result.Add(zRenderInfo);
					}
					break;
				case TransformMode.TRANSFORM_SCALE:
					{
						RenderInfo xRenderInfo = new RenderInfo(Gizmo.XScaleMesh, 0, transform.Top, (selectedAxes == GizmoSelectedAxes.X_AXIS) ? Gizmo.HighlightMaterial : Gizmo.XMaterial, null, FillMode.Solid, gizmoSphere);
						result.Add(xRenderInfo);
						RenderInfo yRenderInfo = new RenderInfo(Gizmo.YScaleMesh, 0, transform.Top, (selectedAxes == GizmoSelectedAxes.Y_AXIS) ? Gizmo.HighlightMaterial : Gizmo.YMaterial, null, FillMode.Solid, gizmoSphere);
						result.Add(yRenderInfo);
						RenderInfo zRenderInfo = new RenderInfo(Gizmo.ZScaleMesh, 0, transform.Top, (selectedAxes == GizmoSelectedAxes.Z_AXIS) ? Gizmo.HighlightMaterial : Gizmo.ZMaterial, null, FillMode.Solid, gizmoSphere);
						result.Add(zRenderInfo);
					}
					break;
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
			if (!enabled)
				return;

			position = Item.CenterFromSelection(affectedItems).ToVector3();

			if ((affectedItems.Count == 1) && isTransformLocal)
				rotation = new Rotation(affectedItems[0].Rotation.X, affectedItems[0].Rotation.Y, affectedItems[0].Rotation.Z);
			else
				rotation = new Rotation();

			enabled = affectedItems.Count > 0;
		}

		/// <summary>
		/// Transforms the Items that belong to this Gizmo.
		/// </summary>
		/// <param name="xChange">Input for x axis.</param>
		/// <param name="yChange">Input for y axis.</param>
		public void TransformAffected(float xChange, float yChange)
		{
			// don't operate with an invalid axis seleciton, or invalid mode
			if (!enabled)
				return;
			if ((selectedAxes == GizmoSelectedAxes.NONE) || (mode == TransformMode.NONE))
				return;

			float yFlip = -1; // I don't think we'll ever need to mess with this
			float xFlip = 1; // TODO: this though, should get flipped depending on the camera's orientation to the object.

			for (int i = 0; i < affectedItems.Count; i++) // loop through operands
			{
				Item currentItem = affectedItems[i];
				switch (mode)
				{
					case TransformMode.TRANFORM_MOVE:
						if (isTransformLocal) // then check operant space.
						{
							Vector3 Up = new Vector3(), Look = new Vector3(), Right = new Vector3();
							affectedItems[i].GetLocalAxes(out Up, out Right, out Look);

							Vector3 currentPosition = currentItem.Position.ToVector3();
							Vector3 destination = new Vector3();

							switch (selectedAxes)
							{
								case GizmoSelectedAxes.X_AXIS:
									destination = (currentPosition + Right * ((Math.Abs(xChange) > Math.Abs(yChange)) ? xChange : yChange));
									break;
								case GizmoSelectedAxes.Y_AXIS:
									destination = (currentPosition + Up * ((Math.Abs(xChange) > Math.Abs(yChange)) ? xChange : yChange));
									break;
								case GizmoSelectedAxes.Z_AXIS:
									destination = (currentPosition + Look * ((Math.Abs(xChange) > Math.Abs(yChange)) ? xChange : yChange));
									break;
							}

							currentItem.Position = destination.ToVertex();
						}
						else
						{
							float xOff = 0.0f, yOff = 0.0f, zOff = 0.0f;

							switch (selectedAxes)
							{
								case GizmoSelectedAxes.X_AXIS:
									xOff = xChange;
									break;
								case GizmoSelectedAxes.Y_AXIS:
									yOff = yChange * yFlip;
									break;
								case GizmoSelectedAxes.Z_AXIS:
									zOff = xChange;
									break;
								case GizmoSelectedAxes.XY_AXIS:
									xOff = xChange; yOff = yChange * yFlip;
									break;
								case GizmoSelectedAxes.XZ_AXIS:
									xOff = xChange; zOff = yChange;
									break;
								case GizmoSelectedAxes.ZY_AXIS:
									zOff = xChange; yOff = yChange * yFlip;
									break;
							}

							currentItem.Position = new Vertex(currentItem.Position.X + xOff, currentItem.Position.Y + yOff, currentItem.Position.Z + zOff);
						}

						if (currentItem is LevelItem)
						{
							LevelItem levelItem = (LevelItem)currentItem;
							levelItem.Save();
						}
						break;

					case TransformMode.TRANSFORM_ROTATE:
						if (currentItem is StartPosItem)
						{
							currentItem.Rotation.Y += (int)xChange;
							continue;
						}
						if (currentItem is CAMItem)
						{
							currentItem.Rotation.Y += (int)xChange * 2;
							continue;
						}

						if (isTransformLocal) // then check operant space.
						{
							switch (selectedAxes)
							{
								case GizmoSelectedAxes.X_AXIS:
									currentItem.Rotation.XDeg += (int)xChange;
									break;
								case GizmoSelectedAxes.Y_AXIS:
									currentItem.Rotation.ZDeg += (int)xChange;
									break;
								case GizmoSelectedAxes.Z_AXIS:
									currentItem.Rotation.YDeg += (int)xChange;
									break;
							}
						}
						else
						{
							// This code doesn't work - we need to find another way to do global rotations
							//int xOff = 0, yOff = 0, zOff = 0;
							MatrixStack objTransform = new MatrixStack();

							/*
							switch (selectedAxes)
							{
								case GizmoSelectedAxes.X_AXIS:
									xOff = (int)xChange;
									break;

								case GizmoSelectedAxes.Y_AXIS:
									yOff = (int)xChange;
									break;

								case GizmoSelectedAxes.Z_AXIS:
									zOff = (int)xChange;
									break;
							}
							*/

							objTransform.Push();
							//objTransform.RotateXYZLocal(xOff, yOff, zOff);
							objTransform.RotateXYZLocal(currentItem.Rotation.X, currentItem.Rotation.Y, currentItem.Rotation.Z);

							//Rotation oldRotation = currentItem.Rotation;
							//Rotation newRotation = SAModel.Direct3D.Extensions.FromMatrix(objTransform.Top);

							// todo: Fix Matrix->Euler conversion, then uncomment the line with the rotatexyz call. Then uncomment the call below and the gizmo should work.
							//currentItem.Rotation = newRotation;
						}
						break;

					case TransformMode.TRANSFORM_SCALE:
						if (currentItem is LevelItem)
						{
							LevelItem levelItem = (LevelItem)currentItem;
							switch (selectedAxes)
							{
								case GizmoSelectedAxes.X_AXIS:
									levelItem.CollisionData.Model.Scale = new Vertex(levelItem.CollisionData.Model.Scale.X + xChange, levelItem.CollisionData.Model.Scale.Y, levelItem.CollisionData.Model.Scale.Z);
									break;
								case GizmoSelectedAxes.Y_AXIS:
									levelItem.CollisionData.Model.Scale = new Vertex(levelItem.CollisionData.Model.Scale.X, levelItem.CollisionData.Model.Scale.Y + yChange, levelItem.CollisionData.Model.Scale.Z);
									break;
								case GizmoSelectedAxes.Z_AXIS:
									levelItem.CollisionData.Model.Scale = new Vertex(levelItem.CollisionData.Model.Scale.X, levelItem.CollisionData.Model.Scale.Y, levelItem.CollisionData.Model.Scale.Z + xChange);
									break;
							}
						}
						else if (currentItem is CAMItem)
						{
							CAMItem camItem = (CAMItem)currentItem;
							switch (selectedAxes)
							{
								case GizmoSelectedAxes.X_AXIS:
									camItem.Scale = new Vertex(MathHelper.Clamp(camItem.Scale.X + xChange, 1, float.MaxValue), MathHelper.Clamp(camItem.Scale.Y, 1, float.MaxValue), MathHelper.Clamp(camItem.Scale.Z, 1, float.MaxValue)); // Clamping is to prevent invalid scale valeus (0 or less)
									break;
								case GizmoSelectedAxes.Y_AXIS:
									camItem.Scale = new Vertex(MathHelper.Clamp(camItem.Scale.X, 1, float.MaxValue), MathHelper.Clamp(camItem.Scale.Y + yChange, 1, float.MaxValue), MathHelper.Clamp(camItem.Scale.Z, 1, float.MaxValue));
									break;
								case GizmoSelectedAxes.Z_AXIS:
									camItem.Scale = new Vertex(MathHelper.Clamp(camItem.Scale.X, 1, float.MaxValue), MathHelper.Clamp(camItem.Scale.Y, 1, float.MaxValue), MathHelper.Clamp(camItem.Scale.Z + xChange, 1, float.MaxValue)); // I just realized that Math.Min would work for these. Oh well, gave me an excuse to write a clamp function
									break;
							}
						}
						break;
				}
			}

			SetGizmo();
		} // end of TransformAffected()
	} // end of TransformGizmo class
} // end of namespace
