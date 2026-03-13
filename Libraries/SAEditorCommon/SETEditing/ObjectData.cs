using System.Collections.Generic;
using System.ComponentModel;
using SplitTools;

namespace SAModel.SAEditorCommon.SETEditing
{
	/// <summary>
	/// Class for object definition data entries stored in INI files in the objdefs folder.
	/// </summary>
	public class ObjectData
	{
		/// <summary>
		/// Object description. Example: "Beach chair".
		/// </summary>
		public string Name;

		/// <summary>
		/// If the object uses custom code, this is the path to the .cs file. Only CodeFile and CodeType are required for custom definitons.
		/// Example: objdefs/Common/Spring.cs
		/// </summary>
		[IniName("codefile")] // Why lowercase for this and CodeType? Everything else isn't
		public string CodeFile;

		/// <summary>
		/// If the object uses custom code, this is the name of the object class in the .cs file.
		/// Example: SADXObjectDefinitions.Common.Spring
		/// </summary>
		[IniName("codetype")]
		public string CodeType;

		/// <summary>
		/// If the object doesn't use custom code, this specifies the path to the object's model.
		/// Escape backslashes or use slashes instead. Example: object/rocket_launcher_body.nja.sa1mdl
		/// </summary>
		public string Model;

		/// <summary>
		/// Name of the texture file used by the object. 
		/// Example: SS_PEOPLE
		/// </summary>
		public string Texture;

		/// <summary>
		/// For objects in SA2 that use multiple texlists, specify a comma-separated list of all PVM/PAK filenames without extension used by the object.
		/// Example: OBJTEX_STG13 (haven't found one in current data where it uses multiple)
		/// </summary>
		[IniCollection(IniCollectionMode.SingleLine, Format = ",")]
		public List<string> TexturePacks = [];
		
		/// <summary>
		/// Location of the .satex file used by the object with partial texlists in SA2.
		/// Example: OBJECT/tls/RING.satex
		/// </summary>
		public string Texlist;

		/// <summary>
		/// Position offset. Can be added in objects that are translared in addition to what's specified in the SET file.
		/// </summary>
		public float? AddXPos, AddYPos, AddZPos;

		/// <summary>
		/// Type of rotation order.
		/// </summary>
		public RotationOrder RotType;

		/// <summary>
		/// Initial rotation. Used for objects that have a "starting" rotation different from the one specified in the SET file.
		/// </summary>
		[TypeConverter(typeof(UInt16HexConverter))]
		public ushort? DefXRot, DefYRot, DefZRot;

		/// <summary>
		/// Rotation offset. Can be added in objects that rotate in addition to what's specified in the SET file.
		/// </summary>
		[TypeConverter(typeof(UInt16HexConverter))]
		public ushort? AddXRot, AddYRot, AddZRot;

		/// <summary>
		/// Type of scale order.
		/// </summary>
		public ScaleOrder SclType;

		/// <summary>
		/// Default X, Y and Z position. Used for objects that need an override of their NJS_OBJECT's position.
		/// </summary>
		public float? DefXPos, DefYPos, DefZPos;

		/// <summary>
		/// Default X, Y and Z scale. Used for objects that have a "starting" scale different from the one specified in the SET file.
		/// </summary>
		public float? DefXScl, DefYScl, DefZScl;

		/// <summary>
		/// Scale offset. Can be added in objects that scale up in addition to what's specified in the SET file.
		/// </summary>
		public float? AddXScl, AddYScl, AddZScl;

		/// <summary>
		/// Distance from the Ground. Some objects (e.g. rings) need this for proper ground placement in SALVL.
		/// </summary>
		public float? GndDst;

		/// <summary>
		/// Set to true if the model's root NJS_OBJECT position should be ignored for rendering and collision.
		/// </summary>
		public bool IgnorePos;
	}

	/// <summary>
	/// Type of rotation by axis. Depends on the object.
	/// </summary>
	public enum RotationOrder
	{
		None,
		XYZ,
		X,
		Y,
		Z,
		XY,
		XZ,
		YX,
		YZ,
		ZX,
		ZY,
		XZY,
		YXZ,
		YZX,
		ZXY,
		ZYX
	}

	/// <summary>
	/// Type of scaling by axis. Depends on the object.
	/// </summary>
	public enum ScaleOrder
	{
		None,
		XYZ,
		X,
		Y,
		Z,
		XY,
		XZ,
		YZ,
		AllX,
		AllY,
		AllZ
	}
}