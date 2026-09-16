using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Travility.Tests.TestData;
using Travility.WinForms.Infrastructure;

namespace Travility.Tests.Infrastructure
{
    [TestFixture]
    public sealed class FileAppLoggerTests
    {
        [Test]
        public void Phai_GhiErrorIdVaException_Khi_LogError()
        {
            using (var temp = new TempDirectory())
            {
                var logger = new FileAppLogger(temp.Path, retentionDays: 14);
                logger.Error("Auth", "ERR-1234", new InvalidOperationException("boom"));
                var text = File.ReadAllText(Directory.GetFiles(temp.Path).Single());
                StringAssert.Contains("ERR-1234", text);
                StringAssert.Contains("InvalidOperationException", text);
            }
        }

        [Test]
        public void Phai_KhongMatDong_Khi_GhiDongThoi()
        {
            using (var temp = new TempDirectory())
            {
                var logger = new FileAppLogger(temp.Path, retentionDays: 14);
                Parallel.For(0, 20, i => logger.Info("Parallel", "line-" + i));
                var lines = File.ReadAllLines(Directory.GetFiles(temp.Path).Single());
                Assert.That(lines.Length, Is.EqualTo(20));
            }
        }

        [Test]
        public void Phai_XoaLogCu_Khi_QuaHanMuoiBonNgay()
        {
            using (var temp = new TempDirectory())
            {
                var oldFile = System.IO.Path.Combine(temp.Path, "travility-20260101.log");
                File.WriteAllText(oldFile, "old");
                File.SetLastWriteTimeUtc(oldFile, DateTime.UtcNow.AddDays(-15));
                var logger = new FileAppLogger(temp.Path, retentionDays: 14);
                logger.Info("Test", "new");
                Assert.That(File.Exists(oldFile), Is.False);
            }
        }
    }
}
