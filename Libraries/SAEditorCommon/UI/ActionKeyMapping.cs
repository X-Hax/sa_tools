using System.Windows.Forms;

namespace SAModel.SAEditorCommon.UI
{
	public enum ActionFireType
	{
		OnPress,
		OnHold
	}

	public struct ActionKeyMapping
	{
		public string Name;
		public Keys MainKey;
		public Keys AltKey;
		public Keys Modifiers;
		public ActionFireType FireType;
		public bool IsSearchable;

		public string Description;
		public string[] Synonyms;
	}
}
