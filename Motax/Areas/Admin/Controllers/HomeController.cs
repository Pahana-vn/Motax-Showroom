using Microsoft.AspNetCore.Mvc;

namespace Motax.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Home")]
    public class HomeController : Controller
    {
        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
