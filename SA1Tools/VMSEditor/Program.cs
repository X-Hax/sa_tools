using System;
using System.IO;
using System.Windows.Forms;
using System.Text;

namespace VMSEditor
{
	static partial class Program
	{
		internal static string[] args;
		public static Form primaryForm;

		public static Form CheckData(byte[] file)
		{
			// Garden save or Chao upload
			if (Encoding.GetEncoding(932).GetString(file, 0, 4) == "CHAO" || Encoding.GetEncoding(932).GetString(file, 0, 6) == "A-LIFE" || BitConverter.ToUInt32(file, 0) == 0x5FB5ACC1)
				return new EditorChao();
			// Event result
			else if (Encoding.GetEncoding(932).GetString(file, 0, 12) == "EVENT_RESULT" || BitConverter.ToUInt32(file, 0) == 0xDDDECDB2)
				return new EditorChallengeResult();
			// Cart result
			else if (Encoding.GetEncoding(932).GetString(file, 0, 9) == "CART_TIME" || BitConverter.ToUInt32(file, 0) == 0xC0C4B0B6)
				return new EditorChallengeResult();
			// World Ranking
			else if (Encoding.GetEncoding(932).GetString(file, 0, 11) == "DATA_UPLOAD" || BitConverter.ToUInt32(file, 0) == 0xC0B0DEC3)
				return new EditorWorldRank();
			// Download Data / Chao Adventure
			else
				for (int u = 0; u < file.Length - 11; u++)
					if (System.Text.Encoding.GetEncoding(932).GetString(file, u, 11) == "CHAO ACCEPT")
						return new EditorChao();
			// If none of the above is found, run the DLC editor
			return new EditorDLC();
		}

		public static Form CheckFile(string filepath)
		{
			switch (Path.GetExtension(filepath).ToLowerInvariant())
			{
				case ".dci":
					byte[] original = File.ReadAllBytes(filepath);
					byte[] vms = VMSFile.GetVMSFromDCI(original);
					return CheckData(vms);
				case ".vmi":
					return new EditorVMI();
				case ".vms":
				default:
					byte[] file = File.ReadAllBytes(filepath);
					return CheckData(file);
			}
		}

		[STAThread]
		static void Main(string[] args)
		{
			Program.args = args;
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
			Application.EnableVisualStyles();
			Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
			Application.SetCompatibleTextRenderingDefault(false);
			System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
			// No arguments: Select program mode
			if (args.Length == 0)
				primaryForm = new ProgramModeSelector();
			else if (args.Length == 2)
				GetPdata(args[0]);
			else
				primaryForm = CheckFile(args[0]);
			Application.Run(primaryForm);
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (primaryForm != null)
			{
				Exception ex = (Exception)e.ExceptionObject;
				string errDesc = "VMS Editor has crashed with the following error:\n" + ex.GetType().Name + ".\n\n" +
					"If you wish to report a bug, please include the following in your report:";
				SAModel.SAEditorCommon.ErrorDialog report = new SAModel.SAEditorCommon.ErrorDialog("VMS Editor", errDesc, ex.ToString());
				DialogResult dgresult = report.ShowDialog(primaryForm);
				switch (dgresult)
				{
					case DialogResult.Abort:
					case DialogResult.OK:
						Application.Exit();
						break;
				}
			}
			else
			{
				string logPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SA Tools", "VMSEditor.log");
				if (!Directory.Exists(Path.GetDirectoryName(logPath)))
					Directory.CreateDirectory(Path.GetDirectoryName(logPath));
				File.WriteAllText(logPath, e.ExceptionObject.ToString());
				MessageBox.Show("Unhandled Exception " + e.ExceptionObject.GetType().Name + "\nLog file has been saved to:\n" + logPath + ".", "VMS Editor Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		static void GetPdata(string filepath)
		{
			byte[] file = File.ReadAllBytes(filepath);
			byte[] data = VMSFile.GetDataFromHTML(file);
			PDATA pd = new PDATA(data, 8);
			File.WriteAllBytes(Path.ChangeExtension(filepath, ".pd"), pd.GetBytes());
			Application.Exit();
		}
	}
}