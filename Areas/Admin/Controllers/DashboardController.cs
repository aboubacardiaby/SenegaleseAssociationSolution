using Microsoft.AspNetCore.Mvc;
using SenegaleseAssociation.Data;
using Microsoft.AspNetCore.Authorization;
using SenegaleseAssociation.Areas.Admin.Models;

namespace SenegaleseAssociation.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var dashboardData = new DashboardViewModel
            {
                TotalEvents = _context.Events.Count(e => e.IsActive),
                UpcomingEvents = _context.Events.Count(e => e.IsActive && e.Date >= DateTime.Now),
                TotalServices = _context.Services.Count(s => s.IsActive),
                TotalLeadership = _context.Leadership.Count(l => l.IsActive),
                TotalTestimonials = _context.Testimonials.Count(t => t.IsActive),
                UnreadMessages = _context.ContactMessages.Count(m => !m.IsRead),
                RecentEvents = _context.Events
                    .Where(e => e.IsActive && e.Date >= DateTime.Now)
                    .OrderBy(e => e.Date)
                    .Take(5)
                    .ToList(),
                RecentMessages = _context.ContactMessages
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(5)
                    .ToList()
            };

            return View(dashboardData);
        }
    }
}