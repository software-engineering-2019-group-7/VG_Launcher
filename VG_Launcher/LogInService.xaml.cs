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
        private string lockCode = "1234";
        public LogInService()
        {
            InitializeComponent();
        }

        private void ParentButton_Click(object sender, RoutedEventArgs e)
        {
            if (codeBox.Text.Equals("CODE"))
                ;
            else if (codeBox.Text.Equals(lockCode))
            {
                Properties.Settings.Default.ParentalLockEngaged = false;
                this.Close();
            }
        }

        private void ChildButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ParentalLockEngaged = true;
            this.Close();
        }
    }
}
