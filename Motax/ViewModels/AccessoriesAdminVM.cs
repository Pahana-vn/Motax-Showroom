using System.ComponentModel.DataAnnotations;

namespace Motax.ViewModels
{
    public class AccessoriesAdminVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter Name *")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Please enter Price *")]
        public double? Price { get; set; }
        [Required(ErrorMessage = "Please enter Description *")]
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
