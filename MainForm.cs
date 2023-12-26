using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeHollow.FeedReader;
using Gma.System.MouseKeyHook;
using Gma.System.MouseKeyHook.HotKeys;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using Timers = System.Timers;

//TODO: logging selective if it goes to status or log or both (default just log, 1 both, 2 just status?)
//TODO: Make config windows connectiom & keys
//TODO: move config to registry
//TODO: export config option
//TODO: Import/Export settings
//TODO: Show config options if ran and no config set
//TODO: Check Config On Connect (Scenes and Sources)
namespace OHK
{
    public partial class MainForm:Form
    {
        private static readonly Lazy<MainForm> lazyInstance = new Lazy<MainForm>(() => new MainForm());
        public static MainForm Instance => lazyInstance.Value;
        private IKeyboardMouseEvents _events;
        public OBSWebsocket OBSws;
        private Dictionary<Keys,Timer> keyTimer = new Dictionary<Keys, Timer>();
        private bool disconnectButtonFlag = false;
        private Timer reconnectTimer;
        private int reconnectCountdown;
        private string updateURL = "";
        private Timer ConnectionTimer;

        public MainForm()
        {
            OBSws = new OBSWebsocket();
            OBSws.Connected += OnConnect;
            OBSws.Disconnected += OnDisconnect;

            InitializeComponent();
            connectionStatusPicture.Image = imageList1.Images[1];
            SubscribeGlobal();         

            Log(string.Format("Started {0} {1} {2}", Constant.appName, Constant.releaseVersion,""));
            githubReleaseCheck();
        }

