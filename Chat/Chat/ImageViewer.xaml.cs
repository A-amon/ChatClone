using Chat.Classes;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Chat
{
    /// <summary>
    /// Interaction logic for ImageViewer.xaml
    /// </summary>
    public partial class ImageViewer : Window
    {
        Uri image_uri = null;
        public ImageViewer()
        {
            InitializeComponent();
            SetImage(Global.current_user.Image);
        }

        private void ChooseImage(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Choose an image";
            dialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
        "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
        "Portable Network Graphic (*.png)|*.png";
            DialogResult res = dialog.ShowDialog();
            if(res == System.Windows.Forms.DialogResult.OK)
            {
                SetImage(dialog.FileName);
            }
        }

        private async void SaveImage(object sender, RoutedEventArgs e)
        {
            string image_path = image_uri.AbsolutePath;
            UserHandler user_handler = new UserHandler();
            bool res = await user_handler.UpdateImage(image_path);
            if(res)
                this.Close();
            else
                System.Windows.Forms.MessageBox.Show("Failed to update profile picture. Please try again later.");
        }

        private void DragPreview(object sender, System.Windows.DragEventArgs e)
        {
            MainCard.Opacity = 0.5;
        }

        private void EndPreview(object sender, System.Windows.DragEventArgs e)
        {
            MainCard.Opacity = 1;
        }

        private void DropImage(object sender, System.Windows.DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
            SetImage(files[0]);
            MainCard.Opacity = 1;
        }

        private void SetImage(string source)
        {
            ImageBrush brush = new ImageBrush();
            image_uri = new Uri(source);
            brush.ImageSource = new BitmapImage(image_uri);
            ImageView.Fill = brush;
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
