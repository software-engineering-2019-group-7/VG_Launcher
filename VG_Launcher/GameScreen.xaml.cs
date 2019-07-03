using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VG_Launcher
{
    /// <summary>
    /// Interaction logic for GameScreen.xaml
    /// </summary>
    public partial class GameScreen : Window
    {

        public GameScreen()
        {
            InitializeComponent();
           if (Properties.Settings.Default.ParentalLockEngaged)
            {
                settingsButton.Visibility = Visibility.Hidden;
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button; //lets us edit the button that sent the function call 
            Game game = (Game)btn.Tag;

            SettingsMenu set = new SettingsMenu(game);
            set.Tag = game;
            set.saveButton.Tag = game;
            Point point = btn.PointToScreen(new Point(0, 0));
            set.Left = point.X;
            set.Top = point.Y;
            this.Close();
            set.Show();
            //open the settings menu
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (Process myProcess = new Process())
                {
                    Button btn = sender as Button; //lets us edit the button that sent the function call
                    Game game = (Game)btn.Tag;
                    myProcess.StartInfo.UseShellExecute = true;
                    myProcess.StartInfo.FileName = game.path;
                    myProcess.StartInfo.Arguments = game.settings;
                    myProcess.Start();
                }
            }catch(Exception exe)
            {
                Console.WriteLine(exe.Message);
            }
            Trace.WriteLine("Launched");//launch the exe
        }

        private void launchSteam (String appID, String launchOptions)
        {
            //use steam appID to run steam launch
            String steamLaunchURL = "steam://rungameid/";
            String commandCode = steamLaunchURL + appID;
            //Run URL
        }

        private void launchBethesda(String appID, String launchOptions)
        {

        }
    }
}
