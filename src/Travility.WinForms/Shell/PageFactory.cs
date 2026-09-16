using System;
using System.Windows.Forms;
using Travility.Core.Session;
using Travility.WinForms.Pages;

namespace Travility.WinForms.Shell
{
    public interface IPageFactory
    {
        UserControl Create(string key);
    }

    public sealed class PageFactory : IPageFactory
    {
        private readonly UserSession _session;

        public PageFactory(UserSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public UserControl Create(string key)
        {
            if (string.Equals(key, "Home", StringComparison.OrdinalIgnoreCase))
            {
                return new HomePage(_session);
            }

            throw new NotSupportedException($"Trang '{key}' chưa khả dụng trong bản Foundation.");
        }
    }
}
