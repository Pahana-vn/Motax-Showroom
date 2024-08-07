using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Motax.Models;
using Motax.ViewModels;
using System.IO;

namespace Motax.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Category")]
    [Authorize(Policy = "CheckAdmin")]
    public class CategoryController : Controller
    {
        private readonly MotaxContext db;
        private readonly IWebHostEnvironment webHostEnvironment;

        public CategoryController(MotaxContext context, IWebHostEnvironment hc)
        {
            db = context;
            webHostEnvironment = hc;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await db.Categories.OrderByDescending(p => p.Id).ToListAsync());
        }

        [Route("Create")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryAdminVM cate)
        {
            if (ModelState.IsValid)
            {
                string filename = "";
                if (cate.Image != null)
                {
                    string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "Images/Category/Car");
                    filename = Guid.NewGuid().ToString() + "_" + cate.Image.FileName;
                    string filePath = Path.Combine(uploadFolder, filename);
                    cate.Image.CopyTo(new FileStream(filePath, FileMode.Create));
                }

                cate.Slug = cate.Name ?? "".Replace(" ", "-");

                // Kiểm tra xem tên danh mục đã tồn tại chưa
                bool exists = await db.Categories.AnyAsync(c => c.Name == cate.Name);
                if (exists)
                {
                    ModelState.AddModelError("", "Danh mục đã có trong cơ sở dữ liệu.");
                    return View(cate);
                }

                Category c = new Category()
                {
                    Name = cate.Name ?? "",
                    Slug = cate.Slug,
                    Description = cate.Description ?? "",
                    Status = cate.Status,
                    Image = filename
                };
                db.Categories.Add(c);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(cate);
        }

        [Route("Edit")]
        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            Category? cate = await db.Categories.FindAsync(Id);
            if (cate == null)
            {
                return NotFound();
            }

            CategoryAdminVM vm = new CategoryAdminVM
            {
                Id = cate.Id,
                Name = cate.Name,
                Slug = cate.Slug,
                Description = cate.Description,
                Status = cate.Status,
                ExistingImage = cate.Image // Biến để giữ hình ảnh hiện tại
            };

            return View(vm);
        }

        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryAdminVM cate)
        {
            if (ModelState.IsValid)
            {
                var categoryToUpdate = await db.Categories.FindAsync(cate.Id);
                if (categoryToUpdate == null)
                {
                    return NotFound();
                }

                // Kiểm tra xem hình ảnh có thay đổi hay không
                if (cate.Image != null)
                {
                    // Xóa hình ảnh cũ nếu có
                    if (!string.IsNullOrEmpty(categoryToUpdate.Image))
                    {
                        string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Category/Car", categoryToUpdate.Image);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Thêm hình ảnh mới
                    string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "Images/Category/Car");
                    string filename = Guid.NewGuid().ToString() + "_" + cate.Image.FileName;
                    string filePath = Path.Combine(uploadFolder, filename);
                    cate.Image.CopyTo(new FileStream(filePath, FileMode.Create));
                    categoryToUpdate.Image = filename;
                }

                categoryToUpdate.Name = cate.Name ?? "";
                categoryToUpdate.Slug = cate.Name ?? "".Replace(" ", "-");
                categoryToUpdate.Description = cate.Description ?? "";
                categoryToUpdate.Status = cate.Status;

                db.Update(categoryToUpdate);
                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(cate);
        }

        #region Delete
        [Route("Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int Id)
        {
            var categoryToDelete = await db.Categories.FindAsync(Id);
            if (categoryToDelete == null)
            {
                return NotFound();
            }

            // Check for related records in Accessories table
            bool hasRelatedAccessories = await db.Accessories.AnyAsync(a => a.CategoryId == Id);

            if (hasRelatedAccessories)
            {
                TempData["error"] = "Cannot delete category. This category is referenced by one or more accessories.";
                return RedirectToAction("Index");
            }

            // Delete image if exists
            if (!string.IsNullOrEmpty(categoryToDelete.Image))
            {
                string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Category/Car", categoryToDelete.Image);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            db.Categories.Remove(categoryToDelete);
            await db.SaveChangesAsync();
            TempData["success"] = "Category has been deleted successfully";
            return RedirectToAction("Index");
        }

        #endregion
    }
}
