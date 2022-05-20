using System;
using SAModel.Direct3D;
using Point = System.Drawing.Point;
using System.Runtime.InteropServices;

namespace SAModel.SAMDL
{
	public partial class MainForm
	{
		SAModel.SAEditorCommon.HiResTimer AnimationTimer;
		bool NeedUpdateAnimation;

		void HandleWaitLoop(object sender, EventArgs e)
		{
			if (!loaded)
				return;
			while (IsApplicationIdle())
			{
				if (NeedUpdateAnimation)
				{
					if (AnimationPlaying && currentAnimation != null)
					{
						animframe += animspeed;
						if (animframe >= currentAnimation.Frames - 1)
							animframe = 0;
						else if (animframe < 0)
							animframe = currentAnimation.Frames - 1;
						NeedRedraw = true;
					}
					NeedUpdateAnimation = false;
				}
				if (hasWeight)
				{
					if (currentAnimation != null)
						model.UpdateWeightedModelAnimated(new MatrixStack(), currentAnimation, animframe, meshes);
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

		private void AdvanceAnimation(object obj, SAEditorCommon.HiResTimerElapsedEventArgs args)
		{
			NeedUpdateAnimation = true;
		}
	}
}