/*
* FILE          : HttpHandler.cs
* PROJECT       : WDD-A6-WebServer
* PROGRAMMER    : Balazs Karner 8646201 & Josh Braga 5895818
* FIRST VERSION : 11/29/2020
* DESCRIPTION   :
*       The purpose of this file is to 
*/



using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;



namespace WDD_A6_WebServer
{
    /*
     *  NAME    : HttpHandler
     *  PURPOSE : 
     *      Processing HTTP requests and building a response to 
     *      be sent back to the client.
     */
    public class HttpHandler
    {
        public string ServerRoot { get; private set; }
        public string Method { get; private set; }
        public string RequestURI { get; private set; }
        public string Host { get; private set; }
        public string HTTPVersion { get; private set; }
        public string ContentType { get; private set; }
        public string ServerName { get; private set; }
        public int StatusCode { get; private set; }
        public string StatusMessage { get; private set; }


        private const int ALL_OKAY = 200;                   //
        private const int BAD_REQUEST = 400;                //
        private const int HTTP_VERSION_NOT_SUPPORTED = 505; //
        private const int METHOD_NOT_ALLOWED = 405;         //
        private const int NOT_FOUND = 404;                  //


        public HttpHandler()
        {

        }


        public HttpHandler(string serverRoot, string serverName)
        {
            ServerName = serverName;
            ServerRoot = serverRoot;
        }

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

            //Split request string by spaces. GET | HTTP://.... | HTTP/Version
            string[] split = request.Split(' ');

            //Validate request method to ensure GET was used
            if (ValidateRequestMethod(split[0]) != true)
            {
                StatusCode = METHOD_NOT_ALLOWED;
            }
            else if (ValidateHTTPStructure(split[1], serverInfo) != true)
            {
                StatusCode = BAD_REQUEST;
            }
            else if (ValidateHTTPVersion(split[2]) != true)
            {
                StatusCode = HTTP_VERSION_NOT_SUPPORTED;
            }
            else if (ValidateFileType(split[1]) != true)
            {
                StatusCode = BAD_REQUEST;
            }
            else if (ValidateFileExists(split[1]) != true)
            {
                StatusCode = NOT_FOUND;
            }
            else
            {
                Method = split[0];
                RequestURI = split[1];
                HTTPVersion = split[2];
                StatusCode = ALL_OKAY;
                isValid = true;
            }

            //If the header is invalid in any way, log it to the log fil
            if (isValid == false)
            {
                string message = "[REQUEST] " + Method + " method for " + RequestURI;
                Logger.Log(message);
            }

            //Build Response somewhere?
            return isValid;
        }

        /* 
        * FUNCTION    : ValidateRequestMethod()
        * DESCRIPTION :
        *       This method validates whether the request method int he HTTP request is GET or not
        * PARAMETERS  :
        *       string : requestMethod (String with the request method of GET or POST)
        * RETURNS     : 
        *       bool : isValid (true if request method is GET)
        *       bool : isValid (false if request method is not GET)
        */
        private bool ValidateRequestMethod(string requestMethod)
        {
            bool isValid = false;

            if (requestMethod == "GET")
            {
                isValid = true;
            }

            return isValid;
        }

        /* 
        * FUNCTION    : ValidateHTTPStructure()
        * DESCRIPTION :
        *       This method validates the structure of the HTTP request. If it contains http instead of https, the serverInfo, etc.
        * PARAMETERS  :
        *       string : request (String with the request string)
        *       string : serverInfo (String with host and port)
        * RETURNS     : 
        *       bool : isValid (true if structure is valid)
        *       bool : isValid (false if structure is invalid)
        */
        private bool ValidateHTTPStructure(string request, string serverInfo)
        {
            bool isValid = false;

            if (request.StartsWith(@"http://" + serverInfo + @"/"))
            {
                isValid = true;
            }

            return isValid;
        }

        /* 
        * FUNCTION    : ValidateHTTPVersion()
        * DESCRIPTION :
        *       This method validates the HTTP version and ensure it is version 1.1
        * PARAMETERS  :
        *       string : version (String with HTTP version)
        * RETURNS     : 
        *       bool : isValid (true if version is 1.1)
        *       bool : isValid (false if version is not 1.1)
        */
        private bool ValidateHTTPVersion(string version)
        {
            bool isValid = false;

            if (version == @"HTTP/1.1")
            {
                isValid = true;
            }

            return isValid;
        }

        /* 
        * FUNCTION    : ValidateFileType()
        * DESCRIPTION :
        *       This method validates the file extension in the request
        * PARAMETERS  :
        *       string : request (String with HTTP request)
        * RETURNS     : 
        *       bool : isValid (true if extension is html, htm, jpg, jpeg, txt, or gif)
        *       bool : isValid (false if extension is not html, htm, jpg, jpeg, txt, or gif)
        */
        private bool ValidateFileType(string request)
        {
            bool isValid = false;

            string extension =  Path.GetExtension(request);

            //Regex pattern for accepted file types
            Regex fileTypePattern = new Regex("(.txt|.html|.htm|.jpg|.jpeg|.gif|.aip|.acgi|.htlms|.htx|.jfif|.jfif-tbnl|.jpe)");

            //If a match is found, then isValid is switched to true
            if (fileTypePattern.IsMatch(extension) == true)
            {
                isValid = true;
            } 

            return isValid;
        }

        /* 
        * FUNCTION    : ValidateFileExists()
        * DESCRIPTION :
        *       This method validates if the file exists
        * PARAMETERS  :
        *       string : request (String with HTTP request)
        * RETURNS     : 
        *       bool : isValid (true if file exists)
        *       bool : isValid (false if file does not exist)
        */
        private bool ValidateFileExists(string request)
        {
            bool isValid = false;

            string fileName = Path.GetFileName(request);
            
            //DEBUG
            //MAKE SURE TO ACTUALLY FIGURE OUT FILE PATHS

            //If file exists then change isValid to true
            if (File.Exists(fileName) == true)
            {
                isValid = true;
            }

            return isValid;
        }



        public string CreateResponse()
        {
            string response = HTTPVersion + StatusCode + "\n";
            //string contentPath = RequestURI.Re

            if (!StatusCode.ToString().StartsWith("4") || !StatusCode.ToString().StartsWith("5"))
            {
                

                if (ContentType.StartsWith("text"))
                {
                    //string data = File.ReadAllText(ServerRoot + contentPath);
                }
                else
                {
                    //byte[] data = File.ReadAllBytes(ServerRoot + contentPath);
                }

                response += "Content-Type: " + ContentType + "\n";
                response += "Content-Length: ";
            }

            return response;
        }

    }
}