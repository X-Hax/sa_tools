using System;
using System.IO;
using SharpDX.D3DCompiler;

namespace SonicRetro.SAModel.Direct3D
{
	class DefaultIncludeHandler : Include
	{
		public IDisposable Shadow { get; set; }

		public void Dispose()
		{
		}

		public Stream Open(IncludeType type, string fileName, Stream parentStream)
		{
			return File.Open(Path.Combine("Shaders", fileName), FileMode.Open);
		}

		public void Close(Stream stream)
		{
			stream?.Close();
		}
	}
}