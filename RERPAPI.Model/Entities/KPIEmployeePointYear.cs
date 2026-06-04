using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class KPIEmployeePointYear
{
    public int ID { get; set; }

    public int? YearValue { get; set; }

    public int? EmployeeID { get; set; }

    public decimal? PointPercentYear { get; set; }

    public int? IsApproveYear { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
