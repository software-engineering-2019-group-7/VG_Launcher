using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VG_Launcher
{
    /// <summary>
    /// Interaction logic for SettingsMenu.xaml
    /// </summary>
    public partial class SettingsMenu : Window
    {
        Game setGame;
        public SettingsMenu(Game game)
        {
            setGame = game;
            InitializeComponent();
            launchText.Text = setGame.settings;
            if (game.provider != "Steam" && game.provider != null)
            {
                warningLabel2.Visibility = Visibility.Visible;
                launchText.Visibility = Visibility.Collapsed;
                saveButton.Visibility = Visibility.Collapsed;
                warningLabel.Content = "Launch options are not yet available for this provider.";
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).clickReciever.Visibility = Visibility.Hidden;
            this.Close();

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button; //lets us edit the button that sent the function call 
            Game game = (Game)btn.Tag;
            
            String settings = launchText.Text;
            game.settings = settings;
        }

        private void ChangeDetails_Click(object sender, RoutedEventArgs e)
        {
            customExe cus = new customExe(setGame);
            cus.close.Visibility = Visibility.Hidden;
            this.Hide();
            cus.ShowDialog();
            this.Show();            
        }

        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            System.Diagnostics.Process.Start("appwiz.cpl");
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Button btn = sender as Button; //lets us edit the button that sent the function call 
            Game g = (Game) btn.Tag;
            ((MainWindow)Application.Current.MainWindow).HideGame(setGame.name);
        }
    }
}
