using System;
using System.Collections.Generic;

namespace Motax.Models;

public partial class Role
{
    public int Id { get; set; }

    public string Value { get; set; } = null!;

    public string Title { get; set; } = null!;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
