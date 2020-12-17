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
    public partial class DmPage : Page, IObserver<List<DmChat>>
    {
        private ChatHandler chat_handler;
        List<DmChat> chats = new List<DmChat>();

        public DmPage()
        {
            InitializeComponent();
            List<User> user = new List<User>();
            user.Add(Global.current_user);
            UserPanel.ItemsSource = user;

            chat_handler = new ChatHandler();

            chat_handler.Subscribe((IObserver<List<DmChat>>)this);
            chat_handler.ListenForChatsUpdates();

            //set first tab - Friends as default
            ((ListBoxItem)TabsList.Items[0]).RaiseEvent(new RoutedEventArgs(ListBoxItem.SelectedEvent));
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public async void OnNext(List<DmChat> value)
        {
            chats = value;

            List<Friend> friends = new List<Friend>();

            List<string> ids = new List<string>();
            foreach(DmChat chat in chats)
            {
                ids.Add(chat.FriendId);
            }

            UserHandler user_handler = new UserHandler();
            friends = await user_handler.GetAllRequests(ids);

            await Dispatcher.BeginInvoke(new Action(() =>
            {
                ChatsList.ItemsSource = friends;
            }), null);
        }

        //private async void GetAllChats()
        //{
        //    chats = await chat_handler.GetAllChats();
        //await Dispatcher.BeginInvoke(new Action(() =>
        //    {
        //    ChatsList.ItemsSource = chats;
        //}), null);
        //}

        private void SelectFriends(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Uri("Friends/Main.xaml", UriKind.RelativeOrAbsolute));
        }

        private void OpenChat(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
