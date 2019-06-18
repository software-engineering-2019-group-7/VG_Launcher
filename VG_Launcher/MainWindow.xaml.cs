using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace VG_Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public Library Curlibrary { get; private set; }

        public MainWindow()
        {
            #region DPI Scaling fix (TEMPORARY) (Makes other DPIs look like trash)
            var setDpiHwnd = typeof(HwndTarget).GetField("_setDpi", BindingFlags.Static | BindingFlags.NonPublic);
            setDpiHwnd?.SetValue(null, false);

            var setProcessDpiAwareness = typeof(HwndTarget).GetProperty("ProcessDpiAwareness", BindingFlags.Static | BindingFlags.NonPublic);
            setProcessDpiAwareness?.SetValue(null, 1, null);

            var setDpi = typeof(UIElement).GetField("_setDpi", BindingFlags.Static | BindingFlags.NonPublic);

            setDpi?.SetValue(null, false);

            var setDpiXValues = (List<double>)typeof(UIElement).GetField("DpiScaleXValues", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null);

            setDpiXValues?.Insert(0, 1);

            var setDpiYValues = (List<double>)typeof(UIElement).GetField("DpiScaleYValues", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null);

            setDpiYValues?.Insert(0, 1);
            #endregion 

            InitializeComponent();
            Curlibrary = new Library();
            Curlibrary.InitLib();
        }


        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }



        public void CreateButtons()
        {
            foreach (Game game in Curlibrary.gameList)
            {
                if (game.name != "Steamworks Common Redistributables") //Will have to put in much safer safegaurds than this.
                {

                    Button btn = new Button();
                    btn.Name = "button" + gameWrapPanel.Children.Count; //replace this with an identitier ie: game.id
                    btn.Tag = game;
                    if (!File.Exists("../../Resources/" + CleanName(game.name).ToLower() + ".png"))
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
                        string imageUrl = imageJson["data"][0]["url"];
                        game.image = "../../Resources/" + CleanName(game.name).ToLower() + ".png";
                        Console.WriteLine(game.name);


                        //As of right now, we do nothing with this downloaded file. I havent been able to get the "ImageSource" further down to actually see the downloaded file
                        //But I am storing it just in case we can figure out how to use it

                        Console.WriteLine("Pulled image " + game.name);
                        wc.Headers.Clear();
                        wc.DownloadFile(imageUrl, "../../Resources/" + CleanName(game.name).ToLower() + ".png");

                    }
                    if (File.Exists("../../Resources/" + CleanName(game.name).ToLower() + ".png"))
                    {

                        ImageBrush myBrush = new ImageBrush();
                        myBrush.ImageSource = new BitmapImage(new Uri("../../Resources/" + CleanName(game.name).ToLower() + ".png", UriKind.Relative));
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
                    btn.Click += Button_Click;

                    gameWrapPanel.Children.Add(btn);

                }
            }
        }

        public string CleanName(string str)
        {
            str = str.Replace(" ", "_");
            return str;
        }

        private void ServiceLoader_Click(object sender, RoutedEventArgs e)
        {
            ServiceProvider sp = new ServiceProvider(Curlibrary);
            sp.ShowDialog();
            //Curlibrary = sp.library;
        }


        public void Addbtns_Click(object sender, RoutedEventArgs e)
        {
            List<Game> games = new List<Game>();
            CreateButtons();
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button; //lets us edit the button that sent the function call


            GameScreen gs = new GameScreen();
            Game game = (Game)btn.Tag;
            gs.Name = "gs";
            gs.gameName.Content = btn.Content;

            //Setting up the background image
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri("../../Resources/" + CleanName(game.name).ToLower() + ".png", UriKind.Relative);
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            gs.image.Source = bitmapImage;


            //location of gamescreen
            Point point = btn.PointToScreen(new Point(0, 0));
            if ((point.X + gs.Width) > (mainWindow.Left + mainWindow.Width))
            {
                gs.Left = point.X - (gs.Width - btn.Width);
                gs.Top = point.Y + btn.Height + 2;
            }
            else if (point.X - 90 < mainWindow.Left)
            {
                gs.Left = point.X;
                gs.Top = point.Y + btn.Height + 2;
            }
            else
            {
                gs.Left = point.X - 90;
                gs.Top = point.Y + btn.Height + 2;
            }
            //this will keep the gamescreens from going off the bottom of the window
            if ((point.Y + gs.Height + btn.Height ) > (mainWindow.Top + mainWindow.Height))
            {
                gs.Top = point.Y - gs.Height - 2;
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
    }
}