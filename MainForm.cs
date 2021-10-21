using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using OBSWebsocketDotNet;
using Octokit;

namespace OBSKeys
{
    static class Constant
    {
        public const string appName = "OBSKeys";
        public const string releaseTag = "v0.1";
        public const string githubName = "Morgyn";
        public const string githubRepo = "OBSKeys";
    }

    public partial class MainForm:Form
    {
        private IKeyboardMouseEvents _events;
        private bool _isHoldingDown;
        private bool _isPaused;
        private OBSWebsocket _obs;
        private Timer myTimer;

        public MainForm()
        {
            _obs = new OBSWebsocket();
            InitializeComponent();
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
                "The latest release is tagged at {0} and is named {1}",
                latest.TagName,
                latest.Name));
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

            logConsole.AppendText($"{DateTime.Now.ToString("HH:mm:ss")}: " + text + "\n");
            logConsole.ScrollToCaret();
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
                        if (myTimer != null && myTimer.Enabled == true)
                        {
                            myTimer.Stop();
                        }

                        _obs.SetSourceRender(Configuration.ObsKeys.KeysSetup[e.KeyCode], true); 
                        Log($"Visible \t{Configuration.ObsKeys.KeysSetup[e.KeyCode]}");   
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
                        myTimer = new Timer();
                        myTimer.Tick += TimerEventProcessor;
                        myTimer.Interval = Configuration.ObsKeys.Delay;
                        myTimer.Tag = Configuration.ObsKeys.KeysSetup[e.KeyCode];
                        myTimer.Start();  
                    }
                    else
                    {
                        Log("Item not find in this scene");
                    }
                }
            }
        }

        private void TimerEventProcessor(object sender, EventArgs e)
        {
            if (sender is Timer timer)
            {
                _obs.SetSourceRender(timer.Tag.ToString(), false);               
                Log($"Invsible \t{timer.Tag}");
                timer.Dispose();
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
        }

        private void OnDisconnect(object sender, EventArgs e)
        {
            Log("Disconnected from OBS");
        }

        private void button1_Click(object sender,EventArgs e)
        {
            if(!_obs.IsConnected)
            {
                try
                {
                    Log("Connecting...");
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
            else
            {
                Log("Already connected.");
            }
        }

        private void button2_Click(object sender,EventArgs e)
        {
            if (_obs.IsConnected)
            {
                _obs.Disconnect();
            }
            else
            {
                Log("Not connected.");
            }
        }
    }
}
