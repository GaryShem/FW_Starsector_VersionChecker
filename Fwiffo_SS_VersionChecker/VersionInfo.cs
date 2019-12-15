using System;

namespace Fwiffo_SS_VersionChecker
{
    class VersionInfo
    {
        public string MasterVersionFile { get; set; }
        public string ModName { get; set; }
        public string ModThreadId { get; set; }

        public override string ToString()
        {
            return ModVersion.ToString();
        }

        public ModVersionInfo ModVersion { get; set; }

        public class ModVersionInfo
        {
            public string Major;
            public string Minor;
            public string Patch;

            public override string ToString()
            {
                if (Major != null && Minor != null && Patch != null)
                {
                    return $"{Major}.{Minor}.{Patch}";
                }
                else if (Major != null && Minor != null)
                {
                    return $"{Major}.{Minor}";
                }
                else if (Major != null && Patch != null)
                {
                    return $"{Major}.{Patch}";
                }
                else if (Minor != null && Patch != null)
                {
                    return $"{Minor}.{Patch}";
                }
                else if (Major != null)
                {
                    return $"{Major}";
                }
                else if (Minor != null)
                {
                    return $"{Minor}";
                }
                else if (Patch != null)
                {
                    return $"{Patch}";
                }
                else return "";
            }

            public override bool Equals(object obj)
            {
                ModVersionInfo mvi = obj as ModVersionInfo;
                if (mvi == null) return false;
                if (Major != null && Minor != null && Patch != null)
                {
                    return Major.Equals(mvi.Major) && Minor.Equals(mvi.Minor) && Patch.Equals(mvi.Patch);
                }
                else if (Major != null && Minor != null)
                {
                    return Major.Equals(mvi.Major) && Minor.Equals(mvi.Minor);
                }
                else if (Major != null && Patch != null)
                {
                    return Major.Equals(mvi.Major) && Patch.Equals(mvi.Patch);
                }
                else if (Minor != null && Patch != null)
                {
                    return Minor.Equals(mvi.Minor) && Patch.Equals(mvi.Patch);
                }
                else if (Major != null)
                {
                    return Major.Equals(mvi.Major);
                }
                else if (Minor != null)
                {
                    return Minor.Equals(mvi.Minor);
                }
                else if (Patch != null)
                {
                    return Patch.Equals(mvi.Patch);
                }
                else return false;
            }

            public int Compare(ModVersionInfo info)
            {
                if (info == null) return 1;
                int mjrCmp = String.Compare(Major, info.Major, StringComparison.OrdinalIgnoreCase);
                int mnrCmp = String.Compare(Minor, info.Minor, StringComparison.OrdinalIgnoreCase);
                int ptchCmp = String.Compare(Patch, info.Patch, StringComparison.OrdinalIgnoreCase);

                if (mjrCmp > 0 || mnrCmp > 0 || ptchCmp > 0) return 1;
                else if (mjrCmp < 0 || mnrCmp < 0 || ptchCmp < 0) return -1;
                else return 0;
            }
        }
    }
}