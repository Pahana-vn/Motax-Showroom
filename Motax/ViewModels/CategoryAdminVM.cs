namespace Motax.ViewModels
{
    public class CategoryAdminVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Slug { get; set; }

        public string? Description { get; set; }

        public int? Status { get; set; }

        public IFormFile? Image { get; set; }
        public string? ExistingImage { get; set; }
    }
}
