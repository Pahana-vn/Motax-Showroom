namespace Motax.ViewModels
{
    public class ServiceUnitViewModel
    {
        public int CarId { get; set; }
        public int DealerId { get; set; }
        public int CarRegistrationId { get; set; }
        public DateTime ServiceDate { get; set; }
        public string? ServiceDetails { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? PickupDate { get; set; }
        public string? DealerName { get; set; }
        public string? DealerAddress { get; set; }
        public string? DealerPhone { get; set; }
    }
}
