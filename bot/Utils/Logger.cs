using System;
using System.Threading.Tasks;
using System.IO;

namespace TAGGER.Utils
{
    internal class Logger
    {
        public static void Exception(string message)
        {
            if (!Directory.Exists("Logs"))
                Directory.CreateDirectory("Logs");

            var log = new StreamWriter("Logs/Exceptions.txt");
            
            log.WriteLine($"{DateTime.Now}: {message}");
            log.Close();
        }

        public static void Warning(string message)
        {
            if (!Directory.Exists("Logs"))
                Directory.CreateDirectory("Logs");

            var log = new StreamWriter("Logs/Warnings.txt");

            log.WriteLine($"{DateTime.Now}: {message}");
            log.Close();
        }

        public static void Verbose(string message)
        {
            if (!Directory.Exists("Logs"))
                Directory.CreateDirectory("Logs");

            var log = new StreamWriter("Logs/Verbose.txt");

            log.WriteLine($"{DateTime.Now}: {message}");
            log.Close();
        }
    }
}
