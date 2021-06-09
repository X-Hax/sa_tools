using System;
using System.Collections.Generic;
using System.Linq;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
	public class EditorItemSelection
	{
		public delegate void SelectionChangeHandler(EditorItemSelection sender);
		public event SelectionChangeHandler SelectionChanged = delegate { };

		private List<Item> selection;

		public int ItemCount { get { return selection.Count(); } }
		public IEnumerable<Item> Items { get { return (IEnumerable<Item>)selection; } }

		public EditorItemSelection()
		{
			selection = new List<Item>();
		}

		public List<Item> GetSelection()
		{
			return selection;
		}

		public void Add(Item item)
		{
			selection.Add(item);

			SelectionChanged(this);
		}

		public void Add(List<Item> items)
		{
			selection.AddRange(items);

			SelectionChanged(this);
		}

		public void Clear()
		{
			selection.Clear();
			SelectionChanged(this);
		}

		public void Remove(Item item)
		{
			selection.Remove(item);

			SelectionChanged(this);
		}

		public bool Contains(Item item) => selection.Contains(item);

		// May be a better idea to overload [] instead.		
		/// <summary>
		/// Gets the item at the specified index in the current selection.
		/// </summary>
		/// <param name="index">The in the selection.</param>
		/// <returns>The item at the specified index.</returns>
		/// <exception cref="System.IndexOutOfRangeException"></exception>
		public Item Get(int index)
		{
			if (index >= selection.Count())
				throw new IndexOutOfRangeException();

			return selection[index];
		}
	}
}
