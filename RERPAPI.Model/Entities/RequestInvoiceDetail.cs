using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class RequestInvoiceDetail
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? RequestInvoiceID { get; set; }

    public int? ProductSaleID { get; set; }

    public string? ProductByProject { get; set; }

    public decimal? Quantity { get; set; }

    public int? ProjectID { get; set; }

    public int? POKHDetailID { get; set; }

    public string? Specifications { get; set; }

    public string? InvoiceNumber { get; set; }

    public DateTime? InvoiceDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? Note { get; set; }
}
