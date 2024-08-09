namespace Motax.ViewModels
{
    public class DeliveryUnitViewModel
    {
        public int CarId { get; set; }
        public int DealerId { get; set; }
        public int ServiceUnitId { get; set; }
        public DateTime ScheduledPickupDate { get; set; }
        public string? PickupLocation { get; set; }
        public string? CustomerContact { get; set; }
        public bool IsConfirmed { get; set; }
        public int? CarRegistrationId { get; set; }
    }
}
