using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class SALE
{
    public int ID { get; set; }

    public string? POCustomer { get; set; }

    public int? CustomerID { get; set; }

    public decimal? Sale1 { get; set; }

    public DateTime? SaleDate { get; set; }

    public int? UserID { get; set; }

    public int? Type { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public int? Month { get; set; }

    public int? Year { get; set; }
}
