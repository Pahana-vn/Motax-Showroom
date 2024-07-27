using Microsoft.AspNetCore.Mvc;
using Motax.Models;
using Motax.ViewModels;
using Motax.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;

namespace Motax.Controllers
{
    public class CartAccessoriesItemController : Controller
    {
        private readonly MotaxContext db;
        private readonly ILogger<CartAccessoriesItemController> _logger;

        public CartAccessoriesItemController(MotaxContext context, ILogger<CartAccessoriesItemController> logger)
        {
            db = context;
            _logger = logger;
        }

        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();

        public IActionResult Index()
        {
            return View(Cart);
        }

        [Authorize]
        public IActionResult PaymentSuccess()
        {
            return View("Success");
        }

        [Authorize]
        public IActionResult PaymentFail()
        {
            return View("Fail");
        }

        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.Id == id);
            if (item == null)
            {
                var accessories = db.Accessories.SingleOrDefault(p => p.Id == id);
                if (accessories == null)
                {
                    TempData["error"] = "Not found";
                    return Redirect("/404");
                }
                item = new CartItem
                {
                    Id = accessories.Id,
                    Name = accessories.Name,
                    Price = accessories.Price,
                    ImageSingle = accessories.ImageSingle,
                    Quantity = quantity
                };
                gioHang.Add(item);
            }
            else
            {
                item.Quantity += quantity;
            }
            HttpContext.Session.Set(MySetting.CART_KEY, gioHang);

            return RedirectToAction("Index");
        }

        public IActionResult RemoveCart(int id)
        {
            var gioHang = Cart;
            var item = gioHang.FirstOrDefault(p => p.Id == id);
            if (item != null)
            {
                gioHang.Remove(item);
                TempData["success"] = "Remove cart success!";
                HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            if (Cart.Count == 0)
            {
                return Redirect("/");
            }
            return View(Cart);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Checkout(CheckoutAccessoriesVM model)
        {
            if (ModelState.IsValid)
            {
                var customerClaim = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID);
                if (customerClaim == null)
                {
                    // Handle the case where the claim is not found, perhaps redirect to login
                    return RedirectToAction("Login", "Secure");
                }

                var customerId = int.Parse(customerClaim.Value);

                var khachHang = new Account();
                if (model.GiongKhachHang)
                {
                    khachHang = db.Accounts.SingleOrDefault(kh => kh.Id == customerId);
                }

                var hoadon = new OrderAccessories
                {
                    AccountId = customerId,
                    Username = model.UserName ?? khachHang?.Username,
                    Address = model.Address ?? khachHang?.Address,
                    Phone = model.Phone ?? khachHang?.Phone,
                    OrderDate = DateTime.Now,
                    HowToPay = "COD",
                    HowToTransport = "GRAB",
                    Status = 1, // Set status to 1
                    TypeCode = GenerateOrderCode(), // Generate random order code
                    Note = model.Note,
                };

                db.Database.BeginTransaction();
                try
                {
                    db.Add(hoadon);
                    db.SaveChanges();

                    var cthd = new List<OrderDetailAccessories>();
                    foreach (var item in Cart)
                    {
                        cthd.Add(new OrderDetailAccessories
                        {
                            OrderAccessoriesId = hoadon.Id,
                            Quantity = item.Quantity,
                            Price = item.Price,
                            AccessoriesId = item.Id,
                            Discount = 0
                        });
                    }
                    db.AddRange(cthd);
                    db.SaveChanges();

                    db.Database.CommitTransaction();

                    HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());
                    return RedirectToAction("PaymentSuccess");
                }
                catch (Exception ex)
                {
                    db.Database.RollbackTransaction();
                    _logger.LogError(ex, "An error occurred during the checkout process");
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                    // Log the error if necessary
                }
            }
            else
            {
                _logger.LogWarning("ModelState is invalid");
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _logger.LogWarning(error.ErrorMessage);
                    }
                }
            }
            return View(Cart);
        }

        private string GenerateOrderCode()
        {
            return $"ORD-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
