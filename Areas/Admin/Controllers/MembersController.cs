using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SenegaleseAssociation.Data;
using SenegaleseAssociation.Models;
using SenegaleseAssociation.Areas.Admin.Models;
using Microsoft.EntityFrameworkCore;

namespace SenegaleseAssociation.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MembersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MembersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var members = await _context.Members
                .Include(m => m.ApprovedBy)
                .OrderByDescending(m => m.RegistrationDate)
                .Select(m => new MemberListViewModel
                {
                    Id = m.Id,
                    FullName = m.FullName,
                    Email = m.Email,
                    PhoneNumber = m.PhoneNumber,
                    City = m.City,
                    Country = m.Country,
                    Profession = m.Profession,
                    MembershipStatus = m.MembershipStatus,
                    RegistrationDate = m.RegistrationDate,
                    ApprovedDate = m.ApprovedDate,
                    ApprovedBy = m.ApprovedBy != null ? m.ApprovedBy.FullName : null,
                    IsActive = m.IsActive
                })
                .ToListAsync();

            return View(members);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.ApprovedBy)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            if (member.MembershipStatus != "Pending")
            {
                TempData["ErrorMessage"] = "Only pending members can be approved.";
                return RedirectToAction(nameof(Index));
            }

            member.MembershipStatus = "Active";
            member.ApprovedDate = DateTime.UtcNow;
            member.ApprovedById = _userManager.GetUserId(User);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Member {member.FullName} has been approved successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string reason = "")
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            if (member.MembershipStatus != "Pending")
            {
                TempData["ErrorMessage"] = "Only pending members can be rejected.";
                return RedirectToAction(nameof(Index));
            }

            member.MembershipStatus = "Rejected";
            if (!string.IsNullOrEmpty(reason))
            {
                member.Notes = string.IsNullOrEmpty(member.Notes) 
                    ? $"Rejection reason: {reason}" 
                    : $"{member.Notes}\n\nRejection reason: {reason}";
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Member {member.FullName} has been rejected.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Suspend(int id, string reason = "")
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            if (member.MembershipStatus != "Active")
            {
                TempData["ErrorMessage"] = "Only active members can be suspended.";
                return RedirectToAction(nameof(Index));
            }

            member.MembershipStatus = "Suspended";
            if (!string.IsNullOrEmpty(reason))
            {
                member.Notes = string.IsNullOrEmpty(member.Notes) 
                    ? $"Suspension reason: {reason}" 
                    : $"{member.Notes}\n\nSuspension reason: {reason}";
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Member {member.FullName} has been suspended.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reactivate(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            if (member.MembershipStatus == "Active")
            {
                TempData["ErrorMessage"] = "Member is already active.";
                return RedirectToAction(nameof(Index));
            }

            member.MembershipStatus = "Active";
            member.ApprovedDate = DateTime.UtcNow;
            member.ApprovedById = _userManager.GetUserId(User);
            member.IsActive = true;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Member {member.FullName} has been reactivated.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Member member)
        {
            if (id != member.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingMember = await _context.Members.FindAsync(id);
                    if (existingMember == null)
                    {
                        return NotFound();
                    }

                    // Update properties
                    existingMember.FirstName = member.FirstName;
                    existingMember.LastName = member.LastName;
                    existingMember.Email = member.Email;
                    existingMember.PhoneNumber = member.PhoneNumber;
                    existingMember.Address = member.Address;
                    existingMember.City = member.City;
                    existingMember.PostalCode = member.PostalCode;
                    existingMember.Country = member.Country;
                    existingMember.DateOfBirth = member.DateOfBirth;
                    existingMember.Gender = member.Gender;
                    existingMember.Profession = member.Profession;
                    existingMember.EmergencyContactName = member.EmergencyContactName;
                    existingMember.EmergencyContactPhone = member.EmergencyContactPhone;
                    existingMember.Notes = member.Notes;
                    existingMember.IsActive = member.IsActive;

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Member information updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(member.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Member {member.FullName} has been deleted.";
            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.Id == id);
        }
    }
}