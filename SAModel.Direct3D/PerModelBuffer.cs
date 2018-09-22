using SharpDX;

namespace SonicRetro.SAModel.Direct3D
{
	public class PerModelBuffer : IModifiable
	{
		public static int SizeInBytes => 2 * Matrix.SizeInBytes;

		public bool Modified => World.Modified || wvMatrixInvT.Modified;

		public readonly Modifiable<Matrix> World        = new Modifiable<Matrix>();
		public readonly Modifiable<Matrix> wvMatrixInvT = new Modifiable<Matrix>();

		public void Clear()
		{
			World.Clear();
			wvMatrixInvT.Clear();
		}
	}
}