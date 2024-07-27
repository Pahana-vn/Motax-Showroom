using System;
using System.Collections.Generic;

namespace Motax.Models;

public partial class Wishlist
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int CarId { get; set; }

    public int AccessoriesId { get; set; }

    public DateTime SelectDate { get; set; }

    public virtual Accessories Accessories { get; set; } = null!;

    public virtual Car Car { get; set; } = null!;

    public virtual Account User { get; set; } = null!;
}
