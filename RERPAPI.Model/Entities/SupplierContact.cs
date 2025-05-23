using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class SupplierContact
{
    public int ID { get; set; }

    public int? SupplierID { get; set; }

    public string? ContactName { get; set; }

    public string? ContactPhone { get; set; }

    public string? ContactEmail { get; set; }

    /// <summary>
    /// Sản phẩm phụ trách
    /// </summary>
    public string? ProductSale { get; set; }

    public string? Manufactures { get; set; }

    public string? Description { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
