using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Motax.Models;
using Motax.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Motax.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Order")]
    public class OrderController : Controller
    {
        private readonly MotaxContext db;

        public OrderController(MotaxContext context)
        {
            db = context;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var orders = await db.Orders
                .Include(o => o.OrderStatus) // Bao gồm OrderStatus
                .OrderByDescending(b => b.Id)
                .ToListAsync();

            return View(orders);
        }

        [Route("Index2")]
        public async Task<IActionResult> Index2()
        {
            var orders = await db.Orders
                .Include(o => o.OrderStatus) // Bao gồm OrderStatus
                .OrderByDescending(b => b.Id)
                .ToListAsync();

            return View(orders);
        }

        [Route("Index3")]
        public async Task<IActionResult> Index3()
        {
            var orders = await db.Orders
                .Include(o => o.OrderStatus)
                .Where(o => o.OrderStatus.Status == "Customer Confirmed")
                .OrderByDescending(b => b.Id)
                .ToListAsync();

            return View(orders);
        }

        #region Detail
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var orderDetails = await db.OrderDetails
                .Include(od => od.Order)
                .ThenInclude(o => o.Account)
                .Include(od => od.Car)
                .ThenInclude(c => c.Dealer)
                .Where(od => od.OrderId == id)
                .Select(od => new OrderDetailViewModel
                {
                    Id = od.Id,
                    Username = od.Order.Account.Username,
                    CarName = od.Car.Name ?? "",
                    DealerName = od.Car.Dealer.Name,
                    Quantity = od.Quantity,
                    Price = od.Price,
                    Discount = od.Discount,
                    TotalAmount = od.Price * od.Quantity - od.Discount,
                    Address = od.Order.Address,
                    Phone = od.Order.Phone,
                    OrderDate = od.Order.OrderDate,
                    DeliveryDate = od.Order.DeliveryDate
                }).ToListAsync();

            if (!orderDetails.Any())
            {
                return Content("No order details found");
            }

            return View(orderDetails);
        }
        #endregion

        #region Detail-2
        [Route("Details2/{id}")]
        public async Task<IActionResult> Details2(int id)
        {
            var orderDetails = await db.OrderDetails
                .Include(od => od.Order)
                .ThenInclude(o => o.Account)
                .Include(od => od.Car)
                .ThenInclude(c => c.Dealer)
                .Where(od => od.OrderId == id)
                .Select(od => new OrderDetailViewModel
                {
                    Id = od.Id,
                    Username = od.Order.Account.Username,
                    CarName = od.Car.Name ?? "",
                    DealerName = od.Car.Dealer.Name,
                    Quantity = od.Quantity,
                    Price = od.Price,
                    Discount = od.Discount,
                    TotalAmount = od.Price * od.Quantity - od.Discount,
                    Address = od.Order.Address,
                    Phone = od.Order.Phone,
                    OrderDate = od.Order.OrderDate,
                    DeliveryDate = od.Order.DeliveryDate,
                    Car = od.Car // Chuyển thông tin Car vào viewModel
                }).ToListAsync();

            if (!orderDetails.Any())
            {
                return Content("No order details found");
            }

            return View(orderDetails.FirstOrDefault());
        }

        #endregion

        #region Admin: Xác nhận và hủy đơn hàng
        [HttpPost]
        [Route("CancelOrder")]
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
            return RedirectToAction("Index");
        }

        [Route("ConfirmOrder")]
        [HttpPost]
        public async Task<IActionResult> ConfirmOrder(int orderId)
        {
            var order = await db.Orders
                .Include(o => o.Car)
                .Include(o => o.Dealer)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (order != null)
            {
                var orderStatus = await db.OrderStatus.FirstOrDefaultAsync(os => os.Status == "Customer Confirmed");
                if (orderStatus != null)
                {
                    order.OrderStatusId = orderStatus.Id;
                    db.Orders.Update(order);
                    await db.SaveChangesAsync();

                    // Create a service order to prepare the vehicle
                    await CreateServiceOrder(order);

                    TempData["success"] = "Order confirmed and service order created successfully!";
                    return RedirectToAction("Details", new { id = orderId });
                }
            }

            TempData["error"] = "Order confirmation failed.";
            return RedirectToAction("Details", new { id = orderId });
        }
        #endregion

        private async Task CreateServiceOrder(Order order)
        {
            var serviceOrder = new ServiceUnit
            {
                CarId = order.CarId,
                DealerId = order.DealerId,
                ServiceDate = DateTime.Now,
                ServiceDetails = "Prepare the vehicle for delivery.",
                IsCompleted = false,
                PickupDate = order.DeliveryDate,
                CarRegistrationId = 0 // Set this appropriately if you have a CarRegistrationId
            };

            db.ServiceUnits.Add(serviceOrder);
            await db.SaveChangesAsync();
        }
    }
}
