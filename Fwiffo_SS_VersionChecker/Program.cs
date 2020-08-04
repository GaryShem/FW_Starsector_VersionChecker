using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Fwiffo_SS_VersionChecker
{
    internal class Program
    {
        static List<ModInfo> allMods = new List<ModInfo>();
        static List<ModInfo> unsupportedMods = new List<ModInfo>();
        static List<ModInfo> masterlessMods = new List<ModInfo>();
        static List<ModInfo> modsUpdate = new List<ModInfo>();
        static List<ModInfo> modsNoUpdate = new List<ModInfo>();

        public static void Main(string[] args)
        {
            ReadMods();
            DisplayMenu();
            while (true)
            {
                var key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                    case ConsoleKey.D0:
                    case ConsoleKey.NumPad0:
                        return;
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        DisplayModsNoUpdates();
                        DisplayMenu();
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        DisplayModsUpdate();
                        DisplayMenu();
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        Util.OpenModForumThreads(modsUpdate);
                        break;
                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        DisplayModsUnsupported();
                        DisplayMenu();
                        break;
                    case ConsoleKey.D5:
                    case ConsoleKey.NumPad5:
                        DisplayModsError();
                        DisplayMenu();
                        break;
                    case ConsoleKey.D6:
                    case ConsoleKey.NumPad6:
                        ReadMods();
                        DisplayMenu();
                        break;
                }
            }
        }

        public static void DisplayMenu()
        {
            Console.WriteLine();
            DisplaySummary();
            Console.WriteLine();
            Console.WriteLine("1. List mods without updates");
            Console.WriteLine("2. List mods with updates");
            Console.WriteLine("3. Open page for all mods with an update available");
            Console.WriteLine("4. List unsupported mods");
            Console.WriteLine("5. List mods with errors in version check");
            Console.WriteLine("6. Read mods again");
            Console.WriteLine("0/Enter. Exit");
            Console.Write("-> ");
        }

        private static void DisplayAllMods()
        {
            DisplayModsNoUpdates();
            DisplayModsUpdate();
            DisplayModsError();
            DisplayModsUnsupported();
        }

        private static void DisplaySummary()
        {
            Console.WriteLine($"There are {modsNoUpdate.Count} up-to-date mods");
            Console.WriteLine($"There are {modsUpdate.Count} mods with updates available");
            Console.WriteLine($"There are {masterlessMods.Count} mods with errors reading master version");
            Console.WriteLine($"There are {unsupportedMods.Count} unsupported mods");
        }

        private static void DisplayModsUnsupported()
        {
            Console.WriteLine();
            Console.WriteLine($"There are {unsupportedMods.Count} unsupported mods");
            foreach (ModInfo modInfo in unsupportedMods)
            {
                Console.WriteLine(modInfo);
            }
        }

        private static void DisplayModsError()
        {
            Console.WriteLine();
            Console.WriteLine($"There are {masterlessMods.Count} mods with errors reading master version");
            foreach (ModInfo modInfo in masterlessMods)
            {
                Console.WriteLine(modInfo);
            }
        }

        private static void DisplayModsUpdate()
        {
            Console.WriteLine();
            Console.WriteLine($"There are {modsUpdate.Count} mods with updates available");
            foreach (ModInfo modInfo in modsUpdate)
            {
                Console.WriteLine(modInfo);
            }
        }

        private static void DisplayModsNoUpdates()
        {
            Console.WriteLine();
            Console.WriteLine($"There are {modsNoUpdate.Count} up-to-date mods");
            foreach (ModInfo modInfo in modsNoUpdate)
            {
                Console.WriteLine(modInfo);
            }
        }

        private static void ReadMods()
        {
            
            string starsectorFolderPath = File.ReadLines("config.txt").FirstOrDefault();
            string modFolderPath = null;
#if DEBUG
            starsectorFolderPath = @"D:\Starsector";
#endif
            try
            {
                string exeFileSS = Directory
                    .GetFiles(starsectorFolderPath, "starsector.exe", SearchOption.TopDirectoryOnly).FirstOrDefault();
                if (exeFileSS == null)
                {
                    throw new ArgumentException("Starsector executable not found");
                }

                modFolderPath = Directory.GetDirectories(starsectorFolderPath, "mods", SearchOption.TopDirectoryOnly).FirstOrDefault();
            }
            catch (Exception ex)
            {
            }

            if (modFolderPath == null)
            {
                starsectorFolderPath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
                string exeFileSS = Directory
                    .GetFiles(starsectorFolderPath, "starsector.exe", SearchOption.TopDirectoryOnly).FirstOrDefault();
                if (exeFileSS == null)
                {
                    Console.WriteLine(
                        "Starsector executable not found, specify Starsector's core folder in config.txt " +
                        "or place the application folder into Starsector's core folder");
                    Console.WriteLine("Press any key to close the app");
                    Console.ReadKey();
                    Environment.Exit(0);
                }

                modFolderPath = Directory.GetDirectories(starsectorFolderPath, "mods", SearchOption.TopDirectoryOnly)
                    .FirstOrDefault();
                if (modFolderPath == null)
                {
                    Console.WriteLine(
                        "Starsector mod folder not found (should not happen), contact Fwiffo in Starsector Discord");
                    Console.WriteLine("Press any key to close the app");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }

            Console.WriteLine("Reading mods, wait a moment");

            var modFolders = Directory.GetDirectories(modFolderPath);
#if DEBUG
            SingleThread(modFolders); 
#else
            MultiThread(modFolders);
#endif
            allMods = allMods.Distinct().ToList();
            modsUpdate = modsUpdate.Distinct().ToList();
            modsNoUpdate = modsNoUpdate.Distinct().ToList();
            masterlessMods = masterlessMods.Distinct().ToList();
            unsupportedMods = unsupportedMods.Distinct().ToList();
            
            unsupportedMods.Sort((x, y) => x.Name.CompareTo(y.Name));
            allMods.Sort((x, y) => x.Name.CompareTo(y.Name));
            modsUpdate.Sort((x, y) => x.Name.CompareTo(y.Name));
            modsNoUpdate.Sort((x, y) => x.Name.CompareTo(y.Name));
            masterlessMods.Sort((x, y) => x.Name.CompareTo(y.Name));
            Console.WriteLine("Done reading mods");
        }

        private static void SingleThread(string[] modFolders)
        {
            foreach (var modPath in modFolders)
            {
                ModInfo modInfo = Util.ReadModInfo(modPath);

                lock (allMods)
                    allMods.Add(modInfo);

                modInfo.LocalVersionInfo = Util.ReadVersionInfo(modPath);
                if (modInfo.LocalVersionInfo != null && modInfo.LocalVersionInfo.MasterVersionFile != null)
                {
                    try
                    {
                        modInfo.MasterVersionInfo =
                            Util.ReadVersionInfo(new Uri(modInfo.LocalVersionInfo.MasterVersionFile));
                    }
                    catch (WebException e)
                    {
                        lock (masterlessMods)
                            masterlessMods.Add(modInfo);
                        continue;
                    }
                    catch (JsonReaderException e)
                    {
                        lock (masterlessMods)
                            masterlessMods.Add(modInfo);
                        continue;
                    }
                }

                if (modInfo.LocalVersionInfo == null)
                {
                    lock (unsupportedMods)
                        unsupportedMods.Add(modInfo);
                }
                else if (modInfo.MasterVersionInfo == null)
                {
                    lock (masterlessMods)
                        masterlessMods.Add(modInfo);
                }
                else if (modInfo.HasUpdate())
                {
                    lock (modsUpdate)
                        modsUpdate.Add(modInfo);
                }
                else if (modInfo.HasUpdate() == false)
                {
                    lock (modsNoUpdate)
                        modsNoUpdate.Add(modInfo);
                }
                else
                {
                    lock (allMods)
                    {
                        Console.WriteLine($"{modInfo.Name} is weird, please report it to Fwiffo in Starsector Discord");
                    }
                }
            }
        }
        
        private static void MultiThread(string[] modFolders)
        {
            Parallel.ForEach(modFolders, modPath =>
            {
                ModInfo modInfo = Util.ReadModInfo(modPath);

                lock (allMods)
                    allMods.Add(modInfo);

                modInfo.LocalVersionInfo = Util.ReadVersionInfo(modPath);
                if (modInfo.LocalVersionInfo != null && modInfo.LocalVersionInfo.MasterVersionFile != null)
                {
                    try
                    {
                        modInfo.MasterVersionInfo =
                            Util.ReadVersionInfo(new Uri(modInfo.LocalVersionInfo.MasterVersionFile));
                    }
                    catch (WebException e)
                    {
                        lock (masterlessMods)
                            masterlessMods.Add(modInfo);
                        return;
                    }
                    catch (JsonReaderException e)
                    {
                        lock (masterlessMods)
                            masterlessMods.Add(modInfo);
                        return;
                    }
                }

                if (modInfo.LocalVersionInfo == null)
                {
                    lock (unsupportedMods)
                        unsupportedMods.Add(modInfo);
                }
                else if (modInfo.MasterVersionInfo == null)
                {
                    lock (masterlessMods)
                        masterlessMods.Add(modInfo);
                }
                else if (modInfo.HasUpdate())
                {
                    lock (modsUpdate)
                        modsUpdate.Add(modInfo);
                }
                else if (modInfo.HasUpdate() == false)
                {
                    lock (modsNoUpdate)
                        modsNoUpdate.Add(modInfo);
                }
                else
                {
                    lock (allMods)
                    {
                        Console.WriteLine($"{modInfo.Name} is weird, please report it to Fwiffo in Starsector Discord");
                    }
                }
            });
        }
    }
}