using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class Project
{
    public int ID { get; set; }

    /// <summary>
    /// Khách hàng
    /// </summary>
    public int CustomerID { get; set; }

    /// <summary>
    /// Mã dự án
    /// </summary>
    public string ProjectCode { get; set; } = null!;

    /// <summary>
    /// Tên dự án
    /// </summary>
    public string ProjectName { get; set; } = null!;

    public string? ProjectShortName { get; set; }

    /// <summary>
    /// 0: Chưa hoàn thành, 1: Hoàn thành
    /// </summary>
    public int? ProjectStatus { get; set; }

    /// <summary>
    /// Người phụ trách chính
    /// </summary>
    public int UserID { get; set; }

    public int? UserTechnicalID { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Note { get; set; }

    public bool? IsApproved { get; set; }

    public int? ContactID { get; set; }

    public string? PO { get; set; }

    public int? ProjectType { get; set; }

    public int? ListCostID { get; set; }

    public DateTime? PlanDateStart { get; set; }

    public DateTime? PlanDateEnd { get; set; }

    public DateTime? ActualDateStart { get; set; }

    public DateTime? ActualDateEnd { get; set; }

    public string? EU { get; set; }

    public int? ProjectManager { get; set; }

    public string? CurrentState { get; set; }

    /// <summary>
    /// Mức độ ưu tiên
    /// </summary>
    public decimal? Priotity { get; set; }

    public DateTime? PODate { get; set; }

    /// <summary>
    /// Link id khách hàng
    /// </summary>
    public int? EndUser { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? BusinessFieldID { get; set; }

    /// <summary>
    /// 1: Dự án; 2: Thương mại; 3:Phim
    /// </summary>
    public int? TypeProject { get; set; }
}
