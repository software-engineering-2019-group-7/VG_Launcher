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
    /// Interaction logic for ParentLockSelect.xaml
    /// </summary>
    public partial class ParentLockSelect : Window
    {
        public Library library = new Library();
        public ParentLockSelect(Library lib)
        {
            InitializeComponent();
            library = lib;
            loadCheckBoxes();
        }
        public void loadCheckBoxes()
        {
            foreach (Game game in library.gameList)
            {
                CheckBox chkbx = new CheckBox();
                chkbx.Tag = game;
                chkbx.Content = game.name;
                chkbx.FontSize = 13;
                chkbx.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("White"));
                chkbx.FontWeight = FontWeights.SemiBold;
                gameList.Children.Add(chkbx);
            }
        }

        private void DoneBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox chkbx in gameList.Children)
            {
                if (chkbx.IsChecked == true)
                {
                    Game game = (Game)chkbx.Tag;
                    game.parentLock = "1";
                }
            }
            App.Current.MainWindow.Show();
            this.Close();
        }
        private void GameScroller_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }
    }
}
