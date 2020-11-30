/*
 *  FILE            :   HttpHandler.cs
 *  PROJECT         :   WDD-A6-WebServer
 *  PROGRAMMER      :   Josh Braga and Balazs Karner
 *  FIRST VERSION   :   2020-11-30
 */

/*
 *  NAME            :   HttpHandler
 *  PURPOSE         :   Processing HTTP requests and building a response to 
 *                      be sent back to the client.
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDD_A6_WebServer
{
    public class HttpHandler
    {
        public string Method { get; private set; }
        public string RequestURI { get; private set; }
        public string HTTPVersion { get; private set; }
        public int StatusCode { get; private set; }




        // METHOD               :   ValidateAndSetRequest
        // DESCRIPTION          :   This method does the initial validation of the 
        //                          HTTP request to see if its structure is valid.
        //
        // PARAMETERS           :
        //  string request      :   contains the request line from the client
        //  string serverInfo   :   contains the ip address and port of the server
        //                          to validate against the URI portion of request
        //
        // RETURNS              :
        //  Boolean             :   returns true if request is valid, false otherwise
        //
        public Boolean ValidateAndSetRequest(string request, string serverInfo)
        {
            Boolean isValid = false;

            string[] split = request.Split(' ');

            if (split[0] == "GET")
            {
                if (split[1].StartsWith(@"http://" + serverInfo + @"/"))
                {
                    if (split[2] == @"HTTP/1.1")
                    {
                        Method = split[0];
                        RequestURI = split[1];
                        HTTPVersion = split[2];
                        isValid = true;
                    }
                }
            }

            return isValid;
        }

        
    }
}
