using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectPartlistPurchaseRequest
{
    public int ID { get; set; }

    public int? ProjectPartListID { get; set; }

    public int? EmployeeID { get; set; }

    public string? ProductCode { get; set; }

    public string? ProductName { get; set; }

    public int? UnitCountID { get; set; }

    /// <summary>
    /// 1:Y/c mua hàng;2:Huỷ Y/c mua; 3: Đã đặt hàng; 4: Đang về; 5:Đã về; 6:Không đặt hàng
    /// </summary>
    public int? StatusRequest { get; set; }

    public DateTime? DateRequest { get; set; }

    /// <summary>
    /// Ngày hàng về mong đợi (Deadline)
    /// </summary>
    public DateTime? DateReturnExpected { get; set; }

    public DateTime? DateOrder { get; set; }

    /// <summary>
    /// Ngày dự kiến hàng về
    /// </summary>
    public DateTime? DateEstimate { get; set; }

    /// <summary>
    /// Ngày hàng về thực tế
    /// </summary>
    public DateTime? DateReturnActual { get; set; }

    public DateTime? DateReceive { get; set; }

    public decimal? Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? TotalPrice { get; set; }

    public string? UnitMoney { get; set; }

    public int? SupplierSaleID { get; set; }

    public string? Note { get; set; }

    public bool? IsApprovedTBP { get; set; }

    public int? ApprovedTBP { get; set; }

    public bool? IsApprovedBGD { get; set; }

    public int? ApprovedBGD { get; set; }

    public DateTime? DateApprovedTBP { get; set; }

    public DateTime? DateApprovedBGD { get; set; }

    public int? ProductSaleID { get; set; }

    public int? ProductGroupID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? CurrencyID { get; set; }

    public decimal? CurrencyRate { get; set; }

    public decimal? HistoryPrice { get; set; }

    public decimal? TotalPriceExchange { get; set; }

    public string? LeadTime { get; set; }

    public decimal? UnitFactoryExportPrice { get; set; }

    public decimal? UnitImportPrice { get; set; }

    public decimal? TotalImportPrice { get; set; }

    public bool? IsImport { get; set; }

    public bool? IsRequestApproved { get; set; }

    public int? EmployeeIDRequestApproved { get; set; }

    public string? ReasonCancel { get; set; }

    public decimal? VAT { get; set; }

    public decimal? TotaMoneyVAT { get; set; }

    public int? TotalDayLeadTime { get; set; }

    public bool? IsCommercialProduct { get; set; }

    public int? POKHDetailID { get; set; }

    public int? JobRequirementID { get; set; }

    public bool? IsDeleted { get; set; }

    public int? InventoryProjectID { get; set; }

    public bool? IsTechBought { get; set; }

    public int? ProductGroupRTCID { get; set; }

    public int? ProductRTCID { get; set; }

    public int? TicketType { get; set; }

    public DateTime? DateReturnEstimated { get; set; }

    public int? EmployeeApproveID { get; set; }
}
