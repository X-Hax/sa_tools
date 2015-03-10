using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SonicRetro.SAModel.SAEditorCommon.DataTypes;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
	public class EditorItemSelection
	{
		public delegate void SelectionChangeHandler(EditorItemSelection sender);
		public event SelectionChangeHandler SelectionChanged;

		private List<Item> oldSelection;
		private List<Item> selection;

		public int ItemCount { get { return selection.Count(); } }

		public EditorItemSelection()
		{
			oldSelection = new List<Item>();
			selection = new List<Item>();
		}

		public List<Item> GetSelection()
		{
			return selection;
		}

		public void Add(Item item)
		{
			oldSelection = selection; // check to make sure this doesn't get screwed up by value/reference. It should be different than selection after selection gets updated
			selection.Add(item);

			SelectionChanged(this);
		}

		public void Add(List<Item> items)
		{
			oldSelection = selection; // check to make sure this doesn't get screwed up by value/reference. It should be different than selection after selection gets updated
			selection.AddRange(items);

			SelectionChanged(this);
		}

		public void Clear()
		{
			oldSelection = selection; // check to make sure this doesn't get screwed up by value/reference. It should be different than selection after selection gets updated
			selection.Clear();
			SelectionChanged(this);
		}

		public void Remove(Item item)
		{
			oldSelection = selection; // check to make sure this doesn't get screwed up by value/reference. It should be different than selection after selection gets updated
			selection.Remove(item);

			SelectionChanged(this);
		}
	}
}
