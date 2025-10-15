using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class BillExportTechnical
{
    public int ID { get; set; }

    public string? Code { get; set; }

    /// <summary>
    /// 0. Trả
    /// 1. Cho mượn
    /// 2. Tặng / Bán
    /// 3. Mất
    /// 4. Bảo hành
    /// 5. Xuất dự án
    /// 6. Hỏng
    /// 7. Xuất kho
    /// </summary>
    public int? BillType { get; set; }

    public int? CustomerID { get; set; }

    public string? Receiver { get; set; }

    public string? Deliver { get; set; }

    public string? Addres { get; set; }

    public int? Status { get; set; }

    public string? WarehouseType { get; set; }

    public string? Note { get; set; }

    public string? Image { get; set; }

    public int? ReceiverID { get; set; }

    public int? DeliverID { get; set; }

    public int? SupplierID { get; set; }

    public string? CustomerName { get; set; }

    public string? SupplierName { get; set; }

    public bool? CheckAddHistoryProductRTC { get; set; }

    public DateTime? ExpectedDate { get; set; }

    public string? ProjectName { get; set; }

    public int? WarehouseID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? SupplierSaleID { get; set; }

    public int? BillDocumentExportType { get; set; }

    public int? ApproverID { get; set; }
    public bool? IsDeleted { get; set; }

}
