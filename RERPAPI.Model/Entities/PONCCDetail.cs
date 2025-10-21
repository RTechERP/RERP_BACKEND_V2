using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class PONCCDetail
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? PONCCID { get; set; }

    public int? ProductID { get; set; }

    public int? Qty { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? IntoMoney { get; set; }

    public string? CodeBill { get; set; }

    public string? NameBill { get; set; }

    /// <summary>
    /// ngày yêu cầu giao hàng
    /// </summary>
    public DateTime? RequestDate { get; set; }

    /// <summary>
    /// ngày giao hàng thực tế
    /// </summary>
    public DateTime? ActualDate { get; set; }

    public int? RequestBuyRTCID { get; set; }

    public decimal? QtyRequest { get; set; }

    public decimal? QtyReal { get; set; }

    public decimal? Soluongcon { get; set; }

    public decimal? Price { get; set; }

    /// <summary>
    /// thuế giá trị gia tăng (%)
    /// </summary>
    public decimal? VAT { get; set; }

    public decimal? VATMoney { get; set; }

    public decimal? ThanhTien { get; set; }

    public decimal? TotalPrice { get; set; }

    /// <summary>
    /// ngày yêu cầu giao hàng
    /// </summary>
    public DateTime? OrderDate { get; set; }

    public DateTime? ExpectedDate { get; set; }

    public decimal? FeeShip { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public decimal? PriceSale { get; set; }

    public decimal? CurrencyExchange { get; set; }

    public decimal? Discount { get; set; }

    public decimal? ProfitRate { get; set; }

    public decimal? PriceHistory { get; set; }

    public string? ProductCodeOfSupplier { get; set; }

    /// <summary>
    /// (0,chưa hoàn thành,1 hoàn thành)
    /// </summary>
    public int? Status { get; set; }

    public int? ProjectPartlistPurchaseRequestID { get; set; }

    public decimal? DiscountPercent { get; set; }

    public decimal? BiddingPrice { get; set; }

    public int? ProductSaleID { get; set; }

    public int? ProjectID { get; set; }

    public int? ProductRTCID { get; set; }

    public string? ProjectName { get; set; }

    public DateTime? DeadlineDelivery { get; set; }

    public int? ProjectPartListID { get; set; }

    public bool? IsBill { get; set; }

    public int? ProductType { get; set; }

    public DateTime? DateReturnEstimated { get; set; }

    public bool? IsStock { get; set; }

    public string? UnitName { get; set; }

    public string? ParentProductCode { get; set; }

    public bool? IsPurchase { get; set; }
}
