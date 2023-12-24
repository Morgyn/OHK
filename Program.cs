﻿using System;
using System.Windows.Forms;

namespace OHK
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm mainForm = MainForm.Instance;

            Application.Run(mainForm);
        }
    }

}
