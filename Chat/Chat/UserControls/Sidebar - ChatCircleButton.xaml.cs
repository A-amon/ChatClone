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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chat
{
    /// <summary>
    /// Interaction logic for Sidebar___ChatCircleButton.xaml
    /// </summary>
    public partial class Sidebar___ChatCircleButton : UserControl
    {
        public Sidebar___ChatCircleButton()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public Uri _ImageSrc;

        public Uri ImageSrc {
            get {
                return _ImageSrc; 
            }
            set {
                _ImageSrc = value;
            }
        }

        public string Type { get; set; }

        private void CheckType(object sender, RoutedEventArgs e)
        {
            Grid grid_object = (Grid)sender;
            Image image_object = grid_object.Children.OfType<Image>().First();
            MaterialDesignThemes.Wpf.PackIcon icon_object = grid_object.Children.OfType<MaterialDesignThemes.Wpf.PackIcon>().First();
            if (Type.Equals("Add"))
            {
                image_object.Visibility = Visibility.Collapsed;
                icon_object.Visibility = Visibility.Visible;
            }
        }
    }
}
