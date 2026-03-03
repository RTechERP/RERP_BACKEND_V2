using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class PONCCHistory
{
    public int ID { get; set; }

    public DateTime? RequestDate { get; set; }

    public string? CompanyText { get; set; }

    public string? BillCode { get; set; }

    public string? Note { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public string? CodeNCC { get; set; }

    public string? NameNCC { get; set; }

    public string? ProductNewCode { get; set; }

    public string? ProductName { get; set; }

    public string? Unit { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? UnitPriceVAT { get; set; }

    public decimal? QtyRequest { get; set; }

    public decimal? QuantityReturn { get; set; }

    public decimal? QuantityRemain { get; set; }

    public string? StatusText { get; set; }

    public string? FullName { get; set; }

    public bool? NCCNew { get; set; }

    public bool? DeptSupplier { get; set; }

    public decimal? FeeShip { get; set; }

    public decimal? PriceSale { get; set; }

    public DateTime? DeadlineDelivery { get; set; }

    public string? POCode { get; set; }

    public string? ProductCode { get; set; }

    public decimal? TotalMoneyChangePO { get; set; }

    public decimal? TotalPrice { get; set; }

    public string? ProjectCode { get; set; }

    public decimal? PriceHistory { get; set; }

    public decimal? BiddingPrice { get; set; }

    public string? SupplierVoucher { get; set; }

    public decimal? TotalQuantityLast { get; set; }

    public decimal? MinQuantity { get; set; }

    public string? ProductCodeOfSupplier { get; set; }

    public string? CurrencyName { get; set; }

    public string? ProjectName { get; set; }

    public decimal? VAT { get; set; }

    public decimal? CurrencyRate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
