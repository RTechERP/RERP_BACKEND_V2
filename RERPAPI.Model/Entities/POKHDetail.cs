using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class POKHDetail
{
    public int ID { get; set; }

    public int? ParentID { get; set; }

    public int? POKHID { get; set; }

    public int? ProductID { get; set; }

    public int? KHID { get; set; }

    public int? Qty { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? IntoMoney { get; set; }

    public string? FilmSize { get; set; }

    public string? IndexPO { get; set; }

    public string? BillNumber { get; set; }

    public decimal? VAT { get; set; }

    public decimal? TotalPriceIncludeVAT { get; set; }

    public DateTime? RecivedMoneyDate { get; set; }

    public DateTime? BillDate { get; set; }

    /// <summary>
    /// Giao hàng thực tế
    /// </summary>
    public DateTime? ActualDeliveryDate { get; set; }

    public decimal? EstimatedPay { get; set; }

    public DateTime? DeliveryRequestedDate { get; set; }

    public DateTime? PayDate { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? STT { get; set; }

    public int? NewRow { get; set; }

    public bool? IsOder { get; set; }

    public int? QuotationDetailID { get; set; }

    public string? GroupPO { get; set; }

    public string? GuestCode { get; set; }

    public int? QtyTT { get; set; }

    public int? QtyCL { get; set; }

    public bool? IsExport { get; set; }

    public int? Debt { get; set; }

    public string? UserReceiver { get; set; }

    public int? QtyRequest { get; set; }

    public decimal? NetUnitPrice { get; set; }

    public string? Note { get; set; }

    public int? CurrencyID { get; set; }

    public string? TT { get; set; }

    public int? ProjectPartListID { get; set; }

    public string? Spec { get; set; }
    public bool? IsDeleted { get; set; }
}
