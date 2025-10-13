using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class JobRequirementCheckPriceDetail
{
    public int ID { get; set; }

    public int? JobRequirementCheckPriceID { get; set; }

    public decimal? OfferPrice { get; set; }

    public decimal? PurchasePrice { get; set; }

    public decimal? ShippingFee { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? LeadTime { get; set; }

    public int? VAT { get; set; }

    public string? Supplier { get; set; }

    public int? Status { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
