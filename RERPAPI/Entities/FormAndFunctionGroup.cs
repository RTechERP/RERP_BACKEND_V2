using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class FormAndFunctionGroup
{
    public int ID { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public int? ParentID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool IsHide { get; set; }
}
