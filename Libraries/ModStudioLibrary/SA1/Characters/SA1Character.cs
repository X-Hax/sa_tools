using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModStudioLibrary.SA1.Characters
{
	public class PlayerBools
	{
		public bool DisableMorphs { get; set; }
		public bool DisableEventFace { get; set; }
		public bool DisableSpinball { get; set; }
		public bool DisableJVList { get; set; }
	}

	public class SA1Character : CharacterBase
    {
		public string? Parameters { get; set; }
		public string? JoinVertexList { get; set; }
		public string? ActionList { get; set; }
		public string? HeadEyeList { get; set; }
	}
}
