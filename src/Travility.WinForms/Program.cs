using System;
using System.Windows.Forms;
using Travility.Core.Security;
using Travility.Core.Services;
using Travility.Data;
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

            var sessionFactory = new TravilityDataSessionFactory();
            var passwordHasher = new Pbkdf2PasswordHasher(600000);
            var authenticationService = new AuthenticationService(sessionFactory, passwordHasher);

            Application.Run(new TravilityApplicationContext(authenticationService, logger));
        }
    }
}
