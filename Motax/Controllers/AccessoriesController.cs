using Microsoft.AspNetCore.Mvc;

namespace Motax.Controllers
{
    public class AccessoriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
