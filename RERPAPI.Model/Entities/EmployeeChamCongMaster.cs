using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class EmployeeChamCongMaster
{
    public int ID { get; set; }

    public string Name { get; set; } = null!;

    /// <summary>
    /// Kiểu thời gian (0,theo buồi) (1,theo ngày)
    /// </summary>
    public int? TimeType { get; set; }

    public int? _Month { get; set; }

    public int? _Year { get; set; }

    public string? Note { get; set; }

    public bool? isApproved { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
