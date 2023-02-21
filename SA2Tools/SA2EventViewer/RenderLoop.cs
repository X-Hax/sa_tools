using System;
using Point = System.Drawing.Point;
using System.Runtime.InteropServices;

namespace SA2EventViewer
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
					if (Playing)
						nextframe += animspeed;
					float oldanimframe = animframe;
					animframe = nextframe;
					if (animframe != oldanimframe)
					{
						if (animframe < 0)
						{
							scenenum--;
							if (scenenum == 0)
								scenenum = @event.Scenes.Count - 1;
							animframe = @event.Scenes[scenenum].FrameCount - 1;
							nextframe = (float)Math.Floor(animframe + 1);
						}
						else if (animframe >= @event.Scenes[scenenum].FrameCount)
						{
							scenenum++;
							if (scenenum == @event.Scenes.Count)
								scenenum = 1;
							nextframe = animframe = 0;
						}
					}
					if (animframe >= 0 && Playing)
					{
						evframe += animspeed;
						if (evframe > @event.Scenes[0].FrameCount)
							evframe = 0;
					}
					if (loaded)
						NeedRedraw = true;
					NeedUpdateAnimation = false;
				}
				if (!NeedRedraw)
					return;
				UpdateWeightedModels();
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

		private void AdvanceAnimation(object obj, SAModel.SAEditorCommon.HiResTimerElapsedEventArgs args)
		{
			NeedUpdateAnimation = true;
		}
	}
}