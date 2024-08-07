using Microsoft.AspNetCore.Mvc;

namespace Motax.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
