using System;
using System.Collections.Generic;

namespace Motax.Models;

public partial class Car
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? BodyType { get; set; }

    public string? Condition { get; set; }

    public string? Mileage { get; set; }

    public string? Transmission { get; set; }

    public int? Year { get; set; }

    public string? FuelType { get; set; }

    public string? ImageSingle { get; set; }

    public string? ImageMultiple { get; set; }

    public string? Color { get; set; }

    public int? Doors { get; set; }

    public int? Cylinders { get; set; }

    public string? EngineSize { get; set; }

    public string? Vin { get; set; }

    public string? CarFeatures { get; set; }

    public string? Title { get; set; }

    public int? BrandId { get; set; }

    public int? CategoryId { get; set; }

    public int? DealerId { get; set; }

    public double? Price { get; set; }

    public string? PriceType { get; set; }

    public string? DriverAirbag { get; set; }

    public int? Status { get; set; }

    public DateOnly? CreateDay { get; set; }

    public DateOnly? UpdateDay { get; set; }

    public bool IsAvailable { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Compare> Compares { get; set; } = new List<Compare>();

    public virtual Dealer? Dealer { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
