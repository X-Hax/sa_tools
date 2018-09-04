using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
    public class ActionInputCollector
    {
        public delegate void ActionHandler(ActionInputCollector sender, string actionName);
        public event ActionHandler OnActionStart;
        /// <summary>
        /// Only used for 'hold' tye actions - 
        /// this gets fired when the action has been 'started' and is how 'released'.
        /// </summary>
        public event ActionHandler OnActionRelease;

        Dictionary<string, ActionKeyMapping> actions = new Dictionary<string, ActionKeyMapping>();
        Dictionary<string, bool> actionsActiveState = new Dictionary<string, bool>();
        List<string> holdActions = new List<string>();
        //List<string> pressActions = new List<string>();

        List<Keys> keysDown = new List<Keys>();

        public void SetActions(ActionKeyMapping[] actionsToAdd)
        {
            actions.Clear();
            actionsActiveState.Clear();
            holdActions.Clear();

            foreach(ActionKeyMapping action in actionsToAdd)
            {
                actions.Add(action.Name, action);
                actionsActiveState.Add(action.Name, false);

                if (action.FireType == ActionFireType.OnHold) holdActions.Add(action.Name);
                //else if (action.FireType == ActionFireType.OnPress) pressActions.Add(action.Name);
            }
        }

        public bool KeyMatch(Keys key, Keys mainKey, Keys altKey, Keys modifiers)
        {
            bool mainKeyMatch = KeysMatch(mainKey, key);
            bool altKeyExists = altKey != Keys.None;
            bool altKeyMatch = KeysMatch(altKey, key);
            bool modifiersExist = modifiers != Keys.None;
            bool modifierMatch = KeysMatch(modifiers, key);

            if (mainKeyMatch)
            {
                if (modifiersExist) return modifierMatch;
                else return true;
            }
            else
            {
                if (altKeyExists)
                {
                    if (modifiersExist) return modifierMatch;
                    else return true;
                }
                else return false;
            }
        }

        private bool KeysMatch(Keys keyA, Keys keyB)
        {
            switch (keyA)
            {
                case (Keys.Menu):
                    return (keyB == Keys.Alt || keyB == Keys.LMenu || keyB == Keys.RMenu || keyB == Keys.Menu);
                case (Keys.Alt):
                    return (keyB == Keys.Menu || keyB == Keys.LMenu || keyB == Keys.RMenu || keyB == Keys.Alt);
                case (Keys.LMenu):
                    return (keyB == Keys.Alt || keyB == Keys.Menu || keyB == Keys.RMenu || keyB == Keys.LMenu);
                case (Keys.RMenu):
                    return (keyB == Keys.Alt || keyB == Keys.LMenu || keyB == Keys.Menu || keyB == Keys.RMenu);

                case (Keys.Control):
                    return (keyB == Keys.LControlKey || keyB == Keys.RControlKey || keyB == Keys.ControlKey || keyB == Keys.Control);
                case (Keys.RControlKey):
                    return (keyB == Keys.Control || keyB == Keys.LControlKey || keyB == Keys.ControlKey || keyB == Keys.RControlKey);
                case (Keys.LControlKey):
                    return (keyB == Keys.Control || keyB == Keys.LControlKey || keyB == Keys.RControlKey || keyB == Keys.LControlKey);
                default:
                    break;
            }
            return keyA == keyB;
        }

        public void KeyDown(Keys keys)
        {
            if(!keysDown.Contains(keys))
            {
                // find any hold actions that are valid under the new key arrangement
                foreach(string holdAction in holdActions)
                {
                    ActionKeyMapping action = actions[holdAction];
                    if (KeyMatch(keys, action.MainKey, action.AltKey, action.Modifiers))
                    {
                        actionsActiveState[holdAction] = true;

                        if (OnActionStart != null)
                        {
                            ActionHandler dispatch = OnActionStart;
                            dispatch(this, holdAction);
                        }                      
                    }
                }

                // modify keysdown
                keysDown.Add(keys);
            }
        }

        public void KeyUp(Keys keys)
        {
            // check for on-release events
            foreach(ActionKeyMapping action in actions.Values)
            {
                if(KeysMatch(action.MainKey, keys) || 
                    (KeysMatch(action.AltKey, keys) && action.AltKey != Keys.None)) // keys.none == 0 so this is causing problems?
                {
                    actionsActiveState[action.Name] = false;

                    if(OnActionRelease != null)
                    {
                        ActionHandler dispatch = OnActionRelease;

                        dispatch(this, action.Name);
                    }
                }
            }

            // modify keysdown
            keysDown.Remove(keys);
        }
    }
}
