using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Motax.ViewModels
{
    public class CompleteProfileViewModel
    {
        [Required(ErrorMessage = "Please enter your full name")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Please enter your email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter your address")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Please enter your phone number")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Please enter your date of birth")]
        public DateTime? Dob { get; set; }

        [Required(ErrorMessage = "Please select your gender")]
        public string? Gender { get; set; }

        public string? ExternalId { get; set; }

        public IFormFile? Image { get; set; }
    }
}
