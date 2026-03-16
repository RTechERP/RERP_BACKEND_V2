using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class BillExportTechnical
{
    public int ID { get; set; }

    /// <summary>
    /// Mã phiếu xuất
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Loại phiếu xuất: 0. Trả
    /// 1. Cho mượn
    /// 2. Tặng / Bán
    /// 3. Mất
    /// 4. Bảo hành
    /// 5. Xuất dự án
    /// 6. Hỏng
    /// 7. Xuất kho
    /// </summary>
    public int? BillType { get; set; }

    /// <summary>
    /// ID khách hàng
    /// </summary>
    public int? CustomerID { get; set; }

    /// <summary>
    /// Người nhận
    /// </summary>
    public string? Receiver { get; set; }

    /// <summary>
    /// Người giao
    /// </summary>
    public string? Deliver { get; set; }

    /// <summary>
    /// Địa chỉ
    /// </summary>
    public string? Addres { get; set; }

    /// <summary>
    /// Trạng thái duyệt
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// Loại kho
    /// </summary>
    public string? WarehouseType { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Note { get; set; }

    public string? Image { get; set; }

    /// <summary>
    /// ID Người nhận
    /// </summary>
    public int? ReceiverID { get; set; }

    /// <summary>
    /// ID người giao
    /// </summary>
    public int? DeliverID { get; set; }

    /// <summary>
    /// ID nhà cung cấp
    /// </summary>
    public int? SupplierID { get; set; }

    /// <summary>
    /// Tên khách hàng
    /// </summary>
    public string? CustomerName { get; set; }

    /// <summary>
    /// Tên nhà cung cấp
    /// </summary>
    public string? SupplierName { get; set; }

    /// <summary>
    /// kiểm tra phiếu mượn
    /// </summary>
    public bool? CheckAddHistoryProductRTC { get; set; }

    /// <summary>
    /// Ngày trả dự kiến
    /// </summary>
    public DateTime? ExpectedDate { get; set; }

    /// <summary>
    /// Tên dự án
    /// </summary>
    public string? ProjectName { get; set; }

    /// <summary>
    /// ID kho
    /// </summary>
    public int? WarehouseID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// ID nhà cung cấp
    /// </summary>
    public int? SupplierSaleID { get; set; }

    public int? BillDocumentExportType { get; set; }

    /// <summary>
    /// Người duyệt
    /// </summary>
    public int? ApproverID { get; set; }

    /// <summary>
    /// 1: Kho Demo; 2: Kho AGV
    /// </summary>
    public int? WarehouseTypeBill { get; set; }

    public bool? IsDeleted { get; set; }
}
