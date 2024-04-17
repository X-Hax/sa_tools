using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModStudioLibrary
{
	public class CharacterBase
	{
		public Dictionary<string, string>? Objects { get; set; }
		public Dictionary<string, string>? Actions { get; set; }
		public Dictionary<string, string>? Textures { get; set; }
	}
}
