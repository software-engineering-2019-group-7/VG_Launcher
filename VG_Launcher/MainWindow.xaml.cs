using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        }

        void CreateButtons(List<Game> list)
        {
            Curlibrary = new Library();
            Curlibrary.InitLib();
            foreach (Game game in Curlibrary.gameList)
            {

                Button btn = new Button();
                btn.Name = "button" + gameWrapPanel.Children.Count; //replace this with an identitier ie: game.id
                btn.Content = game.name;
                btn.Tag = game;

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
                wc.DownloadFile(imageUrl, "../../Resources/" + CleanName(game.name).ToLower() + ".png");




                ImageBrush myBrush = new ImageBrush();

                //This is able to pull the image straight from the url, but it would be great to use the downloaded image.
                myBrush.ImageSource = new BitmapImage(new Uri(imageUrl, UriKind.Absolute));
                btn.Background = myBrush;




                //Static values. All buttons should have the same values for these.
                btn.Width = 360;
                btn.Height = 160;
                btn.Margin = new Thickness(10);
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

        private string CleanName(string str)
        {
            str = str.Replace(" ", "_");
            return str;
        }

        private void ServiceLoader_Click(object sender, RoutedEventArgs e)
        {
            ServiceProvider sp = new ServiceProvider();
            sp.ShowDialog();
        }


        private void Addbtns_Click(object sender, RoutedEventArgs e)
        {
            List<Game> games = new List<Game>();
            CreateButtons(games);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button; //lets us edit the button that sent the function call


            GameScreen gs = new GameScreen();
            Game game = (Game)btn.Tag;
            gs.Name = "gs";
            gs.gameName.Content = btn.Content;
            gs.image.Source = new BitmapImage(new Uri(game.image, UriKind.Absolute));
            //this is where we would link the game the button is related to to the gameScreen


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
    }
}



//Game image api

//https://www.steamgriddb.com/api/v2/search/autocomplete/{term}
//Where term is game name
// returns an array
// array[0].id is game id we want


//https://www.steamgriddb.com/api/v2/grids/game/{gameId}
//Where gameId is array[0].id
//returns an array of images
//  - we want an image with the "blurred" style tag. 
//  - so if image[i].sytle == "blurred"
//  - use this image, it is stored at url image[i].url




