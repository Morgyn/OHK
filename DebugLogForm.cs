using System;
using System.IO;
using System.Windows.Forms;

// TODO: make debug scale
// TODO: allow saving of debug log

namespace OHK
{
    public partial class DebugLogForm : Form
    {
        private static readonly DebugLogForm instance = new DebugLogForm();
        private static SaveFileDialog LogFileDialog = new SaveFileDialog();
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

        private void saveButton_Click(object sender, EventArgs e)
        {
            LogFileDialog.CreatePrompt      = false;
            LogFileDialog.OverwritePrompt   = true;
            LogFileDialog.FileName          = Constant.appName+"_Log";
            LogFileDialog.DefaultExt        = "txt";
            LogFileDialog.Filter            = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            LogFileDialog.InitialDirectory  = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            DialogResult result = LogFileDialog.ShowDialog();
            Stream fileStream;

            if (result == DialogResult.OK)
            {
                // Open the file, copy the contents of memoryStream to fileStream,
                // and close fileStream. Set the memoryStream.Position value to 0 
                // to copy the entire stream. 
                fileStream = LogFileDialog.OpenFile();
                logConsole.SaveFile(fileStream, RichTextBoxStreamType.PlainText);
                fileStream.Close();
                Log(string.Format("Debug log written as {0}",LogFileDialog.FileName));
            }
        }
    }
}
