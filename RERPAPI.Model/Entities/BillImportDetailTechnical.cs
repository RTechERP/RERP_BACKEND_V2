using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class BillImportDetailTechnical
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public int? BillImportTechID { get; set; }

    public int? ProductID { get; set; }

    public decimal? Quantity { get; set; }

    public decimal? TotalQuantity { get; set; }

    public decimal? Price { get; set; }

    public decimal? TotalPrice { get; set; }

    public int? UnitID { get; set; }

    public string? UnitName { get; set; }

    public int? ProjectID { get; set; }

    public string? ProjectCode { get; set; }

    public string? ProjectName { get; set; }

    public string? SomeBill { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? Note { get; set; }

    public string? InternalCode { get; set; }

    public int? HistoryProductRTCID { get; set; }

    public int? ProductRTCQRCodeID { get; set; }

    public int? WarehouseID { get; set; }

    public int? IsBorrowSupplier { get; set; }

    public decimal? QtyRequest { get; set; }

    public int? PONCCDetailID { get; set; }

    public string? BillCodePO { get; set; }

    public int? EmployeeIDBorrow { get; set; }

    public DateTime? DeadlineReturnNCC { get; set; }

    public DateTime? DateSomeBill { get; set; }

    public decimal? COFormE { get; set; }

    public decimal? TaxReduction { get; set; }

    public DateTime? DueDate { get; set; }

    public int? DPO { get; set; }

    public bool? IsDeleted { get; set; }
}
