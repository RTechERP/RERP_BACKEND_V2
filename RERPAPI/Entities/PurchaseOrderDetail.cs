using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class PurchaseOrderDetail
{
    public long ID { get; set; }

    public int? PurchaseOrderID { get; set; }

    public int? ProjectID { get; set; }

    public long? ParentID { get; set; }

    public int? ManufacturerID { get; set; }

    public int? PartID { get; set; }

    public string? PartCode { get; set; }

    public string? PartName { get; set; }

    public string? PartCodeRTC { get; set; }

    public string? PartNameRTC { get; set; }

    public string? Unit { get; set; }

    public decimal? Qty { get; set; }

    public decimal? DeliveryCost { get; set; }

    public decimal? Price { get; set; }

    public decimal? TotalPrice { get; set; }

    public decimal? VAT { get; set; }

    public decimal? FinishPrice { get; set; }

    public string? ManufacturerCode { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
