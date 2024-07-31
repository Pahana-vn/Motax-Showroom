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

        [HttpPost]
        [Route("ConfirmOrder")]
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
            return RedirectToAction("Index");
        }
        #endregion

    }
}
