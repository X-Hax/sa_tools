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
		[XmlElement("TextureFile", typeof(string), IsNullable=true)]
		public string TextureFile { get; set; }
		[XmlElement("Rotation", typeof(RotType), IsNullable=true)]
		public RotType Rotation { get; set; }
		[XmlElement("Scale", typeof(SclType), IsNullable=true)]
		public SclType Scale { get; set; }
		[XmlElement("AltVariables", typeof(classAltVariables), IsNullable=true)]
		public classAltVariables AltVariables { get; set; }
	}

	public class classCodeHandler
	{
		[XmlElement("CodeFile", typeof(string), IsNullable = true)]
		public string CodeFile { get; set; }
		[XmlElement("NamespaceClass", typeof(string), IsNullable=true)]
		public string NamepaceClass { get; set; }
	}

	[XmlRoot(Namespace = "http://www.sonicretro.org")]
	public class ObjectTemplate
	{
		[XmlElement("ItemName", typeof(string))]
		public string ItemName { get; set; }
		[XmlElement("CommonName", typeof(string))]
		public string CommonName { get; set; }
		[XmlElement("BasicDisplay", typeof(classBasicDisplay), IsNullable=true)]
		public classBasicDisplay BasicDisplay { get; set; }
		[XmlElement("CodeHandler", typeof(classCodeHandler), IsNullable=true)]
		public classCodeHandler CodeHandler { get; set; }
	}
}
