using System.ComponentModel.DataAnnotations;

namespace Motax.ViewModels
{
    public class AccountDetailStaffVM
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime? Dob { get; set; }
        public string? Gender { get; set; }
        public string? Image { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
    }
}
