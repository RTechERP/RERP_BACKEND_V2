using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

/// <summary>
/// Danh sách thiết bị nhập
/// </summary>
public partial class BillImportDetail
{
    public int ID { get; set; }

    /// <summary>
    /// Mã master nhập
    /// </summary>
    public int? BillImportID { get; set; }

    public int? ProductID { get; set; }

    public decimal? Qty { get; set; }

    public decimal? Price { get; set; }

    public decimal? TotalPrice { get; set; }

    public string? ProjectName { get; set; }

    public string? ProjectCode { get; set; }

    /// <summary>
    /// sô hóa đơn
    /// </summary>
    public string? SomeBill { get; set; }

    public string? Note { get; set; }

    public int? STT { get; set; }

    public decimal? TotalQty { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int? ProjectID { get; set; }

    public int? PONCCDetailID { get; set; }

    public string? SerialNumber { get; set; }

    public string? CodeMaPhieuMuon { get; set; }

    public int? BillExportDetailID { get; set; }

    public int? ProjectPartListID { get; set; }

    public bool? IsKeepProject { get; set; }

    public decimal? QtyRequest { get; set; }

    public string? BillCodePO { get; set; }

    /// <summary>
    /// 1: Đã trả, 2: Chưa trả
    /// </summary>
    public bool? ReturnedStatus { get; set; }

    public int? InventoryProjectID { get; set; }

    public DateTime? DateSomeBill { get; set; }

    public bool? IsDeleted { get; set; }

    public string? UnitName { get; set; }

    /// <summary>
    /// Số ngày công nợ
    /// </summary>
    public int? DPO { get; set; }

    /// <summary>
    /// Ngày tới hạn
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Tiền thuế giảm
    /// </summary>
    public decimal? TaxReduction { get; set; }

    /// <summary>
    /// Chi phí FE
    /// </summary>
    public decimal? COFormE { get; set; }

    public bool? IsNotKeep { get; set; }
}
