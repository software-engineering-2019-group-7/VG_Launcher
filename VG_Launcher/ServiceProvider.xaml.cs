using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace VG_Launcher
{
    /// <summary>
    /// Interaction logic for ServiceProvider.xaml
    /// </summary>
    public partial class ServiceProvider : Window
    {
        public ServiceProvider()
        {
            InitializeComponent();
        } 

        public void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> serviceList = new List<string>();
            foreach (CheckBox c in services.Children)
            {
                if (c.IsChecked == true)
                {
                    serviceList.Add(c.Name);
                }
            }
            //SomeFunctionThatWillTakeInTheList();
            this.Close();
        }

        private string GetSteamDirectory()
        {
            string steamInstallPath = "";
            RegistryKey localMachineRegistry = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            RegistryKey steamRegistryKey = localMachineRegistry.OpenSubKey(@"SOFTWARE\Valve\Steam");
            if (steamRegistryKey == null)
            {
                localMachineRegistry.Close();
                localMachineRegistry = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                steamRegistryKey = localMachineRegistry.OpenSubKey(@"SOFTWARE\Valve\Steam");
            }
            object registryEntry = steamRegistryKey.GetValue("InstallPath");
            if (registryEntry != null)
                steamInstallPath = (string)registryEntry;
            localMachineRegistry.Close();
            steamRegistryKey.Close();
            return Path.Combine(steamInstallPath, @"steamapps");
        }
        private string GetUPlayDirectory()
        {
            string uplayInstallPath = "";
            RegistryKey localMachineRegistry = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            RegistryKey uplayRegistryKey = localMachineRegistry.OpenSubKey(@"SOFTWARE\Ubisoft\Launcher");
            if (uplayRegistryKey == null)
            {
                localMachineRegistry.Close();
                localMachineRegistry = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                uplayRegistryKey = localMachineRegistry.OpenSubKey(@"SOFTWARE\Ubisoft\Launcher");
            }
            object registryEntry = uplayRegistryKey.GetValue("InstallDir");
            if (registryEntry != null)
                uplayInstallPath = (string)registryEntry;
            localMachineRegistry.Close();
            uplayRegistryKey.Close();
            return uplayInstallPath;
        }
        private string GetOriginDirectory()
        {
            string originInstallPath = "";
            RegistryKey localMachineRegistry = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            RegistryKey originRegistryKey = localMachineRegistry.OpenSubKey(@"SOFTWARE\Origin");
            if (originRegistryKey == null)
            {
                localMachineRegistry.Close();
                localMachineRegistry = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                originRegistryKey = localMachineRegistry.OpenSubKey(@"SOFTWARE\Origin");
            }
            object registryEntry = originRegistryKey.GetValue("ClientPath");
            if (registryEntry != null)
                originInstallPath = (string)registryEntry;
            localMachineRegistry.Close();
            originRegistryKey.Close();
            return originInstallPath;
        }
        private string GetGOGDirectory()
        {
            string gogInstallPath = "";
            RegistryKey localMachineRegistry = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            RegistryKey gogRegistryKey = localMachineRegistry.OpenSubKey(@"SOFTWARE\GOG.com\GalaxyClient\paths");
            if (gogRegistryKey == null)
            {
                localMachineRegistry.Close();
                localMachineRegistry = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                gogRegistryKey = localMachineRegistry.OpenSubKey(@"SOFTWARE\GOG.com\GalaxyClient\paths");
            }
            object registryEntry = gogRegistryKey.GetValue("client");
            if (registryEntry != null)
                gogInstallPath = (string)registryEntry;
            localMachineRegistry.Close();
            gogRegistryKey.Close();
            return gogInstallPath;
        }
        private string GetEpicDirectory()
        {
            string epicInstallPath = "";
            RegistryKey localMachineRegistry = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            RegistryKey epicRegistryKey = localMachineRegistry.OpenSubKey(@"SOFTWARE\Epic Games\EpicGamesLauncher");
            if (epicRegistryKey == null)
            {
                localMachineRegistry.Close();
                localMachineRegistry = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                epicRegistryKey = localMachineRegistry.OpenSubKey(@"SOFTWARE\Epic Games\EpicGamesLauncher");
            }
            object registryEntry = epicRegistryKey.GetValue("AppDataPath");
            if (registryEntry != null)
                epicInstallPath = (string)registryEntry;
            localMachineRegistry.Close();
            epicRegistryKey.Close();
            return epicInstallPath;
        }
        private string GetBlizzardDirectory()
        {
            string blizzardInstallPath = "";
            RegistryKey localMachineRegistry = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            RegistryKey blizzardRegistryKey = localMachineRegistry.OpenSubKey(@"SOFTWARE\Blizzard Entertainment\Battle.net\Capabilities");
            if (blizzardRegistryKey == null)
            {
                localMachineRegistry.Close();
                localMachineRegistry = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                blizzardRegistryKey = localMachineRegistry.OpenSubKey(@"SOFTWARE\Blizzard Entertainment\Battle.net\Capabilities");
            }
            object registryEntry = blizzardRegistryKey.GetValue("ApplicationIcon");
            if (registryEntry != null)
                blizzardInstallPath = ((string)registryEntry).Substring(0,((string)registryEntry).Length - 2);
            localMachineRegistry.Close();
            blizzardRegistryKey.Close();
            return blizzardInstallPath;
        }
        private string GetBethesdaDirectory()
        {
            string bethesdaInstallPath = "";
            RegistryKey localMachineRegistry = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            RegistryKey bethesdaRegistryKey = localMachineRegistry.OpenSubKey(@"SOFTWARE\Bethesda Softworks\Bethesda.net");
            if (bethesdaRegistryKey == null)
            {
                localMachineRegistry.Close();
                localMachineRegistry = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                bethesdaRegistryKey = localMachineRegistry.OpenSubKey(@"SOFTWARE\Bethesda Softworks\Bethesda.net");
            }
            object registryEntry = bethesdaRegistryKey.GetValue("installLocation");
            if (registryEntry != null)
                bethesdaInstallPath = (string)registryEntry;
            localMachineRegistry.Close();
            bethesdaRegistryKey.Close();
            return bethesdaInstallPath;
        }
        private string[] GetSteamGameDirectoryList()
        {
            // For the sake of simplicity (since it can only be done manually these days), we will assume that the user only has one Steam library folder, and that it is not external. This means no reading from libraryfolders.vdf
            string steamLibraryPath = Path.Combine(GetSteamDirectory(), @"common");
            if (Directory.Exists(steamLibraryPath))
                return Directory.GetDirectories(steamLibraryPath);
            return null;
        }
        private string[] GetUPlayGameDirectoryList()
        {
            RegistryKey localMachineRegistry = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            RegistryKey uplayRegistryKey = localMachineRegistry.OpenSubKey(@"SOFTWARE\Ubisoft\Launcher\Installs");
            if (uplayRegistryKey == null)
            {
                localMachineRegistry.Close();
                localMachineRegistry = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                uplayRegistryKey = localMachineRegistry.OpenSubKey(@"SOFTWARE\Ubisoft\Launcher\Installs");
            }
            List<string> directoryList = new List<string>();
            foreach(string subKeyName in uplayRegistryKey.GetSubKeyNames())
            {
                RegistryKey tempKey = uplayRegistryKey.OpenSubKey(subKeyName);
                directoryList.Add((string)tempKey.GetValue("InstallDir"));
                tempKey.Close();
            }
            localMachineRegistry.Close();
            uplayRegistryKey.Close();
            return directoryList.ToArray();
        }

        private string[] GetSteamGameList()
        {
            string steamAppPath = GetSteamDirectory();
            List<string> gameList = new List<string>();
            foreach(string manifest in Directory.EnumerateFiles(steamAppPath, "*.acf"))
            {
                foreach(string line in File.ReadLines(manifest))
                {
                    if(line.Contains("name"))
                    {
                        int quoteCount = 0;
                        int i = 0;
                        for(i = 0; i < line.Length; ++i)
                        {
                            if(line[i] == '\"')
                                quoteCount++;
                            if (quoteCount > 2)
                                break;
                        }
                        if(i < line.Length)
                            gameList.Add(line.Substring(i + 1, line.Length - i - 2)); // we chop off the ends to avoid the quotes
                    }
                }
            }
            return gameList.ToArray();
        }
        private string[] GetUplayGameList()
        {
            List<string> gameList = new List<string>();
            foreach (string path in GetUPlayGameDirectoryList())
            {
                DirectoryInfo di = new DirectoryInfo(path);
                gameList.Add(di.Name);
            }
            return gameList.ToArray();
        }
        private List<Tuple<String, String>> GetOriginGameList()
        {
            var originGamesList = new List<Tuple<String, String>>();
            foreach(string directory in Directory.EnumerateDirectories(@"C:\ProgramData\Origin\LocalContent"))
            {
                string path = Path.Combine(@"C:\ProgramData\Origin\LocalContent", directory);
                foreach(string file in Directory.EnumerateFiles(path,"*.mfst"))
                {
                    FileInfo fi = new FileInfo(file);
                    DirectoryInfo di = new DirectoryInfo(directory);
                    originGamesList.Add(Tuple.Create(di.Name, Path.GetFileNameWithoutExtension(file)));
                }
            }
            return originGamesList;
        }
        private List<Tuple<String, String>> GetGOGGameList()
        {
            var gogGamesList = new List<Tuple<String, String>>();
            RegistryKey localMachineRegistry = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            RegistryKey gogRegistryKey = localMachineRegistry.OpenSubKey(@"SOFTWARE\GOG.com\Games");
            if (gogRegistryKey == null)
            {
                localMachineRegistry.Close();
                localMachineRegistry = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                gogRegistryKey = localMachineRegistry.OpenSubKey(@"SOFTWARE\GOG.com\Games");
            }
            if (gogRegistryKey != null)
            {
                foreach (string subKeyName in gogRegistryKey.GetSubKeyNames())
                {
                    RegistryKey tempKey = gogRegistryKey.OpenSubKey(subKeyName);
                    gogGamesList.Add(Tuple.Create(subKeyName, (string)tempKey.GetValue("gameName")));
                    tempKey.Close();
                }

            }
            localMachineRegistry.Close();
            gogRegistryKey.Close();
            return gogGamesList;
        }

        private List<Tuple<String, String>> GetEpicGameList()
        {
            var epicGamesList = new List<Tuple<String, String>>();
            foreach (string manifest in Directory.EnumerateFiles(Path.Combine(GetEpicDirectory(), @"Manifests"), "*.item"))
            {
                string appname = "", displayname = "";
                foreach (string line in File.ReadLines(manifest))
                {
                    if (line.Contains("AppName"))
                    {
                        int quoteCount = 0;
                        int i = 0;
                        for (i = 0; i < line.Length; ++i)
                        {
                            if (line[i] == '\"')
                                quoteCount++;
                            if (quoteCount > 2)
                                break;
                        }
                        if (i < line.Length)
                            appname = line.Substring(i + 1, line.Length - i - 2); // we chop off the ends to avoid the quotes
                    }
                    if (line.Contains("DisplayName"))
                    {
                        int quoteCount = 0;
                        int i = 0;
                        for (i = 0; i < line.Length; ++i)
                        {
                            if (line[i] == '\"')
                                quoteCount++;
                            if (quoteCount > 2)
                                break;
                        }
                        if (i < line.Length)
                            displayname =line.Substring(i + 1, line.Length - i - 2); // we chop off the ends to avoid the quotes
                    }
                }
                epicGamesList.Add(Tuple.Create(appname, displayname));
            }
            return epicGamesList;
        }
        private string[] GetBlizzardGameList()
        {
            RegistryKey currentUserRegistryKey = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64) : RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32);
            RegistryKey blizzardRegistryKey = currentUserRegistryKey.OpenSubKey(@"Software\Blizzard Entertainment");
            if (blizzardRegistryKey == null)
            {
                currentUserRegistryKey.Close();
                currentUserRegistryKey = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32) : RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
                blizzardRegistryKey = currentUserRegistryKey.OpenSubKey(@"Software\Blizzard Entertainment");
            }
            List<string> gameList = new List<string>();
            foreach (string subKeyName in blizzardRegistryKey.GetSubKeyNames())
            {
                gameList.Add(subKeyName);
            }
            currentUserRegistryKey.Close();
            blizzardRegistryKey.Close();
            return gameList.ToArray();
        }
        private string[] GetBethesdaGameList()
        {
            List<string> gameList = new List<string>();
            foreach (string path in Directory.GetDirectories(Path.Combine(GetBethesdaDirectory(),@"games")))
            {
                DirectoryInfo di = new DirectoryInfo(path);
                gameList.Add(di.Name);
            }
            return gameList.ToArray();
        }
        private string[] GetSteamInstallDirectoryList()
        {
            string steamAppPath = GetSteamDirectory();
            List<string> gameList = new List<string>();
            foreach (string manifest in Directory.EnumerateFiles(steamAppPath, "*.acf"))
            {
                foreach (string line in File.ReadLines(manifest))
                {
                    if (line.Contains("installdir"))
                    {
                        int quoteCount = 0;
                        int i;
                        for (i = 0; i < line.Length; ++i)
                        {
                            if (line[i] == '\"')
                                quoteCount++;
                            if (quoteCount > 2)
                                break;
                        }
                        if (i < line.Length)
                            gameList.Add(line.Substring(i + 1, line.Length - i - 2)); // we chop off the ends to avoid the quotes
                    }
                }
            }
            return gameList.ToArray();
        }
        private string[] GetSteamAppIDList()
        {
            string steamAppPath = GetSteamDirectory();
            List<string> gameList = new List<string>();
            foreach (string manifest in Directory.EnumerateFiles(steamAppPath, "*.acf"))
            {
                foreach (string line in File.ReadLines(manifest))
                {
                    if (line.Contains("appid"))
                    {
                        int quoteCount = 0;
                        int i;
                        for (i = 0; i < line.Length; ++i)
                        {
                            if (line[i] == '\"')
                                quoteCount++;
                            if (quoteCount > 2)
                                break;
                        }
                        if (i < line.Length)
                            gameList.Add(line.Substring(i + 1, line.Length - i - 2)); // we chop off the ends to avoid the quotes
                    }
                }
            }
            return gameList.ToArray();
        }
        private string[] GetUPlayAppIDList()
        {
            RegistryKey localMachineRegistry = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            RegistryKey uplayRegistryKey = localMachineRegistry.OpenSubKey(@"SOFTWARE\Ubisoft\Launcher\Installs");
            if (uplayRegistryKey == null)
            {
                localMachineRegistry.Close();
                localMachineRegistry = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                uplayRegistryKey = localMachineRegistry.OpenSubKey(@"SOFTWARE\Ubisoft\Launcher\Installs");
            }
            List<string> appIDList = new List<string>();
            foreach (string subKeyName in uplayRegistryKey.GetSubKeyNames())
            {
                appIDList.Add(subKeyName);
            }
            localMachineRegistry.Close();
            uplayRegistryKey.Close();
            return appIDList.ToArray();
        }
    }
}
