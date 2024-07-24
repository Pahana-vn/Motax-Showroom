using System.ComponentModel.DataAnnotations;

namespace Motax.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Please enter old password *")]
        public string? OldPassword { get; set; }

        [Required(ErrorMessage = "Please enter new password *")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Please confirm your new password *")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}
