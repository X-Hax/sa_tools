using System;

namespace SAModel
{
	public enum Basic_PolyType
	{
		Triangles,
		Quads,
		NPoly,
		Strips
	}

	[Flags()]
	public enum ObjectFlags : int
	{
		NoPosition = 0x01,
		NoRotate   = 0x02,
		NoScale    = 0x04,
		NoDisplay  = 0x08,
		NoChildren = 0x10,
		RotateZYX  = 0x20,
		NoAnimate  = 0x40,
		NoMorph    = 0x80
	}

	public enum FilterMode
	{
		PointSampled,
		Bilinear,
		Trilinear,
		Reserved
	}

	public enum AlphaInstruction
	{
		Zero,
		One,
		OtherColor,
		InverseOtherColor,
		SourceAlpha,
		InverseSourceAlpha,
		DestinationAlpha,
		InverseDestinationAlpha
	}

	[Flags]
	public enum AnimFlags : ushort
	{
		Position = 0x1,
		Rotation = 0x2,
		Scale = 0x4,
		Vector = 0x8,
		Vertex = 0x10,
		Normal = 0x20,
		Target = 0x40,
		Roll = 0x80,
		Angle = 0x100,
		Color = 0x200,
		Intensity = 0x400,
		Spot = 0x800,
		Point = 0x1000,
		Quaternion = 0x2000
	}

	[Flags]
	public enum SA1SurfaceFlags : int
	{
		Solid                 = 0x1,
		Water                 = 0x2,
		NoFriction            = 0x4,
		NoAcceleration        = 0x8,
		
		LowAcceleration       = 0x10,
		UseSkyDrawDistance    = 0x20,
		CannotLand            = 0x40,
		IncreasedAcceleration = 0x80,
		
		Diggable              = 0x100,
		//0x200
		Waterfall             = 0x400,
		//0x800
		
		Unclimbable           = 0x1000,
		Chaos0Land            = 0x2000, // Turns off Visible when Chaos 0 jumps up a pole
		Stairs                = 0x4000,
		//0x8000
		
		Hurt                  = 0x10000,
		//0x20000
		LowDepth              = 0x40000,
		//0x80000
		
		Footprints            = 0x100000,
		Accelerate            = 0x200000,
		WaterCollision        = 0x400000,
		RotateByGravity       = 0x800000,

		NoZWrite              = 0x1000000, // Sets QueuedModelFlagsB_SomeTextureThing when enabled, QueuedModelFlagsB_EnableZWrite otherwise
		DrawByMesh            = 0x2000000,
		EnableManipulation    = 0x4000000,
		DynamicCollision      = 0x8000000,

		UseRotation           = 0x10000000,
		//0x2000000
		//0x4000000
		Visible               = unchecked((int)0x80000000)
	}

    [Flags]
    public enum SA1LandtableAttributes : short
    {
        EnableMotion       = 0x1,
        LoadTexlist        = 0x2,
        CustomDrawDistance = 0x4,
        LoadTextureFile    = 0x8,
    }

    [Flags]
	public enum SA2SurfaceFlags : int
	{
		Solid			= 0x01,
		Water			= 0x02,
		Diggable		= 0x20,
		Unclimbable		= 0x80,
		Stairs			= 0x100,
		Hurt			= 0x0400,
		CannotLand		= 0x1000,
		Water2			= 0x2000,
		NoShadows		= 0x8000,
		NoFog			= 0x400000,
		Unknown24		= 0x01000000,
		Unknown29		= 0x20000000,
		Unknown30		= 0x40000000,
		Visible			= unchecked((int)0x80000000)
	}

	public enum LandTableFormat
	{
		SA1,
		SADX,
		SA2,
		SA2B
	}

	public enum ModelFormat
	{
		Basic,
		BasicDX,
		Chunk,
		GC,
		XJ
	}

