using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Motax.Models;

namespace Motax.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Route("Staff/OrderAccessories")]
    [Authorize(Policy = "CheckAdminOrStaff")]
    public class OrderAccessoriesController : Controller
    {
        private readonly MotaxContext db;

        public OrderAccessoriesController(MotaxContext context)
        {
            db = context;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await db.OrderAccessories.OrderByDescending(b => b.Id).ToListAsync());
        }

        [Route("Detail")]
        public async Task<IActionResult> Detail(int id)
        {
            var orderDetails = await db.OrderDetailAccessories
                .Where(od => od.OrderAccessoriesId == id)
                .Include(od => od.Accessory)
                .ToListAsync();

            if (orderDetails == null || !orderDetails.Any())
            {
                return NotFound();
            }

            return View(orderDetails);
        }
    }
}
