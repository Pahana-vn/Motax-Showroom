namespace Motax.Models
{
    public class ServiceUnit
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public int DealerId { get; set; }
        public DateTime ServiceDate { get; set; }
        public string? ServiceDetails { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? PickupDate { get; set; }
        public int? CarRegistrationId { get; set; }
        public virtual Car? Car { get; set; }
        public virtual Dealer? Dealer { get; set; }
        public virtual CarRegistration? CarRegistration { get; set; }
    }
}
