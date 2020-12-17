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

namespace Chat.Friends
{
    /// <summary>
    /// Interaction logic for Pending.xaml
    /// </summary>
    public partial class Pending : Page, IObserver<User>
    {
        private List<string> requests;
        private UserHandler user_handler;

        public Pending()
        {
            InitializeComponent();

            requests = new List<string>();

            user_handler = new UserHandler();
            user_handler.Subscribe((IObserver<User>)this);

            user_handler.ListenForUpdates(Global.current_user.Id);

            this.Unloaded += delegate
            {
                user_handler.StopListeningUpdates();
            };
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            LogHandler.Log(error.Message);
        }

        public async void OnNext(User value)
        {
            requests = value.Requests;

            List<Friend> request_users = await user_handler.GetAllRequests(requests);

            await Dispatcher.BeginInvoke(new Action(() =>
            {
                RequestsList.ItemsSource = request_users;
            }), null);
        }
       
        private async void AcceptRequest(object sender, RoutedEventArgs e)
        {
            string request_id = ((Button)sender).Tag.ToString();

            int err_code = await user_handler.AddFriend(request_id);
            if(err_code == 1)
            {
                MessageBox.Show("Failed to add user. Please try again later.");
            }
        }

        private async void DeleteRequest(object sender, RoutedEventArgs e)
        {
            string request_id = ((Button)sender).Tag.ToString();

            bool res = await user_handler.RemoveRequestById(request_id);
            if (!res)
                MessageBox.Show("Failed to delete request. Please try again later.");
        }
    }
}
