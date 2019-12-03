using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Fwiffo_SS_VersionChecker
{
    static class Util
    {
        public static string ValidateJSON(string json)
        {
            json = Regex.Replace(json, @"#[^\r\n]*", "");
            json = Regex.Replace(json, @"(""\w+"":) *([\.\w]+)(,*)(?=[\r\n\t])", @"$1 ""$2""$3");
            json = Regex.Replace(json, @"('\w+':) *([\.\w]+)(,*)(?=[\r\n\t])", @"$1 '$2'$3");
            return json;
        }

        public static ModInfo ReadModInfo(string modPath)
        {
            ModInfo modInfo = null;
            string modInfoFilePath = Directory.GetFiles(modPath, "mod_info.json").FirstOrDefault();
            string modInfoFile = File.ReadAllText(modInfoFilePath);
            modInfoFile = ValidateJSON(modInfoFile);
            modInfo = JsonConvert.DeserializeObject<ModInfo>(modInfoFile);
            return modInfo;
        }

        public static VersionInfo ReadVersionInfo(string modPath)
        {
            VersionInfo versionInfo = null;
            string versionFilePath = Directory.GetFiles(modPath, "*.version").FirstOrDefault();
            if (versionFilePath != null)
            {
                string versionInfoFile = File.ReadAllText(versionFilePath);
                versionInfoFile = ValidateJSON(versionInfoFile);
                versionInfo = JsonConvert.DeserializeObject<VersionInfo>(versionInfoFile);
            }

            return versionInfo;
        }

        public static VersionInfo ReadVersionInfo(Uri uri)
        {
            VersionInfo versionInfo = null;
            string versionInfoFile;
            using (WebClient webClient = new WebClient())
            {
                versionInfoFile = webClient.DownloadString(uri);
            }

            if (versionInfoFile != null)
            {
                versionInfoFile = ValidateJSON(versionInfoFile);
                versionInfo = JsonConvert.DeserializeObject<VersionInfo>(versionInfoFile);
            }

            return versionInfo;
        }

        public static void OpenModForumThreads(List<ModInfo> mods)
        {
            foreach (ModInfo modInfo in mods)
            {
                var thread = modInfo.ModThread;
                if (thread != null)
                    System.Diagnostics.Process.Start(thread);
            }
        }
    }
}