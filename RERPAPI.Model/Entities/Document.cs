using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class Document
{
    public int ID { get; set; }

    public string? Code { get; set; }

    public int? STT { get; set; }

    public string? NameDocument { get; set; }

    public DateTime? DatePromulgate { get; set; }

    public DateTime? DateEffective { get; set; }

    public int? DocumentTypeID { get; set; }

    public int? DepartmentID { get; set; }

    public int? GroupType { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? SignedEmployeeID { get; set; }

    public string? AffectedScope { get; set; }

    public bool? IsOnWeb { get; set; }

    public bool? IsPromulgated { get; set; }
}
