using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SenegaleseAssociation.Data;
using SenegaleseAssociation.Models;
using Microsoft.EntityFrameworkCore;

namespace SenegaleseAssociation.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Organization")]
    public class LeadershipController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeadershipController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var leadership = await _context.Leadership.ToListAsync();
            return View(leadership);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Leadership leadership)
        {
            if (ModelState.IsValid)
            {
                _context.Leadership.Add(leadership);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(leadership);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var leadership = await _context.Leadership.FindAsync(id);
            if (leadership == null)
            {
                return NotFound();
            }
            return View(leadership);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Leadership leadership)
        {
            if (id != leadership.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(leadership);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeadershipExists(leadership.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(leadership);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var leadership = await _context.Leadership.FindAsync(id);
            if (leadership != null)
            {
                _context.Leadership.Remove(leadership);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool LeadershipExists(int id)
        {
            return _context.Leadership.Any(e => e.Id == id);
        }
    }
}