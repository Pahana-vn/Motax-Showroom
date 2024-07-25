namespace Motax.ViewModels
{
    public class AccessoriesAdminVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double? Price { get; set; }
        public string? Description { get; set; }
        public string? TypeCode { get; set; }
        public string? Slug { get; set; }
        public IFormFile? ImageSingle { get; set; }
        public List<IFormFile>? ImageMultiple { get; set; }
        public int? Status { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public string? ExistingImageSingle { get; set; }
        public List<string>? ExistingImageMultiple { get; set; }
    }
}
