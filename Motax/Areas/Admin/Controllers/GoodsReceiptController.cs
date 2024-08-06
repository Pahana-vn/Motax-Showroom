using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Motax.Models;
using Motax.ViewModels;
using System.Threading.Tasks;

namespace Motax.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/GoodsReceipt")]
    [Authorize(Policy = "CheckAdmin")]
    public class GoodsReceiptController : Controller
    {
        private readonly MotaxContext db;

        public GoodsReceiptController(MotaxContext context)
        {
            db = context;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var goodsReceipts = await db.GoodsReceipts
                .Include(gr => gr.Car)
                .Include(gr => gr.Dealer)
                .OrderByDescending(gr => gr.Id)
                .ToListAsync();

            return View(goodsReceipts);
        }

        [Route("Create")]
        [HttpGet]
        public IActionResult Create(int serviceUnitId)
        {
            var serviceUnit = db.ServiceUnits.Include(su => su.Car).Include(su => su.Dealer).FirstOrDefault(su => su.Id == serviceUnitId);
            if (serviceUnit == null)
            {
                TempData["error"] = "Service unit not found.";
                return RedirectToAction("Index");
            }

            var viewModel = new GoodsReceiptViewModel
            {
                CarId = serviceUnit.CarId,
                DealerId = serviceUnit.DealerId,
                VIN = serviceUnit.Car.Vin, // Assuming VIN is a property of the Car model
                ReceiptDate = DateTime.Now,
                ReceiptDetails = "Receive vehicle from manufacturer."
            };

            return View(viewModel);
        }

        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GoodsReceiptViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var dealer = await db.Dealers.FindAsync(viewModel.DealerId);
                if (dealer == null)
                {
                    TempData["error"] = "Invalid dealer.";
                    return RedirectToAction("Create", new { serviceUnitId = viewModel.CarId });
                }

                var goodsReceipt = new GoodsReceipt
                {
                    CarId = viewModel.CarId,
                    DealerId = viewModel.DealerId,
                    VIN = viewModel.VIN,
                    ReceiptDate = viewModel.ReceiptDate,
                    ReceiptDetails = viewModel.ReceiptDetails
                };

                db.GoodsReceipts.Add(goodsReceipt);
                await db.SaveChangesAsync();

                TempData["success"] = "Goods receipt created successfully!";
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }
    }
}
