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
    /// Interaction logic for customExe.xaml
    /// </summary>
    public partial class customExe : Window
    {
        Library Curlibrary;
        Game CurGame;
        public customExe()
        {
            InitializeComponent();
        }
        public customExe(Library lib)
        {
            InitializeComponent();
            Curlibrary = lib;
            CurGame = new Game();
            Curlibrary.gameList.Add(CurGame);
        }
        public customExe(Game game)
        {
            InitializeComponent();
            CurGame = game;
            nameBox.Text = game.name;
            pathButton.Content = game.path;
            imageButton.Content = game.image;
            textBlock.Text = "Change details for: \"" + game.name + "\""; 
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            //Remove the newly created game to prevent filling the JSON with empty entries
            Curlibrary.gameList.Remove(CurGame);
            this.Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if(!(nameBox.Text == "" || pathButton.Content.Equals("Choose path") || imageButton.Content.Equals("Choose path")))
            {
                Game g = new Game();
                CurGame.name = nameBox.Text;
                CurGame.path = pathButton.Content.ToString();
                CurGame.image = imageButton.Content.ToString();
                if (lockBox.IsChecked == true)
                    CurGame.parentLock = "1";
                else
                    CurGame.parentLock = "0";

                ((MainWindow)Application.Current.MainWindow).CreateButtons(Properties.Settings.Default.ParentalLockEngaged);
                this.Close();
            }
        }

        private void PathButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".exe";
            dlg.Filter = "EXE Files (*.exe)|*.exe";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result.HasValue && result.Value)
            {
                string filename = dlg.FileName;
                pathButton.Content = filename;
            }
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".png";
            dlg.Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpeg)|*.jpeg|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result.HasValue && result.Value)
            {
                string filename = dlg.FileName;
                imageButton.Content = filename;
            }
        }

    }
}
