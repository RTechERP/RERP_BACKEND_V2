using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class BillImport
{
    public int ID { get; set; }

    /// <summary>
    /// Mã phiếu nhập kho
    /// </summary>
    public string? BillImportCode { get; set; }

    /// <summary>
    /// Ngày nhập
    /// </summary>
    public DateTime? CreatDate { get; set; }

    /// <summary>
    /// người giao
    /// </summary>
    public string? Deliver { get; set; }

    /// <summary>
    /// Người nhận
    /// </summary>
    public string? Reciver { get; set; }

    /// <summary>
    /// Trạng thái, 1:Duyệt, 0: Chưa duyệt
    /// </summary>
    public bool? Status { get; set; }

    /// <summary>
    /// Tên nhà cung cấp
    /// </summary>
    public string? Suplier { get; set; }

    /// <summary>
    /// Loại phiếu: 1: Phiếu trả, 0: phiếu nhập bình thường
    /// </summary>
    public bool? BillType { get; set; }

    /// <summary>
    /// Kho: 1:Kho sale, 2: kho dự án, 0: tất cả
    /// </summary>
    public string? KhoType { get; set; }

    public string? GroupID { get; set; }

    /// <summary>
    /// ID nhà cung cấp
    /// </summary>
    public int? SupplierID { get; set; }

    public int? DeliverID { get; set; }

    /// <summary>
    /// ID người nhận hàng
    /// </summary>
    public int? ReciverID { get; set; }

    public int? KhoTypeID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int? UnApprove { get; set; }

    public bool? PTNB { get; set; }

    /// <summary>
    /// ID kho
    /// </summary>
    public int? WarehouseID { get; set; }

    /// <summary>
    /// 0: Phiếu nhập
    /// 1: Phiếu trả
    /// 2: Phiếu trả nội bộ
    /// 3: Phiếu mượn NCC
    /// 4: Yêu cầu nhập kho
    /// 
    /// </summary>
    public int? BillTypeNew { get; set; }

    /// <summary>
    /// 1:Hoàn thành; 2:Chưa hoàn thành
    /// </summary>
    public int? BillDocumentImportType { get; set; }

    /// <summary>
    /// Ngày yêu cầu nhập kho
    /// </summary>
    public DateTime? DateRequestImport { get; set; }

    /// <summary>
    /// ID điều khoản thanh toán
    /// </summary>
    public int? RulePayID { get; set; }

    public bool? IsDeleted { get; set; }

    /// <summary>
    /// 1: đã đủ, 0: chưa đủ
    /// </summary>
    public bool? StatusDocumentImport { get; set; }

    public int? BillExportID { get; set; }
}
