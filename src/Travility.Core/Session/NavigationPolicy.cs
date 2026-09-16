using System;
using System.Collections.Generic;

namespace Travility.Core.Session
{
    public static class NavigationPolicy
    {
        public static IList<NavigationItem> ForRole(string role, bool enableAssistant = false)
        {
            var items = new List<NavigationItem>();

            if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                items.Add(new NavigationItem("Home", "Trang chủ"));
                items.Add(new NavigationItem("Places", "Quản lý địa điểm"));
                items.Add(new NavigationItem("Users", "Quản lý người dùng"));
                items.Add(new NavigationItem("TransportOptions", "Phương tiện di chuyển"));
                items.Add(new NavigationItem("Achievements", "Thành tựu"));
                items.Add(new NavigationItem("Statistics", "Thống kê"));
                return items;
            }

            // Default or Traveler
            items.Add(new NavigationItem("Home", "Trang chủ"));
            items.Add(new NavigationItem("Map", "Bản đồ khám phá"));
            items.Add(new NavigationItem("Trips", "Chuyến đi"));
            items.Add(new NavigationItem("Itineraries", "Lịch trình"));
            items.Add(new NavigationItem("Budget", "Ngân sách"));
            items.Add(new NavigationItem("CheckIn", "Check-in"));
            items.Add(new NavigationItem("Passport", "Hộ chiếu du lịch"));

            if (enableAssistant)
            {
                items.Add(new NavigationItem("Assistant", "Trợ lý AI"));
            }

            return items;
        }
    }
}
