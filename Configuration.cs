using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Gma.System.MouseKeyHook.HotKeys;
using Microsoft.Win32;
using Newtonsoft.Json;


namespace OHK
{
    public class HotKey
    {
        public Keys Key { get; set; }
        public string Scene { get; set; }
        public string Source { get; set; }
        public int Delay { get; set; }
        public string Type { get; set; }
    }
    class Configuration
    {
        private const string ConfigFolder = "Config";
        private const string ConfigFile = "config.json";
        public static OHKConfig OHK;
        private const string RegPath = @"SOFTWARE\"+Constant.Author+@"\"+Constant.appName;

        static Configuration()
        {
            try
            {
                OHK = new OHKConfig();

                using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(RegPath))
                {
                    if (rk == null)
                    {
                        using (RegistryKey rki = Registry.CurrentUser.CreateSubKey(RegPath))
                        {
                            rki.SetValue("IP", OHK.Ip);
                            rki.SetValue("Port", OHK.Port);
                            rki.SetValue("Password", OHK.Password);
                            string hotkeysJSON = JsonConvert.SerializeObject(OHK.hotKeys, Formatting.Indented);
                            rki.SetValue("HotKeys", hotkeysJSON);
                        }
                    }
                    else
                    {
                        OHK.Ip = rk.GetValue("IP").ToString();
                        OHK.Port = rk.GetValue("Port").ToString();
                        OHK.Password = rk.GetValue("Password").ToString();
                        //DebugLogForm.Instance.Log($"reg: { rk.GetValue("HotKeys") }");
                        OHK.hotKeys = JsonConvert.DeserializeObject<HotKey[]>(rk.GetValue("HotKeys").ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                DebugLogForm.Instance.Log($"Registry exception: {ex}");
            }
        }
        private void Log(string text)
        {
            DebugLogForm.Instance.Log(text);
        }
        static public void SaveConfig()
        {
            using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(RegPath, true))
            {
                if (rk != null)
                {
                    rk.SetValue("IP", OHK.Ip);
                    rk.SetValue("Port", OHK.Port);
                    rk.SetValue("Password", OHK.Password);
                    string hotkeysJSON = JsonConvert.SerializeObject(OHK.hotKeys, Formatting.Indented);
                    DebugLogForm.Instance.Log($"Hotkeys: {hotkeysJSON}");
                    rk.SetValue("HotKeys", hotkeysJSON);
                }
            }
        }

        public class OHKConfig
        {
            public string Ip;
            public string Port;
            public string Password;
            public int ReconnectDelay;
            [JsonProperty("Hotkeys")]
            public HotKey[] hotKeys;

            public OHKConfig()
            {
                Ip = "127.0.0.1";
                Port = "4455";
                Password = "testtest";
                ReconnectDelay = 5;
                hotKeys = new HotKey[]
                {
                     new HotKey { Key = Keys.G, Scene = "", Source = "", Delay = 300, Type = "Show" }
                };
            }
        }
    }
}