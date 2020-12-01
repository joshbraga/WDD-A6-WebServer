using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WDD_A6_WebServer;

namespace WDD_A6_UnitTests
{
    [TestClass]
    public class HttpHandlerUnitTests
    {
        
        #region ValidateAndSetRequest

        [TestMethod]
        public void ValidateAndSetRequest_withValidParameters()
        {
            string request = @"GET http://localhost:13000/index.html HTTP/1.1";
            string hostData = "localhost:13000";

            HttpHandler httpHandler = new HttpHandler();
            Boolean response = httpHandler.ValidateAndSetRequest(request, hostData);

            Assert.IsTrue(response);
        }

        [TestMethod]
        public void ValidateAndSetRequest_withInvalidMethod()
        {
            string request = @"POST https://localhost:13000/index.html HTTP/2";
            string hostData = "localhost:13000";

            HttpHandler httpHandler = new HttpHandler();
            Boolean response = httpHandler.ValidateAndSetRequest(request, hostData);

            Assert.IsFalse(response);
        }


        [TestMethod]
        public void ValidateAndSetRequest_withInvalidHostname()
        {
            string request = @"GET http://192.168.0.10:13000/index.html HTTP/1.1";
            string hostData = "localhost:13000";

            HttpHandler httpHandler = new HttpHandler();
            Boolean response = httpHandler.ValidateAndSetRequest(request, hostData);

            Assert.IsFalse(response);
        }


        [TestMethod]
        public void ValidateAndSetRequest_withInvalidHeaderHttps()
        {
            string request = @"GET https://localhost:13000/index.html HTTP/1.1";
            string hostData = "localhost:13000";

            HttpHandler httpHandler = new HttpHandler();
            Boolean response = httpHandler.ValidateAndSetRequest(request, hostData);

            Assert.IsFalse(response);
        }


        [TestMethod]
        public void ValidateAndSetRequest_withInvalidHeaderHttpVersion()
        {
            string request = @"GET https://localhost:13000/index.html HTTP/2";
            string hostData = "localhost:13000";

            HttpHandler httpHandler = new HttpHandler();
            Boolean response = httpHandler.ValidateAndSetRequest(request, hostData);

            Assert.IsFalse(response);
        }
        #endregion

    }
}