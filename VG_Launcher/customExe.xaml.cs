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
    /// Interaction logic for customExe.xaml
    /// </summary>
    public partial class customExe : Window
    {
        Library Curlibrary;
        public customExe()
        {
            InitializeComponent();
        }
        public customExe(Library lib)
        {
            InitializeComponent();
            Curlibrary = lib;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (nameBox.Text == "" || pathBox.Text == "" || imageBox.Text == "")
            {
              //probably tell them something is wrong
            }
            else
            {
                Game g = new Game();
                g.name = nameBox.Text;
                g.path = pathBox.Text;
                g.image = imageBox.Text;
                Curlibrary.addGame(g);
            }
        }
    }
}
