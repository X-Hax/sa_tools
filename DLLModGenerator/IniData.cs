using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using SA_Tools;

namespace DLLModGenerator
{
    public class IniData
    {
        [IniName("game")]
        [DefaultValue(Game.SADX)]
        public Game Game { get; set; }
		[IniName("modulename")]
		public string ModuleName { get; set; }
        [IniCollection(IniCollectionMode.IndexOnly)]
        public Dictionary<string, FileInfo> Files { get; set; }
    }

    public enum Game
    {
        SADX,
        SA2B
    }

    public class FileInfo
    {
        [IniName("type")]
        public string Type { get; set; }
		[IniName("length")]
		public int Length { get; set; }
        [IniName("filename")]
        public string Filename { get; set; }
    }

	public class DllIniData
	{
		[IniName("name")]
		public string Name { get; set; }
		[IniAlwaysInclude]
		[IniName("game")]
		public Game Game { get; set; }
		public DictionaryContainer<string> Exports { get; set; }
		public DictionaryContainer<FileTypeHash> Files { get; set; }
		[IniName("Item")]
		[IniCollection(IniCollectionMode.NoSquareBrackets, StartIndex = 1)]
		public List<DllItemInfo> Items { get; set; }

		public DllIniData()
		{
			Exports = new DictionaryContainer<string>();
			Files = new DictionaryContainer<FileTypeHash>();
			Items = new List<DllItemInfo>();
		}
	}

	public class DictionaryContainer<T> : IDictionary<string, T>
	{
		[IniCollection(IniCollectionMode.IndexOnly)]
		public Dictionary<string, T> Items { get; set; }

		public DictionaryContainer()
		{
			Items = new Dictionary<string, T>();
		}

		public void Add(string key, T value)
		{
			Items.Add(key, value);
		}

		public bool ContainsKey(string key)
		{
			return Items.ContainsKey(key);
		}

		public ICollection<string> Keys
		{
			get { return Items.Keys; }
		}

		public bool Remove(string key)
		{
			return Items.Remove(key);
		}

		public bool TryGetValue(string key, out T value)
		{
			return Items.TryGetValue(key, out value);
		}

		public ICollection<T> Values
		{
			get { return Items.Values; }
		}

		public T this[string key]
		{
			get
			{
				return Items[key];
			}
			set
			{
				Items[key] = value;
			}
		}

		public void Clear()
		{
			Items.Clear();
		}

		public int Count
		{
			get { return Items.Count; }
		}

		bool ICollection<KeyValuePair<string, T>>.IsReadOnly
		{
			get { return false; }
		}

		public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
		{
			return Items.GetEnumerator();
		}

		void ICollection<KeyValuePair<string, T>>.Add(KeyValuePair<string, T> item)
		{
			throw new NotImplementedException();
		}

		void ICollection<KeyValuePair<string, T>>.Clear()
		{
			throw new NotImplementedException();
		}

		bool ICollection<KeyValuePair<string, T>>.Contains(KeyValuePair<string, T> item)
		{
			throw new NotImplementedException();
		}

		void ICollection<KeyValuePair<string, T>>.CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		bool ICollection<KeyValuePair<string, T>>.Remove(KeyValuePair<string, T> item)
		{
			throw new NotImplementedException();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	[System.ComponentModel.TypeConverter(typeof(StringConverter<FileTypeHash>))]
	public class FileTypeHash
	{
		public string Type { get; set; }
		public string Hash { get; set; }

		public FileTypeHash(string type, string hash)
		{
			Type = type;
			Hash = hash;
		}

		public FileTypeHash(string data)
		{
			string[] split = data.Split('|');
			Type = split[0];
			Hash = split[1];
		}

		public override string ToString()
		{
			return Type + "|" + Hash;
		}
	}

	public class DllItemInfo
	{
		public string Export { get; set; }
		public int? Index { get; set; }
		public string Field { get; set; }
		public string Label { get; set; }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder(Export);
			if (Index.HasValue)
				sb.Append(Index.Value);
			if (!string.IsNullOrEmpty(Field))
				sb.AppendFormat("->{0}", Field);
			return sb.ToString();
		}
	}
}