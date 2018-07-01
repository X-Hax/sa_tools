using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                bool modLoaderPresent = File.Exists(string.Concat(sadxPCPath + "\\", "SADXModManager.exe"));

                if (!sadxPCPathStringExists)
                {
                    failReason = "SADX PC path string did not exist in settings";
                    return false;
                }
                else if (!sadxPCPathExists)
                {
                    failReason = string.Format("SADX PC path {0} did not exist or was invalid.", sadxPCPath);
                    return false;
                }
                else if (!sonicExeExists)
                {
                    failReason = "SADX PC path did not contain sonic.exe";
                    return false;
                }
                else if (!modLoaderPresent)
                {
                    failReason = "SADX PC path did not contain mod loader";
                    return false;
                }
            }
            catch (System.InvalidCastException e)
            {
                failReason = "sadxpc path settings variable was not string type";
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
                bool modLoaderPresent = File.Exists(string.Concat(sa2PCPath + "\\", "SA2ModManager.exe"));

                if (!sa2PCPathStringExists)
                {
                    failReason = "SA2 PC path string did not exist in settings";
                    return false;
                }
                else if (!sa2PCPathExists)
                {
                    failReason = string.Format("SA2 PC path {0} did not exist or was invalid.", sa2PCPath);
                    return false;
                }
                else if (!sonic2ExeExists)
                {
                    failReason = "SA2 PC path did not contain sonic2app.exe";
                    return false;
                }
                else if (!modLoaderPresent)
                {
                    failReason = "SA2 PC path did not contain mod loader";
                    return false;
                }
            }
            catch (System.InvalidCastException e)
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
    }
}
