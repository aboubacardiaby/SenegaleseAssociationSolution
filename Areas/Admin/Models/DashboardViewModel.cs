using SenegaleseAssociation.Models;

namespace SenegaleseAssociation.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        public int TotalEvents { get; set; }
        public int UpcomingEvents { get; set; }
        public int TotalServices { get; set; }
        public int TotalLeadership { get; set; }
        public int TotalTestimonials { get; set; }
        public int UnreadMessages { get; set; }
        
        public List<Event> RecentEvents { get; set; } = new();
        public List<ContactMessage> RecentMessages { get; set; } = new();
    }
}