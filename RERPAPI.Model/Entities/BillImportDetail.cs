using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

/// <summary>
/// Danh sách thiết bị nhập
/// </summary>
public partial class BillImportDetail
{
    public int ID { get; set; }

    /// <summary>
    /// ID master nhập
    /// </summary>
    public int? BillImportID { get; set; }

    /// <summary>
    /// ID sản phẩm
    /// </summary>
    public int? ProductID { get; set; }

    /// <summary>
    /// Số lượn thực tế
    /// </summary>
    public decimal? Qty { get; set; }

    /// <summary>
    /// Đơn giá
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// Tổng tiền
    /// </summary>
    public decimal? TotalPrice { get; set; }

    /// <summary>
    /// Tên dự án
    /// </summary>
    public string? ProjectName { get; set; }

    /// <summary>
    /// Mã sản phẩm theo dự án
    /// </summary>
    public string? ProjectCode { get; set; }

    /// <summary>
    /// sô hóa đơn
    /// </summary>
    public string? SomeBill { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// STT
    /// </summary>
    public int? STT { get; set; }

    /// <summary>
    /// Tổng số lượng
    /// </summary>
    public decimal? TotalQty { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    /// <summary>
    /// ID dự án
    /// </summary>
    public int? ProjectID { get; set; }

    /// <summary>
    /// ID chi tiết PONCC
    /// </summary>
    public int? PONCCDetailID { get; set; }

    /// <summary>
    /// số serial
    /// </summary>
    public string? SerialNumber { get; set; }

    /// <summary>
    /// Mã phiếu mượn
    /// </summary>
    public string? CodeMaPhieuMuon { get; set; }

    /// <summary>
    /// ID chi tiết phiếu mượn
    /// </summary>
    public int? BillExportDetailID { get; set; }

    /// <summary>
    /// ID danh mục vật tư
    /// </summary>
    public int? ProjectPartListID { get; set; }

    /// <summary>
    /// Giữ cho dự án
    /// </summary>
    public bool? IsKeepProject { get; set; }

    /// <summary>
    /// Số lượng yêu cầu
    /// </summary>
    public decimal? QtyRequest { get; set; }

    /// <summary>
    /// Đơn mua hàng
    /// </summary>
    public string? BillCodePO { get; set; }

    /// <summary>
    /// Trạng thái trả:1: Đã trả, 2: Chưa trả
    /// </summary>
    public bool? ReturnedStatus { get; set; }

    /// <summary>
    /// ID tồn kho giữ
    /// </summary>
    public int? InventoryProjectID { get; set; }

    /// <summary>
    /// Ngày hóa đơn
    /// </summary>
    public DateTime? DateSomeBill { get; set; }

    public bool? IsDeleted { get; set; }

    /// <summary>
    /// Tên đơn vị tính
    /// </summary>
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

    /// <summary>
    /// Không giữ
    /// </summary>
    public bool? IsNotKeep { get; set; }
}
