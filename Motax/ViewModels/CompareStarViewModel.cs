using Motax.Models;

namespace Motax.ViewModels
{
    public class CompareStarViewModel
    {
        public int Id { get; set; }
        public Car Car { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}
