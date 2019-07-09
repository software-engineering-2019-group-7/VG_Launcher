using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
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
            //These are totally crowdsourced, so YMMV. I asked the bethesda discord to help me with the name/id pairs
            { "Fallout 76", "20" },
            { "Fallout Shelter", "8" },
            { "Rage", "45" },
            { "Quake Champions", "11" },
            { "Quake Champions PTS", "12" },
            { "Fallout", "21" },
            { "Fallout 2", "22" },
            { "The Elder Scrolls: Legends", "5" },
            { "", "" }
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
                try
                {
                    added.AddRange(CreateSteamGamesList(getSteamNameId()));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine("Something went wrong with adding Steam games.");
                }
            }
            if (Origin.IsChecked == true)
            {
                try
                {
                    added.AddRange(GetOriginGameList());
            }
                catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Something went wrong with adding Origin games.");
            }

        }
            if (Uplay.IsChecked == true)
            {
                try
                {
                    added.AddRange(GetUplayGameList());
                            }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine("Something went wrong with adding Uplay games.");
                }

            }
            if (GOG.IsChecked == true)
            {
                try
                {
                    added.AddRange(GetGOGGameList());
                    }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine("Something went wrong with adding GOG games.");
                }

            }
            if (Bethesda.IsChecked == true)
            {
                try
                {
                    added.AddRange(GetBethesdaGameList());
                    }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine("Something went wrong with adding Bethesda games.");
                }

            }
            if (Epic.IsChecked == true)
            {
                try
                {
                    added.AddRange(GetEpicGameList());
                    }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine("Something went wrong with adding Epic games.");
                }

            }
            if (Blizzard.IsChecked == true)
            {
                try
                {
                    added.AddRange(GetBlizzardGameList());
                    }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine("Something went wrong with adding Blizzard games.");
                }

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
                g.provider = "Steam";
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
            //Console.WriteLine(pairs.Count);
            foreach (KeyValuePair<string,string> p in pairs)
            {
                Game g = new Game();
                g.name = p.Key;
                g.path = "uplay://launch/"+p.Value+"/0";
                g.parentLock = "0";
                g.provider = "uPlay";
                gameList.Add(g);
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

        #region GOG
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
                    game.path = GetGOGDirectory() + "\\GalaxyClient.exe";
                    game.settings = "/command=runGame /gameId=" + subKeyName + " /path=" + (string)tempKey.GetValue("path"); //maybe?
                    game.parentLock = "0";
                    game.provider = "GOG";
                    gogGamesList.Add(game);
                    tempKey.Close();
                }

            }
            localMachineRegistry.Close();
            gogRegistryKey.Close();
            return gogGamesList;
        }
        #endregion

        #region Origin
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

            foreach (string directory in Directory.EnumerateDirectories(@"C:\ProgramData\Origin\LocalContent"))
            {
                string path = Path.Combine(@"C:\ProgramData\Origin\LocalContent", directory);
                foreach (string file in Directory.EnumerateFiles(path, "*.mfst"))
                {
                    if (file.Contains("OFB") || file.Contains("OFR"))
                    {
                        FileInfo fi = new FileInfo(file);
                        DirectoryInfo di = new DirectoryInfo(directory);
                        string name = di.Name;
                        string id = Path.GetFileNameWithoutExtension(file);

                        name = Regex.Replace(name, "[0-9]", " $0").Trim();

                        if (id.Contains("OFB"))
                        {
                            char[] dirtyName = id.ToCharArray();
                            int spaceIndex = 0;
                            for (int i = 1; i < dirtyName.Length; i++)
                            {
                                if (Char.IsDigit(dirtyName[i]))
                                {
                                    spaceIndex = i;
                                    if (Char.IsDigit(dirtyName[i + 1]))
                                    {
                                        break;
                                    }
                                }
                            }
                            if (spaceIndex != 0)
                                id = id.Insert(spaceIndex, ":");
                        }

                        var game = new Game();
                        game.name = name;
                        game.path = "origin://launchgame/" + id;
                        game.parentLock = "0";
                        game.provider = "Origin";
                        if (!originGamesList.Any(i=>i.name == name))
                            originGamesList.Add(game);
                    }
                }
            }

            return originGamesList;
        }
        #endregion

        #region Epic
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
                    //Console.WriteLine(line);

                    if (line.Contains("AppName") && !line.Contains("Full") && !line.Contains("Main"))
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
                            appname = line.Substring(i + 1, line.Length - i - 3); // we chop off the ends to avoid the quotes
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
                        {
                            displayname = line.Substring(i + 1, line.Length - i - 3); // we chop off the ends to avoid the quotes
                        }
                    }
                }
                game.path = "com.epicgames.launcher://apps/" + appname + "?action=launch&silent=true";
                game.name = displayname;

                //Console.WriteLine("appname: " + appname);
                //Console.WriteLine("display: " + displayname);

                game.parentLock = "0";
                game.provider = "Epic";
                epicGamesList.Add(game);
            }
            return epicGamesList;
        }
        #endregion

        #region Blizzard
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
                //Console.WriteLine(subKeyName);
                //Console.WriteLine(GetBlizzardDirectory());
                var game = new Game();
                foreach (var name in blizzardIDName.Keys)
                {
                    if (subKeyName.Equals(name))
                    {
                        game.name = name;
                        game.path = GetBlizzardDirectory();
                        game.settings= "--exec=\"launch " + blizzardIDName[subKeyName] + "\""; //maybe?
                        game.parentLock = "0";
                        game.provider = "Blizzard";
                        gameList.Add(game);
                    }
                }
            }
            currentUserRegistryKey.Close();
            blizzardRegistryKey.Close();
            return gameList;
        }
        #endregion

        #region Bethesda
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
        private List<Game> GetBethesdaGameList()
        {
            List<Game> gameList = new List<Game>();
            foreach (string path in Directory.GetDirectories(Path.Combine(GetBethesdaDirectory(), @"games")))
            {
                Console.WriteLine(path);
                var game = new Game();
                DirectoryInfo di = new DirectoryInfo(path);
                string gameName = di.Name;

                ////bethesda convieniently returns the names of the games with no spaces, which screws up the image gathering. Adding spaces between words here
                int spaceIndex = 0;
                char[] dirtyName = gameName.ToCharArray();
                for (int i = 1; i < dirtyName.Length; i++)
                {
                    if (Char.IsUpper(dirtyName[i]) || Char.IsDigit(dirtyName[i]))
                    {
                        spaceIndex = i;
                        if (Char.IsDigit(dirtyName[i + 1]))
                        {
                            break;
                        }
                    }
                }
                if (spaceIndex != 0)
                    gameName = gameName.Insert(spaceIndex, " ");

                Console.WriteLine(gameName);
                foreach (var name in bethesdaIDName.Keys)
                {
                    if (gameName.Equals(name))
                    {
                        game.name = gameName;
                        game.path = "bethesdanet://run/" + bethesdaIDName[gameName];
                        game.parentLock = "0";
                        game.provider = "Bethesda";
                        gameList.Add(game);
                    }
                }
            }
            return gameList;
        }
        #endregion
    }
}
