using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Classes
{
    class ChatGroup
    {
        private string image = null;
        private string id;
        private string type;

        public string Id { get => id; set => id = value; }
        public string Type { get => type; set => type = value; }
        public string Image { get => image; set => image = value; }
    }
}
