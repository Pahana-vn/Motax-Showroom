using System;
using System.Collections.Generic;

namespace Motax.Models;

public partial class Discount
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public double DiscountPercentage { get; set; }

    public DateOnly CreateDay { get; set; }

    public DateOnly UpdateDay { get; set; }
}
