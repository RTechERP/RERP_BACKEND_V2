using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class CommercialPriceRequest
{
    public int ID { get; set; }

    public string? RfqNo { get; set; }

    public int? RequestSeq { get; set; }

    public string? ProductCode { get; set; }

    public string? ProductName { get; set; }

    public string? Specification { get; set; }

    public string? Description { get; set; }

    public decimal? Qty { get; set; }

    public string? Unit { get; set; }

    public decimal? Moq { get; set; }

    public int? PicPurID { get; set; }

    public string? PicPurName { get; set; }

    public DateTime? AdminSentAt { get; set; }

    public DateTime? PurSentAt { get; set; }

    public DateOnly? QuoteDeadline { get; set; }

    public DateOnly? SaleDeadline { get; set; }

    public DateOnly? SalesPushedAt { get; set; }

    public string? Supplier { get; set; }

    public string? UnitPrice { get; set; }

    public decimal? ShippingCost { get; set; }

    public decimal? OtherCost { get; set; }

    public decimal? Vat { get; set; }

    public string? Leadtime { get; set; }

    public string? SaleLeadtime { get; set; }

    public decimal? MarginRate { get; set; }

    public decimal? SaleUnitPrice { get; set; }

    public decimal? SaleTotalPrice { get; set; }

    public int? IsSaleQuoted { get; set; }

    public int? IsPurQuoted { get; set; }

    public int? IsPO { get; set; }

    public int? QuoteRound { get; set; }

    public int? WeekNo { get; set; }

    public int? MonthNo { get; set; }

    public int? YearNo { get; set; }

    public DateTime? PurRepliedAt { get; set; }

    public string? RequestNote { get; set; }

    public string? ImportPriceNote { get; set; }

    public string? SaleNote { get; set; }

    public string? NoteReason { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}
