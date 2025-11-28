using Microsoft.AspNetCore.Mvc;
using SenegaleseAssociation.Data;
using SenegaleseAssociation.Models;

namespace SenegaleseAssociation.Controllers
{
    public class DonateController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonateController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProcessDonation([FromBody] Donation donation)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Invalid donation data. Please check all fields." });
                }

                // Set initial status
                donation.Status = "Pending";
                donation.CreatedAt = DateTime.UtcNow;
                donation.UpdatedAt = DateTime.UtcNow;

                // Generate a transaction ID
                donation.TransactionId = $"TXN-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

                // Save to database
                _context.Donations.Add(donation);
                _context.SaveChanges();

                // Return success response with transaction ID
                return Json(new
                {
                    success = true,
                    message = "Thank you for your donation! Your transaction has been recorded.",
                    transactionId = donation.TransactionId,
                    donationId = donation.Id
                });
            }
            catch (Exception ex)
            {
                // Log the error (you might want to use a logging framework here)
                return Json(new
                {
                    success = false,
                    message = "An error occurred while processing your donation. Please try again or contact us for assistance."
                });
            }
        }

        public IActionResult Success(int id)
        {
            var donation = _context.Donations.Find(id);
            if (donation == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(donation);
        }
    }
}
