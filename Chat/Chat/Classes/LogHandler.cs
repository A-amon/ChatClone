using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Classes
{
    class LogHandler
    {
        public static void Log(string message)
        {
            File.AppendAllText("log.txt", DateTime.Now + " " + message + Environment.NewLine);
        }
    }
}
