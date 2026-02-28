using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class DailyReportSale
{
    public int ID { get; set; }

    public int? CustomerID { get; set; }

    public int? UserID { get; set; }

    public int? ContacID { get; set; }

    public DateTime? DateEnd { get; set; }

    public string? Telesale { get; set; }

    public string? Visit { get; set; }

    public string? Demo { get; set; }

    public string? Result { get; set; }

    public string? ProblemBacklog { get; set; }

    public string? PlanNext { get; set; }

    public string? Note { get; set; }

    public bool? BigAccount { get; set; }

    public int? GroupType { get; set; }

    public string? Content { get; set; }

    public int? UserLoginID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public int? Month { get; set; }

    public int? Year { get; set; }

    public int? EndUser { get; set; }

    public DateTime? DateStart { get; set; }

    public int? DeleteFlag { get; set; }

    public bool? Confirm { get; set; }

    public string? ProductOfCustomer { get; set; }

    public string? RequestOfCustomer { get; set; }

    public int? ProjectID { get; set; }

    public int? FirmBaseID { get; set; }

    public int? ProjectTypeBaseID { get; set; }

    public bool? SaleOpportunity { get; set; }

    public int? WarehouseID { get; set; }
}
