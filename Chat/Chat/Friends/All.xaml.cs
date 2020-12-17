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
    /// Interaction logic for All.xaml
    /// </summary>
    public partial class All : Page
    {
        private List<string> friends;
        private UserHandler user_handler;

        public All()
        {
            InitializeComponent();

            friends = Global.current_user.Friends;
            user_handler = new UserHandler();

            SetDisplayFriends();
        }

        private List<Friend> friend_users;
        private async void SetDisplayFriends()
        {
            friend_users = await user_handler.GetAllFriends(friends);

            await Dispatcher.BeginInvoke(new Action(() =>
            {
                FriendsList.ItemsSource = friend_users;
            }), null);
        }

        private async void Message(object sender, RoutedEventArgs e)
        {
            string friend_id = ((Button)sender).Tag.ToString();

            ChatHandler chat_handler = new ChatHandler();
            bool res = await chat_handler.CreateChat(friend_id);
            if (!res)
                MessageBox.Show("Failed to create conversation. Please try again later.");

        }
    }
}
