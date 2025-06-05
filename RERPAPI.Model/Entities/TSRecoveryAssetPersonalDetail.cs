using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class TSRecoveryAssetPersonalDetail
{
    public int ID { get; set; }

    public int? STT { get; set; }

    /// <summary>
    /// Lấy từ bảng TSRecoveryAssetPersonal(Cột ID)
    /// </summary>
    public int? TSRecoveryAssetPersonalID { get; set; }

    /// <summary>
    /// Lấy từ bảng TSAssetManagementPersonal(cột ID)
    /// </summary>
    public int? TSAssetManagementPersonal { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 0.Chưa xóa 1.Đã xóa
    /// </summary>
    public bool? IsDeleted { get; set; }
}
