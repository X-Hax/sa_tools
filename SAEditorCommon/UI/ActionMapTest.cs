using System;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
	public partial class ActionMapTest : Form
	{
		ActionKeyMapping[] actions = new ActionKeyMapping[]
		{
			new ActionKeyMapping()
			{
				MainKey = Keys.F,
				AltKey = Keys.R,
				Name = "PayRespects",
				Description = "Press F to pay respects",
				FireType = ActionFireType.OnPress,
				IsSearchable = true,
				Modifiers = Keys.None,
				Synonyms = new string[] { }
			},
			new ActionKeyMapping()
			{
				MainKey = Keys.Space,
				AltKey = Keys.None,
				Name = "Search",
				Description = "",
				FireType = ActionFireType.OnPress,
				IsSearchable = false,
				Modifiers = Keys.None,
				Synonyms = new string[] { }
			},
			new ActionKeyMapping()
			{
				MainKey = Keys.MButton,
				AltKey = Keys.None,
				Name = "Camera Button",
				IsSearchable = false,
				FireType = ActionFireType.OnHold,
				Modifiers = Keys.None,
				Description = "Starts a camera action (pan/dolly with no modifiers). Use modifier keys to switch to other camera actions.",
				Synonyms = new string[] { }
			}
		};
		ActionInputCollector inputController;

		public ActionMapTest()
		{
			InitializeComponent();
			inputController = new ActionInputCollector();
			inputController.SetActions(actions);
			inputController.OnActionStart += (ActionInputCollector sender, string action) =>
			{
				string outputText = string.Format("{0} pressed!", action);
				textBox1.Text = string.Format("{0}{1}{2}", textBox1.Text, Environment.NewLine, outputText);
			};

			inputController.OnActionRelease += (ActionInputCollector sender, string action) =>
			{
				string outputText = string.Format("{0} released!", action);
				textBox1.Text = string.Format("{0}{1}{2}", textBox1.Text, Environment.NewLine, outputText);
			};

			KeyPreview = true;
		}

		private void ActionMapTest_KeyDown(object sender, KeyEventArgs e)
		{
			inputController.KeyDown(e.KeyCode);
		}

		private void ActionMapTest_KeyUp(object sender, KeyEventArgs e)
		{
			inputController.KeyUp(e.KeyCode);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.BringToFront();
			this.Focus();
		}

		private void ActionMapTest_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle) inputController.KeyDown(Keys.MButton);
		}

		private void ActionMapTest_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle) inputController.KeyUp(Keys.MButton);
		}
	}
}
