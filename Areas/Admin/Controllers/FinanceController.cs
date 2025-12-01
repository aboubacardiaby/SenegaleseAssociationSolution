using Microsoft.AspNetCore.Mvc;
using SenegaleseAssociation.Constants;
using Microsoft.AspNetCore.Authorization;
using SenegaleseAssociation.Areas.Admin.Models;
using SenegaleseAssociation.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using SenegaleseAssociation.Models;

namespace SenegaleseAssociation.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Finance}")]
    public class FinanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FinanceController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            // Sample financial data - in a real app this would come from database
            var model = new FinanceOverviewViewModel
            {
                TotalIncome = 125000.00m,
                TotalExpenses = 98000.00m,
                MonthlyIncome = 12500.00m,
                MonthlyExpenses = 8900.00m,
                PendingDonations = 15,
                RecentTransactions = new List<FinanceTransactionViewModel>
                {
                    new() { Id = 1, Date = DateTime.Now.AddDays(-1), Description = "Community Event Registration Fees", Amount = 2500.00m, Type = "Income", Category = "Events" },
                    new() { Id = 2, Date = DateTime.Now.AddDays(-2), Description = "Monthly Venue Rental", Amount = -1200.00m, Type = "Expense", Category = "Facilities" },
                    new() { Id = 3, Date = DateTime.Now.AddDays(-3), Description = "Donation from Ahmed Family", Amount = 500.00m, Type = "Income", Category = "Donations" },
                    new() { Id = 4, Date = DateTime.Now.AddDays(-4), Description = "Office Supplies", Amount = -150.00m, Type = "Expense", Category = "Administration" },
                    new() { Id = 5, Date = DateTime.Now.AddDays(-5), Description = "Cultural Festival Sponsorship", Amount = 3000.00m, Type = "Income", Category = "Sponsorship" }
                }
            };
            
            return View(model);
        }

        public async Task<IActionResult> Donations(string status = "All", DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Donations.AsQueryable();

            // Filter by status
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(d => d.Status == status);
            }

            // Filter by date range
            if (startDate.HasValue)
            {
                query = query.Where(d => d.CreatedAt >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(d => d.CreatedAt <= endDate.Value.AddDays(1));
            }

            var donations = await query
                .OrderByDescending(d => d.CreatedAt)
                .Select(d => new DonationViewModel
                {
                    Id = d.Id,
                    DonorName = d.IsAnonymous ? "Anonymous" : d.DonorName,
                    Email = d.DonorEmail,
                    Amount = d.Amount,
                    Date = d.CreatedAt,
                    Status = d.Status,
                    Purpose = d.Frequency // Using Frequency as Purpose for now
                })
                .ToListAsync();

            ViewBag.StatusFilter = status;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View(donations);
        }

        public IActionResult Expenses()
        {
            // Sample expense data
            var expenses = new List<ExpenseViewModel>
            {
                new() { Id = 1, Description = "Monthly Venue Rental", Amount = 1200.00m, Date = DateTime.Now.AddDays(-2), Category = "Facilities", ApprovedBy = "Admin User", Status = "Approved" },
                new() { Id = 2, Description = "Office Supplies", Amount = 150.00m, Date = DateTime.Now.AddDays(-4), Category = "Administration", ApprovedBy = "Finance Manager", Status = "Approved" },
                new() { Id = 3, Description = "Event Catering", Amount = 2500.00m, Date = DateTime.Now.AddDays(-6), Category = "Events", ApprovedBy = "Admin User", Status = "Approved" },
                new() { Id = 4, Description = "Marketing Materials", Amount = 300.00m, Date = DateTime.Now.AddDays(-8), Category = "Marketing", ApprovedBy = "", Status = "Pending" }
            };
            
            return View(expenses);
        }

        public IActionResult Reports()
        {
            // Sample report data
            var model = new FinanceReportsViewModel
            {
                MonthlyIncomeData = new List<decimal> { 8000, 9500, 12000, 11500, 13000, 12500 },
                MonthlyExpenseData = new List<decimal> { 7500, 8200, 9800, 8900, 9500, 8900 },
                MonthLabels = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun" },
                CategoryBreakdown = new Dictionary<string, decimal>
                {
                    { "Events", 45000 },
                    { "Facilities", 25000 },
                    { "Administration", 15000 },
                    { "Programs", 13000 }
                }
            };
            
            return View(model);
        }

        public async Task<IActionResult> DonationDetails(int id)
        {
            var donation = await _context.Donations
                .Include(d => d.ProcessedBy)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (donation == null)
            {
                TempData["Error"] = "Donation not found.";
                return RedirectToAction(nameof(Donations));
            }

            return View(donation);
        }

        [HttpGet]
        public async Task<IActionResult> EditDonation(int id)
        {
            var donation = await _context.Donations.FindAsync(id);
            if (donation == null)
            {
                TempData["Error"] = "Donation not found.";
                return RedirectToAction(nameof(Donations));
            }

            return View(donation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDonation(int id, Donation donation)
        {
            if (id != donation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    donation.UpdatedAt = DateTime.UtcNow;
                    _context.Update(donation);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Donation updated successfully.";
                    return RedirectToAction(nameof(Donations));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DonationExists(donation.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(donation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDonation(int id)
        {
            var donation = await _context.Donations.FindAsync(id);
            if (donation == null)
            {
                TempData["Error"] = "Donation not found.";
                return RedirectToAction(nameof(Donations));
            }

            _context.Donations.Remove(donation);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Donation deleted successfully.";
            return RedirectToAction(nameof(Donations));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveDonation(int id)
        {
            var donation = await _context.Donations.FindAsync(id);
            if (donation == null)
            {
                TempData["Error"] = "Donation not found.";
                return RedirectToAction(nameof(Donations));
            }

            donation.Status = "Completed";
            donation.ProcessedDate = DateTime.UtcNow;
            donation.UpdatedAt = DateTime.UtcNow;

            // Set the ProcessedById if user is authenticated
            if (User.Identity?.IsAuthenticated == true)
            {
                donation.ProcessedById = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Donation approved successfully.";
            return RedirectToAction(nameof(Donations));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDonationStatus(int id, string status)
        {
            var donation = await _context.Donations.FindAsync(id);
            if (donation == null)
            {
                return Json(new { success = false, message = "Donation not found." });
            }

            donation.Status = status;
            donation.UpdatedAt = DateTime.UtcNow;

            if (status == "Completed")
            {
                donation.ProcessedDate = DateTime.UtcNow;
                if (User.Identity?.IsAuthenticated == true)
                {
                    donation.ProcessedById = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                }
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Donation status updated successfully." });
        }

        public async Task<IActionResult> ExportDonations(string status = "All", DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Donations.AsQueryable();

            // Apply same filters as Donations action
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(d => d.Status == status);
            }
            if (startDate.HasValue)
            {
                query = query.Where(d => d.CreatedAt >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(d => d.CreatedAt <= endDate.Value.AddDays(1));
            }

            var donations = await query.OrderByDescending(d => d.CreatedAt).ToListAsync();

            // Create CSV content
            var csv = new StringBuilder();
            csv.AppendLine("ID,Donor Name,Email,Phone,Amount,Frequency,Payment Method,Status,Transaction ID,Date,Processed Date,Message,Notes");

            foreach (var donation in donations)
            {
                csv.AppendLine($"{donation.Id}," +
                    $"\"{(donation.IsAnonymous ? "Anonymous" : donation.DonorName)}\"," +
                    $"\"{donation.DonorEmail}\"," +
                    $"\"{donation.DonorPhone}\"," +
                    $"{donation.Amount}," +
                    $"\"{donation.Frequency}\"," +
                    $"\"{donation.PaymentMethod}\"," +
                    $"\"{donation.Status}\"," +
                    $"\"{donation.TransactionId}\"," +
                    $"\"{donation.CreatedAt:yyyy-MM-dd HH:mm:ss}\"," +
                    $"\"{donation.ProcessedDate?.ToString("yyyy-MM-dd HH:mm:ss")}\"," +
                    $"\"{donation.Message.Replace("\"", "\"\"")}\"," +
                    $"\"{donation.Notes.Replace("\"", "\"\"")}\"");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"donations_{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
        }

        private bool DonationExists(int id)
        {
            return _context.Donations.Any(e => e.Id == id);
        }

        [HttpPost]
        public IActionResult ApproveExpense(int id)
        {
            // In a real app, update the database
            TempData["Success"] = "Expense approved successfully.";
            return RedirectToAction(nameof(Expenses));
        }
    }
}