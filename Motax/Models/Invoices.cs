namespace Motax.Models
{
    public class Invoices
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CarRegistrationId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public double TotalAmount { get; set; }
        public string Status { get; set; }

        public virtual Account User { get; set; }
        public virtual CarRegistration CarRegistration { get; set; }
    }
}
