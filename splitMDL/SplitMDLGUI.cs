using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;

namespace splitMDL
{
    public partial class SplitMDLGUI : Form
    {
        public SplitMDLGUI()
        {
            InitializeComponent();
        }

        private void fileBrowseButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                if(fileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePathTextBox.Text = fileDialog.FileName;
                }
            }
        }

        private void animFilesBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog() { Multiselect = true })
            {
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    listBox1.BeginUpdate();
                    listBox1.Items.Clear();
                    
                    foreach (string path in fileDialog.FileNames)
                    {
                        listBox1.Items.Add(path);
                    }

                    listBox1.EndUpdate();
                }
            }
        }

        private void outputFolderBrowseButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog())
            {
                if(folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    outputFolderPath.Text = folderBrowser.SelectedPath;
                }
            }
        }

        private void splitButton_Click(object sender, EventArgs e)
        {
            string[] animationFiles = new string[listBox1.Items.Count];

            for(int i=0; i < listBox1.Items.Count; i++)
            {
                animationFiles[i] = listBox1.Items[i].ToString();
            }

            SplitMDL.Split(bigEndianCheckbox.Checked, filePathTextBox.Text, outputFolderPath.Text,
                animationFiles);
        }
    }
}