	public enum ChunkType : byte
	{
		Null                                 = 0,
		Bits                                 = 1,
		Bits_BlendAlpha                      = Bits + 0,
		Bits_MipmapDAdjust                   = Bits + 1,
		Bits_SpecularExponent                = Bits + 2,
		Bits_CachePolygonList                = Bits + 3,
		Bits_DrawPolygonList                 = Bits + 4,
		Tiny                                 = 8,
		Tiny_TextureID                       = Tiny + 0,
		Tiny_TextureID2                      = Tiny + 1,
		Material                             = 16,
		Material_Diffuse                     = Material + 1,
		Material_Ambient                     = Material + 2,
		Material_DiffuseAmbient              = Material + 3,
		Material_Specular                    = Material + 4,
		Material_DiffuseSpecular             = Material + 5,
		Material_AmbientSpecular             = Material + 6,
		Material_DiffuseAmbientSpecular      = Material + 7,
		Material_Bump                        = Material + 8,
		Material_Diffuse2                    = Material + 9,
		Material_Ambient2                    = Material + 10,
		Material_DiffuseAmbient2             = Material + 11,
		Material_Specular2                   = Material + 12,
		Material_DiffuseSpecular2            = Material + 13,
		Material_AmbientSpecular2            = Material + 14,
		Material_DiffuseAmbientSpecular2     = Material + 15,
		Vertex                               = 32,
		Vertex_VertexSH                      = Vertex + 0,
		Vertex_VertexNormalSH                = Vertex + 1,
		Vertex_Vertex                        = Vertex + 2,
		Vertex_VertexDiffuse8                = Vertex + 3,
		Vertex_VertexUserFlags               = Vertex + 4,
		Vertex_VertexNinjaFlags              = Vertex + 5,
		Vertex_VertexDiffuseSpecular5        = Vertex + 6,
		Vertex_VertexDiffuseSpecular4        = Vertex + 7,
		Vertex_VertexDiffuseSpecular16       = Vertex + 8,
		Vertex_VertexNormal                  = Vertex + 9,
		Vertex_VertexNormalDiffuse8          = Vertex + 10,
		Vertex_VertexNormalUserFlags         = Vertex + 11,
		Vertex_VertexNormalNinjaFlags        = Vertex + 12,
		Vertex_VertexNormalDiffuseSpecular5  = Vertex + 13,
		Vertex_VertexNormalDiffuseSpecular4  = Vertex + 14,
		Vertex_VertexNormalDiffuseSpecular16 = Vertex + 15,
		Vertex_VertexNormalX                 = Vertex + 16,
		Vertex_VertexNormalXDiffuse8         = Vertex + 17,
		Vertex_VertexNormalXUserFlags        = Vertex + 18,
		Volume                               = 56,
		Volume_Polygon3                      = Volume + 0,
		Volume_Polygon4                      = Volume + 1,
		Volume_Strip                         = Volume + 2,
		Strip                                = 64,
		Strip_Strip                          = Strip + 0,
		Strip_StripUVN                       = Strip + 1,
		Strip_StripUVH                       = Strip + 2,
		Strip_StripNormal                    = Strip + 3,
		Strip_StripUVNNormal                 = Strip + 4,
		Strip_StripUVHNormal                 = Strip + 5,
		Strip_StripColor                     = Strip + 6,
		Strip_StripUVNColor                  = Strip + 7,
		Strip_StripUVHColor                  = Strip + 8,
		Strip_Strip2                         = Strip + 9,
		Strip_StripUVN2                      = Strip + 10,
		Strip_StripUVH2                      = Strip + 11,
		End                                  = 255
	}

	public enum WeightStatus
	{
		Start,
		Middle,
		End
	}

	public enum InterpolationMode
	{
		Linear,
		Spline,
		User
	}

