using System;
using System.Collections.Generic;

namespace Motax.Models;

public partial class Brand
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? ContactPerson { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Description { get; set; }

    public string? Slug { get; set; }

    public string? Image { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Accessory> Accessories { get; set; } = new List<Accessory>();

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}
