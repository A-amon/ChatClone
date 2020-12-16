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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chat
{
    /// <summary>
    /// Interaction logic for DmPage.xaml
    /// </summary>
    public partial class DmPage : Page
    {
        public DmPage()
        {
            InitializeComponent();
            List<User> user = new List<User>();
            user.Add(Global.current_user);
            UserPanel.ItemsSource = user; 
        }

        private void SelectFriends(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Uri("Friends/Main.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}
