using System;
using System.IO;
using System.Text;
using Travility.Core.Contracts;

namespace Travility.WinForms.Infrastructure
{
    public sealed class FileAppLogger : IAppLogger
    {
        private readonly string _logDirectory;
        private readonly int _retentionDays;
        private readonly object _lock = new object();

        public FileAppLogger(string logDirectory = null, int retentionDays = 14)
        {
            _retentionDays = retentionDays > 0 ? retentionDays : 14;
            _logDirectory = !string.IsNullOrWhiteSpace(logDirectory)
                ? logDirectory
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Travility", "Logs");

            EnsureDirectoryAndCleanup();
        }

        public void Info(string component, string message)
        {
            WriteEntry("INFO", component, "-", message);
        }

        public void Warning(string component, string message)
        {
            WriteEntry("WARN", component, "-", message);
        }

        public void Error(string component, string errorId, Exception exception)
        {
            string message = exception != null ? exception.ToString() : "Unknown error";
            WriteEntry("ERROR", component, string.IsNullOrWhiteSpace(errorId) ? "-" : errorId, message);
        }

        private void WriteEntry(string severity, string component, string errorId, string message)
        {
            string sanitizedMessage = Sanitize(message);
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            string line = string.Format("{0}|{1}|{2}|{3}|{4}", timestamp, severity, component, errorId, sanitizedMessage);

            lock (_lock)
            {
                try
                {
                    if (!Directory.Exists(_logDirectory))
                    {
                        Directory.CreateDirectory(_logDirectory);
                    }

                    string filePath = Path.Combine(_logDirectory, string.Format("travility-{0:yyyyMMdd}.log", DateTime.UtcNow));
                    using (var writer = new StreamWriter(filePath, append: true, encoding: Encoding.UTF8))
                    {
                        writer.WriteLine(line);
                    }
                }
                catch
                {
                    // Fail-safe: logger must not throw to caller
                }
            }
        }

        private void EnsureDirectoryAndCleanup()
        {
            try
            {
                if (!Directory.Exists(_logDirectory))
                {
                    Directory.CreateDirectory(_logDirectory);
                    return;
                }

                var cutoff = DateTime.UtcNow.AddDays(-_retentionDays);
                var logFiles = Directory.GetFiles(_logDirectory, "*.log");
                foreach (var file in logFiles)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.LastWriteTimeUtc < cutoff)
                    {
                        try
                        {
                            fileInfo.Delete();
                        }
                        catch
                        {
                            // Best-effort cleanup
                        }
                    }
                }
            }
            catch
            {
                // Best-effort
            }
        }

        private static string Sanitize(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            return input.Replace("\r\n", " ").Replace("\n", " ");
        }
    }
}
