using System.Collections.Generic;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
	public class ActionInputCollector
	{
		public delegate void ActionHandler(ActionInputCollector sender, string actionName);
		public event ActionHandler OnActionStart;
		/// <summary>
		/// Only used for 'hold' type actions - 
		/// this gets fired when the action has been 'started' and is now 'released'.
		/// </summary>
		public event ActionHandler OnActionRelease;

		Dictionary<string, ActionKeyMapping> actions = new Dictionary<string, ActionKeyMapping>();
		Dictionary<string, bool> actionsActiveState = new Dictionary<string, bool>();
		List<string> holdActions = new List<string>();
		//List<string> pressActions = new List<string>();

		List<Keys> keysDown = new List<Keys>();

		public void ReleaseKeys()
		{
			keysDown = new List<Keys>();
			foreach (string holdAction in holdActions)
			{
				ActionKeyMapping action = actions[holdAction];
				actionsActiveState[action.Name] = false;
				if (OnActionRelease != null)
				{
					ActionHandler dispatch = OnActionRelease;
					dispatch(this, action.Name);
				}
			}
		}

		public void SetActions(ActionKeyMapping[] actionsToAdd)
		{
			actions.Clear();
			actionsActiveState.Clear();
			holdActions.Clear();

			foreach (ActionKeyMapping action in actionsToAdd)
			{
				actions.Add(action.Name, action);
				actionsActiveState.Add(action.Name, false);

				if (action.FireType == ActionFireType.OnHold) holdActions.Add(action.Name);
				//else if (action.FireType == ActionFireType.OnPress) pressActions.Add(action.Name);
			}
		}

		// Checks the held keys list for a key/key combination regardless of the actual key that sent the event. 
		// This is needed to detect keypresses in any order for modifiers in "hold" actions.
		public bool KeyMatch_Combination(Keys mainKey, Keys altKey, Keys modifiers)
		{
			bool altKeyExists = altKey != Keys.None;
			bool modifiersExist = modifiers != Keys.None;
			bool mainKeyMatch = false;
			bool altKeyMatch = false;
			bool modifierMatch = false;
			foreach (Keys heldkey in keysDown)
			{
				if (KeysMatch(mainKey, heldkey)) mainKeyMatch = true;
				else if (altKeyExists && KeysMatch(altKey, heldkey)) altKeyMatch = true;
				else if (modifiersExist && KeysMatch(modifiers, heldkey)) modifierMatch = true;
			}
			if (modifiersExist)
			{
				if (mainKeyMatch && modifierMatch) return true;
				if (altKeyMatch && modifierMatch) return true;
			}
			else
			{
				if (mainKeyMatch || altKeyMatch) return true;
			}
			return false;
		}

		public bool KeyMatch(Keys key, Keys mainKey, Keys altKey, Keys modifiers, bool press = true)
		{
			bool mainKeyMatch = KeysMatch(mainKey, key);
			bool altKeyExists = altKey != Keys.None;
			bool altKeyMatch = KeysMatch(altKey, key);
			bool modifiersExist = modifiers != Keys.None;
			bool modifierMatch = keysDown.Contains(modifiers);
			// This is used to detect KeyUp for "press" actions
			if (press)
			{
				if (mainKeyMatch || (altKeyExists && altKeyMatch))
				{
					if (modifiersExist)
					{
						return modifierMatch;
					}
					else return true;
				}
				return false;
			}
			// This is used to detect KeyUp for "hold" actions
			else
			{
				bool modifiersMatch = KeysMatch(modifiers, key);
				if (mainKeyMatch) return true;
				else if (altKeyExists && altKeyMatch) return true;
				else if (modifiersExist && modifiersMatch) return true;
				else return false;
			}
		}

		private bool KeysMatch(Keys keyA, Keys keyB)
		{
			switch (keyA)
			{
				case (Keys.Menu):
				case (Keys.Alt):
				case (Keys.LMenu):
				case (Keys.RMenu):
					return (keyB == Keys.Alt || keyB == Keys.LMenu || keyB == Keys.Menu || keyB == Keys.RMenu);
				case (Keys.Control):
				case (Keys.ControlKey):
				case (Keys.RControlKey):
				case (Keys.LControlKey):
					return (keyB == Keys.Control || keyB == Keys.LControlKey || keyB == Keys.RControlKey || keyB == Keys.ControlKey);
				default:
					break;
			}
			return keyA == keyB;
		}

		public void KeyDown(Keys keys)
		{
			if (!keysDown.Contains(keys))
				keysDown.Add(keys);
			// find any hold actions that are valid under the new key arrangement
			foreach (string holdAction in holdActions)
			{
				ActionKeyMapping action = actions[holdAction];
				if (keys == action.MainKey || keys == action.AltKey || keys == action.Modifiers)
				{
					if (KeyMatch_Combination(action.MainKey, action.AltKey, action.Modifiers))
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
		}

		public void KeyUp(Keys keys)
		{
			keysDown.Remove(keys);
			// check for on-release events
			foreach (ActionKeyMapping action in actions.Values)
			{
				switch (action.FireType)
				{
					case ActionFireType.OnHold:
						if (KeyMatch(keys, action.MainKey, action.AltKey, action.Modifiers, false))
						{
							actionsActiveState[action.Name] = false;
							if (OnActionRelease != null)
							{
								ActionHandler dispatch = OnActionRelease;
								dispatch(this, action.Name);
							}
						}
						break;
					case ActionFireType.OnPress:
						if (KeyMatch(keys, action.MainKey, action.AltKey, action.Modifiers, true))
						{
							actionsActiveState[action.Name] = false;
							if (OnActionRelease != null)
							{
								ActionHandler dispatch = OnActionRelease;
								dispatch(this, action.Name);
							}
						}
						break;
					default:
						break;
				}
			}
		}
	}
}
