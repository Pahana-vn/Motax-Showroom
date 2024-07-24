using System;
using System.Collections.Generic;

namespace Motax.Models;

public partial class Dealer
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }
    public string? City { get; set; }

    public string? ImageBackground { get; set; }

    public int Status { get; set; }
    public int? BrandId { get; set; }
    public virtual Brand? Brand { get; set; }

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();

    public virtual ICollection<DealerDetail> DealerDetails { get; set; } = new List<DealerDetail>();

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    public virtual ICollection<ContactMessage> ContactMessages { get; set; } = new List<ContactMessage>();
}
