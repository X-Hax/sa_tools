using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace SASave
{
    [Designer(typeof(HourControlDesigner))]
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
        public uint TotalFrames
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

        [Browsable(false)]
        public LevelTime LevelTime
        {
            get { return new LevelTime((byte)Minutes, (byte)Seconds, (byte)Frames); }
            set { Minutes = value.Minutes; Seconds = value.Seconds; Frames = value.Frames; }
        }

        [Browsable(false)]
        public CircuitTime CircuitTime
        {
            get { return new CircuitTime((byte)Minutes, (byte)Seconds, (byte)Centiseconds); }
            set { Minutes = value.Minutes; Seconds = value.Seconds; Centiseconds = value.Centiseconds; }
        }
    }
}