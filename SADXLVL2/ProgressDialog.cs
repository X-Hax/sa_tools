using System;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SADXLVL2
{
	// TODO: Move to SAEditorCommon
	public partial class ProgressDialog : Form
	{
		#region Accessors

		/// <summary>
		/// Gets or sets the current task displayed on the window. (Upper label)
		/// </summary>
		public string Task
		{
			get { return labelTask.Text; }
			set { labelTask.Text = value; }
		}

		/// <summary>
		/// Gets or sets the current step displayed on the window. (Lower label)
		/// </summary>
		public string Step
		{
			get { return labelStep.Text; }
			set { labelStep.Text = value; }
		}

		/// <summary>
		/// Gets or sets the title of the window.
		/// </summary>
		public string Title
		{
			get { return Text; }
			set { Text = value; }
		}

		/// <summary>
		/// Gets or sets the enabled state of the close options.
		/// If true, the auto-close checkbox and OK button will be disabled.
		/// </summary>
		public bool EnableCloseOptions
		{
			get
			{
				return checkAutoClose.Checked;
			}
			set
			{
				checkAutoClose.Enabled = value;
				SetOkEnabledState();
			}
		}

		// HACK: Work around to avoid animation which is not configurable and FAR too slow.
		private int progressValue
		{
			get
			{
				return progressBar.Value;
			}
			set
			{
				++progressBar.Maximum;
				progressBar.Value = value + 1;
				progressBar.Value = value;
				--progressBar.Maximum;
			}
		}

		#endregion

		/// <summary>
		/// Initializes a ProgressDialog which displays the current task, the step in that task, and a progress bar.
		/// </summary>
		/// <param name="title">The title of the window</param>
		/// <param name="max">The number of steps required</param>
		/// <param name="enableCloseOptions">Defines whether or not the close-on-completion checkbox and OK button are usable.</param>
		/// <param name="autoClose">The default close-on-completion option.</param>
		public ProgressDialog(string title, int max = 100, bool enableCloseOptions = false, bool autoClose = true)
		{
			InitializeComponent();

			Text = title;
			progressBar.Maximum = max;
			EnableCloseOptions = enableCloseOptions;
			checkAutoClose.Checked = autoClose;
			labelTask.Text = "";
			labelStep.Text = "";
		}

		/// <summary>
		/// Increments the progress bar by one step.
		/// </summary>
		public void StepProgress()
		{
			if (InvokeRequired)
			{
				Invoke((Action)StepProgress);
			}
			else
			{
				// Not using progressBar.Step() because dirt hacks
				progressValue = progressValue + 1;

				if (EnableCloseOptions)
				{
					if (progressBar.Value == progressBar.Maximum)
					{
						if (checkAutoClose.Checked)
							Close();
						else
							buttonOK.Enabled = true;
					}
				}
			}
		}

		/// <summary>
		/// Sets the current task to display on the window. (Upper label)
		/// </summary>
		/// <param name="text">The string to display as the task.</param>
		public void SetTask(string text)
		{
			if (InvokeRequired)
				Invoke((Action<string>)SetTask, text);
			else
				labelTask.Text = text;
		}

		/// <summary>
		/// Sets the current step to display on the window. (Lower label)
		/// </summary>
		/// <param name="text">The string to display as the step.</param>
		public void SetStep(string text)
		{
			if (InvokeRequired)
				Invoke((Action<string>)SetStep, text);
			else
				labelStep.Text = text;
		}

		/// <summary>
		/// Sets the task and step simultaneously.
		/// Both parameters default to null, so you may also use this to clear them.
		/// </summary>
		/// <param name="task">The string to display as the task. (Upper label)</param>
		/// <param name="step">The string to display as the step. (Lower label)</param>
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

		private void SetOkEnabledState()
		{
			if (progressBar.Value < progressBar.Maximum)
				buttonOK.Enabled = !checkAutoClose.Checked;
		}

		private void checkAutoClose_CheckedChanged(object sender, EventArgs e)
		{
			SetOkEnabledState();
		}

		private void ProgressDialog_Load(object sender, EventArgs e)
		{
			CenterToParent();
			buttonOK.Select();
		}
		private void buttonOK_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
