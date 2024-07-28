using Microsoft.AspNetCore.Mvc;
using Motax.Models;
using Motax.ViewModels;
using System.Diagnostics;

namespace Motax.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MotaxContext db;

        public HomeController(ILogger<HomeController> logger, MotaxContext context)
        {
            _logger = logger;
            db = context;
        }

        public IActionResult Index()
        {
            var model = new HomeViewModel
            {
                Cars = db.Cars.Select(p => new CarVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Condition = p.Condition,
                    Mileage = p.Mileage,
                    ImageSingle = p.ImageSingle,
                    Transmission = p.Transmission,
                    Year = p.Year,
                    FuelType = p.FuelType,
                    Price = p.Price,
                    NameBrand = p.Brand.Name,
                    IsAvailable = p.IsAvailable,
                    AverageRating = p.Comments.Any() ? p.Comments.Average(c => c.Rating) : 0,
                    ReviewCount = p.Comments.Count()
                }).Take(8).ToList(),
                Dealers = db.Dealers.Select(p => new DealerVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Phone = p.Phone,
                    Address = p.Address,
                    City = p.City,
                    ImageBackground = p.ImageBackground,
                    Quantity = p.Cars.Count()
                }).Take(8).ToList(),
                Brands = db.Brands.Select(p => new BrandAdminVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    ContactPerson = p.ContactPerson,
                    Email = p.Email,
                    Phone = p.Phone,
                    Address = p.Address,
                    Description = p.Description,
                    Slug = p.Slug,
                    Status = p.Status,
                    ExistingImage = p.Image
                }).Take(6).ToList(),
                Categories = db.Categories.Select(p => new CategoryAdminVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Slug = p.Slug,
                    Description = p.Description,
                    Status = p.Status,
                    ExistingImage = p.Image
                }).Take(12).ToList()
            };

            return View(model);
        }

        [Route("/404")]
        public IActionResult PageNotFound()
        {
            return View();
        }
    }
}
