using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class POKHHistory
{
    public int ID { get; set; }

    public string? CustomerCode { get; set; }

    public string? IndexCode { get; set; }

    public string? PONumber { get; set; }

    public DateTime? PODate { get; set; }

    public string? ProductCode { get; set; }

    public string? Model { get; set; }

    public decimal? Quantity { get; set; }

    public decimal? QuantityDeliver { get; set; }

    public decimal? QuantityPending { get; set; }

    public string? Unit { get; set; }

    public decimal? NetPrice { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? TotalPrice { get; set; }

    public decimal? VAT { get; set; }

    public decimal? TotalPriceVAT { get; set; }

    public DateTime? DeliverDate { get; set; }

    public DateTime? PaymentDate { get; set; }

    public DateTime? BillDate { get; set; }

    public string? BillNumber { get; set; }

    public string? Dept { get; set; }

    public string? Sale { get; set; }

    public string? Pur { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? POTypeCode { get; set; }
}
