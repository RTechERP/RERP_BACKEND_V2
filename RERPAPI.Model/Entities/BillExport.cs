using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class BillExport
{
    public int ID { get; set; }

    public string? Code { get; set; }

    public bool? TypeBill { get; set; }

    public int? SupplierID { get; set; }

    public int? CustomerID { get; set; }

    public int? UserID { get; set; }

    public int? SenderID { get; set; }

    public int? StockID { get; set; }

    public string? Description { get; set; }

    public string? Address { get; set; }

    public DateTime? CreatDate { get; set; }

    public bool? IsApproved { get; set; }

    /// <summary>
    /// 0. Mượn
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

    public int? KhoTypeID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int? ProductType { get; set; }

    public int? AddressStockID { get; set; }

    public bool? IsMerge { get; set; }

    public int? UnApprove { get; set; }

    public int? WarehouseID { get; set; }

    public bool? IsPrepared { get; set; }

    public bool? IsReceived { get; set; }

    public DateTime? RequestDate { get; set; }

    public DateTime? PreparedDate { get; set; }

    public int? BillDocumentExportType { get; set; }

    public bool? IsDeleted { get; set; }

    public bool? BillImportID { get; set; }

    public int? WareHouseTranferID { get; set; }

    public bool? IsTransfer { get; set; }
}
