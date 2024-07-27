namespace Motax.Models
{
    public class OrderDetailAccessories
    {
        public int Id { get; set; }
        public int? OrderAccessoriesId { get; set; }
        public int? AccessoriesId { get; set; }
        public int? Quantity { get; set; }
        public double? Price { get; set; }
        public double? Discount { get; set; }

        public virtual OrderAccessories? OrderAccessories { get; set; }
        public virtual Accessories? Accessory { get; set; }
    }
}
