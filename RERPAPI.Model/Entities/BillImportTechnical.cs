using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class BillImportTechnical
{
    public int ID { get; set; }

    /// <summary>
    /// Mã phiếu
    /// </summary>
    public string? BillCode { get; set; }

    /// <summary>
    /// Ngày nhập
    /// </summary>
    public DateTime? CreatDate { get; set; }

    /// <summary>
    /// Người giao
    /// </summary>
    public string? Deliver { get; set; }

    /// <summary>
    /// Người nhận
    /// </summary>
    public string? Receiver { get; set; }

    /// <summary>
    /// Trạng thái phiếu (0: chưa duyệt, 1: đã duyệt)
    /// </summary>
    public bool? Status { get; set; }

    /// <summary>
    /// Tên nhà cung cấp
    /// </summary>
    public string? Suplier { get; set; }

    /// <summary>
    /// 0: Kho Demo; 1: Kho AGV
    /// </summary>
    public bool? BillType { get; set; }

    /// <summary>
    /// Loại kho (1:demo;2:agv)
    /// </summary>
    public string? WarehouseType { get; set; }

    /// <summary>
    /// ID người giao
    /// </summary>
    public int? DeliverID { get; set; }

    /// <summary>
    /// ID người nhận
    /// </summary>
    public int? ReceiverID { get; set; }

    /// <summary>
    /// ID nhà cung cấp
    /// </summary>
    public int? SuplierID { get; set; }

    public int? GroupTypeID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? Image { get; set; }

    /// <summary>
    /// ID kho
    /// </summary>
    public int? WarehouseID { get; set; }

    /// <summary>
    /// ID nhà cung cấp 
    /// </summary>
    public int? SupplierSaleID { get; set; }

    public int? BillTypeNew { get; set; }

    /// <summary>
    /// Mượn hàng từ nhà cung cấp (1: có, 0: không)
    /// </summary>
    public int? IsBorrowSupplier { get; set; }

    /// <summary>
    /// ID khách hàng
    /// </summary>
    public int? CustomerID { get; set; }

    /// <summary>
    /// Loại chứng từ nhập
    /// </summary>
    public int? BillDocumentImportType { get; set; }

    /// <summary>
    /// Ngày yêu cầu nhập hàng
    /// </summary>
    public DateTime? DateRequestImport { get; set; }

    /// <summary>
    /// ID điều khoản thanh toán
    /// </summary>
    public int? RulePayID { get; set; }

    /// <summary>
    /// Đánh dấu chuẩn hóa dữ liệu
    /// </summary>
    public bool? IsNormalize { get; set; }

    /// <summary>
    /// ID người duyệt
    /// </summary>
    public int? ApproverID { get; set; }

    public bool IsDeleted { get; set; }
}
