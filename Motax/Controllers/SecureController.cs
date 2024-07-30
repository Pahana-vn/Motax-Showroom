using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Motax.Models;
using System.Security.Claims;
using Motax.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Motax.Helpers;

namespace Motax.Controllers
{
    public class SecureController : Controller
    {
        private readonly MotaxContext db;
        private readonly IWebHostEnvironment webHostEnvironment;

        public SecureController(MotaxContext context, IWebHostEnvironment hc)
        {
            db = context;
            webHostEnvironment = hc;
        }

        #region Register 
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel acc)
        {
            if (ModelState.IsValid)
            {
                string filename = "";
                if (acc.Image != null)
                {
                    string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "Images/Account/Avatar");
                    filename = Guid.NewGuid().ToString() + "_" + acc.Image.FileName;
                    string filePath = Path.Combine(uploadFolder, filename);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await acc.Image.CopyToAsync(fileStream);
                    }
                }

                var hasher = new PasswordHasher<Account>();
                var user = new Account
                {
                    Username = acc.Username,
                    Password = BCrypt.Net.BCrypt.HashPassword(acc.Password),
                    Email = acc.Email,
                    Phone = acc.Phone,
                    Address = acc.Address,
                    Image = filename,
                    Dob = acc.Dob,
                    Gender = acc.Gender,
                    Status = 1,
                    RoleId = 2,
                };
                db.Accounts.Add(user);
                await db.SaveChangesAsync();
                return RedirectToAction("Login", "Secure");
            }
            return View(acc);
        }

        #endregion

        #region Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel acc)
        {
            if (ModelState.IsValid)
            {
                var user = await db.Accounts
                                   .Include(u => u.Role) // Ensure Role is included
                                   .FirstOrDefaultAsync(x => x.Username == acc.UserName);

                if (user != null && BCrypt.Net.BCrypt.Verify(acc.Password, user.Password))
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role?.Title ?? "user"),
                new Claim(MySetting.CLAIM_CUSTOMERID, user.Id.ToString())
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    // Redirect based on role
                    switch (user.RoleId)
                    {
                        case 1: // Admin
                            return Redirect("https://localhost:7181/admin/Home/Index");
                        case 2: // Customer
                            return Redirect("https://localhost:7181");
                        case 3: // Staff
                            return Redirect("https://localhost:7181/staff/Home/Index");
                        case 4: // Shipper
                            return Redirect("https://localhost:7181/shipper/Home/Index");
                        default:
                            return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    TempData["error"] = "Invalid login information.";
                }
            }

            return View(acc);
        }
        #endregion


        #region Profile
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userId, out int intUserId))
                {
                    var user = await db.Accounts.FindAsync(intUserId);
                    if (user != null)
                    {
                        var viewModel = new ProfileUpdateViewModel
                        {
                            Id = user.Id,
                            Username = user.Username,
                            Dob = user.Dob,
                            Email = user.Email,
                            Phone = user.Phone,
                            Gender = user.Gender,
                            Address = user.Address,
                            ExistingImage = user.Image
                        };
                        return View(viewModel);
                    }
                }
                else
                {
                    var user = await db.Accounts.FirstOrDefaultAsync(u => u.ExternalId == userId);
                    if (user != null)
                    {
                        var viewModel = new ProfileUpdateViewModel
                        {
                            Id = user.Id,
                            Username = user.Username,
                            Dob = user.Dob,
                            Email = user.Email,
                            Phone = user.Phone,
                            Gender = user.Gender,
                            Address = user.Address,
                            ExistingImage = user.Image
                        };
                        return View(viewModel);
                    }
                }
            }
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ProfileUpdateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await db.Accounts.FindAsync(viewModel.Id);
                if (user != null)
                {
                    if (!string.IsNullOrEmpty(viewModel.Username))
                    {
                        user.Username = viewModel.Username;
                    }

                    if (!string.IsNullOrEmpty(viewModel.Email))
                    {
                        user.Email = viewModel.Email;
                    }

                    if (!string.IsNullOrEmpty(viewModel.Phone))
                    {
                        user.Phone = viewModel.Phone;
                    }

                    if (!string.IsNullOrEmpty(viewModel.Address))
                    {
                        user.Address = viewModel.Address;
                    }

                    if (viewModel.Dob.HasValue)
                    {
                        user.Dob = viewModel.Dob;
                    }

                    if (!string.IsNullOrEmpty(viewModel.Gender))
                    {
                        user.Gender = viewModel.Gender;
                    }

                    if (!string.IsNullOrEmpty(viewModel.Password))
                    {
                        if (viewModel.Password == viewModel.ConfirmPassword)
                        {
                            user.Password = BCrypt.Net.BCrypt.HashPassword(viewModel.Password);
                        }
                        else
                        {
                            ModelState.AddModelError("Password", "The password and confirmation password do not match.");
                            return View("Profile", viewModel);
                        }
                    }

                    if (viewModel.Image != null)
                    {
                        if (!string.IsNullOrEmpty(user.Image))
                        {
                            string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Brand/Car", user.Image);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "Images/Account/Avatar");
                        string filename = Guid.NewGuid().ToString() + "_" + viewModel.Image.FileName;
                        string filePath = Path.Combine(uploadFolder, filename);
                        viewModel.Image.CopyTo(new FileStream(filePath, FileMode.Create));
                        user.Image = filename;
                    }

                    db.Accounts.Update(user);
                    await db.SaveChangesAsync();

                    TempData["success"] = "Profile updated successfully!";
                    return RedirectToAction("Profile");
                }
            }

            return View("Profile", viewModel);
        }

        #endregion

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Denied()
        {
            return View();
        }

        public async Task<IActionResult> UpdateUserAvatar()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await db.Accounts.FindAsync(int.Parse(userId));
                if (user != null)
                {
                    ViewData["UserAvatar"] = user.Image;
                }
            }
            return View();
        }

        private async Task SignInUser(Account user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        }
    }
}