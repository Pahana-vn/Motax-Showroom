namespace Motax.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Message { get; set; }
        public int DealerId { get; set; }
        public virtual Dealer? Dealer { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
