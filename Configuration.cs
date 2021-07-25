using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace OBSKeys
{
    class Configuration
    {
        private const string ConfigFolder = "Config";
        private const string ConfigFile = "config.json";
        public static ObsKeysConfig ObsKeys;

        static Configuration()
        {
            if (!Directory.Exists(ConfigFolder))
            {
                Directory.CreateDirectory(ConfigFolder);
            }

            if (!File.Exists(ConfigFolder + "/" + ConfigFile))
            {
                ObsKeys = new ObsKeysConfig();
                string json = JsonConvert.SerializeObject(ObsKeys, Formatting.Indented);
                File.WriteAllText(ConfigFolder + "/" + ConfigFile, json);
            }
            else
            {
                string json = File.ReadAllText(ConfigFolder + "/" + ConfigFile);
                ObsKeys = JsonConvert.DeserializeObject<ObsKeysConfig>(json);
            }
        }

        public class ObsKeysConfig
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

            public Dictionary<Keys, string> KeysSetup;

            public ObsKeysConfig()
            {
                Ip = "127.0.0.1";
                Port = "4444";
                Password = "test";
                PauseKey = Keys.F6;
                Delay = 300;
                KeysSetup = new Dictionary<Keys, string>
                {
                    [Keys.G] = "Image"
                };
            }
        }
    }
}