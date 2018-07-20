using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
    public partial class ActionKeybindControl : UserControl
    {
        public delegate void KeyChangedEvent(string actionName, Keys key);
        public event KeyChangedEvent MainKeyChanged;
        public event KeyChangedEvent AltKeyChanged;
        public event KeyChangedEvent ModifierKeyChanged;

        string[] keyNames;
        string actionName;

        public ActionKeybindControl(string actionName, Keys mainkey, Keys altKey,
            Keys modifierKey, string description)
        {
            InitializeComponent();

            keyNames = Enum.GetNames(typeof(Keys));

            this.actionName = actionName;

            groupBox1.Text = actionName;
            descriptionLabel.Text = string.Format("Description: {0}", description);

            // fill our key controls
            mainKeyDropDown.Items.Clear();
            altKeyDropDown.Items.Clear();
            modifierDropDown.Items.Clear();

            for(int i=0; i < keyNames.Length; i++)
            {
                mainKeyDropDown.Items.Add(keyNames[i]);
                altKeyDropDown.Items.Add(keyNames[i]);
                modifierDropDown.Items.Add(keyNames[i]);
            }

            string mainKeyName = mainkey.ToString();
            mainKeyDropDown.SelectedIndex = Array.FindIndex<string>(keyNames, item => item == mainKeyName);

            string altKeyName = altKey.ToString();
            altKeyDropDown.SelectedIndex = Array.FindIndex<string>(keyNames, item => item == altKeyName);

            string modifierKeyName = modifierKey.ToString();
            modifierDropDown.SelectedIndex = Array.FindIndex<string>(keyNames, item => item == modifierKeyName);

            // subscribe to our events
            mainKeyDropDown.SelectedIndexChanged += MainKeyDropDown_SelectedIndexChanged;
            altKeyDropDown.SelectedIndexChanged += AltKeyDropDown_SelectedIndexChanged;
            modifierDropDown.SelectedIndexChanged += ModifierDropDown_SelectedIndexChanged;
        }

        private void ModifierDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            Keys modifierKey = (Keys)Enum.Parse(typeof(Keys), keyNames[modifierDropDown.SelectedIndex]);

            if(ModifierKeyChanged != null)
            {
                KeyChangedEvent disptach = ModifierKeyChanged;

                disptach(actionName, modifierKey);
            }
        }

        private void AltKeyDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            Keys altKey = (Keys)Enum.Parse(typeof(Keys), keyNames[altKeyDropDown.SelectedIndex]);

            if (AltKeyChanged != null)
            {
                KeyChangedEvent disptach = AltKeyChanged;

                disptach(actionName, altKey);
            }
        }

        private void MainKeyDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            Keys mainkey = (Keys)Enum.Parse(typeof(Keys), keyNames[mainKeyDropDown.SelectedIndex]);

            if (MainKeyChanged != null)
            {
                KeyChangedEvent disptach = MainKeyChanged;

                disptach(actionName, mainkey);
            }
        }
    }
}
