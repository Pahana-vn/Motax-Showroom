using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Motax.Models;
using Motax.ViewModels;
using System.Text.RegularExpressions;

namespace Motax.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Brand")]
    [Authorize(Policy = "CheckAdmin")]
    public class BrandController : Controller
    {
        private readonly MotaxContext db;
        private readonly IWebHostEnvironment webHostEnvironment;

        public BrandController(MotaxContext context, IWebHostEnvironment hc)
        {
            db = context;
            webHostEnvironment = hc;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await db.Brands.OrderByDescending(b => b.Id).ToListAsync());
        }

        #region Create

        [Route("Create")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandAdminVM br)
        {
            if (ModelState.IsValid)
            {
                string filename = "";
                if (br.Image != null)
                {
                    string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "Images/Brand/Car");
                    filename = Guid.NewGuid().ToString() + "_" + br.Image.FileName;
                    string filePath = Path.Combine(uploadFolder, filename);
                    br.Image.CopyTo(new FileStream(filePath, FileMode.Create));
                }

                br.Slug = br.Name ?? "".Replace(" ", "-");

                // Kiểm tra xem tên danh mục đã tồn tại chưa
                bool exists = await db.Brands.AnyAsync(b => b.Name == br.Name);
                if (exists)
                {
                    ModelState.AddModelError("", "Danh mục đã có trong cơ sở dữ liệu.");
                    return View(br);
                }

                Brand b = new Brand()
                {
                    Name = br.Name ?? "",
                    ContactPerson = br.ContactPerson,
                    Email = br.Email,
                    Phone = br.Phone,
                    Address = br.Address,
                    Slug = br.Slug,
                    Description = br.Description ?? "",
                    Status = br.Status,
                    Image = filename
                };
                db.Brands.Add(b);
                await db.SaveChangesAsync();
                TempData["success"] = "Create success brand!";
                return RedirectToAction("Index");
            }
            return View(br);
        }
        #endregion

        #region Edit
        [Route("Edit")]
        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            Brand? br = await db.Brands.FindAsync(Id);
            if (br == null)
            {
                return RedirectToAction("/404");
            }

            BrandAdminVM vm = new BrandAdminVM
            {
                Id = br.Id,
                Name = br.Name,
                ContactPerson = br.ContactPerson,
                Email = br.Email,
                Phone = br.Phone,
                Address = br.Address,
                Slug = br.Slug,
                Description = br.Description,
                Status = br.Status,
                ExistingImage = br.Image // Biến để giữ hình ảnh hiện tại
            };

            return View(vm);
        }

        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BrandAdminVM br)
        {
            if (ModelState.IsValid)
            {
                var BrandToUpdate = await db.Brands.FindAsync(br.Id);
                if (BrandToUpdate == null)
                {
                    return NotFound();
                }

                // Kiểm tra xem hình ảnh có thay đổi hay không
                if (br.Image != null)
                {
                    // Xóa hình ảnh cũ nếu có
                    if (!string.IsNullOrEmpty(BrandToUpdate.Image))
                    {
                        string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Brand/Car", BrandToUpdate.Image);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Thêm hình ảnh mới
                    string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "Images/Brand/Car");
                    string filename = Guid.NewGuid().ToString() + "_" + br.Image.FileName;
                    string filePath = Path.Combine(uploadFolder, filename);
                    br.Image.CopyTo(new FileStream(filePath, FileMode.Create));
                    BrandToUpdate.Image = filename;
                }

                BrandToUpdate.Name = br.Name ?? "";
                BrandToUpdate.ContactPerson = br.ContactPerson ?? "";
                BrandToUpdate.Email = br.Email ?? "";
                BrandToUpdate.Phone = br.Phone ?? "";
                BrandToUpdate.Address = br.Address ?? "";
                BrandToUpdate.Slug = br.Name ?? "".Replace(" ", "-");
                BrandToUpdate.Description = br.Description ?? "";
                BrandToUpdate.Status = br.Status;

                db.Update(BrandToUpdate);
                await db.SaveChangesAsync();
                TempData["success"] = "Brand Update successfully!";
                return RedirectToAction("Index");
            }

            return View(br);
        }

        #endregion

        #region Delete
        [Route("Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int Id)
        {
            var brandToDelete = await db.Brands.FindAsync(Id);
            if (brandToDelete == null)
            {
                return NotFound();
            }

            // Check for related records in Cars table
            bool hasRelatedCars = await db.Cars.AnyAsync(c => c.BrandId == Id);

            if (hasRelatedCars)
            {
                TempData["error"] = "Cannot delete brand. This brand is referenced by one or more cars.";
                return RedirectToAction("Index");
            }

            // Delete image if exists
            if (!string.IsNullOrEmpty(brandToDelete.Image))
            {
                string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Brand/Car", brandToDelete.Image);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            db.Brands.Remove(brandToDelete);
            await db.SaveChangesAsync();
            TempData["success"] = "Brand has been deleted successfully";
            return RedirectToAction("Index");
        }
        #endregion
    }
}
