using System;
using System.Windows.Forms;

namespace SAToolsHub
{

    static class Program
    {
        static internal string[] Arguments { get; set; }
        public static SAToolsHub toolsHub;

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Arguments = args;
            toolsHub = new SAToolsHub();
            //Application.ThreadException += Application_ThreadException;
            Application.Run(toolsHub);
        }
    }
}
