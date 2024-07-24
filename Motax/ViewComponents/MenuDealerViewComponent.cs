using Microsoft.AspNetCore.Mvc;
using Motax.Models;
using Motax.ViewModels;

namespace Motax.ViewComponents
{
    public class MenuDealerViewComponent : ViewComponent
    {
        private readonly MotaxContext db;

        public MenuDealerViewComponent(MotaxContext context)
        {
            db = context;
        }

        public IViewComponentResult Invoke()
        {
            var data = db.Dealers.Select(br => new MenuDealerVM
            {
                Id = br.Id,
                Name = br.Name,
                Quantity = br.Cars.Count,
            }).OrderBy(p => p.Name);

            return View(data);
        }
    }
}
