using System;
using System.Windows.Forms;

namespace OHK
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetCompatibleTextRenderingDefault(false);
            Application.EnableVisualStyles();

            Application.Run(MainForm.Instance);
        }
    }

}
