using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeeLuckyNumber
{
    public int ID { get; set; }

    public int? EmployeeID { get; set; }

    public string? EmployeeCode { get; set; }

    public string? EmployeeName { get; set; }

    public string? PhoneNumber { get; set; }

    public int? YearValue { get; set; }

    public int? LuckyNumber { get; set; }

    public bool? IsChampion { get; set; }

    public string? ImageName { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }
}
