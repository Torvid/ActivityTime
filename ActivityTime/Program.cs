using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

static class Program
{
    public static bool Quit = false;
    public static Form1 form1;

    [DllImport("User32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // If another instance is running, bring it to front and close self
        //Process[] otherInstances = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
        //if (otherInstances.Length > 0)
        //{
        //    for (int i = 0; i < otherInstances.Length; i++)
        //    {
        //        SetForegroundWindow(otherInstances[i].MainWindowHandle);
        //    }
        //    return;
        //}

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Form1());
        //form1 = new Form1();
        //form1.Show();
        //
        ////form1.Initialize();
        //Quit = false;
        //while (!Quit)
        //{
        //    Application.DoEvents();
        //    System.Threading.Thread.Sleep(1);
        //}
    }
}
