using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDD_A6_WebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            WebServerListener server = new WebServerListener(@"C:\", "127.0.0.1", 13000);
            server.StartServer();
        }


    }
}
