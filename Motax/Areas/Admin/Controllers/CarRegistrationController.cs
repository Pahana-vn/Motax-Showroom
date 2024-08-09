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
    [Authorize(Policy = "CheckAdmin")]
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
        public async Task<IActionResult> Register(int orderId)
        {
            var order = await db.Orders
                .Include(o => o.Car)
                .Include(o => o.Dealer)
                .Include(o => o.Account)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                TempData["error"] = "Order not found.";
                return RedirectToAction("Index", "Home");
            }

            var orderStatuses = await db.OrderStatus.ToListAsync();

            var viewModel = new CarRegistrationVM
            {
                CarId = order.CarId,
                UserId = order.AccountId,
                RegistrationDate = DateTime.Now,
                CarPrice = order.Car.Price ?? 0,
                RegistrationFee = (order.Car.Price ?? 0) * 0.02, // Registration fee is 2%
                TaxAmount = (order.Car.Price ?? 0) * 0.10, // Tax is 10%
                TotalAmount = (order.Car.Price ?? 0) + (order.Car.Price ?? 0) * 0.01 + (order.Car.Price ?? 0) * 0.02 + (order.Car.Price ?? 0) * 0.10,
                OrderStatusList = orderStatuses,
                CustomerName = order.Account.Username,
                CustomerAddress = order.Address,
                CustomerPhone = order.Phone,
                CustomerEmail = order.Account.Email
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
                    RegistrationFee = registrationVM.CarPrice * 0.02,
                    TaxAmount = registrationVM.CarPrice * 0.10,
                    PaymentStatus = registrationVM.PaymentStatus,
                    InsuranceDetails = registrationVM.InsuranceDetails,
                    InspectionDate = registrationVM.InspectionDate,
                    Notes = registrationVM.Notes,
                    TotalAmount = registrationVM.CarPrice + registrationVM.CarPrice * 0.01 + registrationVM.CarPrice * 0.02 + registrationVM.CarPrice * 0.10,
                    OrderStatusId = registrationVM.OrderStatusId,
                    DriverLicenseNumber = registrationVM.DriverLicenseNumber,
                    Status = "Pending"
                };

                db.CarRegistrations.Add(registration);
                await db.SaveChangesAsync();

                TempData["success"] = "Car registration submitted successfully!";
                return RedirectToAction("Index", "CarRegistration");
            }

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

            var orderDetail = registration.Car?.OrderDetails.FirstOrDefault();
            var order = orderDetail?.Order;

            var viewModel = new CarRegistrationDetailVM
            {
                CarRegistrationId = registration.Id, // Set this property
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


        [Route("SendInvoice/{id}")]
        [HttpPost]
        public async Task<IActionResult> SendInvoice(int id)
        {
            var carRegistration = await db.CarRegistrations
                .Include(cr => cr.User)
                .Include(cr => cr.Car)
                .ThenInclude(car => car.Dealer)
                .FirstOrDefaultAsync(cr => cr.Id == id);

            if (carRegistration == null)
            {
                TempData["error"] = "Car registration not found.";
                return RedirectToAction("Index");
            }

            // Ensure the invoice is created for the correct user
            var invoice = new Invoices
            {
                UserId = carRegistration.UserId,
                CarRegistrationId = carRegistration.Id,
                InvoiceDate = DateTime.Now,
                TotalAmount = carRegistration.TotalAmount ?? 0,
                Status = "Pending"
            };

            db.Invoices.Add(invoice);
            await db.SaveChangesAsync();

            TempData["success"] = "Invoice sent successfully.";
            return RedirectToAction("Detail", new { id = id });
        }

        #region Update
        [Route("Update")]
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var registration = await db.CarRegistrations
                                       .Include(cr => cr.Car)  // Bao gồm thông tin của Car
                                       .FirstOrDefaultAsync(cr => cr.Id == id);

            if (registration == null)
            {
                TempData["error"] = "Car registration not found.";
                return RedirectToAction("Index");
            }

            var viewModel = new CarRegistrationVM
            {
                Id = registration.Id,
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
                OrderStatusList = await db.OrderStatus.ToListAsync(),
                CarPrice = registration.Car?.Price ?? 0  // Gán giá trị CarPrice từ đối tượng Car
            };

            return View(viewModel);
        }

        [Route("Update")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(CarRegistrationVM registrationVM)
        {
            if (ModelState.IsValid)
            {
                var registrationToUpdate = await db.CarRegistrations
                                                   .Include(cr => cr.Car)  // Bao gồm thông tin của Car
                                                   .FirstOrDefaultAsync(cr => cr.Id == registrationVM.Id);
                if (registrationToUpdate == null)
                {
                    TempData["error"] = "Car registration not found.";
                    return RedirectToAction("Index");
                }

                registrationToUpdate.RegistrationDate = registrationVM.RegistrationDate;
                registrationToUpdate.CustomerName = registrationVM.CustomerName;
                registrationToUpdate.CustomerAddress = registrationVM.CustomerAddress;
                registrationToUpdate.CustomerPhone = registrationVM.CustomerPhone;
                registrationToUpdate.CustomerEmail = registrationVM.CustomerEmail;
                registrationToUpdate.LicensePlate = registrationVM.LicensePlate;
                registrationToUpdate.RegistrationNumber = registrationVM.RegistrationNumber;
                registrationToUpdate.RegistrationFee = registrationToUpdate.Car.Price * 0.02;  // Sử dụng giá trị từ Car
                registrationToUpdate.TaxAmount = registrationToUpdate.Car.Price * 0.10;  // Sử dụng giá trị từ Car
                registrationToUpdate.PaymentStatus = registrationVM.PaymentStatus;
                registrationToUpdate.InsuranceDetails = registrationVM.InsuranceDetails;
                registrationToUpdate.InspectionDate = registrationVM.InspectionDate;
                registrationToUpdate.Notes = registrationVM.Notes;
                registrationToUpdate.TotalAmount = registrationToUpdate.Car.Price + registrationToUpdate.Car.Price * 0.01 + registrationToUpdate.Car.Price * 0.02 + registrationToUpdate.Car.Price * 0.10;  // Sử dụng giá trị từ Car
                registrationToUpdate.OrderStatusId = registrationVM.OrderStatusId;
                registrationToUpdate.DriverLicenseNumber = registrationVM.DriverLicenseNumber;

                db.CarRegistrations.Update(registrationToUpdate);
                await db.SaveChangesAsync();

                TempData["success"] = "Car registration updated successfully!";
                return RedirectToAction("Index");
            }

            registrationVM.OrderStatusList = await db.OrderStatus.ToListAsync();
            return View(registrationVM);
        }

        #endregion

        #region Delete
        [Route("Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var registrationToDelete = await db.CarRegistrations.FindAsync(id);
            if (registrationToDelete == null)
            {
                TempData["error"] = "Car registration not found.";
                return RedirectToAction("Index");
            }


            bool hasRelatedRecords = await db.ServiceUnits.AnyAsync(su => su.CarRegistrationId == id);

            if (hasRelatedRecords)
            {
                TempData["error"] = "Cannot delete car registration as it has related records in other tables.";
                return RedirectToAction("Index");
            }

            db.CarRegistrations.Remove(registrationToDelete);
            await db.SaveChangesAsync();
            TempData["success"] = "Car registration deleted successfully!";
            return RedirectToAction("Index");
        }
        #endregion

    }
}
