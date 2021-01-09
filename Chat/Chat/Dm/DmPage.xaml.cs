using Chat.Classes;
using Chat.Dm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public partial class DmPage : Page, IObserver<Dictionary<string, DmChat>>
    {
        private ChatHandler chat_handler;
        //List<DmChat> chats = new List<DmChat>();
        ObservableCollection<DmChat> chats;

        public DmPage()
        {
            InitializeComponent();
            //List<User> user = new List<User>();
            //user.Add(Global.current_user);
            //UserPanel.ItemsSource = user;
            UserPanel.DataContext = Global.current_user;
            Global.current_user.PropertyChanged += UpdateProperty;

            chat_handler = new ChatHandler();

            chat_handler.Subscribe((IObserver<Dictionary<string, DmChat>>)this);
            chat_handler.ListenForChatsUpdates();

            chats = new ObservableCollection<DmChat>();

            //set first tab - Friends as default
            ((ListBoxItem)TabsList.Items[0]).RaiseEvent(new RoutedEventArgs(ListBoxItem.SelectedEvent));

            this.Unloaded += delegate
            {
                chat_handler.StopListeningChatsUpdates();
            };

            GetAllChats();
        }

        private void UpdateProperty(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Image"))
            {
                Console.WriteLine(Global.current_user.Image);
                ImageBrush brush = new ImageBrush();
                Uri image_uri = new Uri(Global.current_user.Image);
                brush.ImageSource = new BitmapImage(image_uri);
                UserProfile.Fill = brush;
            }
        }

        private async void GetAllChats()
        {
            List<DmChat> res = await chat_handler.GetAllChats();
            foreach (DmChat chat in res)
                chats.Add(chat);

            ChatsList.ItemsSource = chats;

            Global.current_user.Chats = res;
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(Dictionary<string, DmChat> value)
        {
            string update_type = value.First().Key;
            if (update_type.Equals("Added"))
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    chats.Add((DmChat)value.First().Value);
                }), null);

                Global.current_user.Chats.Add((DmChat)value.First().Value);
            }
            else if (update_type.Equals("Removed"))
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    chats.Remove(chats.Where(chat => chat.ChatId.Equals(((DmChat)value.First().Value).ChatId)).ToArray()[0]);
                }), null);

                Global.current_user.Chats.Remove(Global.current_user.Chats.Where(chat => chat.ChatId.Equals(((DmChat)value.First().Value).ChatId)).ToArray()[0]);
            }
        }

        private void SelectFriends(object sender, RoutedEventArgs e)
        {
            ChatsList.UnselectAll();

            MainFrame.Navigate(new Uri("Friends/Main.xaml", UriKind.RelativeOrAbsolute));
        }

        private void OpenChat(object sender, SelectionChangedEventArgs e)
        {
            TabsList.UnselectAll();
            if (ChatsList.SelectedItem != null)
            {
                string chat_id = ((DmChat)ChatsList.SelectedItem).ChatId;

                MainFrame.Navigate(new DmChatbox(chat_id));
            }
        }

        private void CopyId(object sender, MouseEventArgs e)
        {
            Clipboard.SetText(Global.current_user.Id);
        }

        private void ViewImage(object sender, MouseButtonEventArgs e)
        {
            ImageViewer image_viewer = new ImageViewer();
            image_viewer.ShowDialog();
        }
    }
}
