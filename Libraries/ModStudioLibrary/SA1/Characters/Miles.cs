using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModStudioLibrary.SA1.Characters
{
	public class MilesToggles : PlayerBools
	{
		public bool DisableTailsAnimation { get; set; }
		public bool DisableJetAnklet { get; set; }
		public bool DisableRhythmBadge { get; set; }

		public MilesToggles()
		{
			DisableMorphs = false;
			DisableEventFace = false;
			DisableJVList = false;
			DisableSpinball = false;
			DisableTailsAnimation = false;
			DisableJetAnklet = false;
			DisableRhythmBadge = false;
		}

		public MilesToggles(bool disMorphs, bool disEventFace, bool disJVList, bool disSpinball,
			bool disTailsAnimation, bool disJetAnklet, bool disRhythmBadge)
		{
			DisableMorphs = disMorphs;
			DisableEventFace = disEventFace;
			DisableJVList = disJVList;
			DisableSpinball = disSpinball;
			DisableTailsAnimation = disTailsAnimation;
			DisableJetAnklet = disJetAnklet;
			DisableRhythmBadge = disRhythmBadge;
		}
	}

	public class MilesVariables
	{

	}

    public class Miles : SA1Character
    {
		public MilesToggles Toggles { get; set; }
		public MilesVariables Variables { get; set; }

		public Miles()
		{
			Objects = new Dictionary<string, string>();
			Actions = new Dictionary<string, string>();
			Textures = new Dictionary<string, string>();
			Parameters = string.Empty;
			JoinVertexList = string.Empty;
			ActionList = string.Empty;
			HeadEyeList = string.Empty;

			Toggles = new MilesToggles();
			Variables = new MilesVariables();
		}
    }
}
