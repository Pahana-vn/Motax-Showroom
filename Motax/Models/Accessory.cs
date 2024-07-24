using System;
using System.Collections.Generic;

namespace Motax.Models;

public partial class Accessory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public double DescriptionUnitPrice { get; set; }

    public int BrandId { get; set; }

    public int CategoryId { get; set; }

    public double Discount { get; set; }

    public int StockQuantity { get; set; }

    public string? ImageSingle { get; set; }

    public string? ImageMultiple { get; set; }

    public string? TypeCode { get; set; }

    public int Status { get; set; }

    public DateTime CreateDay { get; set; }

    public DateTime UpdateDay { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