        private async void githubReleaseCheck()
        {
            string feedUrl = $"https://github.com/{Constant.githubName}/{Constant.githubRepo}/releases.atom";

            try
            {
                var feed = await FeedReader.ReadAsync(feedUrl);

                if (feed != null && feed.Items != null && feed.Items.Any())
                {

                    var versions = feed.Items
                        .Select(item => new
                        {
                            Version = item.Title,
                            Item = item
                        })
                        .Where(versionItem => !string.IsNullOrEmpty(versionItem.Version))
                        .OrderByDescending(versionItem => versionItem.Version)
                        .FirstOrDefault();
                    var latestVersion = versions?.Version;

                    Log($"Latest Version: {latestVersion}");

                    if (float.TryParse(latestVersion, out float ghVersion) && float.TryParse(Constant.releaseVersion, out float cVersion))
                    {
                        // Compare floats
                        if (ghVersion < cVersion)
                        {
                            updateURL = "";
                            Log("Version: You're in the future");
                            UpdateMenuItem.Text = "Future version";
                        }
                        else if (ghVersion > cVersion)
                        {
                            updateURL = versions?.Item.Link;
                            Log("Version: Update available.");
                            UpdateMenuItem.Text = "Update available";
                            toolStripDropDownButton1.Image = imageList1.Images[2];
                            UpdateMenuItem.Image = imageList1.Images[3];
                        }
                        else
                        {
                            updateURL = "";
                            Log("Version: Up to date");
                            UpdateMenuItem.Text = "Check for update";
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error parsing floats");
                    }
                }
                else
                {
                    Log("No feed items found.");
                }


            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
            }
 
        }

        private void SubscribeGlobal()
        {
            Subscribe(Hook.GlobalEvents());
        }

        private void Subscribe(IKeyboardMouseEvents events)
        {
            _events = events;
            Log("Subscribing to events");
            _events.KeyDown += OnKeyDown;
            Log("Subscribed to KeyDown");
            _events.KeyUp += OnKeyUp;
            Log("Subscribed to KeyUp");
        }

        private void Log(string text)
        {
            if (IsDisposed)
            {
                return;
            }
            statusLabel.Text = text;
            DebugLogForm.Instance.Log(text);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (OBSws.IsConnected)
            {
                HotKey pressedHotKey = Configuration.OHK.hotKeys.FirstOrDefault(hotKey => hotKey.Key == e.KeyCode);
                if (pressedHotKey != null && pressedHotKey.Scene!="" && pressedHotKey.Source!="")
                { 
                        if (keyTimer.ContainsKey(e.KeyCode) && keyTimer[e.KeyCode].Enabled == true)
                        {
                            keyTimer[e.KeyCode].Stop();
                            keyTimer.Remove(e.KeyCode);
                        }
                        else
                        {
                            try
                            {
                                int source_id = OBSws.GetSceneItemId(pressedHotKey.Scene, pressedHotKey.Source, -1);
                                OBSws.SetSceneItemEnabled(pressedHotKey.Scene, source_id, true);
                                Log($"Show \t{pressedHotKey.Scene}/{pressedHotKey.Source}");
                            } catch { Log($"Unable to set {pressedHotKey.Scene}/{pressedHotKey.Source}");
                        }
                    }
                }
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (OBSws.IsConnected)
            {
                HotKey pressedHotKey = Configuration.OHK.hotKeys.FirstOrDefault(hotKey => hotKey.Key == e.KeyCode);
                if (pressedHotKey != null && pressedHotKey.Scene != "" && pressedHotKey.Source != "")
                {
                    keyTimer.Add(pressedHotKey.Key, new Timer());
                    keyTimer[pressedHotKey.Key].Tick += (timerSender, timerE) => KeyTimerEventProcessor(timerSender, timerE, pressedHotKey);
                    keyTimer[pressedHotKey.Key].Interval = pressedHotKey.Delay;
                    keyTimer[pressedHotKey.Key].Tag = pressedHotKey.Key;
                    keyTimer[pressedHotKey.Key].Start();  
                }
            }
        }

        private void KeyTimerEventProcessor(object sender, EventArgs e, HotKey pressedHotKey)
        {
            if (sender is Timer timer)
            {
                int source_id = OBSws.GetSceneItemId(pressedHotKey.Scene, pressedHotKey.Source, -1);
                OBSws.SetSceneItemEnabled(pressedHotKey.Scene, source_id, false);
                Log($"Hide \t{pressedHotKey.Scene}/{pressedHotKey.Source}");
                timer.Dispose();
                keyTimer.Remove(pressedHotKey.Key);
            }
        }

        private void OnConnect(object sender, EventArgs e)
        {
            if (ConnectionTimer != null && ConnectionTimer.Enabled)
            {
                ConnectionTimer.Stop();
                ConnectionTimer.Dispose();
            }

            Log("Connected to OBS");
            
            connectionStatusPicture.Image = imageList1.Images[0];

            UpdateConnectButton("Disconnect");

            ConfigureForm.Instance.UpdateFields();

        }



        private void OnDisconnect(object sender, OBSWebsocketDotNet.Communication.ObsDisconnectionInfo e)
        {
            if (ConnectionTimer != null && ConnectionTimer.Enabled)
            {
                ConnectionTimer.Stop();
                ConnectionTimer.Dispose();
            }
            Log("Disconnected from OBS");
            
            UpdateConnectButton("Connect");
            connectionStatusPicture.Image = imageList1.Images[1];

            ConfigureForm.Instance.UpdateFields();
            if (disconnectButtonFlag)
            {
                disconnectButtonFlag = false;
                //don't reconnect
            }
            else
            {
                if (reconnectCheckBox.Checked)
                {
                    reconnectCountdown = Configuration.OHK.ReconnectDelay;
                    connectButton.Invoke(new MethodInvoker(delegate { StartReconnectTimer(); }));
                    // start reconnect
                }
            }
        }

        private void UpdateConnectButton( string text)
        {
            if (connectButton.InvokeRequired)
            {
                connectButton.Invoke(new MethodInvoker(delegate { connectButton.Text = text; }));
            } else
            {
                connectButton.Text = text;
            }
        }

        private void StartReconnectTimer()
        {
            if (reconnectTimer != null && reconnectTimer.Enabled)
            {
                reconnectTimer.Stop();
                reconnectTimer.Dispose();
            }
            reconnectTimer = new Timer();
            reconnectTimer.Tick += ReconnectTimerEventProcessor;
            reconnectTimer.Interval = 1000;
            reconnectTimer.Start();
        }
        private void ReconnectTimerEventProcessor(object sender, EventArgs e)
        {
            if (sender is Timer timer)
            {
                if (reconnectCheckBox.Checked)
                {
                    if (reconnectCountdown < 1)
                    {
                        timer.Start();
                        timer.Dispose();
                        ConnectOBS();
                    }
                    else
                    {
                        Log(string.Format("Reconnect countdown {0}", reconnectCountdown));
                        reconnectCountdown--;
                    }
                } else
                {
                    timer.Dispose();
                }
            }
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            if (!OBSws.IsConnected)
            {
                disconnectButtonFlag = false;
                ConnectOBS();
                
            }
            else
            {
                disconnectButtonFlag = true;
                DisconnectOBS();
            }
            
        }
    
        private void ConnectOBS()
        {
            try
            {
                connectButton.Text = "Connecting...";
                OBSws.ConnectAsync($"ws://{Configuration.OHK.Ip}:{Configuration.OHK.Port}", Configuration.OHK.Password);

                ConnectionTimer = new Timer();
                ConnectionTimer.Tick += ConnectionTimerEventProcessor;
                ConnectionTimer.Interval = 5000;
                ConnectionTimer.Start();
            }
            catch (AuthFailureException)
            {
                Log("Authentication failed.");
            }
            catch (ErrorResponseException ex)
            {
                Log(string.Format("Connect failed: ",ex.Message));
            }
        }

        private void ConnectionTimerEventProcessor(object sender, EventArgs e)
        {
            if (sender is Timer timer)
            {
                timer.Stop();
                timer.Dispose();
                Log("Connection initiation timed out.");
                OBSws.Disconnect();
            }
        }

        private void DisconnectOBS()
        {
            if (ConnectionTimer != null)
            {
                ConnectionTimer.Stop();
                ConnectionTimer.Dispose();
            }
            OBSws.Disconnect();
        }


        private void OpenDebugLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DebugLogForm.Instance.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void UpdateMenuItem_Click(object sender, EventArgs e)
        {
            if (updateURL == "")
            {
                githubReleaseCheck();
            } else
            {
                System.Diagnostics.Process.Start(updateURL); 
            }
        }

        private void configureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureForm.Instance.UnHide();
        }
    }
}
