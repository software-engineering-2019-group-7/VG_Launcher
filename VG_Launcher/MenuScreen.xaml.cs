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
        }

        public MenuScreen(Library lib)
        {
            InitializeComponent();
            Curlibrary = lib;
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
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddGameButton_Click(object sender, RoutedEventArgs e)
        {
            customExe cus = new customExe(Curlibrary);
            this.Close();
            cus.ShowDialog();
        }
    }
}
