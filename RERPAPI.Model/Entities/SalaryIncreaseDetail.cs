using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class SalaryIncreaseDetail
{
    public int ID { get; set; }

    public int? EmployeeID { get; set; }

    public string? EmailTBP { get; set; }

    public decimal? PreviousBaseSalary { get; set; }

    public decimal? CurrentBaseSalary { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public int? SalaryIncreaseID { get; set; }

    public bool? IsSend { get; set; }
}
