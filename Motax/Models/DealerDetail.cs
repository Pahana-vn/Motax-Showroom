using System;
using System.Collections.Generic;

namespace Motax.Models;

public partial class DealerDetail
{
    public int Id { get; set; }

    public int DealerId { get; set; }

    public string? CoverImage { get; set; }

    public string? AvatarImage { get; set; }

    public string? ConsultantName { get; set; }

    public string? ConsultantAvatar { get; set; }

    public virtual Dealer Dealer { get; set; } = null!;
}
