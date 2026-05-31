using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng liên kết nhân viên và cấu hình nhận email
/// </summary>
public partial class ConfigNotificationKeyLink
{
    /// <summary>
    /// ID bản ghi
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID nhân viên
    /// </summary>
    public int? EmployeeID { get; set; }

    /// <summary>
    /// ID cấu hình nhận email
    /// </summary>
    public int? ConfigNotificationKeyID { get; set; }

    /// <summary>
    /// Trạng thái gửi mail: 0-Không nhận mail, 1-Nhận mail
    /// </summary>
    public bool? IsActive { get; set; }
}
