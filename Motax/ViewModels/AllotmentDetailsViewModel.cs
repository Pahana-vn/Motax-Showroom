namespace Motax.ViewModels
{
    public class AllotmentDetailsViewModel
    {
        public string? CarName { get; set; }
        public string? DealerName { get; set; }
        public string? DealerAddress { get; set; }
        public string? DealerPhone { get; set; }
        public DateTime ScheduledPickupDate { get; set; }
        public string? PickupLocation { get; set; }
        public string? CustomerContact { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
