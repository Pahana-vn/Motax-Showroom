﻿using System.ComponentModel.DataAnnotations;

namespace Motax.ViewModels
{
    public class RegisterStaffViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter fullname *")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Please enter email *")]
        public string? Email { get; set; }


        public string? Password { get; set; }


        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please enter phone *")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Please enter address *")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Please enter Date of birth *")]
        public DateTime? Dob { get; set; }

        [Required(ErrorMessage = "Please enter Gender *")]
        public string? Gender { get; set; }

        public IFormFile? Image { get; set; }

        public string? ExistingImage { get; set; }
    }
}
