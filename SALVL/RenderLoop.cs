using System;
using SAModel.Direct3D;
using Point = System.Drawing.Point;
using System.Runtime.InteropServices;
using SAModel.SAEditorCommon.DataTypes;

namespace SAModel.SALVL
{
	public partial class MainForm
	{
		SAModel.SAEditorCommon.HiResTimer AnimationTimer;
		bool NeedUpdateAnimation;

		void HandleWaitLoop(object sender, EventArgs e)
		{
			if (!isStageLoaded)
				return;
			while (IsApplicationIdle())
			{
				if (NeedUpdateAnimation)
				{
					if (AnimationPlaying && LevelData.geo.Anim != null)
					{
						foreach (GeoAnimData geoanim in LevelData.geo.Anim)
						{
							geoanim.AnimationFrame += geoanim.AnimationSpeed;
							if (geoanim.AnimationFrame >= geoanim.MaxFrame - 1)
								geoanim.AnimationFrame = 0;
						}
						System.Collections.Generic.List<Item> selection = selectedItems.GetSelection();
						if (selection.Count != 0)
						{
							foreach (Item item in selection)
							{
								if (item is LevelAnim)
									NeedPropertyRefresh = true;
								break;
							}
						}
					}
					NeedRedraw = true;
					NeedUpdateAnimation = false;
				}
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

		private void AdvanceAnimation(object obj, SAModel.SAEditorCommon.HiResTimerElapsedEventArgs args)
		{
			NeedUpdateAnimation = true;
		}
	}
}