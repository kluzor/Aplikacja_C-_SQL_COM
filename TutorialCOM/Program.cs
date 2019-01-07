using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;

namespace programCOM
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
            Application.Run(new programCOM());
        }
    }
}
