using System;

namespace SonicRetro.SAModel.Direct3D
{
	[Flags]
	public enum ShaderFlags
	{
		Light   = 1 << 0,
		Texture = 1 << 1,
		Fog     = 1 << 2
	}
}