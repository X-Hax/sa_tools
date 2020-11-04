namespace SonicRetro.SAModel.DataToolbox
{
	using System.Windows.Forms; // for XML docs

	/// <summary>
	/// Defines the possible modes for a <see cref="FileSelector"/>.
	/// </summary>
	public enum FileSelectorMode
	{
		/// <summary>
		/// The <see cref="FileSelector"/> will use an <see cref="OpenFileDialog"/>.
		/// </summary>
		Open,

		/// <summary>
		/// The <see cref="FileSelector"/> will use a <see cref="SaveFileDialog"/>.
		/// </summary>
		Save
	}
}
