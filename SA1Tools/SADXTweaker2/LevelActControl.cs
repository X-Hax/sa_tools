using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;
using SA_Tools;

namespace SADXTweaker2
{
	[Designer(typeof(LevelActControlDesigner))]
	[DefaultEvent("ValueChanged")]
	public partial class LevelActControl : UserControl
	{
		public LevelActControl()
		{
			InitializeComponent();
		}

		public event EventHandler ValueChanged = delegate { };

		private void LevelActControl_Load(object sender, EventArgs e)
		{
			if (level.SelectedIndex == -1)
				level.SelectedIndex = 0;
		}


		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SA1LevelAct Value
		{
			get { return new SA1LevelAct(level.SelectedIndex, (int)act.Value); }
			set
			{
				level.SelectedIndex = (int)value.Level;
				act.Value = value.Act;
			}
		}

		private void level_SelectedIndexChanged(object sender, EventArgs e)
		{
			ValueChanged(this, e);
		}

		private void act_ValueChanged(object sender, EventArgs e)
		{
			ValueChanged(this, e);
		}
	}

	public class LevelActControlDesigner : ControlDesigner
	{
		public override IList SnapLines
		{
			get
			{
				ArrayList list = new ArrayList(base.SnapLines);
				list.Add(new SnapLine(SnapLineType.Baseline, 15));
				return list;
			}
		}
	}
}