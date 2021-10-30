using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using OBSWebsocketDotNet;
using Timers = System.Timers;
using Octokit;

//TODO: logging selective if it goes to status or log or both (default just log, 1 both, 2 just status?)
//TODO: Make config windows connectiom & keys
//TODO: move config to registry
//TODO: export config option
namespace OHK
{
    public partial class MainForm:Form
    {
        private IKeyboardMouseEvents _events;
        private bool _isHoldingDown;
        private bool _isPaused;
        private OBSWebsocket OBSws;
        private Dictionary<Keys,Timer> keyTimer = new Dictionary<Keys, Timer>();
        private bool disconnectButtonFlag = false;
        private Timer reconnectTimer;
        private int reconnectCountdown;
        private string updateURL = "";

        public MainForm()
        {
            OBSws = new OBSWebsocket();
            InitializeComponent();
            connectionStatusPicture.Image = imageList1.Images[1];
            SubscribeGlobal();         
            OBSws.Connected += OnConnect;
            OBSws.Disconnected += OnDisconnect;
            Log(string.Format("Started {0} {1} {2}",Constant.appName,Constant.releaseTag,""));
            githubReleaseCheck();
        }

        private async void githubReleaseCheck()
        {
            var github = new GitHubClient(new ProductHeaderValue(Constant.appName));
            var releases = await github.Repository.Release.GetAll(Constant.githubName, Constant.githubRepo);
            var latest = releases[0];
            Log(string.Format(
                "The latest release is tagged at {0} and is named {1} and the url is: {2}",
                latest.TagName,
                latest.Name,
                latest.HtmlUrl));
            var versionCheck = tagToVersion(Constant.releaseTag).CompareTo(tagToVersion(latest.TagName));
            if (0 > versionCheck)
            {
                updateURL = latest.HtmlUrl;
                Log("Version: Update available.");
                UpdateMenuItem.Text = "Update available";
                toolStripDropDownButton1.Image = imageList1.Images[2];
                UpdateMenuItem.Image = imageList1.Images[3];
            }
            else if (0 == versionCheck)
            {
                updateURL = "";
                Log("Version: Up to date");
                UpdateMenuItem.Text = "Check for update";
            }
            else if (0 < versionCheck)
            {
                updateURL = "";
                Log("Version: You're in the future");
                UpdateMenuItem.Text = "Future version";
            }
        }

        private Version tagToVersion(string tag)
        {
            return new Version(tag.Substring(1));
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
            if(e.KeyCode == Configuration.OHK.PauseKey)
            {
                if (_isPaused)
                {
                    Log("Continue listening");
                    _isPaused = false;
                }
                else
                {
                    Log("Paused listening");
                    _isPaused = true;
                }
            }

            if (_isPaused)
            {
                return;
            }

            if (Configuration.OHK.KeysSetup.ContainsKey(e.KeyCode))
            {
                if (_isHoldingDown)
                {
                    return;
                }

                _isHoldingDown = true;

                if (OBSws.IsConnected)
                {
                    if (CheckSceneItems(Configuration.OHK.KeysSetup[e.KeyCode]))
                    {
                        if (keyTimer.ContainsKey(e.KeyCode) && keyTimer[e.KeyCode].Enabled == true)
                        {
                            keyTimer[e.KeyCode].Stop();
                            keyTimer.Remove(e.KeyCode);
                            Log($"Timer still running, resetting timer \t{Configuration.OHK.KeysSetup[e.KeyCode]}");
                        }
                        else
                        {

                            OBSws.SetSourceRender(Configuration.OHK.KeysSetup[e.KeyCode], true);
                            Log($"Show \t{Configuration.OHK.KeysSetup[e.KeyCode]}");
                        }
                    }
                    else
                    {
                        Log("Item not find in this scene");
                    }
                }
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (_isPaused)
            {
                return;
            }

            if (Configuration.OHK.KeysSetup.ContainsKey(e.KeyCode))
            {
                _isHoldingDown = false;
                if (OBSws.IsConnected && CheckSceneItems(Configuration.OHK.KeysSetup[e.KeyCode]))
                {
                    if (CheckSceneItems(Configuration.OHK.KeysSetup[e.KeyCode]))
                    {
                        keyTimer.Add(e.KeyCode, new Timer());
                        keyTimer[e.KeyCode].Tick += (timerSender, timerE) => KeyTimerEventProcessor(timerSender, timerE, e.KeyCode);
                        keyTimer[e.KeyCode].Interval = Configuration.OHK.Delay;
                        keyTimer[e.KeyCode].Start();  
                    }
                    else
                    {
                        Log("Item not find in this scene");
                    }
                }
            }
        }

        private void KeyTimerEventProcessor(object sender, EventArgs e, Keys keyCode)
        {
            if (sender is Timer timer)
            {
                string sourceName = Configuration.OHK.KeysSetup[keyCode];
                OBSws.SetSourceRender(sourceName, false);               
                Log($"Hide \t{sourceName}");
                timer.Dispose();
                // somehow need to remove parent
                keyTimer.Remove(keyCode);
            }
        }

        private bool CheckSceneItems(string item)
        {
            var itemNames = new List<string>();
            foreach (var sceneItem in OBSws.GetCurrentScene().Items)
            {
                itemNames.Add(sceneItem.SourceName);
            }

            return itemNames.Contains(item);
        }

        private void OnConnect(object sender, EventArgs e)
        {
            Log("Connected to OBS");
            connectButton.Text = "Disconnect";
            connectionStatusPicture.Image = imageList1.Images[0];
        }

        private void OnDisconnect(object sender, EventArgs e)
        {
            Log(string.Format("Disconnected from OBS"));
            
            UpdateConnectButton("Connect");
            connectionStatusPicture.Image = imageList1.Images[1];
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
                OBSws.Connect($"ws://{Configuration.OHK.Ip}:{Configuration.OHK.Port}", Configuration.OHK.Password);
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

        private void DisconnectOBS()
        {
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
    }
}
