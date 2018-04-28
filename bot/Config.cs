using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TAGGER
{
    internal class Config
    {
        public static bool Exists()
        {
            return File.Exists("config.txt");
        }

        public static string GetConfig(string entry)
        {
            if (!Exists()) return null;
            var dict = File.ReadLines("config.txt").Select(l => l.Split(':')).ToDictionary(split => split.First(), split => split.Last());
            return dict[entry].Trim('"');
        }

        public static void Make()
        {
            var file = new StreamWriter("config.txt");
            file.WriteLine("DiscordToken:\"\"");
            file.WriteLine("OsuIRCToken:\"\"");
            file.WriteLine("WebAPIKey:\"\"");
            file.WriteLine("OsuAPIKey:\"\"");
            file.WriteLine("UseTagAPI:\"true\"");
            file.Close();
        }
    }
}
