using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDD_A6_WebServer
{
    class RequestData
    {
        public string Method { get; set; }
        public string RequestURI { get; set; }
        public string HTTPVersion { get; set; }

    }
}
