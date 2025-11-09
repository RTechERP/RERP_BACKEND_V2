using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class SupplierSaleContact
{
    public int ID { get; set; }

    public int SupplierID { get; set; }

    public string? SupplierName { get; set; }

    public string? SupplierPhone { get; set; }

    public string? SupplierEmail { get; set; }

    public string? Describe { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
