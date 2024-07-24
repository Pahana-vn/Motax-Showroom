using Motax.Models;
using System.ComponentModel.DataAnnotations;

namespace Motax.ViewModels
{
    public class CarRegistrationVM
    {
        public int CarId { get; set; }
        public int UserId { get; set; }
        public DateTime RegistrationDate { get; set; }

        [Required]
        public string? CustomerName { get; set; }

        [Required]
        public string? CustomerAddress { get; set; }

        [Required]
        [Phone]
        public string? CustomerPhone { get; set; }

        [Required]
        [EmailAddress]
        public string? CustomerEmail { get; set; }

        [Required]
        public string? LicensePlate { get; set; }

        [Required]
        public string? RegistrationNumber { get; set; }

        [Required]
        public string? PaymentStatus { get; set; }

        public string? InsuranceDetails { get; set; }
        public string? DriverLicenseNumber { get; set; }

        public DateTime? InspectionDate { get; set; }

        public string? Notes { get; set; }

        public double? CarPrice { get; set; } // Giá trị xe
        public double? RegistrationFee { get; set; } // Phí đăng ký là 2%
        public double? TaxAmount { get; set; } // Thuế là 10%
        public double? TotalAmount { get; set; } // Tổng giá trị
        public int? OrderStatusId { get; set; }
        public List<OrderStatus> OrderStatusList { get; set; } = new List<OrderStatus>(); // Danh sách trạng thái đơn hàng
    }
}
