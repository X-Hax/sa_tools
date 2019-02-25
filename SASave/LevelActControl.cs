using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;

namespace SASave
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

		public byte Level
		{
			get { return (byte)level.SelectedIndex; }
			set { level.SelectedIndex = value; }
		}

		public byte Act
		{
			get { return (byte)act.Value; }
			set { act.Value = value; }
		}

		public ushort LevelAct
		{
			get { return (ushort)((Level << 8) | Act); }
			set
			{
				Level = (byte)(value >> 8);
				Act = (byte)(value & 0xFF);
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