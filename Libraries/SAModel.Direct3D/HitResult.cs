using SharpDX;
using System;

namespace SonicRetro.SAModel.Direct3D
{
	public class HitResult : IComparable<HitResult>
	{
		public NJS_OBJECT Model { get; private set; }
		public float Distance { get; private set; }
		public Vector3 Position { get; private set; }
		public Vector3 Normal { get; private set; }
		public bool IsHit { get; private set; }

		private HitResult() { }

		public HitResult(NJS_OBJECT model, float distance, Vector3 position, Vector3 normal)
		{
			Model = model;
			Distance = distance;
			Position = position;
			Normal = normal;
			IsHit = true;
		}

		public static readonly HitResult NoHit = new HitResult();

		public static HitResult Min(HitResult a, HitResult b)
		{
			if (!a.IsHit) return b;
			if (!b.IsHit) return a;
			if (a.Distance < b.Distance) return a;
			else return b;
		}

		public int CompareTo(HitResult other)
		{
			if (!IsHit && !other.IsHit) return 0;
			if (!IsHit) return 1;
			if (!other.IsHit) return -1;
			return Distance.CompareTo(other.Distance);
		}

		public static bool operator <(HitResult a, HitResult b)
		{
			if (!a.IsHit && !b.IsHit) return false;
			if (!a.IsHit) return false;
			if (!b.IsHit) return true;
			return a.Distance < b.Distance;
		}

		public static bool operator >(HitResult a, HitResult b)
		{
			if (!a.IsHit && !b.IsHit) return false;
			if (!a.IsHit) return true;
			if (!b.IsHit) return false;
			return a.Distance > b.Distance;
		}
	}
}