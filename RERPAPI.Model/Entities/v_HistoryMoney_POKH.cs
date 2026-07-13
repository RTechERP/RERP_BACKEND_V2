using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class v_HistoryMoney_POKH
{
    public int ID { get; set; }

    public decimal? Money { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? POType { get; set; }

    public int? UserID { get; set; }

    public int POKHID { get; set; }

    public int? AccountType { get; set; }

    public decimal? MoneyVAT { get; set; }

    public DateTime? MoneyDate { get; set; }

    public decimal? VAT { get; set; }

    public int? HistoryMoneyUserID { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? ReceivedDatePO { get; set; }

    public int? PMUserID { get; set; }
}
