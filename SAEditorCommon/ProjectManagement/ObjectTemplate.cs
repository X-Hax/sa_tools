using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Serialization;
using SA_Tools;

namespace SAEditorCommon.StageManagement
{
	public enum RotType
	{
		XYZ,
		Y,
		ZYX,
		NoRotation
	}

	public enum SclType
	{
		XYZ,
		X,
		Y,
		Z,
		XY,
		YZ,
		XZ,
		NoScale
	}

	public class classAltVariables
	{
		public int AltRotX { get; set; }
		public int AltRotY { get; set; }
		public int AltRotZ { get; set; }
		public float AltScaleX { get; set; }
		public float AltScaleY { get; set; }
		public float AltScaleZ { get; set; }
	}

	public class classBasicDisplay
	{
		[XmlAttribute("TextureFile", typeof(string)]
		public string TextureFile { get; set; }
		[XmlAttribute("Rotation", typeof(RotType))]
		public RotType Rotation { get; set; }
		[XmlAttribute("Scale", typeof(SclType))]
		public SclType Scale { get; set; }
		[XmlAttribute("AltVariables", typeof(classAltVariables))]
		public classAltVariables AltVariables { get; set; }
	}

	public class classCodeHandler
	{
		[XmlAttribute("CodeFile", typeof(string))]
		public string CodeFile { get; set; }
		[XmlAttribute("NamespaceClass", typeof(string))]
		public string NamepaceClass { get; set; }
	}

	[XmlRoot(Namespace = "http://www.sonicretro.org")]
	public class ObjectTemplate
	{
		[XmlAttribute("ItemName", typeof(string))]
		public string ItemName { get; set; }
		[XmlAttribute("CommonName", typeof(string))]
		public string CommonName { get; set; }
		[XmlAttribute("BasicDisplay", typeof(classBasicDisplay))]
		public classBasicDisplay BasicDisplay { get; set; }
		[XmlAttribute("CodeHandler", typeof(classCodeHandler))]
		public classCodeHandler CodeHandler { get; set; }
	}
}
