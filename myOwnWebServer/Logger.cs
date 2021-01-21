/*
 *  FILE            :   Logger.cs
 *  PROJECT         :   myOwnWebServer
 *  PROGRAMMER      :   Balazs Karner 8646201 & Josh Braga 5895818
 *  FIRST VERSION   :   2020-12-01
 */

/*
 *  NAME            :   Logger
 *  PURPOSE         :   The purpose of this class is to log  passed in messages to the 
 *                      specified file at the specified location with a date and time stamp.
 */


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myOwnWebServer
{
    public static class Logger
    {
        //lock to prevent race conditions when writing to file from other threads
        private static readonly object loggerLock = new object();
        private const string YEAR_MONTH_DAY_TWENTYFOUR_HOUR_FORMAT = "yyyy-MM-dd HH:mm:ss";
        private const string LOG_FILE_NAME = "myOwnWebServer.log";



        // METHOD               :   Log
        // DESCRIPTION          :   Takes in a string and outputs it to a file indicated by
        //                          the LOG_FILE_NAME constant prefixed with a time stamp.
        //
        // PARAMETERS           :
        //   string message     :   contains the data to write to log
        //
        // RETURNS              :
        //      Nothing.
        public static void Log(string message)
        {
            //filepath to where the exe is located + log file path
            string path = AppDomain.CurrentDomain.BaseDirectory + LOG_FILE_NAME;

            //string to write to log
            string toWrite = DateTime.Now.ToString(YEAR_MONTH_DAY_TWENTYFOUR_HOUR_FORMAT) + " " + message + "\n";

            //for multithreaded applications
            lock (loggerLock)
            {
                try
                {
                    File.AppendAllText(path, toWrite);
                }
                catch (Exception e)
                {
                    Console.WriteLine("EXCEPTION: {0}", e);
                }

            }
        }


        // METHOD           :   CreateAndLog
        // DESCRIPTION      :   This method is used when it is desirable to overwrite a
        //                      potentially existing file and create a new one containing
        //                      the passed in message
        //
        // PARAMETERS       :
        //  string message  :   contains the entry for the log file to be written
        //
        // RETURNS          :
        //  Nothing.
        //
        public static void CreateAndLog(string message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + LOG_FILE_NAME;
            FileStream stream = File.Create(path);
            stream.Close();
            Logger.Log(message);
        }
    }
}
