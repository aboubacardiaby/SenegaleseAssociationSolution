using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SenegaleseAssociation.Data;
using SenegaleseAssociation.Models;
using SenegaleseAssociation.Services;
using System.Diagnostics;

namespace SenegaleseAssociation.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public HomeController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        public IActionResult Index()
        {
            var upcomingEvents = _context.Events
                .Where(e => e.IsActive && e.Date >= DateTime.Now)
                .OrderBy(e => e.Date)
                .Take(3)
                .ToList();
                
            var services = _context.Services
                .Where(s => s.IsActive)
                .OrderBy(s => s.DisplayOrder)
                .ToList();
                
            var communityHighlights = _context.CommunityHighlights
                .Where(ch => ch.IsActive)
                .OrderBy(ch => ch.DisplayOrder)
                .ToList();
                
            var president = _context.Leadership
                .FirstOrDefault(l => l.IsActive && l.IsPresident);
                
            var testimonials = _context.Testimonials
                .Where(t => t.IsActive && t.IsFeatured)
                .ToList();
            
            ViewBag.UpcomingEvents = upcomingEvents;
            ViewBag.Services = services;
            ViewBag.CommunityHighlights = communityHighlights;
            ViewBag.President = president;
            ViewBag.Testimonials = testimonials;
            
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Events()
        {
            var events = _context.Events
                .Where(e => e.IsActive)
                .OrderBy(e => e.Date)
                .ToList();
            return View(events);
        }

        public IActionResult Contact()
        {
            return View(new ContactMessage());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactMessage model)
        {
            if (ModelState.IsValid)
            {
                _context.ContactMessages.Add(model);
                _context.SaveChanges();

                try
                {
                    await _emailService.SendContactNotificationAsync(
                        model.Name,
                        model.Email,
                        model.Subject,
                        model.Message
                    );
                }
                catch (Exception ex)
                {
                    // Log the error but don't show it to the user
                    Console.WriteLine($"Failed to send email notification: {ex.Message}");
                }

                TempData["Success"] = "Thank you for your message! We will get back to you soon.";
                return RedirectToAction(nameof(Contact));
            }

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}