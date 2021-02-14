using System.Windows.Forms;

using SonicRetro.SAModel.SAEditorCommon.UI;

namespace SonicRetro.SAModel.SAMDL
{
	public static class DefaultActionList
	{
		private static ActionKeyMapping[] defaultActionMapping = new ActionKeyMapping[]
		{
			#region Camera Hotkeys
			new ActionKeyMapping()
			{
				Name = "Camera Mode",
				MainKey = Keys.X,
				AltKey = Keys.None,
				Description = "Switch between normal cam mode and orbit cam mode",
				FireType = ActionFireType.OnPress,
				IsSearchable = true,
				Modifiers = Keys.None,
				Synonyms = new string[] { "orbit" }
			},
			new ActionKeyMapping()
			{
				Name = "Zoom to target",
				MainKey = Keys.Z,
				AltKey = Keys.F,
				Description = "Zoom to the selected object",
				FireType = ActionFireType.OnPress,
				IsSearchable = true,
				Modifiers = Keys.None,
				Synonyms = new string[] { "zoom", "focus" }
			},
			new ActionKeyMapping()
			{
				Name = "Increase camera move speed",
				MainKey = Keys.Add,
				AltKey = Keys.None,
				Description = "",
				IsSearchable = true,
				FireType = ActionFireType.OnPress,
				Modifiers = Keys.None,
				Synonyms = new string[] { }
			},
			new ActionKeyMapping()
			{
				Name = "Decrease camera move speed",
				MainKey = Keys.Subtract,
				AltKey = Keys.None,
				Description = "",
				IsSearchable = true,
				FireType = ActionFireType.OnPress,
				Modifiers = Keys.None,
				Synonyms = new string[] { }
			},
			new ActionKeyMapping()
			{
				Name = "Reset camera move speed",
				MainKey = Keys.NumPad5,
				AltKey = Keys.None,
				Description = "",
				IsSearchable = true,
				FireType = ActionFireType.OnPress,
				Modifiers = Keys.None,
				Synonyms = new string[] { }
			},
			new ActionKeyMapping()
			{
				Name = "Reset Camera Position",
				MainKey = Keys.E,
				AltKey = Keys.None,
				Description = "",
				IsSearchable = true,
				FireType = ActionFireType.OnPress,
				Modifiers = Keys.None,
				Synonyms = new string[] { }
			},
			new ActionKeyMapping()
			{
				Name = "Reset Camera Rotation",
				MainKey = Keys.R,
				AltKey = Keys.None,
				Description = "",
				IsSearchable = true,
				FireType = ActionFireType.OnPress,
				Modifiers = Keys.None,
				Synonyms = new string[] { }
			},
			new ActionKeyMapping()
			{
				Name = "Camera Move",
				MainKey = Keys.ShiftKey,
				AltKey = Keys.None,
				FireType = ActionFireType.OnHold,
				Description = "Press to move the camera. Use rotate & zoom buttons to change behavior.",
				IsSearchable = true,
				Modifiers = Keys.MButton,
				Synonyms = new string[] { "Pan", "Scroll", "Dolly" }
			},
			new ActionKeyMapping()
			{
				Name = "Camera Zoom",
				MainKey = Keys.ControlKey,
				AltKey = Keys.None,
				Modifiers = Keys.MButton,
				Description = "Combine with Camera Move to zoom the camera.",
				FireType = ActionFireType.OnHold,
				IsSearchable = true,
				Synonyms = new string[] { }
			},
			new ActionKeyMapping()
			{
				Name = "Camera Look",
				MainKey = Keys.MButton,
				AltKey = Keys.None,
				Description = "Combine with Camera Move to mouselook the camera.",
				FireType = ActionFireType.OnHold,
				IsSearchable = true,
				Modifiers = Keys.None,
				Synonyms = new string[] { "mouselook", "rotate" }
			},
			#endregion
			new ActionKeyMapping()
			{
				Name = "Change Render Mode",
				MainKey = Keys.F3,
				AltKey = Keys.N,
				Description = "Toggle between filled polys, wireframe, and point cloud",
				FireType = ActionFireType.OnPress,
				IsSearchable = true,
				Modifiers = Keys.None,
				Synonyms = new string[] { "wireframe", "filled polygon" }
			},
			new ActionKeyMapping()
			{
				Name = "Delete",
				MainKey = Keys.Delete,
				AltKey = Keys.None,
				Description = "Delete the selected items",
				FireType = ActionFireType.OnPress,
				IsSearchable = true,
				Modifiers = Keys.None,
				Synonyms = new string[] { }
			},

			#region Animation hotkeys
			new ActionKeyMapping()
			{
				Name = "Next Animation",
				MainKey = Keys.OemQuotes,
				AltKey = Keys.None,
				Description = "Switch to the next animation.",
				FireType = ActionFireType.OnPress,
				IsSearchable = true,
				Modifiers = Keys.None,
				Synonyms = new string[] { }
			},
			new ActionKeyMapping()
			{
				Name = "Previous Animation",
				MainKey = Keys.OemSemicolon,
				AltKey = Keys.None,
				Description = "Switch to the previous animation.",
				FireType = ActionFireType.OnPress,
				IsSearchable = true,
				Modifiers = Keys.None,
				Synonyms = new string[] { }
			},
			new ActionKeyMapping()
			{
				Name = "Next Frame",
				MainKey = Keys.OemCloseBrackets,
				AltKey = Keys.None,
				Description = "Next Frame in animation",
				FireType = ActionFireType.OnPress,
				IsSearchable = true,
				Modifiers = Keys.None,
				Synonyms = new string[] { }
			},
			new ActionKeyMapping()
			{
				Name = "Previous Frame",
				MainKey = Keys.OemOpenBrackets,
				AltKey = Keys.None,
				Description = "Previous Frame in animation",
				FireType = ActionFireType.OnPress,
				IsSearchable = true,
				Modifiers = Keys.None,
				Synonyms = new string[] { }
			},
			new ActionKeyMapping()
			{
				Name = "Play/Pause Animation",
				MainKey = Keys.P,
				AltKey = Keys.None,
				Description = "Toggles the play state of the animation",
				FireType = ActionFireType.OnPress,
				IsSearchable = true,
				Modifiers = Keys.None,
				Synonyms = new string[] { }
			},
			#endregion

			#region Lighting hotkeys
			new ActionKeyMapping()
			{
				Name = "Brighten Ambient",
				MainKey = Keys.Home,
				AltKey = Keys.None,
				Description = "Make ambient lighting brighter (back light)",
				FireType = ActionFireType.OnPress,
				IsSearchable = true,
				Modifiers = Keys.None,
				Synonyms = new string[] { }
			},
			new ActionKeyMapping()
			{
				Name = "Darken Ambient",
				MainKey = Keys.End,
				AltKey = Keys.None,
				Description = "Make ambient lighting darker (back light)",
				FireType = ActionFireType.OnPress,
				IsSearchable = true,
				Modifiers = Keys.None,
				Synonyms = new string[] { }
			},
			#endregion

		};

		public static ActionKeyMapping[] DefaultActionMapping { get { return defaultActionMapping; } }
	}
}
