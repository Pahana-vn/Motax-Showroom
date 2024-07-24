using Microsoft.AspNetCore.Mvc;
using Motax.Models;
using Motax.ViewModels;
using System.Linq;
using System.Collections.Generic;

namespace Motax.ViewComponents
{
    public class MenuBrandViewComponent : ViewComponent
    {
        private readonly MotaxContext db;

        public MenuBrandViewComponent(MotaxContext context)
        {
            db = context;
        }

        public IViewComponentResult Invoke()
        {
            var brands = db.Brands.Select(br => new MenuBrandVM
            {
                Id = br.Id,
                Name = br.Name,
                Quantity = br.Cars.Count(),
            }).OrderBy(p => p.Name).ToList();

            var allBrands = new List<MenuBrandVM>
            {
                new MenuBrandVM { Id = 0, Name = "All Brands", Quantity = brands.Sum(b => b.Quantity) } // Thêm mục All Brands
            };

            allBrands.AddRange(brands);

            return View(allBrands);
        }
    }
}
