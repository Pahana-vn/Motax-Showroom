namespace Motax.ViewModels
{
    public class AccessoriesVM
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public double? Price { get; set; }
        public string? ImageSingle { get; set; }
    }

    public class DetailAccessoriesVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? TypeCode { get; set; }
        public string? Description { get; set; }
        public string? ImageSingle { get; set; }
        public string? ImageMultiple { get; set; }
        public string? NameCategory { get; set; }
        public double? Price { get; set; }

        public List<AccessoriesVM>? RelatedAccs { get; set; }
    }
}
