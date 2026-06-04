using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class InventoryProjectExport
{
    /// <summary>
    /// ID tồn kho xuất dự án
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// ID tồn kho giữ
    /// </summary>
    public int? InventoryProjectID { get; set; }

    /// <summary>
    /// ID chi tiết phiếu xuất
    /// </summary>
    public int? BillExportDetailID { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    /// <summary>
    /// số lượng
    /// </summary>
    public decimal? Quantity { get; set; }
}
