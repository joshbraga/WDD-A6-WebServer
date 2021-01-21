/*
 *  FILE            :   WebServerListener.cs
 *  PROJECT         :   myOwnWebServer
 *  PROGRAMMER      :   Balazs Karner 8646201 & Josh Braga 5895818
 *  FIRST VERSION   :   2020-12-01
 */

/*
 *  NAME            :   WebServerListener
 *  PURPOSE         :   The purpose of this class is to operate as a TCP listener for
 *                      incoming connections and handle them by using another class to
 *                      interpret the incoming information, receiving a correct response
 *                      and sending it back.
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace myOwnWebServer
{
    class WebServerListener
    {
        public string WebRoot { get; private set; }
        public string IpAddress { get; private set; }
        public Int32 Port { get; private set; }



        public WebServerListener(string webRoot, string ipAddress, Int32 port)
        {
            WebRoot = webRoot;
            IpAddress = ipAddress;
            Port = port;
        }




        // METHOD           :   StartServer
        // DESCRIPTION      :   This method contains the code necessary to listen for
        //                      incoming connections, call a method to handle that data
        //                      and then communicate the response back to the client.
        //
        // PARAMETERS       :
        //  Nothing.
        //
        // RETURNS          :
        //  Nothing.
        //
        public void StartServer()
        {
            TcpListener server = null;
            try
            {
                Logger.CreateAndLog("[SERVER START]");

                // Set the TcpListener on port 13000.
                //Int32 port = 13000;
                //IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                Int32 port = Port;
                IPAddress localAddr = IPAddress.Parse(IpAddress);

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data



                // Enter the listening loop.
                while (true)
                {
                    Byte[] bytes = new Byte[8192];
                    String data = null;
                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();


                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();


                    int i = stream.Read(bytes, 0, bytes.Length);
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                    byte[] msg = ProcessRequest(data);

                    // Send back a response.
                    stream.Write(msg, 0, msg.Length);

                    //Shutdown and end connection
                    client.Close();

                }
            }
            catch (SocketException e)
            {
                Logger.Log("[EXCEPTION] - " + e);
                Console.WriteLine("SocketException: " + e);
            }
            catch (Exception e)
            {
                Logger.Log("[EXCEPTION] - " + e);
                Console.WriteLine("Exception: " + e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
                Logger.Log("[SERVER SHUTDOWN]");
            }

        }



        // METHOD           :   ProcessRequest
        // DESCRIPTION      :   The worker method for the server, calls the appropriate methods
        //                      from another class to process the request and then calls
        //                      the response methods.
        //
        // PARAMETERS       :
        //  string request  :   The request line from the client containing the command
        //
        // RETURNS          :
        //  byte[]          :   Contains the response message to send back to server
        //
        public byte[] ProcessRequest(string request)
        {
            string serverInfo = IpAddress + ":" + Port.ToString();
            HttpHandler httpHandler = new HttpHandler(WebRoot, serverInfo);
            byte[] response;

            Boolean isValid;

            isValid = httpHandler.ValidateAndSetRequest(request, serverInfo);
            response = httpHandler.CreateResponse(isValid);

            return response;
        }
    }
}