using Motax.Models;
using Motax.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList.Extensions;

namespace Motax.Controllers
{
    public class DealerController : Controller
    {
        private readonly MotaxContext db;

        public DealerController(MotaxContext context)
        {
            db = context;
        }

        public IActionResult Index(int? brand, string location, int sortOption = 1, int page = 1, int pageSize = 9)
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

            // Xử lý sắp xếp
            switch (sortOption)
            {

                case 3: // Sort by Popular
                    dealers = dealers.OrderByDescending(p => p.Cars.Count()); // Sắp xếp theo số lượng xe
                    break;
                case 4: // Sort by Most Rated
                    dealers = dealers.OrderByDescending(p => p.Cars.SelectMany(c => c.Comments).Average(c => c.Rating));
                    break;
                default: // Sort by Default
                    dealers = dealers.OrderBy(p => p.Name);
                    break;
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
            }).ToPagedList(page, pageSize);

            // Tạo danh sách SelectListItem và truyền qua ViewBag
            ViewBag.SortOptions = new List<SelectListItem>
    {
        new SelectListItem { Value = "1", Text = "Sort By Default", Selected = sortOption == 1 },
        new SelectListItem { Value = "3", Text = "Sort By vehicles", Selected = sortOption == 3 },
        new SelectListItem { Value = "4", Text = "Sort By Most Rated", Selected = sortOption == 4 }
    };

            ViewBag.CurrentSort = sortOption;

            return View(result);
        }


        public IActionResult Search(string? query, int sortOption = 1, int page = 1, int pageSize = 9)
        {
            var dealers = db.Dealers.AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                dealers = dealers.Where(p => p.Name.Contains(query));
            }

            // Sort options similar to the Index method
            switch (sortOption)
            {
                case 3: // Sort by Popular
                    dealers = dealers.OrderByDescending(p => p.Cars.Count());
                    break;
                case 4: // Sort by Most Rated
                    dealers = dealers.OrderByDescending(p => p.Cars.SelectMany(c => c.Comments).Average(c => c.Rating));
                    break;
                default: // Sort by Default
                    dealers = dealers.OrderBy(p => p.Name);
                    break;
            }

            // Convert the result to a PagedList
            var result = dealers.Select(p => new DealerVM
            {
                Id = p.Id,
                Name = p.Name,
                ImageBackground = p.ImageBackground,
                Phone = p.Phone,
                Address = p.Address,
                City = p.City,
                Quantity = p.Cars.Count(),
            }).ToPagedList(page, pageSize);

            // Populate the SortOptions ViewBag
            ViewBag.SortOptions = new List<SelectListItem>
    {
        new SelectListItem { Value = "1", Text = "Sort By Default", Selected = sortOption == 1 },
        new SelectListItem { Value = "3", Text = "Sort By vehicles", Selected = sortOption == 3 },
        new SelectListItem { Value = "4", Text = "Sort By Most Rated", Selected = sortOption == 4 }
    };

            ViewBag.CurrentSort = sortOption;

            return View(result);
        }



        public IActionResult Detail(int id, int sortOption = 1, int page = 1, int pageSize = 9)
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

            // Xử lý sắp xếp
            var cars = dealer.Cars.AsQueryable();
            switch (sortOption)
            {
                case 2: // Sort by Latest
                    cars = cars.OrderByDescending(c => c.Year);
                    break;
                case 3: // Sort by Low Price
                    cars = cars.OrderBy(c => c.Price);
                    break;
                case 4: // Sort by High Price
                    cars = cars.OrderByDescending(c => c.Price);
                    break;
                default: // Sort by Default
                    cars = cars.OrderBy(c => c.Name);
                    break;
            }

            var carPagedList = cars.Select(c => new CarVM
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
            }).ToPagedList(page, pageSize);

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
                Cars = carPagedList // Cập nhật Cars thành carPagedList
            };

            ViewBag.SortOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Sort By Default", Selected = sortOption == 1 },
                new SelectListItem { Value = "2", Text = "Sort By Latest", Selected = sortOption == 2 },
                new SelectListItem { Value = "3", Text = "Sort By Low Price", Selected = sortOption == 3 },
                new SelectListItem { Value = "4", Text = "Sort By High Price", Selected = sortOption == 4 }
            };

            ViewBag.CurrentSort = sortOption;

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
