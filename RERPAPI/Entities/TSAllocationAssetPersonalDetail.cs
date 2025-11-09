using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class TSAllocationAssetPersonalDetail
{
    public int ID { get; set; }

    public int? STT { get; set; }

    /// <summary>
    /// Lấy từ bảng TSAllocationAssetPersonal( cột ID)
    /// </summary>
    public int? TSAllocationAssetPersonalID { get; set; }

    /// <summary>
    /// Lấy từ bảng TSAssetManagementPersonal(cột ID)
    /// </summary>
    public int? TSAssetManagementPersonalID { get; set; }

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
