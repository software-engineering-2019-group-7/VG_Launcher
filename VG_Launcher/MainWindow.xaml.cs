using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace VG_Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        void CreateButtons()
        {
            for (int i = 0; i < 5; i++)
            {
                Button btn = new Button();
                btn.Content = "String";
                btn.Width = 350;
                btn.Height = 160;
                btn.Margin = new Thickness(10);

                ///this commented out chunk would let us set the pictures of the background
                ///of the buttons. Just setting them to red for now

                //Uri resourceUri = new Uri("Resources/header.jpg", UriKind.Relative); 
                //StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);
                //BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
                //var brush = new ImageBrush();
                //brush.ImageSource = temp;
                //btn.Background = brush;
                btn.Background = Brushes.Red;
                gameWrapPanel.Children.Add(btn);
            }
        }

        private void ServiceLoader_Click(object sender, RoutedEventArgs e)
        {
            ServiceProvider sp = new ServiceProvider();
            sp.ShowDialog();
        }

        private void Addbtns_Click(object sender, RoutedEventArgs e)
        {
            CreateButtons();
            gameWrapPanel.Height = gameWrapPanel.Height + 180;
        }
    }
}
