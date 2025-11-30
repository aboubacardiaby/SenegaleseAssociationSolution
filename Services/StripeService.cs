using Stripe;
using Stripe.Checkout;
using SenegaleseAssociation.Models;

namespace SenegaleseAssociation.Services
{
    public class StripeService
    {
        private readonly IConfiguration _configuration;

        public StripeService(IConfiguration configuration)
        {
            _configuration = configuration;
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
        }

        public async Task<string> CreateACHCheckoutSession(Donation donation, string successUrl, string cancelUrl)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "us_bank_account"  // ACH Direct Debit
                },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = GetDonationDescription(donation),
                                Description = donation.Message ?? "Donation to Senegalese Association of Minnesota"
                            },
                            UnitAmount = (long)(donation.Amount * 100), // Convert to cents
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = $"{successUrl}?session_id={{CHECKOUT_SESSION_ID}}&donationId={donation.Id}",
                CancelUrl = $"{cancelUrl}?donationId={donation.Id}",
                CustomerEmail = donation.DonorEmail,
                Metadata = new Dictionary<string, string>
                {
                    { "donationId", donation.Id.ToString() },
                    { "transactionId", donation.TransactionId },
                    { "frequency", donation.Frequency }
                },
                PaymentIntentData = new SessionPaymentIntentDataOptions
                {
                    Description = $"Donation - {donation.TransactionId}",
                    Metadata = new Dictionary<string, string>
                    {
                        { "donationId", donation.Id.ToString() },
                        { "donorName", donation.DonorName },
                        { "transactionId", donation.TransactionId }
                    }
                }
            };

            // Add setup mode for recurring donations
            if (donation.Frequency == "Monthly" || donation.Frequency == "Annual")
            {
                options.Mode = "setup";
                options.SetupIntentData = new SessionSetupIntentDataOptions
                {
                    Metadata = new Dictionary<string, string>
                    {
                        { "donationId", donation.Id.ToString() },
                        { "frequency", donation.Frequency },
                        { "amount", donation.Amount.ToString() }
                    }
                };
            }

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            return session.Url;
        }

        public async Task<bool> VerifyPaymentSession(string sessionId)
        {
            try
            {
                var service = new SessionService();
                var session = await service.GetAsync(sessionId);

                return session.PaymentStatus == "paid" || session.Status == "complete";
            }
            catch
            {
                return false;
            }
        }

        public async Task<Session> GetSession(string sessionId)
        {
            var service = new SessionService();
            return await service.GetAsync(sessionId);
        }

        public async Task<PaymentIntent> GetPaymentIntent(string paymentIntentId)
        {
            var service = new PaymentIntentService();
            return await service.GetAsync(paymentIntentId);
        }

        private string GetDonationDescription(Donation donation)
        {
            var frequencyText = donation.Frequency switch
            {
                "OneTime" => "One-time",
                "Monthly" => "Monthly",
                "Annual" => "Annual",
                _ => "One-time"
            };

            return $"{frequencyText} Donation - ${donation.Amount:F2}";
        }

        public async Task<bool> CreateRecurringPayment(string setupIntentId, int donationId, decimal amount, string frequency)
        {
            try
            {
                // Get the SetupIntent to retrieve payment method
                var setupIntentService = new SetupIntentService();
                var setupIntent = await setupIntentService.GetAsync(setupIntentId);

                if (setupIntent.Status != "succeeded")
                {
                    return false;
                }

                // Create a subscription or scheduled payment based on frequency
                var interval = frequency == "Monthly" ? "month" : "year";

                // Create a product for the donation
                var productService = new ProductService();
                var product = await productService.CreateAsync(new ProductCreateOptions
                {
                    Name = $"Recurring Donation - {frequency}"
                });

                // Create a price for the product
                var priceService = new PriceService();
                var price = await priceService.CreateAsync(new PriceCreateOptions
                {
                    Product = product.Id,
                    Currency = "usd",
                    UnitAmount = (long)(amount * 100),
                    Recurring = new PriceRecurringOptions
                    {
                        Interval = interval
                    }
                });

                // Create a customer
                var customerService = new CustomerService();
                var customer = await customerService.CreateAsync(new CustomerCreateOptions
                {
                    PaymentMethod = setupIntent.PaymentMethodId,
                    InvoiceSettings = new CustomerInvoiceSettingsOptions
                    {
                        DefaultPaymentMethod = setupIntent.PaymentMethodId
                    },
                    Metadata = new Dictionary<string, string>
                    {
                        { "donationId", donationId.ToString() }
                    }
                });

                // Create a subscription
                var subscriptionService = new SubscriptionService();
                var subscription = await subscriptionService.CreateAsync(new SubscriptionCreateOptions
                {
                    Customer = customer.Id,
                    Items = new List<SubscriptionItemOptions>
                    {
                        new SubscriptionItemOptions
                        {
                            Price = price.Id,
                        },
                    },
                    Metadata = new Dictionary<string, string>
                    {
                        { "donationId", donationId.ToString() },
                        { "frequency", frequency }
                    }
                });

                return subscription.Status == "active" || subscription.Status == "trialing";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Stripe recurring payment error: {ex.Message}");
                return false;
            }
        }
    }
}
