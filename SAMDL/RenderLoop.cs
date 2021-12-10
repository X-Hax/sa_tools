using System;
using SAModel.Direct3D;
using Point = System.Drawing.Point;
using System.Runtime.InteropServices;

namespace SAModel.SAMDL
{
	public partial class MainForm
	{
		SAModel.SAEditorCommon.AccurateTimer AnimationTimer;

		public void HandleWaitLoop(object sender, EventArgs e)
		{
			if (!loaded)
				return;
			while (IsApplicationIdle())
			{
				if (hasWeight)
				{
					if (animation != null)
						model.UpdateWeightedModelAnimated(new MatrixStack(), animation, animframe, meshes);
					else
						model.UpdateWeightedModel(new MatrixStack(), meshes);
				}
				if (!NeedRedraw)
					return;
				DrawEntireModel();
				UpdateStatusString();
				NeedRedraw = false;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct NativeMessage
		{
			public IntPtr Handle;
			public uint Message;
			public IntPtr WParameter;
			public IntPtr LParameter;
			public uint Time;
			public Point Location;
		}

		[DllImport("user32.dll")]
		public static extern int PeekMessage(out NativeMessage message, IntPtr window, uint filterMin, uint filterMax, uint remove);

		bool IsApplicationIdle()
		{
			NativeMessage result;
			return PeekMessage(out result, IntPtr.Zero, (uint)0, (uint)0, (uint)0) == 0;
		}

		private void AdvanceAnimation()
		{
			if (AnimationPlaying && animation != null)
			{
				animframe += animspeed * 0.96f; // 0.96 because the 16ms timer is 62.5 FPS
				if (animframe >= animation.Frames)
					animframe -= animation.Frames;
				else if (animframe < 0)
					animframe += animation.Frames;
				NeedRedraw = true;
			}
		}
	}
}