using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Motax.Models
{
    public class GoodsReceipt
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public int DealerId { get; set; }

        [Required]
        public string? VIN { get; set; }

        [Required]
        public DateTime ReceiptDate { get; set; }

        [Required]
        public string? ReceiptDetails { get; set; }
        public virtual Car Car { get; set; }
        public virtual Dealer Dealer { get; set; }
    }
}
