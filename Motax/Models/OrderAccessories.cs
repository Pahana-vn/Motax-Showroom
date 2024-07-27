namespace Motax.Models
{
    public class OrderAccessories
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public string? TypeCode { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? Username { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? HowToPay { get; set; }
        public string? HowToTransport { get; set; }
        public int? Status { get; set; }
        public string? Note { get; set; }

        public virtual Account? Account { get; set; }
    }
}
