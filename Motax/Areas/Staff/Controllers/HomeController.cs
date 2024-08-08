using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Motax.Models;


namespace Motax.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Route("Staff/Home")]
    [Authorize(Policy = "CheckAdminOrStaff")]
    public class HomeController : Controller
    {

        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
