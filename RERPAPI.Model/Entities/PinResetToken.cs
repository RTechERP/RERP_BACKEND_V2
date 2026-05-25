using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class PinResetToken
{
    /// <summary>
    /// Khóa chính, tự tăng
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID người dùng yêu cầu reset PIN
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Mã token dùng để reset PIN
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Thời điểm hết hạn của token
    /// </summary>
    public DateTime? ExpiredAt { get; set; }

    /// <summary>
    /// Token đã được sử dụng hay chưa (0: chưa, 1: rồi)
    /// </summary>
    public bool? IsUsed { get; set; }

    /// <summary>
    /// Ngày tạo bản ghi
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người tạo bản ghi
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Ngày cập nhật gần nhất
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Người cập nhật gần nhất
    /// </summary>
    public string? UpdatedBy { get; set; }
}
