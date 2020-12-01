using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WDD_A6_WebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            WebServerListener listener = new WebServerListener(@"C:\localWebSite", "192.168.0.10", 65000);

            listener.StartServer();


            
            

            


            //string contentPath = RequestURI.Replace(Host, RequestURI);
        }


    }
}
