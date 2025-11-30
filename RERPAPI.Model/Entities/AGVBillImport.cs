using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class AGVBillImport
{
    public int ID { get; set; }

    public string? BillCode { get; set; }

    public DateTime? BillDate { get; set; }

    /// <summary>
    /// 1: Duyệt; 0: Chờ duyệt
    /// </summary>
    public bool? Status { get; set; }

    /// <summary>
    /// Loại phiếu (1: Mượn NCC; 2: Mua NCC; 3: Trả; 4: Nhập nội bộ; 5: Y/c nhập kho; 6: Nhập hàng bảo hành; 7: NCC tặng/cho)
    /// </summary>
    public int? BillType { get; set; }

    public string? WarehouseType { get; set; }

    /// <summary>
    /// Người giao (EmployeeID)
    /// </summary>
    public int? EmployeeDeliverID { get; set; }

    /// <summary>
    /// Người nhận (EmployeeID)
    /// </summary>
    public int? EmployeeReceiverID { get; set; }

    public string? Image { get; set; }

    public int? WarehouseID { get; set; }

    public int? SupplierSaleID { get; set; }

    public int? IsBorrowSupplier { get; set; }

    public int? CustomerID { get; set; }

    public int? BillDocumentImportType { get; set; }

    public DateTime? DateRequestImport { get; set; }

    public int? RulePayID { get; set; }

    public bool? IsNormalize { get; set; }

    public int? ApproverID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
