using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class TradePrice
{
    public int ID { get; set; }

    public int? CustomerID { get; set; }

    public int? ProjectID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int? IsApprovedSale { get; set; }

    public int? IsApprovedLeader { get; set; }

    public int? IsApprovedBGD { get; set; }

    public int? ApprovedSaleID { get; set; }

    public int? ApprovedLeaderID { get; set; }

    public int? ApprovedBGDID { get; set; }

    public DateTime? SaleApprovedDate { get; set; }

    public DateTime? LeaderApprovedDate { get; set; }

    public DateTime? BGDApprovedDate { get; set; }

    public int? SaleAdminID { get; set; }

    public int? EmployeeID { get; set; }

    public decimal? RateCOM { get; set; }

    public decimal? COM { get; set; }

    public int? CurrencyID { get; set; }

    public decimal? CurrencyRate { get; set; }

    public bool? CurrencyType { get; set; }

    public bool? IsRTCVision { get; set; }

    public int? QuantityRTCVision { get; set; }

    public decimal? UnitPriceRTCVision { get; set; }

    public int? IsQuotation { get; set; }

    public decimal? UnitPriceDelivery { get; set; }

    public int? QuantityDelivery { get; set; }

    public bool? IsDeleted { get; set; }
}
