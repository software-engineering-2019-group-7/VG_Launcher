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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class AccountSetup : Window
    {
        public Library library = new Library();
        public AccountSetup(Library lib)
        {
            InitializeComponent();
            library = lib;
        }

        private void DoneBtn_Click(object sender, RoutedEventArgs e)
        {
            //Get Info From Text Box(es) and save
            if (ChildCheck.IsChecked == true)
            {
                Properties.Settings.Default.ParentName = ParentName.Text;
                Properties.Settings.Default.ChildName = ChildName.Text;
                ParentLockSelect parentLockSelect = new ParentLockSelect(library);
                parentLockSelect.Show();
                this.Close();
            }
            else
            {
                App.Current.MainWindow.Show();
                this.Close();
            }
        }

        private void ChildCheck_Checked(object sender, RoutedEventArgs e)
        {
            this.Height = 473;
            DoneBtn.Margin = new Thickness(133,438,0,0);
            ChildLabel.Visibility = Visibility.Visible;
            ChildName.Visibility = Visibility.Visible;
            ParentLabel.Visibility = Visibility.Visible;
            ParentName.Visibility = Visibility.Visible;
            Properties.Settings.Default.ChildEnabled = true;
        }

        private void ChildCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Height = 360;
            DoneBtn.Margin = new Thickness(133, 330, 0, 0);
            ChildLabel.Visibility = Visibility.Collapsed;
            ChildName.Visibility = Visibility.Collapsed;
            ParentLabel.Visibility = Visibility.Collapsed;
            ParentName.Visibility = Visibility.Collapsed;
            Properties.Settings.Default.ChildEnabled = false;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void ServiceProviderBtn_Click(object sender, RoutedEventArgs e)
        {
            ServiceProvider sp = new ServiceProvider(library);
            this.Hide();
            sp.ShowDialog();
            this.Show();
        }

        private void ManualAddBtn_Click(object sender, RoutedEventArgs e)
        {
            customExe cus = new customExe(library);
            this.Hide();
            cus.ShowDialog();
            this.Show();
        }
    }
}
