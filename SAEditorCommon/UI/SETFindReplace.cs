using System;
using System.Windows.Forms;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
    public partial class SETFindReplace : Form
    {
        private ushort findID;
        private ushort replaceID;

        public SETFindReplace()
        {
            InitializeComponent();

            foreach (ObjectDefinition item in LevelData.ObjDefs)
                findItemDropDown.Items.Add(item.Name);

            foreach (ObjectDefinition item in LevelData.ObjDefs)
                replaceItemDropDown.Items.Add(item.Name);

            findItemDropDown.SelectedIndex = 0;
            replaceItemDropDown.SelectedIndex = 0;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ReplaceTypesButton_Click(object sender, EventArgs e)
        {
            findID = (ushort)findItemDropDown.SelectedIndex;
            replaceID = (ushort)replaceItemDropDown.SelectedIndex;

            bool changed=false;

            for (int itemIndx = 0; itemIndx < LevelData.SETItems[LevelData.Character].Count; itemIndx++)
            {
                if (LevelData.SETItems[LevelData.Character][itemIndx].ID == findID)
                {
                    LevelData.SETItems[LevelData.Character][itemIndx].ID = replaceID;

                    changed = true;
                }
            }

            if (!changed)
            {
                MessageBox.Show("No items found, no replacements made.");
                DialogResult = DialogResult.No;
                Close();
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
