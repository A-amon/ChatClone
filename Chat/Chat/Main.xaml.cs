using Chat.Classes;
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

namespace Chat
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        //        private Sidebar___ChatCircleButton current_tab;
        private Button current_tab;

        public Main()
        {
            InitializeComponent();
           
            List<ChatGroup> chats = new List<ChatGroup>();
            chats.Add(new ChatGroup { Type = "Dm", Image = "/Images/DM.png", Id = "Dm" });
            chats.Add(new ChatGroup { Type = "Add", Id = "Add" });
            ChatsPanel.ItemsSource = chats;

            //set first chatgroup - DM as default
            this.Loaded += delegate{
                ContentPresenter cp = ChatsPanel.ItemContainerGenerator.ContainerFromIndex(0) as ContentPresenter;
                //trigger click event for first chatgroup - DM
                ((Button)VisualTreeHelper.GetChild(cp, 0)).RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

            };
        }

        //click event handler for clicking chatgroup
        private void SelectTab(object sender, RoutedEventArgs e)
        {
            ChangeTabButtonBg(sender);
            
            //change frame content - Navigation
            if (current_tab.Tag.ToString().Equals("Dm"))
            {
                MainFrame.Navigate(new System.Uri("DmPage.xaml", UriKind.RelativeOrAbsolute));
            }
            else
            {
                MainFrame.Navigate(new System.Uri("ServerPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }

        //change background color of button
        //blue for selected / current chatgroup
        private void ChangeTabButtonBg(object sender)
        {
            BrushConverter bc = new BrushConverter();
            if (current_tab != null)
            {
                current_tab.Background = (Brush)bc.ConvertFrom("#323232");
            }
            current_tab = (Button)sender;
            current_tab.Background = (Brush)bc.ConvertFrom("#FF1A73E8");
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void RestoreWindow(object sender, RoutedEventArgs e)
        {
            WindowState state = this.WindowState;
            if (state == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CheckType(object sender, RoutedEventArgs e)
        {
            Grid grid_object = (Grid)sender;
            Image image_object = grid_object.Children.OfType<Image>().First();
            MaterialDesignThemes.Wpf.PackIcon icon_object = grid_object.Children.OfType<MaterialDesignThemes.Wpf.PackIcon>().First();
            if ((grid_object.Tag.ToString()).Equals("Add"))
            {
                image_object.Visibility = Visibility.Collapsed;
                icon_object.Visibility = Visibility.Visible;
            }
        }
    }
}
