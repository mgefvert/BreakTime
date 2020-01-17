using System;
using System.Threading;
using System.Windows.Forms;
using BreakTime.Forms;

namespace BreakTime
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            var mutex = new Mutex(true, "org.gefvert.breaktime", out var created);
            try
            {
                if (!created)
                    return;
                
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                using (new MainForm())
                    Application.Run();
            }
            finally
            {
                mutex.Dispose();
            }
        }
    }
}
