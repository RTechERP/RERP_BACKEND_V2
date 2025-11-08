using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class BillImportQCDetail
{
    public int ID { get; set; }

    public int? BillImportQCID { get; set; }

    public int? ProductSaleID { get; set; }

    public int? LeaderTechID { get; set; }

    /// <summary>
    /// 1.OK 2.NG
    /// </summary>
    public int? Status { get; set; }

    public int? EmployeeTechID { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int? BillImportDetailID { get; set; }

    public bool? IsDeleted { get; set; }

    public int? ProjectID { get; set; }

    public string? POKHCode { get; set; }

    public decimal? Quantity { get; set; }
}
