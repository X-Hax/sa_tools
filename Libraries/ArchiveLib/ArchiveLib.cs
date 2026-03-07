using System.Collections.Generic;
using System.IO;
using System.Drawing;

// Archive library for SA Tools

namespace ArchiveLib
{	
	/// <summary>
	/// Class for archives supported by SA Tools. This is a generic class that doesn't access format-specific properties.
	/// </summary>
	public abstract class GenericArchive
	{
		/// <summary>This field is true when the archive stores filenames.</summary>
		public bool HasNameData = true;

		/// <summary>List of individual archive entries.</summary>
		public List<GenericArchiveEntry> Entries { get; set; }

		public GenericArchive()
		{
			Entries = new List<GenericArchiveEntry>();
		}

		/// <summary>Returns a new archive entry that is relevant to the archive type.</summary>
		/// <returns>New PVMEntry if the archive is a PVM, new PVMXEntry if the archive is a PVMX etc.</returns>
		public abstract GenericArchiveEntry NewEntry();

		/// <summary>Writes the archive to the specified path.</summary>
		/// <param name="outputFile">Path to the output file.</param>
		public void Save(string outputFile)
		{
			File.WriteAllBytes(outputFile, GetBytes());
		}

		/// <summary>Gets the archive's binary data as a byte array.</summary>
		/// <returns>Array of bytes.</returns>
		public abstract byte[] GetBytes();

		/// <summary>Creates a default format index file used by ArchiveTool for building archives.</summary>
		/// <param name="path">Path to the output index file.</param>
		internal void CreateDefaultIndexFile(string path)
		{
			using (TextWriter texList = File.CreateText(Path.Combine(path, "index.txt")))
			{
				foreach (GenericArchiveEntry pvmentry in Entries)
				{
					texList.WriteLine(pvmentry.Name);
				}
				texList.Flush();
				texList.Close();
			}
		}

		/// <summary>
		/// Creates a format-specific index file used by ArchiveTool for building archives.
		/// If the format does not require a specific index file but the file order is still important, this function should call CreateDefaultIndexFile().
		/// </summary>
		/// <param name="path">Path to the output index file.</param>
		public abstract void CreateIndexFile(string path);

		/// <summary>Individual entry in the archive.
		/// The common properties are Name (numeric ID if the actual name is not defined) and Data as a byte array.
		/// </summary>
		public abstract class GenericArchiveEntry
		{
			/// <summary>
			/// Entry filename with extension.
			/// If the archive doesn't store entry names, a numeric ID will be used.
			/// </summary>
			public string Name { get; set; }

			/// <summary>Entry binary data.</summary>
			public byte[] Data { get; set; }

			/// <summary>
			/// Returns the entry's data converted to System.Drawing.Bitmap.
			/// Can be used to retrieve texture preview images.
			/// </summary>
			/// <returns>Bitmap containing the texture image.</returns>
			public abstract Bitmap GetBitmap();

			public GenericArchiveEntry()
			{
				Name = string.Empty;
			}
		}
	}
}