using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Bảng khai báo danh sách loại phạt
/// </summary>
public partial class EmployeeDeductionType
{
    /// <summary>
    /// ID bản ghi
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Mã loại phạt
    /// </summary>
    public string? DeductionTypeCode { get; set; }

    /// <summary>
    /// Tên loại phạt
    /// </summary>
    public string? DeductionTypeName { get; set; }

    /// <summary>
    /// Tiền phạt cấp 1
    /// </summary>
    public decimal? MoneyLevel1 { get; set; }

    /// <summary>
    /// Tiền phạt cấp 2
    /// </summary>
    public decimal? MoneyLevel2 { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Người tạo
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Ngày cập nhật
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Người cập nhật
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Trạng thái xóa mềm: 0-Chưa xóa, 1-Đã xóa
    /// </summary>
    public bool? IsDeleted { get; set; }
}
