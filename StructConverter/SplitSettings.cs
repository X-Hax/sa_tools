using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ModGenerator
{
    public partial class SplitSettings : Form
    {
        private NewProject projectCreationDialog;

        public SplitSettings(NewProject projectCreationDialog)   
        {
            InitializeComponent();
            this.projectCreationDialog = projectCreationDialog;

            // add our options to the flowboxes
            CheckBox sadxSonicExeCheckBox = new CheckBox() { Name = "sonicEXECheckBox", Checked = projectCreationDialog.SplitSonicExe, Text = "Sonic.exe" };
            sadxSonicExeCheckBox.CheckedChanged += sadxSonicExeCheckBox_CheckedChanged;
            sadxFlow.Controls.Add(sadxSonicExeCheckBox);

            foreach(KeyValuePair<string, bool> splitFile in projectCreationDialog.SadxSplitDLLFiles)
            {
                // add a checkbox
                CheckBox dllCheckBox = new CheckBox() { Name = splitFile.Key, Checked = splitFile.Value, Text = splitFile.Key };
                dllCheckBox.CheckedChanged += SADX_DllCheckBox_CheckedChanged;
                sadxFlow.Controls.Add(dllCheckBox);
            }

            // todo: Add sa2's splitter options, once they're known.
            

            SADXOptions.Enabled = projectCreationDialog.SADXRadioButton.Checked;
            SA2Options.Enabled = projectCreationDialog.SA2PCButton.Checked;
        }

        void SADX_DllCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox senderBox = (CheckBox)sender;

            for(int i=0; i < projectCreationDialog.SadxSplitDLLFiles.Length; i++)
            {
                if(projectCreationDialog.SadxSplitDLLFiles[i].Key == senderBox.Name)
                {
                    projectCreationDialog.SadxSplitDLLFiles[i] = new KeyValuePair<string, bool>(senderBox.Name, senderBox.Checked);
                    return;
                }
            }
        }

        void sadxSonicExeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox senderBox = (CheckBox)sender;
            projectCreationDialog.SplitSonicExe = senderBox.Checked;
        }

        private void acceptButton_Click(object sender, EventArgs e)
        {
            // check to see if the user got themselves into a silly situation.
            if(SADXOptions.Enabled)
            {
                int numberOfChecks = 0;

                if (projectCreationDialog.SplitSonicExe) numberOfChecks++;
                foreach(KeyValuePair<string, bool> sadxDLLFile in projectCreationDialog.SadxSplitDLLFiles)
                {
                    if (sadxDLLFile.Value) numberOfChecks++;
                }

                if (numberOfChecks == 0)
                {
                    DialogResult warningResult = MessageBox.Show("If you don't split any data, your mod will be restricted to overriding /system/ folder files and custom source-code mods.", "Warning", MessageBoxButtons.OKCancel);
                    if (warningResult == System.Windows.Forms.DialogResult.OK) this.Close();
                    return;
                }
                else this.Close();
            }
            else if (SA2Options.Enabled)
            {
                throw new System.NotImplementedException(); // todo: add SA2 support.
            }

            this.Close(); // this shouldn't happen, but if it does, this is ok behavior.
        }
    }
}
