using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Motax.Models;
using Motax.ViewModels;

namespace Motax.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Secure")]
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

        [Route("Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int Id)
        {
            var AccountToDelete = await db.Accounts.FindAsync(Id);
            if (AccountToDelete == null)
            {
                return NotFound();
            }

            // Xóa hình ảnh cũ nếu có
            if (!string.IsNullOrEmpty(AccountToDelete.Image))
            {
                string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Account/Avatar", AccountToDelete.Image);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            db.Accounts.Remove(AccountToDelete);
            await db.SaveChangesAsync();
            TempData["success"] = "Account has been deleted";
            return RedirectToAction("Index");
        }
    }
}
