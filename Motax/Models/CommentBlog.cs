using System;
using System.Collections.Generic;

namespace Motax.Models;

public partial class CommentBlog
{
    public int Id { get; set; }

    public int BlogId { get; set; }

    public int UserId { get; set; }

    public string CommentBlogs { get; set; } = null!;

    public int Rating { get; set; }

    public DateTime CommentDate { get; set; }

    public virtual Blog Blog { get; set; } = null!;

    public virtual Account User { get; set; } = null!;
}
