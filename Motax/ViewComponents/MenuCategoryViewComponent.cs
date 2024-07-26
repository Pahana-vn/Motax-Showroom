using Microsoft.AspNetCore.Mvc;
using Motax.Models;
using Motax.ViewModels;

namespace Motax.ViewComponents
{
    public class MenuCategoryViewComponent : ViewComponent
    {
        private readonly MotaxContext db;

        public MenuCategoryViewComponent(MotaxContext context)
        {
            db = context;
        }

        public IViewComponentResult Invoke()
        {
            var cates = db.Categories.Select(br => new MenuCategoryVM
            {
                Id = br.Id,
                Name = br.Name,
                Quantity = br.Accessories.Count(),
            }).OrderBy(p => p.Name).ToList();

            var allCates = new List<MenuCategoryVM>
            {
                new MenuCategoryVM { Id = 0, Name = "All cates", Quantity = cates.Sum(b => b.Quantity) }
            };

            allCates.AddRange(cates);

            return View(allCates);
        }
    }
}
