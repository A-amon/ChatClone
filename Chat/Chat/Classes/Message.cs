using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Classes
{
    class Message
    {
        private DateTime datetime;
        private string senderId, text;

        public DateTime Datetime { get => datetime; set => datetime = value; }
        public string SenderId { get => senderId; set => senderId = value; }
        public string Text { get => text; set => text = value; }
    }
}
