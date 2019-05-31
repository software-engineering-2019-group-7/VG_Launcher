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



                ///this commented out chunk would let us set the buttons to whatever we wanted them to look like.
                ///Picture backgrounds included, but they need to be downloaded as of right now. Will look into this.

                btn.Name = "button" + i.ToString();
                btn.Content = "Path of Exile"; //replace this with the name of the game recieved
                btn.Width = 360;
                btn.Height = 160;
                btn.Margin = new Thickness(10);
                btn.HorizontalContentAlignment = HorizontalAlignment.Center;
                btn.VerticalContentAlignment = VerticalAlignment.Bottom;
                btn.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CFFFFFF"));
                btn.FontSize = 48;
                btn.FontWeight = FontWeights.SemiBold;
                Uri resourceUri = new Uri("Resources/header.jpg", UriKind.Relative);
                StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);
                BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
                var brush = new ImageBrush();
                brush.ImageSource = temp;
                btn.Background = brush;
                btn.Click += Button_Click;
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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            btn.Content = "Clicked";
        }
    }
}
