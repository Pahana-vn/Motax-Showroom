using System;
using System.Collections.Generic;

namespace Motax.Models;

public partial class BlogDetail
{
    public int Id { get; set; }

    public int BlogId { get; set; }

    public string ContentBlogs { get; set; } = null!;

    public string ContentBlogsUnit { get; set; } = null!;

    public virtual Blog Blog { get; set; } = null!;
}
