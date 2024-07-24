namespace Motax.ViewModels
{
    public class CarAdminVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? BodyType { get; set; }
        public string? Condition { get; set; }
        public string? Mileage { get; set; }
        public string? Transmission { get; set; }
        public int? Year { get; set; }
        public string? FuelType { get; set; }
        public IFormFile? ImageSingle { get; set; }
        public List<IFormFile>? ImageMultiple { get; set; }
        public string? Color { get; set; }
        public int? Doors { get; set; }
        public int? Cylinders { get; set; }
        public string? EngineSize { get; set; }
        public string? Vin { get; set; }
        public string? CarFeatures { get; set; }
        public string? Title { get; set; }
        public int? BrandId { get; set; }
        public int? DealerId { get; set; }
        public double? Price { get; set; }
        public string? PriceType { get; set; }
        public string? DriverAirbag { get; set; }
        public int? Status { get; set; }

        public string? ExistingImageSingle { get; set; }
        public List<string>? ExistingImageMultiple { get; set; }




    }
}
