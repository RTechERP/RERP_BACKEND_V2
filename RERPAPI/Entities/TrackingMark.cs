using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class TrackingMark
{
    public int ID { get; set; }

    public DateTime? RegisterDate { get; set; }

    public int? EmployeeID { get; set; }

    public int? DocumentTypeID { get; set; }

    public string? DocumentName { get; set; }

    public int? DocumentQuantity { get; set; }

    public int? DocumentTotalPage { get; set; }

    public int? ApprovedID { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public int? EmployeeSignID { get; set; }

    public DateTime? SignDatedActual { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int? Status { get; set; }

    public string? ReasonCancel { get; set; }

    public bool? IsUrgent { get; set; }

    public DateTime? Deadline { get; set; }

    public DateTime? ExpectDateComplete { get; set; }
}