	public class StructEnums
	{
		const int BIT_0  = (1 << 0);
		const int BIT_1  = (1 << 1);
		const int BIT_2  = (1 << 2);
		const int BIT_3  = (1 << 3);
		const int BIT_4  = (1 << 4);
		const int BIT_5  = (1 << 5);
		const int BIT_6  = (1 << 6);
		const int BIT_7  = (1 << 7);
		const int BIT_8  = (1 << 8);
		const int BIT_9  = (1 << 9);
		const int BIT_10 = (1 << 10);
		const int BIT_11 = (1 << 11);
		const int BIT_12 = (1 << 12);
		const int BIT_13 = (1 << 13);
		const int BIT_14 = (1 << 14);
		const int BIT_15 = (1 << 15);
		const int BIT_16 = (1 << 16);
		const int BIT_17 = (1 << 17);
		const int BIT_18 = (1 << 18);
		const int BIT_19 = (1 << 19);
		const int BIT_20 = (1 << 20);
		const int BIT_21 = (1 << 21);
		const int BIT_22 = (1 << 22);
		const int BIT_23 = (1 << 23);
		const int BIT_24 = (1 << 24);
		const int BIT_25 = (1 << 25);
		const int BIT_26 = (1 << 26);
		const int BIT_27 = (1 << 27);
		const int BIT_28 = (1 << 28);
		const int BIT_29 = (1 << 29);
		const int BIT_30 = (1 << 30);
		const int BIT_31 = (1 << 31);

		[Flags]
		public enum NJD_EVAL
		{
			NJD_EVAL_UNIT_POS   = BIT_0, /* ignore translation */
			NJD_EVAL_UNIT_ANG   = BIT_1, /* ignore rotation */
			NJD_EVAL_UNIT_SCL   = BIT_2, /* ignore scaling */
			NJD_EVAL_HIDE       = BIT_3, /* do not draw model */
			NJD_EVAL_BREAK      = BIT_4, /* terminate tracing children */
			NJD_EVAL_ZXY_ANG    = BIT_5,
			NJD_EVAL_SKIP       = BIT_6,
			NJD_EVAL_SHAPE_SKIP = BIT_7,
			NJD_EVAL_CLIP       = BIT_8,
			NJD_EVAL_MODIFIER   = BIT_9
		}

		[Flags]
		public enum MaterialFlags
		{
			NJD_SA_ONE               = (BIT_29),                   /* 1 one                 */
			NJD_SA_OTHER             = (BIT_30),                   /* 2 Other Color         */
			NJD_SA_INV_OTHER         = (BIT_30 | BIT_29),          /* 3 Inverse Other Color */
			NJD_SA_SRC               = (BIT_31),                   /* 4 SRC Alpha           */
			NJD_SA_INV_SRC           = (BIT_31 | BIT_29),          /* 5 Inverse SRC Alpha   */
			NJD_SA_DST               = (BIT_31 | BIT_30),          /* 6 DST Alpha           */
			NJD_SA_INV_DST           = (BIT_31 | BIT_30 | BIT_29), /* 7 Inverse DST Alpha   */
			NJD_DA_ONE               = (BIT_26),                   /* 1 one                 */
			NJD_DA_OTHER             = (BIT_27),                   /* 2 Other Color         */
			NJD_DA_INV_OTHER         = (BIT_27 | BIT_26),          /* 3 Inverse Other Color */
			NJD_DA_SRC               = (BIT_28),                   /* 4 SRC Alpha           */
			NJD_DA_INV_SRC           = (BIT_28 | BIT_26),          /* 5 Inverse SRC Alpha   */
			NJD_DA_DST               = (BIT_28 | BIT_27),          /* 6 DST Alpha           */
			NJD_DA_INV_DST           = (BIT_28 | BIT_27 | BIT_26), /* 7 Inverse DST Alpha   */
			NJD_FILTER_BILINEAR      = (BIT_13),
			NJD_FILTER_TRILINEAR     = (BIT_14),
			NJD_FILTER_BLEND         = (BIT_14 | BIT_13),
			NJD_D_025                = (BIT_8),                           /* 0.25        */
			NJD_D_050                = (BIT_9),                           /* 0.50        */
			NJD_D_075                = (BIT_9 | BIT_8),                   /* 0.75        */
			NJD_D_100                = (BIT_10),                          /* 1.00        */
			NJD_D_125                = (BIT_10 | BIT_8),                  /* 1.25        */
			NJD_D_150                = (BIT_10 | BIT_9),                  /* 1.50        */
			NJD_D_175                = (BIT_10 | BIT_9 | BIT_8),          /* 1.75        */
			NJD_D_200                = (BIT_11),                          /* 2.00        */
			NJD_D_225                = (BIT_11 | BIT_8),                  /* 2.25        */
			NJD_D_250                = (BIT_11 | BIT_9),                  /* 2.50        */
			NJD_D_275                = (BIT_11 | BIT_9 | BIT_8),          /* 2.75        */
			NJD_D_300                = (BIT_11 | BIT_10),                 /* 3.00        */
			NJD_D_325                = (BIT_11 | BIT_10 | BIT_8),         /* 3.25        */
			NJD_D_350                = (BIT_11 | BIT_10 | BIT_9),         /* 3.50        */
			NJD_D_375                = (BIT_11 | BIT_10 | BIT_9 | BIT_8), /* 3.75        */
			NJD_FLAG_IGNORE_LIGHT    = (BIT_25),
			NJD_FLAG_USE_FLAT        = (BIT_24),
			NJD_FLAG_DOUBLE_SIDE     = (BIT_23),
			NJD_FLAG_USE_ENV         = (BIT_22),
			NJD_FLAG_USE_TEXTURE     = (BIT_21),
			NJD_FLAG_USE_ALPHA       = (BIT_20),
			NJD_FLAG_IGNORE_SPECULAR = (BIT_19),
			NJD_FLAG_FLIP_U          = (BIT_18),
			NJD_FLAG_FLIP_V          = (BIT_17),
			NJD_FLAG_CLAMP_U         = (BIT_16),
			NJD_FLAG_CLAMP_V         = (BIT_15),
			NJD_FLAG_USE_ANISOTROPIC = (BIT_12),
			NJD_FLAG_PICK            = (BIT_7),
		}

