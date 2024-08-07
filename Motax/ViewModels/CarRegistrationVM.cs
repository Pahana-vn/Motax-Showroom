using Motax.Models;
using System.ComponentModel.DataAnnotations;

namespace Motax.ViewModels
{
    public class CarRegistrationVM
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public int UserId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public double CarPrice { get; set; }
        public double? RegistrationFee { get; set; }
        public double? TaxAmount { get; set; }
        public double? TotalAmount { get; set; }
        public IEnumerable<OrderStatus>? OrderStatusList { get; set; }
        [Required(ErrorMessage = "Please enter CustomerName *")]
        public string? CustomerName { get; set; }
        [Required(ErrorMessage = "Please enter CustomerAddress *")]
        public string? CustomerAddress { get; set; }
        [Required(ErrorMessage = "Please enter CustomerPhone *")]
        public string? CustomerPhone { get; set; }
        [Required(ErrorMessage = "Please enter CustomerEmail *")]
        public string? CustomerEmail { get; set; }
        [Required(ErrorMessage = "Please enter LicensePlate *")]
        public string? LicensePlate { get; set; }
        [Required(ErrorMessage = "Please enter RegistrationNumber *")]
        public string? RegistrationNumber { get; set; }
        [Required(ErrorMessage = "Please enter PaymentStatus *")]
        public string? PaymentStatus { get; set; }
        [Required(ErrorMessage = "Please enter InsuranceDetails *")]
        public string? InsuranceDetails { get; set; }

        [Required(ErrorMessage = "Please enter InspectionDate *")]
        public DateTime? InspectionDate { get; set; }
        [Required(ErrorMessage = "Please enter Notes *")]
        public string? Notes { get; set; }
        [Required(ErrorMessage = "Please enter OrderStatusId *")]
        public int? OrderStatusId { get; set; }
        [Required(ErrorMessage = "Please enter DriverLicenseNumber *")]
        public string? DriverLicenseNumber { get; set; }
    }
}
