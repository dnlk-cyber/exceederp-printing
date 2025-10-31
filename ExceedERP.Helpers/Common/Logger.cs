using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ExceedERP.Helpers.Common
{
    public class Logger
    {
        
        public static void Trace(string message)
        {
            //var logger = NLog.LogManager.GetLogger("Logger1");
            //logger.Trace("|TRACE|" + message);
        }
        public static void Debug(string message)
        {
            //var logger = NLog.LogManager.GetLogger("Logger1");
            //logger.Debug("|DEBUG|" + message);
        }
        public static void Info(string message)
        {
            //var logger = NLog.LogManager.GetLogger("Logger1");
            //logger.Info("|INFO|" + message);
        }
        public static void Warn(string message)
        {
            //var logger = NLog.LogManager.GetLogger("Logger1");
            //logger.Warn("|WARN|" + message);
        }

        public static void Error(string message)
        {
            //var logger = NLog.LogManager.GetLogger("Logger1");
            //logger.Error("|ERROR|" + message);
        }
        public static void Fatal(string message)
        {
            //var logger = NLog.LogManager.GetLogger("Logger1");
            //logger.Fatal("|FATAL|" + message);
        }
    }
}
