using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Motax.Models;
using Motax.ViewModels;
using System.Text;

namespace Motax.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Route("Staff/Car")]
    [Authorize(Policy = "CheckAdminOrStaff")]
    public class CarController : Controller
    {
        private readonly MotaxContext db;
        private readonly IWebHostEnvironment webHostEnvironment;

        public CarController(MotaxContext context, IWebHostEnvironment hostEnvironment)
        {
            db = context;
            webHostEnvironment = hostEnvironment;
        }

        private string GenerateRandomVin(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }
            return result.ToString();
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await db.Cars.OrderByDescending(c => c.Id).ToListAsync());

        }

        #region Create
        [Route("Create")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.dealers = new SelectList(db.Dealers, "Id", "Name");
            ViewBag.Brands = new SelectList(db.Brands, "Id", "Name");
            return View();
        }

        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarAdminVM carVM)
        {
            if (ModelState.IsValid)
            {
                string singleImagePath = "";
                if (carVM.ImageSingle != null)
                {
                    string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "Images/Car/Single");
                    singleImagePath = Guid.NewGuid().ToString() + "_" + carVM.ImageSingle.FileName;
                    string singleFilePath = Path.Combine(uploadFolder, singleImagePath);
                    using (var fileStream = new FileStream(singleFilePath, FileMode.Create))
                    {
                        await carVM.ImageSingle.CopyToAsync(fileStream);
                    }
                }

                List<string> multipleImagePaths = new List<string>();
                if (carVM.ImageMultiple != null && carVM.ImageMultiple.Count > 0)
                {
                    string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "Images/Car/Multiple");
                    foreach (var imageFile in carVM.ImageMultiple)
                    {
                        string multipleImagePath = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        string multipleFilePath = Path.Combine(uploadFolder, multipleImagePath);
                        using (var fileStream = new FileStream(multipleFilePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }
                        multipleImagePaths.Add("/Images/Car/Multiple/" + multipleImagePath);
                    }
                }

                var car = new Car
                {
                    Name = carVM.Name,
                    BodyType = carVM.BodyType,
                    Mileage = carVM.Mileage,
                    Transmission = carVM.Transmission,
                    Year = carVM.Year,
                    FuelType = carVM.FuelType,
                    Color = carVM.Color,
                    Doors = carVM.Doors,
                    Cylinders = carVM.Cylinders,
                    EngineSize = carVM.EngineSize,
                    Vin = GenerateRandomVin(17),
                    CarFeatures = carVM.CarFeatures,
                    Title = carVM.Title,
                    BrandId = carVM.BrandId,
                    DealerId = carVM.DealerId,
                    Price = carVM.Price,
                    DriverAirbag = carVM.DriverAirbag,
                    Status = carVM.Status,
                    Condition = carVM.Condition,
                    PriceType = carVM.PriceType,
                    ImageSingle = singleImagePath,
                    ImageMultiple = string.Join(",", multipleImagePaths)

                };

                db.Cars.Add(car);
                await db.SaveChangesAsync();
                TempData["success"] = "Car created successfully!";
                return RedirectToAction("Index");
            }

            ViewBag.Dealers = new SelectList(db.Dealers, "Id", "Name");
            ViewBag.Brands = new SelectList(db.Brands, "Id", "Name");
            return View(carVM);
        }
        #endregion


        #region Edit
        [Route("Edit")]
        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            Car? car = await db.Cars.FindAsync(Id);
            if (car == null)
            {
                return RedirectToAction("/404");
            }

            CarAdminVM vm = new CarAdminVM
            {
                Id = car.Id,
                Name = car.Name,
                BodyType = car.BodyType,
                Mileage = car.Mileage,
                Transmission = car.Transmission,
                Year = car.Year,
                FuelType = car.FuelType,
                Color = car.Color,
                Doors = car.Doors,
                Cylinders = car.Cylinders,
                EngineSize = car.EngineSize,
                Vin = car.Vin,
                CarFeatures = car.CarFeatures,
                Title = car.Title,
                BrandId = car.BrandId,
                DealerId = car.DealerId,
                Price = car.Price,
                DriverAirbag = car.DriverAirbag,
                Status = car.Status,
                Condition = car.Condition,
                PriceType = car.PriceType,
                ExistingImageSingle = car.ImageSingle,
                ExistingImageMultiple = car.ImageMultiple?.Split(",").ToList() ?? new List<string>()
            };

            ViewBag.Dealers = new SelectList(db.Dealers, "Id", "Name", car.DealerId);
            ViewBag.Brands = new SelectList(db.Brands, "Id", "Name", car.BrandId);
            return View(vm);
        }

        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CarAdminVM carVM)
        {
            if (ModelState.IsValid)
            {
                var carToUpdate = await db.Cars.FindAsync(carVM.Id);
                if (carToUpdate == null)
                {
                    return NotFound();
                }

                if (carVM.ImageSingle != null)
                {
                    if (!string.IsNullOrEmpty(carToUpdate.ImageSingle))
                    {
                        string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Car/Single", carToUpdate.ImageSingle);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "Images/Car/Single");
                    string filename = Guid.NewGuid().ToString() + "_" + carVM.ImageSingle.FileName;
                    string filePath = Path.Combine(uploadFolder, filename);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await carVM.ImageSingle.CopyToAsync(fileStream);
                    }
                    carToUpdate.ImageSingle = filename;
                }

                if (carVM.ImageMultiple != null && carVM.ImageMultiple.Count > 0)
                {
                    if (!string.IsNullOrEmpty(carToUpdate.ImageMultiple))
                    {
                        var existingImages = carToUpdate.ImageMultiple.Split(",");
                        foreach (var image in existingImages)
                        {
                            string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Car/Multiple", image);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }
                    }

                    List<string> multipleImagePaths = new List<string>();
                    string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "Images/Car/Multiple");
                    foreach (var imageFile in carVM.ImageMultiple)
                    {
                        string multipleImagePath = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        string multipleFilePath = Path.Combine(uploadFolder, multipleImagePath);
                        using (var fileStream = new FileStream(multipleFilePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }
                        multipleImagePaths.Add("/Images/Car/Multiple/" + multipleImagePath);
                    }
                    carToUpdate.ImageMultiple = string.Join(",", multipleImagePaths);
                }

                carToUpdate.Name = carVM.Name;
                carToUpdate.BodyType = carVM.BodyType;
                carToUpdate.Mileage = carVM.Mileage;
                carToUpdate.Transmission = carVM.Transmission;
                carToUpdate.Year = carVM.Year;
                carToUpdate.FuelType = carVM.FuelType;
                carToUpdate.Color = carVM.Color;
                carToUpdate.Doors = carVM.Doors;
                carToUpdate.Cylinders = carVM.Cylinders;
                carToUpdate.EngineSize = carVM.EngineSize;
                carToUpdate.Vin = carToUpdate.Vin ?? GenerateRandomVin(17);
                carToUpdate.CarFeatures = carVM.CarFeatures;
                carToUpdate.Title = carVM.Title;
                carToUpdate.BrandId = carVM.BrandId;
                carToUpdate.DealerId = carVM.DealerId;
                carToUpdate.Price = carVM.Price;
                carToUpdate.DriverAirbag = carVM.DriverAirbag;
                carToUpdate.Status = carVM.Status;
                carToUpdate.Condition = carVM.Condition;
                carToUpdate.PriceType = carVM.PriceType;

                db.Update(carToUpdate);
                await db.SaveChangesAsync();
                TempData["success"] = "Car updated successfully!";
                return RedirectToAction("Index");
            }

            ViewBag.Dealers = new SelectList(db.Dealers, "Id", "Name", carVM.DealerId);
            ViewBag.Brands = new SelectList(db.Brands, "Id", "Name", carVM.BrandId);
            return View(carVM);
        }
        #endregion

        #region Delete
        [Route("Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int Id)
        {
            var carToDelete = await db.Cars.FindAsync(Id);
            if (carToDelete == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(carToDelete.ImageSingle))
            {
                string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Car/Single", carToDelete.ImageSingle);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            if (!string.IsNullOrEmpty(carToDelete.ImageMultiple))
            {
                var existingImages = carToDelete.ImageMultiple.Split(",");
                foreach (var image in existingImages)
                {
                    string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Car/Multiple", image);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
            }

            db.Cars.Remove(carToDelete);
            await db.SaveChangesAsync();
            TempData["success"] = "Car deleted successfully!";
            return RedirectToAction("Index");
        }

        #endregion

        [Route("Details")]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var car = await db.Cars
                .Include(c => c.Dealer)
                .Include(c => c.Brand)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            var carViewModel = new CarDetailAdminVM
            {
                Id = car.Id,
                Name = car.Name,
                BodyType = car.BodyType,
                Mileage = car.Mileage,
                Transmission = car.Transmission,
                Year = car.Year,
                FuelType = car.FuelType,
                Color = car.Color,
                Doors = car.Doors,
                Cylinders = car.Cylinders,
                EngineSize = car.EngineSize,
                Vin = car.Vin,
                CarFeatures = car.CarFeatures,
                Title = car.Title,
                BrandName = car.Brand?.Name,
                DealerName = car.Dealer?.Name,
                Price = car.Price,
                DriverAirbag = car.DriverAirbag,
                Status = car.Status,
                Condition = car.Condition,
                PriceType = car.PriceType,
                ImageSingle = car.ImageSingle,
                ImageMultiple = car.ImageMultiple?.Split(",").ToList() ?? new List<string>(),
                IsAvailable = car.IsAvailable
            };

            return View(carViewModel);
        }

    }
}
