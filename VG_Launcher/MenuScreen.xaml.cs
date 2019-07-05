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
            if (Properties.Settings.Default.ParentalLockEngaged)
            {
                serviceLoader.Visibility = Visibility.Collapsed;
                addGameButton.Visibility = Visibility.Collapsed;
                lockButton.Margin = new Thickness(93, 65, 0, 0);
                this.Height = 185;
            }
            else if (!Properties.Settings.Default.ChildEnabled)
            {
                this.Height = 325;
                lockButton.Visibility = Visibility.Collapsed;
                ChildPromptBlock.Visibility = Visibility.Visible;
                ChildCheck.Visibility = Visibility.Visible;
                //Child lock isnt set up, give user the option to enable it here, default window height = 356, expand it here when adding child lock options
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

        private void LockButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).gameWrapPanel.Children.Clear();
            ((MainWindow)Application.Current.MainWindow).gameWrapPanel.Children.Clear();
            LogInService li = new LogInService();
            this.Hide();
            ((MainWindow)Application.Current.MainWindow).logIn();
            bool locked = Properties.Settings.Default.ParentalLockEngaged;
            if (locked)
                ((MainWindow)Application.Current.MainWindow).CreateButtons(true);
            else
                ((MainWindow)Application.Current.MainWindow).CreateButtons(false);
            this.Close();
        }

        private void ChildCheck_Checked(object sender, RoutedEventArgs e)
        {
            this.Height = 528;
            ParentLabel.Visibility = Visibility.Visible;
            ParentName.Visibility = Visibility.Visible;
            ChildLabel.Visibility = Visibility.Visible;
            ChildName.Visibility = Visibility.Visible;
            LockCodeLabel.Visibility = Visibility.Visible;
            LockCode.Visibility = Visibility.Visible;
            DoneBtn.Visibility = Visibility.Visible;
        }
        private void ChildCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Height = 325;
            ParentLabel.Visibility = Visibility.Collapsed;
            ParentName.Visibility = Visibility.Collapsed;
            ChildLabel.Visibility = Visibility.Collapsed;
            ChildName.Visibility = Visibility.Collapsed;
            LockCodeLabel.Visibility = Visibility.Collapsed;
            LockCode.Visibility = Visibility.Collapsed;
            DoneBtn.Visibility = Visibility.Collapsed;
        }
        private void DoneBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ChildCheck.IsChecked == true)
            {
                Properties.Settings.Default.ParentName = ParentName.Text;
                Properties.Settings.Default.ChildName = ChildName.Text;
                Properties.Settings.Default.ParentLockCode = LockCode.Text;
                Properties.Settings.Default.ChildEnabled = true;
                ParentLockSelect parentLockSelect = new ParentLockSelect(Curlibrary);
                parentLockSelect.Show();
                App.Current.MainWindow.Show();
                this.Close();
            }
            else
            {
                Properties.Settings.Default.ChildEnabled = false;
                App.Current.MainWindow.Show();
                this.Close();
            }
        }
    }
}
