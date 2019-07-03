using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using Point = System.Windows.Point;

namespace VG_Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public Library Curlibrary { get; private set; }
        public bool locked;

        public MainWindow()
        {
            InitializeComponent();
            Curlibrary = new Library();
            Curlibrary.InitLib();
            if (Curlibrary.gameList.Count < 1)
            {
                //Empty game Library, launch account setup
                AccountSetup accountSetup = new AccountSetup(Curlibrary);
                Console.WriteLine("NO GAMES, INIT CONDITIONS");

                App.Current.MainWindow.Hide();
                accountSetup.Show();
                //Continue with setup procedures
            }
            else
            {
                if (Properties.Settings.Default.ChildEnabled)
                {
                    //Go to Login Screen
                    logIn();
                    locked = Properties.Settings.Default.ParentalLockEngaged;
                    if (locked)
                        CreateButtons(true);
                    else
                        CreateButtons(false);
                }
                else
                {
                    CreateButtons(false);
                }
            }            
        }

        public void CreateButtons(bool locked)
        {
            gameWrapPanel.Children.Clear();
            foreach (Game game in Curlibrary.gameList)
            {
                if (!(Int32.Parse(game.parentLock) == 1 && locked == true))
                {
                    try
                    {

                        Button btn = new Button();
                        btn.Name = "button" + gameWrapPanel.Children.Count; //replace this with an identitier ie: game.id
                        btn.Tag = game;
                        if (!File.Exists(game.image))
                        {
                            Console.WriteLine(game.name);
                            WebClient wc = new WebClient();

                            wc.Headers.Add("Authorization", "Bearer 47af29a9fb8d5d08ba57a06f2bc15261");

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
                            try
                            {
                                string imageUrl = imageJson["data"][0]["url"];
                                game.image = "../../Resources/" + CleanName(game.name).ToLower() + ".png";
                                Console.WriteLine(game.name);


                                //As of right now, we do nothing with this downloaded file. I havent been able to get the "ImageSource" further down to actually see the downloaded file
                                //But I am storing it just in case we can figure out how to use it

                                Console.WriteLine("Pulled image " + game.name);
                                wc.Headers.Clear();
                                wc.DownloadFile(imageUrl, "../../Resources/" + CleanName(game.name).ToLower() + ".png");
                            }
                            catch(Exception e)
                            {
                                game.image = "../../Resources/DefaultGameLogo.jpg";
                            }
                         
                            ImageBrush myBrush = new ImageBrush();
                            myBrush.ImageSource = new BitmapImage(new Uri(game.image, UriKind.Relative));
                            btn.Background = myBrush;
                        }
                        else if (File.Exists(game.image))
                        {
                            ImageBrush myBrush = new ImageBrush();
                            myBrush.ImageSource = new BitmapImage(new Uri(game.image, UriKind.Relative));
                            btn.Background = myBrush;
                        }
                        else
                        {
                            //Image wasn't found locally or in GridDB.. ask user to select a new image
                            //Right now this case is never reached.. it will probably have to be a catch to the Grid search
                            Console.WriteLine(game.name + "No Image found");
                        }
                        if(game.image == "../../Resources/DefaultGameLogo.jpg")
                        {
                            //If we are using the default Logo, display the name **NOT WORKING YET**
                            btn.Content = game.name;
                        }
                        //"#4CFFFFFF"
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
                        btn.Click += Button_Click;

                        gameWrapPanel.Children.Add(btn);

                    }
                    catch (Exception e)
                    {
                        //Let the user choose their own image here
                        Console.WriteLine(e);
                    }
                }
            }
        }

        public string CleanName(string str)
        {
            str = str.Replace(" ", "_");
            str = str.Replace(":", "");
            str = str.Replace(",", "");
            return str;
        }

        public void logIn()
        {
            LogInService li = new LogInService();
            li.ShowDialog();
        }
       
        public void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button; //lets us edit the button that sent the function call


            GameScreen gs = new GameScreen();
            Game game = (Game)btn.Tag;
            gs.Tag = game;
            gs.playButton.Tag = game;
            gs.settingsButton.Tag = game;



            gs.Name = "gs";
            gs.gameName.Content = game.name;

            //Setting up the background image
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(game.image, UriKind.Relative);
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            gs.image.Source = bitmapImage;

            var graphics = Graphics.FromHwnd(IntPtr.Zero);
            var scaleWidth = (int)(graphics.DpiX / 96);
            var scaleHeight = (int)(graphics.DpiY / 96);
            //location of gamescreen
            Point point = btn.PointToScreen(new Point(0, 0));
            if ((point.X + gs.Width) > (mainWindow.Left + mainWindow.Width))
            {
                gs.Left = ((point.X - (gs.Width - btn.Width))/(scaleWidth));
                gs.Top = ((point.Y + btn.Height) / (scaleHeight))+2;
            }
            else if (point.X - 90 < mainWindow.Left)
            {
                gs.Left = (point.X / (scaleWidth));
                gs.Top = ((point.Y + btn.Height) / (scaleHeight))+2;
            }
            else
            {
                gs.Left = ((point.X - 90) / (scaleWidth));
                gs.Top = ((point.Y + btn.Height) / (scaleHeight))+2;
            }
            //this will keep the gamescreens from going off the bottom of the window
            if ((point.Y + gs.Height + btn.Height ) > (mainWindow.Top + mainWindow.Height))
            {
                gs.Top = ((point.Y - gs.Height) / (scaleHeight))-2;
            }
            gs.Show();
            clickReciever.Visibility = Visibility.Visible;
        }

        private void ClickReciever_MouseDown(object sender, MouseButtonEventArgs e)
        {
            foreach (Window w in App.Current.Windows)
            {
                if (w.Name.Equals("gs"))
                {
                    w.Close();
                }
            }
            clickReciever.Visibility = Visibility.Hidden;
            
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            Curlibrary.SaveJson(Curlibrary);
            System.Windows.Application.Current.Shutdown();
        }

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            foreach (Window w in App.Current.Windows)
            {
                if (w.Name.Equals("gs"))
                {
                    w.Close();
                }
            }
            clickReciever.Visibility = Visibility.Hidden;
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (Window w in App.Current.Windows)
            {
                if (w.Name.Equals("gs"))
                {
                    w.Close();
                }
            }
            clickReciever.Visibility = Visibility.Hidden;
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void GameScroller_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void GameWrapPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button; 
            Point point = btn.PointToScreen(new Point(0, 0));
            
            MenuScreen ms = new MenuScreen(Curlibrary);
            ms.Top = point.Y + btn.Height;
            ms.Left = point.X + btn.Width;
            ms.Show();
        }
    }
}