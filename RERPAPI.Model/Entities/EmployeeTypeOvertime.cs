using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeeTypeOvertime
{
    public int ID { get; set; }

    public string? TypeCode { get; set; }

    public string? Type { get; set; }

    public decimal? Ratio { get; set; }

    public decimal? Cost { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public string? Note { get; set; }
    public bool? IsDeleted { get; set; }

}
