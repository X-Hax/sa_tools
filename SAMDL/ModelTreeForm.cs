using System.Collections.Generic;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SAMDL
{
    public partial class ModelTreeForm : Form
    {
        public ModelTreeForm(MainForm mainForm)
        {
            InitializeComponent();
			this.mainForm = mainForm;
        }

		MainForm mainForm;
		Dictionary<Object, TreeNode> nodeDict;

		public void Repopulate(Object model)
		{
			treeView1.Nodes.Clear();
			nodeDict = new Dictionary<Object, TreeNode>();
			AddNode(model, treeView1.Nodes);
		}

        private void AddNode(Object model, TreeNodeCollection nodes)
        {
			TreeNode node = nodes.Add(model.Name);
			node.Tag = model;
			nodeDict[model] = node;
            foreach (Object child in model.Children)
                AddNode(child, node.Nodes);
        }

		private void ModelTreeForm_Activated(object sender, System.EventArgs e)
		{
			Opacity = 1;
		}

		private void ModelTreeForm_Deactivate(object sender, System.EventArgs e)
		{
			Opacity = 0.5;
		}

		bool suppressEvent = false;
		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (suppressEvent) return;
			mainForm.selectedObject = (Object)e.Node.Tag;
			mainForm.SelectedItemChanged();
		}

		public void SelectNode(Object model)
		{
			suppressEvent = true;
			treeView1.SelectedNode = nodeDict[model];
			suppressEvent = false;
		}
    }
}
