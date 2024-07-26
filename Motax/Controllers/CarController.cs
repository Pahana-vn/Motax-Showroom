using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Motax.Models;
using Motax.ViewModels;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
        public IActionResult Index(int? brand, int? dealer)
        {
            var cars = db.Cars.AsQueryable();

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
                IsAvailable = p.IsAvailable // Add this line
            });
            return View(result);
        }
        #endregion

        #region Search and FindFilter
        public IActionResult Search(string? query)
        {
            var cars = db.Cars.AsQueryable();

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
                IsAvailable = p.IsAvailable // Add this line
            });
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

        #region Detail
        public IActionResult Detail(int id)
        {
            var data = db.Cars
                .Include(p => p.Brand)
                .Include(p => p.Dealer)
                .SingleOrDefault(p => p.Id == id);

            if (data == null)
            {
                TempData["error"] = "This product was not found";
                return Redirect("/404");
            }
            //them xe có liên quan
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
               IsAvailable = p.IsAvailable // Add this line
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
                IsAvailable = data.IsAvailable, // Add this line
                RelatedCars = relatedCars
            };
            return View(result);
        }
        #endregion

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
                    .Where(c => c.UserId == int.Parse(userId))
                    .ToListAsync();

                return View(carsToCompare);
            }

            return RedirectToAction("Login", "Secure");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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


        public IActionResult AdvancedSearch(string? condition, string? brand, string? transmission, int? year, int? doors, string? price, string? bodyType)
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

            if (!string.IsNullOrEmpty(price))
            {
                var priceRange = price.Split('-');
                if (priceRange.Length == 2 &&
                    double.TryParse(priceRange[0], out double minPrice) &&
                    double.TryParse(priceRange[1], out double maxPrice))
                {
                    cars = cars.Where(p => p.Price >= minPrice && p.Price <= maxPrice);
                }
            }

            if (!string.IsNullOrEmpty(bodyType) && bodyType != "All")
            {
                cars = cars.Where(p => p.BodyType == bodyType);
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
    }
}