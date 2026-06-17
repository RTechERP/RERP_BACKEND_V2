using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class TSAssetManagementPersonal
{
    public int ID { get; set; }

    public int? STT { get; set; }

    /// <summary>
    /// Lấy từ bảng UnitCount(ID)
    /// </summary>
    public int? UnitCountID { get; set; }

    /// <summary>
    /// Lấy từ bảng TSTypeAssetPersonal cột(ID)
    /// </summary>
    public int? TSTypeAssetPersonalID { get; set; }

    public int? Quantity { get; set; }

    public DateTime? DateBuy { get; set; }

    /// <summary>
    /// ID của nhân viên quản lí tài sản cá nhân
    /// </summary>
    public int? EmployeeID { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 0.Chưa xóa , 1.Đã xóa
    /// </summary>
    public bool? IsDeleted { get; set; }
}
