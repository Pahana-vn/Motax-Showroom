using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Motax.Models;
using Motax.ViewModels;

namespace Motax.Controllers
{
    public class AccessoriesController : Controller
    {
        private readonly MotaxContext db;

        public AccessoriesController(MotaxContext context)
        {
            db = context;
        }
        public IActionResult Index(int? brand, int? category)
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

            var result = accs.Select(p => new AccessoriesVM
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ImageSingle = p.ImageSingle,
            });
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
