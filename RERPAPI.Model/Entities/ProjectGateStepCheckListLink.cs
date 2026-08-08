using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng lưu checklist/hồ sơ kiểm tra của từng bước trong dự án
/// </summary>
public partial class ProjectGateStepCheckListLink
{
    /// <summary>
    /// Khóa chính tự tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID liên kết tới bản ghi ProjectGateStepLink
    /// </summary>
    public int? ProjectGateStepLinkID { get; set; }

    /// <summary>
    /// Đường dẫn thư mục lưu hồ sơ, tài liệu hoặc checklist
    /// </summary>
    public string? PathFolder { get; set; }

    /// <summary>
    /// Trạng thái kiểm tra: 0 = Chưa đạt, 1 = Đạt
    /// </summary>
    public bool? IsPass { get; set; }

    public int? ProjectGateStepCheckListID { get; set; }
}
