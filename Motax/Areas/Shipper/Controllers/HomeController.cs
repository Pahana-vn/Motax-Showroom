using Microsoft.AspNetCore.Mvc;

namespace Motax.Areas.Shipper.Controllers
{
    [Area("Shipper")]
    [Route("Shipper/Home")]
    public class HomeController : Controller
    {
        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
