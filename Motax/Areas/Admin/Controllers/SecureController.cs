using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Motax.Models;
using Motax.ViewModels;
using System.Security.Claims;

namespace Motax.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Secure")]
    [Authorize(Policy = "CheckAdmin")]
    public class SecureController : Controller
    {
        private readonly MotaxContext db;
        private readonly IWebHostEnvironment webHostEnvironment;
        public SecureController(MotaxContext context, IWebHostEnvironment hc)
        {
            db = context;
            webHostEnvironment = hc;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await db.Accounts.OrderByDescending(b => b.Id).ToListAsync());
        }

        private IEnumerable<SelectListItem> GetRoles()
        {
            return db.Roles.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.Title
            }).ToList();
        }


        #region Create
        [HttpGet]
        public IActionResult Create()
        {
            var model = new RegisterAdminViewModel
            {
                Roles = GetRoles()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterAdminViewModel acc)
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
                    RoleId = acc.RoleId,
                };
                db.Accounts.Add(user);
                await db.SaveChangesAsync();
                TempData["success"] = "Account created successfully!";
                return RedirectToAction("Index");
            }
            acc.Roles = GetRoles(); // Re-populate the roles in case of an error
            return View(acc);
        }
        #endregion

        #region Edit
        [Route("Edit")]
        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            var account = await db.Accounts.FindAsync(Id);
            if (account == null)
            {
                return RedirectToAction("/404");
            }

            var vm = new RegisterAdminViewModel
            {
                Id = account.Id,
                Username = account.Username,
                Email = account.Email,
                Phone = account.Phone,
                Address = account.Address,
                Dob = account.Dob,
                Gender = account.Gender,
                ExistingImage = account.Image,
                RoleId = account.RoleId
            };

            vm.Roles = GetRoles(); // Populate the roles dropdown

            return View(vm);
        }

        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RegisterAdminViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var accountToUpdate = await db.Accounts.FindAsync(vm.Id);
                if (accountToUpdate == null)
                {
                    return NotFound();
                }

                // Check if image has changed
                if (vm.Image != null)
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(accountToUpdate.Image))
                    {
                        string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Account/Avatar", accountToUpdate.Image);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Add new image
                    string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "Images/Account/Avatar");
                    string filename = Guid.NewGuid().ToString() + "_" + vm.Image.FileName;
                    string filePath = Path.Combine(uploadFolder, filename);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await vm.Image.CopyToAsync(fileStream);
                    }
                    accountToUpdate.Image = filename;
                }

                accountToUpdate.Username = vm.Username ?? "";
                accountToUpdate.Email = vm.Email ?? "";
                accountToUpdate.Phone = vm.Phone ?? "";
                accountToUpdate.Address = vm.Address ?? "";
                accountToUpdate.Dob = vm.Dob;
                accountToUpdate.Gender = vm.Gender;
                accountToUpdate.RoleId = vm.RoleId;

                // Hash new password if provided
                if (!string.IsNullOrEmpty(vm.Password))
                {
                    accountToUpdate.Password = BCrypt.Net.BCrypt.HashPassword(vm.Password);
                }

                db.Update(accountToUpdate);
                await db.SaveChangesAsync();
                TempData["success"] = "Account updated successfully!";
                return RedirectToAction("Index");
            }

            vm.Roles = GetRoles(); // Re-populate roles in case of an error
            return View(vm);
        }
        #endregion

        #region Delete
        [Route("Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int Id)
        {
            var accountToDelete = await db.Accounts.FindAsync(Id);
            if (accountToDelete == null)
            {
                return NotFound();
            }

            // Check for related records
            bool hasRelatedInvoices = await db.Invoices.AnyAsync(i => i.UserId == Id);
            bool hasRelatedOrders = await db.Orders.AnyAsync(o => o.AccountId == Id);
            bool hasRelatedCarRegistrations = await db.CarRegistrations.AnyAsync(cr => cr.UserId == Id);


            bool hasRelatedCompares = await db.Compares.AnyAsync(c => c.UserId == Id);
            bool hasRelatedWishlists = await db.Wishlists.AnyAsync(w => w.UserId == Id);

            if (hasRelatedInvoices || hasRelatedOrders || hasRelatedCarRegistrations || hasRelatedCompares || hasRelatedWishlists)
            {
                TempData["error"] = "Cannot delete account. This account has related records in other tables.";
                return RedirectToAction("Index");
            }

            // Delete image if exists
            if (!string.IsNullOrEmpty(accountToDelete.Image))
            {
                string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Account/Avatar", accountToDelete.Image);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            db.Accounts.Remove(accountToDelete);
            try
            {
                await db.SaveChangesAsync();
                TempData["success"] = "Account has been deleted successfully!";
            }
            catch (DbUpdateException ex)
            {
                TempData["error"] = $"Error deleting account: {ex.InnerException?.Message ?? ex.Message}";
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region Detail
        [Route("Detail")]
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var account = await db.Accounts
                                  .Include(a => a.Role)
                                  .FirstOrDefaultAsync(a => a.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            var vm = new AccountDetailAdminVM
            {
                Id = account.Id,
                Username = account.Username,
                Email = account.Email,
                Phone = account.Phone,
                Address = account.Address,
                Dob = account.Dob,
                Gender = account.Gender,
                Image = account.Image,
                Role = account.Role.Title,
                Status = account.Status == 1 ? "Active" : "Inactive"
            };

            return View(vm);
        }
        #endregion

        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Secure");
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
