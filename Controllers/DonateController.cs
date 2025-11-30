using Microsoft.AspNetCore.Mvc;
using SenegaleseAssociation.Data;
using SenegaleseAssociation.Models;
using SenegaleseAssociation.Services;

namespace SenegaleseAssociation.Controllers
{
    public class DonateController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PayPalService _payPalService;
        private readonly StripeService _stripeService;

        public DonateController(ApplicationDbContext context, PayPalService payPalService, StripeService stripeService)
        {
            _context = context;
            _payPalService = payPalService;
            _stripeService = stripeService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessDonation([FromBody] Donation donation)
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

                // Handle PayPal payment
                if (donation.PaymentMethod.ToLower() == "paypal")
                {
                    try
                    {
                        var approvalUrl = await _payPalService.CreateOrder(donation);

                        return Json(new
                        {
                            success = true,
                            redirectUrl = approvalUrl,
                            paymentProvider = "PayPal",
                            message = "Redirecting to PayPal...",
                            transactionId = donation.TransactionId,
                            donationId = donation.Id
                        });
                    }
                    catch (Exception paypalEx)
                    {
                        // Log PayPal error
                        donation.Status = "Failed";
                        donation.Notes = $"PayPal error: {paypalEx.Message}";
                        _context.SaveChanges();

                        return Json(new
                        {
                            success = false,
                            message = "Failed to create PayPal payment. Please try again or use another payment method."
                        });
                    }
                }

                // Handle ACH payment through Stripe
                if (donation.PaymentMethod.ToLower() == "ach")
                {
                    try
                    {
                        var successUrl = $"{Request.Scheme}://{Request.Host}/Donate/StripeSuccess";
                        var cancelUrl = $"{Request.Scheme}://{Request.Host}/Donate/StripeCancel";

                        var checkoutUrl = await _stripeService.CreateACHCheckoutSession(donation, successUrl, cancelUrl);

                        return Json(new
                        {
                            success = true,
                            redirectUrl = checkoutUrl,
                            paymentProvider = "Stripe",
                            message = "Redirecting to secure payment...",
                            transactionId = donation.TransactionId,
                            donationId = donation.Id
                        });
                    }
                    catch (Exception stripeEx)
                    {
                        // Log Stripe error
                        donation.Status = "Failed";
                        donation.Notes = $"Stripe error: {stripeEx.Message}";
                        _context.SaveChanges();

                        return Json(new
                        {
                            success = false,
                            message = "Failed to create ACH payment. Please try again or use another payment method."
                        });
                    }
                }

                // For other payment methods (Zelle, Venmo)
                return Json(new
                {
                    success = true,
                    redirectUrl = (string?)null,
                    paymentProvider = donation.PaymentMethod,
                    message = "Thank you for your donation! Your transaction has been recorded.",
                    transactionId = donation.TransactionId,
                    donationId = donation.Id
                });
            }
            catch (Exception ex)
            {
                // Log the error
                return Json(new
                {
                    success = false,
                    message = "An error occurred while processing your donation. Please try again or contact us for assistance."
                });
            }
        }

        public async Task<IActionResult> PayPalSuccess(string token, int donationId)
        {
            try
            {
                var donation = _context.Donations.Find(donationId);
                if (donation == null)
                {
                    TempData["Error"] = "Donation not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Capture the PayPal payment
                var captured = await _payPalService.CaptureOrder(token);

                if (captured)
                {
                    donation.Status = "Completed";
                    donation.ProcessedDate = DateTime.UtcNow;
                    donation.Notes = $"PayPal Order ID: {token}";
                    _context.SaveChanges();

                    TempData["Success"] = $"Thank you for your donation! Your payment has been processed successfully. Transaction ID: {donation.TransactionId}";
                }
                else
                {
                    donation.Status = "Failed";
                    donation.Notes = $"PayPal capture failed for Order ID: {token}";
                    _context.SaveChanges();

                    TempData["Error"] = "Payment capture failed. Please contact us for assistance.";
                }

                return View("Success", donation);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred processing your payment.";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult PayPalCancel(int donationId)
        {
            var donation = _context.Donations.Find(donationId);
            if (donation != null)
            {
                donation.Status = "Cancelled";
                donation.Notes = "Payment cancelled by user";
                _context.SaveChanges();
            }

            TempData["Error"] = "Payment was cancelled. You can try again if you'd like.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> StripeSuccess(string session_id, int donationId)
        {
            try
            {
                var donation = _context.Donations.Find(donationId);
                if (donation == null)
                {
                    TempData["Error"] = "Donation not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Verify the session
                var session = await _stripeService.GetSession(session_id);

                if (session.PaymentStatus == "paid" || session.Status == "complete")
                {
                    donation.Status = "Completed";
                    donation.ProcessedDate = DateTime.UtcNow;
                    donation.Notes = $"Stripe Session ID: {session_id}";

                    // If it's a recurring donation, set up the subscription
                    if (donation.Frequency == "Monthly" || donation.Frequency == "Annual")
                    {
                        if (!string.IsNullOrEmpty(session.SetupIntentId))
                        {
                            var recurringSuccess = await _stripeService.CreateRecurringPayment(
                                session.SetupIntentId,
                                donation.Id,
                                donation.Amount,
                                donation.Frequency
                            );

                            if (recurringSuccess)
                            {
                                donation.Notes += " | Recurring payment setup successful";
                            }
                        }
                    }

                    _context.SaveChanges();

                    TempData["Success"] = $"Thank you for your donation! Your payment has been processed successfully. Transaction ID: {donation.TransactionId}";
                }
                else
                {
                    donation.Status = "Pending";
                    donation.Notes = $"Stripe payment pending - Session ID: {session_id}";
                    _context.SaveChanges();

                    TempData["Success"] = "Your donation is being processed. You'll receive a confirmation email once it's complete.";
                }

                return View("Success", donation);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred processing your payment.";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult StripeCancel(int donationId)
        {
            var donation = _context.Donations.Find(donationId);
            if (donation != null)
            {
                donation.Status = "Cancelled";
                donation.Notes = "Stripe payment cancelled by user";
                _context.SaveChanges();
            }

            TempData["Error"] = "Payment was cancelled. You can try again if you'd like.";
            return RedirectToAction(nameof(Index));
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
