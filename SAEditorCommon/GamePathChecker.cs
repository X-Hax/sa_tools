using SA_Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace SonicRetro.SAModel.SAEditorCommon
{
	public static class GamePathChecker
	{
		public static string getMD5(string filename)
		{
			using (var md5 = MD5.Create())
			{
				using (var stream = File.OpenRead(filename))
				{
					return md5.ComputeHash(stream).ToString();
				}
			}
		}

		public static bool checkMD5(Game game, string hash)
		{
			string sa1Hash = "060CAD2CEEFC07C7429085F30A356046";
			string sa1adHash = "FCB1DA8942278871136E41E127CE979B";
			string sa2Hash = "";
			string sa2ttHash = "";
			string sa2pHash = "";

			switch (game)
			{
				case (Game.SA1):
					if (hash.ToLower() == sa1Hash.ToLower())
						return true;
					else
						return false;
				case (Game.SA1AD):
					if (hash.ToLower() == sa1adHash.ToLower())
						return true;
					else
						return false;
				case (Game.SA2):
					if (hash.ToLower() == sa2Hash.ToLower())
						return true;
					else
						return false;
				case (Game.SA2TT):
					if (hash.ToLower() == sa2ttHash.ToLower())
						return true;
					else
						return false;
				case (Game.SA2P):
					if (hash.ToLower() == sa2pHash.ToLower())
						return true;
					else
						return false;
				default:
					return false;
			}
		}

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

		public static bool CheckBinaryPath(string system, SA_Tools.Game game, string gamePath, out string failReason)
		{
			bool gameIsValid = true;
			string dcGameVersion = "";
			try
			{
				switch (system)
				{
					case ("DC"):
						bool dcPathStringExists = gamePath != null && gamePath.Length > 0;
						bool dcPathExists = Directory.Exists(gamePath);
						bool dcFileExists = File.Exists(string.Concat(gamePath + "\\", "1ST_READ.BIN"));
						//string fileMD5 = getMD5(string.Concat(gamePath + "\\", "1ST_READ.BIN"));
						//bool dcFileHashCheck = checkMD5(game, fileMD5);

						if (!dcPathStringExists)
						{
							failReason = (game + "'s path was not supplied");
							return false;
						}
						else if (!dcPathExists)
						{
							failReason = (game + "'s path does not exist.");
							return false;
						}
						else if (!dcFileExists)
						{
							failReason = (game + "'s 1ST_READ.BIN was not located.");
							return false;
						}
						//else if (!dcFileHashCheck)
						//{
						//	switch (game)
						//	{
						//		case (Game.SA1):
						//			dcGameVersion = "Sonic Adventure (NTSC-U), Final Release, rev. 1.005, Build: Oct 10, 1995";
						//			break;
						//		case (Game.SA1AD):
						//			dcGameVersion = "Sonic Adventure AutoDemo (NTSC-J), rev. 1.000, Build: Oct 16, 1998";
						//			break;
						//		default:
						//			break;
						//	}
								
						//	failReason = (game + "'s 1ST_READ.BIN hash does not match the supported game version.\n\n" +
						//		"Please use the correct version of the game.\n\n" +
						//		"Supported version: " + dcGameVersion);
						//	return false;
						//}
						
						break;

					case ("GC"):
						bool gcPathStringExists = gamePath != null && gamePath.Length > 0;
						bool gcPathExists = Directory.Exists(gamePath);
						bool gcFileExists = File.Exists(string.Concat(gamePath + "\\", "_Main.rel"));

						if (!gcPathStringExists)
						{
							failReason = (game + "'s path was not supplied");
							return false;
						}
						else if (!gcPathExists)
						{
							failReason = (game + "'s path does not exist.");
							return false;
						}
						else if (!gcFileExists)
						{
							failReason = (game + "'s _Main.rel was not located.");
							return false;
						}
						break;

					case ("360"):
						bool xPathStringExists = gamePath != null && gamePath.Length > 0;
						bool xPathExists = Directory.Exists(gamePath);
						bool xFileExists = File.Exists(string.Concat(gamePath + "\\", "SonicApp.exe"));

						if (!xPathStringExists)
						{
							failReason = (game + "'s path was not supplied");
							return false;
						}
						else if (!xPathExists)
						{
							failReason = (game + "'s path does not exist.");
							return false;
						}
						else if (!xFileExists)
						{
							failReason = ("SonicApp.exe was not located.");
							return false;
						}
						break;

					default:
						break;
				}
			}
			catch (InvalidCastException)
			{
				gameIsValid = false;
			}

			failReason = "";
			return gameIsValid;
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
