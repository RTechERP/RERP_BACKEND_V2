using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Khoa
/// </summary>
public partial class TSAssetManagement
{
    public int ID { get; set; }

    public bool? IsAllocation { get; set; }

    public int? StatusID { get; set; }

    public int? DepartmentID { get; set; }

    /// <summary>
    /// Trưởng phòng quản lý tài sản
    /// </summary>
    public int? EmployeeID { get; set; }

    public int? TSAssetID { get; set; }

    public string? TSAssetCode { get; set; }

    public string? TSAssetName { get; set; }

    /// <summary>
    /// Nguồn gốc tài sản
    /// </summary>
    public int? SourceID { get; set; }

    public string? Seri { get; set; }

    /// <summary>
    /// quy cách tài sản
    /// </summary>
    public string? SpecificationsAsset { get; set; }

    /// <summary>
    /// nhà cung cấp tài sản
    /// </summary>
    public int? SupplierID { get; set; }

    public DateTime? DateBuy { get; set; }

    /// <summary>
    /// Thời gian bảo hành
    /// </summary>
    public decimal? Insurance { get; set; }

    /// <summary>
    /// Thời gian áp dụng
    /// </summary>
    public DateTime? DateEffect { get; set; }

    public string? Status { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public int? UnitID { get; set; }

    public string? TSCodeNCC { get; set; }

    public int? STT { get; set; }

    /// <summary>
    /// 1: Chưa active; 2: Đã active; 3: Crack
    /// </summary>
    public int? OfficeActiveStatus { get; set; }

    /// <summary>
    /// 1: Chưa active; 2: Đã active; 3: Crack
    /// </summary>
    public int? WindowActiveStatus { get; set; }

    public string? Model { get; set; }
}
