using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class DocumentImportPONCC
{
    public int ID { get; set; }

    public int? PONCCID { get; set; }

    public int? DocumentImportID { get; set; }

    /// <summary>
    /// 1:Nhận; 2:Huỷ nhận;3:Khum có
    /// </summary>
    public int? Status { get; set; }

    public string? ReasonCancel { get; set; }

    public string? Note { get; set; }

    public DateTime? DateRecive { get; set; }

    public int? EmployeeReciveID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsAdditional { get; set; }

    public int? EmployeeAdditionalID { get; set; }

    public DateTime? DateAdditional { get; set; }

    public int? BillImportID { get; set; }

    /// <summary>
    /// 1:Nhận; 2:Huỷ nhận;3:Khum có
    /// </summary>
    public int? StatusHR { get; set; }

    public int? StatusPurchase { get; set; }
}
