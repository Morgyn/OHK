using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

// TODO: make debug scale
// TODO: allow saving of debug log

namespace OBSKeys
{   
    public partial class DebugLogForm : Form
    {
        private delegate void SafeCallDelegate(string text);
        public DebugLogForm()
        {
            InitializeComponent();
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
            if (IsDisposed)
            {
                return;
            }
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

        private void DebugLogForm_ResizeEnd(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
