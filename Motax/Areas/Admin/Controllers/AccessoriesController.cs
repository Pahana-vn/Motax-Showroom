using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Motax.Models;
using Motax.ViewModels;
using System.Text;

namespace Motax.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Accessories")]
    [Authorize(Policy = "CheckAdmin")]
    public class AccessoriesController : Controller
    {
        private readonly MotaxContext db;
        private readonly IWebHostEnvironment webHostEnvironment;
        public AccessoriesController(MotaxContext context, IWebHostEnvironment hc)
        {
            db = context;
            webHostEnvironment = hc;
        }

        private string GenerateRandomTypeCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }
            return result.ToString();
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await db.Accessories.OrderByDescending(b => b.Id).ToListAsync());
        }

        #region Create
        [Route("Create")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.categories = new SelectList(db.Categories, "Id", "Name");
            ViewBag.brands = new SelectList(db.Brands, "Id", "Name");
            return View();
        }

        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AccessoriesAdminVM accVM)
        {
            if (ModelState.IsValid)
            {
                string singleImagePath = "";
                if (accVM.ImageSingle != null)
                {
                    string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "Images/Accessories/Single");
                    singleImagePath = Guid.NewGuid().ToString() + "_" + accVM.ImageSingle.FileName;
                    string singleFilePath = Path.Combine(uploadFolder, singleImagePath);
                    using (var fileStream = new FileStream(singleFilePath, FileMode.Create))
                    {
                        await accVM.ImageSingle.CopyToAsync(fileStream);
                    }
                }

                List<string> multipleImagePaths = new List<string>();
                if (accVM.ImageMultiple != null && accVM.ImageMultiple.Count > 0)
                {
                    string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "Images/Accessories/Multiple");
                    foreach (var imageFile in accVM.ImageMultiple)
                    {
                        string multipleImagePath = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        string multipleFilePath = Path.Combine(uploadFolder, multipleImagePath);
                        using (var fileStream = new FileStream(multipleFilePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }
                        multipleImagePaths.Add("/Images/Accessories/Multiple/" + multipleImagePath);
                    }
                }

                accVM.Slug = accVM.Name ?? "".Replace(" ", "-");
                // Kiểm tra xem tên accessories đã tồn tại chưa
                bool exists = await db.Accessories.AnyAsync(b => b.Name == accVM.Name);
                if (exists)
                {
                    ModelState.AddModelError("", "Accessories đã có trong cơ sở dữ liệu.");
                    return View(accVM);
                }

                var accessory = new Accessories
                {
                    Name = accVM.Name,
                    TypeCode = GenerateRandomTypeCode(7),
                    Description = accVM.Description,
                    CategoryId = accVM.CategoryId,
                    BrandId = accVM.BrandId,
                    Price = accVM.Price,
                    Status = accVM.Status,
                    Slug = accVM.Slug,
                    ImageSingle = singleImagePath,
                    ImageMultiple = string.Join(",", multipleImagePaths)

                };

                db.Accessories.Add(accessory);
                await db.SaveChangesAsync();
                TempData["success"] = "Accessory created successfully!";
                return RedirectToAction("Index");
            }

            ViewBag.categories = new SelectList(db.Categories, "Id", "Name");
            ViewBag.brands = new SelectList(db.Brands, "Id", "Name");
            return View(accVM);
        }
        #endregion

        #region Edit
        [Route("Edit")]
        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            Accessories? acce = await db.Accessories.FindAsync(Id);
            if (acce == null)
            {
                return RedirectToAction("/404");
            }

            AccessoriesAdminVM vm = new AccessoriesAdminVM
            {
                Name = acce.Name,
                TypeCode = acce.TypeCode,
                Description = acce.Description,
                CategoryId = acce.CategoryId,
                BrandId = acce.BrandId,
                Price = acce.Price,
                Status = acce.Status,
                Slug = acce.Slug,
                ExistingImageSingle = acce.ImageSingle,
                ExistingImageMultiple = acce.ImageMultiple?.Split(",").ToList() ?? new List<string>()
            };

            ViewBag.categories = new SelectList(db.Categories, "Id", "Name", acce.CategoryId);
            ViewBag.brands = new SelectList(db.Brands, "Id", "Name", acce.BrandId);
            return View(vm);
        }

        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AccessoriesAdminVM accVM)
        {
            if (ModelState.IsValid)
            {
                var accToUpdate = await db.Accessories.FindAsync(accVM.Id);
                if (accToUpdate == null)
                {
                    return NotFound();
                }

                if (accVM.ImageSingle != null)
                {
                    // Xóa ảnh cũ nếu tồn tại
                    if (!string.IsNullOrEmpty(accToUpdate.ImageSingle))
                    {
                        string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Accessories/Single", accToUpdate.ImageSingle);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Lưu ảnh mới
                    string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "Images/Accessories/Single");
                    string filename = Guid.NewGuid().ToString() + "_" + accVM.ImageSingle.FileName;
                    string filePath = Path.Combine(uploadFolder, filename);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await accVM.ImageSingle.CopyToAsync(fileStream);
                    }
                    accToUpdate.ImageSingle = filename;
                }

                if (accVM.ImageMultiple != null && accVM.ImageMultiple.Count > 0)
                {
                    if (!string.IsNullOrEmpty(accToUpdate.ImageMultiple))
                    {
                        var existingImages = accToUpdate.ImageMultiple.Split(",");
                        foreach (var image in existingImages)
                        {
                            string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Accessories/Multiple", image);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }
                    }

                    List<string> multipleImagePaths = new List<string>();
                    string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "Images/Accessories/Multiple");
                    foreach (var imageFile in accVM.ImageMultiple)
                    {
                        string multipleImagePath = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        string multipleFilePath = Path.Combine(uploadFolder, multipleImagePath);
                        using (var fileStream = new FileStream(multipleFilePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }
                        multipleImagePaths.Add("/Images/Accessories/Multiple/" + multipleImagePath);
                    }
                    accToUpdate.ImageMultiple = string.Join(",", multipleImagePaths);
                }

                accToUpdate.Name = accVM.Name;
                accToUpdate.TypeCode = accVM.TypeCode;
                accToUpdate.Description = accVM.Description;
                accToUpdate.CategoryId = accVM.CategoryId;
                accToUpdate.BrandId = accVM.BrandId;
                accToUpdate.Price = accVM.Price;
                accToUpdate.Slug = accVM.Name?.Replace(" ", "-") ?? accToUpdate.Slug;
                accToUpdate.Status = accVM.Status;

                db.Update(accToUpdate);
                await db.SaveChangesAsync();
                TempData["success"] = "Accessories updated successfully!";
                return RedirectToAction("Index");
            }

            ViewBag.categories = new SelectList(db.Categories, "Id", "Name", accVM.CategoryId);
            ViewBag.brands = new SelectList(db.Brands, "Id", "Name", accVM.BrandId);
            return View(accVM);
        }

        #endregion

        #region Delete
        [Route("Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int Id)
        {
            var accessoryToDelete = await db.Accessories.FindAsync(Id);
            if (accessoryToDelete == null)
            {
                return NotFound();
            }

            // Check for related records in OrderAccessories table
            bool hasRelatedOrders = await db.OrderDetailAccessories.AnyAsync(o => o.AccessoriesId == Id);
            // Check for related records in Category table
            bool hasRelatedCategory = await db.Categories.AnyAsync(c => c.Id == accessoryToDelete.CategoryId);
            // Check for related records in Brand table
            bool hasRelatedBrand = await db.Brands.AnyAsync(b => b.Id == accessoryToDelete.BrandId);

            if (hasRelatedOrders || hasRelatedCategory || hasRelatedBrand)
            {
                TempData["error"] = "Cannot delete accessory. This accessory has related records in other tables.";
                return RedirectToAction("Index");
            }

            // Delete image if exists
            if (!string.IsNullOrEmpty(accessoryToDelete.ImageSingle))
            {
                string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Accessories/Single", accessoryToDelete.ImageSingle);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            // Delete multiple images if exists
            if (!string.IsNullOrEmpty(accessoryToDelete.ImageMultiple))
            {
                var existingImages = accessoryToDelete.ImageMultiple.Split(",");
                foreach (var image in existingImages)
                {
                    string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Accessories/Multiple", image);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
            }

            db.Accessories.Remove(accessoryToDelete);
            await db.SaveChangesAsync();
            TempData["success"] = "Accessory deleted successfully!";
            return RedirectToAction("Index");
        }



        #endregion
    }
}
