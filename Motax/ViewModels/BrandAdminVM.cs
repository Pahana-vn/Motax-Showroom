using System.ComponentModel.DataAnnotations;

namespace Motax.ViewModels
{
    public class BrandAdminVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter Name *")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Please enter ContactPerson *")]
        public string? ContactPerson { get; set; }

        [Required(ErrorMessage = "Please enter Email *")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter Phone *")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Please enter Address *")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Please enter Description *")]
        public string? Description { get; set; }
        public string? Slug { get; set; }

        public IFormFile? Image { get; set; }

        public int? Status { get; set; }
        public string? ExistingImage { get; set; }
    }
}
