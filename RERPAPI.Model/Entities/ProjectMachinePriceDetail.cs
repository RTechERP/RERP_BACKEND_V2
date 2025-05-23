using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectMachinePriceDetail
{
    public int ID { get; set; }

    public int? ProjectMachinePriceID { get; set; }

    public int? ProjectVersionID { get; set; }

    public int? STT { get; set; }

    public string? CodeGroup { get; set; }

    public string? NameGroup { get; set; }

    /// <summary>
    /// Nội dung
    /// </summary>
    public string? ContentPrice { get; set; }

    /// <summary>
    /// Số tiền chi
    /// </summary>
    public decimal? AmountSpent { get; set; }

    /// <summary>
    /// Đối tượng phụ trách
    /// </summary>
    public string? DependentObject { get; set; }

    /// <summary>
    /// Chi phí dự toán
    /// </summary>
    public decimal? EstimateCost { get; set; }

    /// <summary>
    /// Hệ số
    /// </summary>
    public decimal? Coefficient { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}
