using System.ComponentModel.DataAnnotations;

namespace Motax.ViewModels
{
    public class DealerAdminVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter Name *")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Please enter Phone *")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Please enter Email *")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter Address *")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Please enter City *")]
        public string? City { get; set; }

        public IFormFile? ImageBackground { get; set; }

        public int Status { get; set; }

        public string? ExistingImage { get; set; }
        public string? ImageBackgroundPath { get; set; }
        public int MessageCount { get; set; }
    }
}
