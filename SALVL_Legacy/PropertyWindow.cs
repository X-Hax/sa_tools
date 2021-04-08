using System.Windows.Forms;

namespace SonicRetro.SAModel.SALVL
{
	public partial class PropertyWindow : Form
	{
		public PropertyWindow()
		{
			InitializeComponent();
		}

		private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			LevelData.MainForm.DrawLevel();
		}
	}
}
