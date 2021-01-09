using Google.Cloud.Firestore;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Classes
{
    class UserHandler:IObservable<User>
    {
        private List<IObserver<User>> observers;

        private CollectionReference users_col;

        public UserHandler()
        {
            observers = new List<IObserver<User>>();

            users_col = Global.firestore.Collection("Users");
        }

        public async Task<bool> UpdateImage(string image_path)
        {
            try
            {
                var storage = StorageClient.Create();
                using (var file = File.OpenRead(image_path))
                {
                    Google.Apis.Storage.v1.Data.Object uploaded = await storage.UploadObjectAsync("ferrous-destiny-298611.appspot.com", Global.current_user.Id, null, file);
                    LogHandler.Log(string.Format("Uploaded user {0} profile image", Global.current_user.Id));
                    try
                    {
                        DocumentReference doc_ref = users_col.Document(Global.current_user.Id);
                        Dictionary<string, object> update = new Dictionary<string, object>
                        {
                            {"Profile", uploaded.MediaLink }
                        };
                        await doc_ref.UpdateAsync(update);
                        Global.current_user.Image = uploaded.MediaLink;

                        LogHandler.Log(string.Format("Updated user {0} profile image", Global.current_user.Id));
                        return true;
                    }
                    catch(Exception err)
                    {
                        LogHandler.Log(err.Message);
                    }
                }
                return true;
            }
            catch(Exception err)
            {
                LogHandler.Log(err.Message);
            }
            return false;
        }

        private string HashPassword(string password)
        {
            var hasher = new SHA256Managed();
            var bytes = System.Text.Encoding.Unicode.GetBytes(password);
            var hashed = hasher.ComputeHash(bytes);

            return Convert.ToBase64String(hashed);
        }

        //get user info by their Id
        public async Task<DocumentSnapshot> GetUserById(string id)
        {
            try
            {
                DocumentReference docRef = users_col.Document(id);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                return snapshot;
            }
            catch(Exception err)
            {
                throw err;
            }
        }

        //get users' name and id from list of friends
        public async Task<List<Friend>> GetAllFriends(List<string> friends)
        {
            List<string> invalid_friends = new List<string>();
            List<Friend> friend_users = new List<Friend>();
            foreach (string friend in friends)
            {
                try
                {
                    DocumentSnapshot snapshot = await GetUserById(friend);
                    //if user with Id exists
                    if (snapshot.Exists)
                    {
                        Dictionary<string, object> user = snapshot.ToDictionary();
                        friend_users.Add(new Friend { Id = friend, Name = user["Username"].ToString(), Image = user["Profile"].ToString() });
                    }
                    //add invalid/not found user Id to list of Id (to be removed from friends array)
                    else
                        invalid_friends.Add(friend);
                }
                catch (Exception err)
                {
                    LogHandler.Log(err.Message);
                }
            }

            //remove Id from friends array if not valid/ found
            foreach (string friend in invalid_friends)
                await RemoveFriendById(friend); 

            return friend_users;
        }

        //delete invalid request's user id in user's list of friends
        private async Task<bool> RemoveFriendById(string friend_id)
        {
            try
            {
                Global.current_user.Friends.Remove(friend_id);
                DocumentReference doc_ref = users_col.Document(Global.current_user.Id);
                Dictionary<string, object> update = new Dictionary<string, object>
                {
                    {"Friends", Global.current_user.Friends }
                };
                await doc_ref.UpdateAsync(update);
                LogHandler.Log(string.Format("Removed invalid friend Id= {0}", friend_id));
                return true;
            }
            catch (Exception err)
            {
                LogHandler.Log(err.Message);
            }
            return false;
        }

        //get users' name and id from list of requests
        public async Task<List<Friend>> GetAllRequests(List<string> requests)
        {
            List<string> invalid_req = new List<string>();
            List<Friend> request_users = new List<Friend>();
            foreach(string request in requests)
            {
                try
                {
                    DocumentSnapshot snapshot = await GetUserById(request);
                    //if user with Id exists
                    if (snapshot.Exists)
                    {
                        Dictionary<string, object> user = snapshot.ToDictionary();
                        request_users.Add(new Friend { Id = request, Name = user["Username"].ToString(), Image = user["Profile"].ToString() });
                    }
                    //add invalid/ not found user Id to list of Id (to be removed from requests array)
                    else
                        invalid_req.Add(request);
                }
                catch(Exception err)
                {
                    LogHandler.Log(err.Message);
                }
            }

            //remove Id from requests array if not valid/ found
            foreach (string req in invalid_req)
                await RemoveRequestById(req);

            return request_users;
        }

        //delete invalid request's user id in user's list of requests
        public async Task<bool> RemoveRequestById(string request_id)
        {
            try
            {
                Global.current_user.Requests.Remove(request_id);
                DocumentReference doc_ref = users_col.Document(Global.current_user.Id);
                Dictionary<string, object> update = new Dictionary<string, object>
                {
                    {"Requests", Global.current_user.Requests }
                };
                await doc_ref.UpdateAsync(update);
                LogHandler.Log(string.Format("Removed invalid request Id= {0}",request_id));
                return true;
            }
            catch (Exception err)
            {
                LogHandler.Log(err.Message);
            }
            return false;
        }

        //error codes
        //0 = no error
        //1 = database error
        //2 = user id invalid
        public async Task<int> SendRequest(string id)
        {
            int error_code = 0;
            try
            {
                DocumentSnapshot snapshot = await GetUserById(id);
                if (snapshot.Exists)
                {
                    Dictionary<string, object> user = snapshot.ToDictionary();

                    object requests_val;
                    List<string> requests = new List<string>();
                    if (user.TryGetValue("Requests",out requests_val))
                    {
                        //if user has requests array, set to the existing array
                        foreach (object request in (List<object>)requests_val)
                            requests.Add((string)request);     
                    }
                 
                    //add user id to list of requests
                    if (!requests.Contains(Global.current_user.Id))
                        requests.Add(Global.current_user.Id);

                    try
                    {
                        DocumentReference docRef = users_col.Document(id);
                        Dictionary<string, object> update = new Dictionary<string,object>
                        {
                            {"Requests",requests }
                        };
                        await docRef.UpdateAsync(update);

                        LogHandler.Log(string.Format("User {0} sent request to {1}",Global.current_user.Id,id));
                    }
                    catch (Exception err)
                    {
                        LogHandler.Log(err.Message);
                        error_code = 1;
                    }
                }
                else
                {
                    LogHandler.Log(string.Format("User {0} does not exist", id));
                    error_code = 2;
                }
            }
            catch(Exception err)
            {
                LogHandler.Log(err.Message);
                error_code = 1;
            }

            return error_code;
        }

        private async Task<QuerySnapshot> GetUserByEmail(string email)
        {
            Query query = users_col.WhereEqualTo("Email", email);
            try
            {
                QuerySnapshot users = await query.GetSnapshotAsync();
                return users;
            }
            catch(Exception err)
            {
                throw err;
            }
        }

        //error codes
        //0 = no error
        //1 = database insert error
        //2 = email has been used
        public async Task<int> Register(string email, string username, string password)
        {
            int error_code = 0;
            try
            {
                QuerySnapshot snapshot = await GetUserByEmail(email);
               
                if(snapshot.Count == 0)
                {
                    try
                    {
                        DocumentReference doc_ref = await users_col.AddAsync(new { Email = email, Username = username, Password = HashPassword(password) });
                        LogHandler.Log(string.Format("Add new user [ email= {0}, username= {1}, docRef = {2}]", email, username, doc_ref.Id));

                        Global.current_user.Id = doc_ref.Id;
                        Global.current_user.Name = username;
                        Global.current_user.Email = email;
                        Global.current_user.Requests = new List<string>();
                        Global.current_user.Friends = new List<string>();
                        Global.current_user.Chats = new List<DmChat>();
                    }
                    catch (Exception err)
                    {
                        LogHandler.Log(err.Message);
                        error_code = 1;
                    }
                }
                else
                {
                    LogHandler.Log("Email has been registered.");
                    error_code = 2;
                }
            }
            catch(Exception err)
            {
                LogHandler.Log(err.Message);
                error_code = 1;
            }

            return error_code;
        }

        private FirestoreChangeListener listener;

        //listen for realtime updates on user
        public void ListenForUpdates(string id)
        {
            DocumentReference doc_ref = users_col.Document(id);

            try
            {
                listener = doc_ref.Listen((Action<DocumentSnapshot>)(user_snapshot =>
                {
                    Dictionary<string, object> user = user_snapshot.ToDictionary();

                    object requests_val, friends_val;
                    if (user.TryGetValue("Requests", out requests_val) || user.TryGetValue("Friends",out friends_val))
                    {
                        //get current user's request list
                        if(user.TryGetValue("Requests", out requests_val))
                        {
                            List<string> requests = new List<string>();
                            foreach (object request in (List<object>)requests_val)
                                requests.Add((string)request);

                            Global.current_user.Requests = requests;
                            LogHandler.Log(string.Format("Retrieved {0} requests.", requests.Count));
                        }
                        //get current user's friends list
                        if(user.TryGetValue("Friends", out friends_val))
                        {
                            List<string> friends = new List<string>();
                            foreach (object friend in (List<object>)friends_val)
                                friends.Add((string)friend);

                            Global.current_user.Friends = friends;
                            LogHandler.Log(string.Format("Retrieved {0} friends.", friends.Count));
                        }

                        foreach (var observer in observers)
                        {
                            observer.OnNext(Global.current_user);
                        }
                    }
                }));
            }
            catch(Exception err)
            {
                LogHandler.Log(err.Message);
            }
        }

        public async void StopListeningUpdates()
        {
            await listener.StopAsync();
        }

        //error codes
        //0 = no error
        //1 = database error
        //2 = user does not exist
        //3 = wrong password
        public async Task<int> LogIn(string email, string password)
        {
            int error_code = 0;

            try
            {
                QuerySnapshot snapshot = await GetUserByEmail(email);

                if(snapshot.Count > 0)
                {
                    try
                    {
                        Dictionary<string, object> user = null;
                        string db_id = null;

                        foreach (DocumentSnapshot document in snapshot.Documents)
                        {
                            user = document.ToDictionary();
                            db_id = document.Id;
                        }

                        string db_username, db_password, db_image;
                        db_username = db_password = db_image = string.Empty;
                        List<string> requests = new List<string>();
                        List<string> friends = new List<string>();

                        foreach (KeyValuePair<string, object> info in user)
                        {
                            if (info.Key.Equals("Username"))
                                db_username = info.Value.ToString();
                            else if (info.Key.Equals("Password"))
                                db_password = info.Value.ToString();
                            else if (info.Key.Equals("Requests"))
                            {
                                foreach (object request in (List<object>)info.Value)
                                    requests.Add((string)request);
                            }
                            else if (info.Key.Equals("Friends"))
                            {
                                foreach (object friend in (List<object>)info.Value)
                                    friends.Add((string)friend);
                            }               
                            else if(info.Key.Equals("Profile"))
                                db_image = info.Value.ToString();
                        }
                        if (db_password.Equals(HashPassword(password)))
                        {
                            LogHandler.Log(string.Format("User: Id={0} signed in.", db_id));
                            Global.current_user.Id = db_id;
                            Global.current_user.Name = db_username;
                            Global.current_user.Email = email;
                            Global.current_user.Requests = requests;
                            Global.current_user.Friends = friends;
                            Global.current_user.Chats = new List<DmChat>();
                            Global.current_user.Image = db_image;
                        }
                        else
                        {
                            LogHandler.Log(string.Format("Wrong password entered for [Email = {0}]", email));
                            error_code = 3;
                        }
                    }
                    catch (Exception err)
                    {
                        LogHandler.Log(err.Message);
                        error_code = 1;
                    }
                }
                else
                {
                    LogHandler.Log("User does not exist.");
                    error_code = 2;
                }
            }
            catch(Exception err)
            {
                LogHandler.Log(err.Message);
                error_code = 1;
            }

            return error_code;
        }

        //error codes
        //0 = no error
        //1 = database error
        public async Task<int> AddFriend(string friend_id)
        {
            int error_code = 0;
            try
            {
                DocumentSnapshot snapshot = await GetUserById(Global.current_user.Id);

                Global.current_user.Friends.Add(friend_id);
                Global.current_user.Requests.Remove(friend_id);

                Dictionary<string, object> update = new Dictionary<string, object>
                {
                    {"Friends", Global.current_user.Friends},
                    {"Requests", Global.current_user.Requests }
                };
                try
                {
                    DocumentReference doc_ref = users_col.Document(Global.current_user.Id);
                    await doc_ref.UpdateAsync(update);

                    try
                    {
                        DocumentSnapshot friend_snapshot = await GetUserById(friend_id);
                        Dictionary<string, object> friend_info = friend_snapshot.ToDictionary();
                        List<string> friend_ids = new List<string>();
                        object friend_val;
                        if (friend_info.TryGetValue("Friends",out friend_val))
                        {
                            foreach (object id in (List<object>)friend_val)
                                friend_ids.Add((string)id);
                        }
                        friend_ids.Add(Global.current_user.Id);

                        await users_col.Document(friend_id).UpdateAsync(new Dictionary<string, object>
                        {
                            {"Friends",friend_ids }
                        });
                    }
                    catch(Exception err)
                    {
                        LogHandler.Log(err.Message);
                    }
                }
                catch (Exception err)
                {
                    LogHandler.Log(err.Message);
                    error_code = 1;
                }
            }
            catch (Exception err)
            {
                LogHandler.Log(err.Message);
                error_code = 1;
            }

            return error_code;
        }

        public IDisposable Subscribe(IObserver<User> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);

            return new Unsubscriber(observers, observer);
        }

        internal class Unsubscriber : IDisposable
        {
            private List<IObserver<User>> observers;
            private IObserver<User> current_observer;

            internal Unsubscriber(List<IObserver<User>> observers, IObserver<User> current_observer)
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
