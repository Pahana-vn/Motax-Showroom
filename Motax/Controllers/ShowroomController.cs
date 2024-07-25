using Microsoft.AspNetCore.Mvc;

namespace Motax.Controllers
{
    public class ShowroomController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
