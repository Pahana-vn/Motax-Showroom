using System;
using System.Collections.Generic;

namespace Motax.Models;

public partial class Blog
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string ImageSingle { get; set; } = null!;

    public string Summary { get; set; } = null!;

    public int AuthorId { get; set; }

    public DateOnly CreateDay { get; set; }

    public DateOnly UpdateDay { get; set; }

    public virtual Account Author { get; set; } = null!;

    public virtual ICollection<BlogDetail> BlogDetails { get; set; } = new List<BlogDetail>();

    public virtual ICollection<CommentBlog> CommentBlogs { get; set; } = new List<CommentBlog>();
}
