using System;
using System.Windows.Forms;

namespace SASave
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm(args));
		}

		public static string ToInvariantString(this int @int)
		{
			return @int.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
		}

		public static T[] CastClone<T>(this T[] array)
		{
			return (T[])array.Clone();
		}
	}
}
