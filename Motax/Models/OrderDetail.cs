using System;
using System.Collections.Generic;

namespace Motax.Models;

public partial class OrderDetail
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int CarId { get; set; }

    public int AccessoriesId { get; set; }

    public int Quantity { get; set; }

    public double Price { get; set; }

    public double Discount { get; set; }

    public virtual Accessories Accessories { get; set; } = null!;

    public virtual Car Car { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
