using Microsoft.Win32;
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
                //in progress
            }
            if (Uplay.IsChecked == true)
            {
                added.AddRange(GetUplayGameList());
            }
            if (GOG.IsChecked == true)
            {
                //in progress
            }
            if (Bethesda.IsChecked == true)
            {
                //in progress
            }
            if (Epic.IsChecked == true)
            {
                //in progress
            }
            if (Blizzard.IsChecked == true)
            {
                //in progress
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
    }
}
