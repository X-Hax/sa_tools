using System;
using SAModel.Direct3D;
using Point = System.Drawing.Point;
using System.Runtime.InteropServices;

namespace SAModel.SALVL
{
	public partial class MainForm
	{
		void HandleWaitLoop(object sender, EventArgs e)
		{
			if (!isStageLoaded)
				return;
			while (IsApplicationIdle())
			{
				/*
				if (AnimationPlaying && animation != null)
				{
					animframe += animspeed;
					if (animframe >= animation.Frames)
						animframe = 0;
					else if (animframe < 0)
						animframe = animation.Frames - 1;
					NeedRedraw = true;
				}
				*/
				if (NeedRedraw)
				{
					DrawLevel();
					NeedRedraw = false;
				}
				if (!NeedPropertyRefresh)
					return;
				propertyGrid1.Refresh();
				NeedPropertyRefresh = false;
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
	}
}