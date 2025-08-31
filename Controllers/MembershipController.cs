using Microsoft.AspNetCore.Mvc;
using SenegaleseAssociation.Data;
using SenegaleseAssociation.Models;
using Microsoft.EntityFrameworkCore;

namespace SenegaleseAssociation.Controllers
{
    public class MembershipController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MembershipController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Member member)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                var existingMember = await _context.Members.FirstOrDefaultAsync(m => m.Email == member.Email);
                if (existingMember != null)
                {
                    ModelState.AddModelError("Email", "A member with this email address already exists.");
                    return View(member);
                }

                member.RegistrationDate = DateTime.UtcNow;
                member.MembershipStatus = "Pending";
                member.IsActive = true;

                _context.Members.Add(member);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thank you for your membership application! Your registration has been submitted and is pending approval.";
                return RedirectToAction(nameof(Success));
            }

            return View(member);
        }

        public IActionResult Success()
        {
            return View();
        }

        public async Task<IActionResult> Index()
        {
            var members = await _context.Members
                .Where(m => m.MembershipStatus == "Active" && m.IsActive)
                .OrderBy(m => m.FirstName)
                .ThenBy(m => m.LastName)
                .Select(m => new
                {
                    m.Id,
                    m.FirstName,
                    m.LastName,
                    m.City,
                    m.State,
                    m.Country,
                    m.Profession
                })
                .ToListAsync();

            return View(members);
        }
    }
}