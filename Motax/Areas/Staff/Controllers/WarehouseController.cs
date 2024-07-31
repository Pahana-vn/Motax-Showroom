using Microsoft.AspNetCore.Mvc;
using Motax.Models;
using System.Threading.Tasks;
using System.Linq;

namespace Motax.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Route("Staff/Warehouse")]
    public class WarehouseController : Controller
    {
        private readonly MotaxContext db;

        public WarehouseController(MotaxContext context)
        {
            db = context;
        }

        [HttpPost]
        [Route("UpdateAvailability")]
        public async Task<IActionResult> UpdateAvailability(int carId, bool isAvailable)
        {
            var car = await db.Cars.FindAsync(carId);
            if (car == null)
            {
                TempData["error"] = "Car not found.";
                return RedirectToAction("ManageAvailability");
            }

            car.IsAvailable = isAvailable;
            db.Cars.Update(car);
            await db.SaveChangesAsync();

            TempData["success"] = "Car availability updated successfully.";
            return RedirectToAction("ManageAvailability");
        }

        [HttpGet]
        [Route("ManageAvailability")]
        public IActionResult ManageAvailability()
        {
            var cars = db.Cars.ToList();
            return View(cars);
        }
    }
}
