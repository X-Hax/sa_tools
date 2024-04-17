using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModStudioLibrary.SA1.Characters
{
	public class SonicToggles : PlayerBools
	{
		public bool DisableStretchyShoes { get; set; }
		public bool DisableLightShoes { get; set; }
		public bool DisableCrystalRing { get; set; }
		public bool DisableLightDashAura { get; set; }

		public SonicToggles()
		{
			DisableMorphs = false;
			DisableEventFace = false;
			DisableJVList = false;
			DisableSpinball = false;
			DisableStretchyShoes = false;
			DisableLightShoes = false;
			DisableCrystalRing = false;
			DisableLightDashAura = false;
		}

		public SonicToggles(bool disMorphs, bool disEventFace, bool disJVList, bool disSpinball,
			bool disStretchyShoes, bool disLightShoes, bool disCrystalRing, bool disLightDashAura)
		{
			DisableMorphs = disMorphs;
			DisableEventFace = disEventFace;
			DisableJVList = disJVList;
			DisableSpinball = disSpinball;
			DisableStretchyShoes = disStretchyShoes;
			DisableLightShoes = disLightShoes;
			DisableCrystalRing = disCrystalRing;
			DisableLightDashAura = disLightDashAura;
		}
	}

	public class SonicVariables
	{
		public ARGB LSAuraColor { get; set; }

		public SonicVariables()
		{
			LSAuraColor = new ARGB();
		}
	}

    public class Sonic : SA1Character
    {
		public SonicToggles Toggles { get; set; }
		public SonicVariables Variables { get; set; }

		public Sonic()
		{
			Objects = new Dictionary<string, string>();
			Actions = new Dictionary<string, string>();
			Textures = new Dictionary<string, string>();
			Parameters = string.Empty;
			JoinVertexList = string.Empty;
			ActionList = string.Empty;
			HeadEyeList = string.Empty;

			Toggles = new SonicToggles();
			Variables = new SonicVariables();
		}
    }
}
