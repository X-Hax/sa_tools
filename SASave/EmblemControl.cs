using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SASave
{
    [DefaultEvent("CheckedChanged")]
    public partial class EmblemControl : UserControl
    {
        public EmblemControl()
        {
            InitializeComponent();
        }

        public event EventHandler CheckedChanged = delegate { };

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Checked = !Checked;
        }

        private bool @checked;

        [DefaultValue(false)]
        public bool Checked 
        {
            get { return @checked; }
            set
            {
                @checked = value;
                pictureBox1.Image = value ? Properties.Resources.emblem : Properties.Resources.noemblem;
                CheckedChanged(this, EventArgs.Empty);
            }
        }
    }
}