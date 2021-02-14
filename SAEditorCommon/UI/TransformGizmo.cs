using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D9;
using SonicRetro.SAModel.Direct3D;
using Color = System.Drawing.Color;

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

	public enum Pivot
	{
		CenterOfMass,
		Origin
	}

	/// <summary>
	/// This gives the user a handle to click on for movement and rotation operations. Create inherited versions for operating on different kinds of objects.
	/// (Maybe it's a better idea to do this with a single static class like Unity does?)
	/// </summary>
	public class TransformGizmo
	{
		#region Private Variables
		private Vector3 position = new Vector3();
		private Matrix localTransformMatrix = Matrix.Identity;
		private Matrix globalTransformMatrix = Matrix.Identity;

		private bool enabled = false;
		private bool isTransformLocal = false; // if TRUE,  the gizmo is in Local mode.
		private GizmoSelectedAxes selectedAxes = GizmoSelectedAxes.NONE; // if this value is not NONE and enabled is true, you've gotten yourself into an invalid state.
		private TransformMode mode = TransformMode.NONE;
		private Pivot pivot = Pivot.CenterOfMass;
		#endregion

		#region Public Accesors
		public Vector3 Position { get { return position; } }
		public Rotation Rotation { get; }
		public bool Enabled { get { return enabled; } set { enabled = value; } }

		public GizmoSelectedAxes SelectedAxes { get { return selectedAxes; } set { selectedAxes = value; } }

		public TransformMode Mode { get { return mode; } set { mode = value; } }

		public Pivot Pivot { get { return pivot; } set { pivot = value; } }

		public bool LocalTransform
		{
			get { return isTransformLocal; }
			set
			{
				isTransformLocal = value;
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

			float dist = Direct3D.Extensions.Distance(cam.Position, position) * 0.0825f;

			MatrixStack transform = new MatrixStack();

			transform.Push();
			Matrix transformMatrix = (isTransformLocal) ? localTransformMatrix : globalTransformMatrix;

			if (pivot == Pivot.CenterOfMass && isTransformLocal)
			{
				// we'll need to correct for the difference in local transform and current position variable (which will be in world space)
				//Vector3 localMatrixGlobalPosition = Vector3.Zero * localTransformMatrix.;
				Vector3 localPos = (Vector3)Vector3.Transform(Vector3.Zero, localTransformMatrix);
				Vector3 offset = position - localPos;
				transformMatrix = Matrix.Multiply(localTransformMatrix, Matrix.Translation(offset));
			}

			transform.LoadMatrix(transformMatrix);
			transform.NJScale(Math.Abs(dist), Math.Abs(dist), Math.Abs(dist));
			switch (mode)
			{
				case (TransformMode.TRANFORM_MOVE):
					if (Gizmo.XMoveMesh.CheckHit(Near, Far, Viewport, Projection, View, transform).IsHit)
						return GizmoSelectedAxes.X_AXIS;
					if (Gizmo.YMoveMesh.CheckHit(Near, Far, Viewport, Projection, View, transform).IsHit)
						return GizmoSelectedAxes.Y_AXIS;
					if (Gizmo.ZMoveMesh.CheckHit(Near, Far, Viewport, Projection, View, transform).IsHit)
						return GizmoSelectedAxes.Z_AXIS;

					if (!isTransformLocal) // don't even look for these if the transform is local.
					{
						if (Gizmo.XYMoveMesh.CheckHit(Near, Far, Viewport, Projection, View, transform).IsHit)
							return GizmoSelectedAxes.XY_AXIS;
						if (Gizmo.ZXMoveMesh.CheckHit(Near, Far, Viewport, Projection, View, transform).IsHit)
							return GizmoSelectedAxes.XZ_AXIS;
						if (Gizmo.ZYMoveMesh.CheckHit(Near, Far, Viewport, Projection, View, transform).IsHit)
							return GizmoSelectedAxes.ZY_AXIS;
					}
					break;

				case (TransformMode.TRANSFORM_ROTATE):
					if (Gizmo.XRotateMesh.CheckHit(Near, Far, Viewport, Projection, View, transform).IsHit)
						return GizmoSelectedAxes.X_AXIS;
					if (Gizmo.YRotateMesh.CheckHit(Near, Far, Viewport, Projection, View, transform).IsHit)
						return GizmoSelectedAxes.Y_AXIS;
					if (Gizmo.ZRotateMesh.CheckHit(Near, Far, Viewport, Projection, View, transform).IsHit)
						return GizmoSelectedAxes.Z_AXIS;
					break;

				case (TransformMode.TRANSFORM_SCALE):
					if (Gizmo.XScaleMesh.CheckHit(Near, Far, Viewport, Projection, View, transform).IsHit)
						return GizmoSelectedAxes.X_AXIS;
					if (Gizmo.YScaleMesh.CheckHit(Near, Far, Viewport, Projection, View, transform).IsHit)
						return GizmoSelectedAxes.Y_AXIS;
					if (Gizmo.ZScaleMesh.CheckHit(Near, Far, Viewport, Projection, View, transform).IsHit)
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
			d3ddevice.SetRenderState(RenderState.ZEnable, true);
			d3ddevice.BeginScene();

			RenderInfo.Draw(Render(d3ddevice, new MatrixStack(), cam), d3ddevice, cam);

			d3ddevice.EndScene(); //all drawings before this line
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
			dev.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Point); // no fancy filtering is required because no textures are even being used
			dev.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Point);
			dev.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.Point);
			dev.SetRenderState(RenderState.Lighting, true);
			dev.SetRenderState(RenderState.SpecularEnable, false);
			dev.SetRenderState(RenderState.Ambient, Color.White.ToArgb());
			dev.SetRenderState(RenderState.AlphaBlendEnable, false);
			dev.SetRenderState(RenderState.ColorVertex, false);
			dev.Clear(ClearFlags.ZBuffer, Color.Transparent.ToRawColorBGRA(), 1, 0);
			#endregion

			transform.Push();
			Matrix transformMatrix = (isTransformLocal) ? localTransformMatrix : globalTransformMatrix;

			if (pivot == Pivot.CenterOfMass && isTransformLocal)
			{
				// we'll need to correct for the difference in local transform and current position variable (which will be in world space)
				//Vector3 localMatrixGlobalPosition = Vector3.Zero * localTransformMatrix.;
				Vector3 localPos = (Vector3)Vector3.Transform(Vector3.Zero, localTransformMatrix);
				Vector3 offset = position - localPos;
				transformMatrix = Matrix.Multiply(localTransformMatrix, Matrix.Translation(offset));
			}

			transform.LoadMatrix(transformMatrix);
			transform.NJScale(Math.Abs(dist), Math.Abs(dist), Math.Abs(dist));

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

					RenderInfo xyRenderInfo = new RenderInfo(Gizmo.XYMoveMesh, 0, transform.Top, (selectedAxes == GizmoSelectedAxes.XY_AXIS) ? Gizmo.HighlightMaterial : Gizmo.DoubleAxisMaterial, null, FillMode.Solid, gizmoSphere);
					result.Add(xyRenderInfo);
					RenderInfo zxRenderInfo = new RenderInfo(Gizmo.ZXMoveMesh, 0, transform.Top, (selectedAxes == GizmoSelectedAxes.XZ_AXIS) ? Gizmo.HighlightMaterial : Gizmo.DoubleAxisMaterial, null, FillMode.Solid, gizmoSphere);
					result.Add(zxRenderInfo);
					RenderInfo zyRenderInfo = new RenderInfo(Gizmo.ZYMoveMesh, 0, transform.Top, (selectedAxes == GizmoSelectedAxes.ZY_AXIS) ? Gizmo.HighlightMaterial : Gizmo.DoubleAxisMaterial, null, FillMode.Solid, gizmoSphere);
					result.Add(zyRenderInfo);
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

		public void SetGizmo(Vector3 center, Matrix transformMatrix)
		{
			if (!enabled)
				return;

			position = center;

			globalTransformMatrix = Matrix.Identity;
			Vector3 tempPosition = position;
			Matrix.Translation(ref tempPosition, out globalTransformMatrix);
			localTransformMatrix = transformMatrix;
		}

		#region Move Methods
		/// <summary>
		/// Converts an input vector2 into a direction-based movement for a point in space.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="cam"></param>
		/// <param name="Up"></param>
		/// <param name="Look"></param>
		/// <param name="Right"></param>
		/// <returns></returns>
		public Vector3 MoveDirection(Vector2 input, EditorCamera cam/*, Vector3 Up, Vector3 Look, Vector3 Right*/)
		{
			float yFlip = -1; // I don't think we'll ever need to mess with this
			float xFlip = 1;
			float axisDot = 0;

			Vector3 Right = new Vector3(1, 0, 0), Up = new Vector3(0, 1, 0), Look = new Vector3(0, 0, 1);

			Matrix transformMatrix = (isTransformLocal) ? localTransformMatrix : globalTransformMatrix;

			Right = Vector3.TransformNormal(Right, transformMatrix);
			Up = Vector3.TransformNormal(Up, transformMatrix);
			Look = Vector3.TransformNormal(Look, transformMatrix);

			Vector3 offset = new Vector3();

			switch (selectedAxes)
			{
				case GizmoSelectedAxes.X_AXIS:
					axisDot = Vector3.Dot(cam.Look, Right);
					xFlip = (axisDot > 0) ? 1 : -1;
					offset = (Right * ((Math.Abs(input.X) > Math.Abs(input.Y)) ? input.X * xFlip : input.Y));
					break;
				case GizmoSelectedAxes.Y_AXIS:
					offset = (Up * ((Math.Abs(input.X) > Math.Abs(input.Y)) ? input.X : input.Y * yFlip));
					break;
				case GizmoSelectedAxes.Z_AXIS:
					axisDot = Vector3.Dot(cam.Look, Right);
					xFlip = (axisDot > 0) ? -1 : 1;
					offset = (Look * ((Math.Abs(input.X) > Math.Abs(input.Y)) ? input.X * xFlip : input.Y));
					break;
				case GizmoSelectedAxes.XY_AXIS:
					//axisDot = Vector3.Dot(cam.Look, Right); // figure this out later. We'll need to make it camera relative, and 
					//xFlip = (axisDot > 0) ? 1 : -1;
					offset = (Right * (input.X) * xFlip);
					offset += (Up * (input.Y * yFlip));
					break;
				case GizmoSelectedAxes.XZ_AXIS:
					offset = (Right * (input.X) * xFlip);
					offset += (Look * (input.Y * yFlip));
					break;
				case GizmoSelectedAxes.ZY_AXIS:
					offset = (Look * (input.Y * yFlip));
					offset += (Up * (input.Y * yFlip));
					break;
			}

			return offset;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="input">input direction</param>
		/// <param name="sourcePosition">position to move</param>
		/// <param name="cam">reference frame for mover</param>
		/// <param name="Up"></param>
		/// <param name="Look"></param>
		/// <param name="Right"></param>
		/// <returns></returns>
		public Vector3 Move(Vector2 input, Vector3 sourcePosition, EditorCamera cam)
		{
			return position = (sourcePosition + MoveDirection(input, cam));
		}

		public bool TransformGizmoMove(System.Drawing.Point mouseDelta, EditorCamera cam, EditorItemSelection selectedItems)
		{
			bool result = false;

			// Returns false if no actual movement happened
			if (!Enabled) 
				return false;

				float xChange = mouseDelta.X / 2 * cam.MoveSpeed;
				float yChange = mouseDelta.Y / 2 * cam.MoveSpeed;
				if (xChange == 0 && yChange == 0) return false;

				Item firstItem = selectedItems.Get(0);
				Vector2 gizmoMouseInput = new Vector2(xChange, yChange);

			switch (Mode)
			{
				case TransformMode.TRANFORM_MOVE:
					// Move selected items
					foreach (Item item in selectedItems.Items)
					{
						Vertex backuppos = item.Position.Clone();
						item.Position = Move(gizmoMouseInput,
							item.Position.ToVector3(), cam).ToVertex();
						// Update item bounds
						if (item is LevelItem)
						{
							LevelItem levelItem = item as LevelItem;
							Vertex newpos = item.Position;
							levelItem.Bounds.Center.X += newpos.X - backuppos.X;
							levelItem.Bounds.Center.Y += newpos.Y - backuppos.Y;
							levelItem.Bounds.Center.Z += newpos.Z - backuppos.Z;
						}
						result = true;
					}
					// Update gizmo position
					SetGizmo(
						((Pivot == Pivot.CenterOfMass) ? firstItem.Bounds.Center : firstItem.Position).ToVector3(),
						firstItem.TransformMatrix);
					return result;
				case TransformMode.TRANSFORM_ROTATE:
					// rotate all of our editor selected items
					foreach (Item item in selectedItems.Items)
					{
						item.Rotation = Rotate(gizmoMouseInput, cam, item.Rotation);
						result = true;
					}
					SetGizmo(
						((Pivot == Pivot.CenterOfMass) ? firstItem.Bounds.Center : firstItem.Position).ToVector3(),
						firstItem.TransformMatrix);
					return result;
				case TransformMode.TRANSFORM_SCALE:
					// scale all of our editor selected items
					foreach (Item item in selectedItems.Items)
					{
						if (item is IScaleable scalableItem)
						{
							// Scaling speed for SET items
							float speed = 1.0f;

							// Non-SET items should have slower scaling
							if (item is LevelItem || item is DeathZoneItem)
								speed = 0.01f;

							scalableItem.SetScale(Scale(gizmoMouseInput, scalableItem.GetScale(), cam, true, 0, speed));
						}
						result = true;
					}
					SetGizmo(
						((Pivot == Pivot.CenterOfMass) ? firstItem.Bounds.Center : firstItem.Position).ToVector3(),
						firstItem.TransformMatrix);
					return result;
				case TransformMode.NONE:
				default:
					return false;
			}
		}
		#endregion

		#region Rotation Methods
		public Rotation Rotate(Vector2 input, EditorCamera cam, Rotation rotation)
		{
			if (isTransformLocal)
			{
				try
				{
					switch (selectedAxes) // todo: find a way to handle y-axis mouse motion, as well as camera relative-direction
					{
						case GizmoSelectedAxes.X_AXIS:
							rotation.XDeg += (int)input.X;
							break;
						case GizmoSelectedAxes.Y_AXIS:
							rotation.ZDeg += (int)input.X;
							break;
						case GizmoSelectedAxes.Z_AXIS:
							rotation.YDeg += (int)input.X;
							break;
					}
				}
				catch (NotSupportedException)
				{
					Console.WriteLine("Certain Item types don't support rotations. This can be ignored.");
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
				objTransform.NJRotateXYZ(rotation.X, rotation.Y, rotation.Z);

				//Rotation oldRotation = currentItem.Rotation;
				//Rotation newRotation = SAModel.Direct3D.Extensions.FromMatrix(objTransform.Top);

				// todo: Fix Matrix->Euler conversion, then uncomment the line with the rotatexyz call. Then uncomment the call below and the gizmo should work.
				//currentItem.Rotation = newRotation;
			}

			return rotation;
		}
		#endregion

		#region Scale Methods
		/// <summary>
		/// Gets an adjusted scale value based on mouse input.
		/// </summary>
		/// <param name="input">Mouse Delta.</param>
		/// <param name="sourceScale">Input scale value</param>
		/// <param name="cam">Reference camera. Provides extra context for Mouse Delta.</param>
		/// <param name="clamp">If TRUE, minScale is used as the lowest value that the scale can be. Clamp is applied to all dimensions of the vector.</param>
		/// <param name="minScale">Minimum acceptable scale values.</param>
		/// /// <param name="multiplier">Scale multiplier to make scaling faster or slower.</param>
		/// <returns></returns>
		public Vertex Scale(Vector2 input, Vertex sourceScale, EditorCamera cam, bool clamp, float minScale, float multiplier)
		{
			switch (selectedAxes)
			{
				case GizmoSelectedAxes.X_AXIS:
					return new Vertex(MathHelper.Clamp(sourceScale.X + input.X * multiplier, (clamp) ? minScale : float.NegativeInfinity, float.PositiveInfinity),
						MathHelper.Clamp(sourceScale.Y, (clamp) ? minScale : float.NegativeInfinity, float.PositiveInfinity),
						MathHelper.Clamp(sourceScale.Z, (clamp) ? minScale : float.NegativeInfinity, float.PositiveInfinity));

				case GizmoSelectedAxes.Y_AXIS:
					return new Vertex(MathHelper.Clamp(sourceScale.X, (clamp) ? minScale : float.NegativeInfinity, float.PositiveInfinity),
						MathHelper.Clamp(sourceScale.Y + input.Y * multiplier, (clamp) ? minScale : float.NegativeInfinity, float.PositiveInfinity),
						MathHelper.Clamp(sourceScale.Z, (clamp) ? minScale : float.NegativeInfinity, float.PositiveInfinity));

				case GizmoSelectedAxes.Z_AXIS:
					return new Vertex(MathHelper.Clamp(sourceScale.X, (clamp) ? minScale : float.NegativeInfinity, float.PositiveInfinity),
						MathHelper.Clamp(sourceScale.Y, (clamp) ? minScale : float.NegativeInfinity, float.PositiveInfinity),
						MathHelper.Clamp(sourceScale.Z + input.X * multiplier, (clamp) ? minScale : float.NegativeInfinity, float.PositiveInfinity));
			}

			return sourceScale;
		}
		#endregion
	} // end of TransformGizmo class
} // end of namespace
