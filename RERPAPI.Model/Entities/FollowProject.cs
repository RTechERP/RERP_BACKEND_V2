using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class FollowProject
{
    public int ID { get; set; }

    public string? POCode { get; set; }

    public int? CustomerID { get; set; }

    public string? Project { get; set; }

    public string? Status { get; set; }

    /// <summary>
    /// tổng chi phí không có VAT
    /// </summary>
    public decimal? TotalCostWithoutVAT { get; set; }

    /// <summary>
    /// tổng chi phí bao gồm VAT
    /// </summary>
    public decimal? TotalCostIncludingVAT { get; set; }

    /// <summary>
    /// Thuế VAT
    /// </summary>
    public decimal? Tax { get; set; }

    /// <summary>
    /// tổng chi phí hải quan
    /// </summary>
    public decimal? TotalCustomFees { get; set; }

    /// <summary>
    /// Tổng chi phí vận chuyển
    /// </summary>
    public decimal? TotalTransportFee { get; set; }

    /// <summary>
    /// tổng báo giá khách hàng bảo gồm VAT
    /// </summary>
    public decimal? TotalCustomerQuotation { get; set; }

    /// <summary>
    /// Tổng phí ngân hàng
    /// </summary>
    public decimal? TotalBankCharges { get; set; }

    public decimal? TransportFee { get; set; }

    public decimal? CustomerQuotation { get; set; }

    public DateTime? CreateDate { get; set; }

    public int? UserID { get; set; }

    public int? ProjectID { get; set; }

    public int? POKHID { get; set; }

    public decimal? Exchange { get; set; }

    public decimal? CustomFees { get; set; }

    public decimal? Declaration { get; set; }

    public decimal? BankCharges { get; set; }

    public decimal? NumberOfTransactions { get; set; }

    public bool IsApproved { get; set; }
}
