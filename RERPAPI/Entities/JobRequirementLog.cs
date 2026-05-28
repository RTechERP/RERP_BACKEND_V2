using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

/// <summary>
/// Lưu lịch sử log các thao tác liên quan đến đơn đặt đồ ăn của nhân viên
/// </summary>
public partial class JobRequirementLog
{
    /// <summary>
    /// ID tự tăng
    /// </summary>
    public int ID { get; set; }

    public int? JobRequirementID { get; set; }

    public int? EmployeeID { get; set; }

    public DateTime? DateLog { get; set; }

    public string? LogContent { get; set; }

    /// <summary>
    /// Người tạo
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người cập nhật
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Ngày cập nhật
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    public string? TypeLog { get; set; }
}
