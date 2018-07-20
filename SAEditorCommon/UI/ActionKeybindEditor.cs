using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
    public partial class ActionKeybindEditor : Form
    {
        ActionKeyMapping[] actionKeyMappings;
        ActionKeybindControl[] actionKeyControls;
        
        public ActionKeybindEditor(ActionKeyMapping[] actionKeyMappings)
        {
            InitializeComponent();

            this.actionKeyMappings = actionKeyMappings;
            actionKeyControls = new ActionKeybindControl[actionKeyMappings.Length];

            // generate our functions and controls
            int index = 0;
            foreach (ActionKeyMapping keyMapping in actionKeyMappings)
            {
                ActionKeybindControl keybindControl = new ActionKeybindControl(keyMapping.Name,
                    keyMapping.MainKey, keyMapping.AltKey, keyMapping.Modifiers, keyMapping.Description);

                keybindControl.Parent = flowLayoutPanel1;

                keybindControl.MainKeyChanged += KeybindControl_MainKeyChanged;
                keybindControl.AltKeyChanged += KeybindControl_AltKeyChanged;
                keybindControl.ModifierKeyChanged += KeybindControl_ModifierKeyChanged;

                actionKeyControls[index] = keybindControl;
                index++;
            }

            this.FormClosing += ActionKeybindEditor_FormClosing;
        }

        private void ActionKeybindEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        public ActionKeyMapping[] GetActionkeyMappings()
        {
            return actionKeyMappings;
        }

        private void KeybindControl_ModifierKeyChanged(string actionName, Keys key)
        {
            // find index
            int index = Array.FindIndex<ActionKeyMapping>(actionKeyMappings, item => item.Name == actionName);
            actionKeyMappings[index].Modifiers = key;
        }

        private void KeybindControl_AltKeyChanged(string actionName, Keys key)
        {
            // find index
            int index = Array.FindIndex<ActionKeyMapping>(actionKeyMappings, item => item.Name == actionName);
            actionKeyMappings[index].AltKey = key;
        }

        private void KeybindControl_MainKeyChanged(string actionName, Keys key)
        {
            // find index
            int index = Array.FindIndex<ActionKeyMapping>(actionKeyMappings, item => item.Name == actionName);
            actionKeyMappings[index].MainKey = key;
        }
    }
}
