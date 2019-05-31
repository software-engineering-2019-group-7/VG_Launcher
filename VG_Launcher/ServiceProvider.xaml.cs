using System.Windows;
using System.Windows.Controls;

namespace VG_Launcher
{
    /// <summary>
    /// Interaction logic for ServiceProvider.xaml
    /// </summary>
    public partial class ServiceProvider : Window
    {
        public ServiceProvider()
        {
            InitializeComponent();
        } 

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Control c in services.Children)
            {
                if (((CheckBox)c).IsChecked == true)
                {
                    //push c.Content into a list
                }
                else
                {
                    //nothing
                }
            }
            this.Close();
        }
    }
}
