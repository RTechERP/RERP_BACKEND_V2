using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class PhasedAllocationPersonDetail
{
    public int ID { get; set; }

    public int? PhasedAllocationPersonID { get; set; }

    public int? EmployeeID { get; set; }

    public DateTime? DateReceive { get; set; }

    /// <summary>
    /// 0: Chưa nhận; 1: Đã nhận
    /// </summary>
    public int? StatusReceive { get; set; }

    public int? Quantity { get; set; }

    public string? UnitName { get; set; }

    public string? ContentReceive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
