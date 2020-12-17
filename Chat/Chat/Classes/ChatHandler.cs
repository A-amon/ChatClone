using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Classes
{
    class ChatHandler:IObservable<List<DmChat>>
    {
        private List<IObserver<List<DmChat>>> observers;

        private CollectionReference chat_col;

        public ChatHandler()
        {
            observers = new List<IObserver<List<DmChat>>>();
            chat_col = Global.firestore.Collection("Chats");
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
                    chats.Add(new DmChat
                    {
                        ChatId = doc.Id,
                        FriendId = (string)((List<object>)chat["Users"]).Where(user => !((string)user).Equals(Global.current_user.Id)).ToArray()[0]
                    });
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

        public void ListenForChatsUpdates()
        {
            Query query = chat_col.WhereArrayContains("Users", Global.current_user.Id);
            try
            {
                FirestoreChangeListener listener = query.Listen((Action<QuerySnapshot>)(chat_snapshot =>
                {
                    List<DmChat> chats = new List<DmChat>();
                    foreach(DocumentSnapshot snapshot in chat_snapshot.Documents)
                    {
                        Dictionary<string, object> chat_info = snapshot.ToDictionary();
                        chats.Add(new DmChat
                        {
                            ChatId = snapshot.Id,
                            FriendId = (string)((List<object>)chat_info["Users"]).Where(user => !((string)user).Equals(Global.current_user.Id)).ToArray()[0]
                        });
                    }
                    Global.current_user.Chats = chats;
                    foreach (var observer in observers)
                        observer.OnNext(chats);
                }));
            }
            catch(Exception err)
            {
                LogHandler.Log(err.Message);
            }
        }

        public void ListenForMessageUpdates(string chat_id)
        {
            CollectionReference col_ref = chat_col.Document(chat_id).Collection("Messages");

            try
            {
                FirestoreChangeListener listener = col_ref.Listen((Action<QuerySnapshot>)(message_snapshot =>
                {
                    foreach(DocumentSnapshot snapshot in message_snapshot.Documents)
                    {
                        Dictionary<string, object> message = snapshot.ToDictionary();
                        Console.WriteLine(snapshot.ToString());
                    }
                }));
            }
            catch(Exception err)
            {
                LogHandler.Log(err.Message);
            }
        }

        public IDisposable Subscribe(IObserver<List<DmChat>> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);

            return new Unsubscriber(observers, observer);
        }

        internal class Unsubscriber : IDisposable
        {
            private List<IObserver<List<DmChat>>> observers;
            private IObserver<List<DmChat>> current_observer;

            internal Unsubscriber(List<IObserver<List<DmChat>>> observers, IObserver<List<DmChat>> current_observer)
            {
                this.observers = observers;
                this.current_observer = current_observer;
            }

            public void Dispose()
            {
                if (observers.Contains(current_observer))
                    observers.Remove(current_observer);
            }
        }
    }
}
