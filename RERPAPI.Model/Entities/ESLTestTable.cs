using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ESLTestTable
{
    /// <summary>
    /// ID tự tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Tên bàn test
    /// </summary>
    public string TestTableName { get; set; } = null!;

    /// <summary>
    /// Bar code của E-INK
    /// </summary>
    public string Barcode { get; set; } = null!;

    /// <summary>
    /// Mặt bàn
    /// </summary>
    public int TableSide { get; set; }

    /// <summary>
    /// Số lượng mặt bàn
    /// </summary>
    public int? NumberOfSides { get; set; }

    /// <summary>
    /// Mô tả
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Trạng thái hoạt động của bàn
    /// </summary>
    public bool? IsActive { get; set; }

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
    /// Trạng thái xóa
    /// </summary>
    public bool? IsDeleted { get; set; }

    /// <summary>
    /// Trạng thái online
    /// </summary>
    public bool? online { get; set; }

    /// <summary>
    /// Dung lượng Pin
    /// </summary>
    public int? esl_battery { get; set; }
}
