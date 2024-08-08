using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Motax.Helpers;
using Motax.Models;
using Motax.Services;
using Motax.ViewModels;
using System.Linq;
using System.Security.Claims;

using System.Threading.Tasks;

namespace Motax.Controllers
{
    public class OrderController : Controller
    {
        private readonly MotaxContext db;
        private readonly ILogger<OrderController> logger;
        private readonly PaypalClient _paypalClient;
        private readonly IVnPayService _vnPayservice;

        public OrderController(MotaxContext context, ILogger<OrderController> logger, PaypalClient paypalClient, IVnPayService vnPayservice)
        {
            db = context;
            this.logger = logger;
            _paypalClient = paypalClient;
            _vnPayservice = vnPayservice;
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(int carId)
        {
            // Get the logged-in user's ID from claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                TempData["error"] = "User not logged in.";
                return RedirectToAction("Login", "Secure");
            }

            var user = await db.Accounts.FindAsync(int.Parse(userId));
            var carDetails = await db.Cars.FindAsync(carId);

            if (user == null || carDetails == null)
            {
                TempData["error"] = "User or Car details not found.";
                return RedirectToAction("Index", "Home");
            }

            // Check if all required user fields are present
            if (string.IsNullOrWhiteSpace(user.Username) || user.Username.ToLower() == "null" ||
                string.IsNullOrWhiteSpace(user.Email) || user.Email.ToLower() == "null" ||
                string.IsNullOrWhiteSpace(user.Address) || user.Address.ToLower() == "null" ||
                string.IsNullOrWhiteSpace(user.Phone) || user.Phone.ToLower() == "null" ||
                string.IsNullOrWhiteSpace(user.Gender) || user.Gender.ToLower() == "null" ||
                !user.Dob.HasValue)
            {
                TempData["error"] = "Please complete your profile before proceeding with the order.";
                return RedirectToAction("Profile", "Secure");
            }

            var viewModel = new CheckoutViewModel
            {
                User = user,
                CarDetails = carDetails,
                Price = carDetails.Price ?? 0 // Use null-coalescing operator to provide a default value if null
            };

            return View(viewModel);
        }



