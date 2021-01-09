using Chat.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Chat.Dm
{
    /// <summary>
    /// Interaction logic for DmChatbox.xaml
    /// </summary>
    public partial class DmChatbox : Page, IObserver<Dictionary<string, Message>>
    {
        private string chat_id;
        private ChatHandler chat_handler;
        //private List<Message> messages;
        private ObservableCollection<Message> messages;

        public DmChatbox(string chat_id)
        {
            InitializeComponent();
            chat_handler = new ChatHandler();

            this.chat_id = chat_id;

            messages = new ObservableCollection<Message>();

            GetAllMessages();

            chat_handler.Subscribe((IObserver<Dictionary<string, Message>>)this);
            chat_handler.ListenForMessageUpdates(chat_id);

            this.Unloaded += delegate
            {
                chat_handler.StopListeningMessagesUpdates();
            };
        }

        private async void GetAllMessages()
        {
            List<Message> res = await chat_handler.GetAllMessages(chat_id);
            if (res.Count > 0)
            {
                foreach (Message msg in res)
                    messages.Add(msg);
            }
            MessageList.ItemsSource = messages;
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(Dictionary<string,Message> value)
        {
            string update_type = value.First().Key;
            if (update_type.Equals("Added"))
            {
                Dispatcher.BeginInvoke(new Action(()=> {
                    messages.Add((Message)value.First().Value);
                }), null);
            }
            else if(update_type.Equals("Removed"))
            {
                Dispatcher.BeginInvoke(new Action(()=> {
                    messages.Remove(messages.Where(message => message.MessageId.Equals(((Message)value.First().Value).MessageId)).ToArray()[0]);
                }), null);
            }
            //foreach (Message msg in value)
            //    messages.Add(msg);
            ////messages = value;
            //Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    MessageList.ItemsSource = messages;
            //}), null);
        }

        private async void SendMessage(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter) || e.Key.Equals(Key.Return))
            {
                string message = MessageTextbox.Text;
                if (!string.IsNullOrEmpty(message) && !string.IsNullOrWhiteSpace(message))
                {
                    bool res = await chat_handler.SendMessage(chat_id, message);
                    MessageTextbox.Text = "";
                    if (!res)
                        MessageBox.Show("Failed to send message. Please try again later.");
                }
            }
        }
    }
}
