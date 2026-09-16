using System;
using System.Windows.Forms;
using Travility.WinForms.Infrastructure;

namespace Travility.WinForms
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            var logger = new FileAppLogger();
            GlobalExceptionHandler.Register(logger);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }
    }
}
