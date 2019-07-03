﻿using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace VG_Launcher
{
    /// <summary>
    /// Interaction logic for ServiceProvider.xaml
    /// </summary>
    public partial class ServiceProvider : Window
    {
        public Library library = new Library();
        public Dictionary<string, string> blizzardIDName = new Dictionary<string, string>
        {
            //Game name, ID
            { "World of Warcraft", "WoW" },
            { "Destiny 2", "DST2" },
            { "Call of Duty: Black Ops 4", "VIPR" },
            { "Diablo 3", "D3" },
            { "Starcraft Remastered", "S1" },
            { "Starcraft 2", "S2" },
            { "Hearthstone", "WTCG" },
            { "Heroes of the Storm", "Hero" },
            { "Overwatch", "Pro" }
        };
        public Dictionary<string, string> bethesdaIDName = new Dictionary<string, string>
        {
            //Game name, ID
            { "World of Warcraft", "WoW" },
            { "Destiny 2", "DST2" },
            { "Call of Duty: Black Ops 4", "VIPR" },
            { "Diablo 3", "D3" },
            { "Starcraft Remastered", "S1" },
            { "Starcraft 2", "S2" },
            { "Hearthstone", "WTCG" },
            { "Heroes of the Storm", "Hero" },
            { "Overwatch", "Pro" }
        };

        public ServiceProvider()
        {
            InitializeComponent();
        }

        //Feed in the library that will be added to and set this forms temp library to it
        public ServiceProvider(Library lib)
        {
            InitializeComponent();
            library = lib;
        }

        public void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            List<Game> added = new List<Game>();
            if (Steam.IsChecked == true)
            {
                added.AddRange(CreateSteamGamesList(getSteamNameId()));
            }
            if (Origin.IsChecked == true)
            {
                added.AddRange(GetOriginGameList());
            }
            if (Uplay.IsChecked == true)
            {
                added.AddRange(GetUplayGameList());
            }
            if (GOG.IsChecked == true)
            {
                added.AddRange(GetGOGGameList());
            }
            if (Bethesda.IsChecked == true)
            {
                //in progress
            }
            if (Epic.IsChecked == true)
            {
                added.AddRange(GetEpicGameList());
            }
            if (Blizzard.IsChecked == true)
            {
                GetBlizzardGameList();
            }

            //Add the games that the scraper found to the library's gamelist
            foreach (Game g in added)
            {
                bool contains = false;
                foreach (Game temp in library.gameList)
                {
                    if (temp.path == g.path)
                        contains = true;
                }
                if (!contains)
                {
                    library.gameList.Add(g);
                }
            }

            ((MainWindow)Application.Current.MainWindow).CreateButtons(Properties.Settings.Default.ParentalLockEngaged);

            this.Close();
        }


        #region Steam
        public List<Game> CreateSteamGamesList(Dictionary<string,string> pairs)
        {
            List<Game> games = new List<Game>();

            //for each of the games the scraper found, create a new game object
            //We will also add the exe path here when we get that figured out
            foreach(KeyValuePair<string,string> game in pairs)
            {
                Game g = new Game();
                g.name = game.Key;
                g.path = "steam://rungameid/" + game.Value;
                g.parentLock = "0";
                bool containsGame = false;


                foreach(Game gm in library.gameList)
                {
                    if (gm.name == game.Key)
                        containsGame = true;
                }
                if (!containsGame)
                    games.Add(g);

                
            }
            return games;
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

        private Dictionary<string, string> getSteamNameId()
        {
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            string steamAppPath = GetSteamDirectory();
            foreach (string manifest in Directory.EnumerateFiles(steamAppPath, "*.acf"))
            {
                string name = "";
                string id = "";
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
                            id = line.Substring(i + 1, line.Length - i - 2); // we chop off the ends to avoid the quotes
                    }
                    if (line.Contains("name"))
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
                            name = line.Substring(i + 1, line.Length - i - 2);// we chop off the ends to avoid the quotes
                    }
                }
                if (!pairs.ContainsKey(name))
                    pairs.Add(name, id);
            }
            return pairs;
        }
        #endregion

        #region uPlay
        private List<Game> GetUplayGameList()
        {
            List<Game> gameList = new List<Game>();
            Dictionary<string, string> pairs = GetUPlayAppIDNameList();
            Console.WriteLine(pairs.Count);
            foreach (KeyValuePair<string,string> p in pairs)
            {
                Game g = new Game();
                g.name = p.Key;
                g.path = "uplay://launch/"+p.Value+"/0";
                g.parentLock = "0";
                gameList.Add(g);
            }
            foreach(Game g in gameList)
            {
                Console.WriteLine("@@@@" + g.name + " " + g.path);
            }
            return gameList;
        }

        private Dictionary<string, string> GetUPlayAppIDNameList()
        {
            RegistryKey localMachineRegistry = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            RegistryKey uplayRegistryKey = localMachineRegistry.OpenSubKey(@"SOFTWARE\Ubisoft\Launcher\Installs");
            if (uplayRegistryKey == null)
            {
                localMachineRegistry.Close();
                localMachineRegistry = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                uplayRegistryKey = localMachineRegistry.OpenSubKey(@"SOFTWARE\Ubisoft\Launcher\Installs");
            }
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            List<string> appIDList = new List<string>();
            foreach (string subKeyName in uplayRegistryKey.GetSubKeyNames())
            {
                string gameName;
                string gameId;
                gameId = subKeyName;
                RegistryKey tempKey = uplayRegistryKey.OpenSubKey(subKeyName);
                DirectoryInfo di = new DirectoryInfo((string)tempKey.GetValue("InstallDir"));
                gameName = di.Name;
                pairs.Add(gameName, gameId);
                tempKey.Close();
            }
            localMachineRegistry.Close();
            uplayRegistryKey.Close();
            return pairs;
        }
        #endregion

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
        private List<Game> GetGOGGameList()
        {
            var gogGamesList = new List<Game>();
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
                    var game = new Game();
                    game.name = (string)tempKey.GetValue("gameName");
                    game.path = GetGOGDirectory() + "/command=runGame /gameId=" + subKeyName + " /path=" + (string)tempKey.GetValue("path"); //maybe?
                    gogGamesList.Add(game);
                    tempKey.Close();
                }

            }
            localMachineRegistry.Close();
            gogRegistryKey.Close();
            return gogGamesList;
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
        private List<Game> GetOriginGameList()
        {
            var originGamesList = new List<Game>();
            RegistryKey localMachineRegistry = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            RegistryKey originRegistryKey = localMachineRegistry.OpenSubKey(@"SOFTWARE\Origin Games");
            if (originRegistryKey == null)
            {
                localMachineRegistry.Close();
                localMachineRegistry = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32) : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                originRegistryKey = localMachineRegistry.OpenSubKey(@"SOFTWARE\Origin Games");
            }
            if (originRegistryKey != null)
            {
                foreach (string subKeyName in originRegistryKey.GetSubKeyNames())
                {
                    var game = new Game();
                    RegistryKey tempKey = originRegistryKey.OpenSubKey(subKeyName);
                    game.name = (string)tempKey.GetValue("DisplayName");
                    game.path = "origin://launchgame/OFB-EAST:" + subKeyName;
                    originGamesList.Add(game);
                    tempKey.Close();
                }

            }
            localMachineRegistry.Close();
            originRegistryKey.Close();
            return originGamesList;
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
        private List<Game> GetEpicGameList()
        {
            var epicGamesList = new List<Game>();
            foreach (string manifest in Directory.EnumerateFiles(Path.Combine(GetEpicDirectory(), @"Manifests"), "*.item"))
            {
                var game = new Game();
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
                            displayname = line.Substring(i + 1, line.Length - i - 2); // we chop off the ends to avoid the quotes
                    }
                }
                game.name = displayname;
                game.path = "com.epicgames.launcher://apps/" + appname + "?action=launch";
                epicGamesList.Add(game);
            }
            return epicGamesList;
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
                blizzardInstallPath = ((string)registryEntry).Substring(0, ((string)registryEntry).Length - 2);
            localMachineRegistry.Close();
            blizzardRegistryKey.Close();
            return blizzardInstallPath;
        }
        private List<Game> GetBlizzardGameList()
        {
            RegistryKey currentUserRegistryKey = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64) : RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32);
            RegistryKey blizzardRegistryKey = currentUserRegistryKey.OpenSubKey(@"Software\Blizzard Entertainment");
            if (blizzardRegistryKey == null)
            {
                currentUserRegistryKey.Close();
                currentUserRegistryKey = Environment.Is64BitOperatingSystem ? RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32) : RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
                blizzardRegistryKey = currentUserRegistryKey.OpenSubKey(@"Software\Blizzard Entertainment");
            }
            List<Game> gameList = new List<Game>();
            foreach (string subKeyName in blizzardRegistryKey.GetSubKeyNames())
            {
                var game = new Game();
                foreach (var name in blizzardIDName.Keys)
                {
                    if (subKeyName.Equals(name))
                    {
                        game.name = name;
                        game.path = GetBlizzardDirectory() + "--exec = \"launch " + blizzardIDName[name] + "\""; //maybe?
                    }
                }
                gameList.Add(game);
            }
            currentUserRegistryKey.Close();
            blizzardRegistryKey.Close();
            return gameList;
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
        private string[] GetBethesdaGameList()
        {
            List<string> gameList = new List<string>();
            foreach (string path in Directory.GetDirectories(Path.Combine(GetBethesdaDirectory(), @"games")))
            {
                DirectoryInfo di = new DirectoryInfo(path);
                gameList.Add(di.Name);
            }
            return gameList.ToArray();
        }
    }
}
