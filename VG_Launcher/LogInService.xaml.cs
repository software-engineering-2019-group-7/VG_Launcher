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
    /// Interaction logic for LogInService.xaml
    /// </summary>
    public partial class LogInService : Window
    {
        private string lockCode = Properties.Settings.Default.ParentLockCode;
        public LogInService()
        {
            InitializeComponent();
            parentButton.Content = Properties.Settings.Default.ParentName;
            childButton.Content = Properties.Settings.Default.ChildName;
        }

        private void ParentButton_Click(object sender, RoutedEventArgs e)
        {
            if (passwordBox.Password.Equals(lockCode))
            {
                Properties.Settings.Default.ParentalLockEngaged = false;
                this.Close();
            }
            else
            {
                InvalidCode.Visibility = Visibility.Visible;
            }
        }

        private void ChildButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ParentalLockEngaged = true;
            this.Close();
        }
    }
}
