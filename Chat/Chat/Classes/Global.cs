using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Classes
{
    class Global
    {
        public static FirestoreDb firestore = FirestoreDb.Create("ferrous-destiny-298611");

        public static User current_user = new User();
    }
}
