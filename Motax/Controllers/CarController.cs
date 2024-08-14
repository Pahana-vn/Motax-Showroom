using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Motax.Models;
using Motax.ViewModels;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace Motax.Controllers
{
    public class CarController : Controller
    {
        private readonly MotaxContext db;

        public CarController(MotaxContext context)
        {
            db = context;
        }

        #region Index
        public IActionResult Index(int? brand, int? dealer, int sortOption = 1, int page = 1, int pageSize = 9)
        {
            var cars = db.Cars.Include(p => p.Comments).AsQueryable();

            if (brand.HasValue && brand.Value != 0)
            {
                cars = cars.Where(p => p.BrandId == brand.Value);
            }

            if (dealer.HasValue)
            {
                cars = cars.Where(p => p.DealerId == dealer.Value);
            }

            // Xử lý sắp xếp
            switch (sortOption)
            {
                case 2: // Sort by Latest
                    cars = cars.OrderByDescending(p => p.Year);
                    break;
                case 3: // Sort by Low Price
                    cars = cars.OrderBy(p => p.Price);
                    break;
                case 4: // Sort by High Price
                    cars = cars.OrderByDescending(p => p.Price);
                    break;
                case 5: // Sort by Featured
                    cars = cars.OrderByDescending(p => p.Comments.Count());
                    break;
                default: // Sort by Default
                    cars = cars.OrderBy(p => p.Name);
                    break;
            }

            var result = cars.Select(p => new CarVM
            {
                Id = p.Id,
                Name = p.Name,
                Condition = p.Condition,
                FuelType = p.FuelType,
                Mileage = p.Mileage,
                Transmission = p.Transmission,
                Year = p.Year,
                Price = p.Price,
                ImageSingle = p.ImageSingle,
                NameBrand = p.Brand != null ? p.Brand.Name : null,
                IsAvailable = p.IsAvailable,
                AverageRating = p.Comments.Any() ? p.Comments.Average(c => c.Rating) : 0,
                ReviewCount = p.Comments.Count()
            }).ToPagedList(page, pageSize);

            // Tạo danh sách SelectListItem và truyền qua ViewBag
            ViewBag.SortOptions = new List<SelectListItem>
    {
        new SelectListItem { Value = "1", Text = "Sort By Default", Selected = sortOption == 1 },
        new SelectListItem { Value = "5", Text = "Sort By Featured", Selected = sortOption == 5 },
        new SelectListItem { Value = "2", Text = "Sort By Latest", Selected = sortOption == 2 },
        new SelectListItem { Value = "3", Text = "Sort By Low Price", Selected = sortOption == 3 },
        new SelectListItem { Value = "4", Text = "Sort By High Price", Selected = sortOption == 4 }
    };

            ViewBag.CurrentSort = sortOption;

            return View(result);
        }





        public IActionResult Index2(int? brand, int? dealer)
        {
            var cars = db.Cars
                .Include(p => p.Comments)
                .AsQueryable();

            if (brand.HasValue && brand.Value != 0)
            {
                cars = cars.Where(p => p.BrandId == brand.Value);
            }

            if (dealer.HasValue)
            {
                cars = cars.Where(p => p.DealerId == dealer.Value);
            }

            var result = cars.Select(p => new CarVM
            {
                Id = p.Id,
                Name = p.Name,
                Condition = p.Condition,
                FuelType = p.FuelType,
                Mileage = p.Mileage,
                Transmission = p.Transmission,
                Year = p.Year,
                Price = p.Price,
                ImageSingle = p.ImageSingle,
                NameBrand = p.Brand != null ? p.Brand.Name : null,
                IsAvailable = p.IsAvailable,
                AverageRating = p.Comments.Any() ? p.Comments.Average(c => c.Rating) : 0,
                ReviewCount = p.Comments.Count()
            });

            return View(result);
        }

        #endregion

        #region Comment
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment(CommentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var comment = new Comment
                {
                    CarId = model.CarId,
                    AccountId = int.Parse(userId),
                    AccessoriesId = model.AccessoriesId,  // Handle AccessoriesId
                    Rating = model.Rating,
                    Comment1 = model.Comment1,
                    CommentDate = DateTime.Now
                };

                db.Comments.Add(comment);
                await db.SaveChangesAsync();

                TempData["success"] = "You have successfully commented.";
                return RedirectToAction("Detail", new { id = model.CarId });
            }

            return View(model);
        }
        #endregion

        #region Detail
        public IActionResult Detail(int id)
        {
            var data = db.Cars
                .Include(p => p.Brand)
                .Include(p => p.Dealer)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Account)
                .SingleOrDefault(p => p.Id == id);

            if (data == null)
            {
                TempData["error"] = "This product was not found";
                return Redirect("/404");
            }

            var relatedCars = db.Cars
                .Where(p => p.BrandId == data.BrandId && p.Id != id)
                .Select(p => new CarVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Condition = p.Condition,
                    FuelType = p.FuelType,
                    Mileage = p.Mileage,
                    Transmission = p.Transmission,
                    Year = p.Year,
                    Price = p.Price,
                    ImageSingle = p.ImageSingle,
                    NameBrand = p.Brand != null ? p.Brand.Name : null,
                    IsAvailable = p.IsAvailable
                }).ToList();

            var result = new DetailCarVM
            {
                Id = data.Id,
                Name = data.Name,
                ImageSingle = data.ImageSingle,
                ImageMultiple = data.ImageMultiple,
                Price = data.Price,
                NameBrand = data.Brand?.Name,
                AddressDealer = data.Dealer?.Address,
                BodyType = data.BodyType,
                Condition = data.Condition,
                FuelType = data.FuelType,
                Mileage = data.Mileage,
                Transmission = data.Transmission,
                Year = data.Year,
                Color = data.Color,
                Doors = data.Doors,
                Cylinders = data.Cylinders,
                EngineSize = data.EngineSize,
                Vin = data.Vin,
                Title = data.Title,
                CarFeatures = data.CarFeatures,
                PriceType = data.PriceType,
                IsAvailable = data.IsAvailable,
                RelatedCars = relatedCars,
                Comments = data.Comments.Select(c => new CommentViewModel
                {
                    CarId = c.CarId,
                    Rating = c.Rating,
                    Comment1 = c.Comment1,
                    CommentDate = c.CommentDate,
                    AccountName = c.Account.Username,
                    AvatarUrl = c.Account?.Image ?? "/images/default-avatar.png"
                }).ToList()
            };

            return View(result);
        }
        #endregion

        #region Search and FindFilter
        public IActionResult Search(string? query)
        {
            var cars = db.Cars
                .Include(p => p.Comments)
                .AsQueryable();

            if (query != null)
            {
                cars = cars.Where(p => p.Name != null && p.Name.Contains(query));
            }

            var result = cars.Select(p => new CarVM
            {
                Id = p.Id,
                Name = p.Name,
                Condition = p.Condition,
                FuelType = p.FuelType,
                Mileage = p.Mileage,
                Transmission = p.Transmission,
                Year = p.Year,
                Price = p.Price,
                ImageSingle = p.ImageSingle,
                IsAvailable = p.IsAvailable,
                AverageRating = p.Comments.Any() ? p.Comments.Average(c => c.Rating) : 0,  // Tính toán AverageRating
                ReviewCount = p.Comments.Count()  // Tính toán ReviewCount
            }).ToList();

            return View(result);
        }


        public IActionResult FindFilter(double max, double min)
        {
            var result = db.Cars.Where(d => d.Price >= min && d.Price <= max).Select(p => new CarVM
            {
                Id = p.Id,
                Name = p.Name,
                Condition = p.Condition,
                FuelType = p.FuelType,
                Mileage = p.Mileage,
                Transmission = p.Transmission,
                Year = p.Year,
                Price = p.Price,
                ImageSingle = p.ImageSingle,
                IsAvailable = p.IsAvailable // Add this line
            }).ToList();
            return View("Index", result);
        }
        #endregion

        #region Compare
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddToCompare(int carId)
        {
            var user = User;
            if (user?.Identity != null && user.Identity.IsAuthenticated)
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    TempData["error"] = "User ID is invalid.";
                    return RedirectToAction("Index", "Car");
                }

                var existingComparison = await db.Compares
                    .Where(c => c.UserId == int.Parse(userId))
                    .ToListAsync();

                if (existingComparison.Count >= 3)
                {
                    TempData["error"] = "You can only compare up to 3 cars. <a href='/Car/Compare'><b style=\"color:blue\">Click My Compare</b></a>";
                    return RedirectToAction("Index", "Car");
                }

                var carAlreadyInCompare = existingComparison.Any(c => c.CarId == carId);
                if (carAlreadyInCompare)
                {
                    TempData["error"] = "Car is already in your compare. <a href='/Car/Compare'><b style=\"color:blue\">Click My Compare</b></a>";
                    return RedirectToAction("Index", "Car");
                }

                var compare = new Compare
                {
                    CarId = carId,
                    UserId = int.Parse(userId),
                    CompareDate = DateTime.Now
                };

                db.Compares.Add(compare);
                await db.SaveChangesAsync();

                TempData["success"] = "Car added to comparison. <a href='/Car/Compare'><b style=\"color:blue\">Click My Compare</b></a>";
                return RedirectToAction("Index", "Car");
            }

            return RedirectToAction("Login", "Secure");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Compare()
        {
            var user = User;
            if (user?.Identity != null && user.Identity.IsAuthenticated)
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    TempData["error"] = "User ID is invalid.";
                    return RedirectToAction("Index", "Car");
                }

                var carsToCompare = await db.Compares
                    .Include(c => c.Car)
                    .ThenInclude(c => c.Comments)
                    .Where(c => c.UserId == int.Parse(userId))
                    .Select(c => new CompareStarViewModel
                    {
                        Id = c.Id,
                        Car = c.Car,
                        AverageRating = c.Car.Comments.Any() ? c.Car.Comments.Average(com => com.Rating) : 0,
                        ReviewCount = c.Car.Comments.Count
                    })
                    .ToListAsync();

                return View(carsToCompare);
            }

            return RedirectToAction("Login", "Secure");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> RemoveFromCompare(int compareId)
        {
            var compare = await db.Compares.FindAsync(compareId);
            if (compare != null)
            {
                db.Compares.Remove(compare);
                await db.SaveChangesAsync();
            }
            TempData["success"] = "Car removed from Compare.";
            return RedirectToAction("Compare");
        }
        #endregion

        #region Wishlist
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddToWishlist(int carId)
        {
            var user = User;
            if (user?.Identity != null && user.Identity.IsAuthenticated)
            {
                var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
                {
                    TempData["error"] = "Invalid user ID.";
                    return RedirectToAction("Index");
                }

                var existingWishlist = await db.Wishlists
                    .Where(w => w.UserId == userId && w.CarId == carId)
                    .FirstOrDefaultAsync();

                if (existingWishlist != null)
                {
                    TempData["error"] = "Car is already in your wishlist. <a href='/Car/Wishlist'><b style=\"color:blue\">Click My Wishlist</b></a>";
                    return RedirectToAction("Index");
                }

                var wishlistItem = new Wishlist
                {
                    UserId = userId,
                    CarId = carId,
                    SelectDate = DateTime.Now
                };

                db.Wishlists.Add(wishlistItem);
                await db.SaveChangesAsync();

                TempData["success"] = "Car added to wishlist. <a href='/Car/Wishlist'><b style=\"color:blue\">Click My Wishlist</b></a>";
                return RedirectToAction("Index");
            }

            TempData["error"] = "You need to be logged in to add items to your wishlist.";
            return RedirectToAction("Login", "Secure");
        }


        [Authorize]
        public async Task<IActionResult> Wishlist()
        {
            var user = User;
            if (user?.Identity != null && user.Identity.IsAuthenticated)
            {
                var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
                {
                    TempData["error"] = "Invalid user ID.";
                    return RedirectToAction("Index");
                }

                var wishlist = await db.Wishlists
                    .Where(w => w.UserId == userId)
                    .Include(w => w.Car)
                    .ThenInclude(c => c.Comments)
                    .Select(w => new WishlistStarViewModel
                    {
                        Id = w.Id,
                        CarId = w.CarId, // Add this line
                        Car = w.Car,
                        AverageRating = w.Car.Comments.Any() ? w.Car.Comments.Average(c => c.Rating) : 0,
                        ReviewCount = w.Car.Comments.Count()
                    })
                    .ToListAsync();

                return View(wishlist);
            }

            TempData["error"] = "You need to be logged in to view your wishlist.";
            return RedirectToAction("Login", "Secure");
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> RemoveFromWishlist(int wishlistId)
        {
            var user = User;
            if (user?.Identity != null && user.Identity.IsAuthenticated)
            {
                var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
                {
                    TempData["error"] = "Invalid user ID.";
                    return RedirectToAction("Wishlist");
                }

                var wishlistItem = await db.Wishlists.FirstOrDefaultAsync(w => w.Id == wishlistId && w.UserId == userId);

                if (wishlistItem != null)
                {
                    db.Wishlists.Remove(wishlistItem);
                    await db.SaveChangesAsync();
                    TempData["success"] = "Car removed from wishlist.";
                }
                else
                {
                    TempData["error"] = "Car not found in your wishlist.";
                }

                return RedirectToAction("Wishlist");
            }

            TempData["error"] = "You need to be logged in to remove items from your wishlist.";
            return RedirectToAction("Login", "Secure");
        }
        #endregion

        #region AdvancedSearch
        public IActionResult AdvancedSearch(string? condition, string? brand, string? transmission, int? year, int? doors, string? bodyType, int? priceRange)
        {
            var cars = db.Cars.Include(c => c.Brand).AsQueryable();

            if (!string.IsNullOrEmpty(condition) && condition != "All")
            {
                cars = cars.Where(p => p.Condition == condition);
            }

            if (!string.IsNullOrEmpty(brand) && brand != "All")
            {
                cars = cars.Where(p => p.Brand != null && p.Brand.Name == brand);
            }

            if (!string.IsNullOrEmpty(transmission) && transmission != "All")
            {
                cars = cars.Where(p => p.Transmission == transmission);
            }

            if (year.HasValue && year.Value != 0)
            {
                cars = cars.Where(p => p.Year == year.Value);
            }

            if (doors.HasValue && doors.Value != 0)
            {
                cars = cars.Where(p => p.Doors == doors.Value);
            }

            if (!string.IsNullOrEmpty(bodyType) && bodyType != "All")
            {
                cars = cars.Where(p => p.BodyType == bodyType);
            }

            // Filter by price range
            if (priceRange.HasValue)
            {
                switch (priceRange.Value)
                {
                    case 2:
                        cars = cars.Where(p => p.Price >= 0 && p.Price <= 100000);
                        break;
                    case 3:
                        cars = cars.Where(p => p.Price > 100000 && p.Price <= 500000);
                        break;
                    case 4:
                        cars = cars.Where(p => p.Price > 500000 && p.Price <= 2000000);
                        break;
                    case 5:
                        cars = cars.Where(p => p.Price > 2000000 && p.Price <= 10000000);
                        break;
                    case 6:
                        cars = cars.Where(p => p.Price > 10000000 && p.Price <= 100000000);
                        break;
                }
            }

            var result = cars.Select(p => new CarVM
            {
                Id = p.Id,
                Name = p.Name,
                Condition = p.Condition,
                FuelType = p.FuelType,
                Mileage = p.Mileage,
                Transmission = p.Transmission,
                Year = p.Year,
                Price = p.Price,
                ImageSingle = p.ImageSingle,
                NameBrand = p.Brand != null ? p.Brand.Name : null,
                IsAvailable = p.IsAvailable
            }).ToList();

            return View("Index", result);
        }
        #endregion

    }
}