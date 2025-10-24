using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class HistoryMoneyPO
{
    public int ID { get; set; }

    public int? Number { get; set; }

    public decimal? Money { get; set; }

    public DateTime? MoneyDate { get; set; }

    public int? POKHID { get; set; }

    public string? Note { get; set; }

    public int? ProjectID { get; set; }

    public int? ProductID { get; set; }

    public string? InvoiceNo { get; set; }

    public string? BankName { get; set; }

    public decimal? MoneyVAT { get; set; }

    public decimal? VAT { get; set; }

    public int? Type { get; set; }

    public int? POKHDetailID { get; set; }

    public bool? IsFilm { get; set; }

    public bool? IsMergePO { get; set; }

    public bool? IsDelivered { get; set; }
}
