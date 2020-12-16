using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Classes
{
    class Friend
    {
        private string image = null;
        private string name;
        private string id;

        public string Image { get => image; set => image = value; }
        public string Name { get => name; set => name = value; }
        public string Id { get => id; set => id = value; }
    }
}
