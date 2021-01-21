/*
* FILE          : HttpHandler.cs
* PROJECT       : myOwnWebServer
* PROGRAMMER    : Balazs Karner 8646201 & Josh Braga 5895818
* FIRST VERSION : 2020-12-01
*/


/*
 *  NAME            :   HttpHandler
 *  PURPOSE         :   The purpose of this class is to contain all the attributes and methods
*                       necessary to process HTTP requests and build a response string for
*                       said request.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;



namespace myOwnWebServer
{
    public class HttpHandler
    {
        //Class attributes
        public string ServerRoot { get; private set; }      //web root
        public string Method { get; private set; }          //method (ie. GET, POST)
        public string RequestURI { get; private set; }      //URI to resource location
        public string HTTPVersion { get; private set; }     //http version
        public string ContentType { get; private set; }     //content-type for response
        public string ServerName { get; private set; }      //name of server
        public int StatusCode { get; private set; }         //status code



        private const int ALL_OKAY = 200;                   //Everything is okay. No validation errors
        private const int BAD_REQUEST = 400;                //Extension or HTTP Structure invalid
        private const int HTTP_VERSION_NOT_SUPPORTED = 505; //Version 1.1 was not used
        private const int NOT_IMPLEMENTED = 501;            //GET was not used
        private const int NOT_FOUND = 404;                  //Image, or file was not found (Doesn't exist)
        private const int FORBIDDEN = 403;                  //Access not allowed
        private const int UNSUPPORTED_MEDIA_TYPE = 415;

        //compansion strings to go with their matching status code
        private const string ALL_OKAY_MESSAGE = "OK";
        private const string BAD_REQUEST_MESSAGE = "Bad Request";
        private const string HTTP_VERSION_NOT_SUPPORTED_MESSAGE = "HTTP Version Not Supported";
        private const string NOT_IMPLEMENTED_MESSAGE = "Not Implemented";
        private const string NOT_FOUND_MESSAGE = "Not Found";
        private const string FORBIDDEN_MESSAGE = "Forbidden";
        private const string UNSUPPORTED_MEDIA_TYPE_MESSAGE = "Unsupported Media Type";

        //misc consts
        private const string VALID_HTTP_VERSION = @"HTTP/1.1";

        //contains associated error for the status code
        private Dictionary<int, string> statusPairs;




        // METHOD           :   HttpHandler
        // DESCRIPTION      :   Constructor for HttpHandler class, sets a dictionary
        //                      containing all the status code and error message pairs
        //                      as well as setting serverRoot and serverName from passed
        //                      in parameters, removing a trailing "\" if it exists to
        //                      keep compatibility with code.
        //
        // PARAMETERS       :
        //  string serverRoot   :   contains the server root information for server
        //  string serverName   :   contains name of server
        //        
        public HttpHandler(string serverRoot, string serverName)
        {
            statusPairs = new Dictionary<int, string>
            {
                {ALL_OKAY, ALL_OKAY_MESSAGE },
                {BAD_REQUEST, BAD_REQUEST_MESSAGE },
                {HTTP_VERSION_NOT_SUPPORTED, HTTP_VERSION_NOT_SUPPORTED_MESSAGE },
                {NOT_IMPLEMENTED, NOT_IMPLEMENTED_MESSAGE },
                {NOT_FOUND, NOT_FOUND_MESSAGE },
                {FORBIDDEN, FORBIDDEN_MESSAGE },
                {UNSUPPORTED_MEDIA_TYPE, UNSUPPORTED_MEDIA_TYPE_MESSAGE }
            };

            ServerName = serverName;
            ServerRoot = serverRoot;
            if (ServerRoot.EndsWith(@"\"))
            {
                ServerRoot = ServerRoot.TrimEnd('\\');
            }
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

            //Split request string by spaces and newlines. GET | HTTP://.... | HTTP/Version
            //Protect against empty requests from badly behaved browsers
            if (request != String.Empty)
            {
                string[] split = request.Split(' ', '\n');

                //trimming carriage returns if they exist (ie. windows style \r\n not caught by split)
                int i = 0;
                foreach (var s in split)
                {
                    split[i] = s.TrimEnd('\r');
                    ++i;
                }

                if (split.Length >= 3)
                {
                    Method = split[0];
                    RequestURI = split[1];
                    HTTPVersion = split[2];




                    //Validate request method to ensure GET was used
                    if (ValidateRequestMethod(split[0]) != true)
                    {
                        StatusCode = NOT_IMPLEMENTED;
                    }
                    //validate structure of the requestURI
                    else if (ValidateURIStructure(split[1]) != true)
                    {
                        StatusCode = BAD_REQUEST;
                    }
                    //http version validation
                    else if (ValidateHTTPVersion(split[2]) != true)
                    {
                        StatusCode = HTTP_VERSION_NOT_SUPPORTED;
                    }
                    //checking file extension to see if supported
                    //404 NOT FOUND is used because it seems to be the defacto code
                    //when nothing else is appropriate, and IIS uses it for this too
                    else if (ValidateFileType(split[1]) != true)
                    {
                        StatusCode = NOT_FOUND;
                    }
                    else if (WithinWebRoot(split[1]) != true)
                    {
                        StatusCode = FORBIDDEN;
                    }
                    //checking if file exists
                    else if (ValidateFileExists(split[1]) != true)
                    {
                        StatusCode = NOT_FOUND;
                    }
                    else
                    {
                        StatusCode = ALL_OKAY;
                        isValid = true;
                    }
                }
                else
                {
                    StatusCode = BAD_REQUEST;
                }
            }
            else
            {
                StatusCode = BAD_REQUEST;
            }


            //Log incoming request
            string message = "[REQUEST] - " + Method + " " + RequestURI;
            Logger.Log(message);


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
        *       Validates that the URI is of the correct structure, containing a leading forward slash
        * PARAMETERS  :
        *       string : request (String with the request string)
        * RETURNS     : 
        *       bool : isValid (true if structure is valid)
        *       bool : isValid (false if structure is invalid)
        */
        private bool ValidateURIStructure(string request)
        {
            bool isValid = false;

            if (request.StartsWith(@"/"))
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

            if (version == VALID_HTTP_VERSION)
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
        *       bool : true if it matches one of the valid regex statements, false otherwise
        */
        private bool ValidateFileType(string request)
        {
            bool isValid = false;

            string extension = Path.GetExtension(request);

            //Regex pattern for accepted file types            
            Regex plainFileTypePattern = new Regex("(.txt)");
            Regex htmlFileTypePattern = new Regex("(.html|.htm|.htmls|.htt|.acgi)");
            Regex jpegFileTypePattern = new Regex("(.jfif|.jfif-tbnl|.jpe|.jpeg|.jpg)");
            Regex gifFileTypePattern = new Regex("(.gif)");

            //If a match is found, then isValid is switched to true           

            if (htmlFileTypePattern.IsMatch(extension))
            {
                ContentType = "text/html";
                isValid = true;
            }
            else if (jpegFileTypePattern.IsMatch(extension))
            {
                ContentType = "image/jpeg";
                isValid = true;
            }
            else if (gifFileTypePattern.IsMatch(extension))
            {
                ContentType = "image/gif";
                isValid = true;
            }
            else if (plainFileTypePattern.IsMatch(extension))
            {
                ContentType = "text/plain";
                isValid = true;
            }


            return isValid;
        }




        // METHOD           :   WithinWebRoot
        // DESCRIPTION      :   This method checks if the request URI is attempting 
        //                      a transversal attack and going above the web root.
        //                      Compares the absolute paths of the request to make sure
        //                      it starts with the webroot otherwise it's attempting to
        //                      access above it.
        //
        // PARAMETERS       :
        //  string request  :   contains the URI to be validated
        //
        // RETURNS          :
        //  bool            :   true if it is valid and within root, false otherwise
        //
        private bool WithinWebRoot(string request)
        {
            bool isValid = true;

            string fileName = request.Replace("/", @"\");
            string filePath = ServerRoot + fileName;

            //get full absolute path
            string pathCheck = Path.GetFullPath(filePath);

            if (!pathCheck.StartsWith(ServerRoot))
            {
                isValid = false;
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

            string fileName = request.Replace("/", @"\");

            //Then put the file path together with the server root and check if the file exists
            string filePath = ServerRoot + fileName;

            //If file exists then change isValid to true
            if (File.Exists(filePath) == true)
            {
                isValid = true;
            }

            return isValid;
        }




        // METHOD           :   CreateResponse
        // DESCRIPTION      :   This function assembles the response message to send back to the
        //                      client who sent the request.
        //
        // PARAMETERS       :
        //  Boolean isValid :   Indicates whether the request is an error response or good
        //                      response.
        //
        // RETURNS          :
        //  byte[]          :   Returns a byte array containing the data to send back to client.
        //   
        public byte[] CreateResponse(Boolean isValid)
        {
            string response = HTTPVersion + " " + StatusCode + " " + statusPairs[StatusCode] + "\r\n";
            string contentPath = RequestURI;
            string data;
            string date = DateTime.UtcNow.ToString("ddd, dd MMM yyyy HH:mm:ss");
            byte[] byteData;
            byte[] byteResponse = null;
            int contentLength = 0;

            //setting content type to text/html if invalid so a message page can be sent
            //if desired
            if (!isValid)
            {
                ContentType = "text/html";
            }

            //Assembling response headers for date, content-type and server
            response += "Date: " + date + "GMT\r\n";
            response += "Content-Type: " + ContentType + "\r\n";
            response += "Server: " + ServerName + "\r\n";

            //sending a response to a valid request
            if (isValid)
            {
                //replacing forward slashes with backslashes for file paths
                contentPath = contentPath.Replace("/", @"\");

                //for reading the file as text
                if (ContentType.StartsWith("text"))
                {
                    data = File.ReadAllText(ServerRoot + contentPath);
                    contentLength = data.Length;
                    response += "Content-Length: " + contentLength + "\r\n";
                    response += "\r\n" + data;
                    byteResponse = Encoding.ASCII.GetBytes(response);
                }

                //for reading the file as bytes, for images and other non text content
                else if (ContentType.StartsWith("image"))
                {
                    byteData = File.ReadAllBytes(ServerRoot + contentPath);
                    contentLength = byteData.Length;
                    response += "Content-Length: " + contentLength + "\r\n";
                    response += "\r\n";
                    byte[] temp = Encoding.ASCII.GetBytes(response);

                    byte[] combined = new byte[temp.Length + byteData.Length];
                    temp.CopyTo(combined, 0);
                    byteData.CopyTo(combined, temp.Length);
                    byteResponse = combined;

                }

                Logger.Log("[RESPONSE] - content-type: " + ContentType + ", content-length: " +
                            contentLength + ", server: " + ServerName + ", date: " + date);
            }
            //responding with an error code
            else
            {
                string errorMessage = "<h1>" + StatusCode + ": " + statusPairs[StatusCode] + "</h1>";
                contentLength = errorMessage.Length;
                response += "Content-Length: " + contentLength + "\r\n\r\n";
                response += errorMessage;
                byteResponse = Encoding.ASCII.GetBytes(response);

                Logger.Log("[RESPONSE] - status: " + StatusCode);
            }


            return byteResponse;
        }

    }
}