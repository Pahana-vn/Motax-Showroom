using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Motax.Models;
using Motax.ViewModels;

namespace Motax.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/ServiceUnit")]
    [Authorize(Policy = "CheckAdmin")]
    public class ServiceUnitController : Controller
    {
        private readonly MotaxContext db;

        public ServiceUnitController(MotaxContext context)
        {
            db = context;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var serviceUnits = await db.ServiceUnits
                .Include(su => su.Car)
                .Include(su => su.Dealer)
                .OrderByDescending(su => su.Id)
                .ToListAsync();

            return View(serviceUnits);
        }

        [Route("Index2")]
        public async Task<IActionResult> Index2()
        {
            var serviceUnits = await db.ServiceUnits
                .Include(su => su.Car)
                .Include(su => su.Dealer)
                .Where(su => su.ServiceDetails == "Prepare the vehicle for delivery.")
                .OrderByDescending(su => su.Id)
                .ToListAsync();

            return View(serviceUnits);
        }

        [Route("Index3")]
        public async Task<IActionResult> Index3()
        {
            var serviceUnits = await db.ServiceUnits
                .Include(su => su.Car)
                .Include(su => su.Dealer)
                .Where(su => su.ServiceDetails == "Prepare the vehicle for delivery.")
                .OrderByDescending(su => su.Id)
                .ToListAsync();

            return View(serviceUnits);
        }


        [Route("Create")]
        [HttpGet]
        public IActionResult Create(int carRegistrationId)
        {
            var carRegistration = db.CarRegistrations.Include(cr => cr.Car).Include(cr => cr.Car.Dealer).FirstOrDefault(cr => cr.Id == carRegistrationId);

            if (carRegistration == null)
            {
                TempData["error"] = "Car registration not found.";
                return RedirectToAction("Index", "Home");
            }

            var viewModel = new ServiceUnitViewModel
            {
                CarId = carRegistration.CarId,
                DealerId = carRegistration.Car?.DealerId ?? 0,
                CarRegistrationId = carRegistrationId,
                ServiceDate = DateTime.Now
            };

            return View(viewModel);
        }

        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceUnitViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var dealer = await db.Dealers.FindAsync(viewModel.DealerId);
                if (dealer == null)
                {
                    TempData["error"] = "Invalid dealer.";
                    return RedirectToAction("Create", new { carRegistrationId = viewModel.CarRegistrationId });
                }

                var serviceUnit = new ServiceUnit
                {
                    CarId = viewModel.CarId,
                    DealerId = viewModel.DealerId,
                    ServiceDate = viewModel.ServiceDate,
                    ServiceDetails = viewModel.ServiceDetails,
                    IsCompleted = viewModel.IsCompleted,
                    PickupDate = viewModel.PickupDate,
                    CarRegistrationId = viewModel.CarRegistrationId
                };

                db.ServiceUnits.Add(serviceUnit);
                await db.SaveChangesAsync();

                TempData["success"] = "Service unit created successfully!";
                return RedirectToAction("Index", "ServiceUnit");
            }

            return View(viewModel);
        }
        #region Create 2
        [Route("Create2")]
        [HttpGet]
        public IActionResult Create2(int carId, int dealerId)
        {
            var car = db.Cars.Include(c => c.Dealer).FirstOrDefault(c => c.Id == carId);
            if (car == null || car.Dealer == null)
            {
                TempData["error"] = "Car or Dealer not found.";
                return RedirectToAction("Index", "Order");
            }

            var viewModel = new ServiceUnitViewModel
            {
                CarId = carId,
                DealerId = dealerId,
                ServiceDate = DateTime.Now,
                PickupDate = DateTime.Now.AddDays(7), // Ngày lấy xe ví dụ
                ServiceDetails = "Prepare the vehicle for delivery."
            };

            return View(viewModel);
        }


        [Route("Create2")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create2(ServiceUnitViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var dealer = await db.Dealers.FindAsync(viewModel.DealerId);
                if (dealer == null)
                {
                    TempData["error"] = "Invalid dealer.";
                    return RedirectToAction("Create2", new { carId = viewModel.CarId, dealerId = viewModel.DealerId });
                }

                var serviceUnit = new ServiceUnit
                {
                    CarId = viewModel.CarId,
                    DealerId = viewModel.DealerId,
                    ServiceDate = viewModel.ServiceDate,
                    ServiceDetails = viewModel.ServiceDetails,
                    IsCompleted = viewModel.IsCompleted,
                    PickupDate = viewModel.PickupDate,
                    CarRegistrationId = null
                };

                db.ServiceUnits.Add(serviceUnit);
                await db.SaveChangesAsync();

                TempData["success"] = "Service unit created successfully!";
                return RedirectToAction("Index3");
            }

            return View(viewModel);
        }
        #endregion
    }
}
