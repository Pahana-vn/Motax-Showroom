using System.ComponentModel.DataAnnotations;

namespace Motax.ViewModels
{
    public class CarAdminVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter Name *")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Please enter BodyType *")]
        public string? BodyType { get; set; }
        [Required(ErrorMessage = "Please enter Condition *")]
        public string? Condition { get; set; }
        [Required(ErrorMessage = "Please enter Mileage *")]
        public string? Mileage { get; set; }
        [Required(ErrorMessage = "Please enter Transmission *")]
        public string? Transmission { get; set; }
        [Required(ErrorMessage = "Please enter Year *")]
        public int? Year { get; set; }
        [Required(ErrorMessage = "Please enter FuelType *")]
        public string? FuelType { get; set; }
        public IFormFile? ImageSingle { get; set; }
        public List<IFormFile>? ImageMultiple { get; set; }
        [Required(ErrorMessage = "Please enter Color *")]
        public string? Color { get; set; }
        [Required(ErrorMessage = "Please enter Doors *")]
        public int? Doors { get; set; }
        [Required(ErrorMessage = "Please enter Cylinders *")]
        public int? Cylinders { get; set; }
        [Required(ErrorMessage = "Please enter EngineSize *")]
        public string? EngineSize { get; set; }
        public string? Vin { get; set; }
        [Required(ErrorMessage = "Please enter CarFeatures *")]
        public string? CarFeatures { get; set; }
        [Required(ErrorMessage = "Please enter Title *")]
        public string? Title { get; set; }
        public int? BrandId { get; set; }
        public int? DealerId { get; set; }
        [Required(ErrorMessage = "Please enter Price *")]
        public double? Price { get; set; }
        [Required(ErrorMessage = "Please enter PriceType *")]
        public string? PriceType { get; set; }
        [Required(ErrorMessage = "Please enter DriverAirbag *")]
        public string? DriverAirbag { get; set; }
        public int? Status { get; set; }

        public string? ExistingImageSingle { get; set; }
        public List<string>? ExistingImageMultiple { get; set; }




    }
}
