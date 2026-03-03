using System;
using System.Collections.Generic;

namespace RERPAPI.Entities;

public partial class HistoryDeleteBill
{
    public int ID { get; set; }

    public int? BillID { get; set; }

    public int? UserID { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? DeleteDate { get; set; }

    public string? Name { get; set; }

    public string? TypeBill { get; set; }
}
