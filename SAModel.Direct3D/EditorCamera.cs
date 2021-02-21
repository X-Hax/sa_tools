using SharpDX;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SonicRetro.SAModel.Direct3D
{
	public class EditorCamera
	{
		public float Distance { get; set; }
		public Vector3 Position { get; set; }
		public ushort Yaw { get; set; }
		public ushort Pitch { get; set; }
		public int Roll { get; set; }
		public Vector3 Up { get; private set; }
		public Vector3 Look { get; private set; }
		public Vector3 Right { get; private set; }
		public int mode { get; set; }
		public int ModifierKey { get; set; }
		private bool MouseCursorHidden { get; set; }
		public Vector3 FocalPoint { get; set; }
		public Vector3 Direction { get; set; }
		public float DrawDistance { get; set; }
		public float MoveSpeed { get; set; }
		public float RotateSpeed { get; set; }

		public System.Drawing.Point mouseLast { get; set; }
		private System.Drawing.Point mouseBackup { get; set; }
		public System.Drawing.Point mouseDelta { get; set; }
		public float FOV { get; set; }
		public float Aspect { get; set; }
		public float HNear { get; set; }
		public float WNear { get; set; }
		public float HFar { get; set; }
		public float WFar { get; set; }

		#region Frustum Vars
		private Vertex farTopLeft, farTopRight, farBottomLeft, farBottomRight;
		private Vertex nearTopLeft, nearTopRight, nearBottomLeft, nearBottomRight;

		Plane[] frustumPlanes = new Plane[6];
		#endregion

		public static float DefaultMoveSpeed = 1.125f;

		public EditorCamera(float drawDistance)
		{
			RotateSpeed = 1.5f;
			MoveSpeed = DefaultMoveSpeed;
			Distance = 20;
			DrawDistance = drawDistance;
		}

		public Matrix ToMatrix()
		{
			Matrix ViewMatrix = Matrix.Identity;
			switch (mode)
			{
				default: // normal
					Up = new Vector3(0, 1, 0);
					Look = new Vector3(0, 0, 1);
					Right = new Vector3(1, 0, 0);
					Matrix yawm = Matrix.RotationAxis(Up, Extensions.BAMSToRad(Yaw));
					Look = Vector3.TransformCoordinate(Look, yawm);
					Right = Vector3.TransformCoordinate(Right, yawm);
					Matrix pitchm = Matrix.RotationAxis(Right, Extensions.BAMSToRad(Pitch));
					Look = Vector3.TransformCoordinate(Look, pitchm);
					Up = Vector3.TransformCoordinate(Up, pitchm);
					ViewMatrix.M11 = Right.X;
					ViewMatrix.M12 = Up.X;
					ViewMatrix.M13 = Look.X;
					ViewMatrix.M21 = Right.Y;
					ViewMatrix.M22 = Up.Y;
					ViewMatrix.M23 = Look.Y;
					ViewMatrix.M31 = Right.Z;
					ViewMatrix.M32 = Up.Z;
					ViewMatrix.M33 = Look.Z;
					ViewMatrix.M41 = -Vector3.Dot(Position, Right);
					ViewMatrix.M42 = -Vector3.Dot(Position, Up);
					ViewMatrix.M43 = -Vector3.Dot(Position, Look);
					break;
				case 1: // orbit
					Vector3 distvec = Vector3.TransformCoordinate(new Vector3(0, 0, Distance), Matrix.RotationYawPitchRoll(Extensions.BAMSToRad(Yaw), Extensions.BAMSToRad(Pitch), 0));
					Position = distvec + FocalPoint;
					ViewMatrix = Matrix.LookAtRH(Position, FocalPoint, new Vector3(0, 1, 0));
					Up = new Vector3(ViewMatrix.M12, ViewMatrix.M22, ViewMatrix.M32);
					Look = new Vector3(ViewMatrix.M13, ViewMatrix.M23, ViewMatrix.M33);
					Right = new Vector3(ViewMatrix.M11, ViewMatrix.M21, ViewMatrix.M31);
					break;
				case 2: // ninja
					float bams_sin = Extensions.NJSin(Roll);
					float bams_cos = Extensions.NJCos(-Roll);
					float thing = Direction.X * Direction.X + Direction.Z * Direction.Z;
					double sqrt = Math.Sqrt(thing);
					float v3 = Direction.Y * Direction.Y + thing;
					double v4 = 1.0 / Math.Sqrt(v3);
					double sqrt__ = sqrt * v4;
					double sqrt___ = v4 * Direction.Y;
					double v7, v8;
					if (thing <= 0.000001)
					{
						v7 = 1.0;
						v8 = 0.0;
					}
					else
					{
						double v5 = 1.0 / Math.Sqrt(thing);
						double v6 = v5;
						v7 = v5 * Direction.Z;
						v8 = -(v6 * Direction.X);
					}
					double v9 = sqrt___ * v8;
					ViewMatrix.M14 = 0;
					ViewMatrix.M23 = (float)sqrt___;
					ViewMatrix.M24 = 0;
					ViewMatrix.M34 = 0;
					ViewMatrix.M11 = (float)(v7 * bams_cos - v9 * bams_sin);
					ViewMatrix.M12 = (float)(v9 * bams_cos + v7 * bams_sin);
					ViewMatrix.M13 = -(float)(sqrt__ * v8);
					ViewMatrix.M21 = -(float)(sqrt__ * bams_sin);
					ViewMatrix.M22 = (float)(sqrt__ * bams_cos);
					double v10 = v7 * sqrt___;
					ViewMatrix.M31 = (float)(bams_sin * v10 + v8 * bams_cos);
					ViewMatrix.M32 = (float)(v8 * bams_sin - v10 * bams_cos);
					ViewMatrix.M33 = (float)(v7 * sqrt__);
					ViewMatrix.M41 = -(ViewMatrix.M31 * Position.Z) - ViewMatrix.M11 * Position.X - ViewMatrix.M21 * Position.Y;
					ViewMatrix.M42 = -(ViewMatrix.M32 * Position.Z) - ViewMatrix.M12 * Position.X - ViewMatrix.M22 * Position.Y;
					float v12 = -(ViewMatrix.M33 * Position.Z) - ViewMatrix.M13 * Position.X;
					double v13 = sqrt___ * Position.Y;
					ViewMatrix.M44 = 1;
					ViewMatrix.M43 = (float)(v12 - v13);
					break;
			}
			return ViewMatrix;
		}

		/// <summary>
		/// Calculates the near and far plane dimensions, as well as the vertices for the entire frustum.
		/// </summary>
		/// <param name="viewMatrix"></param>
		public void BuildFrustum(Matrix view, Matrix projection)
		{
			Matrix viewMatrix = Matrix.Multiply(view, projection);

			#region Calculating vertex representation of Frustum
			// This might not even match the planes below, since these weren't used to calculate them. 
			// in fact, this code probably isn't even necessary. MainMemory if you see this, yes you can ask me to remove it.
			HNear = 2 * (float)Math.Tan(FOV / 2) * 1f;
			WNear = HNear * Aspect;

			HFar = 2 * (float)Math.Tan(FOV / 2) * DrawDistance;
			WFar = HFar * Aspect;

			Vector3 fc = (Position + Look) * DrawDistance;
			Vector3 ftl = fc + (Up * (HFar / 2)) - (Right * (WFar / 2));
			farTopLeft = ftl.ToVertex();

			Vector3 ftr = fc + (Up * (HFar / 2)) + (Right * (WFar / 2));
			farTopRight = ftr.ToVertex();
			Vector3 fbl = fc - (Up * (HFar / 2)) - (Right * (WFar / 2));
			farBottomLeft = fbl.ToVertex();
			Vector3 fbr = fc - (Up * (HFar / 2)) + (Right * (WFar / 2));
			farBottomRight = fbr.ToVertex();

			Vector3 nc = Position + Look * 1f;

			Vector3 ntl = nc + (Up * (HNear / 2)) - (Right * (WNear / 2));
			nearTopLeft = ntl.ToVertex();
			Vector3 ntr = nc + (Up * (HNear / 2)) + (Right * (WNear / 2));
			nearTopRight = ntr.ToVertex();
			Vector3 nbl = nc - (Up * (HNear / 2)) - (Right * (WNear / 2));
			nearBottomLeft = nbl.ToVertex();
			Vector3 nbr = nc - (Up * (HNear / 2)) + (Right * (WNear / 2));
			nearBottomRight = nbr.ToVertex();
			#endregion

			#region Building Frustum Planes
			// Left plane
			frustumPlanes[0].Normal.X = viewMatrix.M14 + viewMatrix.M11;
			frustumPlanes[0].Normal.Y = viewMatrix.M24 + viewMatrix.M21;
			frustumPlanes[0].Normal.Z = viewMatrix.M34 + viewMatrix.M31;
			frustumPlanes[0].D = viewMatrix.M44 + viewMatrix.M41;

			// Right plane
			frustumPlanes[1].Normal.X = viewMatrix.M14 - viewMatrix.M11;
			frustumPlanes[1].Normal.Y = viewMatrix.M24 - viewMatrix.M21;
			frustumPlanes[1].Normal.Z = viewMatrix.M34 - viewMatrix.M31;
			frustumPlanes[1].D = viewMatrix.M44 - viewMatrix.M41;

			// Top plane
			frustumPlanes[2].Normal.X = viewMatrix.M14 - viewMatrix.M12;
			frustumPlanes[2].Normal.Y = viewMatrix.M24 - viewMatrix.M22;
			frustumPlanes[2].Normal.Z = viewMatrix.M34 - viewMatrix.M32;
			frustumPlanes[2].D = viewMatrix.M44 - viewMatrix.M42;

			// Bottom plane
			frustumPlanes[3].Normal.X = viewMatrix.M14 + viewMatrix.M12;
			frustumPlanes[3].Normal.Y = viewMatrix.M24 + viewMatrix.M22;
			frustumPlanes[3].Normal.Z = viewMatrix.M34 + viewMatrix.M32;
			frustumPlanes[3].D = viewMatrix.M44 + viewMatrix.M42;

			// Near plane
			frustumPlanes[4].Normal.X = viewMatrix.M13;
			frustumPlanes[4].Normal.Y = viewMatrix.M23;
			frustumPlanes[4].Normal.Z = viewMatrix.M33;
			frustumPlanes[4].D = viewMatrix.M43;

			// Far plane
			frustumPlanes[5].Normal.X = viewMatrix.M14 - viewMatrix.M13;
			frustumPlanes[5].Normal.Y = viewMatrix.M24 - viewMatrix.M23;
			frustumPlanes[5].Normal.Z = viewMatrix.M34 - viewMatrix.M33;
			frustumPlanes[5].D = viewMatrix.M44 - viewMatrix.M43;
			#endregion

			// Normalize planes
			for (int i = 0; i < 6; i++)
			{
				frustumPlanes[i].Normalize();
			}
		}

		public bool SphereInFrustum(BoundingSphere sphere)
		{
			for (int i = 0; i < 6; i++)
			{
				if (Plane.DotCoordinate(frustumPlanes[i], sphere.Center.ToVector3()) + sphere.Radius < 0)
				{
					return false;
				}
			}

			return true;
		}

		public void MoveToShowBounds(BoundingSphere itemVolume)
		{
			if (mode == 0)
			{
				Vector3 finalPosition = itemVolume.Center.ToVector3();

				finalPosition += (Look * (itemVolume.Radius + (itemVolume.Radius / 4)));

				Position = finalPosition;
			}
			else
			{
				FocalPoint = itemVolume.Center.ToVector3();
				Distance = (itemVolume.Radius + (itemVolume.Radius / 4));
			}
		}

		public int UpdateCamera(System.Drawing.Point point, System.Drawing.Rectangle mouseBounds, bool lookKeyDown = false, bool zoomKeyDown = false, bool moveKeyDown = false, bool hideCursor = false, bool moveGizmo = false)
		{
			int result = 0; // 0 - no redraw, 1 - redraw, 2 - redraw + refresh controls, 3 - refresh controls only

			// Check for multiple keys being pressed, and disable the modifier key. This is to allow the same key to be used both independently and as a modifier.
			if (Convert.ToInt32(lookKeyDown) + Convert.ToInt32(moveKeyDown) + Convert.ToInt32(zoomKeyDown) > 1)
			{
				switch (ModifierKey)
				{
					case 0: // Move
						if (moveKeyDown)
							moveKeyDown = false;
						break;
					case 1: // Look
						if (lookKeyDown)
							lookKeyDown = false;
						break;
					case 2: // Zoom
						if (zoomKeyDown)
							zoomKeyDown = false;
						break;
				}
			}

			System.Drawing.Point mouseEvent = point;
			if (mouseLast == System.Drawing.Point.Empty)
			{
				mouseLast = mouseEvent;
				if (MouseCursorHidden)
				{
					MouseCursorHidden = false;
					Cursor.Position = mouseBackup;
					Cursor.Show();
				}
				return 0;
			}

			ushort mouseWrapThreshold = 2;

			mouseDelta = mouseEvent - (Size)mouseLast;
			bool performedWrap = false;

			if (lookKeyDown || zoomKeyDown || moveKeyDown || moveGizmo)
			{
				// Hide mouse cursor in alternative mode
				if (hideCursor && !MouseCursorHidden && !moveGizmo)
				{
					mouseBackup = Cursor.Position;
					MouseCursorHidden = true;
					Cursor.Hide();
				}

				// Wrap X
				if (Cursor.Position.X <= (mouseBounds.Left + mouseWrapThreshold))
				{
					//MessageBox.Show("Wrap X 1");
					Cursor.Position = new System.Drawing.Point(mouseBounds.Right - mouseWrapThreshold, Cursor.Position.Y);
					mouseEvent = new System.Drawing.Point(mouseEvent.X + mouseBounds.Width - mouseWrapThreshold, mouseEvent.Y);
					performedWrap = true;
				}
				else if (Cursor.Position.X >= (mouseBounds.Right - mouseWrapThreshold))
				{
					//MessageBox.Show("Wrap X 2");
					Cursor.Position = new System.Drawing.Point(mouseBounds.Left + mouseWrapThreshold, Cursor.Position.Y);
					mouseEvent = new System.Drawing.Point(mouseEvent.X - mouseBounds.Width + mouseWrapThreshold, mouseEvent.Y);
					performedWrap = true;
				}

				// Wrap Y
				if (Cursor.Position.Y <= (mouseBounds.Top + mouseWrapThreshold))
				{
					//MessageBox.Show("Wrap Y 1");
					Cursor.Position = new System.Drawing.Point(Cursor.Position.X, mouseBounds.Bottom - mouseWrapThreshold);
					mouseEvent = new System.Drawing.Point(mouseEvent.X, mouseEvent.Y + mouseBounds.Height - mouseWrapThreshold);
					performedWrap = true;
				}
				else if (Cursor.Position.Y >= (mouseBounds.Bottom - mouseWrapThreshold))
				{
					//MessageBox.Show("Wrap Y 2");
					Cursor.Position = new System.Drawing.Point(Cursor.Position.X, mouseBounds.Top + mouseWrapThreshold);
					mouseEvent = new System.Drawing.Point(mouseEvent.X, mouseEvent.Y - mouseBounds.Height + mouseWrapThreshold);
					performedWrap = true;
				}

				switch (mode)
				{
					case 0: // Normal camera
						if (zoomKeyDown) // Zoom
						{
							Position += Look * (mouseDelta.Y * MoveSpeed);
							result = 1;
						}
						else if (moveKeyDown) // Move
						{
							Position += Up * (mouseDelta.Y * MoveSpeed);
							Position += Right * (mouseDelta.X * MoveSpeed) * -1;
							result = 1;
						}
						else if (lookKeyDown) // Look
						{
							Yaw = unchecked((ushort)(Yaw - mouseDelta.X * 0x10));
							Pitch = unchecked((ushort)(Pitch - mouseDelta.Y * 0x10));
							result = 1;
						}
						break;
					case 1: // Orbit camera
						if (zoomKeyDown) // Zoom
						{
							Distance += (mouseDelta.Y * MoveSpeed) * 3;
							result = 1;
						}
						else if (moveKeyDown) // Move
						{
							FocalPoint += Up * (mouseDelta.Y * MoveSpeed);
							FocalPoint += Right * (mouseDelta.X * MoveSpeed) * -1;
							result = 1;
						}
						else if (lookKeyDown) // Look
						{
							Yaw = unchecked((ushort)(Yaw - mouseDelta.X * 0x10));
							Pitch = unchecked((ushort)(Pitch - mouseDelta.Y * 0x10));
							result = 1;
						}
						break;
				}

			}

			if (performedWrap || Math.Abs(mouseDelta.X / 2) * MoveSpeed > 0 || Math.Abs(mouseDelta.Y / 2) * MoveSpeed > 0)
			{
				mouseLast = mouseEvent;
				if (lookKeyDown || zoomKeyDown || moveKeyDown) result = 2;
				else result = 3;
			}

			if (MouseCursorHidden && !lookKeyDown && !zoomKeyDown && !moveKeyDown)
			{
				MouseCursorHidden = false;
				Cursor.Position = mouseBackup;
				Cursor.Show();
			}

			return result;
		}
	}
}
