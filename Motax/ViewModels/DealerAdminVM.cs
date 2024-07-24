namespace Motax.ViewModels
{
    public class DealerAdminVM
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }
        public string? City { get; set; }

        public IFormFile? ImageBackground { get; set; }

        public int Status { get; set; }

        public string? ExistingImage { get; set; }
        public string? ImageBackgroundPath { get; set; }
        public int MessageCount { get; set; }
    }
}
