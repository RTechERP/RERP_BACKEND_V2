using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class BillExportDetailTechnical
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? BillExportTechID { get; set; }

    public int? UnitID { get; set; }

    public string? UnitName { get; set; }

    public int? ProjectID { get; set; }

    public int? ProductID { get; set; }

    public decimal? Quantity { get; set; }

    public decimal? TotalQuantity { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? Note { get; set; }

    public string? Internalcode { get; set; }

    public int? HistoryProductRTCID { get; set; }

    public int? ProductRTCQRCodeID { get; set; }

    public int? WarehouseID { get; set; }

    public int? BillImportDetailTechnicalID { get; set; }
}
