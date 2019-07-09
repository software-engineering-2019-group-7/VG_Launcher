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
        public bool warningDisplay = false;
        public AccountSetup(Library lib)
        {
            InitializeComponent();
            library = lib;
        }

        private void DoneBtn_Click(object sender, RoutedEventArgs e)
        {
            //Get Info From Text Box(es) and save
            if (library.gameList.Count < 1)
            {
                //no games were added to the library, let user know
                warningDisplay = true;
                if (ChildCheck.IsChecked == true)
                {
                    this.Height = 645;
                    EmptyLibraryWarning.Visibility = Visibility.Visible;
                }
                else
                {
                    this.Height = 480;
                    EmptyLibraryWarning.Margin = new Thickness(10, 366, 0, 0);
                    EmptyLibraryWarning.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (ChildCheck.IsChecked == true)
                {
                    Properties.Settings.Default.ParentName = ParentName.Text;
                    Properties.Settings.Default.ChildName = ChildName.Text;
                    Properties.Settings.Default.ParentLockCode = LockCode.Text;
                    Properties.Settings.Default.ChildEnabled = true;
                    ParentLockSelect parentLockSelect = new ParentLockSelect(library);
                    parentLockSelect.Show();
                    this.Close();
                }
                else
                {
                    Properties.Settings.Default.ChildEnabled = false;
                    Properties.Settings.Default.ParentalLockEngaged = false;
                    App.Current.MainWindow.Show();
                    this.Close();
                }
            }
        }

        private void ChildCheck_Checked(object sender, RoutedEventArgs e)
        {
            if (warningDisplay)
            {
                this.Height = 645;
                EmptyLibraryWarning.Visibility = Visibility.Visible;
                EmptyLibraryWarning.Margin = new Thickness(10, 530, 0, 0);
                warningDisplay = true;
            }
            else
            {
                this.Height = 530;
            }
            DoneBtn.Margin = new Thickness(133,495,0,0);
            LockCodeLabel.Visibility = Visibility.Visible;
            LockCode.Visibility = Visibility.Visible;
            ChildLabel.Visibility = Visibility.Visible;
            ChildName.Visibility = Visibility.Visible;
            ParentLabel.Visibility = Visibility.Visible;
            ParentName.Visibility = Visibility.Visible;
            Properties.Settings.Default.ChildEnabled = true;
        }

        private void ChildCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            if (warningDisplay)
            {
                this.Height = 480;
                EmptyLibraryWarning.Margin = new Thickness(10, 366, 0, 0);
                EmptyLibraryWarning.Visibility = Visibility.Visible;
            }
            else
            {
                this.Height = 360;
            }
            DoneBtn.Margin = new Thickness(133, 330, 0, 0);
            LockCodeLabel.Visibility = Visibility.Collapsed;
            LockCode.Visibility = Visibility.Collapsed;
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
            if (library.gameList.Count > 0)
            {
                //no games were added to the library, let user know
                warningDisplay = false;
                EmptyLibraryWarning.Visibility = Visibility.Collapsed;
                if (ChildCheck.IsChecked == true)
                {
                    this.Height = 530;
                    DoneBtn.Margin = new Thickness(133, 495, 0, 0);
                    LockCodeLabel.Visibility = Visibility.Visible;
                    LockCode.Visibility = Visibility.Visible;
                    ChildLabel.Visibility = Visibility.Visible;
                    ChildName.Visibility = Visibility.Visible;
                    ParentLabel.Visibility = Visibility.Visible;
                    ParentName.Visibility = Visibility.Visible;
                    Properties.Settings.Default.ChildEnabled = true;
                }
                else
                {
                    this.Height = 360;
                    DoneBtn.Margin = new Thickness(133, 330, 0, 0);
                    LockCodeLabel.Visibility = Visibility.Collapsed;
                    LockCode.Visibility = Visibility.Collapsed;
                    ChildLabel.Visibility = Visibility.Collapsed;
                    ChildName.Visibility = Visibility.Collapsed;
                    ParentLabel.Visibility = Visibility.Collapsed;
                    ParentName.Visibility = Visibility.Collapsed;
                    Properties.Settings.Default.ChildEnabled = false;
                }
            }
        }

        private void ManualAddBtn_Click(object sender, RoutedEventArgs e)
        {
            customExe cus = new customExe(library);
            this.Hide();
            cus.ShowDialog();
            this.Show();
            if (library.gameList.Count > 0)
            {
                //no games were added to the library, let user know
                warningDisplay = false;
                EmptyLibraryWarning.Visibility = Visibility.Collapsed;
                if (ChildCheck.IsChecked == true)
                {
                    this.Height = 530;
                    DoneBtn.Margin = new Thickness(133, 495, 0, 0);
                    LockCodeLabel.Visibility = Visibility.Visible;
                    LockCode.Visibility = Visibility.Visible;
                    ChildLabel.Visibility = Visibility.Visible;
                    ChildName.Visibility = Visibility.Visible;
                    ParentLabel.Visibility = Visibility.Visible;
                    ParentName.Visibility = Visibility.Visible;
                    Properties.Settings.Default.ChildEnabled = true;
                }
                else
                {
                    this.Height = 360;
                    DoneBtn.Margin = new Thickness(133, 330, 0, 0);
                    LockCodeLabel.Visibility = Visibility.Collapsed;
                    LockCode.Visibility = Visibility.Collapsed;
                    ChildLabel.Visibility = Visibility.Collapsed;
                    ChildName.Visibility = Visibility.Collapsed;
                    ParentLabel.Visibility = Visibility.Collapsed;
                    ParentName.Visibility = Visibility.Collapsed;
                    Properties.Settings.Default.ChildEnabled = false;
                }
            }
        }

        private void IntroScreen_LocationChanged(object sender, EventArgs e)
        {
            
        }
    }
}
