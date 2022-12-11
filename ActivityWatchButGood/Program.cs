using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ActivityWatchButGood
{
    static class Program
    {
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // If another instance is running, bring it to front and close self
            Process[] otherInstances = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
            if (otherInstances.Length > 1)
            {
                SetForegroundWindow(otherInstances[0].MainWindowHandle);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
