using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class AGVBillExport
{
    public int ID { get; set; }

    public string? BillCode { get; set; }

    /// <summary>
    /// 0. Trả ,1. Cho mượn,2. Tặng / Bán,3. Mất,4. Bảo hành,5. Xuất dự án,6. Hỏng,7. Xuất kho
    /// </summary>
    public int? BillType { get; set; }

    public DateTime? BillDate { get; set; }

    public int? CustomerID { get; set; }

    public string? Addres { get; set; }

    /// <summary>
    /// 1: duyệt; 0: hủy duyệt
    /// </summary>
    public int? Status { get; set; }

    public string? WarehouseType { get; set; }

    public string? Note { get; set; }

    public string? Image { get; set; }

    /// <summary>
    /// Người giao (EmployeeID)
    /// </summary>
    public int? EmployeeReceiverID { get; set; }

    /// <summary>
    /// Người nhận (EmployeeID)
    /// </summary>
    public int? EmployeeDeliverID { get; set; }

    public bool? CheckAddAGVHistoryProduct { get; set; }

    public DateTime? ExpectedDate { get; set; }

    public string? ProjectName { get; set; }

    public int? WarehouseID { get; set; }

    public int? SupplierSaleID { get; set; }

    public int? BillDocumentExportType { get; set; }

    public int? ApproverID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }
}
