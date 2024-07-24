using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Motax.Models;
using Motax.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Motax.Controllers
{
    public class CarController : Controller
    {
        private readonly MotaxContext db;

        public CarController(MotaxContext context)
        {
            db = context;
        }

        public IActionResult Index(int? brand, int? dealer)
        {
            var cars = db.Cars.AsQueryable();

            if (brand.HasValue && brand.Value != 0)
            {
                cars = cars.Where(p => p.BrandId == brand.Value);
            }

            if (dealer.HasValue)
            {
                cars = cars.Where(p => p.DealerId == dealer.Value);
            }

            var result = cars.Select(p => new CarVM
            {
                Id = p.Id,
                Name = p.Name,
                Condition = p.Condition,
                FuelType = p.FuelType,
                Mileage = p.Mileage,
                Transmission = p.Transmission,
                Year = p.Year,
                Price = p.Price,
                ImageSingle = p.ImageSingle,
                NameBrand = p.Brand != null ? p.Brand.Name : null,
                IsAvailable = p.IsAvailable // Add this line
            });
            return View(result);
        }

        public IActionResult Search(string? query)
        {
            var cars = db.Cars.AsQueryable();

            if (query != null)
            {
                cars = cars.Where(p => p.Name != null && p.Name.Contains(query));
            }
            var result = cars.Select(p => new CarVM
            {
                Id = p.Id,
                Name = p.Name,
                Condition = p.Condition,
                FuelType = p.FuelType,
                Mileage = p.Mileage,
                Transmission = p.Transmission,
                Year = p.Year,
                Price = p.Price,
                ImageSingle = p.ImageSingle,
                IsAvailable = p.IsAvailable // Add this line
            });
            return View(result);
        }

        public IActionResult FindFilter(double max, double min)
        {
            var result = db.Cars.Where(d => d.Price >= min && d.Price <= max).Select(p => new CarVM
            {
                Id = p.Id,
                Name = p.Name,
                Condition = p.Condition,
                FuelType = p.FuelType,
                Mileage = p.Mileage,
                Transmission = p.Transmission,
                Year = p.Year,
                Price = p.Price,
                ImageSingle = p.ImageSingle,
                IsAvailable = p.IsAvailable // Add this line
            }).ToList();
            return View("Index", result);
        }

        public IActionResult Detail(int id)
        {
            var data = db.Cars
                .Include(p => p.Brand)
                .Include(p => p.Dealer)
                .SingleOrDefault(p => p.Id == id);

            if (data == null)
            {
                TempData["error"] = "This product was not found";
                return Redirect("/404");
            }
            //them xe có liên quan
            var relatedCars = db.Cars
           .Where(p => p.BrandId == data.BrandId && p.Id != id)
           .Select(p => new CarVM
           {
               Id = p.Id,
               Name = p.Name,
               Condition = p.Condition,
               FuelType = p.FuelType,
               Mileage = p.Mileage,
               Transmission = p.Transmission,
               Year = p.Year,
               Price = p.Price,
               ImageSingle = p.ImageSingle,
               NameBrand = p.Brand != null ? p.Brand.Name : null,
               IsAvailable = p.IsAvailable // Add this line
           }).ToList();

            var result = new DetailCarVM
            {
                Id = data.Id,
                Name = data.Name,
                ImageSingle = data.ImageSingle,
                ImageMultiple = data.ImageMultiple,
                Price = data.Price,
                NameBrand = data.Brand?.Name,
                AddressDealer = data.Dealer?.Address,
                BodyType = data.BodyType,
                Condition = data.Condition,
                FuelType = data.FuelType,
                Mileage = data.Mileage,
                Transmission = data.Transmission,
                Year = data.Year,
                Color = data.Color,
                Doors = data.Doors,
                Cylinders = data.Cylinders,
                EngineSize = data.EngineSize,
                Vin = data.Vin,
                Title = data.Title,
                CarFeatures = data.CarFeatures,
                PriceType = data.PriceType,
                IsAvailable = data.IsAvailable, // Add this line
                RelatedCars = relatedCars
            };
            return View(result);
        }
    }
}
