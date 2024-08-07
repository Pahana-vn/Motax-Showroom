using Motax.Models;

namespace Motax.ViewModels
{
    public class WishlistStarViewModel
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public Car Car { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}
