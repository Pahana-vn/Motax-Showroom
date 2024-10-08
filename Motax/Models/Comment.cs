﻿using System;
using System.Collections.Generic;

namespace Motax.Models;

public partial class Comment
{
    public int Id { get; set; }
    public int? CarId { get; set; }
    public int AccountId { get; set; }
    public int? AccessoriesId { get; set; }
    public int Rating { get; set; }
    public string? Comment1 { get; set; }
    public DateTime CommentDate { get; set; }
    public virtual Car? Car { get; set; }
    public virtual Accessories? Accessories { get; set; }
    public virtual Account? Account { get; set; }

}
