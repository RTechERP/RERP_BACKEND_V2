using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProjectPartList
{
    public int ID { get; set; }

    public int? ProjectID { get; set; }

    public int? STT { get; set; }

    public string? GroupMaterial { get; set; }

    public string? Model { get; set; }

    public string? ProductCode { get; set; }

    public string? Manufacturer { get; set; }

    public string? Unit { get; set; }

    public decimal? QtyMin { get; set; }

    public decimal? QtyFull { get; set; }

    public decimal? Price { get; set; }

    public decimal? Amount { get; set; }

    public decimal? VAT { get; set; }

    public string? LeadTime { get; set; }

    public DateTime? ExpectedReturnDate { get; set; }

    public int? Status { get; set; }

    public string? Note { get; set; }

    public string? Note1 { get; set; }

    public string? Note2 { get; set; }

    public int? EmployeeID { get; set; }

    public string? NCC { get; set; }

    public DateTime? RequestDate { get; set; }

    public bool? IsDeleted { get; set; }

    public decimal? QtyReturned { get; set; }

    public bool? IsApprovedTBP { get; set; }

    public bool? IsApprovedPurchase { get; set; }

    public int? ProjectPartListTypeID { get; set; }

    public int? ParentID { get; set; }

    public string? Quality { get; set; }

    public DateTime? OrderDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public string? TT { get; set; }

    public string? OrderCode { get; set; }

    public string? NCCFinal { get; set; }

    public decimal? PriceOrder { get; set; }

    public decimal? TotalPriceOrder { get; set; }

    public int? ProjectPartListVersionID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public string? SpecialCode { get; set; }

    public int? ProjectTypeID { get; set; }

    public int? StatusPriceRequest { get; set; }

    public DateTime? DeadlinePriceRequest { get; set; }

    public int? SupplierSaleID { get; set; }

    public string? UnitMoney { get; set; }

    public DateTime? DatePriceRequest { get; set; }

    public DateTime? DatePriceQuote { get; set; }

    public decimal? QuantityReturn { get; set; }

    public string? ReasonProblem { get; set; }

    public bool? IsProblem { get; set; }

    public int? SuplierSaleFinalID { get; set; }

    public string? LeadTimeRequest { get; set; }

    public string? ReasonDeleted { get; set; }

    public bool? IsNewCode { get; set; }

    public bool? IsApprovedTBPNewCode { get; set; }

    public DateTime? DateApprovedNewCode { get; set; }
}
