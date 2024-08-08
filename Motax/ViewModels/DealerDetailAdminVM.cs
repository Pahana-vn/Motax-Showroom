using System.ComponentModel.DataAnnotations;

namespace Motax.ViewModels
{
    public class DealerDetailAdminVM
    {
        public int Id { get; set; }

        public int DealerId { get; set; }

        public IFormFile? CoverImage { get; set; }

        public IFormFile? AvatarImage { get; set; }

        [Required(ErrorMessage = "Please enter ConsultantName *")]
        public string? ConsultantName { get; set; }

        public IFormFile? ConsultantAvatar { get; set; }

        public string? ExistingCoverImage { get; set; }
        public string? ExistingAvatarImage { get; set; }
        public string? ExistingConsultantAvatar { get; set; }
    }
}
