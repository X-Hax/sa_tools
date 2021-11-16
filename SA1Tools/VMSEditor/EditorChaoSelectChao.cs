using System.Collections.Generic;
using System.Windows.Forms;

namespace VMSEditor
{
	public partial class EditorChaoSelectChao : Form
	{
		List<int> ChaoIndices = new List<int>();
		List<int> EggIndices = new List<int>();
		public int resultChao = -1;
		public int resultEgg = -1;
		public EditorChaoSelectChao(List<VMS_Chao> listData)
		{
			InitializeComponent();
			foreach(VMS_Chao chao in listData)
			{
				switch (chao.Type)
				{
					case ChaoTypeSA1.ChaoEgg:
						listBoxEggs.Items.Add(listData.IndexOf(chao) + ": " + chao.Name + " in " + EditorChao.ToStringEnums(chao.Garden));
						EggIndices.Add(listData.IndexOf(chao));
						break;
					case ChaoTypeSA1.EmptySlot:
						break;
					default:
						listBoxChao.Items.Add(listData.IndexOf(chao) + ": " + chao.Name + " " + EditorChao.ToStringEnums(chao.Type) + " in " + EditorChao.ToStringEnums(chao.Garden));
						ChaoIndices.Add(listData.IndexOf(chao));
						break;
				}
			}
		}

		private void buttonUnselectChao_Click(object sender, System.EventArgs e)
		{
			listBoxChao.SelectedIndex = -1;
		}

		private void buttonUnselectEggs_Click(object sender, System.EventArgs e)
		{
			listBoxEggs.SelectedIndex = -1;
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			resultChao = listBoxChao.SelectedIndex != -1 ? ChaoIndices[listBoxChao.SelectedIndex] : -1;
			resultEgg = listBoxEggs.SelectedIndex != -1 ? EggIndices[listBoxEggs.SelectedIndex] : -1;
			Hide();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			resultChao = -2;
			resultEgg = -2;
			Hide();
		}
	}
}
