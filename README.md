# WebServer

This is a very simple web server that can serve basic text content with jpeg and gif support. A simple hello world html file will
suffice as a test file.

To use:

Run compiled exe from command line with 3 command line args -webRoot= -webIP= and -webPort= .
-webRoot is the location of your web root directory, where you keep your files to serve, -webIP will be the IP
the web server is listening on and -webPort is the port.

Example:

myOwnWebServer.exe -webRoot=C:\localwebsite -webIP=127.0.0.1 -webPort=65000

The above will have you running the web server listening on localhost on port 65000 with the web root directory
pointing to your C drive in the folder localwebsite.
