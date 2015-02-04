using System;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SADXLVL2
{
	public partial class ProgressDialog : Form
	{
		public string Task
		{
			get { return labelTask.Text; }
			set { labelTask.Text = value; }
		}

		public string Step
		{
			get { return labelStep.Text; }
			set { labelStep.Text = value; }
		}

		public string Title
		{
			get { return Text; }
			set { Text = value; }
		}

		public ProgressDialog(string title, int max = 100)
		{
			InitializeComponent();

			Text = title;
			progressBar.Maximum = max;
			labelTask.Text = "";
			labelStep.Text = "";
		}

		private void ProgressDialog_Shown(object sender, EventArgs e)
		{
			CenterToParent();
		}

		public void StepProgress()
		{
			if (InvokeRequired)
				Invoke((Action)StepProgress);
			else
				progressBar.PerformStep();
		}

		public void SetTask(string str)
		{
			if (InvokeRequired)
				Invoke((Action<string>)SetTask, str);
			else
				labelTask.Text = str;
		}

		public void SetStep(string str)
		{
			if (InvokeRequired)
				Invoke((Action<string>)SetStep, str);
			else
				labelStep.Text = str;
		}

		public void SetTaskAndStep(string task = null, string step = null)
		{
			if (InvokeRequired)
			{
				Invoke((Action<string, string>)SetTaskAndStep, task, step);
			}
			else
			{
				labelTask.Text = task;
				labelStep.Text = step;
			}
		}

		public void Complete()
		{
			if (InvokeRequired)
				Invoke((Action)Complete);
			else
				progressBar.Value = progressBar.Maximum;
		}

		private void ProgressDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			--progressBar.Value;
			++progressBar.Value;
		}
	}
}
