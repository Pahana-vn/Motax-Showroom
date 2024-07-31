using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Motax.Models;
using Motax.ViewModels;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Motax.Controllers
{
    public class OrderController : Controller
    {
        private readonly MotaxContext db;

        public OrderController(MotaxContext context)
        {
            db = context;
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(int carId)
        {
            // Get the logged-in user's ID from claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Secure");
            }

            var user = await db.Accounts.FindAsync(int.Parse(userId));
            var carDetails = await db.Cars.FindAsync(carId);

            if (user == null || carDetails == null)
            {
                TempData["error"] = "User or Car details not found.";
                return RedirectToAction("Index", "Home");
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

            var orderStatus = db.OrderStatus.FirstOrDefault(os => os.Status == "New Order");

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

                    var viewModel = new MyOrderViewModel
                    {
                        Orders = orders
                    };

                    return View(viewModel);
                }
            }
            return RedirectToAction("Login");
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
    }
}
