using Motax.Models;

namespace Motax.ViewModels
{
    public class CarRegistrationDetailVM
    {
        public int CarId { get; set; }
        public int UserId { get; set; }
        public DateTime RegistrationDate { get; set; }
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
        public string? Status { get; set; }
        public Car? Car { get; set; }
        public Account? User { get; set; }
        public Dealer? Dealer { get; set; }
        public string? OrderCode { get; set; }
        public DateTime? OrderCreateDate { get; set; }
        public DateTime? OrderExpiryDate { get; set; }
        public double? OrderTotalAmount { get; set; }
    }
}

