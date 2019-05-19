using System;
using System.IO;
using SonicRetro.SAModel.GC;
using System.Windows.Forms;

namespace SonicRetro.SAModel.SAMDL
{
	static class Program
	{
		static internal string[] Arguments { get; set; }

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			TryReadRingModel();

			Arguments = args;
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}

		static void TryReadRingModel()
		{
			//byte[] exe = File.ReadAllBytes(@"D:\Steam\steamapps\common\Sonic Adventure 2\resource\gd_PC\DLL\Win32\Data_DLL_orig.dll");

			//GCAttach test = new GCAttach(exe, 0xC61C8C, 0x10002000);
			//test.ExportOBJ(@"D:\Steam\steamapps\common\Sonic Adventure 2\ring.obj");
		}
	}
}
