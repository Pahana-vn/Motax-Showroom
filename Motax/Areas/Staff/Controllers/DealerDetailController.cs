using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Motax.Models;
using Motax.ViewModels;
using System.IO;
using System.Threading.Tasks;

namespace Motax.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Route("Staff/DealerDetail")]
    [Authorize(Policy = "CheckAdminOrStaff")]
    public class DealerDetailController : Controller
    {
        private readonly MotaxContext db;
        private readonly IWebHostEnvironment webHostEnvironment;

        public DealerDetailController(MotaxContext context, IWebHostEnvironment hc)
        {
            db = context;
            webHostEnvironment = hc;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await db.DealerDetails.OrderByDescending(b => b.Id).ToListAsync());
        }

        #region Create
        [Route("Create")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.dealers = new SelectList(db.Dealers, "Id", "Name");
            return View();
        }

        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DealerDetailAdminVM dd)
        {
            if (ModelState.IsValid)
            {
                string coverImageFilename = "", avatarImageFilename = "", consultantAvatarFilename = "";

                if (dd.CoverImage != null)
                {
                    string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "Images/Dealers/DealerDetail/Cover");
                    coverImageFilename = Guid.NewGuid().ToString() + "_" + dd.CoverImage.FileName;
                    string filePath = Path.Combine(uploadFolder, coverImageFilename);
                    dd.CoverImage.CopyTo(new FileStream(filePath, FileMode.Create));
                }

                if (dd.AvatarImage != null)
                {
                    string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "Images/Dealers/DealerDetail/Avatar");
                    avatarImageFilename = Guid.NewGuid().ToString() + "_" + dd.AvatarImage.FileName;
                    string filePath = Path.Combine(uploadFolder, avatarImageFilename);
                    dd.AvatarImage.CopyTo(new FileStream(filePath, FileMode.Create));
                }

                if (dd.ConsultantAvatar != null)
                {
                    string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "Images/Dealers/DealerDetail/Consultant");
                    consultantAvatarFilename = Guid.NewGuid().ToString() + "_" + dd.ConsultantAvatar.FileName;
                    string filePath = Path.Combine(uploadFolder, consultantAvatarFilename);
                    dd.ConsultantAvatar.CopyTo(new FileStream(filePath, FileMode.Create));
                }

                DealerDetail dealerDetail = new DealerDetail()
                {
                    DealerId = dd.DealerId,
                    CoverImage = coverImageFilename,
                    AvatarImage = avatarImageFilename,
                    ConsultantName = dd.ConsultantName,
                    ConsultantAvatar = consultantAvatarFilename
                };

                db.DealerDetails.Add(dealerDetail);
                await db.SaveChangesAsync();
                TempData["success"] = "Create success dealer detail!";
                return RedirectToAction("Index");
            }

            return View(dd);
        }
        #endregion

        #region Edit
        [Route("Edit")]
        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            DealerDetail? dd = await db.DealerDetails.FindAsync(Id);
            if (dd == null)
            {
                return RedirectToAction("/404");
            }

            DealerDetailAdminVM vm = new DealerDetailAdminVM
            {
                Id = dd.Id,
                DealerId = dd.DealerId,
                ConsultantName = dd.ConsultantName,
                ExistingCoverImage = dd.CoverImage,
                ExistingAvatarImage = dd.AvatarImage,
                ExistingConsultantAvatar = dd.ConsultantAvatar
            };

            ViewBag.dealers = new SelectList(db.Dealers, "Id", "Name");
            return View(vm);
        }

        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DealerDetailAdminVM dd)
        {
            if (ModelState.IsValid)
            {
                var dealerDetailToUpdate = await db.DealerDetails.FindAsync(dd.Id);
                if (dealerDetailToUpdate == null)
                {
                    return NotFound();
                }

                if (dd.CoverImage != null)
                {
                    if (!string.IsNullOrEmpty(dealerDetailToUpdate.CoverImage))
                    {
                        string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Dealers/DealerDetail/Cover", dealerDetailToUpdate.CoverImage);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "Images/Dealers/DealerDetail/Cover");
                    string filename = Guid.NewGuid().ToString() + "_" + dd.CoverImage.FileName;
                    string filePath = Path.Combine(uploadFolder, filename);
                    dd.CoverImage.CopyTo(new FileStream(filePath, FileMode.Create));
                    dealerDetailToUpdate.CoverImage = filename;
                }

                if (dd.AvatarImage != null)
                {
                    if (!string.IsNullOrEmpty(dealerDetailToUpdate.AvatarImage))
                    {
                        string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Dealers/DealerDetail/Avatar", dealerDetailToUpdate.AvatarImage);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "Images/Dealers/DealerDetail/Avatar");
                    string filename = Guid.NewGuid().ToString() + "_" + dd.AvatarImage.FileName;
                    string filePath = Path.Combine(uploadFolder, filename);
                    dd.AvatarImage.CopyTo(new FileStream(filePath, FileMode.Create));
                    dealerDetailToUpdate.AvatarImage = filename;
                }

                if (dd.ConsultantAvatar != null)
                {
                    if (!string.IsNullOrEmpty(dealerDetailToUpdate.ConsultantAvatar))
                    {
                        string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Dealers/DealerDetail/Consultant", dealerDetailToUpdate.ConsultantAvatar);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "Images/Dealers/DealerDetail/Consultant");
                    string filename = Guid.NewGuid().ToString() + "_" + dd.ConsultantAvatar.FileName;
                    string filePath = Path.Combine(uploadFolder, filename);
                    dd.ConsultantAvatar.CopyTo(new FileStream(filePath, FileMode.Create));
                    dealerDetailToUpdate.ConsultantAvatar = filename;
                }

                dealerDetailToUpdate.DealerId = dd.DealerId;
                dealerDetailToUpdate.ConsultantName = dd.ConsultantName;

                db.Update(dealerDetailToUpdate);
                await db.SaveChangesAsync();
                TempData["success"] = "Dealer detail updated successfully!";
                return RedirectToAction("Index");
            }

            return View(dd);
        }
        #endregion

        #region Delete
        [Route("Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int Id)
        {
            var dealerDetailToDelete = await db.DealerDetails.FindAsync(Id);
            if (dealerDetailToDelete == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(dealerDetailToDelete.CoverImage))
            {
                string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Dealers/DealerDetail/Cover", dealerDetailToDelete.CoverImage);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            if (!string.IsNullOrEmpty(dealerDetailToDelete.AvatarImage))
            {
                string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Dealers/DealerDetail/Avatar", dealerDetailToDelete.AvatarImage);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            if (!string.IsNullOrEmpty(dealerDetailToDelete.ConsultantAvatar))
            {
                string oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images/Dealers/DealerDetail/Consultant", dealerDetailToDelete.ConsultantAvatar);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            db.DealerDetails.Remove(dealerDetailToDelete);
            await db.SaveChangesAsync();
            TempData["success"] = "Dealer detail deleted successfully!";
            return RedirectToAction("Index");
        }
        #endregion
    }
}
