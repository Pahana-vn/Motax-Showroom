using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Motax.Models;
using Motax.ViewModels;

namespace Motax.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Reports")]
    [Authorize(Policy = "CheckAdmin")]
    public class ReportsController : Controller
    {
        private readonly MotaxContext db;

        public ReportsController(MotaxContext context)
        {
            db = context;
        }

        [Route("StockAvailability")]
        public async Task<IActionResult> StockAvailability()
        {
            var cars = await db.Cars
                .Include(c => c.Brand)
                .Include(c => c.Dealer)
                .Where(c => c.IsAvailable == true) // Ensure the check for IsAvailable is correct
                .ToListAsync();

            if (!cars.Any())
            {
                TempData["error"] = "No available cars found.";
                return View(new List<StockAvailabilityViewModel>());
            }

            var stock = cars.Select(c => new StockAvailabilityViewModel
            {
                CarName = c.Name,
                DealerName = c.Dealer != null ? c.Dealer.Name : "N/A",
                BrandName = c.Brand != null ? c.Brand.Name : "N/A",
                Year = c.Year,
                Price = c.Price,
                WarehouseLocation = c.Dealer != null ? c.Dealer.Address : "N/A"
            }).ToList();

            return View(stock);
        }

        [Route("CustomerInformation")]
        public async Task<IActionResult> CustomerInformation()
        {
            var customers = await db.Accounts
                .Where(a => a.RoleId == 2 && a.Orders.Any()) // Filter by RoleId and check if they have orders
                .Select(a => new CustomerInformationViewModel
                {
                    Username = a.Username,
                    Email = a.Email,
                    Phone = a.Phone,
                    Address = a.Address,
                    Dob = a.Dob,
                    Gender = a.Gender
                })
                .ToListAsync();

            return View(customers);
        }

        [Route("VehicleMasterInformation")]
        public async Task<IActionResult> VehicleMasterInformation()
        {
            var vehicles = await db.Cars
                .Include(c => c.Brand)
                .Include(c => c.Dealer)
                .Select(c => new VehicleMasterInformationViewModel
                {
                    Name = c.Name,
                    Brand = c.Brand != null ? c.Brand.Name : "N/A",
                    DealerName = c.Dealer != null ? c.Dealer.Name : "N/A",
                    DealerAddress = c.Dealer != null ? c.Dealer.Address : "N/A",
                    DealerPhone = c.Dealer != null ? c.Dealer.Phone : "N/A",
                    Year = c.Year,
                    Price = c.Price,
                    Status = c.Status,
                    FuelType = c.FuelType,
                    Transmission = c.Transmission,
                    Mileage = c.Mileage,
                    Color = c.Color
                })
                .ToListAsync();

            return View(vehicles);
        }

        [Route("AllotmentDetails")]
        public async Task<IActionResult> AllotmentDetails()
        {
            var allotments = await db.DeliveryUnits
                .Include(d => d.Car)
                .Include(d => d.Dealer)
                .Include(d => d.ServiceUnit)
                .Select(d => new AllotmentDetailsViewModel
                {
                    CarName = d.Car.Name,
                    DealerName = d.Dealer.Name,
                    DealerAddress = d.Dealer.Address,
                    DealerPhone = d.Dealer.Phone,
                    ScheduledPickupDate = d.ScheduledPickupDate,
                    PickupLocation = d.PickupLocation,
                    CustomerContact = d.CustomerContact,
                    IsConfirmed = d.IsConfirmed
                })
                .ToListAsync();

            return View(allotments);
        }

        [Route("WaitingListDetails")]
        public async Task<IActionResult> WaitingListDetails()
        {
            var waitingList = await db.Orders
                .Include(o => o.Account)
                .Include(o => o.Car)
                .Include(o => o.OrderStatus)
                .Where(o => o.Status == 1) // Assuming status 1 is 'Pending'
                .Select(o => new WaitingListDetailsViewModel
                {
                    Username = o.Account.Username,
                    CarName = o.Car.Name,
                    OrderDate = o.OrderDate,
                    DeliveryDate = o.DeliveryDate,
                    StatusDescription = o.OrderStatus.Status // Assuming 'Status' is the description in OrderStatus
                })
                .ToListAsync();

            return View(waitingList);
        }



    }
}
