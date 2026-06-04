using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class AGVBillExportDetail
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? AGVBillExportID { get; set; }

    public int? UnitID { get; set; }

    public string? UnitName { get; set; }

    public int? ProjectID { get; set; }

    public int? AGVProductID { get; set; }

    public decimal? Quantity { get; set; }

    public string? Note { get; set; }

    public int? AGVHistoryProductID { get; set; }

    public int? AGVProductQRCodeID { get; set; }

    public int? AGVBillImportDetailID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }
}
