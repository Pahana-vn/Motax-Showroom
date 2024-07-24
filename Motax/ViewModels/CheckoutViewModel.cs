using Motax.Models;

namespace Motax.ViewModels
{
    public class CheckoutViewModel
    {
        public Account? User { get; set; }
        public Car? CarDetails { get; set; }
        public double Price { get; set; }
    }
}
