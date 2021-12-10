using System;
using Point = System.Drawing.Point;
using System.Runtime.InteropServices;

namespace SA2EventViewer
{
	public partial class MainForm
	{
		SAModel.SAEditorCommon.AccurateTimer AnimationTimer;

		void HandleWaitLoop(object sender, EventArgs e)
		{
			if (!loaded)
				return;
			while (IsApplicationIdle())
			{
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
	}
}