using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ModGenerator
{
    public partial class ModBuildProgress : Form
    {
        public ModBuildProgress()
        {
            InitializeComponent();
        }

        public void SetProgress(string message, int step)
        {
            StepText.Text = message;
            Progress.Step = step;
        }
    }
}
