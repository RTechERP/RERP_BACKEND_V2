using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class BillExport
{
    public int ID { get; set; }

    /// <summary>
    /// Mã phiếu xuất
    /// </summary>
    public string? Code { get; set; }

    public bool? TypeBill { get; set; }

    /// <summary>
    /// ID nhà cung cấp
    /// </summary>
    public int? SupplierID { get; set; }

    /// <summary>
    /// ID khách hàng
    /// </summary>
    public int? CustomerID { get; set; }

    /// <summary>
    /// Người nhận
    /// </summary>
    public int? UserID { get; set; }

    /// <summary>
    /// Người giao
    /// </summary>
    public int? SenderID { get; set; }

    public int? StockID { get; set; }

    /// <summary>
    /// Mô tả
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Địa chỉ
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Ngày nhập
    /// </summary>
    public DateTime? CreatDate { get; set; }

    /// <summary>
    /// Duyệt
    /// </summary>
    public bool? IsApproved { get; set; }

    /// <summary>
    /// Trạng thái phiếu: 0. Mượn
    /// 1. Tồn kho
    /// 2. Đã xuất kho
    /// 3. Chia trước
    /// 4. Phiếu mượn nội bộ
    /// 5. Xuất trả NCC
    /// 6. Yêu cầu xuất kho
    /// </summary>
    public int? Status { get; set; }

    public string? GroupID { get; set; }

    /// <summary>
    /// loại kho
    /// </summary>
    public string? WarehouseType { get; set; }

    /// <summary>
    /// Loại kho nội bộ
    /// </summary>
    public int? KhoTypeID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int? ProductType { get; set; }

    public int? AddressStockID { get; set; }

    public bool? IsMerge { get; set; }

    public int? UnApprove { get; set; }

    /// <summary>
    /// ID kho 
    /// </summary>
    public int? WarehouseID { get; set; }

    public bool? IsPrepared { get; set; }

    public bool? IsReceived { get; set; }

    /// <summary>
    /// Ngày yêu cầu xuất
    /// </summary>
    public DateTime? RequestDate { get; set; }

    public DateTime? PreparedDate { get; set; }

    public int? BillDocumentExportType { get; set; }

    public bool? IsDeleted { get; set; }

    public int? BillImportID { get; set; }

    /// <summary>
    /// ID kho chuyển
    /// </summary>
    public int? WareHouseTranferID { get; set; }

    /// <summary>
    /// Chuyển kho
    /// </summary>
    public bool? IsTransfer { get; set; }
}
