using System;
using System.IO;


namespace MyOnlineShop.Services
{
    public class Logger
    {
        public static void LoggerFunc(string user, string type, string func)
        {
            string path = @"c:\\Log_File.txt";
            string Log = "User : " + user + " Function : " + func + " Type : " + type;

            StreamWriter LogWriter = new StreamWriter(path, true);

            LogWriter.WriteLine(Log);
            LogWriter.Close();
            
        }
    }
}
