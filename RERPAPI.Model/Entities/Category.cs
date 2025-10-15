using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool? IsDeleted { get; set; }
}