        [HttpPost]
        public async Task<IActionResult> CreateOrder(int userId, int carId, double totalPrice)
        {

            var user = await db.Accounts.FindAsync(userId);
            var carDetails = await db.Cars.FindAsync(carId);
            if (carDetails == null)
            {
                TempData["error"] = "Car details not found.";
                return RedirectToAction("Index", "Home");
            }
            var dealerDetails = await db.Dealers.FindAsync(carDetails.DealerId);


            if (user == null || carDetails == null || dealerDetails == null)
            {
                TempData["error"] = "User, Car, or Dealer details not found.";
                return RedirectToAction("Index", "Home");
            }

            var orderStatus = db.OrderStatus.FirstOrDefault(os => os.Status == "Pending");

            var order = new Order
            {
                AccountId = user.Id,
                CarId = carDetails.Id,
                DealerId = dealerDetails.Id,
                OrderCode = Guid.NewGuid().ToString(),
                OrderDate = DateTime.Now,
                DeliveryDate = DateTime.Now.AddDays(7),
                Username = user.Username ?? "",
                Address = user.Address ?? "",
                Phone = user.Phone ?? "",
                HowToPay = "Credit Card",
                HowToTransport = "Delivery",
                TransportFee = 0,
                Status = 1,
                CreateDay = DateTime.Now,
                UpdateDay = DateTime.Now,
                TotalAmount = totalPrice,
                OrderStatusId = orderStatus?.Id ?? 1 // Default to 1 if not found
            };

            db.Orders.Add(order);
            await db.SaveChangesAsync(); // Save the order to get the order ID

            var orderDetail = new OrderDetail
            {
                OrderId = order.Id, // Use the order ID
                CarId = carDetails.Id,
                Quantity = 1, // Assume quantity 1 for simplicity
                Price = carDetails.Price ?? 0,
                Discount = 0 // Assume no discount for simplicity
            };

            db.OrderDetails.Add(orderDetail);

            // Mark car as unavailable
            carDetails.IsAvailable = false;
            db.Cars.Update(carDetails);

            await db.SaveChangesAsync();

            TempData["success"] = "Order created successfully.";
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public IActionResult Calculate(double price, double rate, int loanPeriod, double downPayment)
        {
            double monthlyRate = rate / 100 / 12;
            int totalPayments = loanPeriod * 12;
            double loanAmount = price - downPayment;
            double monthlyPayment = (loanAmount * monthlyRate) / (1 - Math.Pow(1 + monthlyRate, -totalPayments));

            ViewBag.MonthlyPayment = monthlyPayment;
            ViewBag.TotalPayment = monthlyPayment * totalPayments;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MyOrder()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userId, out int intUserId))
                {
                    var orders = await db.Orders
                                         .Where(o => o.AccountId == intUserId)
                                         .Include(o => o.OrderStatus)
                                         .ToListAsync();
                    var invoices = await db.Invoices
                                           .Where(i => i.UserId == intUserId)
                                           .Include(i => i.CarRegistration)
                                           .ToListAsync();

                    var viewModel = new MyOrderViewModel
                    {
                        Orders = orders,
                        Invoices = invoices
                    };

                    return View(viewModel);
                }
            }
            return RedirectToAction("Login", "Secure");
        }

        [HttpGet]
        [Route("Order/Invoices")]
        public async Task<IActionResult> Invoices()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userId, out int intUserId))
                {
                    logger.LogInformation($"Fetching invoices for user {intUserId}");
                    var invoices = await db.Invoices
                                           .Where(i => i.UserId == intUserId)
                                           .Include(i => i.CarRegistration)
                                           .ToListAsync();

                    if (invoices.Any())
                    {
                        logger.LogInformation($"Found {invoices.Count} invoices for user {intUserId}");
                        foreach (var invoice in invoices)
                        {
                            logger.LogInformation($"Invoice {invoice.Id}: Date={invoice.InvoiceDate}, TotalAmount={invoice.TotalAmount}, Status={invoice.Status}");
                        }
                    }
                    else
                    {
                        logger.LogWarning($"No invoices found for user {intUserId}");
                    }

                    return View(invoices);
                }
                else
                {
                    logger.LogError("Failed to parse userId from claims");
                }
            }
            else
            {
                logger.LogWarning("User not authenticated");
            }

            return RedirectToAction("Login", "Secure");
        }

        [HttpGet]
        [Route("Order/InvoiceDetail/{id}")]
        public async Task<IActionResult> InvoiceDetail(int id)
        {
            var invoice = await db.Invoices
                                  .Include(i => i.CarRegistration)
                                  .ThenInclude(cr => cr.Car)
                                  .ThenInclude(car => car.Dealer)
                                  .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
            {
                TempData["error"] = "Invoice not found.";
                return RedirectToAction("Invoices");
            }

            ViewBag.PaypalClientId = _paypalClient.ClientId;
            return View(invoice);
        }

        [Authorize]
        public IActionResult PaymentSuccess()
        {
            return View();
        }

        [Authorize]
        public IActionResult PaymentFail()
        {
            return View("");
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var order = await db.Orders.FindAsync(orderId);
            if (order != null)
            {
                var orderStatus = db.OrderStatus.FirstOrDefault(os => os.Status == "Cancelled");
                order.OrderStatusId = orderStatus?.Id ?? 0;
                db.Orders.Update(order);
                await db.SaveChangesAsync();
                TempData["success"] = "Order cancelled successfully.";
            }
            else
            {
                TempData["error"] = "Order not found.";
            }
            return RedirectToAction("MyOrder");
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmOrder(int orderId)
        {
            var order = await db.Orders.FindAsync(orderId);
            if (order != null)
            {
                var orderStatus = db.OrderStatus.FirstOrDefault(os => os.Status == "Customer Confirmed");
                order.OrderStatusId = orderStatus?.Id ?? 0;
                db.Orders.Update(order);
                await db.SaveChangesAsync();
                TempData["success"] = "Order confirmed successfully.";
            }
            else
            {
                TempData["error"] = "Order not found.";
            }
            return RedirectToAction("MyOrder");
        }

        #region Payment Paypal
        public class CreatePaypalOrderRequest
        {
            public int OrderId { get; set; }
        }

        [Authorize]
        [HttpPost("/order/create-paypal-order")]
        public async Task<IActionResult> CreatePaypalOrder([FromBody] CreatePaypalOrderRequest request, CancellationToken cancellationToken)
        {
            var invoice = await db.Invoices.FindAsync(request.OrderId);
            if (invoice == null)
            {
                return BadRequest(new { message = "Invoice not found." });
            }

            var totalAmount = invoice.TotalAmount.ToString("F2"); // Assuming TotalAmount is a double or decimal
            var currency = "USD";
            var invoiceReference = invoice.Id.ToString(); // Use invoice ID as reference

            try
            {
                var response = await _paypalClient.CreateOrder(totalAmount, currency, invoiceReference);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }

        [Authorize]
        [HttpPost("/order/capture-paypal-order")]
        public async Task<IActionResult> CapturePaypalOrder(string orderID, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _paypalClient.CaptureOrder(orderID);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }



        #endregion

        #region Payment VNPay

        [Authorize]
        [HttpPost("/order/create-vnpay-order")]
        public IActionResult CreateVnPayOrder(int invoiceId)
        {
            var invoice = db.Invoices.Include(i => i.CarRegistration).ThenInclude(cr => cr.Car).FirstOrDefault(i => i.Id == invoiceId);
            if (invoice == null)
            {
                TempData["error"] = "Invoice not found.";
                return RedirectToAction("InvoiceDetail", new { id = invoiceId });
            }

            var vnPayModel = new VnPaymentRequestModel
            {
                Amount = invoice.TotalAmount,
                CreatedDate = DateTime.Now,
                Description = $"Payment for invoice {invoice.Id}",
                FullName = invoice.CarRegistration.CustomerName,
                OrderId = invoice.Id // Ensure this is an integer
            };

            return Redirect(_vnPayservice.CreatePaymentUrl(HttpContext, vnPayModel));
        }



        [Authorize]
        [HttpGet("/order/vnpay-callback")]
        public IActionResult PaymentCallBack()
        {
            var response = _vnPayservice.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["error"] = $"VNPay payment error: {response?.VnPayResponseCode}";
                return RedirectToAction("PaymentFail");
            }

            // Retrieve the invoice using the order ID passed from VNPay
            if (!int.TryParse(response.OrderId, out int invoiceId))
            {
                TempData["error"] = "Invalid Order ID.";
                return RedirectToAction("PaymentFail");
            }

            var invoice = db.Invoices.FirstOrDefault(i => i.Id == invoiceId);
            if (invoice != null)
            {
                invoice.Status = "Paid";
                db.SaveChanges();
            }

            TempData["success"] = "VNPay payment successful";
            return RedirectToAction("PaymentSuccess");
        }

        #endregion

        #region COD
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateCodOrder(int invoiceId)
        {
            var invoice = await db.Invoices.FindAsync(invoiceId);
            if (invoice == null)
            {
                TempData["error"] = "Invoice not found.";
                return RedirectToAction("InvoiceDetail", new { id = invoiceId });
            }

            // Update invoice status to Paid or COD, as needed
            invoice.Status = "COD"; // Or any appropriate status
            db.Invoices.Update(invoice);
            await db.SaveChangesAsync();

            TempData["success"] = "Payment successful.";
            return RedirectToAction("PaymentSuccess");
        }
        #endregion

    }
}

