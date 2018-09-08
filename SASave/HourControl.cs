using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Collections;
using System.Windows.Forms.Design.Behavior;

namespace SASave
{
	[Designer(typeof(HourControlDesigner))]
	[DefaultEvent("ValueChanged")]
	public partial class HourControl : UserControl
	{
		public HourControl()
		{
			InitializeComponent();
		}

		bool updating = false;
		public event EventHandler ValueChanged = delegate { };

		public int Hours { get { return (int)hours.Value; } set { hours.Value = value; ValueChanged(this, EventArgs.Empty); } }
		public int Minutes { get { return (int)minutes.Value; } set { minutes.Value = value; ValueChanged(this, EventArgs.Empty); } }
		public int Seconds { get { return (int)seconds.Value; } set { seconds.Value = value; ValueChanged(this, EventArgs.Empty); } }
		public int Centiseconds { get { return (int)centiseconds.Value; } set { centiseconds.Value = value; ValueChanged(this, EventArgs.Empty); } }

		[Browsable(false)]
		public TimeSpan TimeSpan
		{
			get
			{
				return TimeSpan.FromHours(Hours) + TimeSpan.FromMinutes(Minutes) + TimeSpan.FromSeconds(Seconds) + TimeSpan.FromMilliseconds(Centiseconds * 10);
			}
			set
			{
				updating = true;
				Centiseconds = (int)Math.Round(value.Milliseconds / 10.0, MidpointRounding.AwayFromZero);
				Seconds = value.Seconds;
				Minutes = value.Minutes;
				Hours = (int)value.TotalHours;
				updating = false;
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
				return (uint)Math.Round(TotalCentiseconds / Frame, MidpointRounding.AwayFromZero);
			}
			set
			{
				TotalCentiseconds = (uint)Math.Round(value * Frame, MidpointRounding.AwayFromZero);
			}
		}

		private void hours_ValueChanged(object sender, EventArgs e)
		{
			if (!updating) ValueChanged(this, EventArgs.Empty);
		}
	}

	public class HourControlDesigner : ControlDesigner
	{
		public override IList SnapLines
		{
			get
			{
				ArrayList list = new ArrayList(base.SnapLines);
				list.Add(new SnapLine(SnapLineType.Baseline, 17));
				return list;
			}
		}
	}
}