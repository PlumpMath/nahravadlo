using System;
using System.Windows.Forms;

namespace Nahravadlo
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(String[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length > 0)
                Application.Run(new formMain(args[0]));
            else
                Application.Run(new formMain());
        }
    }
}