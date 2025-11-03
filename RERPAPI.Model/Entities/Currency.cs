using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class Currency
{
    public int ID { get; set; }

    public string? Code { get; set; }

    public string? NameEnglist { get; set; }

    public string? NameVietNamese { get; set; }

    public string? MinUnit { get; set; }

    public decimal? CurrencyRate { get; set; }

    public string? Note { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public DateTime? DateExpried { get; set; }

    public DateTime? DateStart { get; set; }

    public decimal? CurrencyRateOfficialQuota { get; set; }

    public decimal? CurrencyRateUnofficialQuota { get; set; }

    public DateTime? DateExpriedOfficialQuota { get; set; }

    public DateTime? DateExpriedUnofficialQuota { get; set; }

    public bool? IsDeleted { get; set; }
}
