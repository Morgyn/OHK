 using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using OBSWebsocketDotNet;
using Octokit;

namespace OBSKeys
{
    public partial class MainForm:Form
    {
        private IKeyboardMouseEvents _events;
        private bool _isHoldingDown;
        private bool _isPaused;
        private OBSWebsocket _obs;
        private Dictionary<Keys,Timer> keyTimer = new Dictionary<Keys, Timer>();
        private DebugLogForm debugLog;
        private bool disconnectButtonFlag;
        private Timer reconnectTimer;
        private int reconnectCountdown;

        public MainForm()
        {
            debugLog = new DebugLogForm();
            _obs = new OBSWebsocket();
            InitializeComponent();
            connectionStatusPicture.Image = imageList1.Images[1];
            SubscribeGlobal();         
            _obs.Connected += OnConnect;
            _obs.Disconnected += OnDisconnect;
            Log(String.Format("Started {0} {1}",Constant.appName,"(Modified by Morgyn)"));
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
            if (0 < versionCheck)
            {
                Log("Version: Update available.");
            }
            else if (0 == versionCheck)
            {
                Log("Version: Up to date");
            }
            else if (0 > versionCheck)
            {
                Log("Version: You're in the future");
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
            debugLog.Log(text);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Configuration.ObsKeys.PauseKey)
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

            if (Configuration.ObsKeys.KeysSetup.ContainsKey(e.KeyCode))
            {
                if (_isHoldingDown)
                {
                    return;
                }

                _isHoldingDown = true;

                if (_obs.IsConnected)
                {
                    if (CheckSceneItems(Configuration.ObsKeys.KeysSetup[e.KeyCode]))
                    {
                        if (keyTimer.ContainsKey(e.KeyCode) && keyTimer[e.KeyCode].Enabled == true)
                        {
                            keyTimer[e.KeyCode].Stop();
                            keyTimer.Remove(e.KeyCode);
                            Log($"Timer still running, resetting timer \t{Configuration.ObsKeys.KeysSetup[e.KeyCode]}");
                        }
                        else
                        {

                            _obs.SetSourceRender(Configuration.ObsKeys.KeysSetup[e.KeyCode], true);
                            Log($"Show \t{Configuration.ObsKeys.KeysSetup[e.KeyCode]}");
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

            if (Configuration.ObsKeys.KeysSetup.ContainsKey(e.KeyCode))
            {
                _isHoldingDown = false;
                if (_obs.IsConnected && CheckSceneItems(Configuration.ObsKeys.KeysSetup[e.KeyCode]))
                {
                    if (CheckSceneItems(Configuration.ObsKeys.KeysSetup[e.KeyCode]))
                    {
                        keyTimer.Add(e.KeyCode, new Timer());
                        keyTimer[e.KeyCode].Tick += (timerSender, timerE) => KeyTimerEventProcessor(timerSender, timerE, e.KeyCode);
                        keyTimer[e.KeyCode].Interval = Configuration.ObsKeys.Delay;
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
                string sourceName = Configuration.ObsKeys.KeysSetup[keyCode];
                _obs.SetSourceRender(sourceName, false);               
                Log($"Hide \t{sourceName}");
                timer.Dispose();
                // somehow need to remove parent
                keyTimer.Remove(keyCode);
            }
        }

        private bool CheckSceneItems(string item)
        {
            var itemNames = new List<string>();
            foreach (var sceneItem in _obs.GetCurrentScene().Items)
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
            Log("Disconnected from OBS");
            connectButton.Text = "Connect";
            connectionStatusPicture.Image = imageList1.Images[1];
            if (disconnectButtonFlag)
            {
                disconnectButtonFlag = false;
                //don't reconnect
            } else
            {
                reconnectCountdown = Configuration.ObsKeys.ReconnectDelay;
                startReconnectTimer();
                // start reconnect
            }
        }

        private void startReconnectTimer()
        {
            reconnectTimer = new Timer();
            reconnectTimer.Tick += ReconnectTimerEventProcessor;
            reconnectTimer.Interval = 1000;
            reconnectTimer.Start();
        }
        private void ReconnectTimerEventProcessor(object sender, EventArgs e)
        {
            if (sender is Timer timer)
            {
                if (reconnectCountdown < 1)
                {
                    
                    timer.Dispose();
                    connectOBS();
                }
                else
                {
                    Log(string.Format("Reconnect countdown {0}", reconnectCountdown));
                    reconnectCountdown--;
                }
            }
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (!_obs.IsConnected)
            {
                disconnectButtonFlag = false;
                connectOBS();
                
            }
            else
            {
                disconnectButtonFlag = true;
                _obs.Disconnect();
            }
            
        }
        private void connectOBS()
        {
            try
            {
                connectButton.Text = "Connecting...";
                _obs.Connect($"ws://{Configuration.ObsKeys.Ip}:{Configuration.ObsKeys.Port}", Configuration.ObsKeys.Password);
            }
            catch (AuthFailureException)
            {
                Log("Authentication failed.");
            }
            catch (ErrorResponseException ex)
            {
                Log($"Connect failed : {ex}");
            }
        }

        private void disconnectOBS()
        {
            _obs.Disconnect();
        }


        private void ergergToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            debugLog.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
    }
}
