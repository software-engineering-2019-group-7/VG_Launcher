using System.Collections.Generic;
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

        public void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> serviceList = new List<string>();
            foreach (CheckBox c in services.Children)
            {
                if (c.IsChecked == true)
                {
                    serviceList.Add(c.Name);
                }
            }
            //SomeFunctionThatWillTakeInTheList();
            this.Close();
        }
    }
}
