using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class HandoverMinutesDetail
{
    public int ID { get; set; }

    public int? HandoverMinutesID { get; set; }

    public int? STT { get; set; }

    public int? POKHID { get; set; }

    public int? ProductSaleID { get; set; }

    public decimal? Quantity { get; set; }

    /// <summary>
    /// 1: Hàng mới
    /// </summary>
    public int? ProductStatus { get; set; }

    public string? Guarantee { get; set; }

    /// <summary>
    /// 1: đã nhận đủ; 2: thiếu
    /// </summary>
    public int? DeliveryStatus { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? POKHDetailID { get; set; }

    public bool IsDeleted { get; set; }
}
