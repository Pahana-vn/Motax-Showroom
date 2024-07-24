using Microsoft.AspNetCore.Mvc;
using Motax.Models;
using Motax.ViewModels;
using System.Linq;

namespace Motax.ViewComponents
{
    public class MenuLocationViewComponent : ViewComponent
    {
        private readonly MotaxContext db;

        public MenuLocationViewComponent(MotaxContext context)
        {
            db = context;
        }

        public IViewComponentResult Invoke()
        {
            var cities = db.Dealers
                .Select(br => br.City)
                .Distinct()
                .OrderBy(city => city)
                .ToList();

            var allLocations = new List<DealerVM>
            {
                new DealerVM { City = "All Locations" } // Thêm mục All Locations
            };

            allLocations.AddRange(cities.Select(city => new DealerVM { City = city }));

            return View(allLocations);
        }
    }
}
