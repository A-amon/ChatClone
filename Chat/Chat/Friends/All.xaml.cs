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
    public partial class All : Page, IObserver<User>
    {
        private List<string> friends;
        private UserHandler user_handler;

        public All()
        {
            InitializeComponent();

            friends = new List<string>();

            user_handler = new UserHandler();
            user_handler.Subscribe((IObserver<User>)this);

            user_handler.ListenForUpdates(Global.current_user.Id);

            this.Unloaded += delegate
            {
                user_handler.StopListeningUpdates();
            };
        }     

        private async void Message(object sender, RoutedEventArgs e)
        {
            string friend_id = ((Button)sender).Tag.ToString();

            if(Global.current_user.Chats.Where(chat=>chat.FriendId.Equals(friend_id)).ToArray().Count() == 0)
            {
                ChatHandler chat_handler = new ChatHandler();
                bool res = await chat_handler.CreateChat(friend_id);
                if (!res)
                    MessageBox.Show("Failed to create conversation. Please try again later.");
            }
        }

        public async void OnNext(User value)
        {
            friends = value.Friends;

            List<Friend> friend_users = await user_handler.GetAllFriends(friends);

            await Dispatcher.BeginInvoke(new Action(() =>
            {
                FriendsList.ItemsSource = friend_users;
            }), null);
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
    }
}
