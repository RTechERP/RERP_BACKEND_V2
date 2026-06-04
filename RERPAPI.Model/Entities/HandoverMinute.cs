using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class HandoverMinute
{
    public int ID { get; set; }

    public string? Code { get; set; }

    public DateTime? DateMinutes { get; set; }

    public int? CustomerID { get; set; }

    public string? CustomerAddress { get; set; }

    public string? CustomerContact { get; set; }

    public string? CustomerPhone { get; set; }

    /// <summary>
    /// thủ kho (Lấy từ ID Employee)
    /// </summary>
    public int AdminWarehouseID { get; set; }

    public int? EmployeeID { get; set; }

    /// <summary>
    /// Người nhận
    /// </summary>
    public string? Receiver { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? ReceiverPhone { get; set; }
}
