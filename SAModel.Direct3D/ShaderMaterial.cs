using SharpDX;

namespace SonicRetro.SAModel.Direct3D
{
	public struct ShaderMaterial
	{
		public static int SizeInBytes => 2 * Vector4.SizeInBytes + sizeof(float) + 5 * sizeof(int);

		public Color4 Diffuse;
		public Color4 Specular;
		public float  Exponent;
		public bool   UseLight;
		public bool   UseAlpha;
		public bool   UseEnv;
		public bool   UseTexture;
		public bool   UseSpecular;

		public bool Equals(ShaderMaterial other)
		{
			return Diffuse == other.Diffuse
			       && Specular == other.Specular
			       && Exponent == other.Exponent
			       && UseLight == other.UseLight
			       && UseAlpha == other.UseAlpha
			       && UseEnv == other.UseEnv
			       && UseTexture == other.UseTexture
			       && UseSpecular == other.UseSpecular;
		}

		public override bool Equals(object obj)
		{
			if (obj is null)
			{
				return false;
			}

			return obj is ShaderMaterial material && Equals(material);
		}

		public static bool operator ==(ShaderMaterial lhs, ShaderMaterial rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(ShaderMaterial lhs, ShaderMaterial rhs)
		{
			return !(lhs == rhs);
		}

		public override int GetHashCode()
		{
			return 1;
		}
	}
}