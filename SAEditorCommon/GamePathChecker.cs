using System;
using System.IO;

namespace SonicRetro.SAModel.SAEditorCommon
{
	public static class GamePathChecker
	{
		public static bool CheckSADXPCValid(string sadxPCPath, out string failReason)
		{
			bool sadxPCIsValid = true;

			try // get cast exceptions here
			{
				bool sadxPCPathStringExists = sadxPCPath != null && sadxPCPath.Length > 0;
				bool sadxPCPathExists = Directory.Exists(sadxPCPath);
				bool sonicExeExists = File.Exists(string.Concat(sadxPCPath + "\\", "sonic.exe")); // todo: maybe md5 check this so that we can tell it's the right version?
				bool modLoaderPresent = File.Exists(string.Concat(sadxPCPath + "\\", "system\\chrmodels_orig.dll"));

				if (!sadxPCPathStringExists)
				{
					failReason = "No SADX:PC game path was supplied.";
					return false;
				}
				else if (!sadxPCPathExists)
				{
					failReason = string.Format("SADX:PC path {0} did not exist or was invalid.", sadxPCPath);
					return false;
				}
				else if (!sonicExeExists)
				{
					failReason = "SADX:PC Game path does not contain sonic.exe.";
					return false;
				}
				else if (!modLoaderPresent)
				{
					failReason = "SADX Mod Loader is not installed.";
					return false;
				}
			}
			catch (InvalidCastException)
			{
				failReason = "sadxpc path settings variable was not string type.";
				sadxPCIsValid = false;
			}

			failReason = "";
			return sadxPCIsValid;
		}

		public static bool CheckSA2PCValid(string sa2PCPath, out string failReason)
		{
			bool sa2PCIsValid = true;

			try // get cast exceptions here
			{
				bool sa2PCPathStringExists = sa2PCPath != null && sa2PCPath.Length > 0;
				bool sa2PCPathExists = Directory.Exists(sa2PCPath);
				bool sonic2ExeExists = File.Exists(string.Concat(sa2PCPath + "\\", "sonic2app.exe"));
				bool modLoaderPresent = File.Exists(string.Concat(sa2PCPath + "\\", "resource\\gd_PC\\DLL\\Win32\\Data_DLL_orig.dll"));

				if (!sa2PCPathStringExists)
				{
					failReason = "No SA2:PC game path was supplied.";
					return false;
				}
				else if (!sa2PCPathExists)
				{
					failReason = string.Format("SA2:PC path {0} did not exist or was invalid.", sa2PCPath);
					return false;
				}
				else if (!sonic2ExeExists)
				{
					failReason = "SA2:PC game path does not contain sonic2app.exe";
					return false;
				}
				else if (!modLoaderPresent)
				{
					failReason = "SA2:PC Mod Manager is not installed.";
					return false;
				}
			}
			catch (InvalidCastException)
			{
				sa2PCIsValid = false;
			}

			failReason = "";
			return sa2PCIsValid;
		}

		public static string PathOrFallback(string path, string fallbackPath)
		{
			return (File.Exists(path)) ? path : fallbackPath;
		}

		/// <summary>
		/// Gets the primary game content folder.
		/// For SADX PC this is called 'System',
		/// for SA2PC it's called 'gd_PC'
		/// </summary>
		/// <param name="game"></param>
		/// <returns></returns>
		public static string GetSystemPathName(SA_Tools.Game game)
		{
			switch (game)
			{
				case SA_Tools.Game.SA1:
				case SA_Tools.Game.SA2:
					throw new System.NotSupportedException();

				case SA_Tools.Game.SADX:
					return "system";

				case SA_Tools.Game.SA2B:
					return "resource/gd_PC";

				default:
					throw new System.NotSupportedException();
			}
		}
	}
}
