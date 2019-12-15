using Newtonsoft.Json;
using System;

namespace Fwiffo_SS_VersionChecker
{
    class ModInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }

        [JsonIgnore] public VersionInfo LocalVersionInfo { get; set; } = null;
        [JsonIgnore] public VersionInfo MasterVersionInfo { get; set; } = null;
        
        public string ModThread
        {
            get
            {
                if (String.IsNullOrWhiteSpace(LocalVersionInfo.ModThreadId)) return null;
                return $"http://fractalsoftworks.com/forum/index.php?topic={LocalVersionInfo.ModThreadId}.0";
            }
        }

        public bool HasUpdate()
        {
            if (LocalVersionInfo == null || MasterVersionInfo == null)
                return false;
            if (LocalVersionInfo.ModVersion == null || MasterVersionInfo.ModVersion == null)
                return false;

            return LocalVersionInfo.ModVersion.Compare(MasterVersionInfo.ModVersion) < 0;
        }

        public override string ToString()
        {
            if (HasUpdate())
            {
                return $"{Name} ({LocalVersionInfo} => {MasterVersionInfo})";
            }
            else if (LocalVersionInfo != null)
            {
                return $"{Name} ({LocalVersionInfo})";
            }
            else
            {
                return $"{Name} ({Version})";
            }
        }
    }
}