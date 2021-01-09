using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Classes
{
    public class User:INotifyPropertyChanged
    {
        private string name, email, image, id;
        private List<string> requests;
        private List<string> friends;
        private List<DmChat> chats;

        public string Name { get => name; set => name = value; }
        public string Email { get => email; set => email = value; }
        public string Image {
            get { return image; }
            set {
                image = value;
                PropertyChanged?.Invoke(this,new PropertyChangedEventArgs("Image"));
            }
        }
        public string Id { get => id; set => id = value; }
        public List<string> Requests { get => requests; set => requests = value; }
        public List<string> Friends { get => friends; set => friends = value; }
        public List<DmChat> Chats { get => chats; set => chats = value; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
