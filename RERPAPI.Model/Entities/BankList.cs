using System;
using System.Collections.Generic;

namespace RERPAPI.Model.Entities;

public partial class BankList
{
    public int ID { get; set; }

    public int? STT { get; set; }

    public string? BankName { get; set; }
}
