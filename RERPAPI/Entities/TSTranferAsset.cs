using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

/// <summary>
/// Khoa
/// </summary>
public partial class TSTranferAsset
{
    public int ID { get; set; }

    public int? AssetManagementID { get; set; }

    /// <summary>
    /// ID người giao tài sản lấy từ bảng employee
    /// </summary>
    public int? DeliverID { get; set; }

    /// <summary>
    /// ID người nhận lấy từ bảng employee
    /// </summary>
    public int? ReceiverID { get; set; }

    /// <summary>
    /// ID lấy từ Department (chuyển từ phòng)
    /// </summary>
    public int? FromDepartmentID { get; set; }

    /// <summary>
    /// ID lấy từ DepartmentID (Phòng được nhận)
    /// </summary>
    public int? ToDepartmentID { get; set; }

    public int? FromChucVuID { get; set; }

    public int? ToChucVuID { get; set; }

    /// <summary>
    /// Mã biên bản điều chuyển
    /// </summary>
    public string? CodeReport { get; set; }

    /// <summary>
    /// Ngày điều chuyển
    /// </summary>
    public DateTime? TranferDate { get; set; }

    /// <summary>
    /// Lý do điều chuyển
    /// </summary>
    public string? Reason { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsApproved { get; set; }

    public bool? IsApproveAccountant { get; set; }

    public bool? IsApprovedPersonalProperty { get; set; }

    public DateTime? DateApproveAccountant { get; set; }

    public DateTime? DateApprovedPersonalProperty { get; set; }

    public DateTime? DateApprovedHR { get; set; }
}
