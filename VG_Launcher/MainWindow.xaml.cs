using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                ///This section deals with the background picture. We will have to change this for sure if we are getting them from the internet. 
                ///I suppose we will have to download the images regardless, so maybe we will have the actual images at this point? 
                ///Steam and others load a default background and replace it when they have the actual image (maybe do this on a different thread?)
                Uri resourceUri = new Uri("Resources/" + game.image , UriKind.Relative);
                StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);
                BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
                var brush = new ImageBrush();
                brush.ImageSource = temp;
                btn.Background = brush;


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

        private void ServiceLoader_Click(object sender, RoutedEventArgs e)
        {
            ServiceProvider sp = new ServiceProvider();
            sp.ShowDialog();
        }

        private void Addbtns_Click(object sender, RoutedEventArgs e)
        {
            List<Game> games = new List<Game>();
            //games.Add(new Game("Path of Exile", "pathofexile.exe", "pathofexile.png"));
            //games.Add(new Game("Grand Theft Auto V", "GTAV.exe", "gtav.png"));
            //games.Add(new Game("Terraria", "terraria.exe", "terraria.png"));
            //games.Add(new Game("Risk of Rain", "riskofrain.exe", "riskofrain.png"));
            CreateButtons(games);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button; //lets us edit the button that sent the function call


            GameScreen gs = new GameScreen();
            Game game = (Game) btn.Tag;
            gs.Name = "gs";
            gs.gameName.Content = btn.Content;
            gs.image.Source = new BitmapImage(new Uri("Resources/"+game.image, UriKind.Relative));
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
