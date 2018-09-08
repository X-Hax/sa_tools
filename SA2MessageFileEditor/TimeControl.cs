using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;

namespace SA2MessageFileEditor
{
	[Designer(typeof(TimeControlDesigner))]
	[DefaultEvent("ValueChanged")]
	internal partial class TimeControl : UserControl
	{
		public TimeControl()
		{
			InitializeComponent();
		}

		public event EventHandler ValueChanged = delegate { };

		public int Minutes { get { return (int)minutes.Value; } set { minutes.Value = value; ValueChanged(this, EventArgs.Empty); } }
		public int Seconds { get { return (int)seconds.Value; } set { seconds.Value = value; ValueChanged(this, EventArgs.Empty); } }
		public int Centiseconds { get { return (int)centiseconds.Value; } set { centiseconds.Value = value; ValueChanged(this, EventArgs.Empty); } }

		[Browsable(false)]
		public TimeSpan TimeSpan
		{
			get
			{
				return TimeSpan.FromMinutes(Minutes) + TimeSpan.FromSeconds(Seconds) + TimeSpan.FromMilliseconds(Centiseconds * 10);
			}
			set
			{
				Centiseconds = (int)Math.Round(value.Milliseconds / 10.0, MidpointRounding.AwayFromZero);
				Seconds = value.Seconds;
				Minutes = (int)value.TotalMinutes;
			}
		}

		[Browsable(false)]
		public uint TotalCentiseconds
		{
			get
			{
				return (uint)Math.Round(TimeSpan.TotalMilliseconds / 10, MidpointRounding.AwayFromZero);
			}
			set
			{
				TimeSpan = TimeSpan.FromMilliseconds(value * 10.0);
			}
		}

		private const double Frame = 1 / 60.0 * 100;

		[Browsable(false)]
		public uint Frames
		{
			get
			{
				return (uint)Math.Round(Centiseconds / Frame, MidpointRounding.AwayFromZero);
			}
			set
			{
				Centiseconds = (int)Math.Round(value * Frame, MidpointRounding.AwayFromZero);
			}
		}

		[Browsable(false)]
		public int TotalFrames
		{
			get
			{
				return (int)Math.Round(TotalCentiseconds / Frame, MidpointRounding.AwayFromZero);
			}
			set
			{
				TotalCentiseconds = (uint)Math.Round(value * Frame, MidpointRounding.AwayFromZero);
			}
		}
	}

	public class TimeControlDesigner : ControlDesigner
	{
		public override IList SnapLines
		{
			get
			{
				return new ArrayList(base.SnapLines) { new SnapLine(SnapLineType.Baseline, 17) };
			}
		}
	}
}