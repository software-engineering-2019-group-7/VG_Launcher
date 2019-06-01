using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
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
                Button btn = new Button();

                ///this chunk will let us set the buttons to whatever we wanted them to look like.
                ///Picture backgrounds included, but they need to be downloaded as of right now. Will look into this.

                btn.Name = "button" + gameWrapPanel.Children.Count;
                btn.Content = "Path of Exile"; //replace this with the name of the game recieved
                btn.Width = 360;
                btn.Height = 160;
                btn.Margin = new Thickness(10);
                btn.HorizontalContentAlignment = HorizontalAlignment.Center;
                btn.VerticalContentAlignment = VerticalAlignment.Bottom;
                btn.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CFFFFFF"));
                btn.FontSize = 48;
                btn.FontWeight = FontWeights.SemiBold;

                ///This section deals with the background picture. We will have to change this for sure if we are getting them from the internet. 
                ///I suppose we will have to download the images regardless, so maybe we will have the actual images at his point. 
                ///Steam and others load a default background and replace it when they have the actual image (maybe do this on a different thread?)
                Uri resourceUri = new Uri("Resources/header.jpg", UriKind.Relative);
                StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);
                BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
                var brush = new ImageBrush();
                brush.ImageSource = temp;
                btn.Background = brush;

                //This lets us click the button. All of the buttons will share a function called Button_Click so we will have to be creative.
                //Theres no way we can create a method for each new button, at least not that I know of. 
                btn.Click += Button_Click;


                // A dropshadow would look nice on the added buttons but I cant seem to make it work. I will look into this
                //
                //btn.Effect = new DropShadowEffect()
                //{
                //    BlurRadius = 10,
                //    ShadowDepth = 5
                //};

                gameWrapPanel.Children.Add(btn);
            
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
            Button btn = sender as Button; //lets us edit the button that sent the function call
            btn.Content = "Clicked";
            MessageBox.Show(btn.Name + " Clicked");
        }
    }
}
