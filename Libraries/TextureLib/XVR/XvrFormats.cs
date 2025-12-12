using System;
using static TextureLib.DirectXTexUtility;

namespace TextureLib
{
	/// <summary>
	/// XVR pixel format byte variations. There can be different variations indicating the same format, such as Rgb565 (2) and Rgb565Alt (12).
	/// </summary>
	public enum XvrFormat
	{
		Argb8888 = 1,
		Argb8888Alt = 11,
		Rgb565 = 2,
		Rgb565Alt = 12,
		Argb1555 = 3,
		Argb1555Alt = 13,
		Argb4444 = 4,
		Argb4444Alt = 14,
		Index8 = 5,
		Dxt1 = 6,
		Dxt3 = 7,
		Dxt3Alt = 8,
		Dxt5 = 9,
		Dxt5Alt = 10,
		Yuv422 = 15,
		Vu88 = 16,
		A8 = 17,
		XRgb1555 = 18,
		XRgb8888 = 19,
		Invalid = -1
	}

	[Flags]
	public enum XvrFlags
	{
		Mips = 1,
		Alpha = 2,
	}

	/// <summary>
	/// Class used for conversions of XVR and DDS format values.
	/// </summary>
	public static class XvrFormats
	{
		/// <summary>
		/// Converts a DdsTexture format value to an XvrTexture format value.
		/// </summary>
		/// <param name="format">Input format value of the DDS texture.</param>
		/// <exception cref="NotImplementedException"></exception>
		/// <returns>Output format value of the XVR texture.</returns>
		public static XvrFormat GetXvrPixelFormatFromDdsFormat(DdsFormat format)
		{
			switch (format)
			{
				case DdsFormat.Argb8888:
					return XvrFormat.Argb8888;
				case DdsFormat.Rgb565:
					return XvrFormat.Rgb565;
				case DdsFormat.Argb1555:
					return XvrFormat.Argb1555;
				case DdsFormat.Argb4444:
					return XvrFormat.Argb4444;
				case DdsFormat.Dxt1:
					return XvrFormat.Dxt1;
				case DdsFormat.Dxt3:
					return XvrFormat.Dxt3;
				case DdsFormat.Dxt5:
					return XvrFormat.Dxt5;
				case DdsFormat.Rgb888:
				default:
					throw new NotImplementedException(string.Format("XVR conversion from DDS format {0} is not implemented.", format));
			}
		}

		/// <summary>
		/// Converts an XvrTexture format value to a DdsTexture format value.
		/// </summary>
		/// <param name="format">Input format value of the XVR texture.</param>
		/// <exception cref="NotImplementedException"></exception>
		/// <returns>Output format value of the DDS texture.</returns>
		public static DdsFormat GetDdsFormatFromXvrPixelFormat(XvrFormat xvrPixelFormat)
		{
			switch (xvrPixelFormat)
			{
				case XvrFormat.Argb8888:
				case XvrFormat.Argb8888Alt:
					return DdsFormat.Argb8888;
				case XvrFormat.Rgb565:
				case XvrFormat.Rgb565Alt:
					return DdsFormat.Rgb565;
				case XvrFormat.Argb1555:
				case XvrFormat.Argb1555Alt:
					return DdsFormat.Argb1555;
				case XvrFormat.Argb4444:
				case XvrFormat.Argb4444Alt:
					return DdsFormat.Argb4444;
				case XvrFormat.Dxt1:
					return DdsFormat.Dxt1;
				case XvrFormat.Dxt3:
				case XvrFormat.Dxt3Alt:
					return DdsFormat.Dxt3;
				case XvrFormat.Dxt5:
				case XvrFormat.Dxt5Alt:
					return DdsFormat.Dxt5;
				default:
					throw new NotImplementedException(string.Format("Conversion for XVR format {0} ({1}) is not implemented.", xvrPixelFormat.ToString(), xvrPixelFormat));
			}
		}

		/// <summary>
		/// Converts an XvrTexture format value to a DXGI format value used by DirectXTexUtility.
		/// </summary>
		/// <param name="pixelFormat">Input format value of the XVR texture.</param>
		/// <returns>Output DXGI format value.</returns>
		public static DXGIFormat GetDxgiFormatFromXvrPixelFormat(XvrFormat pixelFormat)
		{
			switch (pixelFormat)
			{
				case XvrFormat.Argb8888:
				case XvrFormat.Argb8888Alt:
					return DXGIFormat.B8G8R8A8UNORM;
				case XvrFormat.Rgb565:
				case XvrFormat.Rgb565Alt:
					return DXGIFormat.B5G6R5UNORM;
				case XvrFormat.Argb1555:
				case XvrFormat.Argb1555Alt:
					return DXGIFormat.B5G5R5A1UNORM;
				case XvrFormat.Argb4444:
				case XvrFormat.Argb4444Alt:
					return DXGIFormat.B4G4R4A4UNORM;
				case XvrFormat.Index8:
					return DXGIFormat.P8;
				case XvrFormat.Dxt1:
					return DXGIFormat.BC1UNORM;
				case XvrFormat.Dxt3:
				case XvrFormat.Dxt3Alt:
					return DXGIFormat.BC2UNORM;
				case XvrFormat.Dxt5:
				case XvrFormat.Dxt5Alt:
					return DXGIFormat.BC3UNORM;
				case XvrFormat.Yuv422:
					return DXGIFormat.YUY2;
				case XvrFormat.Vu88:
					return DXGIFormat.R8G8SNORM;
				case XvrFormat.A8:
					return DXGIFormat.A8UNORM;
				case XvrFormat.XRgb1555:
				case XvrFormat.XRgb8888:
					return DXGIFormat.UNKNOWN;
				default:
					return DXGIFormat.BC1UNORM;
			}
		}	
	}
}