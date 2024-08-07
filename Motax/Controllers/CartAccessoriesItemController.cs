using Microsoft.AspNetCore.Mvc;
using Motax.Models;
using Motax.ViewModels;
using Motax.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using Motax.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Motax.Controllers
{
    public class CartAccessoriesItemController : Controller
    {
        private readonly MotaxContext db;
        private readonly ILogger<CartAccessoriesItemController> _logger;
        private readonly PaypalClient _paypalClient;
        private readonly IVnPayService _vnPayservice;

        public CartAccessoriesItemController(MotaxContext context, ILogger<CartAccessoriesItemController> logger, PaypalClient paypalClient, IVnPayService vnPayservice)
        {
            db = context;
            _logger = logger;
            _paypalClient = paypalClient;
            _vnPayservice = vnPayservice;
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

        public IActionResult IncrementQuantity(int id)
        {
            var cart = Cart;
            var item = cart.SingleOrDefault(p => p.Id == id);
            if (item != null)
            {
                item.Quantity++;
                HttpContext.Session.Set(MySetting.CART_KEY, cart);
            }
            return Json(new { success = true, quantity = item?.Quantity, total = cart.Sum(p => p.Total), itemTotal = item?.Total });
        }

        public IActionResult DecrementQuantity(int id)
        {
            var cart = Cart;
            var item = cart.SingleOrDefault(p => p.Id == id);
            if (item != null && item.Quantity > 1)
            {
                item.Quantity--;
                HttpContext.Session.Set(MySetting.CART_KEY, cart);
            }
            return Json(new { success = true, quantity = item?.Quantity, total = cart.Sum(p => p.Total), itemTotal = item?.Total });
        }



        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            if (Cart.Count == 0)
            {
                return Redirect("/");
            }

            // Get the logged-in user's ID from claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                TempData["error"] = "User not logged in.";
                return RedirectToAction("Login", "Secure");
            }

            var user = db.Accounts.Find(int.Parse(userId));

            if (user == null)
            {
                TempData["error"] = "User not found.";
                return RedirectToAction("Index", "Home");
            }

            // Check if all required user fields are present and valid
            if (string.IsNullOrWhiteSpace(user.Username) || user.Username.Trim().ToLower() == "null" ||
                string.IsNullOrWhiteSpace(user.Email) || user.Email.Trim().ToLower() == "null" ||
                string.IsNullOrWhiteSpace(user.Address) || user.Address.Trim().ToLower() == "null" ||
                string.IsNullOrWhiteSpace(user.Phone) || user.Phone.Trim().ToLower() == "null" ||
                string.IsNullOrWhiteSpace(user.Gender) || user.Gender.Trim().ToLower() == "null" ||
                !user.Dob.HasValue)
            {
                TempData["error"] = "Please complete your profile before proceeding with the order.";
                return RedirectToAction("Profile", "Secure");
            }


            ViewBag.PaypalClientId = _paypalClient.ClientId;
            return View(Cart);
        }


        [Authorize]
        [HttpPost]
        public IActionResult Checkout(CheckoutAccessoriesVM model, string payment = "COD", double total = 0)
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
                    HowToPay = payment == "Thanh toán VNPay" ? "VNPay" : "COD",
                    HowToTransport = "GRAB",
                    Status = 1,
                    DeliveryDate = "3",
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

                    if (payment == "Thanh toán VNPay")
                    {
                        var vnPayModel = new VnPaymentRequestModel
                        {
                            Amount = Cart.Sum(p => p.Total),
                            CreatedDate = DateTime.Now,
                            Description = $"{model.UserName} {model.Phone}",
                            FullName = model.UserName,
                            OrderId = hoadon.Id
                        };
                        return Redirect(_vnPayservice.CreatePaymentUrl(HttpContext, vnPayModel));
                    }

                    HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());
                    return RedirectToAction("PaymentSuccess");
                }
                catch (Exception ex)
                {
                    db.Database.RollbackTransaction();
                    _logger.LogError(ex, "An error occurred during the checkout process");
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
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

        #region Payment Paypal
        [Authorize]
        [HttpPost("/CartAccessoriesItem/create-paypal-order")]
        public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
        {
            var tongTien = Cart.Sum(p => p.Total).ToString();
            var donViTienTe = "USD";
            var maDonHangThamChieu = GenerateOrderCode();

            try
            {
                var response = await _paypalClient.CreateOrder(tongTien, donViTienTe, maDonHangThamChieu);

                // Save order details to database
                var order = new OrderAccessories
                {
                    AccountId = GetCurrentCustomerId(),
                    Username = GetCurrentUserName(),
                    Address = GetCurrentUserAddress(),
                    Phone = GetCurrentUserPhone(),
                    OrderDate = DateTime.Now,
                    HowToPay = "PayPal",
                    HowToTransport = "GRAB",
                    Status = 1, // Set initial status to 1
                    TypeCode = maDonHangThamChieu,
                    Note = "" // Add any relevant note here
                };

                db.OrderAccessories.Add(order);
                await db.SaveChangesAsync();

                // Save order details
                await SaveOrderDetails(order.Id);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }

        [Authorize]
        [HttpPost("/CartAccessoriesItem/capture-paypal-order")]
        public async Task<IActionResult> CapturePaypalOrder(string orderID, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _paypalClient.CaptureOrder(orderID);

                // Update order details in the database
                var order = await db.OrderAccessories.FirstOrDefaultAsync(o => o.TypeCode == orderID);
                if (order != null)
                {
                    order.Status = 2; // Assuming 2 means 'Paid'
                    await db.SaveChangesAsync();
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }

        private async Task SaveOrderDetails(int orderId)
        {
            var cthd = new List<OrderDetailAccessories>();
            foreach (var item in Cart)
            {
                cthd.Add(new OrderDetailAccessories
                {
                    OrderAccessoriesId = orderId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    AccessoriesId = item.Id,
                    Discount = 0
                });
            }
            db.AddRange(cthd);
            await db.SaveChangesAsync();
        }

        private int GetCurrentCustomerId()
        {
            var customerClaim = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID);
            return int.Parse(customerClaim?.Value ?? "0");
        }

        private string GetCurrentUserName()
        {
            var customerId = GetCurrentCustomerId();
            var customer = db.Accounts.SingleOrDefault(kh => kh.Id == customerId);
            return customer?.Username;
        }

        private string GetCurrentUserAddress()
        {
            var customerId = GetCurrentCustomerId();
            var customer = db.Accounts.SingleOrDefault(kh => kh.Id == customerId);
            return customer?.Address;
        }

        private string GetCurrentUserPhone()
        {
            var customerId = GetCurrentCustomerId();
            var customer = db.Accounts.SingleOrDefault(kh => kh.Id == customerId);
            return customer?.Phone;
        }

        #endregion



        #region PaymentCallBack VnPay
        [Authorize]
        public IActionResult PaymentCallBack()
        {
            var response = _vnPayservice.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi thanh toán VN Pay: {response.VnPayResponseCode}";
                return RedirectToAction("PaymentFail");
            }

            // Retrieve the order using the order ID passed from VNPay
            var orderId = response.OrderId;
            var order = db.OrderAccessories.FirstOrDefault(o => o.TypeCode == orderId);
            if (order != null)
            {
                order.Status = 2; // Assuming 2 means 'Paid'
                db.SaveChanges();
            }

            TempData["Message"] = $"Thanh toán VNPay thành công";
            return RedirectToAction("PaymentSuccess");
        }
        #endregion

        #region UpdateQuantity
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            var cart = Cart;
            var item = cart.SingleOrDefault(p => p.Id == id);
            if (item != null)
            {
                if (quantity > 20)
                {
                    return Json(new { success = false, message = "Please contact admin for wholesale prices" });
                }

                item.Quantity = quantity;
                HttpContext.Session.Set(MySetting.CART_KEY, cart);
            }
            return Json(new { success = true, quantity = item?.Quantity, total = cart.Sum(p => p.Total), itemTotal = item?.Total });
        }
        #endregion
    }
}
