using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Classes
{
    public class DmChat
    {
        private string friend_id, chat_id;

        public string FriendId { get => friend_id; set => friend_id = value; }
        public string ChatId { get => chat_id; set => chat_id = value; }
    }
}
