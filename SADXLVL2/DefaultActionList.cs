using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;

using SonicRetro.SAModel.SAEditorCommon.UI;

namespace SonicRetro.SAModel.SADXLVL2
{
    public static class DefaultActionList
    {
        private static ActionKeyMapping[] defaultActionMapping = new ActionKeyMapping[]
        {
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
                MainKey = Keys.Enter,
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
                Name = "Next Character",
                MainKey = Keys.Tab,
                AltKey = Keys.None,
                Description = "Change the currently selected character",
                FireType = ActionFireType.OnPress,
                IsSearchable = true,
                Modifiers = Keys.Control,
                Synonyms = new string[] { }
            },
            new ActionKeyMapping()
            {
                Name = "Previous Character",
                MainKey = Keys.Tab,
                AltKey = Keys.None,
                Description = "Change the currently selected character",
                FireType = ActionFireType.OnPress,
                IsSearchable = true,
                Modifiers = Keys.Shift,
                Synonyms = new string[] { }
            },
            new ActionKeyMapping()
            {
                Name = "Hotkey Search",
                MainKey = Keys.Space,
                AltKey = Keys.None,
                Description = "Search for hotkeys",
                FireType = ActionFireType.OnPress,
                IsSearchable = true,
                Modifiers = Keys.Control,
                Synonyms = new string[] { "shortcut", "keybind" }
            }
        };

        public static ActionKeyMapping[] DefaultActionMapping { get { return defaultActionMapping; } }
    }
}
