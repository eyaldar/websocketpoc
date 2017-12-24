using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketsPOC.Data
{
    public static class Constants
    {
        public const string ConfigFileName = "config.json";
        public static readonly string ConfigPath = Path.Combine(Environment.CurrentDirectory, ConfigFileName);
        public const string DeepStreamDefaultUsername = "userA";
        public const string DeepStreamDefaultPassword = "password";
    }
}
