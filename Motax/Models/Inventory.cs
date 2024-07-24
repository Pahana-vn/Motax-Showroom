using System;
using System.Collections.Generic;

namespace Motax.Models;

public partial class Inventory
{
    public int Id { get; set; }

    public int DealerId { get; set; }

    public int CarId { get; set; }

    public int AccessoriesId { get; set; }

    public int StockQuantity { get; set; }

    public string WarehouseLocation { get; set; } = null!;

    public DateTime LastUpdate { get; set; }

    public virtual Accessory Accessories { get; set; } = null!;

    public virtual Car Car { get; set; } = null!;

    public virtual Dealer Dealer { get; set; } = null!;
}
