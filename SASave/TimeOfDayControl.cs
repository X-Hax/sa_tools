using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SASave
{
    [DefaultEvent("ValueChanged")]
    public partial class TimeOfDayControl : UserControl
    {
        public TimeOfDayControl()
        {
            InitializeComponent();
        }

        public event EventHandler ValueChanged = delegate { };

        internal TimesOfDay Value
        {
            get
            {
                if (day.Checked)
                    return TimesOfDay.Day;
                else if (evening.Checked)
                    return TimesOfDay.Evening;
                else
                    return TimesOfDay.Night;
            }
            set
            {
                switch (value)
                {
                    case TimesOfDay.Day:
                        day.Checked = true;
                        break;
                    case TimesOfDay.Evening:
                        evening.Checked = true;
                        break;
                    case TimesOfDay.Night:
                        night.Checked = true;
                        break;
                }
                ValueChanged(this, EventArgs.Empty);
            }
        }

        public void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            ValueChanged(this, EventArgs.Empty);
        }
    }
}