using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeeNighShift
{
    public int ID { get; set; }

    /// <summary>
    /// 0: Chờ duyệt; 1: Duyệt; 2:Không duyệt
    /// </summary>
    public int? IsApprovedTBP { get; set; }

    /// <summary>
    /// 0: Chờ duyệt; 1: Duyệt
    /// </summary>
    public int? IsApprovedHR { get; set; }

    public int? ApprovedTBP { get; set; }

    public int? ApprovedHR { get; set; }

    public int? EmployeeID { get; set; }

    public DateTime? DateRegister { get; set; }

    public DateTime? DateStart { get; set; }

    public DateTime? DateEnd { get; set; }

    public decimal? BreaksTime { get; set; }

    public decimal? TotalHours { get; set; }

    public string? Location { get; set; }

    public string? Note { get; set; }

    public string? ReasonDeciline { get; set; }

    public int? DecilineApprove { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public string? ReasonHREdit { get; set; }

    public bool? IsProblem { get; set; }

    public bool? IsDeleted { get; set; }

    public decimal? WorkTime { get; set; }

    public int? IsSeniorApproved { get; set; }

    public int? ApprovedSeniorID { get; set; }

    public DateTime? DateApprovedSenior { get; set; }
}
