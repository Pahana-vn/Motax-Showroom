namespace Motax.Models
{
    public class DeliveryUnit
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public int DealerId { get; set; }
        public int ServiceUnitId { get; set; }
        public DateTime ScheduledPickupDate { get; set; }
        public string? PickupLocation { get; set; }
        public string? CustomerContact { get; set; }
        public bool IsConfirmed { get; set; }
        public int? CarRegistrationId { get; set; }
        public virtual Car? Car { get; set; }
        public virtual Dealer? Dealer { get; set; }
        public virtual ServiceUnit? ServiceUnit { get; set; }
        public virtual CarRegistration? CarRegistration { get; set; }
    }
}
