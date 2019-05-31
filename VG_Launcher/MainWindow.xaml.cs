using System.Windows;

namespace VG_Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SteamBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ServiceLoader_Click(object sender, RoutedEventArgs e)
        {
            ServiceProvider sp = new ServiceProvider();
            sp.ShowDialog();
        }
    }
}
