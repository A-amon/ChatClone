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
    /// Interaction logic for FriendsPage.xaml
    /// </summary>
    public partial class FriendsPage : Page
    {
        public FriendsPage()
        {
            InitializeComponent();

            //set first tab - All as default
            TabList.SelectedIndex = 0;
        }

        private void SelectTab(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem selected = TabList.SelectedItem as ListBoxItem;
            if (selected.Tag.ToString().Equals("All"))
            {
                MainFrame.Navigate(new Uri("Friends/All.xaml",UriKind.RelativeOrAbsolute));
            }
            else if (selected.Tag.ToString().Equals("Pending"))
            {
                MainFrame.Navigate(new Uri("Friends/Pending.xaml", UriKind.RelativeOrAbsolute));
            }
        }

        private void AddFriend(object sender, RoutedEventArgs e)
        {
            Friends.Add add_modal = new Friends.Add();
            add_modal.ShowDialog();
        }
    }
}
