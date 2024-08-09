using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Motax.Models;

namespace Motax.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/OrderAccessories")]
    [Authorize(Policy = "CheckAdmin")]
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

        [HttpPost]
        [Route("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Fetch the OrderAccessories record with the specified id
            var orderAccessories = await db.OrderAccessories
                .Include(o => o.OrderDetailAccessories) // Include related OrderDetailAccessories
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orderAccessories == null)
            {
                return NotFound();
            }

            // Remove the related OrderDetailAccessories records first
            db.OrderDetailAccessories.RemoveRange(orderAccessories.OrderDetailAccessories);

            // Remove the OrderAccessories record
            db.OrderAccessories.Remove(orderAccessories);

            // Save the changes to the database
            await db.SaveChangesAsync();

            TempData["success"] = "Order Accessories deleted success!";

            // Redirect to the Index action after deletion
            return RedirectToAction(nameof(Index));
        }
    }
}
