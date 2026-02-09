using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class UpdateVersion
{
    /// <summary>
    /// Khóa chính, tự tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Mã phiên bản / mã cập nhật (vd: VER_2026_02_04)
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Tên bản cập nhật
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Nội dung chi tiết bản cập nhật
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Trạng thái: 1 = Đã public, 2 = Chưa public
    /// </summary>
    public byte? Status { get; set; }

    /// <summary>
    /// Ngày giờ public bản cập nhật
    /// </summary>
    public DateTime? PublicDate { get; set; }

    /// <summary>
    /// Ghi chú thêm
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Ngày tạo bản ghi
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người tạo
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Ngày cập nhật gần nhất
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Người cập nhật
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Xóa mềm: 0 = Chưa xóa, 1 = Đã xóa
    /// </summary>
    public bool? IsDeleted { get; set; }

    public string? FileNameFEOrigin { get; set; }

    public string? FileNameFE { get; set; }

    public string? FileNameBEOrigin { get; set; }

    public string? FileNameBE { get; set; }
}
