using Microsoft.AspNetCore.Mvc;

namespace Motax.Staff.Admin.Controllers
{
    [Area("Staff")]
    [Route("Staff/Home")]
    public class HomeController : Controller
    {
        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
