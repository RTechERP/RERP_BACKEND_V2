using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class FollowProjectBase
{
    public int ID { get; set; }

    public int? ProjectID { get; set; }

    public int? UserID { get; set; }

    public int? CustomerBaseID { get; set; }

    public int? EndUserID { get; set; }

    /// <summary>
    /// trạng thái dự án
    /// </summary>
    public int? ProjectStatusBaseID { get; set; }

    /// <summary>
    /// ngày bắt đầu dự án
    /// </summary>
    public DateTime? ProjectStartDate { get; set; }

    /// <summary>
    /// loại dự án
    /// </summary>
    public int? ProjectTypeBaseID { get; set; }

    /// <summary>
    /// hãng
    /// </summary>
    public int? FirmBaseID { get; set; }

    /// <summary>
    /// việc đã làm
    /// </summary>
    public string? WorkDone { get; set; }

    /// <summary>
    /// ngày thực hiện gần nhất
    /// </summary>
    public DateTime? ImplementationDate { get; set; }

    /// <summary>
    /// việc sẽ làm
    /// </summary>
    public string? WorkWillDo { get; set; }

    /// <summary>
    /// Ngày dự kiến thực hiện
    /// </summary>
    public DateTime? ExpectedDate { get; set; }

    /// <summary>
    /// khả năng có PO
    /// </summary>
    public string? PossibilityPO { get; set; }

    public string? Fail { get; set; }

    /// <summary>
    /// dự kiến ngày lên phương án
    /// </summary>
    public DateTime? ExpectedPlanDate { get; set; }

    /// <summary>
    /// dk ngày báo giá
    /// </summary>
    public DateTime? ExpectedQuotationDate { get; set; }

    /// <summary>
    /// dk ngày PO
    /// </summary>
    public DateTime? ExpectedPODate { get; set; }

    /// <summary>
    /// dk ngày kết thúc dự án
    /// </summary>
    public DateTime? ExpectedProjectEndDate { get; set; }

    /// <summary>
    /// thực tế ngày lên phương án
    /// </summary>
    public DateTime? RealityPlanDate { get; set; }

    /// <summary>
    /// thực tế ngày báo giá
    /// </summary>
    public DateTime? RealityQuotationDate { get; set; }

    /// <summary>
    /// tt ngày po
    /// </summary>
    public DateTime? RealityPODate { get; set; }

    /// <summary>
    /// tt ngày kết thúc dự án
    /// </summary>
    public DateTime? RealityProjectEndDate { get; set; }

    /// <summary>
    /// tổng giá chưa VAT
    /// </summary>
    public decimal? TotalWithoutVAT { get; set; }

    /// <summary>
    /// người phụ trách chính
    /// </summary>
    public string? ProjectContactName { get; set; }

    public string? Note { get; set; }

    public string? WorkDonePM { get; set; }

    public string? WorkWillDoPM { get; set; }

    public DateTime? DateDonePM { get; set; }

    public DateTime? DateWillDoPM { get; set; }

    public DateTime? DateDoneSale { get; set; }

    public DateTime? DateWillDoSale { get; set; }

    public string? Results { get; set; }

    public string? ProblemBacklog { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? WarehouseID { get; set; }
}
