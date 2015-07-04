using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ModGenerator
{
    static class Program
    {
        public static string[] Arguments { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Arguments = args;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm()); // Our exception catcher is dumping us here for some reason instead of giving us a more in-depth exception. Find out why.
            Console.WriteLine("Check this for multiple hits."); // this actually isn't getting called at all.
        }
    }
}
