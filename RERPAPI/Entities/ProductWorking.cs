using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class ProductWorking
{
    public int ID { get; set; }

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

    public bool? IsGetAutoValueComport { get; set; }

    public int? Comport { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsGetAutoValueIP { get; set; }

    public string? Port { get; set; }

    public string? IpAddress { get; set; }

    /// <summary>
    /// 1: Kiểm tra theo kiểu dữ liệu số, 2: kiểm tra theo kiểu dữ liệu ký tự
    /// </summary>
    public int? CheckValueType { get; set; }

    public string? ProductStepCode { get; set; }

    public int? ActionType { get; set; }

    public string? ReasonChange { get; set; }

    public bool? IsApproved { get; set; }
}
