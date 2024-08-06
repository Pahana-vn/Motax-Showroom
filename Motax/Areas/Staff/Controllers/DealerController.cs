using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Motax.Models;
using Motax.ViewModels;

namespace Motax.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Route("Staff/Dealer")]
    [Authorize(Policy = "CheckAdminOrStaff")]
    public class DealerController : Controller
    {
        private readonly MotaxContext db;
        private readonly IWebHostEnvironment webHostEnvironment;

        public DealerController(MotaxContext context, IWebHostEnvironment hc)
        {
            db = context;
            webHostEnvironment = hc;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var dealers = await db.Dealers
                .Include(d => d.ContactMessages)
                .OrderByDescending(d => d.Id)
                .ToListAsync();

            var dealerVMs = dealers.Select(d => new DealerAdminVM
            {
                Id = d.Id,
                Name = d.Name,
                Phone = d.Phone,
                Email = d.Email,
                Address = d.Address,
                City = d.City,
                Status = d.Status,
                ExistingImage = d.ImageBackground,
                ImageBackgroundPath = d.ImageBackground,
                MessageCount = d.ContactMessages.Count  // Lấy số lượng tin nhắn
            }).ToList();

            return View(dealerVMs);
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
        public async Task<IActionResult> Create(DealerAdminVM dl)
        {
            if (ModelState.IsValid)
            {
                string filename = "";
                if (dl.ImageBackground != null)
                {
                    string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "Images/Dealers/Dealer");
                    filename = Guid.NewGuid().ToString() + "_" + dl.ImageBackground.FileName;
                    string filePath = Path.Combine(uploadFolder, filename);
                    dl.ImageBackground.CopyTo(new FileStream(filePath, FileMode.Create));
                }

                // Kiểm tra xem tên danh mục đã tồn tại chưa
                bool exists = await db.Dealers.AnyAsync(d => d.Name == dl.Name);
                if (exists)
                {
                    ModelState.AddModelError("", "The dealer name is already in the database.");
                    return View(dl);
                }

                Dealer d = new Dealer()
                {
                    Name = dl.Name ?? "",
                    Phone = dl.Phone,
                    Email = dl.Email,
                    Address = dl.Address,
                    City = dl.City,
                    Status = dl.Status,
                    ImageBackground = filename
                };
                db.Dealers.Add(d);
                await db.SaveChangesAsync();
                TempData["success"] = "Create success dealer!";
                return RedirectToAction("Index");
            }
            return View(dl);
        }
        #endregion

        #region Edit
        [Route("Edit")]
        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            Dealer? dl = await db.Dealers.FindAsync(Id);
            if (dl == null)
            {
                return RedirectToAction("/404");
            }

            DealerAdminVM dvm = new DealerAdminVM
            {
                Id = dl.Id,
                Name = dl.Name,
                Email = dl.Email,
                Phone = dl.Phone,
                Address = dl.Address,
                City = dl.City,
                Status = dl.Status,
                ExistingImage = dl.ImageBackground // Biến để giữ hình ảnh hiện tại
            };

            return View(dvm);
        }

        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DealerAdminVM dl)
        {
            if (ModelState.IsValid)
            {
                var DealerToUpdate = await db.Dealers.FindAsync(dl.Id);
                if (DealerToUpdate == null)
                {
                    return RedirectToAction("/404");
                }

                // Kiểm tra xem hình ảnh có thay đổi hay không
                if (dl.ImageBackground != null)
                {
                    // Xóa hình ảnh cũ nếu có
                    if (!string.IsNullOrEmpty(DealerToUpdate.ImageBackground))
                    {
                        string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Dealers/Dealer", DealerToUpdate.ImageBackground);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Thêm hình ảnh mới
                    string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "Images/Dealers/Dealer");
                    string filename = Guid.NewGuid().ToString() + "_" + dl.ImageBackground.FileName;
                    string filePath = Path.Combine(uploadFolder, filename);
                    dl.ImageBackground.CopyTo(new FileStream(filePath, FileMode.Create));
                    DealerToUpdate.ImageBackground = filename;
                }

                DealerToUpdate.Name = dl.Name ?? "";
                DealerToUpdate.Email = dl.Email ?? "";
                DealerToUpdate.Phone = dl.Phone ?? "";
                DealerToUpdate.Address = dl.Address ?? "";
                DealerToUpdate.City = dl.City ?? "";
                DealerToUpdate.Status = dl.Status;

                db.Update(DealerToUpdate);
                await db.SaveChangesAsync();
                TempData["success"] = "Dealer Update successfully!";
                return RedirectToAction("Index");
            }

            return View(dl);
        }

        #endregion

        #region Delete
        [Route("Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int Id)
        {
            var DealerToDelete = await db.Dealers.FindAsync(Id);
            if (DealerToDelete == null)
            {
                return RedirectToAction("/404");
            }

            // Xóa hình ảnh cũ nếu có
            if (!string.IsNullOrEmpty(DealerToDelete.ImageBackground))
            {
                string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Dealers/Dealer", DealerToDelete.ImageBackground);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            db.Dealers.Remove(DealerToDelete);
            await db.SaveChangesAsync();
            TempData["success"] = "Dealer has been successfully deleted";
            return RedirectToAction("Index");
        }
        #endregion

        [Route("Messages/{id}")]
        public async Task<IActionResult> Messages(int id)
        {
            var dealer = await db.Dealers
                .Include(d => d.ContactMessages)
                .SingleOrDefaultAsync(d => d.Id == id);

            if (dealer == null)
            {
                TempData["error"] = "Dealer not found";
                return RedirectToAction("Index");
            }

            var messages = dealer.ContactMessages.Select(m => new ContactMessageVM
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Message = m.Message,
                CreatedAt = m.CreatedAt
            }).ToList();

            return View(messages);
        }

        [HttpPost]
        [Route("DeleteMessage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMessage(int id, int dealerId)
        {
            var message = await db.ContactMessages.FindAsync(id);
            if (message != null)
            {
                db.ContactMessages.Remove(message);
                await db.SaveChangesAsync();
                TempData["success"] = "Message deleted successfully!";
            }
            else
            {
                TempData["error"] = "Message not found!";
            }

            return RedirectToAction("Index");
        }

        [Route("Detail/{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            var dealer = await db.Dealers
                .Include(d => d.DealerDetails)
                .SingleOrDefaultAsync(d => d.Id == id);

            if (dealer == null)
            {
                TempData["error"] = "Dealer not found";
                return RedirectToAction("Index");
            }

            var dealerDetail = dealer.DealerDetails.FirstOrDefault();
            var dealerDetailVM = new DealerDetailViewModelVM2
            {
                Id = dealer.Id,
                Name = dealer.Name,
                Email = dealer.Email,
                Phone = dealer.Phone,
                Address = dealer.Address,
                City = dealer.City,
                Status = dealer.Status,
                ExistingImage = dealer.ImageBackground,
                ConsultantName = dealerDetail?.ConsultantName,
                ExistingCoverImage = dealerDetail?.CoverImage,
                ExistingAvatarImage = dealerDetail?.AvatarImage,
                ExistingConsultantAvatar = dealerDetail?.ConsultantAvatar
            };

            return View(dealerDetailVM);
        }



    }
}
