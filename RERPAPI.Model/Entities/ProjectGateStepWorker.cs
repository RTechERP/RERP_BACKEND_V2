using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu danh sách nhân viên tham gia thực hiện từng bước của dự án
/// </summary>
public partial class ProjectGateStepWorker
{
    /// <summary>
    /// Khóa chính tự tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID liên kết đến ProjectGateStepLink
    /// </summary>
    public int? ProjectGateStepLinkID { get; set; }

    /// <summary>
    /// ID nhân viên được chỉ định thực hiện công việc
    /// </summary>
    public int? EmployeeID { get; set; }

    /// <summary>
    /// Số ngày làm việc của nhân viên
    /// </summary>
    public decimal? DayCount { get; set; }

    /// <summary>
    /// Đơn giá công của nhân viên
    /// </summary>
    public decimal? UnitPrice { get; set; }

    /// <summary>
    /// Thành tiền (thường = DayCount × UnitPrice)
    /// </summary>
    public decimal? TotalAmount { get; set; }
}
