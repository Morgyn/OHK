using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;


namespace OHK
{
    class Configuration
    {
        private const string ConfigFolder = "Config";
        private const string ConfigFile = "config.json";
        public static OHKConfig OHK;

        static Configuration()
        {

            DebugLogForm.Instance.Log(string.Format("Checking for config at: {0}/{1}", ConfigFolder, ConfigFile));
            if (!Directory.Exists(ConfigFolder))
            {
                Directory.CreateDirectory(ConfigFolder);
                DebugLogForm.Instance.Log("Config directory doesn't exist. Creating.");
            }

            if (!File.Exists(ConfigFolder + "/" + ConfigFile))
            {
                OHK = new OHKConfig();
                string json = JsonConvert.SerializeObject(OHK, Formatting.Indented);
                File.WriteAllText(ConfigFolder + "/" + ConfigFile, json);
            }
            else
            {
                string json = File.ReadAllText(ConfigFolder + "/" + ConfigFile);
                OHK = JsonConvert.DeserializeObject<OHKConfig>(json);
            }
        }

        public class OHKConfig
        {
            [JsonProperty("IP Adress. Default: 127.0.0.1")]
            public string Ip;

            [JsonProperty("Port. Default: 4444")]
            public string Port;

            [JsonProperty("Password from your OBS websocket plugin")]
            public string Password;

            [JsonProperty("Key to pause the application")]
            public Keys PauseKey;

            [JsonProperty("Delay after releasing key")]
            public int Delay;

            [JsonProperty("Delay before attempting to reconnect")]
            public int ReconnectDelay;

            public Dictionary<Keys, string> KeysSetup;

            public OHKConfig()
            {
                Ip = "127.0.0.1";
                Port = "4444";
                Password = "test";
                PauseKey = Keys.F6;
                Delay = 300;
                ReconnectDelay = 5;
                KeysSetup = new Dictionary<Keys, string>
                {
                    [Keys.G] = "Image"
                };
            }
        }
    }
}