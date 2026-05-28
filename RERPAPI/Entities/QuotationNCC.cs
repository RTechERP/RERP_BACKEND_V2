using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class QuotationNCC
{
    public int ID { get; set; }

    public string? GroupID { get; set; }

    public int? SupplierID { get; set; }

    public bool? IsApproved { get; set; }

    public string? QuoteCode { get; set; }

    public string? UserNCC { get; set; }

    public string? UserName { get; set; }

    public string? Phone { get; set; }

    public int? TotalMoney { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? QuoteDate { get; set; }

    public int? ProjectID { get; set; }

    public int? UserID { get; set; }

    public string? ContactName { get; set; }

    public int? Status { get; set; }

    public int? ContactID { get; set; }
}
