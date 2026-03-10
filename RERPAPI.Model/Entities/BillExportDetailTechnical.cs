using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class BillExportDetailTechnical
{
    public int ID { get; set; }

    /// <summary>
    /// Số thứ tự
    /// </summary>
    public int? STT { get; set; }

    /// <summary>
    /// ID master
    /// </summary>
    public int? BillExportTechID { get; set; }

    /// <summary>
    /// ID đơn vị tính
    /// </summary>
    public int? UnitID { get; set; }

    /// <summary>
    /// Tên đơn vị tính
    /// </summary>
    public string? UnitName { get; set; }

    /// <summary>
    /// ID dự án
    /// </summary>
    public int? ProjectID { get; set; }

    /// <summary>
    /// ID sản phẩm
    /// </summary>
    public int? ProductID { get; set; }

    /// <summary>
    /// Số lượng
    /// </summary>
    public decimal? Quantity { get; set; }

    /// <summary>
    /// Tổng số lượng
    /// </summary>
    public decimal? TotalQuantity { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Note { get; set; }

    public string? Internalcode { get; set; }

    /// <summary>
    /// ID lịch sử mượn
    /// </summary>
    public int? HistoryProductRTCID { get; set; }

    /// <summary>
    /// ID QR code
    /// </summary>
    public int? ProductRTCQRCodeID { get; set; }

    /// <summary>
    /// ID kho
    /// </summary>
    public int? WarehouseID { get; set; }

    public int? BillImportDetailTechnicalID { get; set; }

    public bool? IsDeleted { get; set; }
}
