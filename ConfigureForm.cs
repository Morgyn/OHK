using Gma.System.MouseKeyHook;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static OHK.Configuration;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace OHK
{
    public partial class ConfigureForm : Form
    {
        private static readonly ConfigureForm instance = new ConfigureForm();
        private OBSWebsocket OBSwsTest;
        private Timer TestConnectionTimer;
        private IKeyboardMouseEvents CFkme;
        public ConfigureForm()
        {
            InitializeComponent();
        }
         public static ConfigureForm Instance
        {
            get
            {
                return instance;
            }
        }

        private void ConfigureForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        public void UpdateFields()
        {
            if (MapHideGroup.InvokeRequired)
            {
                MapHideGroup.Invoke(new MethodInvoker(delegate { UpdateFields(); }));
                return;
            }

            var MainFormInstance = MainForm.Instance;
            if (MainFormInstance.OBSws.IsConnected)
            {
                MapHideGroup.Text = "Map Hide";
                foreach (Control control in MapHideGroup.Controls)
                {
                    control.Enabled = true;
                }
            }
            else
            {
                MapHideGroup.Text = "Map Hide (Connection Required)";
                foreach (Control control in MapHideGroup.Controls)
                {
                    control.Enabled = false;
                }
            }

            ip.Text = Configuration.OHK.Ip;
            port.Text = Configuration.OHK.Port;
            password.Text = Configuration.OHK.Password;
            hotkey.Text = Configuration.OHK.hotKeys[0].Key.ToString();
            scene.Items.Clear();
            scene.Items.Add(Configuration.OHK.hotKeys[0].Scene);
            scene.Text = Configuration.OHK.hotKeys[0].Scene;
            source.Items.Clear();
            source.Items.Add(Configuration.OHK.hotKeys[0].Source);
            source.Text = Configuration.OHK.hotKeys[0].Source;
            delay.Text = Configuration.OHK.hotKeys[0].Delay.ToString();
        }

    

        public void UnHide()
        {
            UpdateFields();
            Show();
        }

        private void digit_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 8 is backspace
            if (Char.IsDigit(e.KeyChar)) return;
            if (e.KeyChar == (char)8) return;
            e.Handled = true;
        }

        private void SaveConfig(object sender, EventArgs e)
        {
            Configuration.OHK.Ip = ip.Text;
            Configuration.OHK.Port = port.Text;
            Configuration.OHK.Password = password.Text;
            if (Enum.TryParse<Keys>(hotkey.Text, out Keys keyEnum))
            {
                Configuration.OHK.hotKeys[0].Key = keyEnum;
            }
            Configuration.OHK.hotKeys[0].Scene = scene.Text;
            Configuration.OHK.hotKeys[0].Source = source.Text;
            Configuration.OHK.hotKeys[0].Delay = int.Parse(delay.Text);
            Configuration.SaveConfig();
            Hide();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void TestConnection_Click(object sender, EventArgs e)
        {
            if (!ushort.TryParse(port.Text, out ushort shortPort))
            {
                ConfigureForm.Instance.Invoke((MethodInvoker)delegate {
                    MessageBox.Show(this, "Port must be below 65536", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);;
                });
                return;
            }
            
            if (OBSwsTest != null)
            {
                OBSwsTest.Disconnect();
            }
            TestConnection.Text = "Testing";
            TestConnection.Enabled = false;
            OBSwsTest = new OBSWebsocket();

            OBSwsTest.Connected += TestOnConnect;
            OBSwsTest.Disconnected += TestOnDisconnect;
            OBSwsTest.ConnectAsync($"ws://{ip.Text}:{port.Text}", password.Text);

            TestConnectionTimer = new Timer();
            TestConnectionTimer.Tick += TestConnectionTimerEventProcessor;
            TestConnectionTimer.Interval = 5000;
            TestConnectionTimer.Start();
        }

        private void TestOnConnect(object sender, EventArgs e)
        {
            if (TestConnectionTimer != null && TestConnectionTimer.Enabled)
            {
                TestConnectionTimer.Stop();
                TestConnectionTimer.Dispose();
            }

            var versionInfo = OBSwsTest.GetVersion();
            Log($"PluginV: {versionInfo.PluginVersion} OBSv: {versionInfo.OBSStudioVersion}");
            if (int.Parse(versionInfo.PluginVersion.Split('.')[0]) < 5)
            {
                Log("OBS Test Old websocket server! Check port!");
                OBSwsTest.Disconnect();
            }
            Log("OBS Test Connected to OBS");
            ConfigureForm.Instance.Invoke((MethodInvoker)delegate {
                MessageBox.Show(this, "Test OK!", "Connection test");
            });
            

            OBSwsTest.Disconnect();
        }

        private void TestOnDisconnect(object sender, OBSWebsocketDotNet.Communication.ObsDisconnectionInfo e)
        {
            OBSwsTest.Disconnected -= TestOnDisconnect;
            if (TestConnectionTimer != null && TestConnectionTimer.Enabled)
            {
                TestConnectionTimer.Stop();
                TestConnectionTimer.Dispose();
            }

            Log("Disconnected from OBS Test");
            TestConnection.Invoke(new MethodInvoker(delegate {  
                TestConnection.Text = "Test";
                TestConnection.Enabled = true;
            }));

            if (e.ObsCloseCode == OBSWebsocketDotNet.Communication.ObsCloseCodes.AuthenticationFailed)
            {
                ConfigureForm.Instance.Invoke((MethodInvoker)delegate {
                    MessageBox.Show(this, "Wrong password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                });
                return;
            }
            else if (e.WebsocketDisconnectionInfo != null)
            {
                if (e.WebsocketDisconnectionInfo.Exception != null)
                {
                    ConfigureForm.Instance.Invoke((MethodInvoker)delegate {
                        MessageBox.Show(this, $"Connection failed: CloseCode: {e.ObsCloseCode} Desc: {e.WebsocketDisconnectionInfo?.CloseStatusDescription} Exception:{e.WebsocketDisconnectionInfo?.Exception?.Message}\nType: {e.WebsocketDisconnectionInfo.Type}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    });
                }
                else
                {
                    if (e.WebsocketDisconnectionInfo.Type == Websocket.Client.DisconnectionType.ByUser)
                        return;
                    if (e.WebsocketDisconnectionInfo.Type == Websocket.Client.DisconnectionType.Exit)
                        return;
                    if (e.WebsocketDisconnectionInfo.Type == Websocket.Client.DisconnectionType.Error)
                    {
                        ConfigureForm.Instance.Invoke((MethodInvoker)delegate
                        {
                            MessageBox.Show(this, $"{e.WebsocketDisconnectionInfo?.CloseStatusDescription}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        });
                        return;
                    }
                    ConfigureForm.Instance.Invoke((MethodInvoker)delegate
                    {
                            MessageBox.Show(ConfigureForm.Instance, $"Connection failed: CloseCode: {e.ObsCloseCode}\n Desc: {e.WebsocketDisconnectionInfo?.CloseStatusDescription}\nType: {e.WebsocketDisconnectionInfo.Type}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        });
                }
            }
            else
            {
                ConfigureForm.Instance.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show(this, $"Connection failed: CloseCode: {e.ObsCloseCode}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                });
                return;
            }
        }

        private void TestConnectionTimerEventProcessor(object sender, EventArgs e)
        {
            if (sender is Timer timer)
            {
                timer.Dispose();
                ConfigureForm.Instance.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show(this, "Test timed out", "Connection test");
                });
                Log("Connection timed out waiting for handshake OBS Test");
                OBSwsTest.Disconnect();
            }
        }

        private void SetButtom_Click(object sender, EventArgs e)
        {
            CFkme = Hook.GlobalEvents();
            CFkme.KeyDown += CFkme_KeyDown;
            SetButtom.Text = "Press";
        }
        private void CFkme_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            Log("Set Hotkey: "+ e.ToString());
            hotkey.Text = e.KeyCode.ToString();
            CFkme.KeyDown -= CFkme_KeyDown;
            SetButtom.Text = "Set";
        }

        private void Log(string text)
        {
            if (IsDisposed)
            {
                return;
            }
            DebugLogForm.Instance.Log(text);
        }

        private void scene_DropDown(object sender, EventArgs e)
        {
            var MainFormInstance = MainForm.Instance;
            var scenes = MainFormInstance.OBSws.ListScenes();

            scene.Items.Clear();
            
            foreach (var sceneItem in scenes)
            {
                scene.Items.Add(sceneItem.Name);
            }
            source.Text = "";
        }

        private void source_DropDown(object sender, EventArgs e)
        {
            var MainFormInstance = MainForm.Instance;
            List<SceneItemDetails> sources;
            try
            {
                sources = MainFormInstance.OBSws.GetSceneItemList(scene.Text);
            }
            catch (Exception ex)
            {
                sources = new List<SceneItemDetails>();
            }

            source.Items.Clear();

            foreach (var sourceeItem in sources)
            {
                source.Items.Add(sourceeItem.SourceName);
            }
        }

        private void delay_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(delay.Text))
            {
                delay.Text = "0";
            }
        }
    }
}