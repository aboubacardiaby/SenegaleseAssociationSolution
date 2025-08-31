using Microsoft.AspNetCore.Mvc;
using SenegaleseAssociation.Constants;
using Microsoft.AspNetCore.Authorization;
using SenegaleseAssociation.Areas.Admin.Models;

namespace SenegaleseAssociation.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Finance}")]
    public class FinanceController : Controller
    {
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

        public IActionResult Donations()
        {
            // Sample donation data
            var donations = new List<DonationViewModel>
            {
                new() { Id = 1, DonorName = "Ahmed Family", Email = "ahmed@email.com", Amount = 500.00m, Date = DateTime.Now.AddDays(-1), Status = "Completed", Purpose = "General Fund" },
                new() { Id = 2, DonorName = "Fatima Diop", Email = "fatima@email.com", Amount = 250.00m, Date = DateTime.Now.AddDays(-3), Status = "Pending", Purpose = "Youth Programs" },
                new() { Id = 3, DonorName = "Anonymous", Email = "", Amount = 1000.00m, Date = DateTime.Now.AddDays(-5), Status = "Completed", Purpose = "Emergency Fund" },
                new() { Id = 4, DonorName = "Mamadou Ba", Email = "mamadou@email.com", Amount = 150.00m, Date = DateTime.Now.AddDays(-7), Status = "Completed", Purpose = "Cultural Events" }
            };
            
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

        [HttpPost]
        public IActionResult ApproveDonation(int id)
        {
            // In a real app, update the database
            TempData["Success"] = "Donation approved successfully.";
            return RedirectToAction(nameof(Donations));
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