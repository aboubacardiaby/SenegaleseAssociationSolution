using System.ComponentModel.DataAnnotations;
using SenegaleseAssociation.Areas.Admin.Controllers;
using SenegaleseAssociation.Models; // Add this for Event and ContactMessage entities

namespace SenegaleseAssociation.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        // Stats properties
        public int TotalEvents { get; set; }
        public int UpcomingEvents { get; set; }
        public int TotalServices { get; set; }
        public int TotalLeadership { get; set; }
        public int TotalTestimonials { get; set; }
        public int UnreadMessages { get; set; }

        // Collections - using entity models directly to avoid conflicts
        public List<Event> RecentEvents { get; set; } = new();
        public List<ContactMessage> RecentMessages { get; set; } = new();

        // Additional properties for enhanced functionality
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public string SystemStatus { get; set; } = "Good";
        public int OnlineUsers { get; set; } = 1;
        public int TodayEvents { get; set; }

        // Chart data (can be populated server-side or via API)
        public ChartData? FinancialChart { get; set; }
        public ChartData? DonationsChart { get; set; }
        public ChartData? EventsChart { get; set; }

        // User-specific data
        public List<NotificationViewModel> Notifications { get; set; } = new();
        public UserPreferences UserPreferences { get; set; } = new();
    }

    // Keep the Event and ContactMessage models in your main Models folder
    // These are just for reference - use your actual entity models

    public class NotificationViewModel
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Icon { get; set; } = "fas fa-info-circle";
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public string? ActionUrl { get; set; }
        public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

        public string PriorityClass => Priority switch
        {
            NotificationPriority.High => "bg-danger",
            NotificationPriority.Medium => "bg-warning",
            _ => "bg-primary"
        };
    }

    public class ChartData
    {
        public string Type { get; set; } = string.Empty;
        public List<string> Labels { get; set; } = new();
        public List<ChartDataset> Datasets { get; set; } = new();
        public object? Options { get; set; }
    }

    public class ChartDataset
    {
        public string Label { get; set; } = string.Empty;
        public List<object> Data { get; set; } = new();
        public string? BorderColor { get; set; }
        public string? BackgroundColor { get; set; }
        public double Tension { get; set; } = 0.4;
        public List<string>? BackgroundColors { get; set; }
    }

    public class UserPreferences
    {
        public bool EnableRealtimeUpdates { get; set; } = true;
        public bool EnableNotifications { get; set; } = true;
        public bool EnableAnimations { get; set; } = true;
        public string Theme { get; set; } = "light";
        public int RefreshInterval { get; set; } = 30; // seconds
        public List<string> FavoriteCharts { get; set; } = new();
        public DashboardLayout Layout { get; set; } = DashboardLayout.Default;
    }

    public enum MessagePriority
    {
        Low = 0,
        Normal = 1,
        High = 2,
        Urgent = 3
    }

    public enum NotificationPriority
    {
        Low = 0,
        Normal = 1,
        Medium = 2,
        High = 3
    }

    public enum DashboardLayout
    {
        Default = 0,
        Compact = 1,
        Extended = 2,
        Custom = 3
    }

    // Supporting classes for real-time updates
    public class RealtimeStats
    {
        public int UnreadMessages { get; set; }
        public int TodayEvents { get; set; }
        public int OnlineUsers { get; set; }
        public string SystemHealth { get; set; } = "Good";
        public DateTime LastUpdate { get; set; } = DateTime.Now;
        public Dictionary<string, object> CustomMetrics { get; set; } = new();
    }

    public class DashboardApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    // For paginated results if needed
    public class PaginatedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }

    // Extension methods for better usability
    public static class DashboardViewModelExtensions
    {
        public static bool HasUnreadMessages(this DashboardViewModel model) => model.UnreadMessages > 0;
        public static bool HasUpcomingEvents(this DashboardViewModel model) => model.UpcomingEvents > 0;
        public static bool IsDataStale(this DashboardViewModel model, int minutes = 5) =>
            DateTime.Now - model.LastUpdated > TimeSpan.FromMinutes(minutes);

        public static string GetStatusBadgeClass(this DashboardViewModel model) => model.SystemStatus.ToLower() switch
        {
            "good" or "excellent" => "badge-success",
            "warning" or "caution" => "badge-warning",
            "error" or "critical" => "badge-danger",
            _ => "badge-secondary"
        };

        //public static List<EventViewModel> GetTodayEvents(this DashboardViewModel model) =>
        //    model.RecentEvents.Where(e => e.IsToday).ToList();

        //public static List<ContactMessageViewModel> GetUnreadMessages(this DashboardViewModel model) =>
        //    model.RecentMessages.Where(m => !m.IsRead).ToList();
    }
}