using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SAMDL
{
    public partial class ModelTreeForm : Form
    {
        public ModelTreeForm(Object model)
        {
            InitializeComponent();
            AddNode(model, treeView1.Nodes);
        }

        private void AddNode(Object model, TreeNodeCollection nodes)
        {
            nodes = nodes.Add(model.Name).Nodes;
            foreach (Object child in model.Children)
                AddNode(child, nodes);
        }
    }
}
