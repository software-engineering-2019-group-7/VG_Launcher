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
    /// Interaction logic for MenuScreen.xaml
    /// </summary>
    public partial class MenuScreen : Window
    {
        Library Curlibrary;
        public MenuScreen()
        {
            InitializeComponent();
            if (Properties.Settings.Default.ParentalLockEngaged)
            {
                serviceLoader.Visibility = Visibility.Hidden;
                addGameButton.Visibility = Visibility.Hidden;
            }
        }

        public MenuScreen(Library lib)
        {
            InitializeComponent();
            Curlibrary = lib;
            if (Properties.Settings.Default.ParentalLockEngaged)
            {
                serviceLoader.Visibility = Visibility.Hidden;
                addGameButton.Visibility = Visibility.Hidden;
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ServiceLoader_Click(object sender, RoutedEventArgs e)
        {

            ServiceProvider sp = new ServiceProvider(Curlibrary);
            this.Close();
            sp.ShowDialog();
            foreach (Window w in App.Current.Windows)
            {
                if (w.Name.Equals("gs"))
                {
                    w.Close();
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            foreach (Window w in App.Current.Windows)
            {
                if (w.Name.Equals("gs"))
                {
                    w.Close();
                }
            }
        }

        private void AddGameButton_Click(object sender, RoutedEventArgs e)
        {
            customExe cus = new customExe(Curlibrary);
            this.Close();
            cus.ShowDialog();
            foreach (Window w in App.Current.Windows)
            {
                if (w.Name.Equals("gs"))
                {
                    w.Close();
                }
            }
        }

        private void LockButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window w in App.Current.Windows)
            {
                if (w.Name.Equals("gs"))
                {
                    w.Close();
                }
            }
            
            ((MainWindow)Application.Current.MainWindow).gameWrapPanel.Children.Clear();
            ((MainWindow)Application.Current.MainWindow).gameWrapPanel.Children.Clear();
            LogInService li = new LogInService();
            ((MainWindow)Application.Current.MainWindow).logIn();
            bool locked = Properties.Settings.Default.ParentalLockEngaged;
            if (locked)
                ((MainWindow)Application.Current.MainWindow).CreateButtons(true);
            else
                ((MainWindow)Application.Current.MainWindow).CreateButtons(false);
            this.Close();
        }
    }
}
