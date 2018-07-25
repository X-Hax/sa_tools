using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;

namespace ProjectManager
{
    internal enum SplitType
    {
        BinDLL,
        MDL,
        MTN,
        NB
    }

    public partial class SplitUIControl : UserControl
    {
        
        public delegate void JobChangedHandler(SplitUIControl sender);
        public event JobChangedHandler OnJobAdded;
        public event JobChangedHandler OnJobRemoved;

        SplitType splitType = SplitType.BinDLL;

        public SplitUIControl(bool canRemove, bool canAdd)
        {
            InitializeComponent();

            removeSplitJob.Visible = canRemove;
            removeSplitJob.Enabled = canRemove;

            AddJobButton.Enabled = canAdd;
            AddJobButton.Visible = canAdd;

            splitTypeComboBox.SelectedIndex = 0;

            bigEndianCheckbox.Visible = false;
            bigEndianCheckbox.Enabled = false;
        }

        public void DisableAddingNew()
        {
            AddJobButton.Enabled = false;
        }

        public void EnableAddingNew()
        {
            AddJobButton.Enabled = true;
        }

        public bool IsValid()
        {
            return File.Exists(filePathText.Text) && File.Exists(dataMappingPathText.Text) &&
                Directory.Exists(outputFolderPath.Text);
        }

        public string GetFilePath()
        {
            return filePathText.Text;
        }

        public string GetDataMappingPath()
        {
            return dataMappingPathText.Text;
        }

        public string GetOutputFolder()
        {
            return outputFolderPath.Text;
        }

        public bool IsBigEndian()
        {
            return bigEndianCheckbox.Checked ^ bigEndianCheckbox.Visible;
        }

        internal SplitType GetSplitType()
        {
            return splitType;
        }

        private void AddJobButton_Click(object sender, EventArgs e)
        {
            if(OnJobAdded != null)
            {
                JobChangedHandler dispatch = OnJobAdded;

                dispatch(this);
            }
        }

        private void removeSplitJob_Click(object sender, EventArgs e)
        {
            if (OnJobRemoved != null)
            {
                JobChangedHandler dispatch = OnJobRemoved;

                dispatch(this);
            }
        }

        private void fileBrowseButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                if(fileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePathText.Text = fileDialog.FileName;
                }
            }
        }

        private void dataMappingBrowseButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog() { Filter = "Data Mapping|*.ini", Multiselect = false })
            {
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    dataMappingPathText.Text = fileDialog.FileName;
                }
            }
        }

        private void outputFolderBrowseButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    outputFolderPath.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void splitTypeComboBox_DropDownClosed(object sender, EventArgs e)
        {
            splitType = (SplitType)splitTypeComboBox.SelectedIndex;
            bigEndianCheckbox.Visible = splitType != SplitType.BinDLL && splitType != SplitType.NB;
            bigEndianCheckbox.Enabled = bigEndianCheckbox.Visible;
        }
    }
}
