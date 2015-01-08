using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace SASave
{
    [DefaultEvent("ValueChanged")]
    internal partial class WeightControl : UserControl
    {
        public WeightControl()
        {
            InitializeComponent();
        }

		bool updating = false;
        public event EventHandler ValueChanged = delegate { };

        private WeightControlModes mode;
        public WeightControlModes Mode
        {
            get { return mode; }
            set
            {
                mode = value;
                SuspendLayout();
				panel1.Visible = mode == WeightControlModes.Weight;
				panel2.Visible = mode == WeightControlModes.Time;
                ResumeLayout();
            }
        }

        public ushort Weight1
        {
            get { return (ushort)Math.Round(weight1.Value / 10, MidpointRounding.AwayFromZero); }
            set { weight1.Value = value * 10m; ValueChanged(this, EventArgs.Empty); }
        }

        public ushort Weight2
        {
            get { return (ushort)Math.Round(weight2.Value / 10, MidpointRounding.AwayFromZero); }
            set { weight2.Value = value * 10m; ValueChanged(this, EventArgs.Empty); }
        }

        public ushort Weight3
        {
            get { return (ushort)Math.Round(weight3.Value / 10, MidpointRounding.AwayFromZero); }
            set { weight3.Value = value * 10m; ValueChanged(this, EventArgs.Empty); }
        }

        public ushort[] Weights
        {
            get { return new ushort[] { Weight1, Weight2, Weight3 }; }
            set
			{
				updating = true;
				Weight1 = value[0];
				Weight2 = value[1];
				Weight3 = value[2];
				updating = false;
			}
        }

        public LevelTime Time
        {
            get { return time.LevelTime; }
            set { time.LevelTime = value; }
        }

        private void Time_ValueChanged(object sender, EventArgs e)
        {
			if (!updating) ValueChanged(this, EventArgs.Empty);
        }
    }

    public enum WeightControlModes
    {
        Time,
        Weight
    }
}