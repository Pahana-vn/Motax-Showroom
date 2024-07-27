namespace Motax.ViewModels
{
    public class CartItem
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ImageSingle { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }
        public double? Total => Price * Quantity;
    }
}
