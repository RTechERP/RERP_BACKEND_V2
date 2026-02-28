using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class ProductWorkingAudit
{
    public int ID { get; set; }

    public int? ProductWorkingID { get; set; }

    public int? ProductID { get; set; }

    public int? ProductStepID { get; set; }

    public int? WorkingID { get; set; }

    public string? WorkingName { get; set; }

    /// <summary>
    /// Kiểu kiểm tra 0:check mark, 1: Điền giá trị
    /// </summary>
    public int? ValueType { get; set; }

    public string? ValueTypeName { get; set; }

    /// <summary>
    /// Giá trị tiêu chuẩn theo khoảng lấy lên từ min, max value
    /// </summary>
    public string? PeriodValue { get; set; }

    /// <summary>
    /// Giá trị tiêu chuẩn nhỏ nhất
    /// </summary>
    public decimal? MinValue { get; set; }

    /// <summary>
    /// Giá trị tiêu chuẩn lớn nhất
    /// </summary>
    public decimal? MaxValue { get; set; }

    /// <summary>
    /// Đơn vị tính
    /// </summary>
    public string? Unit { get; set; }

    public int? SortOrder { get; set; }

    /// <summary>
    /// 1: Kiểm tra theo kiểu dữ liệu số, 2: kiểm tra theo kiểu dữ liệu ký tự
    /// </summary>
    public int? CheckValueType { get; set; }

    public string? ProductStepCode { get; set; }

    public decimal? MinValueNew { get; set; }

    /// <summary>
    /// Giá trị tiêu chuẩn lớn nhất
    /// </summary>
    public decimal? MaxValueNew { get; set; }

    /// <summary>
    /// Giá trị tiêu chuẩn theo khoảng lấy lên từ min, max value
    /// </summary>
    public string? PeriodValueNew { get; set; }

    public string? ProductStepCodeNew { get; set; }

    public string? WorkingNameNew { get; set; }

    public int? SortOrderNew { get; set; }

    /// <summary>
    /// Kiểu kiểm tra 0:check mark, 1: Điền giá trị
    /// </summary>
    public int? ValueTypeNew { get; set; }

    /// <summary>
    /// 1: Kiểm tra theo kiểu dữ liệu số, 2: kiểm tra theo kiểu dữ liệu ký tự
    /// </summary>
    public int? CheckValueTypeNew { get; set; }

    public int? ActionType { get; set; }

    public string? ReasonChange { get; set; }

    public bool? IsApproved { get; set; }

    public string? UserApproved { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
