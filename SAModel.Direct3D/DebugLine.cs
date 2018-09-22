namespace SonicRetro.SAModel.Direct3D
{
	public struct DebugLine
	{
		public static int SizeInBytes => 2 * DebugPoint.SizeInBytes;

		public DebugPoint PointA, PointB;

		public DebugLine(DebugPoint a, DebugPoint b)
		{
			PointA = a;
			PointB = b;
		}
	}
}