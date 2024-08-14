using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Motax.Models;
using Motax.ViewModels;
using X.PagedList.Extensions;

namespace Motax.Controllers
{
    public class AccessoriesController : Controller
    {
        private readonly MotaxContext db;

        public AccessoriesController(MotaxContext context)
        {
            db = context;
        }
        public IActionResult Index(int? brand, int? category, int sortOption = 1, int page = 1, int pageSize = 9)
        {
            var accs = db.Accessories.AsQueryable();

            if (brand.HasValue && brand.Value != 0)
            {
                accs = accs.Where(p => p.BrandId == brand.Value);
            }

            if (category.HasValue)
            {
                accs = accs.Where(p => p.CategoryId == category.Value);
            }

            // Xử lý sắp xếp
            switch (sortOption)
            {
                case 2: // Sort by Latest
                    accs = accs.OrderByDescending(p => p.Id); // Giả sử rằng ID tăng dần theo thời gian
                    break;
                case 3: // Sort by Low Price
                    accs = accs.OrderBy(p => p.Price);
                    break;
                case 4: // Sort by High Price
                    accs = accs.OrderByDescending(p => p.Price);
                    break;
                case 5: // Sort by Featured
                    accs = accs.OrderByDescending(p => p.Name); // Giả sử tính năng Featured có thể là một đặc điểm khác
                    break;
                default: // Sort by Default
                    accs = accs.OrderBy(p => p.Name);
                    break;
            }

            var result = accs.Select(p => new AccessoriesVM
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ImageSingle = p.ImageSingle,
            }).ToPagedList(page, pageSize);

            // Tạo danh sách SelectListItem và truyền qua ViewBag
            ViewBag.SortOptions = new List<SelectListItem>
    {
        new SelectListItem { Value = "1", Text = "Sort By Default", Selected = sortOption == 1 },
        new SelectListItem { Value = "5", Text = "Sort By Featured", Selected = sortOption == 5 },
        new SelectListItem { Value = "2", Text = "Sort By Latest", Selected = sortOption == 2 },
        new SelectListItem { Value = "3", Text = "Sort By Low Price", Selected = sortOption == 3 },
        new SelectListItem { Value = "4", Text = "Sort By High Price", Selected = sortOption == 4 }
    };

            ViewBag.CurrentSort = sortOption;

            return View(result);
        }


        public IActionResult Search(string? query)
        {
            var accs = db.Accessories.AsQueryable();

            if (query != null)
            {
                accs = accs.Where(p => p.Name != null && p.Name.Contains(query));
            }
            var result = accs.Select(p => new AccessoriesVM
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ImageSingle = p.ImageSingle,
            });
            return View(result);
        }

        public IActionResult FindFilter(double max, double min)
        {
            var result = db.Accessories.Where(d => d.Price >= min && d.Price <= max).Select(p => new AccessoriesVM
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ImageSingle = p.ImageSingle,
            }).ToList();
            return View("Index", result);
        }

        public IActionResult Detail(int id)
        {
            var data = db.Accessories
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .SingleOrDefault(p => p.Id == id);

            if (data == null)
            {
                TempData["error"] = "This Accessories was not found";
                return Redirect("/404");
            }
            //them xe có liên quan
            var relatedAccs = db.Accessories
           .Where(p => p.CategoryId == data.CategoryId && p.Id != id)
           .Select(p => new AccessoriesVM
           {
               Id = p.Id,
               Name = p.Name,
               Price = p.Price,
               ImageSingle = p.ImageSingle,
           }).ToList();

            var result = new DetailAccessoriesVM
            {
                Id = data.Id,
                Name = data.Name,
                ImageSingle = data.ImageSingle,
                ImageMultiple = data.ImageMultiple,
                Price = data.Price,
                NameCategory = data.Category?.Name,
                Description = data.Description,
                RelatedAccs = relatedAccs
            };
            return View(result);
        }
    }
}
