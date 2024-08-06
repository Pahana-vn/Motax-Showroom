using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Motax.Models;


namespace Motax.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Home")]
    [Authorize(Policy = "CheckAdmin")]
    public class HomeController : Controller
    {
        private readonly MotaxContext db;

        public HomeController(MotaxContext context)
        {
            db = context;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;

            var ordersToday = await db.Orders.Where(o => o.OrderDate.Date == today).ToListAsync();
            var earningsToday = ordersToday.Sum(o => o.TotalAmount);
            var totalEarnings = await db.Orders.SumAsync(o => o.TotalAmount);
            var productSold = await db.OrderDetails.SumAsync(od => od.Quantity);

            ViewBag.TodayOrders = ordersToday.Count;
            ViewBag.TodayEarnings = earningsToday;
            ViewBag.TotalEarnings = totalEarnings;
            ViewBag.ProductSold = productSold;

            return View();
        }

        [Route("GetTodayOrderData")]
        public async Task<IActionResult> GetTodayOrderData()
        {
            var today = DateTime.Today;
            var ordersToday = await db.Orders.Where(o => o.OrderDate.Date == today).ToListAsync();
            var earningsToday = ordersToday.Sum(o => o.TotalAmount);
            var totalEarnings = await db.Orders.SumAsync(o => o.TotalAmount);
            var productSold = await db.OrderDetails.SumAsync(od => od.Quantity);

            return Json(new
            {
                TodayOrders = ordersToday.Count,
                TodayEarnings = earningsToday,
                TotalEarnings = totalEarnings,
                ProductSold = productSold
            });
        }

        [Route("GetMonthlyOrderData")]
        public async Task<IActionResult> GetMonthlyOrderData(int month)
        {
            var startOfMonth = new DateTime(DateTime.Now.Year, month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var ordersInMonth = await db.Orders
                .Where(o => o.OrderDate >= startOfMonth && o.OrderDate <= endOfMonth)
                .ToListAsync();

            Console.WriteLine($"Found {ordersInMonth.Count} orders in month {month}");

            var totalOrders = ordersInMonth.Count;
            var totalEarnings = ordersInMonth.Sum(o => o.TotalAmount);

            return Json(new
            {
                OrdersCount = totalOrders,
                TotalEarnings = totalEarnings
            });
        }




    }
}
