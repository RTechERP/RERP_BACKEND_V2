using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class TradePriceDetail
{
    public int ID { get; set; }

    public string? STT { get; set; }

    public string? Maker { get; set; }

    public int? ProductID { get; set; }

    public string? ProductCodeCustomer { get; set; }

    public int? Quantity { get; set; }

    public string? Unit { get; set; }

    public int? TradePriceID { get; set; }

    public decimal? UnitImportPriceUSD { get; set; }

    public decimal? TotalImportPriceUSD { get; set; }

    public decimal? UnitImportPriceVND { get; set; }

    public decimal? TotalImportPriceVND { get; set; }

    public decimal? BankCharge { get; set; }

    public decimal? ProtectiveTariff { get; set; }

    public decimal? ProtectiveTariffPerPcs { get; set; }

    public decimal? TotalProtectiveTariff { get; set; }

    public decimal? OrtherFees { get; set; }

    public decimal? CustomFees { get; set; }

    public decimal? TotalImportPriceIncludeFees { get; set; }

    public decimal? UnitPriceIncludeFees { get; set; }

    public decimal? CMPerSet { get; set; }

    public decimal? UnitPriceExpectCustomer { get; set; }

    public decimal? TotalPriceExpectCustomer { get; set; }

    public decimal? Profit { get; set; }

    public decimal? ProfitPercent { get; set; }

    public string? LeadTime { get; set; }

    public decimal? TotalPriceLabor { get; set; }

    public decimal? TotalPriceRTCVision { get; set; }

    public decimal? TotalPrice { get; set; }

    public decimal? UnitPricePerCOM { get; set; }

    public string? Note { get; set; }

    public int? ParentID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int? UnitCountID { get; set; }

    public decimal? FeeShipPcs { get; set; }

    public string? ProductName { get; set; }

    public string? ProductCode { get; set; }

    public string? ProductCodeOrigin { get; set; }

    public int? CurrencyID { get; set; }

    public decimal? CurrencyRate { get; set; }

    public decimal? Margin { get; set; }
}
