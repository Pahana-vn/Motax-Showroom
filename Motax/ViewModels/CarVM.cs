namespace Motax.ViewModels
{
    public class CarVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Condition { get; set; }
        public string? Mileage { get; set; }
        public string? ImageSingle { get; set; }
        public string? Transmission { get; set; }
        public int? Year { get; set; }
        public string? FuelType { get; set; }
        public double? Price { get; set; }
        public string? NameBrand { get; set; }
        public bool IsAvailable { get; set; }

    }
    public class DetailCarVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ImageSingle { get; set; }
        public string? ImageMultiple { get; set; }
        public double? Price { get; set; }
        public string? NameBrand { get; set; }
        public string? AddressDealer { get; set; }
        public string? BodyType { get; set; }
        public string? Condition { get; set; }
        public string? Mileage { get; set; }
        public string? Transmission { get; set; }
        public int? Year { get; set; }
        public string? FuelType { get; set; }
        public string? Color { get; set; }
        public int? Doors { get; set; }
        public int? Cylinders { get; set; }
        public string? EngineSize { get; set; }
        public string? Vin { get; set; }
        public string? CarFeatures { get; set; }
        public string? Title { get; set; }
        public string? PriceType { get; set; }
        public bool IsAvailable { get; set; }
        public List<CarVM>? RelatedCars { get; set; }
    }

}
