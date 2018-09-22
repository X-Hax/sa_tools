using SharpDX;

namespace SonicRetro.SAModel.Direct3D
{
	public class PerSceneBuffer : IModifiable
	{
		public static int SizeInBytes => Vector3.SizeInBytes + 2 * Matrix.SizeInBytes;

		public bool Modified => View.Modified || Projection.Modified || CameraPosition.Modified;

		public readonly Modifiable<Matrix>  View           = new Modifiable<Matrix>();
		public readonly Modifiable<Matrix>  Projection     = new Modifiable<Matrix>();
		public readonly Modifiable<Vector3> CameraPosition = new Modifiable<Vector3>();

		public void Clear()
		{
			View.Clear();
			Projection.Clear();
			CameraPosition.Clear();
		}
	}
}