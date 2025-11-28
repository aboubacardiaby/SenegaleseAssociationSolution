using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;
using SenegaleseAssociation.Models;

namespace SenegaleseAssociation.Services
{
    public class PayPalService
    {
        private readonly IConfiguration _configuration;
        private readonly PayPalHttpClient _client;

        public PayPalService(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = GetPayPalClient();
        }

        private PayPalHttpClient GetPayPalClient()
        {
            var mode = _configuration["PayPal:Mode"];
            var clientId = _configuration["PayPal:ClientId"];
            var clientSecret = _configuration["PayPal:ClientSecret"];

            PayPalEnvironment environment = mode == "live"
                ? new LiveEnvironment(clientId, clientSecret)
                : new SandboxEnvironment(clientId, clientSecret);

            return new PayPalHttpClient(environment);
        }

        public async Task<string> CreateOrder(Donation donation)
        {
            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(BuildRequestBody(donation));

            try
            {
                var response = await _client.Execute(request);
                var result = response.Result<Order>();

                // Find the approval URL
                var approvalUrl = result.Links.FirstOrDefault(link => link.Rel == "approve")?.Href;

                return approvalUrl ?? string.Empty;
            }
            catch (HttpException ex)
            {
                // Log the error
                Console.WriteLine($"PayPal API Error: {ex.Message}");
                throw new Exception("Failed to create PayPal order", ex);
            }
        }

        public async Task<bool> CaptureOrder(string orderId)
        {
            var request = new OrdersCaptureRequest(orderId);
            request.RequestBody(new OrderActionRequest());

            try
            {
                var response = await _client.Execute(request);
                var result = response.Result<Order>();

                return result.Status == "COMPLETED";
            }
            catch (HttpException ex)
            {
                Console.WriteLine($"PayPal Capture Error: {ex.Message}");
                return false;
            }
        }

        private OrderRequest BuildRequestBody(Donation donation)
        {
            var returnUrl = _configuration["PayPal:ReturnUrl"];
            var cancelUrl = _configuration["PayPal:CancelUrl"];

            var orderRequest = new OrderRequest()
            {
                CheckoutPaymentIntent = "CAPTURE",
                ApplicationContext = new ApplicationContext
                {
                    ReturnUrl = $"{returnUrl}?donationId={donation.Id}",
                    CancelUrl = $"{cancelUrl}?donationId={donation.Id}",
                    BrandName = "Senegalese Association of Minnesota",
                    LandingPage = "BILLING",
                    UserAction = "PAY_NOW"
                },
                PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new PurchaseUnitRequest
                    {
                        ReferenceId = donation.TransactionId,
                        Description = GetDonationDescription(donation),
                        CustomId = donation.Id.ToString(),
                        SoftDescriptor = "SAM Donation",
                        AmountWithBreakdown = new AmountWithBreakdown
                        {
                            CurrencyCode = "USD",
                            Value = donation.Amount.ToString("F2")
                        }
                    }
                }
            };

            return orderRequest;
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

            return $"{frequencyText} donation to Senegalese Association of Minnesota";
        }

        public async Task<Order> GetOrderDetails(string orderId)
        {
            var request = new OrdersGetRequest(orderId);

            try
            {
                var response = await _client.Execute(request);
                return response.Result<Order>();
            }
            catch (HttpException ex)
            {
                Console.WriteLine($"PayPal Get Order Error: {ex.Message}");
                throw new Exception("Failed to get PayPal order details", ex);
            }
        }
    }
}
