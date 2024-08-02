namespace Motax.ViewModels
{
    public class InvoiceViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CarRegistrationId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public double TotalAmount { get; set; }
        public string Status { get; set; }

        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
    }
}
