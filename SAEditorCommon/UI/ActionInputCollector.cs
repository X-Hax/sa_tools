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
            bool mainKeyMatch = (mainKey == key);
            bool altKeyExists = altKey != Keys.None;
            bool altKeyMatch = (altKey == key);
            bool modifiersExist = modifiers != Keys.None;
            bool modifierMatch = (modifiers == key);

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

        public void KeyDown(Keys keys)
        {
            // check for on-hold events
            if(keysDown.Contains(keys))
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
            }

            // modify keysdown
            keysDown.Add(keys);
        }

        public void KeyUp(Keys keys)
        {
            // check for on-release events
            foreach(ActionKeyMapping action in actions.Values)
            {
                if(action.MainKey == keys || action.AltKey == keys) // keys.none == 0 so this is causing problems?
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
