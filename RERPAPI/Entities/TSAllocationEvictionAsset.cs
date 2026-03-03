using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

/// <summary>
/// Khoa
/// </summary>
public partial class TSAllocationEvictionAsset
{
    public int ID { get; set; }

    /// <summary>
    /// ID của grvMaster
    /// </summary>
    public int? AssetManagementID { get; set; }

    public int? EmployeeID { get; set; }

    public int? ChucVuID { get; set; }

    public int? DepartmentID { get; set; }

    /// <summary>
    /// Ngày cấp phát, thu hồi,  tài sản cho nhân viên
    /// </summary>
    public DateTime? DateAllocation { get; set; }

    /// <summary>
    /// Trạng thái của sản phẩm
    /// </summary>
    public string? Status { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
