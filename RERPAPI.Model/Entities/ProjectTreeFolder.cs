using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectTreeFolder
{
    public int ID { get; set; }

    public string? FolderName { get; set; }

    public int? ParentID { get; set; }

    public int? ProjectTypeID { get; set; }

    public bool? IsDeleted { get; set; }
}
