namespace Motax.ViewModels
{
    public class DealerVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? ImageBackground { get; set; }
        public int Quantity { get; set; }
    }

    public class DealerDetailVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<DealerDetailItemVM>? DealerDetails { get; set; }
        public int Quantity { get; set; }
        public List<CarVM>? Cars { get; set; }
    }

    public class DealerDetailItemVM
    {
        public int Id { get; set; }
        public string? CoverImage { get; set; }
        public string? AvatarImage { get; set; }
        public string? ConsultantName { get; set; }
        public string? ConsultantAvatar { get; set; }
    }


}
