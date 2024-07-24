namespace Motax.ViewModels
{
    public class BrandAdminVM
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? ContactPerson { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? Description { get; set; }
        public string? Slug { get; set; }

        public IFormFile? Image { get; set; }

        public int? Status { get; set; }
        public string? ExistingImage { get; set; }
    }
}
