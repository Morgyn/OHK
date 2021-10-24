using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OBSKeys
{
    public partial class DebugLogForm : Form
    {
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
            logConsole.AppendText($"{DateTime.Now.ToString("HH:mm:ss")}: " + text + "\n");
            logConsole.ScrollToCaret();
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(logConsole.Text);
        }

        private void DebugLogForm_ResizeEnd(object sender, EventArgs e)
        {

        }
    }
}
