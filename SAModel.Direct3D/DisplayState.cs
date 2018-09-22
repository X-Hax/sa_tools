using System;
using SharpDX.Direct3D11;

namespace SonicRetro.SAModel.Direct3D
{
	public class DisplayState : IDisposable
	{
		public SamplerState    Sampler { get; }
		public RasterizerState Raster  { get; }
		public BlendState      Blend   { get; }

		public DisplayState(SamplerState sampler, RasterizerState raster, BlendState blend)
		{
			Sampler = sampler;
			Raster  = raster;
			Blend   = blend;
		}

		public void Dispose()
		{
			Sampler?.Dispose();
			Raster?.Dispose();
			Blend?.Dispose();
		}
	}
}