using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAModel.SALVL
{
	public partial class MainForm
	{
        private void ShowLevelSelect()
        {
			IsLevelOnly = false;
			LevelPath = string.Empty;
            string stageToLoad = string.Empty;
            using (LevelSelectDialog dialog = new LevelSelectDialog(levelNames))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    stageToLoad = dialog.SelectedStage;
                    skipDefs = dialog.skipobjdefs;
                }
            }

            if (!string.IsNullOrEmpty(stageToLoad))
            {
                if (isStageLoaded)
                {
                    if (SavePrompt(true) == DialogResult.Cancel)
                        return;
                }

                UncheckMenuItems(changeLevelToolStripMenuItem);
                CheckMenuItemByTag(changeLevelToolStripMenuItem, stageToLoad);
                LoadStage(stageToLoad);
            }
        }

        private void PopulateLevelMenu(ToolStripMenuItem targetMenu, Dictionary<string, List<string>> levels)
        {
            // Used for keeping track of menu items
            Dictionary<string, ToolStripMenuItem> levelMenuItems = new Dictionary<string, ToolStripMenuItem>();
            targetMenu.DropDownItems.Clear();

            foreach (KeyValuePair<string, List<string>> item in levels)
            {
                // For every section (e.g Adventure Fields) in levels, reset the parent menu.
                // It gets changed later if necessary.
                ToolStripMenuItem parent = targetMenu;
                foreach (string stage in item.Value)
                {
                    // If a menu item for this section has not already been initialized...
                    if (!levelMenuItems.ContainsKey(item.Key))
                    {
                        // Create it
                        ToolStripMenuItem i = new ToolStripMenuItem(item.Key.Replace("&", "&&"));

                        // Add it to the list to keep track of it
                        levelMenuItems.Add(item.Key, i);
                        // Add the new menu item to the parent menu
                        parent.DropDownItems.Add(i);
                        // and set the parent so we know where to put the stage
                        parent = i;
                    }
                    else
                    {
                        // Otherwise, set the parent to the existing reference
                        parent = levelMenuItems[item.Key];
                    }

                    // And finally, create the menu item for the stage name itself and hook it up to the Clicked event.
                    // The Tag member here is vital. The code later on uses this to determine what assets to load.
                    parent.DropDownItems.Add(new ToolStripMenuItem(stage, null, LevelToolStripMenuItem_Clicked)
                    {
                        Tag = item.Key + '\\' + stage
                    });
                }
            }
        }

        /// <summary>
        /// Iterates recursively through menu items and unchecks all sub-items.
        /// </summary>
        /// <param name="menu">The parent menu of the items to be unchecked.</param>
        private static void UncheckMenuItems(ToolStripDropDownItem menu)
        {
            foreach (ToolStripMenuItem i in menu.DropDownItems)
            {
                if (i.HasDropDownItems)
                    UncheckMenuItems(i);
                else
                    i.Checked = false;
            }
        }

        /// <summary>
        /// Unchecks all children of the parent object and checks the target.
        /// </summary>
        /// <param name="target">The item to check</param>
        /// <param name="parent">The parent menu containing the target.</param>
        private static void CheckMenuItem(ToolStripMenuItem target, ToolStripItem parent = null)
        {
            if (target == null)
                return;

            if (parent == null)
                parent = target.OwnerItem;

            UncheckMenuItems((ToolStripDropDownItem)parent);
            target.Checked = true;
        }

        /// <summary>
        /// Iterates recursively through the parent and checks the first item it finds with a matching Tag.
        /// If firstOf is true, recursion stops after the first match.
        /// </summary>
        /// <param name="parent">The parent menu.</param>
        /// <param name="tag">The tag to search for.</param>
        /// <param name="firstOf">If true, recursion stops after the first match.</param>
        /// <returns></returns>
        private static bool CheckMenuItemByTag(ToolStripDropDownItem parent, string tag, bool firstOf = true)
        {
            foreach (ToolStripMenuItem i in parent.DropDownItems)
            {
                if (i.HasDropDownItems)
                {
                    if (CheckMenuItemByTag(i, tag, firstOf))
                        return true;
                }
                else if ((string)i.Tag == tag)
                {
                    if (firstOf)
                    {
                        CheckMenuItem(i, parent);
                        return true;
                    }
                    else
                    {
                        i.Checked = true;
                    }
                }
            }

            return false;
        }

    }
}
