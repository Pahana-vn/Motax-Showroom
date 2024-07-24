namespace Motax.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Image { get; set; }
        public DateTime? Dob { get; set; }
        public string? Gender { get; set; }
        public int? Status { get; set; }
        public int? RoleId { get; set; }
        public string? ExternalId { get; set; }

        public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();
        public virtual ICollection<CommentBlog> CommentBlogs { get; set; } = new List<CommentBlog>();
        public virtual ICollection<Compare> Compares { get; set; } = new List<Compare>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    }
}
