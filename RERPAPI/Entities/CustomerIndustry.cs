using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class CustomerIndustry
{
    /// <summary>
    /// Khóa chính, tự tăng
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Tên lĩnh vực (Tiếng Việt)
    /// </summary>
    public string? IndustriesNameVI { get; set; }

    public string? IndustriesNameEN { get; set; }

    public string? Descriptions { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public int? NumberOrder { get; set; }

    public int? STT { get; set; }
}
