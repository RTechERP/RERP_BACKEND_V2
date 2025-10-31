using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class HandoverFinance
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? HandoverID { get; set; }

    public int? EmployeeID { get; set; }

    public string? DebtType { get; set; }

    public int? DebtAmount { get; set; }

    public int? AccountantID { get; set; }

    public bool? IsApprove { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
