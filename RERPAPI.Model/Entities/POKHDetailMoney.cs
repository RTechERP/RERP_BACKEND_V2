using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class POKHDetailMoney
{
    public int ID { get; set; }

    public int? UserID { get; set; }

    public decimal? MoneyUser { get; set; }

    public int? POKHDetailID { get; set; }

    public decimal? PercentUser { get; set; }

    public int? RowHandle { get; set; }

    public int? STT { get; set; }

    public decimal? ReceiveMoney { get; set; }

    public int? Month { get; set; }

    public int? Year { get; set; }

    public int? POKHID { get; set; }

    public DateTime? CreatedDate { get; set; }
    public bool? IsDeleted { get; set; }
}
