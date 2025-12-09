namespace TextureLib
{
	/// <summary>Class for invalid, unknown or non-texture data (e.g. some files in SA2 PAKs)</summary>
	public class InvalidTexture : GenericTexture
	{
		/// <summary>Create an invalid texture instance from raw binary data</summary>
		public InvalidTexture(byte[] data, int offset = 0, string name = null)
		{
			InitTexture(data, offset, name);
			Image = Properties.Resources.InvalidTexturePreview;
			Width = Image.Width;
			Height = Image.Height;
		}

		public override byte[] GetBytes()
		{
			return RawData;
		}

		public override void Decode()
		{
			return;
		}

		public override void Encode()
		{
			return;
		}

		public override void AddMipmaps()
		{
			return;
		}

		public override void RemoveMipmaps()
		{
			return;
		}
	}
}