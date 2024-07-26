using System;
using System.Collections.Generic;

namespace Motax.Models;

public partial class Compare
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? CarId { get; set; }

    public DateTime CompareDate { get; set; }

    public virtual Car? Car { get; set; }

    public virtual Account? User { get; set; }
}
