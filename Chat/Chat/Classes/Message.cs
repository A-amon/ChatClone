using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Classes
{
    public class Message
    {
        private DateTime datetime;
        private string senderId, text, messageId,name,image;

        public DateTime Datetime { get => datetime; set => datetime = value; }
        public string SenderId { get => senderId; set => senderId = value; }
        public string Text { get => text; set => text = value; }
        public string MessageId { get => messageId; set => messageId = value; }
        public string Image { get => image; set => image = value; }
        public string Name { get => name; set => name = value; }
    }
}
