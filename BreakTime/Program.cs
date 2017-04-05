using System;
using System.Windows.Forms;

namespace BreakTime
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (new MainForm())
                Application.Run();
        }
    }
}
