using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Motax.Models;
using Motax.ViewModels;

namespace Motax.Areas.Shipper.Controllers
{
    [Area("Shipper")]
    [Route("Shipper/DeliveryUnit")]
    public class DeliveryUnitController : Controller
    {
        private readonly MotaxContext db;

        public DeliveryUnitController(MotaxContext context)
        {
            db = context;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var deliveryUnits = await db.DeliveryUnits
                .Include(du => du.Car)
                .Include(du => du.Dealer)
                .Include(du => du.ServiceUnit)
                .OrderByDescending(du => du.Id)
                .ToListAsync();

            return View(deliveryUnits);
        }

        [Route("Create")]
        [HttpGet]
        public IActionResult Create(int serviceUnitId)
        {
            var serviceUnit = db.ServiceUnits
                .Include(su => su.CarRegistration)
                .FirstOrDefault(su => su.Id == serviceUnitId);
            if (serviceUnit == null)
            {
                TempData["error"] = "Service unit not found.";
                return RedirectToAction("Index", "Home");
            }

            var viewModel = new DeliveryUnitViewModel
            {
                CarId = serviceUnit.CarId,
                DealerId = serviceUnit.DealerId,
                ServiceUnitId = serviceUnitId,
                CarRegistrationId = serviceUnit.CarRegistrationId,
                ScheduledPickupDate = serviceUnit.PickupDate ?? DateTime.Now,
                CustomerContact = serviceUnit.CarRegistration.CustomerName
            };

            return View(viewModel);
        }

        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeliveryUnitViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var deliveryUnit = new DeliveryUnit
                {
                    CarId = viewModel.CarId,
                    DealerId = viewModel.DealerId,
                    ServiceUnitId = viewModel.ServiceUnitId,
                    ScheduledPickupDate = viewModel.ScheduledPickupDate,
                    PickupLocation = viewModel.PickupLocation,
                    CustomerContact = viewModel.CustomerContact,
                    IsConfirmed = viewModel.IsConfirmed,
                    CarRegistrationId = viewModel.CarRegistrationId
                };

                db.DeliveryUnits.Add(deliveryUnit);
                await db.SaveChangesAsync();

                TempData["success"] = "Delivery unit created successfully!";
                return RedirectToAction("Index", "DeliveryUnit");
            }

            return View(viewModel);
        }

        [Route("Confirm/{id}")]
        [HttpGet]
        public async Task<IActionResult> Confirm(int id)
        {
            var deliveryUnit = await db.DeliveryUnits.FindAsync(id);
            if (deliveryUnit == null)
            {
                TempData["error"] = "Delivery unit not found.";
                return RedirectToAction("Index");
            }

            deliveryUnit.IsConfirmed = true;
            db.Update(deliveryUnit);
            await db.SaveChangesAsync();

            var order = await db.Orders.FirstOrDefaultAsync(o => o.CarId == deliveryUnit.CarId);
            if (order != null)
            {
                order.OrderStatusId = 3;
                db.Orders.Update(order);
                await db.SaveChangesAsync();
            }

            TempData["success"] = "Delivery unit confirmed successfully!";
            return RedirectToAction("Index");
        }
    }
}
