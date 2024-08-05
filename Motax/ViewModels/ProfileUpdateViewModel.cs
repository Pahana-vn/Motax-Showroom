using Motax.Models;
using System.ComponentModel.DataAnnotations;

namespace Motax.ViewModels
{
    public class ProfileUpdateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter user name *")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Please enter email *")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter phone *")]
        [RegularExpression(@"0[9875]\d{8}", ErrorMessage = "Incorrect Vietnamese phone number format")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Please enter address *")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Please enter Date of birth *")]
        public DateTime? Dob { get; set; }

        [Required(ErrorMessage = "Please enter Gender *")]
        public string? Gender { get; set; }

        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }

        public IFormFile? Image { get; set; }
        public string? ExistingImage { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
