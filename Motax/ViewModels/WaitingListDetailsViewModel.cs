namespace Motax.ViewModels
{
    public class WaitingListDetailsViewModel
    {
        public string? Username { get; set; }
        public string? CarName { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string? StatusDescription { get; set; }
    }
}
