using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Motax.Models;
using Motax.ViewModels;
using System.Security.Claims;

namespace Motax.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/CarRegistration")]
    [Authorize]
    public class CarRegistrationController : Controller
    {
        private readonly MotaxContext db;

        public CarRegistrationController(MotaxContext context)
        {
            db = context;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await db.CarRegistrations.OrderByDescending(b => b.Id).ToListAsync());
        }

        [Route("Index2")]
        public async Task<IActionResult> Index2()
        {
            return View(await db.CarRegistrations.OrderByDescending(b => b.Id).ToListAsync());
        }



        [Route("Register")]
        [HttpGet]
        public async Task<IActionResult> Register(int carId)
        {
            var car = await db.Cars.FindAsync(carId);
            if (car == null)
            {
                TempData["error"] = "Car not found.";
                return RedirectToAction("Index", "Home");
            }

            var orderStatuses = await db.OrderStatus.ToListAsync();
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                TempData["error"] = "User not authenticated.";
                return RedirectToAction("Index", "Home");
            }

            int userId;
            if (!int.TryParse(userIdClaim.Value, out userId))
            {
                TempData["error"] = "Invalid user ID.";
                return RedirectToAction("Index", "Home");
            }

            var carPrice = car.Price ?? 0;

            var viewModel = new CarRegistrationVM
            {
                CarId = carId,
                UserId = userId,
                RegistrationDate = DateTime.Now,
                CarPrice = carPrice, // Set giá trị xe từ cơ sở dữ liệu
                RegistrationFee = carPrice * 0.02, // Phí đăng ký là 2%
                TaxAmount = carPrice * 0.10, // Thuế là 10%
                TotalAmount = carPrice + (carPrice * 0.02) + (carPrice * 0.10), // Tổng giá trị
                OrderStatusList = orderStatuses
            };

            return View(viewModel);
        }


        [Route("Register")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(CarRegistrationVM registrationVM)
        {
            if (ModelState.IsValid)
            {
                var car = await db.Cars.FindAsync(registrationVM.CarId);
                if (car == null)
                {
                    TempData["error"] = "Car not found.";
                    return RedirectToAction("Index", "Home");
                }

                // Ensure the CarPrice is not null
                if (registrationVM.CarPrice == null)
                {
                    TempData["error"] = "Car price not found.";
                    return RedirectToAction("Index", "Home");
                }

                // Ensure the values are calculated properly
                registrationVM.RegistrationFee = registrationVM.CarPrice * 0.02;
                registrationVM.TaxAmount = registrationVM.CarPrice * 0.10;
                registrationVM.TotalAmount = registrationVM.CarPrice + registrationVM.RegistrationFee + registrationVM.TaxAmount;

                var registration = new CarRegistration
                {
                    CarId = registrationVM.CarId,
                    UserId = registrationVM.UserId,
                    RegistrationDate = registrationVM.RegistrationDate,
                    CustomerName = registrationVM.CustomerName,
                    CustomerAddress = registrationVM.CustomerAddress,
                    CustomerPhone = registrationVM.CustomerPhone,
                    CustomerEmail = registrationVM.CustomerEmail,
                    LicensePlate = registrationVM.LicensePlate,
                    RegistrationNumber = registrationVM.RegistrationNumber,
                    RegistrationFee = registrationVM.RegistrationFee,
                    TaxAmount = registrationVM.TaxAmount,
                    PaymentStatus = registrationVM.PaymentStatus,
                    InsuranceDetails = registrationVM.InsuranceDetails,
                    InspectionDate = registrationVM.InspectionDate,
                    Notes = registrationVM.Notes,
                    TotalAmount = registrationVM.TotalAmount,
                    OrderStatusId = registrationVM.OrderStatusId,
                    DriverLicenseNumber = registrationVM.DriverLicenseNumber,
                    Status = "Pending" // Trạng thái ban đầu của đăng ký
                };

                db.CarRegistrations.Add(registration);
                await db.SaveChangesAsync();

                TempData["success"] = "Car registration submitted successfully!";
                return RedirectToAction("Index", "CarRegistration");
            }

            // Reload the OrderStatusList in case of an error
            registrationVM.OrderStatusList = await db.OrderStatus.ToListAsync();
            return View(registrationVM);
        }


        [Route("Detail/{id}")]
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var registration = await db.CarRegistrations
                .Include(cr => cr.Car)
                    .ThenInclude(car => car.Dealer)
                .Include(cr => cr.User)
                .Include(cr => cr.Car.OrderDetails)
                    .ThenInclude(od => od.Order)
                .FirstOrDefaultAsync(cr => cr.Id == id);

            if (registration == null)
            {
                TempData["error"] = "Car registration not found.";
                return RedirectToAction("Index");
            }

            var orderDetail = registration.Car.OrderDetails.FirstOrDefault();
            var order = orderDetail?.Order;

            var viewModel = new CarRegistrationDetailVM
            {
                CarId = registration.CarId,
                UserId = registration.UserId,
                RegistrationDate = registration.RegistrationDate,
                CustomerName = registration.CustomerName,
                CustomerAddress = registration.CustomerAddress,
                CustomerPhone = registration.CustomerPhone,
                CustomerEmail = registration.CustomerEmail,
                LicensePlate = registration.LicensePlate,
                RegistrationNumber = registration.RegistrationNumber,
                RegistrationFee = registration.RegistrationFee,
                TaxAmount = registration.TaxAmount,
                PaymentStatus = registration.PaymentStatus,
                InsuranceDetails = registration.InsuranceDetails,
                InspectionDate = registration.InspectionDate,
                Notes = registration.Notes,
                TotalAmount = registration.TotalAmount,
                OrderStatusId = registration.OrderStatusId,
                DriverLicenseNumber = registration.DriverLicenseNumber,
                Status = registration.Status,
                Car = registration.Car,
                User = registration.User,
                Dealer = registration.Car?.Dealer,
                OrderCode = order?.OrderCode,
                OrderCreateDate = order?.CreateDay,
                OrderExpiryDate = order?.CreateDay.AddDays(7),
                OrderTotalAmount = order?.TotalAmount
            };

            return View(viewModel);
        }



    }
}
