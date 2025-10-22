using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class TSAllocationAssetPersonal
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public string? Code { get; set; }
    public string? Note { get; set; }

    public DateTime? DateAllocation { get; set; }

    /// <summary>
    /// ID của nhân viên được cấp phát lấy từ bảng Employee
    /// </summary>
    public int? EmployeeID { get; set; }

    /// <summary>
    /// 0.Chưa duyệt 1.Đã duyệt
    /// </summary>
    public bool? IsApproveHR { get; set; }

    /// <summary>
    /// 0.Chưa duyệt 1.Đã duyệt
    /// </summary>
    public bool? IsApprovedPersonalProperty { get; set; }

    public DateTime? DateApprovedPersonalProperty { get; set; }

    public DateTime? DateApprovedHR { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 0.Chưa Xóa 1.Đã xóa
    /// </summary>
    public bool? IsDeleted { get; set; }

    public string? Note { get; set; }
}
