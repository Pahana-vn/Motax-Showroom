using Motax.Models;
using Motax.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Motax.Controllers
{
    public class DealerController : Controller
    {
        private readonly MotaxContext db;

        public DealerController(MotaxContext context)
        {
            db = context;
        }

        public IActionResult Index(int? brand, string location)
        {
            var dealers = db.Dealers.AsQueryable();

            if (brand.HasValue && brand.Value != 0)
            {
                dealers = dealers.Where(p => p.BrandId == brand.Value);
            }

            if (!string.IsNullOrEmpty(location) && location != "All Locations")
            {
                dealers = dealers.Where(p => (p.City ?? string.Empty).Contains(location));
            }

            var result = dealers.Select(p => new DealerVM
            {
                Id = p.Id,
                Name = p.Name,
                ImageBackground = p.ImageBackground,
                Phone = p.Phone,
                Address = p.Address,
                City = p.City,
                Quantity = p.Cars.Count(),
            });

            return View(result);
        }

        public IActionResult Search(string? query)
        {
            var dealers = db.Dealers.AsQueryable();

            if (query != null)
            {
                dealers = dealers.Where(p => p.Name != null && p.Name.Contains(query));
            }
            var result = dealers.Select(p => new DealerVM
            {
                Id = p.Id,
                Name = p.Name,
                ImageBackground = p.ImageBackground,
                Phone = p.Phone,
                Address = p.Address,
                City = p.City,
                Quantity = p.Cars.Count(),
            });
            return View(result);
        }

        public IActionResult Detail(int id)
        {
            var dealer = db.Dealers
                .Include(d => d.DealerDetails)
                .Include(d => d.Cars)
                .ThenInclude(c => c.Comments)
                .SingleOrDefault(d => d.Id == id);

            if (dealer == null)
            {
                TempData["error"] = "Dealer not found";
                return RedirectToAction("Index");
            }

            var dealerVM = new DealerDetailVM
            {
                Id = dealer.Id,
                Name = dealer.Name,
                DealerDetails = dealer.DealerDetails.Select(dd => new DealerDetailItemVM
                {
                    Id = dd.Id,
                    CoverImage = dd.CoverImage,
                    AvatarImage = dd.AvatarImage,
                    ConsultantName = dd.ConsultantName,
                    ConsultantAvatar = dd.ConsultantAvatar
                }).ToList(),
                Quantity = dealer.Cars.Count(),
                Cars = dealer.Cars.Select(c => new CarVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    Condition = c.Condition,
                    FuelType = c.FuelType,
                    Mileage = c.Mileage,
                    Transmission = c.Transmission,
                    Year = c.Year,
                    Price = c.Price,
                    ImageSingle = c.ImageSingle,
                    AverageRating = c.Comments.Any() ? c.Comments.Average(co => co.Rating) : 0,
                    ReviewCount = c.Comments.Count()
                }).ToList()
            };

            return View(dealerVM);
        }


        [HttpPost]
        public IActionResult Contact(int dealerId, string name, string email, string message)
        {
            var contactMessage = new ContactMessage
            {
                DealerId = dealerId,
                Name = name,
                Email = email,
                Message = message
            };

            db.ContactMessages.Add(contactMessage);
            db.SaveChanges();
            TempData["success"] = "Send messages success!";

            return RedirectToAction("Detail", new { id = dealerId });
        }
    }
}
