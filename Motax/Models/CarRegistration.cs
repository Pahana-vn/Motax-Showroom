namespace Motax.Models
{
    public class CarRegistration
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public int UserId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string? Status { get; set; }

        public string? CustomerName { get; set; }
        public string? CustomerAddress { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerEmail { get; set; }
        public string? LicensePlate { get; set; }
        public string? RegistrationNumber { get; set; }
        public double? RegistrationFee { get; set; }
        public double? TaxAmount { get; set; }
        public string? PaymentStatus { get; set; }
        public string? InsuranceDetails { get; set; }
        public DateTime? InspectionDate { get; set; }
        public string? DriverLicenseNumber { get; set; }
        public string? Notes { get; set; }
        public double? TotalAmount { get; set; }
        public int? OrderStatusId { get; set; }
        public virtual OrderStatus? OrderStatus { get; set; }
        public virtual Car? Car { get; set; }
        public virtual Account? User { get; set; }
    }
}
