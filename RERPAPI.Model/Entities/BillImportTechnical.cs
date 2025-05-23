using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class BillImportTechnical
{
    public int ID { get; set; }

    public string? BillCode { get; set; }

    public DateTime? CreatDate { get; set; }

    public string? Deliver { get; set; }

    public string? Receiver { get; set; }

    public bool? Status { get; set; }

    public string? Suplier { get; set; }

    public bool? BillType { get; set; }

    public string? WarehouseType { get; set; }

    public int? DeliverID { get; set; }

    public int? ReceiverID { get; set; }

    public int? SuplierID { get; set; }

    public int? GroupTypeID { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? Image { get; set; }

    public int? WarehouseID { get; set; }

    public int? SupplierSaleID { get; set; }

    public int? BillTypeNew { get; set; }

    public int? IsBorrowSupplier { get; set; }

    public int? CustomerID { get; set; }

    public int? BillDocumentImportType { get; set; }

    public DateTime? DateRequestImport { get; set; }

    public int? RulePayID { get; set; }

    public bool? IsNormalize { get; set; }

    public int? ApproverID { get; set; }
}
