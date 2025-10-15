using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class TSTypeAssetPersonal
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public string? Name { get; set; }

    public string? Code { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Năm của tài sản
    /// </summary>
    public int? YearValue { get; set; }

    /// <summary>
    /// 0.Chưa xóa 1.Đã xóa
    /// </summary>
    public bool? IsDeleted { get; set; }

    public int? StandardAmount { get; set; }
}
