using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class TSRecoveryAssetPersonal
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public string? Code { get; set; }

    public DateTime? DateRecovery { get; set; }

    /// <summary>
    /// ID của nhân viên trả đồ(Thu hồi từ)
    /// </summary>
    public int? EmployeeReturnID { get; set; }

    /// <summary>
    /// ID của nhân viên thu hồi(Người thu hồi)
    /// </summary>
    public int? EmployeeRecoveryID { get; set; }

    /// <summary>
    /// 0. Chưa duyệt 1.Đã duyệt
    /// </summary>
    public bool? IsApproveHR { get; set; }

    /// <summary>
    /// 0. Chưa duyệt 1.Đã duyệt
    /// </summary>
    public bool? IsApprovedPersonalProperty { get; set; }

    public DateTime? DateApprovedPersonalProperty { get; set; }

    public DateTime? DateApprovedHR { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdatedBy { get; set; }

    public string? Note { get; set; }

    /// <summary>
    /// 0. Chưa xóa 1.Đã xóa
    /// </summary>
    public bool? IsDeleted { get; set; }
}
