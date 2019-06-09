﻿using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace VG_Launcher
{
    /// <summary>
    /// Interaction logic for GameScreen.xaml
    /// </summary>
    public partial class GameScreen : Window
    {

        public GameScreen()
        {
            InitializeComponent();
           
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            //open the settings menu
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("Launched");//launch the exe
            this.Close();
        }
    }
}
