using System;
using System.Windows.Forms;

// TODO: make debug scale
// TODO: allow saving of debug log

namespace OHK
{
    public partial class DebugLogForm : Form
    {
        private static readonly DebugLogForm instance = new DebugLogForm();
        private delegate void SafeCallDelegate(string text);

        static DebugLogForm ()
        {
        }

        public DebugLogForm()
        {
            InitializeComponent();
        }
        public static DebugLogForm Instance
        {
            get
            {
                return instance;
            }
        }

        private void DebugLogForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }
        public void Log(string text)
        { 
            if (this.logConsole.InvokeRequired)
            {
                var d = new SafeCallDelegate(Log);
                logConsole.Invoke(d, new object[] { text });
            }
            else
            {
                logConsole.AppendText($"{DateTime.Now.ToString("HH:mm:ss")}: {text}\n");
                logConsole.ScrollToCaret();
            }
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(logConsole.Text);
        }
    }
}
