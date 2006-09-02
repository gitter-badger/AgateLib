using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ERY.AudioTester
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (ERY.AgateLib.AgateSetup setup = new ERY.AgateLib.AgateSetup("Agate Audio Tester", args))
            {
                setup.Initialize(false, true, false);
                if (setup.Cancel)
                    return;

                Application.Run(new frmAudioTester());
            }
        }
    }
}