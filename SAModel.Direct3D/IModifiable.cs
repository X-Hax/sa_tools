namespace SonicRetro.SAModel.Direct3D
{
	public interface IModifiable
	{
		bool Modified { get; }
		void Clear();
	}
}
