using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ProductSale
{
    public int ID { get; set; }

    public string? ProductCode { get; set; }

    public string? ProductName { get; set; }

    public string? Maker { get; set; }

    public string? Unit { get; set; }

    public decimal? NumberInStoreDauky { get; set; }

    public decimal? Import { get; set; }

    public decimal? Export { get; set; }

    public decimal? NumberInStoreCuoiKy { get; set; }

    public string? AddressBox { get; set; }

    public string? Note { get; set; }

    public int? ProductGroupID { get; set; }

    public string? ItemType { get; set; }

    public int? LocationID { get; set; }

    public string? SupplierName { get; set; }

    public string? ProductNewCode { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public int? FirmID { get; set; }

    public bool? IsDeleted { get; set; }
}
