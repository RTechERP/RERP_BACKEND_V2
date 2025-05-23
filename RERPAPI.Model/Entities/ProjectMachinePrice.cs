using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectMachinePrice
{
    public int ID { get; set; }

    public int? ProjectID { get; set; }

    public int? EmployeeID { get; set; }

    /// <summary>
    /// Ngày thanh toán
    /// </summary>
    public DateTime? DatePrice { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Ngày update
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Người tạo
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Người update
    /// </summary>
    public string? UpdatedBy { get; set; }

    public bool? IsDelete { get; set; }
}
