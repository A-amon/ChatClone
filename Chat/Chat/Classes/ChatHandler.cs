using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Classes
{
    class ChatHandler:IObservable<Dictionary<string, DmChat>>, IObservable<Dictionary<string,Message>>
    {
        private List<IObserver<Dictionary<string, DmChat>>> chat_observers;
        private List<IObserver<Dictionary<string, Message>>> message_observers;

        private CollectionReference chat_col;

        public ChatHandler()
        {
            chat_observers = new List<IObserver<Dictionary<string, DmChat>>>();
            message_observers = new List<IObserver<Dictionary<string, Message>>>();

            chat_col = Global.firestore.Collection("Chats");
        }

        public async Task<List<Message>> GetAllMessages(string chat_id)
        {
            List<Message> messages = new List<Message>();
            CollectionReference message_col = chat_col.Document(chat_id).Collection("Messages");
            Query query = message_col.OrderBy("Datetime");
            try
            {
                QuerySnapshot snapshot = await query.GetSnapshotAsync();
                foreach (DocumentSnapshot doc in snapshot.Documents)
                {
                    Dictionary<string, object> message_info = doc.ToDictionary();
                    Message message = new Message
                    {
                        MessageId = doc.Id,
                        SenderId = message_info["Sender_Id"].ToString(),
                        Text = message_info["Text"].ToString(),
                        Datetime = ((Timestamp)message_info["Datetime"]).ToDateTime()
                    };

                    //get message sender's username
                    try
                    {
                        UserHandler user_handler = new UserHandler();
                        DocumentSnapshot sender_snapshot = await user_handler.GetUserById(message.SenderId);
                        Dictionary<string, object> sender_info = sender_snapshot.ToDictionary();
                        message.Name = sender_info["Username"].ToString();
                        message.Image = sender_info["Profile"].ToString();
                    }
                    catch (Exception err)
                    {
                        LogHandler.Log(err.Message);
                    }

                    messages.Add(message);
                }
                LogHandler.Log(string.Format("Retrieved {0} messages for chat {1}", messages.Count, chat_id));
            }
            catch (Exception err)
            {
                LogHandler.Log(err.Message);
            }

            return messages;
        }

        public async Task<List<DmChat>> GetAllChats()
        {
            List<DmChat> chats = new List<DmChat>();
            Query query = chat_col.WhereArrayContains("Users", Global.current_user.Id);
            try
            {
                QuerySnapshot snapshot = await query.GetSnapshotAsync();
                foreach (DocumentSnapshot doc in snapshot.Documents)
                {
                    Dictionary<string, object> chat = doc.ToDictionary();
                    DmChat dm_chat = new DmChat
                    {
                        ChatId = doc.Id,
                        FriendId = (string)((List<object>)chat["Users"]).Where(user => !((string)user).Equals(Global.current_user.Id)).ToArray()[0]
                    };

                    //get chat friend's username
                    try
                    {
                        UserHandler user_handler = new UserHandler();
                        DocumentSnapshot friend_snapshot = await user_handler.GetUserById(dm_chat.FriendId);
                        Dictionary<string, object> friend_info = friend_snapshot.ToDictionary();
                        dm_chat.Name = friend_info["Username"].ToString();
                        dm_chat.Image = friend_info["Profile"].ToString();
                    }
                    catch(Exception err)
                    {
                        LogHandler.Log(err.Message);
                    }

                    chats.Add(dm_chat);
                }
                LogHandler.Log(string.Format("Retrieved {0} chats",chats.Count));
            }
            catch(Exception err)
            {
                LogHandler.Log(err.Message);
            }

            return chats;
        }

        public async Task<bool> CreateChat(string friend_id)
        {
            Dictionary<string, object> info = new Dictionary<string, object>
            {
                {"Users",new string[]{Global.current_user.Id,friend_id} }
            };

            try
            {
                DocumentReference doc_ref = await chat_col.AddAsync(info);
                LogHandler.Log(string.Format("Successfully created chat for {0} and {1}", Global.current_user.Id, friend_id));
                    
                return true;
            }
            catch(Exception err)
            {
                LogHandler.Log(err.Message);
            }
            return false;
        }

        private FirestoreChangeListener chat_listener;
        private bool chat_initial_load = true;
        public void ListenForChatsUpdates()
        {
            Query query = chat_col.WhereArrayContains("Users", Global.current_user.Id);
            try
            {
                chat_listener = query.Listen((Action<QuerySnapshot>)(async chat_snapshot =>
                {
                    if (!chat_initial_load)
                    {
                        DocumentChange change = chat_snapshot.Changes[0];

                        Dictionary<string, object> update = change.Document.ToDictionary();

                        DmChat chat = new DmChat
                        {
                            ChatId = change.Document.Id,
                            FriendId = (string)((List<object>)update["Users"]).Where(user => !((string)user).Equals(Global.current_user.Id)).ToArray()[0]
                        };

                        //get chat friend's username
                        try
                        {
                            UserHandler user_handler = new UserHandler();
                            DocumentSnapshot friend_snapshot = await user_handler.GetUserById(chat.FriendId);
                            Dictionary<string, object> friend_info = friend_snapshot.ToDictionary();
                            chat.Name = friend_info["Username"].ToString();
                            chat.Image = friend_info["Profile"].ToString();
                        }
                        catch (Exception err)
                        {
                            LogHandler.Log(err.Message);
                        }

                        foreach (var observer in chat_observers)
                        {
                            observer.OnNext(new Dictionary<string, DmChat> {
                                {change.ChangeType.ToString(),chat }
                            });
                        }

                        if (change.ChangeType.ToString().Equals("Added"))
                            LogHandler.Log(string.Format("Update: Added chat Id= {0}", change.Document.Id));
                        else if (change.ChangeType.ToString().Equals("Removed"))
                            LogHandler.Log(string.Format("Update: Removed chat Id= {0}", change.Document.Id));
                    }
                    chat_initial_load = false;
                }));
            }
            catch(Exception err)
            {
                LogHandler.Log(err.Message);
            }
        }

        public async void StopListeningChatsUpdates()
        {
            await chat_listener.StopAsync();
        }

        private FirestoreChangeListener message_listener;
        private bool message_initial_load = true;

        public void ListenForMessageUpdates(string chat_id)
        {
            CollectionReference col_ref = chat_col.Document(chat_id).Collection("Messages");

            try
            {
                message_listener = col_ref.Listen((Action<QuerySnapshot>)(async message_snapshot =>
                {
                    if(!message_initial_load)
                    {
                        DocumentChange change = message_snapshot.Changes[0];

                        Dictionary<string, object> update = change.Document.ToDictionary();

                        Message message = new Message
                        {
                            MessageId = change.Document.Id,
                            SenderId = update["Sender_Id"].ToString(),
                            Text = update["Text"].ToString(),
                            Datetime = ((Timestamp)update["Datetime"]).ToDateTime()
                        };

                        //get message sender's username
                        try
                        {
                            UserHandler user_handler = new UserHandler();
                            DocumentSnapshot sender_snapshot = await user_handler.GetUserById(message.SenderId);
                            Dictionary<string, object> sender_info = sender_snapshot.ToDictionary();
                            message.Name = sender_info["Username"].ToString();
                            message.Image = sender_info["Profile"].ToString();
                        }
                        catch (Exception err)
                        {
                            LogHandler.Log(err.Message);
                        }

                        foreach (var observer in message_observers)
                        {
                            observer.OnNext(new Dictionary<string, Message> {
                                {change.ChangeType.ToString(),message }
                            });
                        }

                        if(change.ChangeType.ToString().Equals("Added"))
                            LogHandler.Log(string.Format("Update: Added message Id= {0}", change.Document.Id));
                        else if(change.ChangeType.ToString().Equals("Removed"))
                            LogHandler.Log(string.Format("Update: Removed message Id= {0}", change.Document.Id));
                    }
                    message_initial_load = false;
                }));
            }
            catch(Exception err)
            {
                LogHandler.Log(err.Message);
            }
        }

        public async void StopListeningMessagesUpdates()
        {
            await message_listener.StopAsync();
        }

        public async Task<bool> SendMessage(string chat_id, string message)
        {
            CollectionReference col_ref = chat_col.Document(chat_id).Collection("Messages");
            try
            {
                Dictionary<string, object> new_message = new Dictionary<string, object>
                {
                    {"Text",message },
                    {"Sender_Id",Global.current_user.Id },
                    {"Datetime",DateTime.Now.ToUniversalTime() }
                };
                await col_ref.AddAsync(new_message);

                return true;
            }
            catch(Exception err)
            {
                LogHandler.Log(err.Message);
            }

            return false;
        }

        public IDisposable Subscribe(IObserver<Dictionary<string, DmChat>> observer)
        {
            if (!chat_observers.Contains(observer))
                chat_observers.Add(observer);

            return new Unsubscriber(chat_observers, observer);
        }

        public IDisposable Subscribe(IObserver<Dictionary<string, Message>> observer)
        {
            if (!message_observers.Contains(observer))
                message_observers.Add(observer);

            return new Unsubscriber(message_observers, observer);
        }

        internal class Unsubscriber : IDisposable
        {
            private List<IObserver<Dictionary<string, DmChat>>> chat_observers;
            private IObserver<Dictionary<string, DmChat>> chat_observer;

            private List<IObserver<Dictionary<string, Message>>> message_observers;
            private IObserver<Dictionary<string, Message>> message_observer;

            internal Unsubscriber(List<IObserver<Dictionary<string, DmChat>>> observers, IObserver<Dictionary<string, DmChat>> current_observer)
            {
                this.chat_observers = observers;
                this.chat_observer = current_observer;
            }

            internal Unsubscriber(List<IObserver<Dictionary<string, Message>>> observers, IObserver<Dictionary<string, Message>> current_observer)
            {
                this.message_observers = observers;
                this.message_observer = current_observer;
            }

            public void Dispose()
            {
                if (chat_observers != null)
                {
                    if (chat_observers.Contains(chat_observer))
                        chat_observers.Remove(chat_observer);
                }
                else
                {
                    if (message_observers.Contains(message_observer))
                        message_observers.Remove(message_observer);
                }
            }
        }
    }
}
