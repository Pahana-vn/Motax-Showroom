namespace Motax.ViewModels
{
    public class GoodsReceiptViewModel
    {
        public int CarId { get; set; }
        public int DealerId { get; set; }
        public string VIN { get; set; } = null!;
        public DateTime ReceiptDate { get; set; }
        public string? ReceiptDetails { get; set; }
    }
}