		public enum NJD_MESHSET
		{
			NJD_MESHSET_3       = 0x0000, /* polygon3 meshset         */
			NJD_MESHSET_4       = 0x4000, /* polygon4 meshset         */
			NJD_MESHSET_N       = 0x8000, /* polygonN meshset         */
			NJD_MESHSET_TRIMESH = 0xc000, /* trimesh meshset          */
		}

		[Flags]
		public enum NJD_CALLBACK
		{
			NJD_POLYGON_CALLBACK  = (BIT_31), /* polygon callback   */
			NJD_MATERIAL_CALLBACK = (BIT_30)  /* material callback  */
		}

		[Flags]
		public enum NJD_MTYPE
		{
			NJD_MTYPE_POS_0       = BIT_0,
			NJD_MTYPE_ANG_1       = BIT_1,
			NJD_MTYPE_SCL_2       = BIT_2,
			NJD_MTYPE_VEC_3       = BIT_3,
			NJD_MTYPE_VERT_4      = BIT_4,
			NJD_MTYPE_NORM_5      = BIT_5,
			NJD_MTYPE_TARGET_3    = BIT_6,
			NJD_MTYPE_ROLL_6      = BIT_7,
			NJD_MTYPE_ANGLE_7     = BIT_8,
			NJD_MTYPE_RGB_8       = BIT_9,
			NJD_MTYPE_INTENSITY_9 = BIT_10,
			NJD_MTYPE_SPOT_10     = BIT_11,
			NJD_MTYPE_POINT_10    = BIT_12,
			NJD_MTYPE_QUAT_1      = BIT_13
		}

		public enum NJD_MTYPE_FN
		{
			NJD_MTYPE_LINER  = 0x0000, /* use liner                */
			NJD_MTYPE_SPLINE = 0x0040, /* use spline               */
			NJD_MTYPE_USER   = 0x0080, /* use user function        */
			NJD_MTYPE_MASK   = 0x00c0  /* Sampling mask*/
		}
	}
}