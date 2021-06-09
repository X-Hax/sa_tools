using System.IO;
using System.Collections.Generic;
using SA_Tools;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
	public class ActionMappingList
	{
		public List<ActionKeyMapping> ActionKeyMappings { get; set; }

		// Check if the list is from an older version that uses modifiers instead of actual keys (causes problems with current input code)
		private static bool MappingListOutdated(ActionMappingList testlist)
		{
			foreach (ActionKeyMapping action in testlist.ActionKeyMappings)
			{
				if (action.MainKey == Keys.Control || action.AltKey == Keys.Control || action.Modifiers == Keys.Control)
					return true;
			}
			return false;
		}

		private static ActionMappingList CreateMappingList(ActionKeyMapping[] defaultKeyMappings)
		{
			ActionMappingList mappingList = new ActionMappingList();
			mappingList.ActionKeyMappings = new List<ActionKeyMapping>();

			foreach (ActionKeyMapping defaultMapping in defaultKeyMappings)
			{
				mappingList.ActionKeyMappings.Add(defaultMapping);
			}

			return mappingList;
		}

		public static ActionMappingList Load(string path, ActionKeyMapping[] defaultKeyMappings)
		{
			ActionMappingList result = new ActionMappingList();
			if (File.Exists(path))
			{
				ActionMappingList testlist = IniSerializer.Deserialize<ActionMappingList>(path);
				if (MappingListOutdated(testlist))
					result = CreateMappingList(defaultKeyMappings);
				else result = testlist;

				// Check if new actions were added to the default list
				foreach (ActionKeyMapping map_def in defaultKeyMappings)
				{
					bool found = false;
					foreach (ActionKeyMapping map_act in result.ActionKeyMappings)
					{
						if (map_act.Name == map_def.Name)
							found = true;
					}
					if (!found) result.ActionKeyMappings.Add(map_def);
				}
				return result;
			}
			else
			{
				return CreateMappingList(defaultKeyMappings);
			}
		}

		public void Save(string path)
		{
			if (!Directory.Exists(Path.GetDirectoryName(path)))
				Directory.CreateDirectory(Path.GetDirectoryName(path));
			IniSerializer.Serialize(this, path);
		}
	}
}
