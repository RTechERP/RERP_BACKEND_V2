using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class ConfigPrice
{
    public int ID { get; set; }

    public decimal? Exchange { get; set; }

    public decimal? BankCharges { get; set; }

    public decimal? NumberOfTransactions { get; set; }

    public decimal? CustomFees { get; set; }

    public decimal? Declaration { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public decimal? TransportFee { get; set; }
}
