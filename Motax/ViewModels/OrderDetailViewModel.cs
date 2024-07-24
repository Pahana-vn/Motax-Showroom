using Motax.Models;

namespace Motax.ViewModels
{
    public class OrderDetailViewModel
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? CarName { get; set; }
        public string? DealerName { get; set; } // New
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public double TotalAmount { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public Car? Car { get; set; }
    }
}
