using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class DocumentType
{
    public int ID { get; set; }

    public string? Name { get; set; }

    public string? Code { get; set; }

    public bool? IsDeleted { get; set; }
}
