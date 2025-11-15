using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectType
{
    public int ID { get; set; }

    public string? ProjectTypeCode { get; set; }

    public string? ProjectTypeName { get; set; }

    public int? ParentID { get; set; }

    public string? RootFolder { get; set; }

    public int? ApprovedTBPID { get; set; }

    public bool? IsDeleted { get; set; }
}
