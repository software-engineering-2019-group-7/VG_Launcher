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
            List<Game> games = GetSteamGameList();

            //This is how we will do it when we aren't presenting
            //((MainWindow)Application.Current.MainWindow).CreateButtons(games);



            //This is for the presentation. Basically all this is is the Create Buttons function running on the installed games
            foreach (Game game in games)
            {
                if (game.name != "Steamworks Common Redistributables")
                {

                    Button btn = new Button();
                    btn.Name = "button" + ((MainWindow)Application.Current.MainWindow).gameWrapPanel.Children.Count; //replace this with an identitier ie: game.id
                    //btn.Content = game.name;
                    btn.Tag = game;
                    if (!File.Exists("../../Resources/" + ((MainWindow)Application.Current.MainWindow).CleanName(game.name).ToLower() + ".png"))
                    {
                        Console.WriteLine(game.name);
                        WebClient wc = new WebClient();
                        var json = wc.DownloadString("https://www.steamgriddb.com/api/v2/search/autocomplete/" + game.name);

                        //Choose the first game in the list. The first one most closely matches the name
                        dynamic idJson = JsonConvert.DeserializeObject(json);
                        dynamic firstGameInArray = idJson["data"][0];
                        string gameId = firstGameInArray.id;

                        json = wc.DownloadString("https://www.steamgriddb.com/api/v2/grids/game/" + gameId);
                        dynamic imageJson = JsonConvert.DeserializeObject(json);

                        //Choose the first image in the list. We can obviously choose an image based on its properties.
                        //For instance, we could check::::  imageJson["data"][0]["style"] == "blurred"
                        //and if thats not true we could go down the image list
                        string imageUrl = imageJson["data"][0]["url"];
                        game.image = imageUrl;



                        //As of right now, we do nothing with this downloaded file. I havent been able to get the "ImageSource" further down to actually see the downloaded file
                        //But I am storing it just in case we can figure out how to use it

                        Console.WriteLine("Pulled image " + game.name);
                        wc.DownloadFile(imageUrl, "../../Resources/" + ((MainWindow)Application.Current.MainWindow).CleanName(game.name).ToLower() + ".png");
                    }
                    if (File.Exists("../../Resources/" + ((MainWindow)Application.Current.MainWindow).CleanName(game.name).ToLower() + ".png"))
                    {

                        ImageBrush myBrush = new ImageBrush();
                        myBrush.ImageSource = new BitmapImage(new Uri("../../Resources/" + ((MainWindow)Application.Current.MainWindow).CleanName(game.name).ToLower() + ".png", UriKind.Relative));
                        btn.Background = myBrush;
                    }
                    //Static values. All buttons should have the same values for these.
                    btn.Width = 360;
                    btn.Height = 160;
                    btn.Margin = new Thickness(8);
                    btn.HorizontalContentAlignment = HorizontalAlignment.Center;
                    btn.VerticalContentAlignment = VerticalAlignment.Bottom;
                    btn.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CFFFFFF"));
                    btn.FontSize = 48;
                    btn.FontWeight = FontWeights.SemiBold;
                    btn.Style = Resources["noHighlightButton"] as Style;

                    //This lets us click the button. All of the buttons will share a function called Button_Click so we will have to be creative.
                    //Theres no way we can create a method for each new button, at least not that I know of. 
                    btn.Click += ((MainWindow)Application.Current.MainWindow).Button_Click;

                    ((MainWindow)Application.Current.MainWindow).gameWrapPanel.Children.Add(btn);
                }
            }

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
        private string[] GetSteamGameDirectoryList()
        {
            // For the sake of simplicity (since it can only be done manually these days), we will assume that the user only has one Steam library folder, and that it is not external. This means no reading from libraryfolders.vdf
            string steamLibraryPath = Path.Combine(GetSteamDirectory(), @"common");
            if (Directory.Exists(steamLibraryPath))
                return Directory.GetDirectories(steamLibraryPath);
            return null;
        }

        private List<Game> GetSteamGameList()
        {
            string steamAppPath = GetSteamDirectory();
            List<Game> gameList = new List<Game>();
            foreach (string manifest in Directory.EnumerateFiles(steamAppPath, "*.acf"))
            {
                foreach (string line in File.ReadLines(manifest))
                {
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
                        {
                            Game g = new Game();
                            g.name = line.Substring(i + 1, line.Length - i - 2);
                            gameList.Add(g); // we chop off the ends to avoid the quotes
                        }
                    }
                }
            }
            return gameList;
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
    }
}
