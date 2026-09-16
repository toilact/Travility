using System;
using System.Threading;
using System.Windows.Forms;
using Travility.Core.Contracts;

namespace Travility.WinForms.Infrastructure
{
    public static class GlobalExceptionHandler
    {
        private static IAppLogger _logger;

        public static void Register(IAppLogger logger)
        {
            _logger = logger;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += OnThreadException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        public static string GenerateErrorId()
        {
            string prefix = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-");
            string suffix = Guid.NewGuid().ToString("N").Substring(0, 4).ToUpperInvariant();
            return prefix + suffix;
        }

        public static void HandleException(Exception ex)
        {
            string errorId = GenerateErrorId();
            _logger?.Error("Global", errorId, ex);

            string message = string.Format(
                "Ứng dụng gặp lỗi ngoài dự kiến. Vui lòng thử lại.\nMã lỗi: {0}",
                errorId);

            MessageBox.Show(message, "Lỗi ứng dụng", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception ?? new Exception("Unknown unhandled exception occurred.");
            HandleException(ex);
        }
    }
}
