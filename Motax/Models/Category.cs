using System;
using System.Collections.Generic;

namespace Motax.Models;

public partial class Category
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }

    public string? Slug { get; set; }

    public int? Status { get; set; }

    public DateOnly StarDay { get; set; }

    public DateOnly UpdateDay { get; set; }

    public virtual ICollection<Accessories> Accessories { get; set; } = new List<Accessories>();

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}
