using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ESLTestTableRegistrationLog
{
    /// <summary>
    /// ID Tự tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID của đơn đăng ký master
    /// </summary>
    public int RegistrationID { get; set; }

    /// <summary>
    /// Hành động
    /// </summary>
    public string Action { get; set; } = null!;

    /// <summary>
    /// Người thực hiện hành động
    /// </summary>
    public int ActionBy { get; set; }

    /// <summary>
    /// Ngày thực hiện
    /// </summary>
    public DateTime? ActionDate { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Trạng thái cũ
    /// </summary>
    public int? OldStatus { get; set; }

    /// <summary>
    /// Trạng thái mới
    /// </summary>
    public int? NewStatus { get; set; }

    /// <summary>
    /// API sử lý
    /// </summary>
    public string? APIResponse { get; set; }
}
