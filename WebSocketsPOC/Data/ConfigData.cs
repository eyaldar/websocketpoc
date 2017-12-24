using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketsPOC.Data
{
    public class ConfigData
    {
        private ConfigData() { }

        public string ClientName { get; set; }
        public int ZoomChangeInterval { get; set; }
        public string Hostname { get; set; }
        public int Port { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public WebSocketClientType ClientType { get; set; }

        private static Lazy<ConfigData> instance = new Lazy<ConfigData>(
            () =>
            {
                using (StreamReader sr = new StreamReader(Constants.ConfigFileName))
                {
                    string json = sr.ReadToEnd();
                    var configData = JsonConvert.DeserializeObject<ConfigData>(json);
                    return configData;
                }
            },
            true);

        public static ConfigData Instance
        {
            get
            {
                return instance.Value;
            }
        }
    }
}
