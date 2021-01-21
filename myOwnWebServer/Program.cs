/*
 *  FILE            :   Program.cs
 *  PROJECT         :   myOwnWebServer
 *  PROGRAMMER      :   Balazs Karner 8646201 & Josh Braga 5895818
 *  FIRST VERSION   :   2020-12-01
 *  DESCRIPTION     :   Serves as the main() entry point for the myOwnWebServer project.
 *                      Checks if there are appropriate command line arguments and then
 *                      runs the server.
 * 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace myOwnWebServer
{
    class Program
    {
        //constants
        const int VALID_NUMBER_ARGS = 3;
        const string WEB_ROOT = "-webRoot=";
        const string WEB_IP = "-webIP=";
        const string WEB_PORT = "-webPort=";

        static void Main(string[] args)
        {
            string webRoot = String.Empty;
            string ipAddress = String.Empty;
            string temp;
            Int32 port = 0;
            Boolean validArgs = true;


            //check if right number of mandatory arguments
            if (args.Length != VALID_NUMBER_ARGS)
            {
                Console.WriteLine("Invalid number of arguments\n");
                validArgs = false;
            }
            else
            {
                //iterate through args
                foreach (var s in args)
                {
                    if (s.StartsWith(WEB_ROOT))
                    {
                        webRoot = s.Replace(WEB_ROOT, String.Empty);
                    }
                    else if (s.StartsWith(WEB_IP))
                    {
                        ipAddress = s.Replace(WEB_IP, String.Empty);

                        //sets invalid if IP can't be parsed correctly
                        if (!IPAddress.TryParse(ipAddress, out IPAddress ipTemp))
                        {
                            Console.WriteLine("Invalid IP\n");
                            validArgs = false;
                        }
                    }
                    else if (s.StartsWith(WEB_PORT))
                    {
                        temp = s.Replace(WEB_PORT, String.Empty);

                        //sets invalid if port can't be parsed correctly
                        if (!Int32.TryParse(temp, out port))
                        {
                            Console.WriteLine("Invalid Port\n");
                            validArgs = false;
                        }
                    }
                    else
                    {
                        validArgs = false;
                        Console.WriteLine("Invalid arguments\n");
                    }
                }
            }

            //if all arguments valid start server
            if (validArgs)
            {
                WebServerListener listener = new WebServerListener(webRoot, ipAddress, port);
                listener.StartServer();
            }
        }
    }
}
